Landis+Gyr Full Stack Developer Assessment
This solution demonstrates a full-stack application with asynchronous report generation using RabbitMQ. The application consists of two main projects:
ReportService: Handles report generation and management.
MeterService: It collects and processes data from smart meters. It saves the data to the database and performs the necessary validations.
WebInterface: Provides a web interface for users to create and download reports.
Table of Contents
Prerequisites
Setup
Clone the Repository
Database Setup
RabbitMQ Setup
Configuration
Running the Projects
Usage
Creating Reports
Viewing Reports
Structure
Troubleshooting
Prerequisites
Before running this application, ensure you have the following installed:
.NET 5 SDK
SQL Server
RabbitMQ
Visual Studio 2022
Setup
Clone the Repository
Clone the repository to your local machine:
bash
Copy code
git clone <repository-url>
cd <repository-directory>

Database Setup
Create the database in SQL Server by using “SmartMeterDB.sql” file provided in the root folder of the solution
Update the connections strings in appsettings.json files of webservice projects:
json
RabbitMQ Setup
Install RabbitMQ and ensure the service is running.
The default configuration assumes RabbitMQ is running locally with the default settings.
Configuration
Update the configuration in WebInterface/appsettings.json to point to the ReportService URL:
json
Copy code
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Services": {
    "MeterService": "https://localhost:44321",
    "ReportService": "https://localhost:44305"
  },

}

Running the Projects
Open the solution in Visual Studio.
Set the startup projects:
Right-click on the solution in Solution Explorer and select "Set Startup Projects..."
Choose "Multiple startup projects" and set the action to "Start" for both ReportService, Meterservice and WebInterface.
Press F5 to run the projects.
Usage
Creating Reports
Navigate to the WebInterface in your browser (usually https://localhost:44386).
Use the form to create a new report by entering a valid meter serial number and submitting the form.
The report generation request is processed asynchronously.
Viewing Reports
Navigate to the "Reports" section in the WebInterface.
You will see a list of generated reports. Click the "Download" button to download the report file.
Structure
ReportService: Handles report generation, stores report metadata, and manages RabbitMQ communication.
Controllers
Models
Data
Services
WebInterface: Provides a web interface for users to interact with the application.
Controllers
Models
Views
Services
Troubleshooting
Common Issues
Database Connection Issues:
Ensure SQL Server is running.
Verify the connection strings in appsettings.json are correct.
RabbitMQ Connection Issues:
Ensure RabbitMQ is running.
Check the RabbitMQ logs for errors.
Access Issues:
Ensure the SharedReports directory is correctly configured and accessible by both services.
Logs
Check the output window in Visual Studio for any runtime errors.
Review the application logs for detailed error messages.

