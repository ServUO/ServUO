//////////////////////////
/// Created by Mitty ////
//////////////////////// 

namespace Server.Items
{
    public class NaxMarker : Item
    {
        [Constructable]
        public NaxMarker()
            : base(0x176B)
        {
            Weight = 0;
            Name = "Nax Marker";
            Hue = 0;
            LootType = LootType.Blessed;
            Movable = false;
            Visible = false;
        }

        public NaxMarker(Serial serial)
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
    }
}