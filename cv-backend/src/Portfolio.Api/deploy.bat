@echo off
setlocal EnableExtensions EnableDelayedExpansion

set PROJECT_ID=project-46e7739a-8ba2-443f-adb
set REGION=europe-north2
set SERVICE_NAME=portfolio-api
set IMAGE_NAME=dod2315/%SERVICE_NAME%:latest

set CHECK_ERROR=if errorlevel 1 goto :error

call gcloud config set project %PROJECT_ID%
%CHECK_ERROR%

call docker build -t %IMAGE_NAME% "." -f ".\cv-backend\src\Portfolio.Api\Dockerfile"
%CHECK_ERROR%

call docker push %IMAGE_NAME%
%CHECK_ERROR%

call gcloud run deploy %SERVICE_NAME% --image %IMAGE_NAME% --region %REGION% --platform managed --allow-unauthenticated
%CHECK_ERROR%

call gcloud run services describe %SERVICE_NAME% --region %REGION% --format "value(status.url)"
%CHECK_ERROR%

exit /b 0

:error
exit /b %ERRORLEVEL%
