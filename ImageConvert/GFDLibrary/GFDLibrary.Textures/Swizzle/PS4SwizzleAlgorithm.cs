using System;
using System.IO;

namespace GFDLibrary.Textures.Swizzle
{
    // From RawTex by daemon1
    public class PS4SwizzleAlgorithm : ISwizzleAlgorithm
    {
        public SwizzleType Type => SwizzleType.PS4;

        public byte[] Swizzle( byte[] data, int width, int height, int blockSize )
        {
            return DoSwizzle( data, width, height, blockSize, false );
        }

        public byte[] UnSwizzle( byte[] data, int width, int height, int blockSize )
        {
            return DoSwizzle( data, width, height, blockSize, true );
        }

        private byte[] DoSwizzle( byte[] data, int width, int height, int blockSize, bool unswizzle )
        {
           
            int _heightT_4 = height / 4;
            int _widthT_4 = width / 4;
            int _heightR_4 = height % 4;
            int _widthR_4 = width % 4;
            _heightT_4 += _heightR_4 != 0 ? 1 : 0;
            _widthT_4 += _widthR_4 != 0 ? 1 : 0;

            int _heightT_32 = height / 32;
            int _widthT_32 = width / 32;
            int _heightR_32 = height % 32;
            int _widthR_32 = width % 32;
            _heightT_32 += _heightR_32 != 0 ? 1 : 0;
            _widthT_32 += _widthR_32 != 0 ? 1 : 0;

            MemoryStream processed = new MemoryStream();
            int heightTexels        = _heightT_4;
            int widthTexels         = _widthT_4;
            int heightTexelsAligned = _heightT_32;
            int widthTexelsAligned  = _widthT_32;
            int dataIndex           = 0;
            string errorMesage      = string.Empty;

            for ( int y = 0; y < heightTexelsAligned; ++y )
            {
                for ( int x = 0; x < widthTexelsAligned; ++x )
                {
                    for ( int t = 0; t < 64; ++t )
                    {
                        int pixelIndex = SwizzleUtilities.Morton( t, 8, 8 );
                        int num8       = pixelIndex / 8;
                        int num9       = pixelIndex % 8;
                        int yOffset    = ( y * 8 ) + num8;
                        int xOffset    = ( x * 8 ) + num9;

                        if ( xOffset < widthTexels && yOffset < heightTexels )
                        {
                            int destPixelIndex = yOffset * widthTexels + xOffset;
                            int destIndex      = blockSize * destPixelIndex;

                            try
                            {
                                if ( unswizzle )
                                {
                                    processed.Position = destIndex;
                                    processed.Write(data, dataIndex, blockSize);
                                }
                                else
                                {
                                    processed.Position = dataIndex;
                                    processed.Write(data, destIndex, blockSize);
                                }
                            }
                            catch ( Exception e)
                            {
                                errorMesage = e.Message;
                            }
                        }

                        dataIndex += blockSize;
                    }
                }
            }
            if ( errorMesage != string.Empty ) Console.WriteLine( errorMesage );

            return processed.ToArray();
        }
    }
}
