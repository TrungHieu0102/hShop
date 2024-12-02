# hShop - E-commerce Project

hShop is an e-commerce project built using the Clean Architecture pattern. It is designed to manage products, categories, orders, and includes features like product search, order management, and API integration.

# Installation
## 1. System Requirements
Before getting started, ensure that you have the following installed:

- .NET 8 SDK: [Download from .NET website](https://dotnet.microsoft.com/fr-fr/download/dotnet/8.0)

- Visual Studio 2022 or Visual Studio Code

- SQL Server (or any database compatible with Entity Framework Core)

- Git: For cloning the project and version control

## 2. Installation Guide
### Step 1: Clone the Project
First, clone the repository to your local machine:
```bash
git clone https://github.com/TrungHieu0102/hshop.git
cd hshop
```
### Step 2: Configure the Database
1. Create a database in SQL Server (or any database you prefer).
2. Update the connection string in the  ```appsettings.json``` file located in the ```WebApi/```  folder to point to your database:

### Step 3: Apply Migrations and Initialize the Database
Next, apply the migrations using Entity Framework Core to create the necessary tables in your database.

Open a terminal/command prompt, navigate to the Infrastructure folder, and run the following commands:
```bash
cd src/Infrastructure
dotnet ef database update
```
This command will apply the migrations and initialize your database.

### Step 4: Run the Project
Now, navigate to the WebApi folder and start the project using the following commands:

```bash
cd ../WebApi
dotnet run
```
Once the project starts successfully, you can access the API at ```https://localhost:5001``` or ```http://localhost:5000```.
### Step 5: Test the API
You can test the API using **Postman** or **cURL**.

For example, to get a list of products:
``` bash
GET https://localhost:5001/api/products
```
## 3. Folder Structure
 ```bash 
 hShop
├── src/
│   ├── Core/               # Contains entities, interfaces, and business logic
│   ├── Application/        # Contains DTOs, services, and mappings
│   ├── Infrastructure/     # Database configurations and repositories
│   └── WebApi/             # API controllers and web configurations
└── README.md               # Project setup and usage instructions

```
## 4. Technologies Used
- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- AutoMapper

## 5. Contributing and Reporting Issues
If you find any bugs or have suggestions for improving the project, feel free to open an issue  on [GitHub](https://github.com/TrungHieu0102/hshop/issues).

##
By following the above steps, you should be able to set up and run the **hShop** project on your local machine.
