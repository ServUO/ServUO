using Server.Mobiles;

namespace Server.Items
{
    public class GlassblowingBook : Item
    {
        public override int LabelNumber => 1153528;  // Crafting glass with Glassblowing

        [Constructable]
        public GlassblowingBook()
            : base(0xFF4)
        {
            Weight = 5.0;
        }

        public GlassblowingBook(Serial serial)
            : base(serial)
        {
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
            else if (pm.Skills[SkillName.Alchemy].Base < 100.0)
            {
                pm.SendLocalizedMessage(1080042); // Only a Grandmaster Alchemist can learn from this book.
            }
            else if (pm.Glassblowing)
            {
                pm.SendLocalizedMessage(1080066); // You have already learned this information.
            }
            else
            {
                pm.Glassblowing = true;
                pm.SendLocalizedMessage(1080065); // You have learned to make items from glass.  You will need to find miners to mine fine sand for you to make these items.

                Delete();
            }
        }
    }
}