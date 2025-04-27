using System.Diagnostics;

namespace Mantle.Messaging;

[DebuggerDisplay("{Key}: {Value}")]
public sealed class Token
{
    public Token(string key, string value) :
        this(key, value, true)
    {
    }

    public Token(string key, string value, bool htmlEncoded)
    {
        this.Key = key;
        this.Value = value;
        this.HtmlEncoded = htmlEncoded;
    }

    /// <summary>
    /// Token key
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Token value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Indicates whether this token should be HTML encoded
    /// </summary>
    public bool HtmlEncoded { get; }
}