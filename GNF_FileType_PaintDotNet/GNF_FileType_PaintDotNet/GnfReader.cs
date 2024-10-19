using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaintDotNet;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System.IO;

namespace GnfFileType
{
    internal static class GnfReader
    {

        public static Document Load(Stream input, IServiceProvider services)
        {
            Document doc = null;
            try
            {
                GFDLibrary.Textures.GNF.GNFTexture gnf = new GFDLibrary.Textures.GNF.GNFTexture(input);
                MemoryStream ms = new MemoryStream();
                gnf.SaveToDDS(ms);

                IFileTypeInfo fileTypeInfo = DdsUtils.TryGetFileTypeInfo(services);
                FileType fileType = fileTypeInfo.GetInstance();
                ms.Position = 0;
                doc = fileType.Load(ms);
                ms.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return doc;
        }

    }

}
