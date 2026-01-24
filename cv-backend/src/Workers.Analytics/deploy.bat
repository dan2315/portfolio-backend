@echo off
setlocal EnableExtensions EnableDelayedExpansion

set PROJECT_ID=project-46e7739a-8ba2-443f-adb
set REGION=europe-north2
set WORKER_POOL_NAME=portfolio-worker-analytics
set IMAGE_NAME=dod2315/%WORKER_POOL_NAME%:latest

set CHECK_ERROR=if errorlevel 1 goto :error

call gcloud config set project %PROJECT_ID%
%CHECK_ERROR%

call docker build -t %IMAGE_NAME% "." -f ".\cv-backend\src\Workers.Analytics\Dockerfile"
%CHECK_ERROR%

call docker push %IMAGE_NAME%
%CHECK_ERROR%

call gcloud beta run worker-pools deploy %WORKER_POOL_NAME% --image %IMAGE_NAME% --region %REGION%
%CHECK_ERROR%

exit /b 0

:error
exit /b %ERRORLEVEL%
