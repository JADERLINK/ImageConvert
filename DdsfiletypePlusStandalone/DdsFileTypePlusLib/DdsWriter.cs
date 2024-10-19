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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace DdsFileTypePlus
{
    public static class DdsWriter
    {
        public static void Save(
            Stream output,
            DdsFileFormat format,
            bool errorDiffusionDithering,
            DdsErrorMetric errorMetric,
            Bitmap scratchSurface)
        {
            int width = scratchSurface.Width;
            int height = scratchSurface.Height;
            int arraySize = 1;
            int mipLevels = 1; //always 1
            bool cubeMap = false; // always false
            BC7CompressionSpeed compressionSpeed = BC7CompressionSpeed.Fast; //not used

            using (DirectXTexScratchImage textures = GetTextures(scratchSurface, width, height, arraySize, mipLevels, format, cubeMap))
            {
                if (format == DdsFileFormat.R8G8B8X8 || format == DdsFileFormat.B8G8R8)
                {
                    throw new InvalidOperationException($"{nameof(DdsFileFormat)}.{format} not implemented.");
                }
                else
                {
                    DdsProgressCallback ddsProgress = (double progressPercentage) => {return true;};

                    (DXGI_FORMAT saveFormat, DdsFileOptions fileOptions) = GetSaveFormat(format);

                    DDSSaveInfo info = new()
                    {
                        Format = saveFormat,
                        FileOptions = fileOptions,
                        ErrorMetric = errorMetric,
                        CompressionSpeed = compressionSpeed,
                        ErrorDiffusionDithering = errorDiffusionDithering
                    };

                    if (format == DdsFileFormat.BC6HUnsigned || format == DdsFileFormat.BC7 || format == DdsFileFormat.BC7Srgb)
                    {
                        throw new InvalidOperationException($"{nameof(DdsFileFormat)}.{format} not implemented.");
                    }
                    else
                    {
                        DdsNative.Save(info, textures, output, directComputeAdapter: IntPtr.Zero, ddsProgress);
                    }
                }
            }
        }


        private static (DXGI_FORMAT, DdsFileOptions) GetSaveFormat(DdsFileFormat format)
        {
            DXGI_FORMAT dxgiFormat;
            DdsFileOptions options = DdsFileOptions.None;

            switch (format)
            {
                case DdsFileFormat.BC1:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
                    break;
                case DdsFileFormat.BC1Srgb:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB;
                    break;
                case DdsFileFormat.BC2:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM;
                    break;
                case DdsFileFormat.BC2Srgb:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB;
                    break;
                case DdsFileFormat.BC3:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
                    break;
                case DdsFileFormat.BC3Srgb:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB;
                    break;
                case DdsFileFormat.BC4Unsigned:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
                    break;
                case DdsFileFormat.BC5Unsigned:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
                    break;
                case DdsFileFormat.BC5Signed:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM;
                    break;
                case DdsFileFormat.BC6HUnsigned:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16;
                    break;
                case DdsFileFormat.BC7:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM;
                    break;
                case DdsFileFormat.BC7Srgb:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB;
                    break;
                case DdsFileFormat.B8G8R8A8:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                    break;
                case DdsFileFormat.B8G8R8A8Srgb:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
                    break;
                case DdsFileFormat.B8G8R8X8:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM;
                    break;
                case DdsFileFormat.B8G8R8X8Srgb:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB;
                    break;
                case DdsFileFormat.R8G8B8A8:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                    break;
                case DdsFileFormat.R8G8B8A8Srgb:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB;
                    break;
                case DdsFileFormat.B5G5R5A1:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM;
                    break;
                case DdsFileFormat.B4G4R4A4:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM;
                    break;
                case DdsFileFormat.B5G6R5:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM;
                    break;
                case DdsFileFormat.R8Unsigned:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8_UNORM;
                    break;
                case DdsFileFormat.R8G8Unsigned:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM;
                    break;
                case DdsFileFormat.R8G8Signed:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8_SNORM;
                    break;
                case DdsFileFormat.R32Float:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT;
                    break;
                case DdsFileFormat.BC4Ati1:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
                    // DirectXTex normally uses the BC4U four-character-code when saving DX9 compatible
                    // DDS files, ForceLegacyDX9Formats makes it use the ATI1 four-character-code.
                    options = DdsFileOptions.ForceLegacyDX9Formats;
                    break;
                case DdsFileFormat.BC5Ati2:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
                    // DirectXTex normally uses the BC5U four-character-code when saving DX9 compatible
                    // DDS files, ForceLegacyDX9Formats makes it use the ATI2 four-character-code.
                    options = DdsFileOptions.ForceLegacyDX9Formats;
                    break;
                case DdsFileFormat.BC3Rxgb:
                    dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
                    options = DdsFileOptions.ForceBC3ToRXGB;
                    break;
                case DdsFileFormat.R8G8B8X8:
                case DdsFileFormat.B8G8R8:
                default:
                    throw new InvalidOperationException($"{nameof(DdsFileFormat)}.{format} does not map to a DXGI format.");

            }

            return (dxgiFormat, options);
        }

  
        private static DirectXTexScratchImage GetTextures(Bitmap scratchSurface,
                                                          int width,
                                                          int height,
                                                          int arraySize,
                                                          int mipLevels,
                                                          DdsFileFormat format,
                                                          bool cubeMap)
        {
            DirectXTexScratchImage image = null;
            DirectXTexScratchImage tempImage = null;

            try
            {
                DXGI_FORMAT dxgiFormat;
#pragma warning disable IDE0066 // Convert switch statement to expression
                switch (format)
                {
                    case DdsFileFormat.B8G8R8X8Srgb:
                    case DdsFileFormat.BC1Srgb:
                    case DdsFileFormat.BC2Srgb:
                    case DdsFileFormat.BC3Srgb:
                    case DdsFileFormat.BC7Srgb:
                    case DdsFileFormat.R8G8B8A8Srgb:
                        dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB;
                        break;
                    case DdsFileFormat.B8G8R8A8:
                    // R8G8B8X8 and B8G8R8 are legacy DirectX 9 formats that DXGI does not support.
                    // See DX9DdsWriter for the writer implementation.
                    case DdsFileFormat.R8G8B8X8:
                    case DdsFileFormat.B8G8R8:
                        dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                        break;
                    case DdsFileFormat.B8G8R8A8Srgb:
                        dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
                        break;
                    case DdsFileFormat.BC1:
                    case DdsFileFormat.BC2:
                    case DdsFileFormat.BC3:
                    case DdsFileFormat.BC4Unsigned:
                    case DdsFileFormat.BC5Unsigned:
                    case DdsFileFormat.BC5Signed:
                    case DdsFileFormat.BC6HUnsigned:
                    case DdsFileFormat.BC7:
                    case DdsFileFormat.B8G8R8X8:
                    case DdsFileFormat.R8G8B8A8:
                    case DdsFileFormat.B5G5R5A1:
                    case DdsFileFormat.B4G4R4A4:
                    case DdsFileFormat.B5G6R5:
                    case DdsFileFormat.R8Unsigned:
                    case DdsFileFormat.R8G8Unsigned:
                    case DdsFileFormat.R8G8Signed:
                    case DdsFileFormat.R32Float:
                    case DdsFileFormat.BC3Rxgb:
                    case DdsFileFormat.BC4Ati1:
                    case DdsFileFormat.BC5Ati2:
                        dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                        break;
                    default:
                        throw new InvalidOperationException($"{nameof(DdsFileFormat)}.{format} does not map to a DXGI format.");
                }
#pragma warning restore IDE0066 // Convert switch statement to expression

                tempImage = new DirectXTexScratchImage(width,
                                                       height,
                                                       arraySize,
                                                       mipLevels,
                                                       dxgiFormat,
                                                       cubeMap);

                RenderToDirectXTexScratchImage(scratchSurface, tempImage.GetImageData(0, 0, 0), format);


                image = tempImage;
                tempImage = null;
            }
            finally
            {
                tempImage?.Dispose();
            }

            return image;
        }

      

        private static unsafe void RenderToDirectXTexScratchImage(Bitmap surface, DirectXTexScratchImageData scratchImage, DdsFileFormat format)
        {
          
            switch (scratchImage.Format)
            {
                case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB:
                    throw new InvalidOperationException($"Unsupported {nameof(DXGI_FORMAT)} value: {scratchImage.Format}.");
                case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
                    if (TryGetSwizzledImageFormat(format, out SwizzledImageFormat swizzledImageFormat))
                    {
                        throw new InvalidOperationException($"Unsupported {nameof(DXGI_FORMAT)} value: {scratchImage.Format}.");
                    }
                    else
                    {
                        nint length = surface.Height * surface.Width * 4;
                        Rectangle rect = new Rectangle(0, 0, surface.Width, surface.Height);
                        BitmapData bmpData = surface.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                        IntPtr ptr = bmpData.Scan0;
                        byte* ptrbyte = (byte*)ptr;

                        byte* dest = scratchImage.AsPtr<byte>();

                        for (var i = 0; i < length; i += 4)
                        {
                            dest[i + 0] = ptrbyte[i + 2];
                            dest[i + 1] = ptrbyte[i + 1];
                            dest[i + 2] = ptrbyte[i + 0];
                            dest[i + 3] = ptrbyte[i + 3];
                        }

                        surface.UnlockBits(bmpData);
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported {nameof(DXGI_FORMAT)} value: {scratchImage.Format}.");
            }

            static bool TryGetSwizzledImageFormat(DdsFileFormat format, out SwizzledImageFormat swizzledImageFormat)
            {
                if (format == DdsFileFormat.BC3Rxgb)
                {
                    swizzledImageFormat = SwizzledImageFormat.Xgbr;
                    return true;
                }

                swizzledImageFormat = SwizzledImageFormat.Unknown;
                return false;
            }
        }

    }
}
