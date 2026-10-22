@echo off
REM in order for this to work the moto server needs to be running on default port (5000), see start-moto.bat
REM will use bucket name: testJk
REM edit local paths if/when local enviroment paths are different

REM dummy credentials
set AWS_ACCESS_KEY_ID=TEST1
set AWS_SECRET_ACCESS_KEY=TEST2

REM create a bucket  
echo create bucket 
aws --endpoint-url http://127.0.0.1:5000 s3 mb s3://testJk
echo expected output is "make_bucket: testJk"
echo.
echo.

REM copy file to bucket 
echo copy local file to bucket
aws --endpoint-url http://127.0.0.1:5000 s3 cp "C:\Users\JeremyKnue\source\repos\jeremykdev\ef-poc-2025\jeremy-knue_d\samples\files\test\source\demo1.txt" s3://testJk/
echo expected output is something like: "upload: ..\..\..\samples\files\test\source\demo1.txt to s3://testJk/demo1.txt"
echo.
echo.

REM List Files in the Bucket https://docs.aws.amazon.com/cli/latest/reference/s3/ls.html
echo list files in bucket 
aws --endpoint-url http://127.0.0.1:5000 s3 ls s3://testJk/
echo expected output is something like: "2026-10-22 11:39:00         19 demo1.txt"
echo.
echo.

pause


REM download files to local file system  
echo download file from bucket 
aws --endpoint-url http://127.0.0.1:5000 s3 cp s3://testJk/demo1.txt "C:\Users\JeremyKnue\source\repos\jeremykdev\ef-poc-2025\jeremy-knue_d\samples\files\test\from-s3\s3-demo.txt"
echo expected output is something like "download: s3://testJk/demo1.txt to ..\..\..\samples\files\test\from-s3\s3-demo.txt"
echo.
echo.

pause



