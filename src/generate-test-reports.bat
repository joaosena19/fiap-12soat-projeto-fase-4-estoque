@echo off
echo.
echo 🧪 Executando testes e gerando relatórios...
echo.

powershell.exe -ExecutionPolicy Bypass -File "%~dp0generate-test-reports.ps1"

if %ERRORLEVEL% equ 0 (
    echo.
    echo ✅ Relatórios gerados com sucesso!
) else (
    echo.
    echo ❌ Erro na geração dos relatórios
)

echo.
pause