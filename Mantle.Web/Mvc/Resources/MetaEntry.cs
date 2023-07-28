namespace Mantle.Web.Mvc.Resources;

public class MetaEntry
{
    private readonly TagBuilder builder = new("meta");

    public string Charset
    {
        get
        {
            string value;
            builder.Attributes.TryGetValue("charset", out value);
            return value;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                SetAttribute("charset", value);
            }
        }
    }

    public string Content
    {
        get
        {
            string value;
            builder.Attributes.TryGetValue("content", out value);
            return value;
        }
        set { SetAttribute("content", value); }
    }

    public string HttpEquiv
    {
        get
        {
            string value;
            builder.Attributes.TryGetValue("http-equiv", out value);
            return value;
        }
        set { SetAttribute("http-equiv", value); }
    }

    public string Name
    {
        get
        {
            string value;
            builder.Attributes.TryGetValue("name", out value);
            return value;
        }
        set { SetAttribute("name", value); }
    }

    public MetaEntry AddAttribute(string name, string value)
    {
        builder.MergeAttribute(name, value);
        return this;
    }

    public static MetaEntry Combine(MetaEntry meta1, MetaEntry meta2, string contentSeparator)
    {
        var newMeta = new MetaEntry();
        Merge(newMeta.builder.Attributes, new[] { meta1.builder.Attributes, meta2.builder.Attributes });
        if (!string.IsNullOrEmpty(meta1.Content) && !string.IsNullOrEmpty(meta2.Content))
        {
            newMeta.Content = meta1.Content + contentSeparator + meta2.Content;
        }
        return newMeta;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("<meta name=\"");
        sb.Append(Name);
        sb.Append("\" ");

        if (!string.IsNullOrEmpty(Content))
        {
            sb.Append("content=\"");
            sb.Append(Content);
            sb.Append("\" ");
        }

        if (!string.IsNullOrEmpty(Charset))
        {
            sb.Append("charset=\"");
            sb.Append(Charset);
            sb.Append("\" ");
        }

        if (!string.IsNullOrEmpty(HttpEquiv))
        {
            sb.Append("http-equiv=\"");
            sb.Append(HttpEquiv);
            sb.Append("\" ");
        }

        sb.Append("/>");

        return sb.ToString();

        //cannot use this way anymore, because it does not put "name" as first attribute, which is customer's requirement
        //return builder.ToString(TagRenderMode.SelfClosing);
    }

    private static void Merge(IDictionary<string, string> d1, params IDictionary<string, string>[] sources)
    {
        foreach (var d in sources)
        {
            foreach (var pair in d)
            {
                d1[pair.Key] = pair.Value;
            }
        }
    }

    public MetaEntry SetAttribute(string name, string value)
    {
        builder.MergeAttribute(name, value, true);
        return this;
    }
}