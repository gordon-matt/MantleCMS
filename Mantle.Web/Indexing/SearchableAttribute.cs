using System;

namespace Mantle.Web.Indexing
{
    //TODO: This is currently not used anymore because of KorePageType.PopulateDocumentIndex()
    //  Might be better to add some properties to this attribute and use it instead of the
    //  aforementioned method
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SearchableAttribute : Attribute
    {
    }
}