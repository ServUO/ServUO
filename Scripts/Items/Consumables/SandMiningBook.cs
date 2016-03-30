using System;
using Server.Mobiles;

namespace Server.Items
{
    public class SandMiningBook : Item
    {
        [Constructable]
        public SandMiningBook()
            : base(0xFF4)
        {
            this.Weight = 1.0;
        }

        public SandMiningBook(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Find Glass-Quality Sand";
            }
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

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (pm == null || from.Skills[SkillName.Mining].Base < 100.0)
            {
                pm.SendMessage("Only a Grandmaster Miner can learn from this book.");
            }
            else if (pm.SandMining)
            {
                pm.SendMessage("You have already learned this information.");
            }
            else
            {
                pm.SandMining = true;
                pm.SendMessage("You have learned how to mine fine sand. Target sand areas when mining to look for fine sand.");
                this.Delete();
            }
        }
    }
}