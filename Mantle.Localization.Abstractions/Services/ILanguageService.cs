﻿using LanguageEntity = Mantle.Localization.Entities.Language;

namespace Mantle.Localization.Services;

public interface ILanguageService : IGenericDataService<LanguageEntity>
{
    IEnumerable<LanguageEntity> GetActiveLanguages(int tenantId);

    bool CheckIfRightToLeft(int tenantId, string cultureCode);
}