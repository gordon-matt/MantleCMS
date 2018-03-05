using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace Mantle.Web.IO
{
    public class HtmlTextWriter : TextWriter, IHtmlContent
    {
        private readonly TextWriter stringWriter;

        public HtmlTextWriter()
        {
            stringWriter = new StringWriter();
        }

        public override Encoding Encoding
        {
            get { return stringWriter.Encoding; }
        }
        
        public override string ToString()
        {
            return stringWriter.ToString();
        }

        public override void Write(string value)
        {
            stringWriter.Write(value);
        }

        public override void Write(char value)
        {
            stringWriter.Write(value);
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            //if (encoder == null)
            //{
            //    throw new ArgumentNullException(nameof(encoder));
            //}

            writer.Write(stringWriter.ToString());
        }
    }
}