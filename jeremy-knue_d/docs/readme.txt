PHASE 1

To run the console application using Visual Studio

1) Create a file named ".env" to the console project.   
On the file's Properties set Copy to Output Directory to always.
It should be a text file containing environmental variables.
Replace the {Placeholders} with actual values to use at runtime.  

GREET contains the message displayed when the application starts up.

EDI_KEY contains the EdiFabric license key

SQL_CONN contains the connection string for the SQL Server database.

Example connection string containing the name of the database to use or create.  In this case "EDI".
.\SQLExpress;Integrated Security=True;Initial Catalog=EDI;TrustServerCertificate=True;


File template ======================================

GREET={some greeting}
EDI_KEY={EdiFrabric key}
SQL_CONN=Server={SQL Server connection string}

======================================

Example file content ======================================

GREET=Starting application
EDI_KEY=ABC0001xyz
SQL_CONN=Server=.\SQLExpress;Integrated Security=True;Initial Catalog=EDI;TrustServerCertificate=True;

======================================




2) Put a 837 edi file named "837.txt" in the in the same directory as the console exe file.
This is currently done automatically by copying the file to the output directory in Visual Studio.


Examples for both files above can be found in the samples directory.  Note that the .env file in samples has been named "sample.env.txt" to all the file to be included in git commits.  Normally .env files are ignored by git.


3) In program.cs set the filename to process.  This is the filename variable.  
The project currently writes the following files to the output directory.  
837.txt - a valid 837 file
Empty.txt - a blank text file
837-malformed.txt - a malformed EDI file

To process another file ensure that a copy is written to the output directory and set the filename variable to the file's name.


==========================================

Unit tests 

1) The EdiParser class depends on a valid license key for EdiFabric existing in the enviroment variable. EDI_KEY
When testing open the class EdiParserTests and ensure that the Setup method has the path to an .env file containing the key.


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




