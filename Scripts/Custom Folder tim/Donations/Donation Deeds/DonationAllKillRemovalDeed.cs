using System;
using Server.Items;

namespace Server.Items
{
    public class AllKilldeedremoval : Item
    {
        [Constructable]
        public AllKilldeedremoval()
            : base(0x14F0)
        {
            Weight = 1.0;
            Name = "Removes All Murder Counts";
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Kills == 0)
            {
                from.SendMessage("Thou hast no need for this yet until thee is a true murderer");
            }
            else
            {
                from.Kills = 0;

                this.Delete();
            }
        }

        public AllKilldeedremoval(Serial serial)
            : base(serial)
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