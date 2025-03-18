@echo off
@echo Building solution for Windows x64..
dotnet build ./../ --configuration Release -p:arch=win-x64
@echo Press enter to close the window.
pause > nul