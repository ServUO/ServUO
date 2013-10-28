//////////////////////////
/// Created by Mitty ////
//////////////////////// 

using System;

namespace Server.Items 
{
    public class NaxMarker : Item
    {
        [Constructable]
        public NaxMarker()
            : base(0x176B)
        {
            this.Weight = 0;
            this.Name = "Nax Marker";
            this.Hue = 0;
            this.LootType = LootType.Blessed;
            this.Movable = false;
            this.Visible = false;
        }

        public NaxMarker(Serial serial)
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