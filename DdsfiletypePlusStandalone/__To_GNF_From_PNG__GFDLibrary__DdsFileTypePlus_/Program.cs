using System;
using System.Drawing;
using System.IO;

namespace __To_GNF_From_PNG__GFDLibrary__DdsFileTypePlus_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("__To_GNF_From_PNG__GFDLibrary__DdsFileTypePlus_");
            Console.WriteLine("Version 1.0");
            Console.WriteLine("");

            JSON.Config config = null;

            try
            {
                string JsonFileName = Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, "GNF.JSON");

                if (!File.Exists(JsonFileName))
                {
                    config = new JSON.Config();
                    config.ErrorDiffusionDithering = false;
                    config.ErrorMetric = DdsFileTypePlus.DdsErrorMetric.Uniform;
                    JSON.ConfigFile.WriteConfigFile(JsonFileName, config);
                }
                else
                {
                    config = JSON.ConfigFile.ParseConfigFile(JsonFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ");
                Console.WriteLine(ex);
            }

            if (config != null)
            {
                Console.WriteLine("ErrorMetric: " + Enum.GetName(config.ErrorMetric));
                Console.WriteLine("ErrorDiffusionDithering: " + config.ErrorDiffusionDithering.ToString());
                Console.WriteLine("");

                for (int i = 0; i < args.Length; i++)
                {
                    if (File.Exists(args[i]))
                    {
                        try
                        {
                            Action(args[i], config);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + args[i]);
                            Console.WriteLine(ex);
                        }
                    }
                }
            }

            Console.WriteLine("Finished!!!");
        }

        static void Action(string file, JSON.Config config)
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
            var outputFile = Path.Combine(diretory, name + ".GNF");

            Bitmap bitmap = new Bitmap(fileInfo.FullName);
            var output = new FileInfo(outputFile).Create();

            MemoryStream ms = new MemoryStream();
            DdsFileTypePlus.DdsWriter.Save(ms, DdsFileTypePlus.DdsFileFormat.BC3, config.ErrorDiffusionDithering, config.ErrorMetric, bitmap);
            ms.Position = 0;
            GFDLibrary.Textures.DDS.DDSStream ddsStream = new GFDLibrary.Textures.DDS.DDSStream(ms);
            GFDLibrary.Textures.GNF.GNFTexture gnf = new GFDLibrary.Textures.GNF.GNFTexture(ddsStream);
            gnf.Save(output);
            ms.Close();
            output.Close();
        }
    }
}
