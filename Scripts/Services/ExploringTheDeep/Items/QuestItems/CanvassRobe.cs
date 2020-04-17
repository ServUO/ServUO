﻿using Server.Network;

namespace Server.Items
{
    public class CanvassRobe : Robe
    {
        public override int LabelNumber => 1154238;  // A Canvass Robe
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public CanvassRobe()
            : base()
        {
            this.Hue = 2720;
            this.LootType = LootType.Blessed;
            this.StrRequirement = 10;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154239); // *It is a thick canvass robe with specially sewn seals around the wrists and ankles. It appears as though it would protect its wearer from the harsh conditions of a deep aquatic environment.*
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public CanvassRobe(Serial serial)
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
