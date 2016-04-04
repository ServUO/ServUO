using System;
using Server.Engines.Craft;
using Server.Mobiles;

namespace Server.Items
{
    public class BasketweavingPliers : BaseTool
    {
        [Constructable]
        public BasketweavingPliers()
            : base(0x0FBB)
        {
            this.Hue = 2208;
            this.Weight = 1.0;
        }

        [Constructable]
        public BasketweavingPliers(int uses)
            : base(uses, 0x1028)
        {
            this.Weight = 1.0;
            this.Hue = 2208;
        }

        public BasketweavingPliers(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefBasketweaving.CraftSystem;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1112245;
            }
        }// Basket Weaving Pliers
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

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (pm.BasketWeaving)
            {
                pm.SendMessage("Pick a basket to make from the gump.");
            }
            else if (pm.BasketWeaving == false)
            {
                pm.SendMessage("You need to read a book, and learn Basketweaving in order to use this tool.");
                return;
            }

            base.OnDoubleClick(from);
        }
    }
}