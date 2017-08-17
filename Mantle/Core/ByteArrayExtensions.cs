using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Mantle
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Deserializes the Binary data contained in the specified System.Byte[].
        /// </summary>
        /// <typeparam name="T">The type of System.Object to be deserialized.</typeparam>
        /// <param name="data">This System.Byte[] instance.</param>
        /// <returns>The System.Object being deserialized.</returns>
        public static T BinaryDeserialize<T>(this byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var binaryFormatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                var item = (T)binaryFormatter.Deserialize(stream);
                stream.Close();
                return item;
            }
        }

        public static MemoryStream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        public static T XmlDeserialize<T>(this byte[] bytes)
        {
            return bytes.XmlDeserialize<T>(new UTF8Encoding());
        }

        public static T XmlDeserialize<T>(this byte[] bytes, Encoding encoding)
        {
            string s = encoding.GetString(bytes);
            return s.XmlDeserialize<T>();
        }

        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static string ToFileSize(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString(CultureInfo.InvariantCulture) + suf[place];
        }
    }
}