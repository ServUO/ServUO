using Server;
using System;
using Server.Targeting;

namespace Server.Items
{
    public enum HookType
    {
        None,
        Lava,
        Dredging,
        JunkProof
    }

    [Flipable(19270, 19271)]
    public class BaseFishingHook : Item
    {
        private int m_Uses;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Uses 
        {
            get { return m_Uses; } 
            set { m_Uses = value; } 
        }

        public virtual HookType HookType { get { return HookType.None; } }
        public override int LabelNumber { get { return 1098140; } }

        public BaseFishingHook() : this(50)
        {
        }

        public BaseFishingHook(int uses) : base(19270)
        {
            m_Uses = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int condition = GetCondition(m_Uses);
            list.Add(1149847, String.Format("#{0}", condition)); //Condition: ~1_val~
        }

        public static int GetCondition(int uses)
        {
            if (uses < 10)
                return 1149853; //worn
            else if (uses < 20)
                return 1149852; //fair
            else if (uses < 30)
                return 1149851; //very good
            else if (uses < 40)
                return 1149850; //good
            else if (uses < 50)
                return 1149849; //excellent
            else
                return 1149848; //new
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
                //TODO: Message?
            }
        }

        public static int GetHookType(HookType type)
        {
            switch (type)
            {
                case HookType.Lava:
                    return 1150888;
                case HookType.Dredging:
                    return 1150890;
                case HookType.JunkProof:
                    return 1150883;
            }
            return 0;
        }

        private class InternalTarget : Target
        {
            private BaseFishingHook m_Hook;

            public InternalTarget(BaseFishingHook hook)
                : base(-1, false, TargetFlags.None)
            {
                m_Hook = hook;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is FishingPole)
                {
                    FishingPole pole = (FishingPole)targeted;

                    if (pole.HookType != HookType.None)
                    {
                        Item hook = null;

                        switch (pole.HookType)
                        {
                            case HookType.None: break;
                            case HookType.Lava: hook = new LavaHook(pole.HookUses); break;
                            case HookType.Dredging: hook = new DredgingHook(pole.HookUses); break;
                            case HookType.JunkProof: hook = new JunkProofHook(pole.HookUses); break;
                        }

                        if (hook != null)
                            from.AddToBackpack(hook);
                    }

                    pole.HookType = m_Hook.HookType;
                    pole.HookUses = m_Hook.Uses;
                    pole.OriginalHue = pole.Hue;
                    pole.Hue = m_Hook.Hue;
                    from.SendLocalizedMessage(1150884); //You tie the hook to the fishing line.
                    m_Hook.Delete();
                }
            }
        }

        public BaseFishingHook(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Uses);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Uses = reader.ReadInt();
        }

    }

    [Flipable(19268, 19269)]
    public class LavaHook : BaseFishingHook
    {
        public override HookType HookType { get { return HookType.Lava; } }
        public override int LabelNumber { get { return 1150888; } }

        [Constructable]
        public LavaHook(int uses) : base(uses)
        {
            Hue = 2075;
        }

        [Constructable]
        public LavaHook()
        {
            Hue = 2075;
        }

        public LavaHook(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(19268, 19269)]
    public class DredgingHook : BaseFishingHook
    {
        public override HookType HookType { get { return HookType.Dredging; } }
        public override int LabelNumber { get { return 1150890; } }

        [Constructable]
        public DredgingHook(int uses) : base(uses)
        {
            Hue = 33;
        }

        [Constructable]
        public DredgingHook()
        {
            Hue = 33;
        }

        public DredgingHook(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(19268, 19269)]
    public class JunkProofHook : BaseFishingHook
    {
        public override HookType HookType { get { return HookType.JunkProof; } }
        public override int LabelNumber { get { return 1150883; } }

        [Constructable]
        public JunkProofHook(int uses) : base(uses)
        {
            Hue = 99;
        }

        [Constructable]
        public JunkProofHook()
        {
            Hue = 99;
        }

        public JunkProofHook(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}

