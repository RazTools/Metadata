using MetadataConverter2.Utils;
using static MetadataConverter2.Crypto.CryptoHelper;

namespace MetadataConverter2.MetaTypes
{
    public record Mark : MetaBase
    {
        private const int BlockSize = 0xA00;
        private const int ChunkSize = 0x264;
        private const int ChunkPadding = 4;

        private static readonly int BlockPadding = ((BlockSize / ChunkSize) + 1) * ChunkPadding;
        private static readonly int ChunkSizeWithPadding = ChunkSize + ChunkPadding;
        private static readonly int BlockSizeWithPadding = BlockSize + BlockPadding;

        public Mark(MetaType type, double version) : base(type, version) { }

        public override bool Convert(MemoryStream stream) => true;
        public override bool Decrypt(MemoryStream stream)
        {
            using BinaryStream bs = new(stream);
            string signature = bs.ReadStringToNull(0, 4);
            if (signature != "mark")
            {
                throw new InvalidDataException("Invalid Metadata Signature");
            }

            int index = 0;
            byte[] block = new byte[BlockSizeWithPadding];
            byte[] chunk = new byte[ChunkSizeWithPadding];
            MemoryStream dataStream = new();
            while (bs.Length != bs.Position)
            {
                int readBlockBytes = bs.Read(block, 0, block.Length);
                using MemoryStream blockStream = new(block, 0, readBlockBytes);
                while (blockStream.Length != blockStream.Position)
                {
                    int readChunkBytes = blockStream.Read(chunk);
                    if (readBlockBytes == BlockSizeWithPadding || readChunkBytes == ChunkSizeWithPadding)
                    {
                        readChunkBytes -= ChunkPadding;
                    }
                    for (int i = 0; i < readChunkBytes; i++)
                    {
                        chunk[i] ^= MarkKey[index++ % MarkKey.Length];
                    }
                    dataStream.Write(chunk, 0, readChunkBytes);
                }
            }

            stream.Position = 0;
            dataStream.Position = 0;
            dataStream.CopyTo(stream);
            stream.SetLength(dataStream.Length);
            return true;
        }
    }
}
