using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public enum BinderType
    {
        None,
        StatScroll,
        PowerScroll,
        SOT
    }

    public class ScrollBinderDeed : Item
    {
        private BinderType m_BinderType;
        private SkillName m_Skill;
        private double m_Value;
        private int m_Needed;
        private double m_Has;

        [CommandProperty(AccessLevel.GameMaster)]
        public BinderType BinderType { get { return m_BinderType; } set { m_BinderType = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill { get { return m_Skill; } set { m_Skill = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Value { get { return m_Value; } set { m_Value = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Needed { get { return m_Needed; } set { m_Needed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Has { get { return m_Has; } set { m_Has = value; InvalidateProperties(); } }

        public override int LabelNumber => 1113135;  // Scroll Binder

        [Constructable]
        public ScrollBinderDeed()
            : base(0x14F0)
        {
            LootType = LootType.Cursed;
            Hue = 1636;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int v = (int)Value;

            switch (BinderType)
            {
                case BinderType.None: break;
                case BinderType.PowerScroll:
                    {
                        string skillName = SkillInfo.Table[(int)Skill].Name;

                        list.Add(1113149, string.Format("{0}\t{1}\t{2}\t{3}", v.ToString(), skillName, ((int)Has).ToString(), Needed.ToString())); // ~1_bonus~ ~2_type~: ~3_given~/~4_needed~
                        break;
                    }
                case BinderType.StatScroll:
                    {
                        list.Add(1113149, string.Format("+{0}\t#{1}\t{2}\t{3}", v - 225, 1049477, ((int)Has).ToString(), Needed.ToString())); // ~1_bonus~ ~2_type~: ~3_given~/~4_needed~
                        break;
                    }
                case BinderType.SOT:
                    {
                        string value = string.Format("{0:0.##}", Has);
                        string skillName = SkillInfo.Table[(int)Skill].Name;
                        int number;

                        if (Needed == 2)
                            number = 1113148; // ~1_type~ transcendence: ~2_given~/2.0
                        else
                            number = 1113620; // ~1_type~ transcendence: ~2_given~/5.0

                        list.Add(number, string.Format("{0}\t{1}", skillName, value));
                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);

                int cliloc;
                switch (BinderType)
                {
                    default:
                    case BinderType.None:
                        cliloc = 1113141; break; // Target the scroll you wish to bind.
                    case BinderType.PowerScroll:
                        cliloc = 1113138; break; // Target the powerscroll you wish to bind.
                    case BinderType.StatScroll:
                        cliloc = 1113140; break; // Target the stats scroll you wish to bind.
                    case BinderType.SOT:
                        cliloc = 1113139; break; // Target the scroll of transcendence you wish to bind.
                }

                from.SendLocalizedMessage(cliloc);
            }
        }

        public void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item && !((Item)targeted).IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                return;
            }

            switch (BinderType)
            {
                case BinderType.None:
                    {
                        if (targeted is PowerScroll)
                        {
                            PowerScroll ps = (PowerScroll)targeted;

                            if (ps.Value >= 120)
                            {
                                from.SendLocalizedMessage(1113144); // This scroll is already the highest of its type and cannot be bound.
                                return;
                            }

                            double value = ps.Value;
                            int needed = 0;

                            if (value == 105)
                                needed = 8;
                            else if (value == 110)
                                needed = 12;
                            else if (value == 115)
                                needed = 10;
                            else
                                return;

                            Value = value;
                            Needed = needed;
                            Has = 1;
                            Skill = ps.Skill;
                            BinderType = BinderType.PowerScroll;
                            ps.Delete();
                        }
                        else if (targeted is StatCapScroll)
                        {
                            StatCapScroll ss = (StatCapScroll)targeted;

                            if (ss.Value >= 250)
                            {
                                from.SendLocalizedMessage(1113144); //This scroll is already the highest of its type and cannot be bound.
                                return;
                            }

                            double value = ss.Value;
                            int needed = 0;

                            if (value == 230)
                                needed = 6;
                            else if (value == 235)
                                needed = 8;
                            else if (value == 240)
                                needed = 8;
                            else if (value == 245)
                                needed = 5;
                            else
                                return;

                            Value = value;
                            Needed = needed;
                            Has = 1;
                            BinderType = BinderType.StatScroll;
                            ss.Delete();
                        }
                        else if (targeted is ScrollOfTranscendence)
                        {
                            ScrollOfTranscendence sot = (ScrollOfTranscendence)targeted;

                            if (sot.Value >= 5.0)
                            {
                                from.SendLocalizedMessage(1113144); //This scroll is already the highest of its type and cannot be bound.
                                return;
                            }

                            Skill = sot.Skill;
                            BinderType = BinderType.SOT;
                            Needed = 2;
                            Has = sot.Value;
                            sot.Delete();
                        }
                        else
                        {
                            from.SendLocalizedMessage(1113142); // You may only bind powerscrolls, stats scrolls or scrolls of transcendence.
                        }

                        break;
                    }
                case BinderType.PowerScroll:
                    {
                        if (targeted is PowerScroll)
                        {
                            PowerScroll ps = (PowerScroll)targeted;

                            if (ps.Skill != Skill || ps.Value != Value)
                            {
                                from.SendLocalizedMessage(1113143); // This scroll does not match the type currently being bound.
                                return;
                            }

                            Has++;

                            if (Has >= Needed)
                            {
                                GiveItem(from, new PowerScroll(Skill, Value + 5));
                                from.SendLocalizedMessage(1113145); // You've completed your binding and received an upgraded version of your scroll!
                                ps.Delete();
                                Delete();
                            }
                            else
                            {
                                ps.Delete();
                            }
                        }
                        else if (targeted is ScrollBinderDeed)
                        {
                            ScrollBinderDeed sb = (ScrollBinderDeed)targeted;

                            if (sb == this)
                                return;

                            if (sb.BinderType != BinderType || sb.Value != Value || sb.Skill != Skill)
                            {
                                from.SendLocalizedMessage(1113143); // This scroll does not match the type currently being bound.
                                return;
                            }

                            Has += sb.Has;

                            double rest = Has - Needed;

                            if (Has >= Needed)
                            {
                                GiveItem(from, new PowerScroll(Skill, Value + 5));
                                from.SendLocalizedMessage(1113145); // You've completed your binding and received an upgraded version of your scroll!
                                Delete();
                            }

                            if (rest > 0)
                                sb.Has = rest;
                            else
                                sb.Delete();
                        }
                        break;
                    }
                case BinderType.StatScroll:
                    {
                        if (targeted is StatCapScroll)
                        {
                            StatCapScroll ss = (StatCapScroll)targeted;

                            if (ss.Value != Value)
                            {
                                from.SendLocalizedMessage(1113143); // This scroll does not match the type currently being bound.
                                return;
                            }

                            Has++;

                            if (Has >= Needed)
                            {
                                GiveItem(from, new StatCapScroll((int)Value + 5));
                                from.SendLocalizedMessage(1113145); // You've completed your binding and received an upgraded version of your scroll!
                                ss.Delete();
                                Delete();
                            }
                            else
                            {
                                ss.Delete();
                            }
                        }
                        else if (targeted is ScrollBinderDeed)
                        {
                            ScrollBinderDeed sb = (ScrollBinderDeed)targeted;

                            if (sb == this)
                                return;

                            if (sb.BinderType != BinderType || sb.Value != Value)
                            {
                                from.SendLocalizedMessage(1113143); // This scroll does not match the type currently being bound.
                                return;
                            }

                            Has += sb.Has;

                            double rest = Has - Needed;

                            if (Has >= Needed)
                            {
                                GiveItem(from, new StatCapScroll((int)Value + 5));
                                from.SendLocalizedMessage(1113145); // You've completed your binding and received an upgraded version of your scroll!
                                Delete();
                            }

                            if (rest > 0)
                                sb.Has = rest;
                            else
                                sb.Delete();
                        }
                        break;
                    }
                case BinderType.SOT:
                    {
                        if (targeted is ScrollOfTranscendence)
                        {
                            ScrollOfTranscendence sot = (ScrollOfTranscendence)targeted;

                            if (sot.Skill != Skill)
                            {
                                from.SendLocalizedMessage(1113143); // This scroll does not match the type currently being bound.
                                return;
                            }

                            if (sot.Value >= 5.0)
                            {
                                from.SendLocalizedMessage(1113144); // This scroll is already the highest of its type and cannot be bound.
                                return;
                            }

                            double newValue = sot.Value + Has;

                            if (newValue > 2 && Needed == 2)
                                Needed = 5;

                            if (newValue == Needed)
                            {
                                GiveItem(from, new ScrollOfTranscendence(Skill, Needed));
                                from.SendLocalizedMessage(1113145); // You've completed your binding and received an upgraded version of your scroll!
                                Delete();
                            }
                            else if (newValue > Needed)
                            {
                                from.SendGump(new BinderWarningGump(newValue, this, sot, Needed));
                            }
                            else
                            {
                                Has += sot.Value;
                                sot.Delete();
                            }
                        }
                        else if (targeted is ScrollBinderDeed)
                        {
                            ScrollBinderDeed sb = (ScrollBinderDeed)targeted;

                            if (sb == this)
                                return;

                            if (sb.BinderType != BinderType || sb.Skill != Skill)
                            {
                                from.SendLocalizedMessage(1113143); // This scroll does not match the type currently being bound.
                                return;
                            }

                            double newValue = sb.Has + Has;

                            if (newValue > 2 && Needed == 2)
                                Needed = 5;

                            Has = newValue;

                            double rest = Has - Needed;

                            if (Has >= Needed)
                            {
                                GiveItem(from, new ScrollOfTranscendence(Skill, Needed));
                                from.SendLocalizedMessage(1113145); // You've completed your binding and received an upgraded version of your scroll!
                                Delete();
                            }

                            if (rest > 0)
                            {
                                sb.Has = rest;
                            }
                            else
                            {
                                sb.Delete();
                            }
                        }
                        break;
                    }
            }
        }

        public void GiveItem(Mobile from, Item item)
        {
            Container pack = from.Backpack;

            if (pack == null || !pack.TryDropItem(from, item, false))
                item.MoveToWorld(from.Location, from.Map);
        }

        private class InternalTarget : Target
        {
            private readonly ScrollBinderDeed m_Binder;

            public InternalTarget(ScrollBinderDeed binder) : base(-1, false, TargetFlags.None)
            {
                m_Binder = binder;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Binder != null && !m_Binder.Deleted && m_Binder.IsChildOf(from.Backpack))
                    m_Binder.OnTarget(from, targeted);
            }
        }

        private class BinderWarningGump : Gump
        {
            private readonly double m_Value;
            private readonly int m_Needed;
            private readonly ScrollOfTranscendence m_Scroll;
            private readonly ScrollBinderDeed m_Binder;

            public BinderWarningGump(double value, ScrollBinderDeed binder, ScrollOfTranscendence scroll, int needed)
                : base(340, 340)
            {
                TypeID = 0x236C;

                m_Value = value;
                m_Needed = needed;
                m_Scroll = scroll;
                m_Binder = binder;

                AddPage(0);

                AddBackground(0, 0, 291, 99, 0x13BE);
                AddImageTiled(5, 6, 280, 20, 0xA40);

                AddHtmlLocalized(9, 8, 280, 20, 1113146, 0x7FFF, false, false); // Binding Scrolls of Transcendence
                AddImageTiled(5, 31, 280, 40, 0xA40);

                AddHtmlLocalized(9, 35, 272, 40, 1113147, 0x7FFF, false, false); // Binding this SoT will exceed the cap of 5, some points will be lost. Proceed?

                AddButton(215, 73, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(250, 75, 65, 20, 1006044, 0x7FFF, false, false); // OK

                AddButton(5, 73, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 75, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 1 && m_Scroll != null && m_Binder != null)
                {
                    m_Binder.GiveItem(from, new ScrollOfTranscendence(m_Scroll.Skill, m_Needed));
                    m_Scroll.Delete();
                    m_Binder.Delete();
                    from.SendLocalizedMessage(1113145); // You've completed your binding and received an upgraded version of your scroll!
                }
            }
        }

        public ScrollBinderDeed(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);

            writer.Write((int)m_BinderType);
            writer.Write((int)m_Skill);
            writer.Write(m_Value);
            writer.Write(m_Needed);
            writer.Write(m_Has);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if (v < 2)
            {
                m_Skill = (SkillName)reader.ReadInt();
                m_Value = reader.ReadDouble();
                m_Needed = (int)reader.ReadDouble();
                m_Has = reader.ReadDouble();

                switch (reader.ReadInt())
                {
                    case 0: m_BinderType = BinderType.None; break;
                    case 1: m_BinderType = BinderType.PowerScroll; break;
                    case 2: m_BinderType = BinderType.StatScroll; break;
                    case 3: m_BinderType = BinderType.SOT; break;
                }
            }
            else
            {
                m_BinderType = (BinderType)reader.ReadInt();
                m_Skill = (SkillName)reader.ReadInt();
                m_Value = reader.ReadDouble();
                m_Needed = reader.ReadInt();
                m_Has = reader.ReadDouble();
            }

            if (Hue != 1636)
                Hue = 1636;
        }
    }
}
