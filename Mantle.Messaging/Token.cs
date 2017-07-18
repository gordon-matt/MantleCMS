namespace Mantle.Messaging
{
    public sealed class Token
    {
        private readonly string key;
        private readonly bool htmlEncoded;
        private readonly string value;

        public Token(string key, string value) :
            this(key, value, true)
        {
        }

        public Token(string key, string value, bool htmlEncoded)
        {
            this.key = key;
            this.value = value;
            this.htmlEncoded = htmlEncoded;
        }

        /// <summary>
        /// Token key
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Token value
        /// </summary>
        public string Value
        {
            get { return value; }
        }

        /// <summary>
        /// Indicates whether this token should be HTML encoded
        /// </summary>
        public bool HtmlEncoded
        {
            get { return htmlEncoded; }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Key, Value);
        }
    }
}