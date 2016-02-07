using System;

namespace Server.Items
{
    public enum ThinStoneWallTypes
    {
        Corner,
        EastWall,
        SouthWall,
        CornerPost,
        EastDoorFrame,
        SouthDoorFrame,
        NorthDoorFrame,
        WestDoorFrame,
        SouthWindow,
        EastWindow,
        CornerMedium,
        SouthWallMedium,
        EastWallMedium,
        CornerPostMedium,
        CornerArch,
        EastArch,
        SouthArch,
        NorthArch,
        WestArch,
        CornerShort,
        EastWallShort,
        SouthWallShort,
        CornerPostShort,
        SouthWallShort2,
        EastWallShort2
    }

    public class ThinStoneWall : BaseWall
    {
        [Constructable]
        public ThinStoneWall(ThinStoneWallTypes type)
            : base(0x001A + (int)type)
        {
        }

        public ThinStoneWall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}