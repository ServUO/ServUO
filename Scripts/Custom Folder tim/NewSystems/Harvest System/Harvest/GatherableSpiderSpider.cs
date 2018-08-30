using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class GatherableSpiderWeb : Item
    {
        [Constructable]
        public GatherableSpiderWeb()  : base(Utility.RandomList(0xEE3, 0xEE4, 0xEE5, 0xEE6))
        {
            Movable = false;
            Name = "a spider web";
        }

        public override void OnDoubleClick(Mobile from)
        {
            int found = Utility.Random(19) + 1;

            if (from.InRange(this.GetWorldLocation(), 2))
            {
                from.SendMessage("You manage to salvage {0} spiders silk", found);
                from.AddToBackpack(new SpidersSilk(found));
                this.Delete(); // Deletes the spider web
            }
            else
            {
                from.SendMessage("Your not close enough to get anything.");
            }
        }

        public GatherableSpiderWeb(Serial serial) : base(serial)
        {
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