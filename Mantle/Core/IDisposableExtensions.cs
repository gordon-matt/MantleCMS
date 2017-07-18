﻿using System;

namespace Mantle
{
    public static class DisposableExtensions
    {
        public static void DisposeIfNotNull(this IDisposable disposableObject)
        {
            if (disposableObject != null)
            {
                disposableObject.Dispose();
            }
        }
    }
}