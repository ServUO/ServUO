using System;

namespace Server.Items
{
    public class LargeGlowingLadyBug : BaseLight
    {
        public override int LabelNumber => 1071400;  // Large Glowing Lady Bug

        public override int LitItemID => SouthFacing ? 0x2CFE : 0x2D00;

        public override int UnlitItemID => SouthFacing ? 0x2CFD : 0x2CFF;

        public bool SouthFacing => ItemID == 0x2CFD || ItemID == 0x2CFE;

        [Constructable]
        public LargeGlowingLadyBug()
            : base(0x2CFD)
        {
            Duration = TimeSpan.Zero; // Never burnt out
            Burning = false;
            Light = LightType.Circle225;
            Weight = 3.0;
        }

        public LargeGlowingLadyBug(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public void Flip()
        {
            switch (ItemID)
            {
                case 0x2CFD: ItemID = 0x2CFF; break;
                case 0x2CFE: ItemID = 0x2D00; break;
                case 0x2CFF: ItemID = 0x2CFF; break;
                case 0x2D00: ItemID = 0x2CFE; break;
            }
        }
    }
}