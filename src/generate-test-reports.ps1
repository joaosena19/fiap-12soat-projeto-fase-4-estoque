Write-Host "Limpando relatorios antigos..." -ForegroundColor Cyan
if (Test-Path "Tests/Report") {
    Remove-Item "Tests/Report" -Recurse -Force
}
New-Item -ItemType Directory -Path "Tests/Report" -Force | Out-Null

Write-Host "Executando testes com cobertura..." -ForegroundColor Cyan
dotnet test Tests/Tests.csproj --collect:"XPlat Code Coverage" --results-directory "Tests/Report" --settings "Tests/coverlet.runsettings"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Testes executados com sucesso!" -ForegroundColor Green
    
    # Instalar ReportGenerator se necessario
    if (-not (Get-Command "reportgenerator" -ErrorAction SilentlyContinue)) {
        Write-Host "Instalando ReportGenerator..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-reportgenerator-globaltool
    }
    
    # Gerar relatorio HTML
    $coverageFile = Get-ChildItem -Path "Tests/Report" -Filter "coverage.cobertura.xml" -Recurse | Select-Object -First 1
    if ($coverageFile) {
        Write-Host "Gerando relatorio HTML..." -ForegroundColor Cyan
        reportgenerator "-reports:$($coverageFile.FullName)" "-targetdir:Tests/Report/html" "-reporttypes:Html"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Relatorio HTML gerado em: Tests/Report/html/index.html" -ForegroundColor Green
        }
    }
} else {
    Write-Host "Falha na execucao dos testes" -ForegroundColor Red
    exit 1
}