using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnfFileType
{
    internal static class DdsUtils
    {
        internal static IFileTypeInfo TryGetFileTypeInfo(IServiceProvider serviceProvider)
        {
            IFileTypesService? fileTypesService = serviceProvider?.GetService<IFileTypesService>();
            IFileTypeInfo? fileTypeInfo = fileTypesService?.FileTypes.FirstOrDefault(x => x.Type.Name == "DdsFileType");
            return fileTypeInfo;
        }
    }
}
