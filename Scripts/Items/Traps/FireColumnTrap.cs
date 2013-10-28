using System;

namespace Server.Items
{
    public class FireColumnTrap : BaseTrap
    {
        private int m_MinDamage;
        private int m_MaxDamage;
        private bool m_WarningFlame;
        [Constructable]
        public FireColumnTrap()
            : base(0x1B71)
        {
            this.m_MinDamage = 10;
            this.m_MaxDamage = 40;

            this.m_WarningFlame = true;
        }

        public FireColumnTrap(Serial serial)
            : base(serial)
        {
        }

        public override bool PassivelyTriggered
        {
            get
            {
                return true;
            }
        }
        public override TimeSpan PassiveTriggerDelay
        {
            get
            {
                return TimeSpan.FromSeconds(2.0);
            }
        }
        public override int PassiveTriggerRange
        {
            get
            {
                return 3;
            }
        }
        public override TimeSpan ResetDelay
        {
            get
            {
                return TimeSpan.FromSeconds(0.5);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int MinDamage
        {
            get
            {
                return this.m_MinDamage;
            }
            set
            {
                this.m_MinDamage = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int MaxDamage
        {
            get
            {
                return this.m_MaxDamage;
            }
            set
            {
                this.m_MaxDamage = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool WarningFlame
        {
            get
            {
                return this.m_WarningFlame;
            }
            set
            {
                this.m_WarningFlame = value;
            }
        }
        public override void OnTrigger(Mobile from)
        {
            if (from.IsStaff())
                return;

            if (this.WarningFlame)
                this.DoEffect();

            if (from.Alive && this.CheckRange(from.Location, 0))
            {
                Spells.SpellHelper.Damage(TimeSpan.FromSeconds(0.5), from, from, Utility.RandomMinMax(this.MinDamage, this.MaxDamage), 0, 100, 0, 0, 0);

                if (!this.WarningFlame)
                    this.DoEffect();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(this.m_WarningFlame);
            writer.Write(this.m_MinDamage);
            writer.Write(this.m_MaxDamage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_WarningFlame = reader.ReadBool();
                        this.m_MinDamage = reader.ReadInt();
                        this.m_MaxDamage = reader.ReadInt();
                        break;
                    }
            }

            if (version == 0)
            {
                this.m_WarningFlame = true;
                this.m_MinDamage = 10;
                this.m_MaxDamage = 40;
            }
        }

        private void DoEffect()
        {
            Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
            Effects.PlaySound(this.Location, this.Map, 0x225);
        }
    }
}