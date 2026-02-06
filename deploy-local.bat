@echo off
echo ==========================================
echo V2Ray Application - Local Build Script
echo ==========================================
echo.

REM Step 1: Build Vue.js Frontend
echo [1/3] Building Vue.js Frontend...
cd "%~dp0vue\v2ray"
call npm install
call npm run build
if %errorlevel% neq 0 (
    echo ERROR: Vue.js build failed!
    pause
    exit /b 1
)
echo Vue.js build completed successfully!
echo.

REM Step 2: Verify build output
echo [2/3] Verifying build output...
cd "%~dp0"
if not exist "V2Ray.Api\wwwroot\index.html" (
    echo ERROR: Build output not found in V2Ray.Api\wwwroot\
    pause
    exit /b 1
)
echo Build output verified!
echo.

REM Step 3: Create deployment package
echo [3/3] Creating deployment package...
set DEPLOY_DIR=%~dp0deploy-package
if exist "%DEPLOY_DIR%" rmdir /s /q "%DEPLOY_DIR%"
mkdir "%DEPLOY_DIR%"

echo Copying files...
xcopy /E /I /Y "%~dp0V2Ray.Api" "%DEPLOY_DIR%\V2Ray.Api\"
xcopy /E /I /Y "%~dp0nginx" "%DEPLOY_DIR%\nginx\"
copy /Y "%~dp0docker-compose.yml" "%DEPLOY_DIR%\"
copy /Y "%~dp0deploy.sh" "%DEPLOY_DIR%\"
copy /Y "%~dp0DEPLOYMENT.md" "%DEPLOY_DIR%\"

echo.
echo ==========================================
echo Build completed successfully!
echo ==========================================
echo.
echo Deployment package created at:
echo %DEPLOY_DIR%
echo.
echo Next steps:
echo 1. Transfer the entire 'deploy-package' folder to your server
echo 2. Follow instructions in DEPLOYMENT.md
echo.
echo You can use WinSCP or run this command:
echo scp -r "%DEPLOY_DIR%" root@109.94.164.46:/var/www/v2ray
echo.
pause
