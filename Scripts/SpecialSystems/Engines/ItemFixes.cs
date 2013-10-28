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
        }
    }
}