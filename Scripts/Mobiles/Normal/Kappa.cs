using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a kappa corpse")]
    public class Kappa : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public Kappa()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a kappa";
            this.Body = 240;

            this.SetStr(186, 230);
            this.SetDex(51, 75);
            this.SetInt(41, 55);

            this.SetMana(30);

            this.SetHits(151, 180);

            this.SetDamage(6, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 50);
            this.SetResistance(ResistanceType.Fire, 35, 50);
            this.SetResistance(ResistanceType.Cold, 25, 50);
            this.SetResistance(ResistanceType.Poison, 35, 50);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 60.1, 70.0);
            this.SetSkill(SkillName.Tactics, 79.1, 89.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 70.0);

            this.Fame = 1700;
            this.Karma = -1700;

            this.PackItem(new RawFishSteak(3));
            for (int i = 0; i < 2; i++)
            {
                switch ( Utility.Random(6) )
                {
                    case 0:
                        this.PackItem(new Gears());
                        break;
                    case 1:
                        this.PackItem(new Hinge());
                        break;
                    case 2:
                        this.PackItem(new Axle());
                        break;
                }
            }
            if (Core.ML && Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomPeculiarSeed(4));
        }

        public Kappa(Serial serial)
            : base(serial)
        {
        }

        public static bool IsBeingDrained(Mobile m)
        {
            return m_Table.Contains(m);
        }

        public static void BeginLifeDrain(Mobile m, Mobile from)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            t = new InternalTimer(from, m);
            m_Table[m] = t;

            t.Start();
        }

        public static void DrainLife(Mobile m, Mobile from)
        {
            if (m.Alive)
            {
                int damageGiven = AOS.Damage(m, from, 5, 0, 0, 0, 0, 100);
                from.Hits += damageGiven;
            }
            else
            {
                EndLifeDrain(m);
            }
        }

        public static void EndLifeDrain(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);

            m.SendLocalizedMessage(1070849); // The drain on your life force is gone.
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.Average);
        }

        public override int GetAngerSound()
        {
            return 0x50B;
        }

        public override int GetIdleSound()
        {
            return 0x50A;
        }

        public override int GetAttackSound()
        {
            return 0x509;
        }

        public override int GetHurtSound()
        {
            return 0x50C;
        }

        public override int GetDeathSound()
        {
            return 0x508;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomBool())
            {
                if (!IsBeingDrained(defender) && this.Mana > 14)
                {
                    defender.SendLocalizedMessage(1070848); // You feel your life force being stolen away.
                    BeginLifeDrain(defender, this);
                    this.Mana -= 15;
                }
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && from.Map != null)
            {
                int amt = 0;
                Mobile target = this; 
                int rand = Utility.Random(1, 100);
                if (willKill)
                {
                    amt = (((rand % 5) >> 2) + 3);
                }
                if ((this.Hits < 100) && (rand < 21)) 
                {
                    target = (rand % 2) < 1 ? this : from;
                    amt++;
                }
                if (amt > 0)
                {
                    this.SpillAcid(target, amt);
                    from.SendLocalizedMessage(1070820); 
                    if (this.Mana > 14)
                        this.Mana -= 15;
                    amt ^= amt;
                }
            }
            base.OnDamage(amount, from, willKill);
        }

        public override Item NewHarmfulItem()
        {
            return new AcidSlime(TimeSpan.FromSeconds(10), 5, 10);
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
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly Mobile m_Mobile;
            private int m_Count;
            public InternalTimer(Mobile from, Mobile m)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_From = from;
                this.m_Mobile = m;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                DrainLife(this.m_Mobile, this.m_From);

                if (this.Running && ++this.m_Count == 5)
                    EndLifeDrain(this.m_Mobile);
            }
        }
    }
}