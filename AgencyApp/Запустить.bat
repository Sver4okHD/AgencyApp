@echo off
cd /d "%~dp0"
start "" "bin\Debug\net9.0-windows7.0\AgencyApp.exe"

IF EXIST "bin\Debug\net9.0-windows7.0\AgencyApp.exe" (
    start "" "bin\Debug\net9.0-windows7.0\AgencyApp.exe"
    exit
) ELSE IF EXIST "bin\Release\net9.0-windows7.0\AgencyApp.exe" (
    start "" "bin\Release\net9.0-windows7.0\AgencyApp.exe"
    exit
) ELSE (
    echo Файл AgencyApp.exe не найден.
    echo Проверьте папки:
    echo - bin\Debug\net9.0-windows7.0\
    echo - bin\Release\net9.0-windows7.0\
    echo.
    pause
)

