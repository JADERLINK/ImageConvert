////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-ddsfiletype-plus, a DDS FileType plugin
// for Paint.NET that adds support for the DX10 and later formats.
//
// Copyright (c) 2017-2024 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using DdsFileTypePlus.Interop;
using System;
using System.IO;
using System.Drawing;

namespace DdsFileTypePlus
{
    public static class DdsReader
    {
        public static unsafe Bitmap Load(Stream input)
        {
            Bitmap bitmap = null;

            try
            {
                using (DirectXTexScratchImage image = DdsNative.Load(input, out DDSLoadInfo info))
                {
                    if (info.IsTextureArray)
                    {
                        // Reject files containing a texture array because loading only the first item
                        // poses a data loss risk when saving.
                        throw new FormatException("DDS files containing a texture array are not supported.");
                    }
                    else if (info.VolumeMap && info.Depth > 1)
                    {
                        // Reject files containing a volume map with multiple slices because loading
                        // only the first item poses a data loss risk when saving.
                        throw new FormatException("DDS files containing a volume map are not supported.");
                    }

                    DirectXTexScratchImageData data = image.GetImageData(0, 0, 0);
                    int Width = (int)data.Width;
                    int Height = (int)data.Height;

                    var source = data.AsPtr<byte>();

                    byte[] Pixel = new byte[Width * Height * 4];

                    fixed (byte* p = Pixel)
                    {
                        for (var i = 0; i < Pixel.Length; i += 4)
                        {
                            p[i + 0] = source[i + 2];
                            p[i + 1] = source[i + 1];
                            p[i + 2] = source[i + 0];
                            p[i + 3] = source[i + 3];
                        }
                    }

                    bitmap = new Bitmap(Width, Height, Width * 4,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                    System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(Pixel, 0));
                }
            }
            catch (FormatException ex) when (ex.HResult == HResult.InvalidDdsFileSignature)
            {
                throw;
            }

            return bitmap;
        }

    }
}
