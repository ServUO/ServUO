using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Harvest;
using Server.Network;

namespace Server.Items
{
    public class FishingPole : Item
    {
        [Constructable]
        public FishingPole()
            : base(0x0DC0)
        {
            this.Layer = Layer.TwoHanded;
            this.Weight = 8.0;
        }

        public FishingPole(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            Point3D loc = this.GetWorldLocation();

            if (!from.InLOS(loc) || !from.InRange(loc, 2))
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that
            else
                Fishing.System.BeginHarvesting(from, this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            BaseHarvestTool.AddContextMenuEntries(from, this, list, Fishing.System);
        }

        public override bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
        {
            if (base.CheckConflictingLayer(m, item, layer))
                return true;

            if (layer == Layer.OneHanded)
            {
                m.SendLocalizedMessage(500214); // You already have something in both hands.
                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1 && this.Layer == Layer.OneHanded)
                this.Layer = Layer.TwoHanded;
        }
    }
}