using MetadataConverter2.Converters;
using MetadataConverter2.Crypto;
using MetadataConverter2.Extensions;
using MetadataConverter2.Utils;
using System.Buffers.Binary;
using System.Security.Cryptography;
using static MetadataConverter2.IL2CPP.MhyIl2Cpp;

namespace MetadataConverter2.MetaTypes;
public record Blocks : MetaBase
{
    private readonly byte[] _initVector;
    private readonly bool _encrypted;

    public Blocks(MetaType type, byte[] initVector, double version) : base(type, version)
    {
        _initVector = initVector;
        _encrypted = _initVector.Length != 0;
    }
    public override void Convert(MemoryStream stream)
    {
        BlocksConverter.Convert(stream, Version);
    }

    public override void Decrypt(MemoryStream stream)
    {
        if (_encrypted)
        {
            Console.WriteLine("Decrypting blocks...");
            long metadataSize = DecryptBlocks(stream);
            Console.WriteLine("Decrypting stringLiterals...");
            DecryptStrings(stream);
            stream.SetLength(metadataSize);
        }
    }
    private long DecryptBlocks(MemoryStream stream)
    {
        BinaryStream bs = new(stream);
        long metadataSize = InitKeys(bs, out byte[]? iv);

        ulong blocksCount = bs.Length / 0x100;
        ulong blockSize = blocksCount - (blocksCount % 0x40);

        byte[] buffer = new byte[0x40];
        AES128_CBC decryptor = AES128_CBC.CreateDecryptor(iv);
        for (ulong pos = 0; pos < bs.Length; pos += blockSize)
        {
            bs.Position = pos;
            using CryptoStream cryptoStream = new(stream, decryptor, CryptoStreamMode.Read, true);
            _ = cryptoStream.Read(buffer);
            bs.Position = pos;
            bs.Write(buffer);
            decryptor.Reset();
        }

        bs.Position = 0;
        return metadataSize;
    }
    private void DecryptStrings(MemoryStream stream)
    {
        BinaryStream bs = new(stream)
        {
            Version = Version
        };
        GlobalMetadataHeader header = bs.ReadClass<GlobalMetadataHeader>();

        DecryptStringInfo(bs, header, out byte[]? key);

        bs.Position = 0x18;
        bs.Write(header.stringLiteralDataOffset);
        bs.Write(header.stringLiteralDataSize);
        bs.Write(header.stringLiteralOffset);

        bs.Position = 0xD8;
        bs.Write(header.stringOffset);
        bs.Write(header.stringSize);

        StringLiteral[] stringLiterals = bs.ReadMetadataClassArray<StringLiteral>(header.stringLiteralOffset, header.stringLiteralSize);
        for (int i = 0; i < stringLiterals.Length; i++)
        {
            StringLiteral stringLiteral = stringLiterals[i];
            uint offset = header.stringLiteralDataOffset + (uint)stringLiteral.dataIndex;

            if (offset + stringLiteral.length > stream.Length)
            {
                throw new IndexOutOfRangeException("String Offset Out of Bound");
            }

            bs.Position = offset;
            for (int j = 0; j < stringLiteral.length; j++)
            {
                byte b = bs.ReadByte();
                byte cl = key[(j + (key.Length / 4)) % key.Length];
                int al = key[(j % (key.Length / 2)) + (i % (key.Length / 2))] + j;

                bs.Position -= 1;
                bs.Write((byte)(b ^ cl ^ al));
            }
        }

        bs.Position = 0;
    }
    private long InitKeys(BinaryStream bs, out byte[] iv)
    {
        ulong blocksOffset = bs.Length - 0x4000;

        bs.Position = blocksOffset - 8;
        long metadataSize = bs.ReadInt64();
        if (metadataSize > (long)bs.Length)
        {
            throw new ArgumentException("Invalid Metadata Length");
        }

        bs.Position += 0xC8;
        uint signature = bs.ReadUInt32();
        if (signature != 0x2CFEFC2E)
        {
            throw new InvalidDataException("Invalid Metadata Signature");
        }

        bs.Position += 0x06;
        ushort keysOffset = bs.ReadUInt16();

        bs.Position = blocksOffset + keysOffset;

        iv = bs.ReadBytes(0x10);
        byte[] keys = bs.ReadBytes(0xB00);

        bs.Position = blocksOffset + 0x3000;

        byte[] ivKey = bs.ReadBytes(0x10);
        byte[] keysKey = bs.ReadBytes(0xB00);

        for (int i = 0; i < iv.Length; i++)
        {
            iv[i] ^= ivKey[i];
        }
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] ^= (byte)(keysKey[i] ^ iv[i % 0x10]);
        }

        for (int i = 0; i < iv.Length; i++)
        {
            iv[i] ^= _initVector[i];
        }
        for (int i = 0; i < AES128_CBC.ExpansionKey.Length; i++)
        {
            byte b = 0;
            for (int j = 0; j < 0x10; j++)
            {
                b ^= keys.AsSpan(i * 0x10, 0x10)[j];
            }
            AES128_CBC.ExpansionKey[i] = b;
        }

        return metadataSize;
    }
    private static void DecryptStringInfo(BinaryStream stream, GlobalMetadataHeader header, out byte[] key)
    {
        uint[] seedTable = new uint[]
        {
                header.seedPart10, header.seedPart11, header.seedPart12, header.seedPart13,
                header.seedPart20, header.seedPart21, header.seedPart22, header.seedPart23,
                header.seedPart30, header.seedPart31, header.seedPart32, header.seedPart33,
                header.seedPart40, header.seedPart41,
                header.seedPart50, header.seedPart51, header.seedPart52, header.seedPart53,
        };

        uint firstSeed = seedTable[0];
        uint lastSeed = seedTable[^1];

        ulong lowerSeed = seedTable[(lastSeed & 0x0F) + 0x02];
        ulong upperSeed = seedTable[firstSeed & 0x0F];

        ulong seed = (upperSeed << 0x20) | lowerSeed;
        MT19937_64 mt = new(seed);

        header.stringSize ^= (int)mt.Int64();
        header.stringOffset ^= (uint)mt.Int64();

        _ = mt.Int64();
        header.stringLiteralOffset ^= (uint)mt.Int64();
        header.stringLiteralDataSize ^= (int)mt.Int64();
        header.stringLiteralDataOffset ^= (uint)mt.Int64();

        key = new byte[0x5000];
        for (int i = 0; i < key.Length; i += 0x08)
        {
            BinaryPrimitives.WriteUInt64LittleEndian(key.AsSpan(i), mt.Int64());
        }
    }
}