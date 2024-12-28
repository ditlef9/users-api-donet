# Users API DoNet


![Logo](docs/api-dotnet-logo.png)

This API demostrates how a C# .net API would look like. It allows the following:

* Lets users Register, Login and refresh token (JWT Token). 
* Allows users to create posts, get posts, serach for posts, edit and delete posts
* List users, edit user, delete user, update user, add user
* Add users salary information, delete salary and update salary
* Add user job info, delete user job info and update user job info

The API is using C# as the programming language. This is because the .NET ecosystem, including the dotnet CLI, defaults to C# for most project templates, including the webapi template.

It uses `Microsoft SQL` as database and may be deployed to `Azure App Services`. 



---

## Index

[🏠 1 Howto run locally](1-howto-run-locally)<br>
[☁️ 2 Howto deploy to Azure](2-howto-deploy-to-azure)<br>
[📖 3 API Documentation (Swagger)](3-api-documentation-swagger)<br>
[🛠️ 4 How I created the API](4-how-i-created-the-api)<br>
[📜 5 License](📜-5-license)<br>


---

## 🏠 1 Howto run locally

**A. Install software:**
* VS Code - https://code.visualstudio.com
* Git - https://git-scm.com/downloads
* DotNet - https://dotnet.microsoft.com/en-us/download
* Microsoft SQL Server Developer - https://www.microsoft.com/en-us/sql-server/sql-server-downloads
* Azure Data Studio - https://learn.microsoft.com/en-us/azure-data-studio/download-azure-data-studio?view=sql-server-ver16&tabs=win-install%2Cwin-user-install%2Credhat-install%2Cwindows-uninstall%2Credhat-uninstall

**B. Clone the project**

```
mkdir dotnet
cd dotnet
git clone git@github.com:ditlef9/users-api-donet.git
```

**C. Connect to database**

*Windows Users:*

Open Azure Data Studio and create a database named
`UsersDotNetDev`.


*Ubuntu and MacOS:*

Install Docker Desktop:

* MacOS: https://docs.docker.com/desktop/install/mac-install/
* Linux: https://docs.docker.com/desktop/install/linux-install/



MacOS and Linux create a database in Docker:

```
docker run -e "ACCEPT_EULA=1" -e "MSSQL_USER=SA" -e "MSSQL_SA_PASSWORD=SQLConnect1\!" -e "MSSQL_PID=Developer" -p 1433:1433 -d --name=sql_connect mcr.microsoft.com/azure-sql-edge
```

MacOS and Linux open `appsettings.json` and change `DefaultConnection` to: 
```
Server=localhost;Database=UsersDotNetDev;Trusted_Connection=false;TrustServerCertificate=True;User Id=sa;Password=SQLConnect1!;
```

* UserName: sa
* Password: SQLConnect1!

**C. Run the project**

First install the certificate for localhost:
```
dotnet dev-certs https --trust
```

Then lunch the API:

```
dotnet watch run --launch-profile https
```

D. The API is available at:

* https://localhost:7026
* http://localhost:5248

---

## ☁️ 2 Howto deploy to Azure

**A Create a Azure SQL Database**

Go to portal.azure.com > SQL databases > [Create]<br><br>

*Basic*

* Subscription: `Development`
* Resource group: `rg-development`

Database details

* Database Name: `db-development`
* Server: `[Create new]`
    - Server name: `srv-db-development`
    - Location: `Norway`

    - Authentication
        * Authentication method: `Only use Azure Active Directory (Azure AD) authentication` (more safe) or `Use SQL authentication` (less safe)
        * Set Azure AD admin: `[None]` or random username and password, example `azadmin` / `KUEvfJsSeaDa9z6k?wmHadXFD`

Compte + storage: [Configure database] <br>

    * Service Tier: `Basic` <br>
    * Data max size: `0.5 GB`

Backup storae redundancy: `Locally-rendundant backup storage`

*Networking*

* Connectivity endpoint: `Public endpoint` (less secure) or `Private endpoin` (more secure)

Firewall rules:

* Allow Azure services and resources to access this server: `No` (more secure) or `Yes` (less secure)

* Add current client IP address: `Yes` (lets you login to the database from your local machine)



**B Login to Azure:**
```
az login
```

**C Build and release the application on Linux:**
```
dotnet build --configuration Release

az webapp up --sku F1 --name "users-api-dotnet" --os-type linux
```

Go to portal.azure.com > App Services > users-api-dotnet.

---

## 3 📖 API Documentation (Swagger)


https://localhost:7026/swagger


---

## 🛠️ 4 How I created the API

New Web API:
```
dotnet new webapi -n users-api-donet
```


## 📜 5 License

This project is licensed under the
[Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0).

```
Copyright 2024 github.com/ditlef9

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```
