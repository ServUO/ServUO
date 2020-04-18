using System;

namespace Server.Items
{
    public class HeatingStand : BaseLight
    {
        [Constructable]
        public HeatingStand()
            : base(0x1849)
        {
            if (Burnout)
                Duration = TimeSpan.FromMinutes(25);
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Empty;
            Weight = 1.0;
        }

        public HeatingStand(Serial serial)
            : base(serial)
        {
        }

        public override int LitItemID => 0x184A;
        public override int UnlitItemID => 0x1849;
        public override void Ignite()
        {
            base.Ignite();

            if (ItemID == LitItemID)
                Light = LightType.Circle150;
            else if (ItemID == UnlitItemID)
                Light = LightType.Empty;
        }

        public override void Douse()
        {
            base.Douse();

            if (ItemID == LitItemID)
                Light = LightType.Circle150;
            else if (ItemID == UnlitItemID)
                Light = LightType.Empty;
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
    }
}