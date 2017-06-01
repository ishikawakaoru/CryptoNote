using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace CryptoNote
{
    public class GZip
    {
        public static void Compress(String fileSource, String fileDestination)
        {
            using (FileStream fsInput = new FileStream(fileSource, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write))
                {
                    using (GZipStream gzipStream = new GZipStream(fsOutput, CompressionMode.Compress))
                    {
                        Byte[] buffer = new Byte[fsInput.Length];
                        int h;
                        while ((h = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            gzipStream.Write(buffer, 0, h);
                        }
                    }
                }
            }
        }

        public static void Decompress(String fileSource, String fileDestination)
        {
            using (FileStream fsInput = new FileStream(fileSource, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsOutput = new FileStream(fileDestination, FileMode.Create, FileAccess.Write))
                {
                    using (GZipStream gzipStream = new GZipStream(fsInput, CompressionMode.Decompress))
                    {
                        Byte[] buffer = new Byte[fsInput.Length];
                        int h;
                        while ((h = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOutput.Write(buffer, 0, h);
                        }
                    }
                }
            }
        }
    }
}
