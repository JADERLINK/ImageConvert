using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace __From_GNF_To_PNG__ScarletLibrary_
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("__From_GNF_To_PNG__ScarletLibrary_");
            Console.WriteLine("Version 1.0");
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

            var gnf = new Scarlet.IO.ImageFormats.GNF();
            gnf.Open(fileInfo.FullName, Scarlet.IO.Endian.LittleEndian);
            var bitmap = gnf.GetBitmap(0, 0);
            bitmap.Save(outputFile, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
