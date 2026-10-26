@echo off
REM sets up mock s3
REM moto needs to be running on default port (5000) for this to work, run start-moto.bat first

REM dummy credentials
set AWS_ACCESS_KEY_ID=TEST1
set AWS_SECRET_ACCESS_KEY=TEST2

REM create a bucket  
echo create bucket
aws --endpoint-url http://127.0.0.1:5000 s3 mb s3://EdiDemo


REM copy files to bucket 
aws --endpoint-url http://127.0.0.1:5000 s3 cp "C:\Users\JeremyKnue\source\repos\jeremykdev\ef-poc-2025\jeremy-knue_d\src\EdiIngestConsole\837.txt" s3://EdiDemo/

aws --endpoint-url http://127.0.0.1:5000 s3 cp "C:\Users\JeremyKnue\source\repos\jeremykdev\ef-poc-2025\jeremy-knue_d\src\EdiIngestConsole\837-malformed.txt" s3://EdiDemo/

aws --endpoint-url http://127.0.0.1:5000 s3 cp "C:\Users\JeremyKnue\source\repos\jeremykdev\ef-poc-2025\jeremy-knue_d\src\EdiIngestConsole\837-fail-snip3.txt" s3://EdiDemo/

aws --endpoint-url http://127.0.0.1:5000 s3 cp "C:\Users\JeremyKnue\source\repos\jeremykdev\ef-poc-2025\jeremy-knue_d\src\EdiIngestConsole\837-fails-snip2.txt" s3://EdiDemo/

pause