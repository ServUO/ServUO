using System;
using Server.Items;

namespace Server.Mobiles
{
    public class ShadowFiend : BaseCreature
    {
        private UnhideTimer m_Timer;
        [Constructable]
        public ShadowFiend()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a shadow fiend";
            this.Body = 0xA8;

            // this to allow shadow fiend to loot from corpses
            Backpack backpack = new Backpack();
            backpack.Movable = false;
            this.AddItem(backpack);

            this.SetStr(46, 55);
            this.SetDex(121, 130);
            this.SetInt(46, 55);

            this.SetHits(28, 33);
            this.SetStam(46, 55);

            this.SetDamage(10, 22);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 80);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 20, 25);
            this.SetResistance(ResistanceType.Cold, 40, 45);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 20.1, 30.0);
            this.SetSkill(SkillName.Tactics, 20.1, 30.0);
            this.SetSkill(SkillName.Wrestling, 20.1, 30.0);

            this.Fame = 1000;
            this.Karma = -1000;

            this.m_Timer = new UnhideTimer(this);
            this.m_Timer.Start();
        }

        public ShadowFiend(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return true;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int GetIdleSound()
        {
            return 0x37A;
        }

        public override int GetAngerSound()
        {
            return 0x379;
        }

        public override int GetDeathSound()
        {
            return 0x381;
        }

        public override int GetAttackSound()
        {
            return 0x37F;
        }

        public override int GetHurtSound()
        {
            return 0x380;
        }

        public override bool OnBeforeDeath()
        {
            if (this.Backpack != null)
                this.Backpack.Destroy();

            Effects.SendLocationEffect(this.Location, this.Map, 0x376A, 10, 1);
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_Timer = new UnhideTimer(this);
            this.m_Timer.Start();
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            base.OnAfterDelete();
        }

        private class UnhideTimer : Timer
        {
            private readonly ShadowFiend m_Owner;
            public UnhideTimer(ShadowFiend owner)
                : base(TimeSpan.FromSeconds(30.0))
            {
                this.m_Owner = owner;
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (this.m_Owner.Deleted)
                {
                    this.Stop();
                    return;
                }

                IPooledEnumerable eable = m_Owner.GetMobilesInRange(3);

                foreach (Mobile m in eable)
                {
                    if (m != this.m_Owner && m.Player && m.Hidden && this.m_Owner.CanBeHarmful(m) && m.IsPlayer())
                        m.Hidden = false;
                }
                eable.Free();
            }
        }
    }
}