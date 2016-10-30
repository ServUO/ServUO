using System;
using Server;
using Server.Engines.Craft;
using Server.Gumps;

namespace Server.Items
{
    public enum LenseType
    {
        None,
        Enhanced, //From blue medusa scales
        Regular,  //From grey medusa scales
        Limited   //From anyother type of scales
    }

    public class GorgonLense : Item, ICraftable
    {
        public override int LabelNumber { get { return 1112625; } } //  Gorgon Lense

        private LenseType m_LenseType;

        [CommandProperty(AccessLevel.GameMaster)]
        public LenseType LenseType { get { return m_LenseType; } set { m_LenseType = value; InvalidateProperties(); } }

        [Constructable]
        public GorgonLense() : this(1) { }

        [Constructable]
        public GorgonLense(int amount)
            : base(9905)
        {
            Stackable = true;
            Amount = amount;
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped is GorgonLense && ((GorgonLense)dropped).LenseType == m_LenseType)
                return base.StackWith(from, dropped, playSound);

            return false;
        }

        public override void OnAfterDuped(Item newItem)
        {
            if (newItem is GorgonLense)
                ((GorgonLense)newItem).LenseType = this.LenseType;

            base.OnAfterDuped(newItem);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && m_LenseType > LenseType.None)
            {
                from.Target = new InternalTarget(this);
                from.SendLocalizedMessage(1112596); //Which item do you wish to enhance with Gorgon Lenses?
            }
        }

        public void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                if (targeted is BaseArmor)
                {
                    BaseArmor armor = (BaseArmor)targeted;
                    if (armor.Layer == Layer.Neck || armor.Layer == Layer.Helm || armor is BaseShield || (armor.RequiredRace == Race.Gargoyle && armor.Layer== Layer.Earrings))
                    {
                        if (armor.GorgonLenseCharges > 0 && armor.GorgonLenseType != LenseType)
                            from.SendGump(new GorgonLenseWarningGump(this, armor));
                        else
                        {
                            armor.GorgonLenseCharges += Utility.RandomMinMax(28, 40);
                            armor.GorgonLenseType = LenseType;
                            from.SendLocalizedMessage(1112595); //You enhance the item with Gorgon Lenses!
                            Delete();
                        }
                    }
                    else
                        from.SendLocalizedMessage(1112594); //You cannot place gorgon lenses on this.
                }
                else if (targeted is BaseJewel)
                {
                    BaseJewel j = (BaseJewel)targeted;
                    if (j.Layer == Layer.Neck || j.Layer == Layer.Earrings)
                    {
                        if (j.GorgonLenseCharges > 0 && j.GorgonLenseType != LenseType)
                            from.SendGump(new GorgonLenseWarningGump(this, j));
                        else
                        {
                            j.GorgonLenseCharges += Utility.RandomMinMax(28, 40);
                            j.GorgonLenseType = LenseType;
                            from.SendLocalizedMessage(1112595); //You enhance the item with Gorgon Lenses!
                            Delete();
                        }
                    }
                    else
                        from.SendLocalizedMessage(1112594); //You cannot place gorgon lenses on this.
                }
                else if (targeted is BaseClothing)
                {
                    BaseClothing c = (BaseClothing)targeted;
                    if (c.Layer == Layer.Neck || c.Layer == Layer.Helm)
                    {
                        if (c.GorgonLenseCharges > 0 && c.GorgonLenseType != LenseType)
                            from.SendGump(new GorgonLenseWarningGump(this, c));
                        else
                        {
                            c.GorgonLenseCharges += Utility.RandomMinMax(28, 40);
                            c.GorgonLenseType = LenseType;
                            from.SendLocalizedMessage(1112595); //You enhance the item with Gorgon Lenses!
                            Delete();
                        }
                    }
                    else
                        from.SendLocalizedMessage(1112594); //You cannot place gorgon lenses on this.
                }
                else
                    from.SendLocalizedMessage(1112594); //You cannot place gorgon lenses on this.
            }
            else
                from.SendLocalizedMessage(1112594); //You cannot place gorgon lenses on this.
        }

        private class InternalTarget : Server.Targeting.Target
        {
            private GorgonLense m_Lense;

            public InternalTarget(GorgonLense lense) : base(-1, false, Server.Targeting.TargetFlags.None)
            {
                m_Lense = lense;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                m_Lense.OnTarget(from, targeted);
            }
        }

        public int OnCraft(int quality, bool markersMark, Mobile from, CraftSystem system, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            switch (resHue)
            {
                default:
                    m_LenseType = LenseType.Limited; break;
                case 2223:
                    m_LenseType = LenseType.Regular; break;
                case 1266:
                    m_LenseType = LenseType.Enhanced; break;
            }

            Hue = resHue;
            return quality;
        }

        public static int TotalCharges(Mobile m)
        {
            int charges = 0;
            m.Items.ForEach(i =>
            {
                if (i is BaseArmor)
                    charges += ((BaseArmor)i).GorgonLenseCharges;
                else if (i is BaseJewel)
                    charges += ((BaseJewel)i).GorgonLenseCharges;
                else if (i is BaseClothing)
                    charges += ((BaseClothing)i).GorgonLenseCharges;
            });

            return charges;
        }

        public GorgonLense(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
            writer.Write((int)m_LenseType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version >= 2)
                m_LenseType = (LenseType)reader.ReadInt();

            if (version == 0)
                reader.ReadInt();
        }
    }

    public class GorgonLenseWarningGump : BaseConfirmGump
    {
        public override int TitleNumber { get { return 1112597; } } // Replace active Gorgon Lenses
        public override int LabelNumber { get { return 1112598; } } // The remaining charges of the active lenses will be lost. Do you wish to proceed?

        private GorgonLense m_Lense;
        private Item m_Item;

        public GorgonLenseWarningGump(GorgonLense lense, Item item)
        {
            m_Lense = lense;
            m_Item = item;
        }

        public override void Confirm(Mobile from)
        {
            if (m_Item is BaseShield || m_Item.Layer == Layer.Neck || m_Item.Layer == Layer.Earrings || m_Item.Layer == Layer.Helm)
            {
                if (m_Item is BaseArmor)
                {
                    ((BaseArmor)m_Item).GorgonLenseCharges = m_Lense.Amount;
                    ((BaseArmor)m_Item).GorgonLenseType = m_Lense.LenseType;
                    from.SendLocalizedMessage(1112595); //You enhance the item with Gorgon Lenses!
                    m_Lense.Delete();
                }
                else if (m_Item is BaseJewel)
                {
                    ((BaseJewel)m_Item).GorgonLenseCharges = m_Lense.Amount;
                    ((BaseJewel)m_Item).GorgonLenseType = m_Lense.LenseType;
                    from.SendLocalizedMessage(1112595); //You enhance the item with Gorgon Lenses!
                    m_Lense.Delete();
                }
                else if (m_Item is BaseClothing)
                {
                    ((BaseClothing)m_Item).GorgonLenseCharges = m_Lense.Amount;
                    ((BaseClothing)m_Item).GorgonLenseType = m_Lense.LenseType;
                    from.SendLocalizedMessage(1112595); //You enhance the item with Gorgon Lenses!
                    m_Lense.Delete();
                }
            }
            else
                from.SendLocalizedMessage(1112594); //You cannot place gorgon lenses on this.
        }
    }
}