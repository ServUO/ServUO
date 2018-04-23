using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Server.ContextMenus;
using Server.Network;
using Server.Regions;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public enum MagicBallState { Idle, Following, Remaining, Acting }

    public class AMagicCrystalBall : Item, TranslocationItem, IRewardItem
    {
        private const bool m_NeedSkillToFollow = false;
        private const bool m_NeedSecondSkillToFollow = false;
        private const bool m_NeedSkillToRemain = false;
        private const bool m_NeedSkillToAct = false;

        private SkillNameValue m_FollowSkill = new SkillNameValue(SkillName.Magery, 70);
        private SkillNameValue m_FollowSecondSkill = new SkillNameValue(SkillName.Magery, 50);
        private SkillNameValue m_RemainSkill = new SkillNameValue(SkillName.Focus, 50);
        private SkillNameValue m_ActionSkill = new SkillNameValue(SkillName.Meditation, 70);
        private SkillName m_ResistSkill = SkillName.MagicResist;

        private Point3D m_UserLocation;
        private Map m_UserMap;

        private MagicBallState m_State;
        private int m_Charges;
        private int m_Recharges;

        private Mobile m_Subject;
        private string m_SubjectName;

        private Mobile m_CurrentUser;

        public Mobile CurrentUser { get { return m_CurrentUser; } set { m_CurrentUser = value; } }

        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }
                
        public bool NeedSkillToFollow { get { return m_NeedSkillToFollow; } }
        public bool NeedSecondSkillToFollow { get { return m_NeedSecondSkillToFollow; } }
        public bool NeedSkillToRemain { get { return m_NeedSkillToRemain; } }
        public bool NeedSkillToAct { get { return m_NeedSkillToAct; } }

        public Point3D UserLocation { get { return m_UserLocation; } set { m_UserLocation = value; } }
        public Map UserMap { get { return m_UserMap; } set { m_UserMap = value; } }
        public MagicBallState State { get { return m_State; } set { m_State = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillNameValue FollowSkill { get { return m_FollowSkill; } set { m_FollowSkill = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillNameValue FollowSecondSkill { get { return m_FollowSecondSkill; } set { m_FollowSecondSkill = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillNameValue RemainSkill { get { return m_RemainSkill; } set { m_RemainSkill = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillNameValue ActionSkill { get { return m_ActionSkill; } set { m_ActionSkill = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName ResistSkill { get { return m_ResistSkill; } set { m_ResistSkill = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set
            {
                if (value > this.MaxCharges)
                    m_Charges = this.MaxCharges;
                else if (value < 0)
                    m_Charges = 0;
                else
                    m_Charges = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Recharges
        {
            get { return m_Recharges; }
            set
            {
                if (value > this.MaxRecharges)
                    m_Recharges = this.MaxRecharges;
                else if (value < 0)
                    m_Recharges = 0;
                else
                    m_Recharges = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges { get { return 50; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRecharges { get { return 255; } }

        public string TranslocationItemName { get { return "magic tracking ball"; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Subject
        {
            get
            {
                if (m_Subject != null && m_Subject.Deleted)
                {
                    m_Subject = null;
                    InternalUpdateSubjectName();
                }

                return m_Subject;
            }
            set
            {
                m_Subject = value;
                InternalUpdateSubjectName();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string SubjectName { get { return m_SubjectName; } }

        [Constructable]
        public AMagicCrystalBall()
            : base(0xE2E)
        {
            Weight = 10.0;
            Light = LightType.Circle150;

            m_Charges = Utility.RandomMinMax(3, 9);
            m_State = MagicBallState.Idle;

            m_SubjectName = "";
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("Magic Tracking Ball: [charges: {0}] [subject: {1}]",
                m_Charges.ToString(), m_SubjectName == "" ? "(not set)" : m_SubjectName);
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Magic Tracking Ball: [charges: {0}] [subject: {1}]",
                m_Charges.ToString(), m_SubjectName == "" ? "(not set)" : m_SubjectName);
        }

        private delegate void BallCallback(Mobile from);

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
               from.SendMessage( "This does not belong to you!!" );
               return;
            }

            if (from.Alive && from.InRange(this.GetWorldLocation(), 2))
            {
                if (Subject == null)
                {
                    list.Add(new BallEntry(new BallCallback(ConnectSubject), 5119));
                }
                else
                {
                    list.Add(new BallEntry(new BallCallback(FollowSubject), 6108));
                    list.Add(new BallEntry(new BallCallback(DisconnectSubject), 5120));
                }
            }
        }

        private class BallEntry : ContextMenuEntry
        {
            private BallCallback m_Callback;

            public BallEntry(BallCallback callback, int number)
                : base(number, 2)
            {
                m_Callback = callback;
            }

            public override void OnClick()
            {
            
                Mobile from = Owner.From;
                
                if (from.CheckAlive())
                    m_Callback(from);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage( "This does not belong to you!!" );
                return;
            }

            if (from.InRange(this.GetWorldLocation(), 2))
            {
                if (Subject == null) { ConnectSubject(from); }
                else { FollowSubject(from); }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public void ConnectSubject(Mobile from)
        {
            Mobile subject = this.Subject;

            if (Deleted)
                return;
            else if (subject != null)
            {
                from.SendMessage("Please disconnect the previous subject first.");
                return;
            }

            from.SendMessage("Target the subject you wish to follow.");
            from.Target = new SubjectLinkTarget(this);
        }

        private class SubjectLinkTarget : Target
        {
            private AMagicCrystalBall m_Ball;

            public SubjectLinkTarget(AMagicCrystalBall ball)
                : base(-1, false, TargetFlags.None)
            {
                m_Ball = ball;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Ball.Deleted || m_Ball.Subject != null)
                    return;

                if (!from.InRange(m_Ball.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
                else if (targeted is Mobile)
                {
                    Mobile subject = (Mobile)targeted;

                    if (subject.Map == Map.Internal || subject.Deleted)
                    {
                        from.SendMessage("Your subject cannot be found!");
                    }
                    else if (subject.AccessLevel > from.AccessLevel)
                    {
                        from.SendMessage("Your subject cannot be followed by you.");
                    }
                    else if (subject is BaseCreature && ((BaseCreature)subject).Summoned && ((BaseCreature)subject).SummonMaster != from)
                    {
                        from.SendMessage("You cannot follow a summoned creature that you did not summon.");
                    }
                    else
                    {
                        from.SendMessage("You have successfully connected the Tracking Ball to your subject.");

                        m_Ball.Subject = subject;
                    }
                }
                else
                {
                    from.SendMessage("You cannot follow that.");
                }
            }
        }

        public void FollowSubject(Mobile from)
        {
            Mobile subject = this.Subject;

            if (Deleted || subject == null)
                return;

            if (Charges == 0)
            {
                SendLocalizedMessageTo(from, 1054122); // The Crystal Ball darkens. It must be charged before it can be used again.
            }
            else if (!from.Alive || from.Hidden)
            {
                from.SendMessage("The Tracking Ball fills with an orange mist. You must be alive and unhidden to begin following.");
            }
            else if (m_State != MagicBallState.Idle)
            {
                from.SendMessage("The Tracking Ball shimmers. It is not ready to be used yet.");
            }
            else if (subject.Map == Map.Internal || subject.Deleted)
            {
                from.SendMessage("The Tracking Ball fills with a purple mist. Your subject cannot be found!");
            }
            else if (subject.AccessLevel > from.AccessLevel)
            {
                from.SendMessage("The Tracking Ball fills with a yellow mist. Your subject cannot be followed by you.");
            }
            else if (subject is BaseCreature && ((BaseCreature)subject).Summoned && ((BaseCreature)subject).SummonMaster != from)
            {
                from.SendMessage("The Tracking Ball vibrates. You cannot follow a summoned creature that you did not summon.");
            }
            else if (NeedSkillToFollow && !from.CheckSkill(FollowSkill.Name, FollowSkill.Value, 120))
            {
                from.SendMessage("The Tracking Ball darkens. You lacked enough {0} for the attempt. Try again.", FollowSkill.Name);
            }
            else if (NeedSecondSkillToFollow && !from.CheckSkill(FollowSecondSkill.Name, FollowSecondSkill.Value, 120))
            {
                from.SendMessage("The Tracking Ball darkens. You lacked enough {0} for the attempt. Try again.", FollowSkill.Name);
            }
            else
            {
                Charges--;

                double duration = 10.0;
                duration += 2.0 * ((from.Skills[RemainSkill.Name].Value - subject.Skills[ResistSkill].Value) / 10);
                if (duration < 2.0) duration = 2.0;

                UserLocation = from.Location;
                UserMap = from.Map;
                from.Hidden = true;
                from.Paralyzed = true;
                State = MagicBallState.Following;
                CurrentUser = from;

                CountDown = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(2.0), (int)(duration / 2),
                    new TimerStateCallback(Tick), new object[] { from, duration });

                Timer t = new MagicBallIdleTimer(from, this, TimeSpan.FromSeconds(duration));
                t.Start();

                from.MoveToWorld(subject.Location, subject.Map);
                from.SendMessage("The Tracking Ball fills with a green mist. You begin following your subject.");
            }
        }

        private Timer CountDown;

        public void Tick(object o)
        {
            object[] param = (object[])o;
            Mobile from = (Mobile)param[0];
            int timeleft = (int)((double)param[1]);
            from.CloseGump(typeof(FollowerGump));
            if (timeleft > 0)
            {
                if (from.Hidden)
                {
                    if ((State == MagicBallState.Remaining || State == MagicBallState.Acting)
                        && NeedSkillToRemain && !from.CheckSkill(RemainSkill.Name, RemainSkill.Value, 120))
                    {
                        from.SendMessage("You lacked enough {0} to move with your subject.", RemainSkill.Name.ToString());
                    }
                    else
                        from.MoveToWorld(Subject.Location, Subject.Map);
                    if (State != MagicBallState.Acting) State = MagicBallState.Remaining;
                    from.SendGump(new FollowerGump(this, timeleft));
                    param[1] = (double)(timeleft - 2);
                }
                else
                {
                    from.SendMessage("You may have been detected by your subject!");
                    from.MoveToWorld(UserLocation, UserMap);
                    from.SendMessage("It will take another {0} seconds for the effects of the enchantment to wear off.", timeleft);
                    return;
                }
            }
        }

        public void StopFollowing(Mobile from)
        {
            from.Location = UserLocation;
            from.Map = UserMap;
            from.Hidden = false;
            from.Paralyzed = false;
            from.CloseGump(typeof(FollowerGump));
            from.CloseGump(typeof(ActionGump));
            from.SendMessage("The mist fades from the Tracking Ball. You have stopped following your subject.");

            State = MagicBallState.Idle;
            CurrentUser = null;
        }

        public void DisconnectSubject(Mobile from)
        {
            if (!Deleted && Subject != null)
            {
                Subject = null;

                from.SendMessage("The Tracking Ball is no longer linked to a subject.");
            }
        }

        private void InternalUpdateSubjectName()
        {
            Mobile subject = this.Subject;

            if (subject == null)
                m_SubjectName = "";
            else
                m_SubjectName = subject.Name;

            InvalidateProperties();
        }

        public AMagicCrystalBall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)1); // version
            writer.Write((bool)m_IsRewardItem);
            writer.Write((Mobile)m_CurrentUser);

            writer.Write((Point3D)m_UserLocation);
            writer.Write((Map)m_UserMap);

            writer.Write((int)m_FollowSkill.Name);
            writer.Write((int)m_FollowSkill.Value);
            writer.Write((int)m_FollowSecondSkill.Name);
            writer.Write((int)m_FollowSecondSkill.Value);
            writer.Write((int)m_RemainSkill.Name);
            writer.Write((int)m_RemainSkill.Value);
            writer.Write((int)m_ActionSkill.Name);
            writer.Write((int)m_ActionSkill.Value);

            writer.WriteEncodedInt((int)m_Recharges);
            writer.WriteEncodedInt((int)m_Charges);

            writer.Write((Mobile)this.Subject);
            writer.Write((string)m_SubjectName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    {
                        m_IsRewardItem = reader.ReadBool();
                        m_CurrentUser = reader.ReadMobile();
                        goto case 0;
                    }
                case 0:
                    {
                        m_UserLocation = reader.ReadPoint3D();
                        m_UserMap = reader.ReadMap();

                        m_FollowSkill = new SkillNameValue((SkillName)reader.ReadInt(), reader.ReadInt());
                        m_FollowSecondSkill = new SkillNameValue((SkillName)reader.ReadInt(), reader.ReadInt());
                        m_RemainSkill = new SkillNameValue((SkillName)reader.ReadInt(), reader.ReadInt());
                        m_ActionSkill = new SkillNameValue((SkillName)reader.ReadInt(), reader.ReadInt());

                        m_Recharges = reader.ReadEncodedInt();
                        m_Charges = Math.Min(reader.ReadEncodedInt(), MaxCharges);

                        this.Subject = (Mobile)reader.ReadMobile();
                        m_SubjectName = reader.ReadString();

                        m_State = MagicBallState.Idle;

                        break;
                    }
            }
            if (m_CurrentUser != null)
            {
                Mobile undo = m_CurrentUser;
                undo.Location = UserLocation;
                undo.Map = UserMap;
                undo.Hidden = false;
                undo.Paralyzed = false;
            }
        }
    }

    public class FollowerGump : Gump
    {
        private AMagicCrystalBall m_Ball;

        public FollowerGump(AMagicCrystalBall ball, int duration)
            : base(40, 40)
        {
            Dragable = false;
            Closable = false;
            Resizable = false;

            m_Ball = ball;
            AddPage(0);
            AddBackground(0, 0, 400, 40, 9200);
            if (m_Ball.State == MagicBallState.Following || m_Ball.State == MagicBallState.Remaining)
                AddButton(5, 9, 2020, 2019, 1, GumpButtonType.Reply, 0);
            AddLabel(77, 10, 0, string.Format("You have {0} seconds to view your subject.", duration.ToString()));
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (info.ButtonID == 1)
            {
                m_Ball.State = MagicBallState.Acting;
                from.SendGump(new ActionGump(m_Ball, from));
            }
        }
    }

    public class ActionGump : Gump
    {
        private AMagicCrystalBall m_Ball;

        public ActionGump(AMagicCrystalBall ball, Mobile from)
            : base(40, 82)
        {
            from.CloseGump(typeof(ActionGump));

            Dragable = false;
            Closable = false;
            Resizable = false;

            m_Ball = ball;
            AddPage(0);
            AddBackground(0, 0, 400, 40, 9200);
            AddButton(5, 9, 2020, 2019, 1, GumpButtonType.Reply, 0);
            AddTextEntry(77, 10, 304, 20, 0, 1, "Type message here.");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (info.ButtonID > 0)
            {
                if (info.ButtonID == 1)
                {
                    if (m_Ball.NeedSkillToAct && !from.CheckSkill(m_Ball.ActionSkill.Name, m_Ball.ActionSkill.Value, 120))
                    {
                        from.SendMessage("You lacked enough {0} to do that.", m_Ball.ActionSkill.Name.ToString());
                    }
                    else
                    {
                        from.SendMessage("Your message was sent.");
                        Mobile subject = m_Ball.Subject;
                        if (subject != null && !subject.Deleted)
                            subject.SendMessage("** You hear a message from an unknown source: \"{0}\" **", info.GetTextEntry(1).Text);
                    }
                }
            }
            if (m_Ball.State == MagicBallState.Acting) m_Ball.State = MagicBallState.Following;
            from.CloseGump(typeof(ActionGump));
        }
    }

    class MagicBallIdleTimer : Timer
    {
        private AMagicCrystalBall m_Ball;
        private Mobile m_Follower;

        public MagicBallIdleTimer(Mobile follower, AMagicCrystalBall ball, TimeSpan delay)
            : base(delay)
        {
            m_Follower = follower;
            m_Ball = ball;
            Priority = TimerPriority.OneSecond;
        }

        protected override void OnTick()
        {
            m_Ball.StopFollowing(m_Follower);
        }
    }
}
