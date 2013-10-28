using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
    public class BaseEscortable : BaseCreature
    {
        private static readonly string[] m_TownNames = new string[]
        {
            "Cove", "Britain", "Jhelom",
            "Minoc", "Ocllo", "Trinsic",
            "Vesper", "Yew", "Skara Brae", //Original List, will need to add it back for Pre-ML shards
            "Nujel'm", "Moonglow", "Magincia"
        };
        private static readonly string[] m_MLTownNames = new string[]
        {
            "Cove", "Serpent's Hold", "Jhelom", //ML List
            "Nujel'm"
        };
        private static readonly Hashtable m_EscortTable = new Hashtable();
        private static readonly TimeSpan m_EscortDelay = TimeSpan.FromMinutes(5.0);
        private EDI m_Destination;
        private string m_DestinationString;
        private DateTime m_DeleteTime;
        private Timer m_DeleteTimer;
        private DateTime m_LastSeenEscorter;
        [Constructable]
        public BaseEscortable()
            : base(AIType.AI_Melee, FightMode.Aggressor, 22, 1, 0.2, 1.0)
        {
            this.InitBody();
            this.InitOutfit();
        }

        public BaseEscortable(Serial serial)
            : base(serial)
        {
        }

        public static Hashtable EscortTable
        {
            get
            {
                return m_EscortTable;
            }
        }
        public override bool Commandable
        {
            get
            {
                return false;
            }
        }// Our master cannot boss us around!
        [CommandProperty(AccessLevel.GameMaster)]
        public string Destination
        {
            get
            {
                return this.m_Destination == null ? null : this.m_Destination.Name;
            }
            set
            {
                this.m_DestinationString = value;
                this.m_Destination = EDI.Find(value);
            }
        }
        public virtual void InitBody()
        {
            this.SetStr(90, 100);
            this.SetDex(90, 100);
            this.SetInt(15, 25);

            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 401;
                this.Name = NameList.RandomName("female");
            }
            else
            {
                this.Body = 400;
                this.Name = NameList.RandomName("male");
            }
        }

        public virtual void InitOutfit()
        {
            this.AddItem(new FancyShirt(Utility.RandomNeutralHue()));
            this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            this.AddItem(new Boots(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);

            this.PackGold(200, 250);
        }

        public virtual bool SayDestinationTo(Mobile m)
        {
            EDI dest = this.GetDestination();

            if (dest == null || !m.Alive)
                return false;

            Mobile escorter = this.GetEscorter();

            if (escorter == null)
            {
                this.Say("I am looking to go to {0}, will you take me?", (dest.Name == "Ocllo" && m.Map == Map.Trammel) ? "Haven" : dest.Name);
                return true;
            }
            else if (escorter == m)
            {
                this.Say("Lead on! Payment will be made when we arrive in {0}.", (dest.Name == "Ocllo" && m.Map == Map.Trammel) ? "Haven" : dest.Name);
                return true;
            }

            return false;
        }

        public virtual bool AcceptEscorter(Mobile m)
        {
            EDI dest = this.GetDestination();

            if (dest == null)
                return false;

            Mobile escorter = this.GetEscorter();

            if (escorter != null || !m.Alive)
                return false;

            BaseEscortable escortable = (BaseEscortable)m_EscortTable[m];

            if (escortable != null && !escortable.Deleted && escortable.GetEscorter() == m)
            {
                this.Say("I see you already have an escort.");
                return false;
            }
            else if (m is PlayerMobile && (((PlayerMobile)m).LastEscortTime + m_EscortDelay) >= DateTime.UtcNow)
            {
                int minutes = (int)Math.Ceiling(((((PlayerMobile)m).LastEscortTime + m_EscortDelay) - DateTime.UtcNow).TotalMinutes);

                this.Say("You must rest {0} minute{1} before we set out on this journey.", minutes, minutes == 1 ? "" : "s");
                return false;
            }
            else if (this.SetControlMaster(m))
            {
                this.m_LastSeenEscorter = DateTime.UtcNow;

                if (m is PlayerMobile)
                    ((PlayerMobile)m).LastEscortTime = DateTime.UtcNow;

                this.Say("Lead on! Payment will be made when we arrive in {0}.", (dest.Name == "Ocllo" && m.Map == Map.Trammel) ? "Haven" : dest.Name);
                m_EscortTable[m] = this;
                this.StartFollow();
                return true;
            }

            return false;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 3))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            EDI dest = this.GetDestination();

            if (dest != null && !e.Handled && e.Mobile.InRange(this.Location, 3))
            {
                if (e.HasKeyword(0x1D)) // *destination*
                    e.Handled = this.SayDestinationTo(e.Mobile);
                else if (e.HasKeyword(0x1E)) // *i will take thee*
                    e.Handled = this.AcceptEscorter(e.Mobile);
            }
        }

        public override void OnAfterDelete()
        {
            if (this.m_DeleteTimer != null)
                this.m_DeleteTimer.Stop();

            this.m_DeleteTimer = null;

            base.OnAfterDelete();
        }

        public override void OnThink()
        {
            base.OnThink();
            this.CheckAtDestination();
        }

        public virtual void StartFollow()
        {
            this.StartFollow(this.GetEscorter());
        }

        public virtual void StartFollow(Mobile escorter)
        {
            if (escorter == null)
                return;

            this.ActiveSpeed = 0.1;
            this.PassiveSpeed = 0.2;

            this.ControlOrder = OrderType.Follow;
            this.ControlTarget = escorter;

            if ((this.IsPrisoner == true) && (this.CantWalk == true))
            {
                this.CantWalk = false;
            }
            this.CurrentSpeed = 0.1;
        }

        public virtual void StopFollow()
        {
            this.ActiveSpeed = 0.2;
            this.PassiveSpeed = 1.0;

            this.ControlOrder = OrderType.None;
            this.ControlTarget = null;

            this.CurrentSpeed = 1.0;
        }

        public virtual Mobile GetEscorter()
        {
            if (!this.Controlled)
                return null;

            Mobile master = this.ControlMaster;

            if (master == null)
                return null;

            if (master.Deleted || master.Map != this.Map || !master.InRange(this.Location, 30) || !master.Alive)
            {
                this.StopFollow();

                TimeSpan lastSeenDelay = DateTime.UtcNow - this.m_LastSeenEscorter;

                if (lastSeenDelay >= TimeSpan.FromMinutes(2.0))
                {
                    master.SendLocalizedMessage(1042473); // You have lost the person you were escorting.
                    this.Say(1005653); // Hmmm. I seem to have lost my master.

                    this.SetControlMaster(null);
                    m_EscortTable.Remove(master);

                    Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));
                    return null;
                }
                else
                {
                    this.ControlOrder = OrderType.Stay;
                    return master;
                }
            }

            if (this.ControlOrder != OrderType.Follow)
                this.StartFollow(master);

            this.m_LastSeenEscorter = DateTime.UtcNow;
            return master;
        }

        public virtual void BeginDelete()
        {
            if (this.m_DeleteTimer != null)
                this.m_DeleteTimer.Stop();

            this.m_DeleteTime = DateTime.UtcNow + TimeSpan.FromSeconds(30.0);

            this.m_DeleteTimer = new DeleteTimer(this, this.m_DeleteTime - DateTime.UtcNow);
            this.m_DeleteTimer.Start();
        }

        public virtual bool CheckAtDestination()
        {
            EDI dest = this.GetDestination();

            if (dest == null)
                return false;

            Mobile escorter = this.GetEscorter();

            if (escorter == null)
                return false;

            if (dest.Contains(this.Location))
            {
                this.Say(1042809, escorter.Name); // We have arrived! I thank thee, ~1_PLAYER_NAME~! I have no further need of thy services. Here is thy pay.

                // not going anywhere
                this.m_Destination = null;
                this.m_DestinationString = null;

                Container cont = escorter.Backpack;

                if (cont == null)
                    cont = escorter.BankBox;

                Gold gold = new Gold(500, 1000);

                if (!cont.TryDropItem(escorter, gold, false))
                    gold.MoveToWorld(escorter.Location, escorter.Map);

                this.StopFollow();
                this.SetControlMaster(null);
                m_EscortTable.Remove(escorter);
                this.BeginDelete();

                Misc.Titles.AwardFame(escorter, 10, true);

                bool gainedPath = false;

                PlayerMobile pm = escorter as PlayerMobile;

                if (pm != null)
                {
                    if (pm.CompassionGains > 0 && DateTime.UtcNow > pm.NextCompassionDay)
                    {
                        pm.NextCompassionDay = DateTime.MinValue;
                        pm.CompassionGains = 0;
                    }

                    if (pm.CompassionGains >= 5) // have already gained 5 times in one day, can gain no more
                    {
                        pm.SendLocalizedMessage(1053004); // You must wait about a day before you can gain in compassion again.
                    }
                    else if (VirtueHelper.Award(pm, VirtueName.Compassion, this.IsPrisoner ? 400 : 200, ref gainedPath))
                    {
                        if (gainedPath)
                            pm.SendLocalizedMessage(1053005); // You have achieved a path in compassion!
                        else
                            pm.SendLocalizedMessage(1053002); // You have gained in compassion.

                        pm.NextCompassionDay = DateTime.UtcNow + TimeSpan.FromDays(1.0); // in one day CompassionGains gets reset to 0
                        ++pm.CompassionGains;
                    }
                    else
                    {
                        pm.SendLocalizedMessage(1053003); // You have achieved the highest path of compassion and can no longer gain any further.
                    }
                }

                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            EDI dest = this.GetDestination();

            writer.Write(dest != null);

            if (dest != null)
                writer.Write(dest.Name);

            writer.Write(this.m_DeleteTimer != null);

            if (this.m_DeleteTimer != null)
                writer.WriteDeltaTime(this.m_DeleteTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (reader.ReadBool())
                this.m_DestinationString = reader.ReadString(); // NOTE: We cannot EDI.Find here, regions have not yet been loaded :-(

            if (reader.ReadBool())
            {
                this.m_DeleteTime = reader.ReadDeltaTime();
                this.m_DeleteTimer = new DeleteTimer(this, this.m_DeleteTime - DateTime.UtcNow);
                this.m_DeleteTimer.Start();
            }
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            return (from.AccessLevel >= AccessLevel.GameMaster);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            EDI dest = this.GetDestination();

            if (dest != null && from.Alive)
            {
                Mobile escorter = this.GetEscorter();

                if (escorter == null || escorter == from)
                    list.Add(new AskDestinationEntry(this, from));

                if (escorter == null)
                    list.Add(new AcceptEscortEntry(this, from));
                else if (escorter == from)
                    list.Add(new AbandonEscortEntry(this, from));
            }

            base.AddCustomContextEntries(from, list);
        }

        public virtual string[] GetPossibleDestinations()
        {
            if (!Core.ML)
                return m_TownNames;
            else
                return m_MLTownNames;
        }

        public virtual string PickRandomDestination()
        {
            if (Map.Felucca.Regions.Count == 0 || this.Map == null || this.Map == Map.Internal || this.Location == Point3D.Zero)
                return null; // Not yet fully initialized

            string[] possible = this.GetPossibleDestinations();
            string picked = null;

            while (picked == null)
            {
                picked = possible[Utility.Random(possible.Length)];
                EDI test = EDI.Find(picked);

                if (test != null && test.Contains(this.Location))
                    picked = null;
            }

            return picked;
        }

        public EDI GetDestination()
        {
            if (this.m_DestinationString == null && this.m_DeleteTimer == null)
                this.m_DestinationString = this.PickRandomDestination();

            if (this.m_Destination != null && this.m_Destination.Name == this.m_DestinationString)
                return this.m_Destination;

            if (Map.Felucca.Regions.Count > 0)
                return (this.m_Destination = EDI.Find(this.m_DestinationString));

            return (this.m_Destination = null);
        }

        protected override bool OnMove(Direction d)
        {
            if (!base.OnMove(d))
                return false;

            this.CheckAtDestination();

            return true;
        }

        private class DeleteTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public DeleteTimer(Mobile m, TimeSpan delay)
                : base(delay)
            {
                this.m_Mobile = m;

                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                this.m_Mobile.Delete();
            }
        }
    }

    public class EscortDestinationInfo
    {
        private static Hashtable m_Table;
        private readonly string m_Name;
        private readonly Region m_Region;
        //private Rectangle2D[] m_Bounds;
        public EscortDestinationInfo(string name, Region region)
        {
            this.m_Name = name;
            this.m_Region = region;
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public Region Region
        {
            get
            {
                return this.m_Region;
            }
        }
        public static void LoadTable()
        {
            ICollection list = Map.Felucca.Regions.Values;

            if (list.Count == 0)
                return;

            m_Table = new Hashtable();

            foreach (Region r in list)
            {
                if (r.Name == null)
                    continue;

                if (r is Regions.DungeonRegion || r is Regions.TownRegion)
                    m_Table[r.Name] = new EscortDestinationInfo(r.Name, r);
            }
        }

        public static EDI Find(string name)
        {
            if (m_Table == null)
                LoadTable();

            if (name == null || m_Table == null)
                return null;

            return (EscortDestinationInfo)m_Table[name];
        }

        /*public Rectangle2D[] Bounds
        {
        get{ return m_Bounds; }
        }*/
        public bool Contains(Point3D p)
        {
            return this.m_Region.Contains(p);
        }
    }

    public class AskDestinationEntry : ContextMenuEntry
    {
        private readonly BaseEscortable m_Mobile;
        private readonly Mobile m_From;
        public AskDestinationEntry(BaseEscortable m, Mobile from)
            : base(6100, 3)
        {
            this.m_Mobile = m;
            this.m_From = from;
        }

        public override void OnClick()
        {
            this.m_Mobile.SayDestinationTo(this.m_From);
        }
    }

    public class AcceptEscortEntry : ContextMenuEntry
    {
        private readonly BaseEscortable m_Mobile;
        private readonly Mobile m_From;
        public AcceptEscortEntry(BaseEscortable m, Mobile from)
            : base(6101, 3)
        {
            this.m_Mobile = m;
            this.m_From = from;
        }

        public override void OnClick()
        {
            this.m_Mobile.AcceptEscorter(this.m_From);
        }
    }

    public class AbandonEscortEntry : ContextMenuEntry
    {
        private readonly BaseEscortable m_Mobile;
        private readonly Mobile m_From;
        public AbandonEscortEntry(BaseEscortable m, Mobile from)
            : base(6102, 3)
        {
            this.m_Mobile = m;
            this.m_From = from;
        }

        public override void OnClick()
        {
            this.m_Mobile.Delete(); // OSI just seems to delete instantly
        }
    }
}