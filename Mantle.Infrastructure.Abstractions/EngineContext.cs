using System.Runtime.CompilerServices;

namespace Mantle.Infrastructure;

/// <summary>
/// Provides access to the singleton instance of the Mantle engine.
/// </summary>
public static class EngineContext
{
    private static IEngine defaultEngine = null;

    #region Properties

    /// <summary>
    /// Gets the singleton Mantle engine used to access Mantle services.
    /// </summary>
    public static IEngine Current
    {
        get
        {
            if (Singleton<IEngine>.Instance == null)
            {
                Create(defaultEngine);
            }

            return Singleton<IEngine>.Instance;
        }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Create a static instance of the Mantle engine.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static IEngine Create(IEngine engine)
    {
        defaultEngine = engine;

        if (Singleton<IEngine>.Instance == null)
        {
            Singleton<IEngine>.Instance = engine;
        }

        return Singleton<IEngine>.Instance;
    }

    #endregion Methods
}