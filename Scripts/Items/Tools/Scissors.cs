#region References
using Server.Engines.Craft;
using Server.Targeting;
using System;
#endregion

namespace Server.Items
{
    public interface IScissorable
    {
        bool Scissor(Mobile from, Scissors scissors);
    }

    [Flipable(0xf9f, 0xf9e)]
    public class Scissors : Item, ICraftable, IQuality, IUsesRemaining
    {
        private int m_UsesRemaining;
        private Mobile m_Crafter;
        private ItemQuality m_Quality;
        private bool m_ShowUsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining { get { return m_UsesRemaining; } set { m_UsesRemaining = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter { get { return m_Crafter; } set { m_Crafter = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining { get { return m_ShowUsesRemaining; } set { m_ShowUsesRemaining = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get { return m_Quality; }
            set
            {
                UnscaleUses();
                m_Quality = value;
                ScaleUses();
            }
        }

        public bool PlayerConstructed => false;

        [Constructable]
        public Scissors()
            : base(0xF9F)
        {
            Weight = 1.0;

            m_UsesRemaining = 50;

            if (Siege.SiegeShard)
                m_ShowUsesRemaining = true;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Quality == ItemQuality.Exceptional)
                list.Add(1060636); // exceptional
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            if (Siege.SiegeShard)
            {
                list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
            }
        }

        public Scissors(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write(m_ShowUsesRemaining);

            writer.Write(m_UsesRemaining);
            writer.Write(m_Crafter);
            writer.Write((int)m_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    m_ShowUsesRemaining = reader.ReadBool();
                    goto case 1;
                case 1:
                    m_UsesRemaining = reader.ReadInt();
                    m_Crafter = reader.ReadMobile();
                    m_Quality = (ItemQuality)reader.ReadInt();
                    break;
                case 0:
                    break;
            }

        }

        public void ScaleUses()
        {
            m_UsesRemaining = (m_UsesRemaining * GetUsesScalar()) / 100;
            InvalidateProperties();
        }

        public void UnscaleUses()
        {
            m_UsesRemaining = (m_UsesRemaining * 100) / GetUsesScalar();
        }

        public int GetUsesScalar()
        {
            if (m_Quality == ItemQuality.Exceptional)
                return 200;

            return 100;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(502434); // What should I use these scissors on?

            from.Target = new InternalTarget(this);
        }

        #region ICraftable Members
        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            return quality;
        }
        #endregion

        private class InternalTarget : Target
        {
            private readonly Scissors m_Item;

            public InternalTarget(Scissors item)
                : base(2, false, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Item.Deleted)
                {
                    return;
                }

                if (targeted == from)
                {
                    from.SendLocalizedMessage(1062845 + Utility.Random(3));
                    //"That doesn't seem like the smartest thing to do." / "That was an encounter you don't wish to repeat." / "Ha! You missed!"
                }
                else if (Utility.RandomDouble() > .20 && (from.Direction & Direction.Running) != 0 &&
                         (Core.TickCount - from.LastMoveTime) < from.ComputeMovementSpeed(from.Direction))
                {
                    from.SendLocalizedMessage(1063305); // Didn't your parents ever tell you not to run with scissors in your hand?!
                }
                else if (targeted is Item && !((Item)targeted).Movable)
                {
                    if (targeted is IScissorable && (targeted is PlagueBeastInnard || targeted is PlagueBeastMutationCore))
                    {
                        IScissorable obj = (IScissorable)targeted;

                        if (obj.Scissor(from, m_Item))
                        {
                            from.PlaySound(0x248);

                            if (Siege.SiegeShard)
                            {
                                Siege.CheckUsesRemaining(from, m_Item);
                            }
                        }
                    }
                }
                else if (targeted is IScissorable)
                {
                    IScissorable obj = (IScissorable)targeted;

                    if (obj.Scissor(from, m_Item))
                    {
                        from.PlaySound(0x248);

                        if (Siege.SiegeShard)
                        {
                            Siege.CheckUsesRemaining(from, m_Item);
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
                }
            }

            protected override void OnNonlocalTarget(Mobile from, object targeted)
            {
                if (targeted is IScissorable && (targeted is PlagueBeastInnard || targeted is PlagueBeastMutationCore))
                {
                    IScissorable obj = (IScissorable)targeted;

                    if (obj.Scissor(from, m_Item))
                    {
                        from.PlaySound(0x248);
                    }
                }
                else
                {
                    base.OnNonlocalTarget(from, targeted);
                }
            }
        }
    }
}
