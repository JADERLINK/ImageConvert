using PaintDotNet;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PaintDotNet.PropertySystem;
using GFDLibrary.Textures.DDS;
using PaintDotNet.Collections;

namespace GnfFileType
{
    internal static class GnfWrite
    {
        public static void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback, IServiceProvider services)
        {
            try
            {
                //config dds
                IFileTypeInfo fileTypeInfo = DdsUtils.TryGetFileTypeInfo(services);
                FileType fileType = fileTypeInfo.GetInstance();

                var config = (PropertyBasedSaveConfigToken)fileType.CreateDefaultSaveConfigToken();

                var FileFormatProp = (StaticListChoiceProperty)config.GetProperty("FileFormat");
                var FileFormatType = FileFormatProp.DefaultValue.GetType();
                var indexBC3 = FileFormatProp.ValueChoices.IndexOf(Enum.Parse(FileFormatType, "BC3"));

                var newFileFormat = new StaticListChoiceProperty("FileFormat", FileFormatProp.ValueChoices, indexBC3, false);

                List<Property> props = new();
                List<PropertyCollectionRule> rules = new();
                props.Add(newFileFormat);

                foreach (var item in token.Properties.Properties)
                {
                    props.Add(item);
                }

                var CubeMap = config.GetProperty<BooleanProperty>("CubeMap");
                CubeMap.Value = false;
                props.Add(CubeMap);

                var GenerateMipMaps = config.GetProperty<BooleanProperty>("GenerateMipMaps");
                GenerateMipMaps.Value = false;
                props.Add(GenerateMipMaps);

                var BC7CompressionSpeed = config.GetProperty("BC7CompressionSpeed");
                var MipMapResamplingAlgorithm = config.GetProperty("MipMapResamplingAlgorithm");
                var UseGammaCorrection = config.GetProperty("UseGammaCorrection");
                props.Add(BC7CompressionSpeed);
                props.Add(MipMapResamplingAlgorithm);
                props.Add(UseGammaCorrection);

                PropertyBasedSaveConfigToken newToken = new PropertyBasedSaveConfigToken(new PropertyCollection(props, rules));

                //make dds/gnf

                MemoryStream ms = new MemoryStream();
                fileType.Save(input, ms, newToken, scratchSurface, progressCallback, true);
                ms.Position = 0;
                DDSStream ddsStream = new DDSStream(ms);
                GFDLibrary.Textures.GNF.GNFTexture gnf = new GFDLibrary.Textures.GNF.GNFTexture(ddsStream);
                gnf.Save(output);
                ddsStream.Close();
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
