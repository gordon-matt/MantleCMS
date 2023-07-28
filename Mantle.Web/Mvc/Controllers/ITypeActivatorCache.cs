﻿namespace Mantle.Web.Mvc.Controllers;

internal interface ITypeActivatorCache
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TInstance"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve dependencies for
    /// <paramref name="optionType"/>.</param>
    /// <param name="optionType">The <see cref="Type"/> of the <typeparamref name="TInstance"/> to create.</param>
    TInstance CreateInstance<TInstance>(IServiceProvider serviceProvider, Type optionType);
}