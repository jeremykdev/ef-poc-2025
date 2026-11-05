PHASE 1

To run the console application using Visual Studio

1) Create a file named ".env" to the console project.   
On the file's Properties set Copy to Output Directory to always.
It should be a text file containing environmental variables.
Replace the {Placeholders} with actual values to use at runtime.  

GREET contains the message displayed when the application starts up.

EDI_KEY contains the EdiFabric license key

SQL_CONN contains the connection string for the SQL Server database.

AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY contain dummy values for AWS credentials. The values are not validated but must be provided to successfully connect to AWS mock.

S3_BUCKET_NAME contains the bucket name to use when pulling files from AWS S3 mock.

S3_SERVICE_URL contains the AWS URL to use.  This should contain the moto server mock URL.

SNIP_VALIDATION_LEVEL to check.  Should contain an integer that maps to a ValidationLevel, other a default will be used.
See: https://www.edifabric.com/api/html/T_EdiFabric_Core_Model_Edi_ValidationLevel.htm


Example connection string containing the name of the database to use or create.  In this case "EDI".
.\SQLExpress;Integrated Security=True;Initial Catalog=EDI;TrustServerCertificate=True;


File template ======================================

GREET={some greeting}
EDI_KEY={EdiFrabric key}
SQL_CONN=Server={SQL Server connection string}
AWS_ACCESS_KEY_ID={credential value}
AWS_SECRET_ACCESS_KEY={credential value}
S3_BUCKET_NAME={bucket name}
S3_SERVICE_URL={AWS mock URL}
SNIP_VALIDATION_LEVEL={SNIP validation level}
PROCESSOR_KEY={Use 'Local' for processing local files or 'S3' for moto/S3 files, value is case insenstive}
SAVE_ENABLED={Boolean for is save enabled.  Defaults to true if setting omitted or value cannot be parsed as a boolean}
======================================

Example file content ======================================

GREET=Starting application
EDI_KEY=ABC0001xyz
SQL_CONN=Server=.\SQLExpress;Integrated Security=True;Initial Catalog=EDI;TrustServerCertificate=True;
AWS_ACCESS_KEY_ID=DummyValue1
AWS_SECRET_ACCESS_KEY=DummyValue2
S3_BUCKET_NAME=EdiDemo
S3_SERVICE_URL=http://127.0.0.1:5000
SNIP_VALIDATION_LEVEL=2
PROCESSOR_KEY=local
SAVE_ENABLED=true
======================================




2) When running locally go to the EdiFiles directory and for any files to process set the build action to Copy Only so they are copied to bin folder.  See Phase 3 section for notes on running with the S3 processor.

Examples for both files above can be found in the samples directory.  Note that the .env file in samples has been named "sample.env.txt" to all the file to be included in git commits.  Normally .env files are ignored by git.


3) Here are the example files that can used by the project to process:

These file names may differ from what is in the solution.  Files may be renamed to allow for processing in order by filename such as prepending "001-", "002-", etc.

837.txt - a valid 837 file

Empty.txt - a blank text file

837-malformed.txt - a malformed EDI file

837-fails-snip2.txt - same as 837.txt but edited BHT record on line 4 to change valid 'CH' to invalid code 'ZZ'.  This causes it to fail SNIP level 2 validation.

837-fail-snip3.txt - same as 837.txt but edited to cause mismatch between service amount and claim total.  This causes it to fail SNIP level 3 validation.

837-fail-snip4.txt - same as 837.txt but edited to cause Paired attribute validation to fail on DMG segment on line 18, note this passes SNIP 3 validation in online EDINation validator but fails SNIP3 running in this program.

837-PER-paried-item-missing.txt -  same as 837.txt but edited to cause Paired attribute validation to fail on PER segment line 6. In this case TE indicated a telephone number can be expected but none is provided.  This passes SNIP 3 validation in online EDINation validator but fails SNIP3 running in this program.


837-custom-code-pass-snip5.txt - same as 837.txt but edited to change 0019 in BHT to TEST, this 'TEST' value has been added to the validation settings as a valid value for BHT, otherwise that file fails SNIP validation such as in online EDInation validator.

837-fails-snip6-customIEA-validation.txt - same as 837.txt to change IEA segment to fail custom validation

837-custom-snip7.txt - same as 837.txt but edited to added an extra property to end of BHT, the "ExtraPayload".  Parsed with custom classes (see Models) this passes custom validation for SNIP 7

837-custom-snip7-fails.txt  - same as 837.txt but edited to have empty extra property to end of BHT, the "ExtraPayload".  Parsed with custom classes (see Models) this fails custom validation for SNIP 7


To process another file ensure that a copy is written to the output directory and set the filename variable to the file's name when running local files OR modify batch Scripts/setup-s3.bat when using S3 and moto server to upload the file to mock s3.


==========================================

Unit tests 

1) The EdiParser class depends on a valid license key for EdiFabric existing in the enviroment variable. EDI_KEY
When testing open the class EdiParserTests and ensure that the Setup method has the path to an .env file containing the key.


==========================================

PHASE 2 

Setup moto.py
1) Install Python (download at https://www.python.org/ ).  Ensure that python's bin and Scripts directories are added to PATH.
Examples (text in curly braces are placeholders):
C:\Users\{UserName}\AppData\Local\Python\bin
C:\Users\{UserName}\AppData\Local\Python\pythoncore-{version}\Scripts

2) Install Visual Studio C++ version 14.  Required to run moto.
Can use Visual Studio installer to add C++ support.

3) install moto at command line:
pip install "moto[s3,server]"

4) Verify install by starting moto server at command line:
moto_server

5) install AWS CLI (needed to script aws s3 commands).

To test the setup go to the Scripts directory in src\EdiIngestConsole and run the batch files in this order
1) start-moto.bat
This starts the moto server.  Leave this open until finished with testing.  It can be closed when you are finished with s3 tasks/testing.

2) test-moto.bat
This connects to the moto server and runs a few s3 tasks.  Observe the output.  At the end of execution there should be a file in the folder: samples\files\test\from-s3 

Resources

AWS command line reference for S3:
https://docs.aws.amazon.com/cli/latest/reference/s3/

Moto server:
https://docs.getmoto.org/en/latest/docs/server_mode.html


How moto, AWS CLI, and the console app work together.
In phase 2 moto.py is used to simulate an AWS S3 source for files.

See the /Scripts directory for batch files that start moto and setup the s3 files.
Paths in these files need to change if the directory structure changes
Also ensure that the bucket name 'EdiDemo' is used and edit if the changes in the application.

When the console application starts it will call the batch files to a) start the moto server and b) upload the edi files.
Watch those windows and wait on continuing the console application until the server starts and files have been uploaded.



==========================================

PHASE 3 

Phase 3 demonstrates SNIP (Strategic National Implementation Process) validation.

Level 1 - This is the base out of box logic provided by the EDI reader.  No changes needed to support this level.

Level 2 - Checks values against validation attributes in templates such as for Required values.

Level 3 - Checks that line item and total values are valid, such as checking that sum of service line item amounts and claim total amount match.

Level 4 - Checks values on a segment that are related to each other, such as checking that if a contact information segment specifies contact method type of telephone that a value is also provided for the telephone number.
 
Level 5 - Checks that valid codes are provided such as checking that the first value in a BHT segment is '0019'.

Level 6 - Additional checks for valid codes to comply with HIPAA rules.

Level 7 - Checks customized business rules beyond what the X12 standard supports out of the box.

See: https://www.edifabric.com/docs/edifabric-net/how-to-validate-hipaa-snip-levels.html



==========================================


Setup for Phase 3 demo


In env set 
SAVE_ENABLED=false - keep focus on validation
SNIP_VALIDATION_LEVEL=3 - this is SNIP level 4
PROCESSOR_KEY=local 

In EdiFiles folder set the following to Copy to Output Directory = Copy Always. Others should be set to Do Not Copy
001-837.txt - a valid 837 file
Expect pass validation

837-fails-snip2.txt - same as 837.txt but edited 
BHT record on line 4 to change valid 'CH' to invalid code 'ZZ'.  
This causes it to fail SNIP level 2 validation.
Expect fail validation

837-fail-snip3.txt - same as 837.txt but edited 
to cause mismatch between service amount and claim total.  
This causes it to fail SNIP level 3 validation.
Expect fail validation

837-custom-code-pass-snip5.txt - same as 837.txt but edited 
to change 0019 in BHT to TEST, 
this 'TEST' value has been added to the validation settings 
as a valid value for BHT, 
otherwise that file fails SNIP validation such as in online EDInation validator.
Expect pass validation

837-fails-snip6-customIEA-validation.txt - same as 837.txt to change 
IEA segment to fail custom validation. 
IEdiValidator was added to custom class to cause failure 
when InterchangeControlNumberMustEndWith does not end with "101".
Expect fail validation

837-custom-snip7.txt - same as 837.txt but edited to 
add an extra property to end of BHT, the "ExtraPayload".  
Parsed with custom classes (see Models) this passes custom validation
for SNIP 7
Expect pass validation. Will also output the custom extra payload value

837-custom-snip7-fails.txt  - same as 837.txt but edited to have empty extra property to end of BHT, the "ExtraPayload".  Parsed with custom classes (see Models) this fails custom validation for SNIP 7
Expect fail validation. Will also output the custom extra payload value which is missing in this case.
