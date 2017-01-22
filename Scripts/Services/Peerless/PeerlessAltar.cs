using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Engines.PartySystem;

namespace Server.Items
{
    public abstract class PeerlessAltar : Container
    {
        public override bool IsPublicContainer { get { return true; } }
        public override bool IsDecoContainer { get { return false; } }

        public virtual TimeSpan TimeToSlay { get { return TimeSpan.FromMinutes(90); } }
        public virtual TimeSpan DelayAfterBossSlain { get { return TimeSpan.FromMinutes(15); } }

        public abstract int KeyCount { get; }
        public abstract MasterKey MasterKey { get; }

        public abstract Type[] Keys { get; }
        public abstract BasePeerless Boss { get; }

        public abstract Rectangle2D[] BossBounds { get; }

        private BasePeerless m_Peerless;
        private Point3D m_BossLocation;
        private Point3D m_TeleportDest;
        private Point3D m_ExitDest;
        private DateTime m_Deadline;
        private bool m_IsAvailable;

        [CommandProperty(AccessLevel.GameMaster)]
        public BasePeerless Peerless
        {
            get { return m_Peerless; }
            set { m_Peerless = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BossLocation
        {
            get { return m_BossLocation; }
            set { m_BossLocation = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TeleportDest
        {
            get { return m_TeleportDest; }
            set { m_TeleportDest = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D ExitDest
        {
            get { return m_ExitDest; }
            set { m_ExitDest = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Deadline
        {
            get { return m_Deadline; }
            set { m_Deadline = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool ResetPeerless
        {
            get { return false; }
            set { if (value == true) FinishSequence(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FighterCount
        {
            get { return m_Fighters != null ? m_Fighters.Count : 0; }
        }

        private List<Mobile> m_Fighters;
        private Dictionary<Mobile, List<Mobile>> m_Pets;
        private List<Item> m_MasterKeys;

        public List<Mobile> Fighters
        {
            get { return m_Fighters; }
        }

        public Dictionary<Mobile, List<Mobile>> Pets
        {
            get { return m_Pets; }
        }

        public List<Item> MasterKeys
        {
            get { return m_MasterKeys; }
        }

        public bool Activated
        {
            get { return (m_Fighters.Count > 0 || Items.Count == Keys.Length || !m_IsAvailable) ? true : false; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public bool IsAvailable
        {
            get { return m_IsAvailable; }
        }

        public Mobile Summoner
        {
            get
            {
                if (m_Fighters == null || m_Fighters.Count == 0)
                    return null;

                return m_Fighters[0];
            }
        }

        public PeerlessAltar(int itemID)
            : base(itemID)
        {
            Movable = false;

            m_Fighters = new List<Mobile>();
            m_Pets = new Dictionary<Mobile, List<Mobile>>();
            m_MasterKeys = new List<Item>();
            m_IsAvailable = true;
        }

        public PeerlessAltar(Serial serial)
            : base(serial)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (Activated)
            {
                from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
                return false;
            }

            if (!IsKey(dropped))
            {
                from.SendLocalizedMessage(1072682); // This is not the proper key.
                return false;
            }

            if (Items.Count + 1 == Keys.Length)
            {
                from.SendLocalizedMessage(1072680); // You have been given the key to the boss.

                for (int i = 0; i < KeyCount; i++)
                {
                    MasterKey key = MasterKey;

                    if (key != null)
                    {
                        key.Altar = this;

                        if (!from.AddToBackpack(key))
                            key.MoveToWorld(from.Location, from.Map);

                        m_MasterKeys.Add(key);
                    }
                }

                dropped.Delete();
                ClearContainer();
                StopKeyTimer();
            }
            else
                StartTimer(from);

            return base.OnDragDrop(from, dropped);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if ((int)from.AccessLevel > (int)AccessLevel.Player)
                base.OnDoubleClick(from);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if ((int)from.AccessLevel > (int)AccessLevel.Player)
                return base.CheckLift(from, item, ref reject);
            else
                reject = LRReason.CannotLift;

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            // version 1
            writer.Write((bool)(m_Helpers != null));

            if (m_Helpers != null)
                writer.WriteMobileList<BaseCreature>(m_Helpers);

            // version 0			
            writer.Write((Mobile)m_Peerless);
            writer.Write((Point3D)m_BossLocation);
            writer.Write((Point3D)m_TeleportDest);
            writer.Write((Point3D)m_ExitDest);

            writer.Write((DateTime)m_Deadline);

            // serialize master keys						
            writer.WriteItemList(m_MasterKeys);

            // serialize fighters							
            writer.WriteMobileList(m_Fighters);

            // serialize pets
            writer.Write((int)m_Pets.Count);

            foreach (KeyValuePair<Mobile, List<Mobile>> pair in m_Pets)
            {
                writer.Write((Mobile)pair.Key);

                writer.WriteMobileList(pair.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                    if (reader.ReadBool())
                        m_Helpers = reader.ReadStrongMobileList<BaseCreature>();
                    goto case 0;
                case 0:
                    m_Peerless = reader.ReadMobile() as BasePeerless;
                    m_BossLocation = reader.ReadPoint3D();
                    m_TeleportDest = reader.ReadPoint3D();
                    m_ExitDest = reader.ReadPoint3D();

                    m_Deadline = reader.ReadDateTime();

                    m_MasterKeys = reader.ReadStrongItemList();
                    m_Fighters = reader.ReadStrongMobileList();
                    m_Pets = new Dictionary<Mobile, List<Mobile>>();

                    int count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                        m_Pets.Add(reader.ReadMobile(), reader.ReadStrongMobileList());

                    if (version < 2)
                        reader.ReadBool();

                    if (m_Peerless == null && m_Helpers.Count > 0)
                        Timer.DelayCall(TimeSpan.FromSeconds(30), new TimerCallback(CleanupHelpers));

                    break;
            }

            FinishSequence();
        }

        public virtual bool IsKey(Item item)
        {
            if (Keys == null || item == null)
                return false;

            bool isKey = false;

            // check if item is key	
            for (int i = 0; i < Keys.Length && !isKey; i++)
                if (Keys[i].IsAssignableFrom(item.GetType()))
                    isKey = true;

            // check if item is already in container			
            for (int i = 0; i < Items.Count && isKey; i++)
                if (Items[i].GetType() == item.GetType())
                    return false;

            return isKey;
        }

        public virtual void ClearContainer()
        {
            while (Items.Count > 0)
                Items[0].Delete();
        }

        private int toConfirm;

        public virtual void AddFighter(Mobile fighter, bool confirmed)
        {
            if (confirmed)
                AddFighter(fighter);

            toConfirm -= 1;

            if (toConfirm == 0)
                BeginSequence(Summoner);
        }

        public virtual void AddFighter(Mobile fighter)
        {
            m_Fighters.Add(fighter);

            IPooledEnumerable eable = fighter.GetMobilesInRange(5);
            foreach (Mobile m in eable)
            {
                if (m is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)m;

                    if (pet.Controlled && pet.ControlMaster == fighter)
                    {
                        if (!m_Pets.ContainsKey(fighter))
                            m_Pets.Add(fighter, new List<Mobile>());

                        m_Pets[fighter].Add(pet);
                    }
                }
            }
            eable.Free();

            if (fighter.Mounted)
            {
                if (!m_Pets.ContainsKey(fighter))
                    m_Pets.Add(fighter, new List<Mobile>());

                if (fighter.Mount is Mobile)
                    m_Pets[fighter].Add((Mobile)fighter.Mount);
            }
        }

        public virtual void SendConfirmations(Mobile from)
        {
            Party party = Party.Get(from);

            if (party != null)
            {
                toConfirm = 0;

                foreach (PartyMemberInfo info in party.Members)
                {
                    Mobile m = info.Mobile;

                    if (m.InRange(from.Location, 15) && CanEnter(m))
                    {
                        if (m == from)
                            AddFighter(from);
                        else
                        {
                            toConfirm += 1;

                            m.CloseGump(typeof(ConfirmEntranceGump));
                            m.SendGump(new ConfirmEntranceGump(this));
                        }
                    }
                }

                if (toConfirm == 0)
                    BeginSequence(Summoner);
            }
            else
            {
                AddFighter(from);
                BeginSequence(Summoner);
            }
        }

        public virtual void BeginSequence(Mobile from)
        {
            if (m_Peerless == null)
            {
                // spawn boss
                m_Peerless = Boss;
                m_IsAvailable = false;

                if (m_Peerless != null)
                {
                    m_Peerless.Home = m_BossLocation;
                    m_Peerless.RangeHome = 12;
                    m_Peerless.MoveToWorld(m_BossLocation, Map);
                    m_Peerless.Altar = this;
                }
                else
                    return;

                StartSlayTimer();
            }

            Point3D loc = from.Location;

            // teleport fighters
            for (int i = 0; i < m_Fighters.Count; i++)
            {
                Mobile fighter = m_Fighters[i];
                int counter = 1;

                if (fighter.InRange(loc, 15) && CanEnter(fighter))
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(counter), new TimerStateCallback(Enter_Callback), fighter);

                    counter += 1;
                }
            }
        }

        private void Enter_Callback(object state)
        {
            if (state is Mobile)
                Enter((Mobile)state);
        }

        public virtual void Enter(Mobile fighter)
        {
            if (CanEnter(fighter))
            {
                // teleport party member's pets
                if (m_Pets.ContainsKey(fighter))
                {
                    for (int i = 0; i < m_Pets[fighter].Count; i++)
                    {
                        BaseCreature pet = m_Pets[fighter][i] as BaseCreature;

                        if (pet != null && pet.Alive && pet.InRange(fighter.Location, 5) && !(pet is BaseMount && ((BaseMount)pet).Rider != null) && CanEnter(pet))
                        {
                            pet.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                            pet.PlaySound(0x1FE);
                            pet.MoveToWorld(m_TeleportDest, Map);
                        }
                    }
                }

                // teleport party member
                fighter.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                fighter.PlaySound(0x1FE);
                fighter.MoveToWorld(m_TeleportDest, Map);
            }
        }

        public virtual bool CanEnter(Mobile fighter)
        {
            return true;
        }

        public virtual bool CanEnter(BaseCreature pet)
        {
            return true;
        }

        public virtual void FinishSequence()
        {
            StopTimers();

            // delete peerless
            if (m_Peerless != null)
            {
                if (m_Peerless.Corpse != null && !m_Peerless.Corpse.Deleted)
                    m_Peerless.Corpse.Delete();

                if (!m_Peerless.Deleted)
                    m_Peerless.Delete();
            }

            // teleport party to exit if not already there				
            for (int i = 0; i < m_Fighters.Count; i++)
                Exit(m_Fighters[i]);

            // delete master keys				
            for (int i = 0; i < m_MasterKeys.Count; i++)
            {
                if (m_MasterKeys[i] != null && !m_MasterKeys[i].Deleted)
                    m_MasterKeys[i].Delete();
            }

            if (m_MasterKeys != null)
                m_MasterKeys.Clear();

            if (m_Fighters != null)
                m_Fighters.Clear();

            if (m_Pets != null)
                m_Pets.Clear();

            // delete any remaining helpers
            CleanupHelpers();

            // reset summoner, boss		
            m_Peerless = null;
            m_IsAvailable = true;

            m_Deadline = DateTime.MinValue;
        }

        public virtual void Exit(Mobile fighter)
        {
            // teleport fighter
            if (fighter.NetState == null && MobileIsInBossArea(fighter.LogoutLocation))
            {
                fighter.LogoutMap = this is CitadelAltar ? Map.Tokuno : this.Map;
                fighter.LogoutLocation = m_ExitDest;
            }
            else if (MobileIsInBossArea(fighter) && fighter.Map == this.Map)
            {
                fighter.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                fighter.PlaySound(0x1FE);

                if (this is CitadelAltar)
                    fighter.MoveToWorld(m_ExitDest, Map.Tokuno);
                else
                    fighter.MoveToWorld(m_ExitDest, Map);
            }

            // teleport his pets
            if (m_Pets.ContainsKey(fighter))
            {
                for (int i = 0; i < m_Pets[fighter].Count; i++)
                {
                    BaseCreature pet = m_Pets[fighter][i] as BaseCreature;

                    if (pet != null && (pet.Alive || pet.IsBonded) && pet.Map != Map.Internal && MobileIsInBossArea(pet))
                    {
                        if (pet is BaseMount)
                        {
                            BaseMount mount = (BaseMount)pet;

                            if (mount.Rider != null && mount.Rider != fighter)
                            {
                                mount.Rider.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                                mount.Rider.PlaySound(0x1FE);

                                if (this is CitadelAltar)
                                    mount.Rider.MoveToWorld(m_ExitDest, Map.Tokuno);
                                else
                                    mount.Rider.MoveToWorld(m_ExitDest, Map);

                                continue;
                            }
                            else if (mount.Rider != null)
                                continue;
                        }

                        pet.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                        pet.PlaySound(0x1FE);

                        if (this is CitadelAltar)
                            pet.MoveToWorld(m_ExitDest, Map.Tokuno);
                        else
                            pet.MoveToWorld(m_ExitDest, Map);
                    }
                }

                m_Pets.Remove(fighter);
            }

            m_Fighters.Remove(fighter);
            fighter.SendLocalizedMessage(1072677); // You have been transported out of this room.
        }

        public virtual void OnPeerlessDeath()
        {
            SendMessage(1072681); // The master of this realm has been slain! You may only stay here so long.
            SendMessage(1075611, DelayAfterBossSlain.TotalSeconds); // Time left: ~1_time~ seconds

            StopKeyTimer();
            StopSlayTimer();

            // delete master keys				
            for (int i = m_MasterKeys.Count - 1; i >= 0; i--)
                m_MasterKeys[i].Delete();

            m_MasterKeys.Clear();

            m_DeadlineTimer = Timer.DelayCall(DelayAfterBossSlain, new TimerCallback(FinishSequence));
        }

        public virtual bool MobileIsInBossArea(Mobile check)
        {
            return MobileIsInBossArea(check.Location);
        }

        public virtual bool MobileIsInBossArea(Point3D loc)
        {
            if (BossBounds == null || BossBounds.Length == 0)
                return true;

            foreach (Rectangle2D rec in BossBounds)
            {
                if (rec.Contains(loc))
                    return true;
            }

            return false;
        }

        public virtual void SendMessage(int message)
        {
            for (int i = 0; i < m_Fighters.Count; i++)
                m_Fighters[i].SendLocalizedMessage(message);
        }

        public virtual void SendMessage(int message, object param)
        {
            for (int i = 0; i < m_Fighters.Count; i++)
                m_Fighters[i].SendLocalizedMessage(message, param.ToString());
        }

        private Timer m_KeyTimer;
        private Timer m_SlayTimer;
        private Timer m_DeadlineTimer;

        public virtual void StartTimer(Mobile from)
        {
            if (m_KeyTimer != null)
                m_KeyTimer.Stop();

            m_KeyTimer = Timer.DelayCall(TimeSpan.FromMinutes(1), new TimerStateCallback(KeyTimeout_Callback), from);
        }

        public virtual void StopTimers()
        {
            StopKeyTimer();
            StopSlayTimer();
            StopDeadlineTimer();
        }

        public virtual void StopDeadlineTimer()
        {
            if (m_DeadlineTimer != null)
                m_DeadlineTimer.Stop();

            m_DeadlineTimer = null;
        }

        public virtual void StopKeyTimer()
        {
            if (m_KeyTimer != null)
                m_KeyTimer.Stop();

            m_KeyTimer = null;
        }

        public virtual void StopSlayTimer()
        {
            if (m_SlayTimer != null)
                m_SlayTimer.Stop();

            m_SlayTimer = null;
        }

        public virtual void StartSlayTimer()
        {
            if (m_SlayTimer != null)
                m_SlayTimer.Stop();

            if (TimeToSlay != TimeSpan.Zero)
                m_Deadline = DateTime.Now + TimeToSlay;
            else
                m_Deadline = DateTime.Now + TimeSpan.FromHours(1);

            m_SlayTimer = Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), new TimerCallback(DeadlineCheck));
            m_SlayTimer.Priority = TimerPriority.OneMinute;
        }

        private void KeyTimeout_Callback(object state)
        {
            if (state is Mobile)
                KeyTimeout((Mobile)state);
        }

        public virtual void KeyTimeout(Mobile from)
        {
            ClearContainer();

            from.SendLocalizedMessage(1072679); // Your realm offering has reset. You will need to start over.
        }

        public virtual void DeadlineCheck()
        {
            if (DateTime.Now > m_Deadline)
            {
                SendMessage(1072258); // You failed to complete an objective in time!
                FinishSequence();
                return;
            }

            TimeSpan timeLeft = m_Deadline - DateTime.Now;

            if (timeLeft < TimeSpan.FromMinutes(30))
                SendMessage(1075611, timeLeft.TotalSeconds);

            for (int i = m_Fighters.Count - 1; i >= 0; i--)
            {
                if (m_Fighters[i] is PlayerMobile)
                {
                    PlayerMobile player = (PlayerMobile)m_Fighters[i];

                    if (player.NetState == null)
                    {
                        TimeSpan offline = DateTime.Now - player.LastOnline;

                        if (offline > TimeSpan.FromMinutes(10))
                            Exit(player);
                    }
                }
            }
        }

        #region Helpers
        private List<BaseCreature> m_Helpers = new List<BaseCreature>();

        public List<BaseCreature> Helpers
        {
            get { return m_Helpers; }
        }

        public void AddHelper(BaseCreature helper)
        {
            if (helper != null && helper.Alive && !helper.Deleted)
                m_Helpers.Add(helper);
        }

        public bool AllHelpersDead()
        {
            for (int i = 0; i < m_Helpers.Count; i++)
            {
                BaseCreature c = m_Helpers[i];

                if (c.Alive)
                    return false;
            }

            return true;
        }

        public void CleanupHelpers()
        {
            for (int i = 0; i < m_Helpers.Count; i++)
            {
                BaseCreature c = m_Helpers[i];

                if (c != null && c.Alive)
                    c.Kill();
            }

            m_Helpers.Clear();
        }
        #endregion
    }
}
