using System.IO;
using System.Text;

namespace ClassDiagramGenerator.Infrastructure.Utilities
{
    public static class PlantUmlUrlEncoder
    {
        private static readonly string EncodeMap = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";

        public static string ConvertTextToUrl(string uml)
        {
            byte[] data = Encoding.UTF8.GetBytes(uml);

            using var ms = new MemoryStream();
            using (var ds = new System.IO.Compression.DeflateStream(
                       ms,
                       System.IO.Compression.CompressionLevel.Optimal,
                       true))
            {
                ds.Write(data, 0, data.Length);
            }

            byte[] deflated = ms.ToArray();
            string encoded = EncodeBytes(deflated);
            return "https://www.plantuml.com/plantuml/uml/" + encoded;
        }

        private static string EncodeBytes(byte[] data)
        {
            var sb = new StringBuilder();
            int current = 0;
            int bits = 0;

            foreach (byte value in data)
            {
                current = (current << 8) | value;
                bits += 8;

                while (bits >= 6)
                {
                    bits -= 6;
                    sb.Append(EncodeMap[(current >> bits) & 0x3F]);
                }
            }

            if (bits > 0)
                sb.Append(EncodeMap[(current << (6 - bits)) & 0x3F]);

            return sb.ToString();
        }
    }
}