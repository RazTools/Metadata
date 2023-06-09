﻿namespace MetadataConverter2.Crypto;
public static class CryptoHelper
{
    public static readonly byte[] GIInitVector = new byte[] { 0xAD, 0x2F, 0x42, 0x30, 0x67, 0x04, 0xB0, 0x9C, 0x9D, 0x2A, 0xC0, 0xBA, 0x0E, 0xBF, 0xA5, 0x68 };
    public static readonly byte[] BH3InitVector = new byte[] { 0x3F, 0x73, 0xA8, 0x5A, 0x08, 0x32, 0x0A, 0x33, 0x3C, 0xFA, 0x8D, 0x4E, 0x8B, 0x0C, 0x77, 0x45 };
    public static readonly byte[] SRInitVector = new byte[] { 0xF4, 0xB9, 0x54, 0x50, 0x85, 0x21, 0xB4, 0x14, 0x6C, 0x2F, 0xF1, 0xC2, 0x88, 0x9C, 0x79, 0xC4 };

    public static readonly byte[] MarkKey = new byte[] { 0x71, 0x98, 0xAA, 0xE6, 0xCE, 0x1B, 0x05, 0x4A, 0xE9, 0xFF, 0x45, 0x21, 0xC3, 0x38, 0x5E, 0x3C, 0x0F, 0xFB, 0xF5, 0xBB, 0xF6, 0x81, 0x48, 0x15, 0xFA, 0xD7, 0x77, 0x35, 0x82, 0x17, 0xD9, 0x9D, 0x56, 0x28, 0x2E, 0xA9, 0x51, 0xBA, 0x66, 0x2F, 0x22, 0xDD, 0xBB, 0x8A, 0x3B, 0xAD, 0x90, 0x63, 0xC6, 0x64, 0xFB, 0xD6, 0xCF, 0xA8, 0xBC, 0x48, 0x02, 0xC3, 0xBE, 0x36, 0xB2, 0x93, 0xBC, 0xD9 };
}