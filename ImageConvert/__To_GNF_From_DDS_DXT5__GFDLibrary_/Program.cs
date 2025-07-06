using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace __To_GNF_From_DDS_DXT5__GFDLibrary_
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("__To_GNF_From_DDS_DXT5__GFDLibrary_");
            Console.WriteLine("Version 1.1");
            Console.WriteLine("");

            for (int i = 0; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        Action(args[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + args[i]);
                        Console.WriteLine(ex);
                    }
                }
            }

            Console.WriteLine("Finished!!!");
        }

        static void Action(string file)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();
            if (Extension != ".DDS")
            {
                Console.WriteLine("Invalid file.");
                return;
            }

            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var outputFile = Path.Combine(diretory, name + ".GNF");

            GFDLibrary.Textures.DDS.DDSStream stream = new GFDLibrary.Textures.DDS.DDSStream(fileInfo.FullName);
            if (! (stream.PixelFormat.FourCC == GFDLibrary.Textures.DDS.DDSPixelFormatFourCC.DXT5 || stream.PixelFormat.FourCC == GFDLibrary.Textures.DDS.DDSPixelFormatFourCC.DXT4))
            {
                stream.Close();
                throw new NotSupportedException(stream.PixelFormat.FourCC.ToString());
            }
            GFDLibrary.Textures.GNF.GNFTexture gnf = new GFDLibrary.Textures.GNF.GNFTexture(stream);
            gnf.Save(outputFile);
            stream.Close();
        }
    }
}
