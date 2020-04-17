using Server.Mobiles;

namespace Server.Items
{
    public class StoneMiningBook : Item
    {
        public override int LabelNumber => 1153530;  // Mining For Quality Stone

        [Constructable]
        public StoneMiningBook()
            : base(0xFBE)
        {
            Weight = 1.0;
        }

        public StoneMiningBook(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
            {
                return;
            }

            if (!IsChildOf(pm.Backpack))
            {
                pm.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (pm.Skills[SkillName.Mining].Base < 100.0)
            {
                pm.SendLocalizedMessage(1080041); // Only a Grandmaster Miner can learn from this book.
            }
            else if (pm.StoneMining)
            {
                pm.SendLocalizedMessage(1080066); // You have already learned this information.
            }
            else
            {
                pm.StoneMining = true;
                pm.SendLocalizedMessage(1080045); // You have learned to mine for stones.  Target mountains when mining to find stones.

                Delete();
            }
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
