$ErrorActionPreference = "Stop"
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
$proj = "src\FRRobot\FRRobot.csproj"

$frameworks = @(
    @{Ver="v4.7.2"; Name="net472"},
    @{Ver="v4.8";   Name="net48"}
)

foreach ($fw in $frameworks) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host " Building $($fw.Name) ($($fw.Ver))" -ForegroundColor Cyan
    Write-Host "========================================"

    $outDir = "bin\x64\$($fw.Name)\Debug"
    & $msbuild $proj `
        /p:Configuration=Debug `
        /p:Platform=x64 `
        /p:TargetFrameworkVersion=$($fw.Ver) `
        /p:OutputPath=$outDir `
        /nologo /v:m

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed for $($fw.Ver)" -ForegroundColor Red
        exit 1
    }
    Write-Host " -> $outDir\libfairino.dll" -ForegroundColor Green
}

Write-Host "`nAll 5 frameworks built successfully." -ForegroundColor Green
