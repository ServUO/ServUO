using Server.Mobiles;

namespace Server.Items
{
    public class BasketWeavingBook : Item
    {
        public override int LabelNumber => 1153529;  // Making valuables with Basket Weaving

        [Constructable]
        public BasketWeavingBook()
            : base(0xFBE)
        {
            Weight = 1.0;
        }

        public BasketWeavingBook(Serial serial)
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
            else if (pm.Skills[SkillName.Tinkering].Base < 100.0)
            {
                pm.SendLocalizedMessage(1112255); // Only a Grandmaster Tinker can learn from this book.
            }
            else if (pm.BasketWeaving)
            {
                pm.SendLocalizedMessage(1080066); // You have already learned this information.
            }
            else
            {
                pm.BasketWeaving = true;
                pm.SendLocalizedMessage(1112254); // You have learned to make baskets. You will need gardeners to make reeds out of plants for you to make these items.

                Delete();
            }
        }
    }
}