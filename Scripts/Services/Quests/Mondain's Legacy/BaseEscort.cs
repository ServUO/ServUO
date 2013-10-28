using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class BaseEscort : MondainQuester
    {
        private static readonly TimeSpan m_EscortDelay = TimeSpan.FromMinutes(5.0);
        private static readonly Dictionary<Mobile, Mobile> m_EscortTable = new Dictionary<Mobile, Mobile>();
        private BaseQuest m_Quest;
        private DateTime m_LastSeenEscorter;
        private Timer m_DeleteTimer;
        private bool m_Checked;
        public BaseEscort()
            : base()
        {
            this.AI = AIType.AI_Melee;
            this.FightMode = FightMode.Aggressor;
            this.RangePerception = 22;
            this.RangeFight = 1;
            this.ActiveSpeed = 0.2;
            this.PassiveSpeed = 1.0;

            this.ControlSlots = 0;
        }

        public BaseEscort(Serial serial)
            : base(serial)
        {
        }

        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public override bool IsInvulnerable
        {
            get
            {
                return false;
            }
        }
        public override bool Commandable
        {
            get
            {
                return false;
            }
        }
        public override Type[] Quests
        {
            get
            {
                return null;
            }
        }
        public BaseQuest Quest
        {
            get
            {
                return this.m_Quest;
            }
            set
            {
                this.m_Quest = value;
            }
        }
        public DateTime LastSeenEscorter
        {
            get
            {
                return this.m_LastSeenEscorter;
            }
            set
            {
                this.m_LastSeenEscorter = value;
            }
        }
        public override void OnTalk(PlayerMobile player)
        {
            if (this.AcceptEscorter(player))
                base.OnTalk(player);
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            return (from.AccessLevel >= AccessLevel.GameMaster);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && from == this.ControlMaster)
                list.Add(new AbandonEscortEntry(this));

            base.AddCustomContextEntries(from, list);
        }

        public override void OnAfterDelete()
        {
            if (this.m_Quest != null)
            {
                this.m_Quest.RemoveQuest();

                if (this.m_Quest.Owner != null)
                    m_EscortTable.Remove(this.m_Quest.Owner);
            }

            base.OnAfterDelete();
        }

        public override void OnThink()
        {
            base.OnThink();
			
            this.CheckAtDestination();
        }

        public override bool CanBeDamaged()
        {
            return true;
        }

        public override void InitBody()
        {
            this.SetStr(90, 100);
            this.SetDex(90, 100);
            this.SetInt(15, 25);

            this.Hue = Utility.RandomSkinHue();
            this.Female = Utility.RandomBool();
            this.Name = NameList.RandomName(this.Female ? "female" : "male");
            this.Race = Race.Human;

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_DeleteTimer != null);

            if (this.m_DeleteTimer != null)
                writer.WriteDeltaTime(this.m_DeleteTimer.Next);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (reader.ReadBool())
            {
                DateTime deleteTime = reader.ReadDeltaTime();
                this.m_DeleteTimer = Timer.DelayCall(deleteTime - DateTime.UtcNow, new TimerCallback(Delete));
            }
        }

        public void AddHash(PlayerMobile player)
        {
            m_EscortTable[player] = this;
        }

        public virtual void StartFollow()
        {
            this.StartFollow(this.ControlMaster);
        }

        public virtual void StartFollow(Mobile escorter)
        { 
            this.ActiveSpeed = 0.1;
            this.PassiveSpeed = 0.2;

            this.ControlOrder = OrderType.Follow;
            this.ControlTarget = escorter;

            this.CurrentSpeed = 0.1;
        }

        public virtual void StopFollow()
        {
            this.ActiveSpeed = 0.2;
            this.PassiveSpeed = 1.0;

            this.ControlOrder = OrderType.None;
            this.ControlTarget = null;

            this.CurrentSpeed = 1.0;

            this.SetControlMaster(null);
        }

        public virtual void BeginDelete(Mobile m)
        {
            this.StopFollow();
			
            if (m != null)
                m_EscortTable.Remove(m);
			
            this.m_DeleteTimer = Timer.DelayCall(TimeSpan.FromSeconds(45.0), new TimerCallback(Delete));
        }

        public virtual bool AcceptEscorter(Mobile m)
        {
            if (!m.Alive)
            {
                return false;
            }
            else if (this.m_DeleteTimer != null)
            {
                this.Say(500898); // I am sorry, but I do not wish to go anywhere.
                return false;
            }
            else if (this.Controlled)
            {
                if (m == this.ControlMaster)
                    m.SendGump(new MondainQuestGump(this.m_Quest, MondainQuestGump.Section.InProgress, false));
                else
                    this.Say(500897); // I am already being led!
				
                return false;
            }
            else if (!m.InRange(this.Location, 5))
            {
                this.Say(500348); // I am too far away to do that.
                return false;
            }
            else if (m_EscortTable.ContainsKey(m))
            {
                this.Say(500896); // I see you already have an escort.
                return false;
            }
            else if (m is PlayerMobile && (((PlayerMobile)m).LastEscortTime + m_EscortDelay) >= DateTime.UtcNow)
            {
                int minutes = (int)Math.Ceiling(((((PlayerMobile)m).LastEscortTime + m_EscortDelay) - DateTime.UtcNow).TotalMinutes);

                this.Say("You must rest {0} minute{1} before we set out on this journey.", minutes, minutes == 1 ? "" : "s");
                return false;
            }

            return true;
        }

        public virtual EscortObjective GetObjective()
        {
            if (this.m_Quest != null)
            {
                for (int i = 0; i < this.m_Quest.Objectives.Count; i++)
                {
                    EscortObjective escort = this.m_Quest.Objectives[i] as EscortObjective;

                    if (escort != null && !escort.Completed && !escort.Failed)
                        return escort;
                }
            }

            return null;
        }

        public virtual Mobile GetEscorter()
        {
            Mobile master = this.ControlMaster;

            if (master == null || !this.Controlled)
            {
                return master;
            }
            else if (master.Map != this.Map || !master.InRange(this.Location, 30) || !master.Alive)
            {
                TimeSpan lastSeenDelay = DateTime.UtcNow - this.m_LastSeenEscorter;

                if (lastSeenDelay >= TimeSpan.FromMinutes(2.0))
                {
                    EscortObjective escort = this.GetObjective();

                    if (escort != null)
                    {
                        master.SendLocalizedMessage(1071194); // You have failed your escort quest…
                        master.PlaySound(0x5B3);
                        escort.Fail();
                    }

                    master.SendLocalizedMessage(1042473); // You have lost the person you were escorting.
                    this.Say(1005653); // Hmmm.  I seem to have lost my master.

                    this.StopFollow();
                    m_EscortTable.Remove(master);
                    this.m_DeleteTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));

                    return null;
                }
                else
                {
                    this.ControlOrder = OrderType.Stay;
                }
            }
            else
            {
                if (this.ControlOrder != OrderType.Follow)
                    this.StartFollow(master);

                this.m_LastSeenEscorter = DateTime.UtcNow;
            }

            return master;
        }

        public virtual Region GetDestination()
        {
            return null;
        }

        public virtual bool CheckAtDestination()
        {
            if (this.m_Quest != null)
            {
                EscortObjective escort = this.GetObjective();

                if (escort == null)
                    return false;

                Mobile escorter = this.GetEscorter();

                if (escorter == null)
                    return false;

                if (escort.Region != null && escort.Region.Contains(this.Location))
                {
                    this.Say(1042809, escorter.Name); // We have arrived! I thank thee, ~1_PLAYER_NAME~! I have no further need of thy services. Here is thy pay.

                    escort.Complete();

                    if (this.m_Quest.Completed)
                    {
                        escorter.SendLocalizedMessage(1046258, null, 0x23); // Your quest is complete.		

                        if (QuestHelper.AnyRewards(this.m_Quest))
                            escorter.SendGump(new MondainQuestGump(this.m_Quest, MondainQuestGump.Section.Rewards, false, true));
                        else
                            this.m_Quest.GiveRewards();

                        escorter.PlaySound(this.m_Quest.CompleteSound);

                        this.StopFollow();
                        m_EscortTable.Remove(escorter);
                        this.m_DeleteTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));

                        // fame
                        Misc.Titles.AwardFame(escorter, escort.Fame, true);

                        // compassion
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
                            else if (VirtueHelper.Award(pm, VirtueName.Compassion, escort.Compassion, ref gainedPath))
                            {
                                pm.SendLocalizedMessage(1074949, null, 0x2A);  // You have demonstrated your compassion!  Your kind actions have been noted.

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
                    }
                    else
                        escorter.PlaySound(this.m_Quest.UpdateSound);

                    return true;
                }
            }
            else if (!this.m_Checked)
            {
                Region region = this.GetDestination();

                if (region != null && region.Contains(this.Location))
                {
                    this.m_DeleteTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));
                    this.m_Checked = true;
                }
            }

            return false;
        }

        private class AbandonEscortEntry : ContextMenuEntry
        {
            private readonly BaseEscort m_Mobile;
            public AbandonEscortEntry(BaseEscort m)
                : base(6102, 3)
            {
                this.m_Mobile = m;
            }

            public override void OnClick()
            {
                this.Owner.From.SendLocalizedMessage(1071194); // You have failed your escort quest…
                this.Owner.From.PlaySound(0x5B3);
                this.m_Mobile.Delete();
            }
        }
    }
}