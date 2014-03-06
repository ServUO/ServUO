using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class ForgedPardon : Item
    {
        [Constructable]
        public ForgedPardon()
            : base(0x14EE)
        {
            Name = "Forged Pardon";
            Weight = 1.0;
            Hue = 1177;
        }

        public ForgedPardon(Serial serial)
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

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                if (from.Kills >= 1)
                {
                    from.Kills -= 1;
                    this.Delete();
                    from.SendMessage("You remove 1 Kills from your Character !");

                }
                else
                {
                    from.SendMessage("You can use this deed only if you have 1 Kills on your Character !!");

                }
            }
        }
    }
}