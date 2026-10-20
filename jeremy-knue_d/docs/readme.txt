To run the console application using Visual Studio

1) Create a file named ".env" to the console project.   
On the file's Properties set Copy to Output Directory to always.
It should be a text file containing enviromental variables.
Replace the {Placeholders} with actual values to use at runtime.  

GREET contains the message displayed when the application starts up.

EDI_KEY contains the EdiFabric license key

SQL_CONN contain the connection string for the SQL Server database.

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


Examples for both files above can be found in the samples directory.  Note that the .env file in samples has been named "sample.env.txt" to all the file to be inclduded in git commits.  Normally .env files are ignored by git.