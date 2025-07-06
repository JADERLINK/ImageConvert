using System;
using System.IO;

namespace __From_GNF_To_PNG__GFDLibrary__DdsFileTypePlus_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("__From_GNF_To_PNG__GFDLibrary__DdsFileTypePlus_");
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
            if (Extension != ".GNF")
            {
                Console.WriteLine("Invalid file.");
                return;
            }

            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var outputFile = Path.Combine(diretory, name + ".PNG");

            GFDLibrary.Textures.GNF.GNFTexture gnf = new GFDLibrary.Textures.GNF.GNFTexture(fileInfo.FullName);
            MemoryStream ms = new MemoryStream();
            gnf.SaveToDDS(ms);
            ms.Position = 0;
            var bitmap = DdsFileTypePlus.DdsReader.Load(ms);
            ms.Close();
            bitmap.Save(outputFile, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
