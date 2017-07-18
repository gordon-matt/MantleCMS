using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Mantle.IO;

namespace Mantle
{
    public static class ObjectExtensions
    {
        public static T ConvertTo<T>(this object source)
        {
            //return (T)Convert.ChangeType(source, typeof(T));
            return (T)ConvertTo(source, typeof(T));
        }

        public static object ConvertTo(this object source, Type type)
        {
            if (type == typeof(Guid))
            {
                return new Guid(source.ToString());
            }

            return Convert.ChangeType(source, type);
        }

        /// <summary>
        /// Determines whether this T is contained in the specified 'IEnumerable of T'
        /// </summary>
        /// <typeparam name="T">This System.Object's type</typeparam>
        /// <param name="t">This item</param>
        /// <param name="enumerable">The 'IEnumerable of T' to check</param>
        /// <returns>true if enumerable contains this item, otherwise false.</returns>
        public static bool In<T>(this T t, IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
            {
                if (item.Equals(t))
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// Determines whether this T is contained in the specified values
        /// </summary>
        /// <typeparam name="T">This System.Object's type</typeparam>
        /// <param name="t">This item</param>
        /// <param name="items">The values to compare</param>
        /// <returns>true if values contains this item, otherwise false.</returns>
        public static bool In<T>(this T t, params T[] items)
        {
            foreach (T item in items)
            {
                if (item.Equals(t))
                { return true; }
            }
            return false;
        }

        public static bool IsDefault<T>(this T item)
        {
            return EqualityComparer<T>.Default.Equals(item, default(T));
        }

        public static bool GenericEquals<T>(this T item, T other)
        {
            return EqualityComparer<T>.Default.Equals(item, other);
        }

        //public static string SharpSerialize<T>(this T item)
        //{
        //    var sharpSettings = new SharpSerializerXmlSettings
        //    {
        //        IncludeAssemblyVersionInTypeName = false,
        //        IncludeCultureInTypeName = false,
        //        IncludePublicKeyTokenInTypeName = false
        //    };

        //    using (var stream = new MemoryStream())
        //    {
        //        new SharpSerializer(sharpSettings).Serialize(item, stream);
        //        stream.Position = 0;

        //        using (var reader = new StreamReader(stream))
        //        {
        //            return reader.ReadToEnd();
        //        }
        //    }
        //}

        /// <summary>
        /// <para>Serializes the specified System.Object and writes the XML document</para>
        /// <para>to the specified file.</para>
        /// </summary>
        /// <typeparam name="T">This item's type</typeparam>
        /// <param name="item">This item</param>
        /// <param name="fileName">The file to which you want to write.</param>
        /// <param name="omitXmlDeclaration">False to keep the XML declaration. Otherwise, it will be removed.</param>
        /// <param name="removeNamespaces">
        ///     <para>Specify whether to remove xml namespaces.</para>para>
        ///     <para>If your object has any XmlInclude attributes, then set this to false</para>
        /// </param>
        /// <param name="xmlns">If not null, "removeNamespaces" is ignored and the provided namespaces are used.</param>
        /// <param name="encoding">Specify encoding, if required.</param>
        /// <returns>true if successful, otherwise false.</returns>
        public static bool XmlSerialize<T>(
            this T item,
            string fileName,
            bool omitXmlDeclaration = true,
            bool removeNamespaces = true,
            XmlSerializerNamespaces xmlns = null,
            Encoding encoding = null)
        {
            string xml = item.XmlSerialize(omitXmlDeclaration, removeNamespaces, xmlns, encoding);
            return xml.ToFile(fileName);
        }

        /// <summary>
        /// Serializes the specified System.Object and returns the serialized XML
        /// </summary>
        /// <typeparam name="T">This item's type</typeparam>
        /// <param name="item">This item</param>
        /// <param name="omitXmlDeclaration">False to keep the XML declaration. Otherwise, it will be removed.</param>
        /// <param name="removeNamespaces">
        ///     <para>Specify whether to remove xml namespaces.</para>para>
        ///     <para>If your object has any XmlInclude attributes, then set this to false</para>
        /// </param>
        /// <param name="xmlns">If not null, "removeNamespaces" is ignored and the provided namespaces are used.</param>
        /// <param name="encoding">Specify encoding, if required.</param>
        /// <returns>Serialized XML for specified System.Object</returns>
        public static string XmlSerialize<T>(
            this T item,
            bool omitXmlDeclaration = true,
            bool removeNamespaces = true,
            XmlSerializerNamespaces xmlns = null,
            Encoding encoding = null)
        {
            object locker = new object();

            var xmlSerializer = new XmlSerializer(item.GetType());

            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = omitXmlDeclaration
            };

            lock (locker)
            {
                var stringBuilder = new StringBuilder();
                using (var stringWriter = new CustomEncodingStringWriter(encoding, stringBuilder))
                {
                    using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        if (xmlns != null)
                        {
                            xmlSerializer.Serialize(xmlWriter, item, xmlns);
                        }
                        else
                        {
                            if (removeNamespaces)
                            {
                                xmlns = new XmlSerializerNamespaces();
                                xmlns.Add(string.Empty, string.Empty);

                                xmlSerializer.Serialize(xmlWriter, item, xmlns);
                            }
                            else { xmlSerializer.Serialize(xmlWriter, item); }
                        }

                        return stringBuilder.ToString();
                    }
                }
            }
        }
    }
}