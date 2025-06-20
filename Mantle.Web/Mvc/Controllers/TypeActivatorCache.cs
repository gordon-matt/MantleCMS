﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Mvc.Controllers;

internal class TypeActivatorCache : ITypeActivatorCache
{
    private readonly Func<Type, ObjectFactory> _createFactory =
        (type) => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);

    private readonly ConcurrentDictionary<Type, ObjectFactory> _typeActivatorCache = new();

    /// <inheritdoc/>
    public TInstance CreateInstance<TInstance>(
        IServiceProvider serviceProvider,
        Type implementationType)
    {
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        if (implementationType == null)
        {
            throw new ArgumentNullException(nameof(implementationType));
        }

        var createFactory = _typeActivatorCache.GetOrAdd(implementationType, _createFactory);
        return (TInstance)createFactory(serviceProvider, arguments: null);
    }
}