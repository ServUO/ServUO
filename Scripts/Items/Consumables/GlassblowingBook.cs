using System;
using Server.Mobiles;

namespace Server.Items
{
    public class GlassblowingBook : Item
    {
        public override int LabelNumber { get { return 1153528; } } // Crafting glass with Glassblowing

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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Weight != 5.0)
            {
                Weight = 5.0;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (pm == null || from.Skills[SkillName.Alchemy].Base < 100.0)
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