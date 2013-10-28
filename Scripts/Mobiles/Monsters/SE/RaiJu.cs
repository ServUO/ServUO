using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a rai-ju corpse")]
    public class RaiJu : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public RaiJu()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Rai-Ju";
            this.Body = 199;
            this.BaseSoundID = 0x346;

            this.SetStr(151, 225);
            this.SetDex(81, 135);
            this.SetInt(176, 180);

            this.SetHits(201, 280);

            this.SetDamage(12, 15);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Fire, 10);
            this.SetDamageType(ResistanceType.Cold, 10);
            this.SetDamageType(ResistanceType.Poison, 10);
            this.SetDamageType(ResistanceType.Energy, 60);

            this.SetResistance(ResistanceType.Physical, 45, 65);
            this.SetResistance(ResistanceType.Fire, 70, 85);
            this.SetResistance(ResistanceType.Cold, 30, 60);
            this.SetResistance(ResistanceType.Poison, 50, 70);
            this.SetResistance(ResistanceType.Energy, 60, 80);

            this.SetSkill(SkillName.Wrestling, 85.1, 95.0);
            this.SetSkill(SkillName.Tactics, 55.1, 65.0);
            this.SetSkill(SkillName.MagicResist, 110.1, 125.0);
            this.SetSkill(SkillName.Anatomy, 25.1, 35.0);
		
            this.Fame = 8000;
            this.Karma = -8000;
        }

        public RaiJu(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 2);
            this.AddLoot(LootPack.Gems, 2);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble() && !this.IsStunned(defender))
            {
                /* Lightning Fist
                * Cliloc: 1070839
                * Effect: Type: "3" From: "0x57D4F5B" To: "0x0" ItemId: "0x37B9" ItemIdName: "glow" FromLocation: "(884 715, 10)" ToLocation: "(884 715, 10)" Speed: "10" Duration: "5" FixedDirection: "True" Explode: "False"
                * Damage: 35-65, 100% energy, resistable
                * Freezes for 4 seconds
                * Effect cannot stack
                */
                defender.FixedEffect(0x37B9, 10, 5);
                defender.SendLocalizedMessage(1070839); // The creature attacks with stunning force!
 
                // This should be done in place of the normal attack damage.
                //AOS.Damage( defender, this, Utility.RandomMinMax( 35, 65 ), 0, 0, 0, 0, 100 );

                defender.Frozen = true; 

                ExpireTimer timer = new ExpireTimer(defender, TimeSpan.FromSeconds(4.0));
                timer.Start();
                m_Table[defender] = timer;
            }
        }

        public bool IsStunned(Mobile m)
        {
            return m_Table.Contains(m);
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

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public ExpireTimer(Mobile m, TimeSpan delay)
                : base(delay)
            {
                this.m_Mobile = m;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                this.m_Mobile.Frozen = false;
                this.Stop();
                m_Table.Remove(this.m_Mobile);
            }

            protected override void OnTick()
            {
                this.m_Mobile.SendLocalizedMessage(1005603); // You can move again!
                this.DoExpire();
            }
        }
    }
}