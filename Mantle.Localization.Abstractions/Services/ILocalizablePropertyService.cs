using System.Collections.Generic;
using Mantle.Data.Services;
using Mantle.Localization.Domain;

namespace Mantle.Localization.Services
{
    public interface ILocalizablePropertyService : IGenericDataService<LocalizableProperty>
    {
        LocalizableProperty FindOne(string cultureCode, string entityType, string entityId, string property);

        IEnumerable<LocalizableProperty> Find(string cultureCode, string entityType, string entityId);

        IEnumerable<LocalizableProperty> Find(string cultureCode, string entityType, IEnumerable<string> entityIds);

        IEnumerable<LocalizableProperty> Find(string cultureCode, string entityType, IEnumerable<string> entityIds, string property);
    }
}