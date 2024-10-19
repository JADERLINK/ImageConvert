using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace __To_GNF_From_PNG__GFDLibrary__DdsFileTypePlus_.JSON
{
    internal static class ConfigFile
    {
        public static void WriteConfigFile(string filename, Config config) 
        {
            JObject entry = new JObject();
            entry["ErrorMetric"] = Enum.GetName(config.ErrorMetric);
            entry["ErrorDiffusionDithering"] = config.ErrorDiffusionDithering.ToString();

            JObject o = new JObject();
            o["GNF"] = entry;
            try
            {
                File.WriteAllText(filename, o.ToString());
            }
            catch (Exception)
            {
            }
        }

        public static Config ParseConfigFile(string filename) 
        {
            Config config = new Config();

            string json = File.ReadAllText(filename);
            JObject o = JObject.Parse(json);
            JObject oDDS = (JObject)o["GNF"];

            config.ErrorMetric = (DdsFileTypePlus.DdsErrorMetric)Enum.Parse(typeof(DdsFileTypePlus.DdsErrorMetric), oDDS["ErrorMetric"].ToString());
            config.ErrorDiffusionDithering = bool.Parse(oDDS["ErrorDiffusionDithering"].ToString());

            return config;
        }

    }
}
