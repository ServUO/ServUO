using System;
using Server.Gumps;

namespace Server.Items
{
    public class MelisandesHairDye : Item
    {
        [Constructable]
        public MelisandesHairDye()
            : base(0xEFF)
        {
            this.Hue = Utility.RandomMinMax(0x47E, 0x499);
        }

        public MelisandesHairDye(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041088;
            }
        }// Hair Dye
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                if (MondainsLegacy.CheckML(from))
                    from.SendGump(new ConfirmGump(this));
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1075085); // Requirement: Mondain's Legacy
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

        private class ConfirmGump : BaseConfirmGump
        {
            private readonly Item m_Item;
            public ConfirmGump(Item item)
                : base()
            {
                this.m_Item = item;
            }

            public override int TitleNumber
            {
                get
                {
                    return 1074395;
                }
            }// <div align=right>Use Permanent Hair Dye</div>
            public override int LabelNumber
            {
                get
                {
                    return 1074396;
                }
            }// This special hair dye is made of a unique mixture of leaves, permanently changing one's hair color until another dye is used.
            public override void Confirm(Mobile from)
            {
                if (this.m_Item != null && !this.m_Item.Deleted && this.m_Item.IsChildOf(from.Backpack))
                {
                    if (from.HairItemID != 0)
                    {
                        from.HairHue = this.m_Item.Hue;
                        from.PlaySound(0x240);
                        from.SendLocalizedMessage(502622); // You dye your hair.
                        this.m_Item.Delete();
                    }
                    else
                        from.SendLocalizedMessage(502623); // You have no hair to dye and you cannot use this.
                }
                else
                    from.SendLocalizedMessage(1073461); // You don't have enough dye.
            }

            public override void Refuse(Mobile from)
            {
                from.SendLocalizedMessage(502620); // You decide not to dye your hair.
            }
        }
    }
}