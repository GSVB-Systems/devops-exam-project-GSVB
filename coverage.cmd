@echo off
setlocal enableextensions

set "REPO_ROOT=%~dp0"
if "%REPO_ROOT:~-1%"=="\" set "REPO_ROOT=%REPO_ROOT:~0,-1%"

pushd "%REPO_ROOT%" || (
  echo [ERROR] Failed to switch to repo root: "%REPO_ROOT%"
  exit /b 1
)

set "TEST_RESULTS_DIR=%REPO_ROOT%\TestResults"
set "REPORT_DIR=%TEST_RESULTS_DIR%\CoverageReport"
set "REPORT_FILE=%REPORT_DIR%\index.html"
set "COVERAGE_FILE="
set "EXCLUDE_FILE_FILTERS=-*\obj\*;-*\bin\*;-*\Migrations\*;-*\Generated\*;-*\GenerateTypeScript.cs;-*\Extensions\ServiceCollectionExtensions.cs;-*\Program.cs;-*\EggApi\Ei.cs;-*\EggApi\EggApiClient.cs;-*\EggApi\EggApiOptions.cs;-*\Interfaces\IEggApiClient.cs;-*\Interfaces\IEggAccountService.cs;-*\Interfaces\IEggSnapshotService.cs;-*\Services\Service.cs;-*\Services\EggAccountService.cs;-*\Services\EggSnapshotService.cs;-*\Services\EggSnapshotFormulas.cs;-*\DevOpsAppDbContextFactory.cs;-*\*DbContextFactory*.cs;-*\Models\PagedResult.cs;-*\Models\EggSnapshotResultDto.cs;-*.Designer.cs;-*.designer.cs;-*.g.cs;-*.g.i.cs;-*.generated.cs"

echo [0/5] Cleaning old coverage artifacts to avoid stale reports...
if exist "%REPORT_DIR%" rmdir /s /q "%REPORT_DIR%"
if exist "%TEST_RESULTS_DIR%" (
  for /r "%TEST_RESULTS_DIR%" %%F in (coverage.cobertura.xml) do del /q "%%F" >nul 2>&1
)

echo [1/5] Restoring local .NET tools...
dotnet tool restore
if errorlevel 1 goto :fail_restore

echo [2/5] Running tests with XPlat Code Coverage...
dotnet test "Server\Test\Test.csproj" --collect:"XPlat Code Coverage" --results-directory "%TEST_RESULTS_DIR%"
set "TEST_EXIT_CODE=%ERRORLEVEL%"
if not "%TEST_EXIT_CODE%"=="0" (
  echo [WARN] dotnet test exited with code %TEST_EXIT_CODE%.
  echo [WARN] Continuing to generate report if coverage file exists.
)

echo [3/5] Looking for coverage.cobertura.xml...
for /f "delims=" %%F in ('dir /b /s /a:-d /o-d "%TEST_RESULTS_DIR%\coverage.cobertura.xml" 2^>nul') do (
  set "COVERAGE_FILE=%%F"
  goto :coverage_file_found
)
goto :fail_coverage_missing

:coverage_file_found
echo [INFO] Using coverage file: "%COVERAGE_FILE%"

echo [4/5] Generating report with ReportGenerator...
dotnet tool run reportgenerator -reports:"%COVERAGE_FILE%" -targetdir:"%REPORT_DIR%" -reporttypes:"HtmlInline;Cobertura" -filefilters:"%EXCLUDE_FILE_FILTERS%"
if errorlevel 1 goto :fail_reportgenerator

if not exist "%REPORT_FILE%" goto :fail_report_missing

echo [5/5] Opening report in your default browser...
start "" "%REPORT_FILE%"

echo [DONE] Coverage report: "%REPORT_FILE%"
popd

if not "%TEST_EXIT_CODE%"=="0" exit /b %TEST_EXIT_CODE%
exit /b 0

:fail_restore
echo [ERROR] dotnet tool restore failed.
popd
exit /b 2

:fail_coverage_missing
echo [ERROR] No coverage file found under "%TEST_RESULTS_DIR%".
echo [HINT] Make sure Docker is running (tests use Testcontainers).
popd
exit /b 3

:fail_reportgenerator
echo [ERROR] ReportGenerator failed.
popd
exit /b 4

:fail_report_missing
echo [ERROR] Report was not created at "%REPORT_FILE%".
popd
exit /b 5


