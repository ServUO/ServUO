using System;
using Server.Mobiles;

namespace Server.Items
{
    public class SandMiningBook : Item
    {
        public override int LabelNumber { get { return 1153531; } } // Find Glass-Quality Sand

        [Constructable]
        public SandMiningBook()
            : base(0xFF4)
        {
            Weight = 2.0;
        }

        public SandMiningBook(Serial serial)
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

            if (Weight != 2.0)
            {
                Weight = 2.0;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (pm == null || from.Skills[SkillName.Mining].Base < 100.0)
            {
                pm.SendLocalizedMessage(1080041); // Only a Grandmaster Miner can learn from this book.
            }
            else if (pm.SandMining)
            {
                pm.SendLocalizedMessage(1080066); // You have already learned this information.
            }
            else
            {
                pm.SandMining = true;
                pm.SendLocalizedMessage(1111701); // You have learned how to mine fine sand.  Target sand areas when mining to look for fine sand.
                Delete();
            }
        }
    }
}