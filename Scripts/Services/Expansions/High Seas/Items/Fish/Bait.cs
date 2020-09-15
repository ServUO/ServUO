using Server.Prompts;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class Bait : Item
    {
        public static readonly bool UsePrompt = true;

        private Type m_BaitType;
        private object m_Label;
        private int m_UsesRemaining;
        private int m_Index;
        private bool m_Enhanced;

        [CommandProperty(AccessLevel.GameMaster)]
        public Type BaitType => m_BaitType;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining { get { return m_UsesRemaining; } set { m_UsesRemaining = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Index
        {
            get { return m_Index; }
            set
            {
                m_Index = value;

                if (value < 0)
                    m_Index = 0;
                if (value >= FishInfo.FishInfos.Count)
                    m_Index = FishInfo.FishInfos.Count - 1;

                m_BaitType = FishInfo.GetTypeFromIndex(m_Index);
                Hue = FishInfo.GetFishHue(m_Index);
                m_Label = FishInfo.GetFishLabel(m_Index);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enhanced { get { return m_Enhanced; } set { m_Enhanced = value; InvalidateProperties(); } }

        [Constructable]
        public Bait(int index) : base(2454)
        {
            Index = index;
            m_UsesRemaining = 1;
        }

        [Constructable]
        public Bait() : base(2454)
        {
            m_UsesRemaining = 1;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (UsePrompt && m_UsesRemaining > 1)
                {
                    from.SendMessage("How much bait would you like to use?");
                    from.Prompt = new InternalPrompt(this);
                }
                else
                {
                    from.Target = new InternalTarget(this, 1);
                    from.SendMessage("Target the fishing pole or lobster trap that you would like to apply the bait to.");
                }
            }
        }

        public void TryBeginTarget(Mobile from, int amount)
        {
            if (amount < 0) amount = 1;
            if (amount > m_UsesRemaining) amount = m_UsesRemaining;

            from.Target = new InternalTarget(this, amount);
            from.SendMessage("Target the fishing pole or lobster trap that you would like to apply the bait to.");
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            object label = FishInfo.GetFishLabel(m_Index);

            if (m_Enhanced)
            {
                //~1_token~ ~2_token~ bait
                if (label is int)
                    list.Add(1116464, "#{0}\t#{1}", 1116470, (int)label);
                else if (label is string)
                    list.Add(1116464, "#{0}\t{1}", 1116470, (string)label);
            }
            else if (label is int)
                list.Add(1116465, string.Format("#{0}", (int)label)); //~1_token~ bait
            else if (label is string)
                list.Add(1116465, (string)label);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1116466, m_UsesRemaining.ToString());  //amount: ~1_val~
        }

        private class InternalPrompt : Prompt
        {
            private readonly Bait m_Bait;

            public InternalPrompt(Bait bait)
            {
                m_Bait = bait;
            }

            public override void OnResponse(Mobile from, string text)
            {
                int amount = Utility.ToInt32(text);
                m_Bait.TryBeginTarget(from, amount);
            }

            public override void OnCancel(Mobile from)
            {
                from.SendMessage("Not applying bait...");
            }
        }

        private class InternalTarget : Target
        {
            private readonly Bait m_Bait;
            private readonly int m_Amount;

            public InternalTarget(Bait bait, int amount)
                : base(0, false, TargetFlags.None)
            {
                m_Bait = bait;
                m_Amount = amount;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted == m_Bait)
                    return;

                if (targeted is FishingPole)
                {
                    if (!m_Bait.IsFishBait())
                    {
                        from.SendMessage("Think again before applying lobster or crab bait to a fishing pole!");
                        return;
                    }

                    FishingPole pole = (FishingPole)targeted;

                    bool hasBait = pole.BaitType != null;

                    if (hasBait && pole.BaitType != m_Bait.BaitType)
                        from.SendMessage("You swap out the old bait for new.");

                    if (pole.BaitType == m_Bait.BaitType)
                        pole.BaitUses += m_Amount;
                    else
                    {
                        pole.BaitType = m_Bait.BaitType;
                        pole.BaitUses += m_Amount;
                    }

                    if (m_Bait.Enhanced)
                        pole.EnhancedBait = true;

                    from.SendLocalizedMessage(1149759);  //You bait the hook.
                    m_Bait.UsesRemaining -= m_Amount;
                }
                else if (targeted is LobsterTrap)
                {
                    if (m_Bait.IsFishBait())
                    {
                        from.SendMessage("Think again before applying fish bait to a lobster trap!");
                        return;
                    }

                    LobsterTrap trap = (LobsterTrap)targeted;

                    bool hasBait = trap.BaitType != null;

                    trap.BaitType = m_Bait.BaitType;
                    //trap.Hue = m_Bait.Hue;

                    if (hasBait && trap.BaitType != m_Bait.BaitType)
                        from.SendMessage("You swap out the old bait for new.");

                    if (trap.BaitType == m_Bait.BaitType)
                        trap.BaitUses += m_Amount;
                    else
                    {
                        trap.BaitType = m_Bait.BaitType;
                        trap.BaitUses += m_Amount;
                    }

                    if (m_Bait.Enhanced)
                        trap.EnhancedBait = true;

                    from.SendLocalizedMessage(1149760); //You bait the trap.
                    m_Bait.UsesRemaining -= m_Amount;
                }
                else if (targeted is Bait && ((Bait)targeted).IsChildOf(from.Backpack) && ((Bait)targeted).BaitType == m_Bait.BaitType)
                {
                    Bait bait = (Bait)targeted;

                    bait.UsesRemaining += m_Amount;
                    m_Bait.UsesRemaining -= m_Amount;

                    if (m_Bait.UsesRemaining <= 0)
                    {
                        m_Bait.Delete();
                        from.SendLocalizedMessage(1116469); //You combine these baits into one cup and destroy the other cup.
                    }
                    else
                        from.SendMessage("You combine these baits into one cup.");

                    return;
                }

                if (m_Bait.UsesRemaining <= 0)
                {
                    m_Bait.Delete();
                    from.SendLocalizedMessage(1116467); //Your bait is used up so you destroy the container.
                }
            }
        }

        public bool IsFishBait()
        {
            if (m_BaitType == null)
                Index = Utility.RandomMinMax(0, 34);

            return !m_BaitType.IsSubclassOf(typeof(BaseCrabAndLobster));
        }

        public Bait(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_UsesRemaining);
            writer.Write(m_Index);
            writer.Write(m_Enhanced);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_UsesRemaining = reader.ReadInt();
            m_Index = reader.ReadInt();
            m_Enhanced = reader.ReadBool();

            if (m_Index < 0)
                m_Index = 0;
            if (m_Index >= FishInfo.FishInfos.Count)
                m_Index = FishInfo.FishInfos.Count - 1;

            m_BaitType = FishInfo.FishInfos[m_Index].Type;
            //Hue = FishInfo.FishInfos[m_Index].Hue;
            m_Label = FishInfo.GetFishLabel(m_Index);
        }
    }
}