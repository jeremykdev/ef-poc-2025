@echo off
REM this batch file handles both startng the moto server and setting up the s3 files

REM start up motot server
start start-moto.bat

REM wait a few seconds to allow the moto server to start up
echo waiting...
timeout /t 5


REM setup 3s files
call setup-s3.bat