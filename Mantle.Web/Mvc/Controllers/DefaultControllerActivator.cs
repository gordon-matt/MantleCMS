﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Mantle.Web.Mvc.Controllers;

internal class DefaultControllerActivator : IControllerActivator
{
    private readonly ITypeActivatorCache _typeActivatorCache;

    /// <summary>
    /// Creates a new <see cref="DefaultControllerActivator"/>.
    /// </summary>
    /// <param name="typeActivatorCache">The <see cref="ITypeActivatorCache"/>.</param>
    public DefaultControllerActivator(ITypeActivatorCache typeActivatorCache)
    {
        _typeActivatorCache = typeActivatorCache
            ?? throw new ArgumentNullException(nameof(typeActivatorCache));
    }

    /// <inheritdoc />
    public object Create(ControllerContext controllerContext)
    {
        if (controllerContext == null)
        {
            throw new ArgumentNullException(nameof(controllerContext));
        }

        if (controllerContext.ActionDescriptor == null)
        {
            throw new ArgumentException(string.Format("The '{0}' property of '{1}' must not be null.",
                nameof(ControllerContext.ActionDescriptor),
                nameof(ControllerContext)));
        }

        var controllerTypeInfo = controllerContext.ActionDescriptor.ControllerTypeInfo;

        if (controllerTypeInfo == null)
        {
            throw new ArgumentException(string.Format("The '{0}' property of '{1}' must not be null.",
                nameof(controllerContext.ActionDescriptor.ControllerTypeInfo),
                nameof(ControllerContext.ActionDescriptor)));
        }

        var serviceProvider = controllerContext.HttpContext.RequestServices;
        return _typeActivatorCache.CreateInstance<object>(serviceProvider, controllerTypeInfo.AsType());
    }

    /// <inheritdoc />
    public void Release(ControllerContext context, object controller)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        if (controller is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}