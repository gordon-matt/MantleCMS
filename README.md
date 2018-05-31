[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=gordon_matt%40live%2ecom&lc=AU&currency_code=AUD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted)

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

## Donate
If you find this project helpful, consider buying me a cup of coffee.  :-)

#### PayPal:

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=gordon_matt%40live%2ecom&lc=AU&currency_code=AUD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted)

#### Crypto:
- **Bitcoin**: 1EeDfbcqoEaz6bbcWsymwPbYv4uyEaZ3Lp
- **Ethereum**: 0x277552efd6ea9ca9052a249e781abf1719ea9414
- **Litecoin**: LRUP8hukWGXRrcPK6Tm7iUp9vPvnNNt3uz
