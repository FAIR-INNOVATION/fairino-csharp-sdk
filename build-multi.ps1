$ErrorActionPreference = "Stop"
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
$proj = "src\FRRobot\FRRobot.csproj"
$nugetRoot = "$env:USERPROFILE\.nuget\packages"

$frameworks = @(
    @{Ver="v4.5.1"; Name="net451"; DllName=".NETFramework4.5.1"; RefPath="$nugetRoot\microsoft.netframework.referenceassemblies.net451\1.0.3\build\.NETFramework\v4.5.1"},
    @{Ver="v4.6.1"; Name="net461"; DllName=".NETFramework4.6.1"; RefPath="$nugetRoot\microsoft.netframework.referenceassemblies.net461\1.0.3\build\.NETFramework\v4.6.1"},
    @{Ver="v4.7.1"; Name="net471"; DllName=".NETFramework4.7.1"; RefPath="$nugetRoot\microsoft.netframework.referenceassemblies.net471\1.0.3\build\.NETFramework\v4.7.1"},
    @{Ver="v4.7.2"; Name="net472"; DllName=".netFramework4.7.2"; RefPath=""},
    @{Ver="v4.8.1"; Name="net481"; DllName=".NETFramework4.8.1"; RefPath="$nugetRoot\microsoft.netframework.referenceassemblies.net481\1.0.3\build\.NETFramework\v4.8.1"}
)

foreach ($fw in $frameworks) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host " Building $($fw.Name) ($($fw.Ver))" -ForegroundColor Cyan
    Write-Host "========================================"

    $outDir = "..\..\dlls\$($fw.DllName)"
    $intDir = "obj\x64\$($fw.Name)\Debug\"
    $args = @(
        $proj,
        "/p:Configuration=Debug",
        "/p:Platform=x64",
        "/p:TargetFrameworkVersion=$($fw.Ver)",
        "/p:OutputPath=$outDir",
        "/p:IntermediateOutputPath=$intDir",
        "/nologo",
        "/v:m"
    )
    if ($fw.RefPath) {
        $args += "/p:FrameworkPathOverride=$($fw.RefPath)"
    }

    & $msbuild $args

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed for $($fw.Ver)" -ForegroundColor Red
        exit 1
    }
    Write-Host " -> dlls\$($fw.DllName)\libfairino.dll" -ForegroundColor Green
}

Write-Host "`nAll 5 frameworks built successfully." -ForegroundColor Green
