@echo off
cd /d "%~dp0"

start VianaMotos.Web.exe

timeout /t 3 > nul

start http://localhost:5000