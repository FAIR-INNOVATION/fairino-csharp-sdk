using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;

using BcX509 = Org.BouncyCastle.X509.X509Certificate;
using BcCertReq = Org.BouncyCastle.Crypto.Tls.CertificateRequest;
using X509CertificateStructure = Org.BouncyCastle.Asn1.X509.X509CertificateStructure;

namespace fairino
{
    /// <summary>
    /// mTLS/DTLS 通道模块（SDK 版，与机器人端 mtls_link.c 配套）。
    ///   TCP 8080 : SslStream 双向认证（客户端证书 + 私有 CA 信任链校验，证书不绑 IP）
    ///   UDP 20007: DTLS 1.2（BouncyCastle，双向证书认证，仅校验 CA 链）
    /// 开关 = 证书文件：运行目录 certs/ 下存在 client.crt / client.key / ca.crt 则启用；
    /// 否则 Enabled=false，SDK 全部走原有明文路径（与旧版本一致）。
    /// </summary>
    public class MtlsLink : IDisposable
    {
        /// <summary>证书目录（相对工作目录，默认 certs/）</summary>
        public string CertDir { get; set; }

        /// <summary>是否启用（certs 目录证书齐全时自动为 true）</summary>
        public bool Enabled { get; private set; }

        private readonly object dtlsSendLock = new object();
        private DtlsTransport dtlsTransport;

        public MtlsLink(string certDir = null)
        {
            if (string.IsNullOrEmpty(certDir))
            {
                /* 默认证书目录 = SDK dll 所在目录下的 certs/：
                 * 与 dll 一起部署，不依赖程序工作目录，也不受
                 * 编译输出目录 Clean 清理影响 */
                try
                {
                    certDir = Path.Combine(
                        Path.GetDirectoryName(typeof(MtlsLink).Assembly.Location), "certs");
                }
                catch
                {
                    certDir = "certs";
                }
            }
            CertDir = certDir;
            Enabled = MissingCerts.Length == 0;
            if (Enabled)
                PrintClientCertInfo();
        }

        /// <summary>缺失的证书文件名（Enabled=false 时有意义），用于启动时报错提示</summary>
        public string MissingCerts
        {
            get
            {
                string miss = "";
                if (!File.Exists(Path.Combine(CertDir, "client.crt"))) miss += "client.crt ";
                if (!File.Exists(Path.Combine(CertDir, "client.key"))) miss += "client.key ";
                if (!File.Exists(Path.Combine(CertDir, "ca.crt"))) miss += "ca.crt ";
                return miss.Trim();
            }
        }

        /// <summary>打印当前 client 证书信息：证书目录 + 序列号 + 到期时间 + 名称（CN）</summary>
        private void PrintClientCertInfo()
        {
            try
            {
                Console.WriteLine($"[MtlsLink] cert dir: {CertDir}");
                var cert = new X509Certificate2(Path.Combine(CertDir, "client.crt"));
                long serial = Convert.ToInt64(cert.SerialNumber, 16);
                int daysLeft = (int)(cert.NotAfter - DateTime.Now).TotalDays;
                Console.WriteLine($"[MtlsLink] client cert: CN={cert.GetNameInfo(X509NameType.SimpleName, false)}, " +
                                  $"serial={serial}, expires={cert.NotAfter:yyyy-MM-dd HH:mm:ss} ({daysLeft} days left)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MtlsLink] client cert info read failed: {ex.Message}");
            }
        }

        /* ================= TCP 8080：SslStream 双向认证 ================= */

        /// <summary>对已连接的 TCP socket 做 mTLS 握手，返回可收发的 SslStream。</summary>
        public SslStream WrapTcp(Socket socket, string targetIp)
        {
            var netStream = new NetworkStream(socket, false);
            var ssl = new SslStream(netStream, false,
                (sender, cert, chain, errors) => VerifyServer(cert));
            /* 握手超时 2s：机器人端未开启加密时不会回 ServerHello，
             * 无超时会无限等待（卡在建联）；超时后抛异常由上层报错 */
            int oldTimeout = socket.ReceiveTimeout;
            socket.ReceiveTimeout = 2000;
            try
            {
                ssl.AuthenticateAsClient(targetIp, LoadClientCertificate(),
                                         SslProtocols.Tls12, false);
            }
            finally
            {
                socket.ReceiveTimeout = oldTimeout;
            }
            return ssl;
        }

        private string CaPath { get { return Path.Combine(CertDir, "ca.crt"); } }

        private X509Certificate2 LoadCaCert()
        {
            return new X509Certificate2(CaPath);
        }

        /// <summary>BouncyCastle 解析 PEM 私钥（.NET Framework 不支持 PEM 私钥导入）</summary>
        private RsaPrivateCrtKeyParameters LoadClientKeyParams()
        {
            var r = new PemReader(File.OpenText(Path.Combine(CertDir, "client.key")));
            object obj = r.ReadObject();
            return obj as RsaPrivateCrtKeyParameters
                   ?? ((AsymmetricCipherKeyPair)obj).Private as RsaPrivateCrtKeyParameters;
        }

        /// <summary>组装带私钥的客户端证书。用 BouncyCastle 走 PKCS#12 往返：
        /// X509Certificate2.CopyWithPrivateKey 只有 .NET 4.7.2+ 才有，4.5.1/4.6.1/4.7.1 编译不过；
        /// PFX 往返与原来的 Export(Pkcs12) 等价，各框架行为一致</summary>
        private X509Certificate2Collection LoadClientCertificate()
        {
            var store = new Pkcs12Store();
            BcX509 bcCert = new Org.BouncyCastle.X509.X509CertificateParser()
                .ReadCertificate(File.ReadAllBytes(Path.Combine(CertDir, "client.crt")));
            var entry = new X509CertificateEntry(bcCert);
            store.SetCertificateEntry("client", entry);
            store.SetKeyEntry("client", new AsymmetricKeyEntry(LoadClientKeyParams()),
                              new X509CertificateEntry[] { entry });

            const string pwd = "fairino";   /* 内存中一次性使用，不落盘 */
            using (var ms = new MemoryStream())
            {
                store.Save(ms, pwd.ToCharArray(), new SecureRandom());
                return new X509Certificate2Collection(new X509Certificate2(ms.ToArray(), pwd));
            }
        }

        /// <summary>私有 CA 信任链校验：server 证书必须链到本地 ca.crt（.NET Framework 经典做法）。
        /// 证书不绑 IP：机器人换 IP 无需重新签发，SDK 侧也只验证"对方持有本 CA 签发的证书"。</summary>
        private bool VerifyServer(X509Certificate cert)
        {
            /* 不用 using：X509Chain 在 .NET 4.7.2 以下未实现 IDisposable */
            var c2 = new X509Chain();
            try
            {
                var ca = LoadCaCert();
                c2.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                c2.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                c2.ChainPolicy.ExtraStore.Add(ca);
                if (!c2.Build(new X509Certificate2(cert)))
                    return false;
                X509Certificate2 root = c2.ChainElements[c2.ChainElements.Count - 1].Certificate;
                return root.Thumbprint.Equals(ca.Thumbprint, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                c2.Reset();
            }
        }

        /* ================= UDP 20007：BouncyCastle DTLS 1.2 ================= */

        private class UdpDatagramTransport : DatagramTransport
        {
            private readonly Socket sock;
            public UdpDatagramTransport(Socket s) { sock = s; }
            public int GetReceiveLimit() { return 1500; }
            public int GetSendLimit() { return 1500; }
            public void Send(byte[] buf, int off, int len)
            {
                sock.Send(buf, off, len, SocketFlags.None);
            }
            public int Receive(byte[] buf, int off, int len, int waitMillis)
            {
                sock.ReceiveTimeout = waitMillis;
                try
                {
                    return sock.Receive(buf, off, len, SocketFlags.None);
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    /* 超时是 DTLS 的正常等待：返回 -1 而不是抛异常，
                     * 否则空闲期每 2 秒抛一次异常，调试时刷屏 */
                    return -1;
                }
            }
            public void Close()
            {
                /* 注意：不关闭 socket——socket 归 FRUdpClient 所有，
                 * 握手失败回退明文后还要继续用。BC 在握手失败时会调用本方法。 */
            }
        }

        private class MtlsAuthentication : TlsAuthentication
        {
            private readonly TlsContext ctx;
            private readonly string certDir;
            public MtlsAuthentication(TlsContext context, string dir) { ctx = context; certDir = dir; }

            public void NotifyServerCertificate(Certificate serverCertificate)
            {
                BcX509 cert = new BcX509(serverCertificate.GetCertificateAt(0));
                if (!cert.IsValidNow)
                    throw new TlsFatalAlert(AlertDescription.bad_certificate);
                BcX509 ca = LoadBcCert(Path.Combine(certDir, "ca.crt"));
                try { cert.Verify(ca.GetPublicKey()); }
                catch (Exception) { throw new TlsFatalAlert(AlertDescription.bad_certificate); }
            }

            public TlsCredentials GetClientCredentials(BcCertReq certificateRequest)
            {
                BcX509 cert = LoadBcCert(Path.Combine(certDir, "client.crt"));
                AsymmetricKeyParameter key = LoadBcKey(Path.Combine(certDir, "client.key"));
                var sigAlg = new SignatureAndHashAlgorithm(
                    Org.BouncyCastle.Crypto.Tls.HashAlgorithm.sha256, SignatureAlgorithm.rsa);
                return new DefaultTlsSignerCredentials(ctx,
                    new Certificate(new X509CertificateStructure[] { cert.CertificateStructure }),
                    key, sigAlg);
            }
        }

        private class MtlsDtlsClient : DefaultTlsClient
        {
            private readonly string certDir;
            public MtlsDtlsClient(string dir) { certDir = dir; }
            public override ProtocolVersion ClientVersion { get { return ProtocolVersion.DTLSv12; } }
            public override ProtocolVersion MinimumVersion { get { return ProtocolVersion.DTLSv12; } }
            public override TlsAuthentication GetAuthentication()
            {
                return new MtlsAuthentication(mContext, certDir);
            }
            public override int[] GetCipherSuites()
            {
                return new int[] { CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256 };
            }
        }

        private static BcX509 LoadBcCert(string path)
        {
            var r = new PemReader(File.OpenText(path));
            return (BcX509)r.ReadObject();
        }

        private static AsymmetricKeyParameter LoadBcKey(string path)
        {
            var r = new PemReader(File.OpenText(path));
            object obj = r.ReadObject();
            return obj as AsymmetricKeyParameter ?? ((AsymmetricCipherKeyPair)obj).Private;
        }

        /// <summary>对已 bind 的 UDP socket 发起 DTLS 握手（socket 需已 Connect 到对端）。</summary>
        public void StartDtls(Socket udpSocket)
        {
            var protocol = new DtlsClientProtocol(new SecureRandom());
            dtlsTransport = protocol.Connect(new MtlsDtlsClient(CertDir),
                                             new UdpDatagramTransport(udpSocket));
        }

        /// <summary>DTLS 发送（线程安全）。</summary>
        public void DtlsSend(byte[] buf)
        {
            lock (dtlsSendLock)
            {
                dtlsTransport.Send(buf, 0, buf.Length);
            }
        }

        /// <summary>DTLS 接收（仅接收线程调用）。返回实际长度；<=0 表示超时/失败。</summary>
        public int DtlsReceive(byte[] buf, int off, int len, int timeoutMs)
        {
            return dtlsTransport.Receive(buf, off, len, timeoutMs);
        }

        public bool DtlsActive { get { return dtlsTransport != null; } }

        public void Dispose()
        {
            if (dtlsTransport != null)
            {
                try { dtlsTransport.Close(); } catch { }
                dtlsTransport = null;
            }
        }
    }
}
