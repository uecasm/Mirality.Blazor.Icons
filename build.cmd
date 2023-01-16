@echo off
setlocal
set r=0
for %%a in (Base Fluent OpenMoji) do (
  pushd %%a
  echo %%a
  dotnet pack -c:Release
  if errorlevel 1 call :error
  popd
)
exit /b %r%

:error
set r=1
exit /b
