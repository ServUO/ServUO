﻿using Server.Network;
using System;

namespace Server.Items
{
    public class HeplerPaulsonMap : MapItem
    {
        [Constructable]
        public HeplerPaulsonMap()
        {
            //SetDisplay(1578, 1434, 1818, 1760, 400, 400, Map.Malas);
            Name = "Map To An Unknown Shipwreck";
            LootType = LootType.Blessed;
        }

        public override int LabelNumber
        {
            get
            {
                return 1015230;
            }
        }// local map

        public HeplerPaulsonMap(Serial serial) : base(serial)
        {
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1070722, "Quest Item");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                DisplayTo(from);
                from.PublicOverheadMessage(MessageType.Regular, 0x559, 1154270); // *You unfurl the map and study it carefully. You recognize Gravewater Lake. In the center of the lake is a large X*               
            }
            else
                from.SendLocalizedMessage(500446); // That is too far away.

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
