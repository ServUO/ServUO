using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Factions;
using Server.Network;
using Server.Targeting;

namespace Server.Engines.PartySystem
{
    public class Party : IParty
    {
        public const int Capacity = 10;
        private readonly Mobile m_Leader;
        private readonly List<PartyMemberInfo> m_Members;
        private readonly List<Mobile> m_Candidates;
        private readonly List<Mobile> m_Listeners;// staff listening
        public Party(Mobile leader)
        {
            m_Leader = leader;

            m_Members = new List<PartyMemberInfo>();
            m_Candidates = new List<Mobile>();
            m_Listeners = new List<Mobile>();

            m_Members.Add(new PartyMemberInfo(leader));
        }

        public int Count
        {
            get
            {
                return m_Members.Count;
            }
        }
        public bool Active
        {
            get
            {
                return m_Members.Count > 1;
            }
        }
        public Mobile Leader
        {
            get
            {
                return m_Leader;
            }
        }
        public List<PartyMemberInfo> Members
        {
            get
            {
                return m_Members;
            }
        }
        public List<Mobile> Candidates
        {
            get
            {
                return m_Candidates;
            }
        }
        public PartyMemberInfo this[int index]
        {
            get
            {
                return m_Members[index];
            }
        }
        public PartyMemberInfo this[Mobile m]
        {
            get
            {
                for (int i = 0; i < m_Members.Count; ++i)
                    if (m_Members[i].Mobile == m)
                        return m_Members[i];

                return null;
            }
        }
        public static void Initialize()
        {
            EventSink.Logout += new LogoutEventHandler(EventSink_Logout);
            EventSink.Login += new LoginEventHandler(EventSink_Login);
            EventSink.PlayerDeath += new PlayerDeathEventHandler(EventSink_PlayerDeath);

            CommandSystem.Register("ListenToParty", AccessLevel.GameMaster, new CommandEventHandler(ListenToParty_OnCommand));
        }

        public static void ListenToParty_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ListenToParty_OnTarget));
            e.Mobile.SendMessage("Target a partied player.");
        }

        public static void ListenToParty_OnTarget(Mobile from, object obj)
        {
            if (obj is Mobile)
            {
                Party p = Party.Get((Mobile)obj);

                if (p == null)
                {
                    from.SendMessage("They are not in a party.");
                }
                else if (p.m_Listeners.Contains(from))
                {
                    p.m_Listeners.Remove(from);
                    from.SendMessage("You are no longer listening to that party.");
                }
                else
                {
                    p.m_Listeners.Add(from);
                    from.SendMessage("You are now listening to that party.");
                }
            }
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            Mobile from = e.Mobile;
            Party p = Party.Get(from);

            if (p != null)
            {
                Mobile m = from.LastKiller;

                if (m == from)
                    p.SendPublicMessage(from, "I killed myself !!");
                else if (m == null)
                    p.SendPublicMessage(from, "I was killed !!");
                else
                    p.SendPublicMessage(from, String.Format("I was killed by {0} !!", m.Name));
            }
        }

        public static void EventSink_Login(LoginEventArgs e)
        {
            Mobile from = e.Mobile;
            Party p = Party.Get(from);

            if (p != null)
                new RejoinTimer(from).Start();
            else
                from.Party = null;
        }

        public static void EventSink_Logout(LogoutEventArgs e)
        {
            Mobile from = e.Mobile;
            Party p = Party.Get(from);

            if (p != null)
                p.Remove(from);

            from.Party = null;
        }

        public static Party Get(Mobile m)
        {
            if (m == null)
                return null;

            return m.Party as Party;
        }

        public static void Invite(Mobile from, Mobile target)
        {
            Faction ourFaction = Faction.Find(from);
            Faction theirFaction = Faction.Find(target);

            if (ourFaction != null && theirFaction != null && ourFaction != theirFaction)
            {
                from.SendLocalizedMessage(1008088); // You cannot have players from opposing factions in the same party!
                target.SendLocalizedMessage(1008093); // The party cannot have members from opposing factions.
                return;
            }

            Party p = Party.Get(from);

            if (p == null)
                from.Party = p = new Party(from);

            if (!p.Candidates.Contains(target))
                p.Candidates.Add(target);

            //  : You are invited to join the party. Type /accept to join or /decline to decline the offer.
            target.Send(new MessageLocalizedAffix(target.NetState, Serial.MinusOne, -1, MessageType.Label, 0x3B2, 3, 1008089, "", AffixType.Prepend | AffixType.System, from.Name, ""));

            from.SendLocalizedMessage(1008090); // You have invited them to join the party.

            target.Send(new PartyInvitation(from));
            target.Party = from;

            DeclineTimer.Start(target, from);
        }

        public void Add(Mobile m)
        {
            PartyMemberInfo mi = this[m];

            if (mi == null)
            {
                m_Members.Add(new PartyMemberInfo(m));
                m.Party = this;

                Packet memberList = Packet.Acquire(new PartyMemberList(this));
                Packet attrs = Packet.Acquire(new MobileAttributesN(m));

                for (int i = 0; i < m_Members.Count; ++i)
                {
                    Mobile f = ((PartyMemberInfo)m_Members[i]).Mobile;

                    f.Send(memberList);

                    if (f != m)
                    {
                        f.Send(new MobileStatusCompact(m.CanBeRenamedBy(f), m));
                        f.Send(attrs);

                        if (f.NetState != null && f.NetState.IsEnhancedClient)
                            Waypoints.Create(f, m, WaypointType.PartyMember);

                        m.Send(new MobileStatusCompact(f.CanBeRenamedBy(m), f));
                        m.Send(new MobileAttributesN(f));

                        if (m.NetState != null && m.NetState.IsEnhancedClient)
                            Waypoints.Create(m, f, WaypointType.PartyMember);
                    }
                }

                Packet.Release(memberList);
                Packet.Release(attrs);
            }
        }

        public void OnAccept(Mobile from)
        {
            OnAccept(from, false);
        }

        public void OnAccept(Mobile from, bool force)
        {
            Faction ourFaction = Faction.Find(m_Leader);
            Faction theirFaction = Faction.Find(from);

            if (!force && ourFaction != null && theirFaction != null && ourFaction != theirFaction)
                return;

            //  : joined the party.
            SendToAll(new MessageLocalizedAffix(Serial.MinusOne, -1, MessageType.Label, 0x3B2, 3, 1008094, "", AffixType.Prepend | AffixType.System, from.Name, ""));

            from.SendLocalizedMessage(1005445); // You have been added to the party.

            m_Candidates.Remove(from);
            Add(from);
        }

        public void OnDecline(Mobile from, Mobile leader)
        {
            //  : Does not wish to join the party.
            leader.SendLocalizedMessage(1008091, false, from.Name);

            from.SendLocalizedMessage(1008092); // You notify them that you do not wish to join the party.

            m_Candidates.Remove(from);
            from.Send(new PartyEmptyList(from));

            if (m_Candidates.Count == 0 && m_Members.Count <= 1)
            {
                for (int i = 0; i < m_Members.Count; ++i)
                {
                    this[i].Mobile.Send(new PartyEmptyList(this[i].Mobile));
                    this[i].Mobile.Party = null;
                }

                m_Members.Clear();
            }
        }

        public void Remove(Mobile m)
        {
            if (m == m_Leader)
            {
                Disband();
            }
            else
            {
                for (int i = 0; i < m_Members.Count; ++i)
                {
                    Mobile mob = m_Members[i].Mobile;

                    if (mob == m)
                    {
                        m_Members.RemoveAt(i);

                        m.Party = null;
                        m.Send(new PartyEmptyList(m));

                        m.SendLocalizedMessage(1005451); // You have been removed from the party.

                        SendToAll(new PartyRemoveMember(m, this));
                        SendToAll(1005452); // A player has been removed from your party.

                        break;
                    }
                    else
                    {
                        if (mob != null && mob.NetState != null && mob.NetState.IsEnhancedClient)
                        {
                            Waypoints.Remove(mob, m);
                        }

                        if (m.NetState != null && m.NetState.IsEnhancedClient)
                        {
                            Waypoints.Remove(m, mob);
                        }
                    }
                }

                if (m_Members.Count == 1)
                {
                    SendToAll(1005450); // The last person has left the party...
                    Disband();
                }
            }
        }

        public bool Contains(Mobile m)
        {
            return (this[m] != null);
        }

        public void Disband()
        {
            SendToAll(1005449); // Your party has disbanded.

            for (int i = 0; i < m_Members.Count; ++i)
            {
                this[i].Mobile.Send(new PartyEmptyList(this[i].Mobile));
                this[i].Mobile.Party = null;
            }

            m_Members.Clear();
        }

        public void SendToAll(int number)
        {
            SendToAll(number, "", 0x3B2);
        }

        public void SendToAll(int number, string args)
        {
            SendToAll(number, args, 0x3B2);
        }

        public void SendToAll(int number, string args, int hue)
        {
            SendToAll(new MessageLocalized(Serial.MinusOne, -1, MessageType.Regular, hue, 3, number, "System", args));
        }

        public void SendPublicMessage(Mobile from, string text)
        {
            SendToAll(new PartyTextMessage(true, from, text));

            for (int i = 0; i < m_Listeners.Count; ++i)
            {
                Mobile mob = m_Listeners[i];

                if (mob.Party != this)
                    m_Listeners[i].SendMessage("[{0}]: {1}", from.Name, text);
            }

            SendToStaffMessage(from, "[Party]: {0}", text);
        }

        public void SendPrivateMessage(Mobile from, Mobile to, string text)
        {
            to.Send(new PartyTextMessage(false, from, text));

            for (int i = 0; i < m_Listeners.Count; ++i)
            {
                Mobile mob = m_Listeners[i];

                if (mob.Party != this)
                    m_Listeners[i].SendMessage("[{0}]->[{1}]: {2}", from.Name, to.Name, text);
            }

            SendToStaffMessage(from, "[Party]->[{0}]: {1}", to.Name, text);
        }

        public void SendToAll(Packet p)
        {
            p.Acquire();

            for (int i = 0; i < m_Members.Count; ++i)
                m_Members[i].Mobile.Send(p);

            if (p is MessageLocalized || p is MessageLocalizedAffix || p is UnicodeMessage || p is AsciiMessage)
            {
                for (int i = 0; i < m_Listeners.Count; ++i)
                {
                    Mobile mob = m_Listeners[i];

                    if (mob.Party != this)
                        mob.Send(p);
                }
            }

            p.Release();
        }

        public void OnStamChanged(Mobile m)
        {
            Packet p = null;

            for (int i = 0; i < m_Members.Count; ++i)
            {
                Mobile c = m_Members[i].Mobile;

                if (c != m && m.Map == c.Map && Utility.InUpdateRange(c, m) && c.CanSee(m))
                {
                    if (p == null)
                        p = Packet.Acquire(new MobileStamN(m));

                    c.Send(p);
                }
            }

            Packet.Release(p);
        }

        public void OnManaChanged(Mobile m)
        {
            Packet p = null;

            for (int i = 0; i < m_Members.Count; ++i)
            {
                Mobile c = m_Members[i].Mobile;

                if (c != m && m.Map == c.Map && Utility.InUpdateRange(c, m) && c.CanSee(m))
                {
                    if (p == null)
                        p = Packet.Acquire(new MobileManaN(m));

                    c.Send(p);
                }
            }

            Packet.Release(p);
        }

        public void OnStatsQuery(Mobile beholder, Mobile beheld)
        {
            if (beholder != beheld && Contains(beholder) && beholder.Map == beheld.Map && Utility.InUpdateRange(beholder, beheld))
            {
                if (!beholder.CanSee(beheld))
                    beholder.Send(new MobileStatusCompact(beheld.CanBeRenamedBy(beholder), beheld));

                beholder.Send(new MobileAttributesN(beheld));
            }
        }

        private void SendToStaffMessage(Mobile from, string text)
        { 
            Packet p = null;

            foreach (NetState ns in from.GetClientsInRange(8))
            {
                Mobile mob = ns.Mobile;

                if (mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel && mob.Party != this && !m_Listeners.Contains(mob))
                {
                    if (p == null)
                        p = Packet.Acquire(new UnicodeMessage(from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text));

                    ns.Send(p);
                }
            }

            Packet.Release(p);
        }

        private void SendToStaffMessage(Mobile from, string format, params object[] args)
        {
            SendToStaffMessage(from, String.Format(format, args));
        }

        private class RejoinTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public RejoinTimer(Mobile m)
                : base(TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                Party p = Party.Get(m_Mobile);

                if (p == null)
                    return;

                m_Mobile.SendLocalizedMessage(1005437); // You have rejoined the party.
                m_Mobile.Send(new PartyMemberList(p));

                Packet message = Packet.Acquire(new MessageLocalizedAffix(Serial.MinusOne, -1, MessageType.Label, 0x3B2, 3, 1008087, "", AffixType.Prepend | AffixType.System, m_Mobile.Name, ""));
                Packet attrs = Packet.Acquire(new MobileAttributesN(m_Mobile));

                foreach (PartyMemberInfo mi in p.Members)
                {
                    Mobile m = mi.Mobile;

                    if (m != m_Mobile)
                    {
                        m.Send(message);
                        m.Send(new MobileStatusCompact(m_Mobile.CanBeRenamedBy(m), m_Mobile));
                        m.Send(attrs);
                        m_Mobile.Send(new MobileStatusCompact(m.CanBeRenamedBy(m_Mobile), m));
                        m_Mobile.Send(new MobileAttributesN(m));

                        if(m_Mobile.NetState != null && m_Mobile.NetState.IsEnhancedClient)
                            Waypoints.Create(m_Mobile, m, WaypointType.PartyMember);
                    }
                }

                Packet.Release(message);
                Packet.Release(attrs);
            }
        }
    }
}