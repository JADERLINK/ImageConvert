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

using System;
using System.Runtime.InteropServices.Marshalling;

namespace DdsFileTypePlus.Interop
{
    // This must be kept in sync with the Native structure in DirectXTexScratchImageData.Marshaller.cs
    // and the ScratchImageData type in DdsFileTypePlusIO.h
    [NativeMarshalling(typeof(Marshaller))]
    internal sealed unsafe partial class DirectXTexScratchImageData
    {
        public unsafe byte* Pixels { get; init; }

        public nuint Width { get; init; }

        public nuint Height { get; init; }

        public nuint Stride { get; init; }

        public nuint TotalImageDataSize { get; init; }

        public DXGI_FORMAT Format { get; init; }

        public unsafe T* AsPtr<T>() where T : unmanaged
        {
            return (T*)Pixels;
        }

    }
}
