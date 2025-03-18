@echo off
@echo Building solution for Linux x64..
dotnet build ./../ --configuration Release -p:arch=linux-x64
@echo Press enter to close the window.
pause > nul