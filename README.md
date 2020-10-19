# Table of contents
<details>
   <summary>Click here to expand content list.</summary>
   
1. [General information](#1-general-information)
2. [License](#2-license)
3. [System description](#3-system-description)
4. [System requirements](#4-system-requirements)
5. [Supported features](#5-supported-features)
6. [ER diagram](#6-er-diagram)
    * [6.1 ER rules](#61-er-rules)
7. [Sequence diagrams](#7-sequence-diagrams)
    * [7.1 Save page visit](#71-save-page-visit)
    * [7.2 Save settings](#72-save-settings)
8. [User interface](#8-user-interface)
9. [Setup guide](#9-setup-guide)
    * [9.1 Prerequisites](#91-prerequisites)
    * [9.2 Create the database and its tables](#92-create-the-database-and-its-tables)
    * [9.3 Configure the application](#93-configure-the-application)
    * [9.4 NuGet packages](#94-nuget-packages)
    * [9.5 Run and test the application](#95-run-and-test-the-application)
10. [Contact details](#10-contact-details)
</details>

---

# 1 General information
"Visitor Statistics 1.0" was created in Visual Studio Community by Annice Strömberg, 2020, with [Annice.se](https://annice.se) as the primary download location. The script can be used to register visitors of an application to store and view information such as: IP address, host, browser, visit time, referer URL, etc.

---

# 2 License
Released under the MIT license.

MIT: [http://rem.mit-license.org](http://rem.mit-license.org/), see [LICENSE](LICENSE).

---

# 3 System description
"Visitor Statistics 1.0" is built in the front-end languages: CSS (customized, and with Font Awesome 5.0.9), HTML5, JavaScript (classic, and with Ajax, jQuery 3.5.1 and Font Awesome 5.0.9).

Moreover, the back-end code is build in C# 8.0 based on ASP.NET Core 3.1 using the MVC (Model-View-Controller) design pattern. In addition, the database interaction is based on Entity Framework Core 3.1 – database first – using a relational database built in Transact SQL with SQL Server as a DBMS (DataBase Management System).

---

# 4 System requirements
The script can be run on servers that support C# 8.0 and ASP.NET Core 3.1 along with an SQL Server supported database.

---

# 5 Supported features
The following functions and features are supported by this script:
  * Login support based on sessions.
  * User password encryption (HMAC-SHA256).
  * CRUD support for an admin user.
  * Paging function on the visit logs page.
  * Filter function to be able to exclude IPs from the visit logs page.
  * Database storage based on Entity Framework Core – database first.
  * Timer function to automatically delete visit logs from the database based on configured deletion days.
  * Responsive design.
  * Client and server side validation.
  * Ajax post requests for enhanced performance on form submits.

---
  
# 6 ER diagram
The following diagram illustrates the database table relationships reflecting the entity relationships, and the table attributes (columns) reflecting the entity properties used by this script.

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/er-diagram.png" alt="" width="700">

## 6.1 ER rules
  * An application can have many URLs, but a URL is unique for every application.

  * An application can have many visitors, but every application visitor is registrered on a URL by one unique IP address.

  * An application visitor and admin user can visit many application URLs several times.

  * An admin user can be traced to one unique visit, which in turn can be mapped to one unique IP address.
  
---

# 7 Sequence diagrams
This section illustrates some high level context flows to give you an overview of how the application layers interact in a couple of scenarios.

## 7.1 Save page visit

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/save-visit-sequence-diagram.png" alt="" width="750">

## 7.2 Save settings

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/save-settings-sequence-diagram.png" alt="" width="750">

---

# 8 User interface
Screenshot of the visit logs page:

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/gui-visit-logs.png" alt="" width="650">

Screenshot of the visit details page:

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/gui-visit-details.png" alt="" width="650">

Screenshot of the admin settings panel:

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/gui-admin-settings.png" alt="" width="650">

Screenshot of the application settings panel:

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/gui-app-settings.png" alt="" width="650">

Screenshot of the visit settings panel:

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/gui-visit-settings.png" alt="" width="650">

Screenshot of the statistics page:

<img src="https://diagrams.annice.se/c-sharp-visitor-statistics-1.0/gui-statistics.png" alt="" width="650">

---

# 9 Setup guide
As this script was created in Visual Studio Community with SQL Server, I will go through the necessary setup steps accordingly (all softwares used for this application setup are free).

## 9.1 Prerequisites
  * [Install SQL Server Express](https://www.microsoft.com/sv-se/sql-server/sql-server-downloads)
  * [Install SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sv-se/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
  * [Install .NET Core 3.1 (SDK)](https://dotnet.microsoft.com/download)
  * [Install Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
  
## 9.2 Create the database and its tables
  1. Open the file "sql_visitorstatistics.sql", look up the code section below and change the highlighted values to suit your own settings. (Note! The default password is set to "admin", but can be changed after your first login):

```sql
INSERT INTO VisitorStatistics.dbo.VS_Admins 
VALUES 
  (
    'YourFirstName', -- Optional.
    'YourLastName', -- Optional.
    'your@email.com', 
    -- Keep the hashed password below until your first login. Default password is set to
    -- "admin", but can be changed under the admin panel once you're logged in:
   'AQAAAAEAACcQAAAAEBehHmgEHZmjXlTBGlKSW9KVuxMIHp1f4r8sC502SFQkGGxiYeef6HFntNMCMdZ76w=='
)
```

2. Once you have updated the SQL code in "sql_visitorstatistics.sql", then open and execute the SQL file/code in SQL Server Management Studio to create the VisitorStatistics database with its tables.

## 9.3 Configure the application
  3. When the database and tables are created, you can open the application in Visual Studio by double clicking the solution file "VisitorStatistics.sln" under the project folder path: "c-sharp-visitor-statistics-1.0 > VisitorStatistics > VisitorStatistics.sln".

  4. In Visual Studio, you can then change the commented values below to suit your own settings in the appsettings.json file found in the Solution Explorer window:
  
```json
{
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DBConnect": "Server=.\\SQLEXPRESS;Database=VisitorStatistics;Trusted_Connection=True;MultipleActiveResultSets=true" // You can keep this AS-IS unless you use e.g. DB password.
  },
  "Paging": {
    "ItemsPerPage": "50" // Set how many visit logs to be displayed per page.
  },
  "IpDeletionDays": {
    "Days": "1" // This can be edited via the visit settings panel.
  }
}
```

## 9.4 NuGet packages
  6. Also, ensure you have the following NuGet packages installed for the solution, otherwise [install](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-powershell) them:
  
    * Microsoft.EntityFrameworkCore.SqlServer (3.1.7)
    * Microsoft.EntityFrameworkCore.Tools (3.1.7)
    * Microsoft.VisualStudio.Web.CodeGeneration.Design (3.1.4)
    * ReflectionIT.Mvc.Paging (4.0.0)

## 9.5 Run and test the application
  7. Select to run the application via the Visual Studio play button in the top menu bar.

  8. On your first login, use the password "**admin**" along with the user email you specified when you executed the SQL code (see section 9.2).

# 10 Contact details
For general feedback related to this script, such as any discovered bugs etc., you can contact me via the following email address: [info@annice.se](mailto:info@annice.se)
