using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a plague beast lord corpse")]
    public class PlagueBeastLord : BaseCreature, ICarvable, IScissorable
    {
        private Mobile m_OpenedBy;
        private DecayTimer m_Timer;
        [Constructable]
        public PlagueBeastLord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a plague beast lord";
            this.Body = 775;
            this.BaseSoundID = 679;
            this.SpeechHue = 0x3B2;

            this.SetStr(500);
            this.SetDex(100);
            this.SetInt(30);

            this.SetHits(1800);

            this.SetDamage(20, 25);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Poison, 25);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 75, 85);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.Tactics, 100);
            this.SetSkill(SkillName.Wrestling, 100);

            this.Fame = 2000;
            this.Karma = -2000;

            this.VirtualArmor = 50;
        }

        public PlagueBeastLord(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile OpenedBy
        {
            get
            {
                return this.m_OpenedBy;
            }
            set
            {
                this.m_OpenedBy = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsBleeding
        {
            get
            {
                Container pack = this.Backpack;

                if (pack != null)
                {
                    for (int i = 0; i < pack.Items.Count; i++)
                    {
                        PlagueBeastBlood blood = pack.Items[i] as PlagueBeastBlood;

                        if (blood != null && !blood.Patched)
                            return true;
                    }
                }

                return false;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsAccessibleTo(from))
            {
                if (this.m_OpenedBy != null && this.Backpack != null)
                    this.Backpack.DisplayTo(from);
                else
                    this.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1071917, from.NetState); // * You attempt to tear open the amorphous flesh, but it resists *
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (this.IsAccessibleTo(from) && (dropped is PlagueBeastInnard || dropped is PlagueBeastGland))
                return base.OnDragDrop(from, dropped);

            return false;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
			
            for (int i = c.Items.Count - 1; i >= 0; i--)
                c.Items[i].Delete();
        }

        public override void OnDelete()
        {
            if (this.m_OpenedBy != null && this.m_OpenedBy.Holding is PlagueBeastInnard)
                this.m_OpenedBy.Holding.Delete();

            if (this.Backpack != null)
            {
                for (int i = this.Backpack.Items.Count - 1; i >= 0; i--)
                    this.Backpack.Items[i].Delete();

                this.Backpack.Delete();
            }

            base.OnDelete();
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (this.Backpack != null && this.IsAccessibleTo(m) && m.InRange(oldLocation, 3) && !m.InRange(this, 3))
                this.Backpack.SendRemovePacket();
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            return true;
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            return true;
        }

        public override bool IsSnoop(Mobile from)
        {
            return false;
        }

        public override int GetIdleSound()
        {
            return 0x1BF;
        }

        public override int GetAttackSound()
        {
            return 0x1C0;
        }

        public override int GetHurtSound()
        {
            return 0x1C1;
        }

        public override int GetDeathSound()
        {
            return 0x1C2;
        }

        public virtual void OnParalyzed(Mobile from)
        {
            this.FightMode = FightMode.None;
            this.Frozen = true;
            this.Blessed = true;
            this.Combatant = null;
            this.Hue = 0x480;
            from.Combatant = null;
            from.Warmode = false;

            this.m_Timer = new DecayTimer(this);
            this.m_Timer.Start();

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(BroadcastMessage));
        }

        public virtual bool IsAccessibleTo(Mobile check)
        {
            if (check.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (!this.InRange(check, 2))
                this.PrivateOverheadMessage(MessageType.Label, 0x3B2, 500446, check.NetState); // That is too far away.
            else if (this.m_OpenedBy != null && this.m_OpenedBy != check)
                this.PrivateOverheadMessage(MessageType.Label, 0x3B2, 500365, check.NetState); // That is being used by someone else
            else if (this.Frozen)
                return true;

            return false;
        }

        public virtual bool Carve(Mobile from, Item item)
        {
            if (this.m_OpenedBy == null && this.IsAccessibleTo(from))
            {
                this.m_OpenedBy = from;
				
                if (this.m_Timer == null)
                    this.m_Timer = new DecayTimer(this);
				
                if (!this.m_Timer.Running)
                    this.m_Timer.Start();

                this.m_Timer.StartDissolving();

                PlagueBeastBackpack pack = new PlagueBeastBackpack();
                this.AddItem(pack);
                pack.Initialize();

                foreach (NetState state in this.GetClientsInRange(12))
                {
                    Mobile m = state.Mobile;

                    if (m != null && m.Player && m != from)
                        this.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1071919, from.Name, m.NetState); // * ~1_VAL~ slices through the plague beast's amorphous tissue *
                }

                from.LocalOverheadMessage(MessageType.Regular, 0x21, 1071904); // * You slice through the plague beast's amorphous tissue *
                Timer.DelayCall<Mobile>(TimeSpan.Zero, new TimerStateCallback<Mobile>(pack.Open), from);

                return true;
            }

            return false;
        }

        public virtual bool Scissor(Mobile from, Scissors scissors)
        {
            if (this.IsAccessibleTo(from))
                scissors.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071918);  // You can't cut through the plague beast's amorphous skin with scissors!

            return false;
        }

        public void Unfreeze()
        {
            this.FightMode = FightMode.Closest;
            this.Frozen = false;
            this.Blessed = false;

            if (this.m_OpenedBy == null)
                this.Hue = 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_OpenedBy);

            if (this.m_Timer != null)
            {
                writer.Write((bool)true);
                writer.Write((int)this.m_Timer.Count);
                writer.Write((int)this.m_Timer.Deadline);
            }
            else
                writer.Write((bool)false);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_OpenedBy = reader.ReadMobile();

            if (reader.ReadBool())
            {
                int count = reader.ReadInt();
                int deadline = reader.ReadInt();

                this.m_Timer = new DecayTimer(this, count, deadline);
                this.m_Timer.Start();
            }

            if (this.FightMode == FightMode.None)
                this.Frozen = true;
        }

        private void BroadcastMessage()
        {
            this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071920); // * The plague beast's amorphous flesh hardens and becomes immobilized *
        }

        private class DecayTimer : Timer
        {
            private readonly PlagueBeastLord m_Lord;
            private int m_Count;
            private int m_Deadline;
            public DecayTimer(PlagueBeastLord lord)
                : this(lord, 0, 120)
            {
            }

            public DecayTimer(PlagueBeastLord lord, int count, int deadline)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                this.m_Lord = lord;
                this.m_Count = count;
                this.m_Deadline = deadline;
            }

            public int Count
            {
                get
                {
                    return this.m_Count;
                }
            }
            public int Deadline
            {
                get
                {
                    return this.m_Deadline;
                }
            }
            public void StartDissolving()
            {
                this.m_Deadline = Math.Min(this.m_Count + 60, this.m_Deadline);
            }

            protected override void OnTick()
            {
                if (this.m_Lord == null || this.m_Lord.Deleted)
                {
                    this.Stop();
                    return;
                }

                if (this.m_Count + 15 == this.m_Deadline)
                {
                    if (this.m_Lord.OpenedBy != null)
                        this.m_Lord.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071921); // * The plague beast begins to bubble and dissolve! *

                    this.m_Lord.PlaySound(0x103);
                }
                else if (this.m_Count + 10 == this.m_Deadline)
                {
                    this.m_Lord.PlaySound(0x21);
                }
                else if (this.m_Count + 5 == this.m_Deadline)
                {
                    this.m_Lord.PlaySound(0x1C2);
                }
                else if (this.m_Count == this.m_Deadline)
                {
                    this.m_Lord.Unfreeze();

                    if (this.m_Lord.OpenedBy != null)
                        this.m_Lord.Kill();

                    this.Stop();
                }
                else if (this.m_Count % 15 == 0)
                    this.m_Lord.PlaySound(0x1BF);

                this.m_Count++;
            }
        }
    }
}