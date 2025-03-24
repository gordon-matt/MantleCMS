namespace Mantle.Plugins.Messaging.Forums.Extensions;

public static class SeoExtensions
{
    public static string GetSeName(this ForumGroup forumGroup)
    {
        if (forumGroup == null)
        {
            throw new ArgumentNullException(nameof(forumGroup));
        }
        string seName = GetSeName(forumGroup.Name);
        return seName;
    }

    public static string GetSeName(this Forum forum)
    {
        if (forum == null)
        {
            throw new ArgumentNullException(nameof(forum));
        }
        string seName = GetSeName(forum.Name);
        return seName;
    }

    public static string GetSeName(this ForumTopic forumTopic)
    {
        if (forumTopic == null)
        {
            throw new ArgumentNullException(nameof(forumTopic));
        }
        string seName = GetSeName(forumTopic.Subject);

        // Trim SE name to avoid URLs that are too long
        int maxLength = 100;
        if (seName.Length > maxLength)
        {
            seName = seName[..maxLength];
        }

        return seName;
    }

    public static string GetSeName(string name) => GetSeName(name, false, false);// TODO://var seoSettings = EngineContext.Current.Resolve<SeoSettings>();//return GetSeName(name, seoSettings.ConvertNonWesternChars, seoSettings.AllowUnicodeCharsInUrls);

    public static string GetSeName(string name, bool convertNonWesternChars, bool allowUnicodeCharsInUrls) => name.ToSlugUrl();// TODO://if (string.IsNullOrEmpty(name))//{//    return name;//}//string okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";//name = name.Trim().ToLowerInvariant();//if (convertNonWesternChars)//{//    if (_seoCharacterTable == null)//    {//        InitializeSeoCharacterTable();//    }//}//var sb = new StringBuilder();//foreach (char c in name.ToCharArray())//{//    string c2 = c.ToString();//    if (convertNonWesternChars)//    {//        if (_seoCharacterTable.ContainsKey(c2))//        {//            c2 = _seoCharacterTable[c2];//        }//    }//    if (allowUnicodeCharsInUrls)//    {//        if (char.IsLetterOrDigit(c) || okChars.Contains(c2))//        {//            sb.Append(c2);//        }//    }//    else if (okChars.Contains(c2))//    {//        sb.Append(c2);//    }//}//string name2 = sb.ToString();//name2 = name2.Replace(" ", "-");//while (name2.Contains("--"))//{//    name2 = name2.Replace("--", "-");//}//while (name2.Contains("__"))//{//    name2 = name2.Replace("__", "_");//}//return name2;
}