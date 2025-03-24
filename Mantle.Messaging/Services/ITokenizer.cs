namespace Mantle.Messaging.Services;

public interface ITokenizer
{
    /// <summary>
    /// Replace all of the token key occurences inside the specified template text with corresponded token values
    /// </summary>
    /// <param name="template">The template with token keys inside</param>
    /// <param name="tokens">The sequence of tokens to use</param>
    /// <param name="htmlEncode">The value indicating whether tokens should be HTML encoded</param>
    /// <returns>Text with all token keys replaces by token value</returns>
    string Replace(string template, IEnumerable<Token> tokens, bool htmlEncode);
}

public class Tokenizer : ITokenizer
{
    /// <summary>
    /// Replace all of the token key occurences inside the specified template text with corresponded token values
    /// </summary>
    /// <param name="template">The template with token keys inside</param>
    /// <param name="tokens">The sequence of tokens to use</param>
    /// <param name="htmlEncode">The value indicating whether tokens should be HTML encoded</param>
    /// <returns>Text with all token keys replaces by token value</returns>
    public string Replace(string template, IEnumerable<Token> tokens, bool htmlEncode)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            return template;
        }

        if (tokens == null)
        {
            throw new ArgumentNullException(nameof(tokens));
        }

        foreach (var token in tokens)
        {
            string tokenValue = token.Value;

            //do not encode URLs
            if (htmlEncode && token.HtmlEncoded)
            {
                tokenValue = tokenValue.HtmlEncode();
            }
            template = Replace(template, token.Key, tokenValue);
        }
        return template;
    }

    private static string Replace(string original, string pattern, string replacement) => original.Replace(pattern, replacement);
}