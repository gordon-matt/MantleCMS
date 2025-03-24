using Mantle.Web.Common.Areas.Admin.Regions.Entities;
using Mantle.Web.Common.Areas.Admin.Regions.Services;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.Common;

public class StartupTask : IStartupTask
{
    #region IStartupTask Members

    public void Execute()
    {
        var dbContextFactory = EngineContext.Current.Resolve<IDbContextFactory>();
        using var dbContext = dbContextFactory.GetContext();
        EnsureData(dbContext);
    }

    public int Order => 10;

    #endregion IStartupTask Members

    private void EnsureData(DbContext dbContext)
    {
        var regionService = EngineContext.Current.Resolve<IRegionService>();
        int count = regionService.Count();

        if (count > 0)
        {
            return;
        }

        var northAmerica = new Region { Name = "North America", RegionType = RegionType.Continent };
        var southAmerica = new Region { Name = "South America", RegionType = RegionType.Continent };
        var australasia = new Region { Name = "Australasia", RegionType = RegionType.Continent };
        var asiaAndMiddleEast = new Region { Name = "Asia and Middle East", RegionType = RegionType.Continent };
        var africa = new Region { Name = "Africa", RegionType = RegionType.Continent };
        var europe = new Region { Name = "Europe", RegionType = RegionType.Continent };
        var antarctica = new Region { Name = "Antarctica", RegionType = RegionType.Continent };

        regionService.Insert(new[]
        {
            northAmerica,
            southAmerica,
            australasia,
            asiaAndMiddleEast,
            africa,
            europe,
            antarctica
        });

        //var dataSettings = EngineContext.Current.Resolve<DataSettings>();//TODO
        //if (dataSettings.CreateSampleData)
        //{

        #region Insert Countries

        #region North America

        regionService.Insert(
        [
            new Region { Name = "Anguilla", CountryCode = "AI", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Antigua and Barbuda", CountryCode = "AG", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Aruba", CountryCode = "AW", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Bahamas", CountryCode = "BS", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Barbados", CountryCode = "BB", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Belize", CountryCode = "BZ", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Bermuda", CountryCode = "BM", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Bonaire, Sint Eustatius and Saba", CountryCode = "BQ", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Canada", CountryCode = "CA", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Cayman Islands", CountryCode = "KY", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Costa Rica", CountryCode = "CR", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Cuba", CountryCode = "CU", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Curaçao", CountryCode = "CW", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Dominica", CountryCode = "DM", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Dominican Republic", CountryCode = "DO", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "El Salvador", CountryCode = "SV", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Greenland", CountryCode = "GL", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Grenada", CountryCode = "GD", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Guadeloupe", CountryCode = "GP", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Guatemala", CountryCode = "GT", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Haiti", CountryCode = "HT", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Honduras", CountryCode = "HN", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Jamaica", CountryCode = "JM", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Martinique", CountryCode = "MQ", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Mexico", CountryCode = "MX", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Montserrat", CountryCode = "MS", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Nicaragua", CountryCode = "NI", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Panama", CountryCode = "PA", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Puerto Rico", CountryCode = "PR", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Saint Barthélemy", CountryCode = "BL", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Saint Kitts and Nevis", CountryCode = "KN", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Saint Lucia", CountryCode = "LC", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Saint Martin (French Part)", CountryCode = "MF", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Saint Pierre and Miquelon", CountryCode = "PM", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Saint Vincent and the Grenadines", CountryCode = "VC", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Sint Maarten (Dutch Part)", CountryCode = "SX", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Trinidad and Tobago", CountryCode = "TT", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Turks and Caicos Islands", CountryCode = "TC", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "United States", CountryCode = "US", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Virgin Islands, British", CountryCode = "VG", RegionType = RegionType.Country, ParentId = northAmerica.Id },
            new Region { Name = "Virgin Islands, U.S.", CountryCode = "VI", RegionType = RegionType.Country, ParentId = northAmerica.Id },
        ]);

        #endregion North America

        #region South America

        regionService.Insert(
        [
            new Region { Name = "Argentina", CountryCode = "AR", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Bolivia, Plurinational State of", CountryCode = "BO", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Brazil", CountryCode = "BR", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Chile", CountryCode = "CL", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Colombia", CountryCode = "CO", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Ecuador", CountryCode = "EC", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Falkland Islands (Malvinas)", CountryCode = "FK", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "French Guiana", CountryCode = "GF", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Guyana", CountryCode = "GY", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Paraguay", CountryCode = "PY", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Peru", CountryCode = "PE", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Suriname", CountryCode = "SR", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Uruguay", CountryCode = "UY", RegionType = RegionType.Country, ParentId = southAmerica.Id },
            new Region { Name = "Venezuela, Bolivarian Republic of", CountryCode = "VE", RegionType = RegionType.Country, ParentId = southAmerica.Id },
        ]);

        #endregion South America

        #region Australasia

        regionService.Insert(
        [
            new Region { Name = "American Samoa", CountryCode = "AS", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Australia", CountryCode = "AU", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Cook Islands", CountryCode = "CK", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Fiji", CountryCode = "FJ", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Guam", CountryCode = "GU", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Kiribati", CountryCode = "KI", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Marshall Islands", CountryCode = "MH", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Micronesia, Federated States of", CountryCode = "FM", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Nauru", CountryCode = "NR", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "New Caledonia", CountryCode = "NC", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "New Zealand", CountryCode = "NZ", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Niue", CountryCode = "NU", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Norfolk Island", CountryCode = "NF", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Northern Mariana Islands", CountryCode = "MP", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Palau", CountryCode = "PW", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Papua New Guinea", CountryCode = "PG", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Pitcairn", CountryCode = "PN", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Samoa", CountryCode = "WS", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Solomon Islands", CountryCode = "SB", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Tokelau", CountryCode = "TK", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Tonga", CountryCode = "TO", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Tuvalu", CountryCode = "TV", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "United States Minor Outlying Islands", CountryCode = "UM", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Vanuatu", CountryCode = "VU", RegionType = RegionType.Country, ParentId = australasia.Id },
            new Region { Name = "Wallis and Futuna", CountryCode = "WF", RegionType = RegionType.Country, ParentId = australasia.Id },
        ]);

        #endregion Australasia

        #region Asia and Middle East

        regionService.Insert(
        [
            new Region { Name = "Afghanistan", CountryCode = "AF", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Armenia", CountryCode = "AM", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Azerbaijan", CountryCode = "AZ", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Bahrain", CountryCode = "BH", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Bangladesh", CountryCode = "BD", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Bhutan", CountryCode = "BT", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "British Indian Ocean Territory", CountryCode = "IO", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Brunei Darussalam", CountryCode = "BN", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Cambodia", CountryCode = "KH", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "China", CountryCode = "CN", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Christmas Island", CountryCode = "CX", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Cocos (Keeling) Islands", CountryCode = "CC", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "French Polynesia", CountryCode = "PF", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Georgia", CountryCode = "GE", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Hong Kong", CountryCode = "HK", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "India", CountryCode = "IN", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Indonesia", CountryCode = "ID", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Iran, Islamic Republic of", CountryCode = "IR", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Iraq", CountryCode = "IQ", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Israel", CountryCode = "IL", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Japan", CountryCode = "JP", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Jordan", CountryCode = "JO", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Kazakhstan", CountryCode = "KZ", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Mantlea, Democratic People's Republic of", CountryCode = "KP", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Mantlea, Republic of", CountryCode = "KR", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Kuwait", CountryCode = "KW", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Kyrgyzstan", CountryCode = "KG", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Lao People's Democratic Republic", CountryCode = "LA", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Lebanon", CountryCode = "LB", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Macao", CountryCode = "MO", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Malaysia", CountryCode = "MY", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Maldives", CountryCode = "MV", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Mongolia", CountryCode = "MN", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Myanmar", CountryCode = "MM", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Nepal", CountryCode = "NP", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Oman", CountryCode = "OM", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Pakistan", CountryCode = "PK", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Palestinian Territory Occupied", CountryCode = "PS", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Philippines", CountryCode = "PH", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Qatar", CountryCode = "QA", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Saudi Arabia", CountryCode = "SA", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Singapore", CountryCode = "SG", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Sri Lanka", CountryCode = "LK", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Syrian Arab Republic", CountryCode = "SY", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Taiwan, Province of China", CountryCode = "TW", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Tajikistan", CountryCode = "TJ", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Thailand", CountryCode = "TH", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Timor-Leste", CountryCode = "TL", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Turkey", CountryCode = "TR", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Turkmenistan", CountryCode = "TM", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "United Arab Emirates", CountryCode = "AE", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Uzbekistan", CountryCode = "UZ", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Viet Nam", CountryCode = "VN", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id },
            new Region { Name = "Yemen", CountryCode = "YE", RegionType = RegionType.Country, ParentId = asiaAndMiddleEast.Id }
        ]);

        #endregion Asia and Middle East

        #region Africa

        regionService.Insert(
        [
            new Region { Name = "Algeria", CountryCode = "DZ", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Angola", CountryCode = "AO", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Benin", CountryCode = "BJ", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Botswana", CountryCode = "BW", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Burkina Faso", CountryCode = "BF", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Burundi", CountryCode = "BI", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Cameroon", CountryCode = "CM", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Cape Verde", CountryCode = "CV", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Central African Republic", CountryCode = "CF", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Chad", CountryCode = "TD", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Comoros", CountryCode = "KM", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Congo", CountryCode = "CG", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Congo, Democratic Republic of the", CountryCode = "CD", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Cook Islands", CountryCode = "CK", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Côte d'Ivoire", CountryCode = "CI", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Djibouti", CountryCode = "DJ", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Egypt", CountryCode = "EG", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Equatorial Guinea", CountryCode = "GQ", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Eritrea", CountryCode = "ER", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Ethiopia", CountryCode = "ET", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Gabon", CountryCode = "GA", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Gambia", CountryCode = "GM", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Ghana", CountryCode = "GH", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Guinea", CountryCode = "GN", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Guinea-Bissau", CountryCode = "GW", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Kenya", CountryCode = "KE", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Lesotho", CountryCode = "LS", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Liberia", CountryCode = "LR", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Libya", CountryCode = "LY", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Madagascar", CountryCode = "MG", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Malawi", CountryCode = "MW", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Mali", CountryCode = "ML", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Mauritania", CountryCode = "MR", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Mauritius", CountryCode = "MU", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Mayotte", CountryCode = "YT", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Morocco", CountryCode = "MA", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Mozambique", CountryCode = "MZ", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Namibia", CountryCode = "NA", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Niger", CountryCode = "NE", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Nigeria", CountryCode = "NG", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Réunion", CountryCode = "RE", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Rwanda", CountryCode = "RW", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Saint Helena, Ascension and Tristan da Cunha", CountryCode = "SH", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Sao Tome and Principe", CountryCode = "ST", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Senegal", CountryCode = "SN", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Seychelles", CountryCode = "SC", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Sierra Leone", CountryCode = "SL", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Somalia", CountryCode = "SO", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "South Africa", CountryCode = "ZA", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "South Sudan", CountryCode = "SS", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Sudan", CountryCode = "SD", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Swaziland", CountryCode = "SZ", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Tanzania, United Republic of", CountryCode = "TZ", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Togo", CountryCode = "TG", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Tunisia", CountryCode = "TN", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Uganda", CountryCode = "UG", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Western Sahara", CountryCode = "EH", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Zambia", CountryCode = "ZM", RegionType = RegionType.Country, ParentId = africa.Id },
            new Region { Name = "Zimbabwe", CountryCode = "ZW", RegionType = RegionType.Country, ParentId = africa.Id },
        ]);

        #endregion Africa

        #region Europe

        regionService.Insert(
        [
            new Region { Name = "Åland Islands", CountryCode = "AX", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Albania", CountryCode = "AL", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Andorra", CountryCode = "AD", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Austria", CountryCode = "AT", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Belarus", CountryCode = "BY", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Belgium", CountryCode = "BE", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Bosnia and Herzegovina", CountryCode = "BA", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Bulgaria", CountryCode = "BG", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Croatia", CountryCode = "HR", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Cyprus", CountryCode = "CY", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Czech Republic", CountryCode = "CZ", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Denmark", CountryCode = "DK", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Estonia", CountryCode = "EE", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Faroe Islands", CountryCode = "FO", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Finland", CountryCode = "FI", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "France", CountryCode = "FR", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Germany", CountryCode = "DE", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Gibraltar", CountryCode = "GI", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Greece", CountryCode = "GR", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Guernsey", CountryCode = "GG", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Holy See (Vatican City State)", CountryCode = "VA", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Hungary", CountryCode = "HU", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Iceland", CountryCode = "IS", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Ireland", CountryCode = "IE", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Isle of Man", CountryCode = "IM", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Italy", CountryCode = "IT", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Jersey", CountryCode = "JE", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Latvia", CountryCode = "LV", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Liechtenstein", CountryCode = "LI", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Lithuania", CountryCode = "LT", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Luxembourg", CountryCode = "LU", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Macedonia, the former Yuslav Republic of", CountryCode = "MK", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Malta", CountryCode = "MT", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Moldova, Republic of", CountryCode = "MD", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Monaco", CountryCode = "MC", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Montenegro", CountryCode = "ME", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Netherlands", CountryCode = "NL", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Norway", CountryCode = "NO", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Poland", CountryCode = "PL", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Portugal", CountryCode = "PT", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Romania", CountryCode = "RO", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Russia", CountryCode = "RU", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "San Marino", CountryCode = "SM", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Serbia", CountryCode = "RS", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Serbia and Montenegro (former Yugoslavia)", CountryCode = "YU", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Slovakia", CountryCode = "SK", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Slovenia", CountryCode = "SI", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Spain", CountryCode = "ES", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Svalbard and Jan Mayen", CountryCode = "SJ", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Sweden", CountryCode = "SE", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Switzerland", CountryCode = "CH", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "Ukraine", CountryCode = "UA", RegionType = RegionType.Country, ParentId = europe.Id },
            new Region { Name = "United Kingdom", CountryCode = "GB", RegionType = RegionType.Country, ParentId = europe.Id },
        ]);

        #endregion Europe

        #region Antarctica

        regionService.Insert(
        [
            new Region { Name = "Antarctica", CountryCode = "AQ", RegionType = RegionType.Country, ParentId = antarctica.Id },
            new Region { Name = "Bouvet Island", CountryCode = "BV", RegionType = RegionType.Country, ParentId = antarctica.Id },
            new Region { Name = "French Southern Territories", CountryCode = "TF", RegionType = RegionType.Country, ParentId = antarctica.Id },
            new Region { Name = "Heard Island and McDonald Islands", CountryCode = "HM", RegionType = RegionType.Country, ParentId = antarctica.Id },
            new Region { Name = "South Georgia and the South Sandwich Islands", CountryCode = "GS", RegionType = RegionType.Country, ParentId = antarctica.Id },
        ]);

        #endregion Antarctica

        #endregion Insert Countries

        //}
    }
}