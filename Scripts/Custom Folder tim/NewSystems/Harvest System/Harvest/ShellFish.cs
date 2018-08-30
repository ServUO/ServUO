using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class ShellFish : Item
    {
        [Constructable]
        public ShellFish() : base(Utility.RandomList(0xFC4, 0xFC5, 0xFC6, 0xFC7, 0xFC8, 0xFC9, 0xFCA, 0xFCB, 0xFCC ))
        {
            Name = "some shellfish";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it. 
            }
            else
            {
                int found = Utility.Random(19) + 1;
                from.SendMessage("You crack open the shellfish finding {0} black pearl{1}", found, found > 1 ? "s" : "");
                from.AddToBackpack(new BlackPearl(found));
                this.Delete(); // Deletes the shells
            } 
        }

        public ShellFish(Serial serial) : base(serial)
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