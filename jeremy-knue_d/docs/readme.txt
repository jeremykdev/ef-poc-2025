To run the console application using Visual Studio

1) Create a file named ".env" to the console project.   
On the file's Properties set Copy to Output Directory to always.
It should be a text file containing enviromental variables.
Replace the {Placeholders} with actual values.  

Example file content ======================================

GREET=Starting application
EDI_KEY={EdiFrabric key}
SQL_CONN=Server={YOUR_SQL_SERVER_CONNECTION_STRING}

======================================

Example connection string containing the name of the database to use or create.  In this case "EDI".
.\SQLExpress;Integrated Security=True;Initial Catalog=EDI;TrustServerCertificate=True;


2) Put a 837 edi file named "837.txt" in the in the same directory as the console exe file.
This is currently done automatically by copying the file to the output directory in Visual Studio.