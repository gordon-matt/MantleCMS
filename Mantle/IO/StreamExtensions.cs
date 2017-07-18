using System.IO;
using System.IO.Compression;

namespace Mantle.IO
{
    public static class StreamExtensions
    {
        public static MemoryStream DeflateCompress(this Stream stream)
        {
            var memoryStream = new MemoryStream();
            var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress);
            stream.CopyTo(deflateStream);
            return memoryStream;
        }

        public static MemoryStream DeflateDecompress(this Stream stream)
        {
            var memoryStream = new MemoryStream();
            var deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
            deflateStream.CopyTo(memoryStream);
            return memoryStream;
        }

        public static MemoryStream GZipCompress(this Stream stream)
        {
            var memoryStream = new MemoryStream();
            var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress);
            stream.CopyTo(gZipStream);
            return memoryStream;
        }

        public static MemoryStream GZipDecompress(this Stream stream)
        {
            var memoryStream = new MemoryStream();
            var gZipStream = new GZipStream(stream, CompressionMode.Decompress);
            gZipStream.CopyTo(memoryStream);
            return memoryStream;
        }
    }
}