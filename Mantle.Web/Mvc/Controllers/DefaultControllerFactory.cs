﻿// https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc.Core/Controllers/DefaultControllerFactory.cs

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Mantle.Web.Mvc.Controllers;

/// <summary>
/// Default implementation for <see cref="IControllerFactory"/>.
/// </summary>
public class DefaultControllerFactory : IControllerFactory
{
    private readonly IControllerActivator _controllerActivator;
    private readonly IControllerPropertyActivator[] _propertyActivators;

    /// <summary>
    /// Initializes a new instance of <see cref="DefaultControllerFactory"/>.
    /// </summary>
    /// <param name="controllerActivator">
    /// <see cref="IControllerActivator"/> used to create controller instances.
    /// </param>
    /// <param name="propertyActivators">
    /// A set of <see cref="IControllerPropertyActivator"/> instances used to initialize controller
    /// properties.
    /// </param>
    public DefaultControllerFactory(
        IControllerActivator controllerActivator,
        IEnumerable<IControllerPropertyActivator> propertyActivators)
    {
        if (controllerActivator == null)
        {
            throw new ArgumentNullException(nameof(controllerActivator));
        }

        if (propertyActivators == null)
        {
            throw new ArgumentNullException(nameof(propertyActivators));
        }

        _controllerActivator = controllerActivator;
        _propertyActivators = propertyActivators.ToArray();
    }

    /// <inheritdoc />
    public object CreateController(ControllerContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.ActionDescriptor == null)
        {
            throw new ArgumentException($"The '{nameof(ControllerContext.ActionDescriptor)}' property of '{nameof(ControllerContext)}' must not be null.");
        }

        object controller = _controllerActivator.Create(context);
        foreach (var propertyActivator in _propertyActivators)
        {
            propertyActivator.Activate(context, controller);
        }

        return controller;
    }

    /// <inheritdoc />
    public void ReleaseController(ControllerContext context, object controller)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        _controllerActivator.Release(context, controller);
    }
}