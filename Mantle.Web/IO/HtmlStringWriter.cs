using System.IO;
using System.Text;

namespace Mantle.Web.IO
{
    public class HtmlStringWriter : TextWriter
    {
        private readonly TextWriter writer;

        public HtmlStringWriter()
        {
            writer = new StringWriter();
        }

        public override Encoding Encoding
        {
            get { return writer.Encoding; }
        }

        public string ToHtmlString()
        {
            return writer.ToString();
        }

        public override string ToString()
        {
            return writer.ToString();
        }

        public override void Write(string value)
        {
            writer.Write(value);
        }

        public override void Write(char value)
        {
            writer.Write(value);
        }
    }
}