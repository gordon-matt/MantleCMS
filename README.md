![Mantle CMS](https://github.com/gordon-matt/MantleCMS/raw/master/MantleCMS/wwwroot/img/logo.png)

Mantle is a multi-purpose framework and CMS loaded with functionality and features that can be perfectly adapted for any website.

## Getting Started

### JSPM

1. If not already installed, install [NodeJS](https://nodejs.org/en/download/)
2. Install JSPM globally: `npm install -g jspm`
3. Clone/download this project
4. Restore JSPM packages: `jspm install`
> **NOTE:** Do this from the root directory of the project (**MantleCMS**) (not the solution root)

### Database

> There will be an installation page in future for first time setup of the database. But for now, we need to do this manually:

1. Ensure that **MantleCMS** is the startup project
2. Change the connection string in **appsettings.json**
> **NOTE:** The database does NOT need to exist. Just enter a name for the new database in your connection string and it will be created by EF Migrations at step #4, provided that the server instance and credentials are correct.
3. Open the **Package Manager Console** and ensure the **Default project** option is set to **MantleCMS**
3. Run the **Update-Database** command, which will create the database for you.

## License

This project is licensed under the [MIT license](LICENSE.txt).
