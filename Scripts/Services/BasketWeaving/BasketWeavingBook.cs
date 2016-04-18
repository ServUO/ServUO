using System;
using Server.Mobiles;

namespace Server.Items
{
    public class BasketWeavingBook : Item
    {
        [Constructable]
        public BasketWeavingBook()
            : base(0xFBE)
        {
            this.Weight = 1.0;
        }

        public BasketWeavingBook(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Making Valuables With Basket Weaving";
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
            else if (pm == null || from.Skills[SkillName.Tinkering].Base < 100.0)
            {
                pm.SendMessage("Only a Grandmaster Tinkerer can learn from this book.");
            }
            else if (pm.BasketWeaving)
            {
                pm.SendMessage("You have already learned this information.");
            }
            else
            {
                pm.BasketWeaving = true;
                pm.SendMessage("You have learned to make baskets. You will need gardeners to make reeds out of plants for you to make these items.");
                this.Delete();
            }
        }
    }
}