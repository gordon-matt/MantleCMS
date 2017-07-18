using System;
using System.Diagnostics;

namespace Mantle.Infrastructure
{
    public static class EngineContext
    {
        #region Initialization Methods

        public static IEngine Initialize(bool forceRecreate)
        {
            var locker = new object();
            lock (locker)
            {
                if (Singleton<IEngine>.Instance == null || forceRecreate)
                {
                    Debug.WriteLine("Constructing engine " + DateTime.Now);
                    Singleton<IEngine>.Instance = CreateEngineInstance();
                    Debug.WriteLine("Initializing engine " + DateTime.Now);
                    Singleton<IEngine>.Instance.Initialize();
                }
                return Singleton<IEngine>.Instance;
            }
        }

        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        public static IEngine CreateEngineInstance()
        {
            //if (KoreConfiguration.Instance != null && !string.IsNullOrEmpty(KoreConfiguration.Instance.Engine.Type))
            //{
            //    var engineType = Type.GetType(KoreConfiguration.Instance.Engine.Type);
            //    if (engineType == null)
            //    {
            //        throw new KoreException("The type '" + engineType + "' could not be found. Please check the configuration or check for missing assemblies.");
            //    }
            //    if (!typeof(IEngine).GetTypeInfo().IsAssignableFrom(engineType))
            //    {
            //        throw new KoreException("The type '" + engineType + "' doesn't implement 'Kore.Infrastructure.IEngine' and cannot be configured for that purpose.");
            //    }
            //    return Activator.CreateInstance(engineType) as IEngine;
            //}

            return Default;
        }

        #endregion Initialization Methods

        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IEngine>.Instance;
            }
        }

        public static IEngine Default { get; set; }
    }
}