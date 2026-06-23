"""Re-add methods lost by git checkout"""
import sys

file = r'd:\C#SDK\各版本sdk\fairino-csharp_-sdk-3.9.7\src\FRRobot\FRRobot.cs'
with open(file, 'rb') as f:
    content = f.read()

# Find insertion point: after FT_ComplianceStop's closing brace, before LoadIdentifyDynFilterInit
marker = b'        /**\r\n         * @brief \xe8\xb4\x9f\xe8\xbd\xbd\xe8\xbe\xa8\xe8\xaf\x86\xe5\x88\x9d\xe5\xa7\x8b\xe5\x8c\x96\r\n         * @return \xe9\x94\x99\xe8\xaf\xaf\xe7\xa0\x81\r\n         */\r\n        public int LoadIdentifyDynFilterInit()'
pos = content.find(marker)
if pos == -1:
    print('ERROR: marker not found')
    sys.exit(1)

new_methods = b'''        /**
        * @brief  \xe6\x97\x8b\xe8\xbd\xac\xe6\x8f\x92\xe5\x85\xa5
        * @param [in] rcs \xe5\x8f\x82\xe8\x80\x83\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb\xef\xbc\x8c0-\xe5\xb7\xa5\xe5\x85\xb7\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb\xef\xbc\x8c1-\xe5\x9f\xba\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb
        * @param [in] angVelRot \xe6\x97\x8b\xe8\xbd\xac\xe8\xa7\x92\xe9\x80\x9f\xe5\xba\xa6\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8ddeg/s
        * @param [in] ft  \xe5\x8a\x9b/\xe6\x89\xad\xe7\x9f\xa9\xe9\x98\x88\xe5\x80\xbc
        * @param [in] max_angle \xe6\x9c\x80\xe5\xa4\xa7\xe6\x97\x8b\xe8\xbd\xac\xe8\xa7\x92\xe5\xba\xa6\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8ddeg
        * @param [in] orn \xe5\x8a\x9b/\xe6\x89\xad\xe7\x9f\xa9\xe6\x96\xb9\xe5\x90\x91\xef\xbc\x8c1-\xe6\xb2\xbfz\xe8\xbd\xb4\xe6\x96\xb9\xe5\x90\x91\xef\xbc\x8c2-\xe7\xbb\x95z\xe8\xbd\xb4\xe6\x96\xb9\xe5\x90\x91
        * @param [in] max_angAcc \xe6\x9c\x80\xe5\xa4\xa7\xe6\x97\x8b\xe8\xbd\xac\xe5\x8a\xa0\xe9\x80\x9f\xe5\xba\xa6\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8ddeg/s^2\xef\xbc\x8c\xe6\x9a\x82\xe4\xb8\x8d\xe4\xbd\xbf\xe7\x94\xa8\xef\xbc\x8c\xe9\xbb\x98\xe8\xae\xa4\xe4\xb8\xba0
        * @param [in] rotorn  \xe6\x97\x8b\xe8\xbd\xac\xe6\x96\xb9\xe5\x90\x91\xef\xbc\x8c1-\xe9\xa1\xba\xe6\x97\xb6\xe9\x92\x88\xef\xbc\x8c2-\xe9\x80\x86\xe6\x97\xb6\xe9\x92\x88
        * @param [in] strategy \xe6\x9c\xaa\xe6\xa3\x80\xe6\xb5\x8b\xe5\x88\xb0\xe5\x8a\x9b/\xe5\x8a\x9b\xe7\x9f\xa9\xe7\x9a\x84\xe5\xa4\x84\xe7\x90\x86\xe7\xad\x96\xe7\x95\xa5\xef\xbc\x8c0-\xe6\x8a\xa5\xe9\x94\x99\xef\xbc\x9b1-\xe8\xad\xa6\xe5\x91\x8a\xef\xbc\x8c\xe7\xbb\xa7\xe7\xbb\xad\xe8\xbf\x90\xe5\x8a\xa8
        * @return  \xe9\x94\x99\xe8\xaf\xaf\xe7\xa0\x81
        */
        public int FT_RotInsertion(int rcs, double angVelRot, double ft, double max_angle, int orn, double max_angAcc, int rotorn, int strategy)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }

            try
            {
                int rtn = proxy.FT_RotInsertion(rcs, angVelRot, ft, max_angle, orn, max_angAcc, rotorn, strategy);

                if (log != null)
                {
                    log.LogInfo($"FT_RotInsertion: {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief  \xe7\x9b\xb4\xe7\xba\xbf\xe6\x8f\x92\xe5\x85\xa5
        * @param  [in] rcs \xe5\x8f\x82\xe8\x80\x83\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb\xef\xbc\x8c0-\xe5\xb7\xa5\xe5\x85\xb7\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb\xef\xbc\x8c1-\xe5\x9f\xba\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb
        * @param  [in] ft  \xe5\x8a\x9b/\xe6\x89\xad\xe7\x9f\xa9\xe9\x98\x88\xe5\x80\xbc\xef\xbc\x8cfx,fy,fz,tx,ty,tz\xef\xbc\x8c\xe8\x8c\x83\xe5\x9b\xb4[0~100]
        * @param  [in] lin_v \xe7\x9b\xb4\xe7\xba\xbf\xe9\x80\x9f\xe5\xba\xa6\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8dmm/s
        * @param  [in] lin_a \xe7\x9b\xb4\xe7\xba\xbf\xe5\x8a\xa0\xe9\x80\x9f\xe5\xba\xa6\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8dmm/s^2\xef\xbc\x8c\xe6\x9a\x82\xe4\xb8\x8d\xe4\xbd\xbf\xe7\x94\xa8
        * @param  [in] max_dis \xe6\x9c\x80\xe5\xa4\xa7\xe6\x8f\x92\xe5\x85\xa5\xe8\xb7\x9d\xe7\xa6\xbb\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8dmm
        * @param  [in] linorn  \xe6\x8f\x92\xe5\x85\xa5\xe6\x96\xb9\xe5\x90\x91\xef\xbc\x8c0-\xe8\xb4\x9f\xe6\x96\xb9\xe5\x90\x91\xef\xbc\x8c1-\xe6\xad\xa3\xe6\x96\xb9\xe5\x90\x91
        * @return  \xe9\x94\x99\xe8\xaf\xaf\xe7\xa0\x81
        */
        public int FT_LinInsertion(int rcs, float ft, float lin_v, float lin_a, float max_dis, byte linorn)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }

            try
            {
                int rtn = proxy.FT_LinInsertion(rcs, ft, lin_v, lin_a, max_dis, linorn);

                if (log != null)
                {
                    log.LogInfo($"FT_LinInsertion: {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief  \xe8\xa1\xa8\xe9\x9d\xa2\xe5\xae\x9a\xe4\xbd\x8d
        * @param  [in] rcs \xe5\x8f\x82\xe8\x80\x83\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb\xef\xbc\x8c0-\xe5\xb7\xa5\xe5\x85\xb7\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb\xef\xbc\x8c1-\xe5\x9f\xba\xe5\x9d\x90\xe6\xa0\x87\xe7\xb3\xbb
        * @param  [in] dir  \xe7\xa7\xbb\xe5\x8a\xa8\xe6\x96\xb9\xe5\x90\x91\xef\xbc\x8c1-\xe6\xad\xa3\xe6\x96\xb9\xe5\x90\x91\xef\xbc\x8c2-\xe8\xb4\x9f\xe6\x96\xb9\xe5\x90\x91
        * @param  [in] axis \xe7\xa7\xbb\xe5\x8a\xa8\xe8\xbd\xb4\xef\xbc\x8c1-x\xe8\xbd\xb4\xef\xbc\x8c2-y\xe8\xbd\xb4\xef\xbc\x8c3-z\xe8\xbd\xb4
        * @param  [in] lin_v \xe6\x8e\xa2\xe7\xb4\xa2\xe7\x9b\xb4\xe7\xba\xbf\xe9\x80\x9f\xe5\xba\xa6\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8dmm/s
        * @param  [in] lin_a \xe6\x8e\xa2\xe7\xb4\xa2\xe7\x9b\xb4\xe7\xba\xbf\xe5\x8a\xa0\xe9\x80\x9f\xe5\xba\xa6\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8dmm/s^2\xef\xbc\x8c\xe6\x9a\x82\xe4\xb8\x8d\xe4\xbd\xbf\xe7\x94\xa8\xef\xbc\x8c\xe9\xbb\x98\xe8\xae\xa4\xe4\xb8\xba0
        * @param  [in] max_dis \xe6\x9c\x80\xe5\xa4\xa7\xe6\x8e\xa2\xe7\xb4\xa2\xe8\xb7\x9d\xe7\xa6\xbb\xef\xbc\x8c\xe5\x8d\x95\xe4\xbd\x8dmm
        * @param  [in] ft  \xe5\x8a\xa8\xe4\xbd\x9c\xe7\xbb\x88\xe6\xad\xa2\xe5\x8a\x9b/\xe6\x89\xad\xe7\x9f\xa9\xe9\x98\x88\xe5\x80\xbc\xef\xbc\x8cfx,fy,fz,tx,ty,tz
        * @return  \xe9\x94\x99\xe8\xaf\xaf\xe7\xa0\x81
        */
        public int FT_FindSurface(int rcs, byte dir, byte axis, float lin_v, float lin_a, float max_dis, float ft)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }

            try
            {
                int rtn = proxy.FT_FindSurface(rcs, dir, axis, lin_v, lin_a, max_dis, ft);

                if (log != null)
                {
                    log.LogInfo($"FT_FindSurface: {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief  \xe8\xae\xa1\xe7\xae\x97\xe4\xb8\xad\xe9\x97\xb4\xe5\xb9\xb3\xe9\x9d\xa2\xe4\xbd\x8d\xe7\xbd\xae\xe5\xbc\x80\xe5\xa7\x8b
        * @return  \xe9\x94\x99\xe8\xaf\xaf\xe7\xa0\x81
        */
        public int FT_CalCenterStart()
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            if (GetSafetyCode() != 0)
            {
                return GetSafetyCode();
            }

            try
            {
                int rtn = proxy.FT_CalCenterStart();

                if (log != null)
                {
                    log.LogInfo($"FT_CalCenterStart: {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief  \xe8\xae\xa1\xe7\xae\x97\xe4\xb8\xad\xe9\x97\xb4\xe5\xb9\xb3\xe9\x9d\xa2\xe4\xbd\x8d\xe7\xbd\xae\xe7\xbb\x93\xe6\x9d\x9f
        * @param  [out] pos \xe4\xb8\xad\xe9\x97\xb4\xe5\xb9\xb3\xe9\x9d\xa2\xe4\xbd\x8d\xe5\xa7\xbf
        * @return  \xe9\x94\x99\xe8\xaf\xaf\xe7\xa0\x81
        */
        public int FT_CalCenterEnd(ref DescPose pos)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.FT_CalCenterEnd();
                int errcode = (int)result[0];
                if (errcode == 0 && result.Length >= 7)
                {
                    pos.tran.x = Convert.ToDouble(result[1]);
                    pos.tran.y = Convert.ToDouble(result[2]);
                    pos.tran.z = Convert.ToDouble(result[3]);
                    pos.rpy.rx = Convert.ToDouble(result[4]);
                    pos.rpy.ry = Convert.ToDouble(result[5]);
                    pos.rpy.rz = Convert.ToDouble(result[6]);
                }
                else
                {
                    if (log != null)
                    {
                        log.LogError($"FT_CalCenterEnd fail {errcode}");
                    }
                }

                if (log != null)
                {
                    log.LogInfo($"FT_CalCenterEnd: {errcode}");
                }
                return errcode;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief  \xe8\xae\xbe\xe7\xbd\xae\xe6\x91\x86\xe5\x8a\xa8\xe7\xbb\x93\xe6\x9d\x9f\xe5\x9b\x9e\xe5\x91\xa8\xe6\x9c\x9f\xe9\x9b\xb6\xe7\x82\xb9
        * @param [in] flag \xe6\x91\x86\xe5\x8a\xa8\xe7\xbb\x93\xe6\x9d\x9f\xe6\x98\xaf\xe5\x90\xa6\xe5\x9b\x9e\xe5\x91\xa8\xe6\x9c\x9f\xe9\x9b\xb6\xe7\x82\xb9\xef\xbc\x9b0-\xe4\xb8\x8d\xe5\x9b\x9e\xe5\x91\xa8\xe6\x9c\x9f\xe9\x9b\xb6\xe7\x82\xb9\xef\xbc\x9b1-\xe5\x9b\x9e\xe5\x91\xa8\xe6\x9c\x9f\xe9\x9b\xb6\xe7\x82\xb9
        * @return  \xe9\x94\x99\xe8\xaf\xaf\xe7\xa0\x81
        */
        public int SetWeavebackCenterConfig(int flag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                int rtn = proxy.SetWeavebackCenterConfig(flag);

                if (log != null)
                {
                    log.LogInfo($"SetWeavebackCenterConfig({flag}): {rtn}");
                }
                return rtn;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

        /**
        * @brief  \xe8\x8e\xb7\xe5\x8f\x96\xe6\x91\x86\xe5\x8a\xa8\xe7\xbb\x93\xe6\x9d\x9f\xe5\x9b\x9e\xe5\x91\xa8\xe6\x9c\x9f\xe9\x9b\xb6\xe7\x82\xb9\xe5\x8f\x82\xe6\x95\xb0
        * @param [out] flag \xe6\x91\x86\xe5\x8a\xa8\xe7\xbb\x93\xe6\x9d\x9f\xe6\x98\xaf\xe5\x90\xa6\xe5\x9b\x9e\xe5\x91\xa8\xe6\x9c\x9f\xe9\x9b\xb6\xe7\x82\xb9\xef\xbc\x9b0-\xe4\xb8\x8d\xe5\x9b\x9e\xe5\x91\xa8\xe6\x9c\x9f\xe9\x9b\xb6\xe7\x82\xb9\xef\xbc\x9b1-\xe5\x9b\x9e\xe5\x91\xa8\xe6\x9c\x9f\xe9\x9b\xb6\xe7\x82\xb9
        * @return  \xe9\x94\x99\xe8\xaf\xaf\xe7\xa0\x81
        */
        public int GetWeavebackCenterConfig(ref int flag)
        {
            if (IsSockComError())
            {
                return g_sock_com_err;
            }

            try
            {
                object[] result = proxy.GetWeavebackCenterConfig();
                int errcode = (int)result[0];
                if (errcode == 0 && result.Length >= 2)
                {
                    flag = (int)result[1];
                }
                else
                {
                    if (log != null)
                    {
                        log.LogError($"GetWeavebackCenterConfig fail {errcode}");
                    }
                }

                if (log != null)
                {
                    log.LogInfo($"GetWeavebackCenterConfig: flag={flag}, errcode={errcode}");
                }
                return errcode;
            }
            catch
            {
                if (log != null)
                {
                    log.LogError("RPC exception");
                }
                return (int)RobotError.ERR_RPC_ERROR;
            }
        }

'''

content = content[:pos] + new_methods + content[pos:]

with open(file, 'wb') as f:
    f.write(content)
print(f'OK: added 6 methods, new size: {len(content)} bytes')
