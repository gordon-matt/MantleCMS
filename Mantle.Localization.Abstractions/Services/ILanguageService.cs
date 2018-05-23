using System.Collections.Generic;
using Mantle.Data.Services;
using LanguageEntity = Mantle.Localization.Domain.Language;

namespace Mantle.Localization.Services
{
    public interface ILanguageService : IGenericDataService<LanguageEntity>
    {
        IEnumerable<LanguageEntity> GetActiveLanguages(int tenantId);

        bool CheckIfRightToLeft(int tenantId, string cultureCode);
    }
}