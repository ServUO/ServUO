/****************************************
* NAME    : White Stone Wall           *
* SCRIPT  : WhiteStoneWall.cs          *
* VERSION : v1.00                      *
* CREATOR : Mans Sjoberg (Allmight)    *
* CREATED : 10-07.2002                 *
* **************************************/

namespace Server.Items
{
    public enum WhiteStoneWallTypes
    {
        EastWall,
        SouthWall,
        SECorner,
        NWCornerPost,
        EastArrowLoop,
        SouthArrowLoop,
        EastWindow,
        SouthWindow,
        SouthWallMedium,
        EastWallMedium,
        SECornerMedium,
        NWCornerPostMedium,
        SouthWallShort,
        EastWallShort,
        SECornerShort,
        NWCornerPostShort,
        NECornerPostShort,
        SWCornerPostShort,
        SouthWallVShort,
        EastWallVShort,
        SECornerVShort,
        NWCornerPostVShort,
        SECornerArch,
        SouthArch,
        WestArch,
        EastArch,
        NorthArch,
        EastBattlement,
        SECornerBattlement,
        SouthBattlement,
        NECornerBattlement,
        SWCornerBattlement,
        Column,
        SouthWallVVShort,
        EastWallVVShort
    }

    public class WhiteStoneWall : BaseWall
    {
        [Constructable]
        public WhiteStoneWall(WhiteStoneWallTypes type)
            : base(0x0057 + (int)type)
        {
        }

        public WhiteStoneWall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}