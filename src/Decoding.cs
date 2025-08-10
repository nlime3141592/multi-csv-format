using System;
using System.Collections.Generic;

namespace nl
{
    public static class Decoding
    {
        public static char[] ToUnicodeFromUtf8(byte[] utf8)
        {
            List<char> chars = new List<char>(utf8.Length);

            int i = 0;

            while (i < utf8.Length)
            {
                int length = 0;
                uint concatBytes = 0;

                if (utf8[i] >= 248)
                {
                    // decoding error
                    break;
                }
                else if (utf8[i] >= 240)
                {
                    concatBytes += (uint)(utf8[i] & 0x07);
                    length = 4;
                }
                else if (utf8[i] >= 224)
                {
                    concatBytes += (uint)(utf8[i] & 0x0F);
                    length = 3;
                }
                else if (utf8[i] >= 192)
                {
                    concatBytes += (uint)(utf8[i] & 0x1F);
                    length = 2;
                }
                else
                {
                    concatBytes += (uint)(utf8[i] & 0x7F);
                    length = 1;
                }

                if (i + length > utf8.Length)
                    break;

                for (int j = i + 1; j < i + length; ++j)
                {
                    if ((utf8[j] & 0xC0) != 0x80)
                        break;

                    concatBytes <<= 6;
                    concatBytes += (uint)(utf8[j] & 0x3F);
                }

                chars.Add((char)(concatBytes & 0xFFFF));

                if (concatBytes > 0xFFFF)
                {
                    chars.Add((char)(concatBytes >> 16));
                }

                i += length;
            }

            return chars.ToArray();
        }

        public static char[] ToUnicodeFromUtf16Le(byte[] utf16le)
        {
            List<char> chars = new List<char>(utf16le.Length);
            int i = 0;

            while (i + 1 < utf16le.Length)
            {
                uint firstByte = (uint)(utf16le[i + 1] & 0xFC);
                int length = 0;
                uint concatBytes = 0;

                if (firstByte == 0xDC)
                {
                    // decoding error
                    break;
                }
                else if (firstByte == 0xD8)
                {
                    // U+010000-U+10FFFF
                    // utf-16 length is 4
                    length = 4;

                    if (i + length > utf16le.Length)
                        break;

                    uint thirdByte = (uint)(utf16le[i + 3] & 0xFC);

                    if (thirdByte != 0xDC)
                    {
                        // decoding error
                        break;
                    }

                    concatBytes = (uint)(utf16le[i + 1]) << 8;
                    concatBytes += (uint)(utf16le[i]);
                    concatBytes = (concatBytes - 0xD800) << 8;
                    concatBytes += (uint)(utf16le[i + 3]) << 8;
                    concatBytes += (uint)(utf16le[i + 2]);
                    concatBytes -= 0xDC00;

                    chars.Add((char)(concatBytes >> 16));
                    chars.Add((char)(concatBytes & 0xFFFF));

                    i += length;
                }
                else
                {
                    // U+0000-U+D7FF or U+E000-U+FFFF
                    // utf-16 length is 2
                    length = 2;

                    if (i + length > utf16le.Length)
                        break;

                    concatBytes = (uint)(utf16le[i + 1]) << 8;
                    concatBytes += (uint)(utf16le[i]);

                    chars.Add((char)concatBytes);

                    i += length;
                }
            }

            return chars.ToArray();
        }
        
        public static char[] ToUnicodeFromUtf16Be(byte[] utf16be)
        {
            List<char> chars = new List<char>(utf16be.Length);
            int i = 0;

            while (i < utf16be.Length)
            {
                uint firstByte = (uint)(utf16be[i] & 0xFC);
                int length = 0;
                uint concatBytes = 0;

                if (firstByte == 0xDC)
                {
                    // decoding error
                    break;
                }
                else if (firstByte == 0xD8)
                {
                    // U+010000-U+10FFFF
                    // utf-16 length is 4
                    length = 4;

                    if (i + length > utf16be.Length)
                        break;

                    uint thirdByte = (uint)(utf16be[i + 2] & 0xFC);

                    if (thirdByte != 0xDC)
                    {
                        // decoding error
                        break;
                    }

                    concatBytes = (uint)(utf16be[i]) << 8;
                    concatBytes += (uint)(utf16be[i + 1]);
                    concatBytes = (concatBytes - 0xD800) << 8;
                    concatBytes += (uint)(utf16be[i + 2]) << 8;
                    concatBytes += (uint)(utf16be[i + 3]);
                    concatBytes -= 0xDC00;

                    chars.Add((char)(concatBytes >> 16));
                    chars.Add((char)(concatBytes & 0xFFFF));

                    i += length;
                }
                else
                {
                    // U+0000-U+D7FF or U+E000-U+FFFF
                    // utf-16 length is 2
                    length = 2;

                    if (i + length > utf16be.Length)
                        break;

                    concatBytes = (uint)(utf16be[i]) << 8;
                    concatBytes += (uint)(utf16be[i + 1]);

                    chars.Add((char)concatBytes);

                    i += length;
                }
            }

            return chars.ToArray();
        }
    }
}