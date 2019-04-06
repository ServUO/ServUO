using System;

namespace Server.Misc
{
    public static class ItemFixes
    {
        public static void Initialize()
        {
            // Missing NoShoot flags
            TileData.ItemTable[0x2A0].Flags |= TileFlag.NoShoot;
            TileData.ItemTable[0x3E0].Flags |= TileFlag.NoShoot;
            TileData.ItemTable[0x3E1].Flags |= TileFlag.NoShoot;

            // Incorrect height
            TileData.ItemTable[0x34D2].Height = 0;

            TileData.ItemTable[0x1910].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x1911].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x1912].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x1913].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x1918].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x1919].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x191A].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x191B].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x191C].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x191D].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x191E].Flags ^= TileFlag.Wall;
            TileData.ItemTable[0x191F].Flags ^= TileFlag.Wall;

            TileData.ItemTable[0x1910].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x1911].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x1912].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x1913].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x1918].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x1919].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x191A].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x191B].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x191C].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x191D].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x191E].Flags |= TileFlag.Surface;
            TileData.ItemTable[0x191F].Flags |= TileFlag.Surface;
        }
    }
}