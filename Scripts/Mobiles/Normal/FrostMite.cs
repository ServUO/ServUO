using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a beetle corpse")]
    public class FrostMite : BaseCreature
    {
        private static readonly Dictionary<Mobile, ExpireTimer> m_Table = new Dictionary<Mobile, ExpireTimer>();

        [Constructable]
        public FrostMite() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Frost Mite";
            this.Body = 0x590;
            this.Female = true;

            this.SetStr(1017);
            this.SetDex(164);
            this.SetInt(283);

            this.SetHits(862);

            this.SetDamage(21, 28);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Cold, 100);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 15, 25);
            this.SetResistance(ResistanceType.Cold, 90, 100);
            this.SetResistance(ResistanceType.Poison, 50, 70);
            this.SetResistance(ResistanceType.Energy, 40, 45);

            this.SetSkill(SkillName.MagicResist, 50.0, 85.0);
            this.SetSkill(SkillName.Tactics, 70.0, 105.0);
            this.SetSkill(SkillName.Wrestling, 70.0, 110.0);
            this.SetSkill(SkillName.DetectHidden, 60.0, 80.0);
            this.SetSkill(SkillName.Focus, 100.0, 115.0);

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 102.0;
        }

        public override int GetAngerSound()
        {
            return 0x4E8;
        }

        public override int GetIdleSound()
        {
            return 0x4E7;
        }

        public override int GetAttackSound()
        {
            return 0x4E6;
        }

        public override int GetHurtSound()
        {
            return 0x4E9;
        }

        public override int GetDeathSound()
        {
            return 0x4E5;
        }

        public override int Meat { get { return 5; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override bool HasAura { get { return !this.Controlled; } }
        public override int AuraRange { get { return 2; } }
        public override int AuraBaseDamage { get { return 15; } }
        public override int AuraFireDamage { get { return 0; } }
        public override int AuraColdDamage { get { return 100; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override void AuraEffect(Mobile m)
        {
            m.SendLocalizedMessage(1008111, false, this.Name); //  : The intense cold is damaging you!
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                if (m_Table.ContainsKey(defender))
                {
                    m_Table[defender].DoExpire();
                    defender.SendLocalizedMessage(1070831); // The freezing wind continues to blow!
                }
                else
                {
                    defender.SendLocalizedMessage(1070832); // An icy wind surrounds you, freezing your lungs as you breathe!
                }

                ExpireTimer timer = new ExpireTimer(defender, this);
                timer.Start();
                m_Table[defender] = timer;
            }
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_From;
            private int m_Count;
            public ExpireTimer(Mobile m, Mobile from)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Mobile = m;
                this.m_From = from;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                this.Stop();
                m_Table.Remove(this.m_Mobile);
            }

            public void DrainLife()
            {
                if (this.m_Mobile.Alive)
                    this.m_Mobile.Damage(2, this.m_From);
                else
                    this.DoExpire();
            }

            protected override void OnTick()
            {
                this.DrainLife();

                if (++this.m_Count >= 5)
                {
                    this.DoExpire();
                    this.m_Mobile.SendLocalizedMessage(1070830); // The icy wind dissipates.
                }
            }
        }

        public FrostMite(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}