using System.Text.Encodings.Web;

namespace Mantle.Web.IO;

public class HtmlTextWriter : TextWriter, IHtmlContent
{
    private readonly TextWriter stringWriter;

    public HtmlTextWriter()
    {
        stringWriter = new StringWriter();
    }

    public override Encoding Encoding => stringWriter.Encoding;

    public override string ToString() => stringWriter.ToString();

    public override void Write(string value) => stringWriter.Write(value);

    public override void Write(char value) => stringWriter.Write(value);

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

    protected override void Dispose(bool disposing)
    {
        stringWriter?.Dispose();
        base.Dispose(disposing);
    }
}