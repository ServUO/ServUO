using Server.Mobiles;

namespace Server.Items
{
    public class MasonryBook : Item
    {
        public override int LabelNumber => 1153527;  // Making valuables with Stonecrafting

        [Constructable]
        public MasonryBook()
            : base(0xFBE)
        {
            Weight = 1.0;
        }

        public MasonryBook(Serial serial)
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
            else if (pm.Skills.Carpentry.Base < 100.0)
            {
                pm.SendLocalizedMessage(1080043); // Only a Grandmaster Carpenter can learn from this book.
            }
            else if (pm.Masonry)
            {
                pm.SendLocalizedMessage(1080066); // You have already learned this information.
            }
            else
            {
                pm.Masonry = true;
                pm.SendLocalizedMessage(1080044); // You have learned to make items from stone. You will need miners to gather stones for you to make these items.

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
