using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace __To_TGA_Compressed_From_PNG__TGASharpLib_
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("__To_TGA_Compressed_From_PNG__TGASharpLib_");
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
            if (Extension != ".PNG")
            {
                Console.WriteLine("Invalid file.");
                return;
            }

            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var outputFile = Path.Combine(diretory, name + ".TGA");

            Bitmap bitmap = new Bitmap(fileInfo.FullName);
            var tga = new TGASharpLib.TGA(bitmap, true);
            tga.Save(outputFile);
        }
    }
}
