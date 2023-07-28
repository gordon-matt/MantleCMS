// https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc.Core/Controllers/IControllerPropertyActivatorFactory.cs

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Mantle.Web.Mvc.Controllers;

public interface IControllerPropertyActivator
{
    void Activate(ControllerContext context, object controller);

    Action<ControllerContext, object> GetActivatorDelegate(ControllerActionDescriptor actionDescriptor);
}