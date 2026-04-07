@echo off
setlocal EnableExtensions EnableDelayedExpansion

set "ROOT=%~dp0"
if "%ROOT:~-1%"=="\" set "ROOT=%ROOT:~0,-1%"

set "RESULTS_DIR=%ROOT%\TestResults"
set "REPORT_DIR=%ROOT%\coverage-report"
set "TEST_PROJECT=%ROOT%\Test\Test.csproj"

echo Restoring local dotnet tools...
dotnet tool restore
if errorlevel 1 goto :fail

echo Running tests with XPlat Code Coverage...
if exist "%RESULTS_DIR%" rmdir /s /q "%RESULTS_DIR%"
mkdir "%RESULTS_DIR%"
dotnet test "%TEST_PROJECT%" --collect:"XPlat Code Coverage" --results-directory "%RESULTS_DIR%"
if errorlevel 1 goto :fail

set "COVERAGE_FILE="
for /f "delims=" %%F in ('dir /s /b /a:-d /o-d "%RESULTS_DIR%\coverage.cobertura.xml" 2^>nul') do (
    set "COVERAGE_FILE=%%F"
    goto :coverage_found
)

echo Could not find coverage.cobertura.xml under "%RESULTS_DIR%".
goto :fail

:coverage_found
echo Generating HTML report from "!COVERAGE_FILE!"...
if exist "%REPORT_DIR%" rmdir /s /q "%REPORT_DIR%"
mkdir "%REPORT_DIR%"
dotnet tool run reportgenerator -reports:"!COVERAGE_FILE!" -targetdir:"%REPORT_DIR%" -reporttypes:"Html" -filefilters:"-*\obj\*;-*\bin\*;-*\Migrations\*;-*/obj/*;-*/bin/*;-*/Migrations/*;-*Migration*.cs;-*ModelSnapshot.cs;-*DbContextFactory.cs;-*\Api\Program.cs;-*/Api/Program.cs;-*\Api\GenerateTypeScript.cs;-*/Api/GenerateTypeScript.cs;-*\Repository\DevOpsAppDbContext.cs;-*/Repository/DevOpsAppDbContext.cs" -classfilters:"-Ei.*;-*.Migrations.*;-*DbContextFactory*;-DevOpsAppService.Services.Service*;-api.GenerateApiClientsExtensions*;-DevOpsAppRepo.DevOpsAppDbContext*"
if errorlevel 1 goto :fail

set "INDEX_FILE=%REPORT_DIR%\index.html"
if not exist "%INDEX_FILE%" (
    echo Coverage report was generated, but "%INDEX_FILE%" is missing.
    goto :fail
)

echo Opening "%INDEX_FILE%"...
start "" "%INDEX_FILE%"

echo Done.
exit /b 0

:fail
echo Coverage script failed.
exit /b 1



