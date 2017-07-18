﻿using System.Collections.Generic;

namespace Mantle.Web.Messaging
{
    public interface IMessageTemplateTokensProvider
    {
        IEnumerable<string> GetTokens(string templateName);
    }
}