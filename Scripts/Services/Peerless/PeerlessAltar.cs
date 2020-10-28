using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class PeerlessKeyArray
    {
        public Type Key { get; set; }
        public bool Active { get; set; }
    }

    public abstract class PeerlessAltar : Container
    {
        public override bool IsPublicContainer => true;
        public override bool IsDecoContainer => false;

        public virtual TimeSpan TimeToSlay => TimeSpan.FromMinutes(90);
        public virtual TimeSpan DelayAfterBossSlain => TimeSpan.FromMinutes(15);

        public abstract int KeyCount { get; }
        public abstract MasterKey MasterKey { get; }

        protected List<PeerlessKeyArray> KeyValidation;

        public abstract Type[] Keys { get; }
        public abstract BasePeerless Boss { get; }

        public abstract Rectangle2D[] BossBounds { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BasePeerless Peerless { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BossLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TeleportDest { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D ExitDest { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Deadline { get; set; }

        [CommandProperty(AccessLevel.Counselor)]
        public bool ResetPeerless
        {
            get { return false; }
            set { if (value == true) FinishSequence(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FighterCount => Fighters != null ? Fighters.Count : 0;

        public List<Mobile> Fighters { get; set; }

        public List<Item> MasterKeys { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        public PeerlessAltar(int itemID)
            : base(itemID)
        {
            Movable = false;

            Fighters = new List<Mobile>();
            MasterKeys = new List<Item>();
        }

        public PeerlessAltar(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                base.OnDoubleClick(from);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return base.CheckLift(from, item, ref reject);
            else
                reject = LRReason.CannotLift;

            return false;
        }

        public bool CheckParty(Mobile from)
        {
            if (Owner == null)
            {
                return false;
            }

            if (Owner == from)
            {
                return true;
            }

            var party = Party.Get(Owner);

            if (party == null)
            {
                return false;
            }

            return party == Party.Get(from);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (Owner != null && Owner != from)
            {
                if (Peerless != null && Peerless.CheckAlive())
                    from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
                else
                    from.SendLocalizedMessage(1072683, Owner.Name); // ~1_NAME~ has already activated the Prism, please wait...

                return false;
            }

            if (IsKey(dropped) && MasterKeys.Count == 0)
            {
                if (KeyValidation == null)
                {
                    KeyValidation = new List<PeerlessKeyArray>();

                    for (int i = 0; i < Keys.Length; i++)
                    {
                        KeyValidation.Add(new PeerlessKeyArray { Key = Keys[i], Active = false });
                    }
                }

                if (KeyValidation.Any(x => x.Active))
                {
                    if (KeyValidation.Any(x => x.Key == dropped.GetType() && !x.Active))
                    {
                        KeyValidation.Find(s => s.Key == dropped.GetType()).Active = true;
                    }
                    else
                    {
                        from.SendLocalizedMessage(1072682); // This is not the proper key.
                        return false;
                    }
                }
                else
                {
                    Owner = from;
                    KeyStartTimer(from);
                    from.SendLocalizedMessage(1074575); // You have activated this object!
                    KeyValidation.Find(s => s.Key == dropped.GetType()).Active = true;
                }

                if (KeysValidated())
                {
                    ActivateEncounter(from);
                }
            }
            else
            {
                from.SendLocalizedMessage(1072682); // This is not the proper key.
                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        public virtual void ActivateEncounter(Mobile from)
        {
            KeyStopTimer();

            from.SendLocalizedMessage(1072678); // You have awakened the master of this realm. You need to hurry to defeat it in time!
            BeginSequence(from);

            for (int k = 0; k < KeyCount; k++)
            {
                from.SendLocalizedMessage(1072680); // You have been given the key to the boss.

                MasterKey key = MasterKey;

                if (key != null)
                {
                    key.Altar = this;
                    key.PeerlessMap = Map;

                    if (!from.AddToBackpack(key))
                        key.MoveToWorld(from.Location, from.Map);

                    MasterKeys.Add(key);
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(1), () => ClearContainer());
            KeyValidation = null;
        }

        public bool KeysValidated()
        {
            if (KeyValidation == null)
            {
                return false;
            }

            return KeyValidation.Count(x => x.Active) == Keys.Length;
        }

        public virtual bool IsKey(Item item)
        {
            if (Keys == null || item == null)
                return false;

            bool isKey = false;

            // check if item is key	
            for (int i = 0; i < Keys.Length && !isKey; i++)
            {
                if (Keys[i].IsAssignableFrom(item.GetType()))
                    isKey = true;
            }

            // check if item is already in container			
            for (int i = 0; i < Items.Count && isKey; i++)
            {
                if (Items[i].GetType() == item.GetType())
                    return false;
            }

            return isKey;
        }

        private Timer m_KeyResetTimer;

        public virtual void KeyStartTimer(Mobile from)
        {
            if (m_KeyResetTimer != null)
                m_KeyResetTimer.Stop();

            m_KeyResetTimer = Timer.DelayCall(TimeSpan.FromSeconds(30 * Keys.Count()), () =>
            {
                from.SendLocalizedMessage(1072679); // Your realm offering has reset. You will need to start over.

                if (Owner != null)
                {
                    Owner = null;
                }

                KeyValidation = null;

                ClearContainer();
            });
        }

        public virtual void KeyStopTimer()
        {
            if (m_KeyResetTimer != null)
                m_KeyResetTimer.Stop();

            m_KeyResetTimer = null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(5); // version

            writer.Write(Owner);

            // version 4 remove pet table

            // version 3 remove IsAvailable

            // version 1
            writer.Write(m_Helpers != null);

            if (m_Helpers != null)
                writer.WriteMobileList(m_Helpers);

            // version 0			
            writer.Write(Peerless);
            writer.Write(BossLocation);
            writer.Write(TeleportDest);
            writer.Write(ExitDest);

            writer.Write(Deadline);

            // serialize master keys						
            writer.WriteItemList(MasterKeys);

            // serialize fighters							
            writer.WriteMobileList(Fighters);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 5:
                    {
                        Owner = reader.ReadMobile();
                        goto case 4;
                    }
                case 4:
                case 3:
                    if (version < 5)
                    {
                        reader.ReadBool();
                    }
                    goto case 2;
                case 2:
                case 1:
                    if (reader.ReadBool())
                        m_Helpers = reader.ReadStrongMobileList<BaseCreature>();
                    goto case 0;
                case 0:
                    Peerless = reader.ReadMobile() as BasePeerless;
                    BossLocation = reader.ReadPoint3D();
                    TeleportDest = reader.ReadPoint3D();
                    ExitDest = reader.ReadPoint3D();

                    Deadline = reader.ReadDateTime();

                    MasterKeys = reader.ReadStrongItemList();
                    Fighters = reader.ReadStrongMobileList();

                    if (version < 4)
                    {
                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            reader.ReadMobile();
                            reader.ReadStrongMobileList();
                        }
                    }

                    if (version < 2)
                        reader.ReadBool();

                    if (Peerless == null && m_Helpers.Count > 0)
                        Timer.DelayCall(TimeSpan.FromSeconds(30), CleanupHelpers);

                    break;
            }


            if (Owner != null && Peerless == null)
            {
                FinishSequence();
            }
        }

        public virtual void ClearContainer()
        {
            for (int i = Items.Count - 1; i >= 0; --i)
            {
                if (i < Items.Count)
                    Items[i].Delete();
            }
        }

        public virtual void AddFighter(Mobile fighter)
        {
            if (!Fighters.Contains(fighter))
                Fighters.Add(fighter);
        }

        public virtual void SendConfirmations(Mobile from)
        {
            Party party = Party.Get(from);

            if (party != null)
            {
                foreach (Mobile m in party.Members.Select(info => info.Mobile))
                {
                    if (m.InRange(from.Location, 25) && CanEnter(m))
                    {
                        m.SendGump(new ConfirmEntranceGump(this, from));
                    }
                }
            }
            else
            {
                from.SendGump(new ConfirmEntranceGump(this, from));
            }
        }

        public virtual void BeginSequence(Mobile from)
        {
            SpawnBoss();
        }

        public virtual void SpawnBoss()
        {
            if (Peerless == null)
            {
                // spawn boss
                Peerless = Boss;

                if (Peerless == null)
                    return;

                Peerless.Home = BossLocation;
                Peerless.RangeHome = 12;
                Peerless.MoveToWorld(BossLocation, Map);
                Peerless.Altar = this;

                StartSlayTimer();
            }
        }

        public void Enter(Mobile fighter)
        {
            if (CanEnter(fighter))
            {
                // teleport party member's pets
                if (fighter is PlayerMobile)
                {
                    foreach (BaseCreature pet in ((PlayerMobile)fighter).AllFollowers.OfType<BaseCreature>().Where(pet => pet.Alive &&
                                                                                                                 pet.InRange(fighter.Location, 5) &&
                                                                                                                 !(pet is BaseMount &&
                                                                                                                 ((BaseMount)pet).Rider != null) &&
                                                                                                                 CanEnter(pet)))
                    {
                        pet.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                        pet.PlaySound(0x1FE);
                        pet.MoveToWorld(TeleportDest, Map);
                    }
                }

                // teleport party member
                fighter.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                fighter.PlaySound(0x1FE);
                fighter.MoveToWorld(TeleportDest, Map);

                AddFighter(fighter);
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

            if (Owner != null)
            {
                Owner = null;
            }

            // delete peerless
            if (Peerless != null)
            {
                if (Peerless.Corpse != null && !Peerless.Corpse.Deleted)
                    Peerless.Corpse.Delete();

                if (!Peerless.Deleted)
                    Peerless.Delete();
            }

            // teleport party to exit if not already there
            if (Fighters != null)
            {
                var fighters = new List<Mobile>(Fighters);

                fighters.ForEach(x => Exit(x));

                ColUtility.Free(fighters);
            }

            // delete master keys
            if (MasterKeys != null)
            {
                var keys = new List<Item>(MasterKeys);

                keys.ForEach(x => x.Delete());

                ColUtility.Free(keys);
            }

            // delete any remaining helpers
            CleanupHelpers();

            // reset summoner, boss		
            Peerless = null;
            Deadline = DateTime.MinValue;

            ColUtility.Free(Fighters);
            ColUtility.Free(MasterKeys);
        }

        public virtual void Exit(Mobile fighter)
        {
            if (fighter == null)
                return;

            // teleport fighter
            if (fighter.NetState == null && MobileIsInBossArea(fighter.LogoutLocation))
            {
                fighter.LogoutMap = this is CitadelAltar ? Map.Tokuno : Map;
                fighter.LogoutLocation = ExitDest;
            }
            else if (MobileIsInBossArea(fighter) && fighter.Map == Map)
            {
                fighter.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                fighter.PlaySound(0x1FE);

                if (this is CitadelAltar)
                    fighter.MoveToWorld(ExitDest, Map.Tokuno);
                else
                    fighter.MoveToWorld(ExitDest, Map);
            }

            // teleport his pets
            if (fighter is PlayerMobile)
            {
                foreach (BaseCreature pet in ((PlayerMobile)fighter).AllFollowers.OfType<BaseCreature>().Where(pet => pet != null &&
                                                                                                             (pet.Alive || pet.IsBonded) &&
                                                                                                             pet.Map != Map.Internal &&
                                                                                                             MobileIsInBossArea(pet)))
                {
                    if (pet is BaseMount)
                    {
                        BaseMount mount = (BaseMount)pet;

                        if (mount.Rider != null && mount.Rider != fighter)
                        {
                            mount.Rider.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                            mount.Rider.PlaySound(0x1FE);

                            if (this is CitadelAltar)
                                mount.Rider.MoveToWorld(ExitDest, Map.Tokuno);
                            else
                                mount.Rider.MoveToWorld(ExitDest, Map);

                            continue;
                        }
                        else if (mount.Rider != null)
                            continue;
                    }

                    pet.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                    pet.PlaySound(0x1FE);

                    if (this is CitadelAltar)
                        pet.MoveToWorld(ExitDest, Map.Tokuno);
                    else
                        pet.MoveToWorld(ExitDest, Map);
                }
            }

            Fighters.Remove(fighter);
            fighter.SendLocalizedMessage(1072677); // You have been transported out of this room.

            if (MasterKeys.Count == 0 && Fighters.Count == 0 && Owner != null)
            {
                StopTimers();

                Owner = null;

                if (Peerless != null)
                {
                    if (Peerless.Corpse != null && !Peerless.Corpse.Deleted)
                        Peerless.Corpse.Delete();

                    if (!Peerless.Deleted)
                        Peerless.Delete();
                }

                CleanupHelpers();

                // reset summoner, boss		
                Peerless = null;

                Deadline = DateTime.MinValue;
            }
        }

        public virtual void OnPeerlessDeath()
        {
            SendMessage(1072681); // The master of this realm has been slain! You may only stay here so long.

            StopSlayTimer();

            // delete master keys
            ColUtility.SafeDelete(MasterKeys);

            ColUtility.Free(MasterKeys);
            m_DeadlineTimer = Timer.DelayCall(DelayAfterBossSlain, FinishSequence);
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
            Fighters.ForEach(x => x.SendLocalizedMessage(message));
        }

        public virtual void SendMessage(int message, object param)
        {
            Fighters.ForEach(x => x.SendLocalizedMessage(message, param.ToString()));
        }

        private Timer m_SlayTimer;
        private Timer m_DeadlineTimer;

        public virtual void StopTimers()
        {
            StopSlayTimer();
            StopDeadlineTimer();
        }

        public virtual void StopDeadlineTimer()
        {
            if (m_DeadlineTimer != null)
                m_DeadlineTimer.Stop();

            m_DeadlineTimer = null;

            if (Owner != null)
            {
                Owner = null;
            }
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
                Deadline = DateTime.UtcNow + TimeToSlay;
            else
                Deadline = DateTime.UtcNow + TimeSpan.FromHours(1);

            m_SlayTimer = Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), DeadlineCheck);
            m_SlayTimer.Priority = TimerPriority.OneMinute;
        }

        public virtual void DeadlineCheck()
        {
            if (DateTime.UtcNow > Deadline)
            {
                SendMessage(1072258); // You failed to complete an objective in time!
                FinishSequence();
                return;
            }

            TimeSpan timeLeft = Deadline - DateTime.UtcNow;

            if (timeLeft < TimeSpan.FromMinutes(30))
            {
                SendMessage(1075611, timeLeft.TotalSeconds);
            }

            var now = DateTime.UtcNow;
            var remove = Fighters.Count;

            while (--remove >= 0)
            {
                if (remove < Fighters.Count && Fighters[remove] is PlayerMobile player)
                {
                    if (player.NetState == null && (now - player.LastOnline).TotalMinutes > 10)
                        Exit(player);
                }
            }
        }

        #region Helpers
        private List<BaseCreature> m_Helpers = new List<BaseCreature>();
        public List<BaseCreature> Helpers => m_Helpers;

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
                    c.Delete();
            }

            m_Helpers.Clear();
        }
        #endregion
    }
}
