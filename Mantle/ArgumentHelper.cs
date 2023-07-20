namespace Mantle
{
    public static class ArgumentHelper
    {
        public static void ThrowIfFalse(bool condition, string message, string paramName = null)
        {
            if (!condition)
            {
                if (paramName == null)
                {
                    throw new ArgumentException(message);
                }
                else
                {
                    throw new ArgumentException(message, paramName);
                }
            }
        }

        public static void ThrowIfNull<T>(T value, string paramName, string message = null) where T : class
        {
            if (value == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                else
                {
                    throw new ArgumentNullException(paramName, message);
                }
            }
        }

        public static void ThrowIfNullOrEmpty(string value, string paramName, string message = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                else
                {
                    throw new ArgumentNullException(paramName, message);
                }
            }
        }
    }
}