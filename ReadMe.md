# Phonebook Practice App

## Console Program to practice Entity Framework Core

### Important! - Start Configuration

1. Before running this app, open the appsettings.json file in the root directory of the project.
In case you dont know your connection string, use "localhost" as your server name and "PhoneGallery_Data" as your database name. App was created on .NET **9.0**

2. Save the appsettings.json file and run the app.

### Main functions of this app and their implementation

  1. Project initialization
  2. Create filebase and structure for this project
  3. Create Models in Models directory
  4. Create DbContext for this application
     - I found out that Entity Framework Core in console apps is different compared to webapps
     - I was searching, digging and finally found how to create DbContextFactory!
  5. Build working configuration file and services to provide access to database.

### Main Steps that are yet to be implemented

  1. Add Category model to existing classes
  2. Connect Contacts to Categories with a Foreign Key ✓
  3. Start implementing database CRUD operations
    -  (for Contacts and Categories) ✓
  5. Create user interface ✓
  6. Create validation for Phone numbers + Emails ✓
  7. Handle Exceptions and possible Bugs ✓
  8. Handle possible null values ✓
  9. Windthrough and implement a few try-catch blocks to add stability ✓

#### Additional things i experienced while developing this *simple* app

<p>This was my first experience with Entity Framework Core and SQL Server together. I was frustrated, i didnt know how to fix all the bugs when I was trying to create the migration.. I was asking myself "What am I doing wrong?" I got a solution, and learned a lot. I appreciate all the work you do with the projects and reviews!</p>

#### Created with

- VS 2022, VSCode
- SQL Server
- Entity Framework Core
- Spectre Console

-DreamFXX
