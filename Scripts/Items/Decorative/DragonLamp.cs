using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.CraftableDragonLamp")]
    public class DragonLamp : BaseLight, IFlipable
    {
        public override int LitItemID { get { return ItemID == 0x4C4C ? 0x4C4D : 0x4C4F; } }
        public override int UnlitItemID { get { return ItemID == 0x4C4D ? 0x4C4C : 0x4C4E; } }

        public int NorthID { get { return Burning ? 0x4C4D : 0x4C4C; } }
        public int WestID { get { return Burning ? 0x4C4F : 0x4C4E; } }

        [Constructable]
        public DragonLamp()
            : base(0x4C4C)
        {
            Duration = Burnout ? TimeSpan.FromMinutes(60) : TimeSpan.Zero;
            Burning = false;
            Light = LightType.Circle225;
            Weight = 1.0;
        }

        public void OnFlip()
        {
            if (ItemID == NorthID)
                ItemID = WestID;
            else if (ItemID == WestID)
                ItemID = NorthID;
        }

        public DragonLamp(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}