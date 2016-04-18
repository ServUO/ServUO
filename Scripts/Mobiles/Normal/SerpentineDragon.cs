using System;

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class SerpentineDragon : BaseCreature
    {
        [Constructable]
        public SerpentineDragon()
            : base(AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.Name = "a serpentine dragon";
            this.Body = 103;
            this.BaseSoundID = 362;

            this.SetStr(111, 140);
            this.SetDex(201, 220);
            this.SetInt(1001, 1040);

            this.SetHits(480);

            this.SetDamage(5, 12);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Poison, 25);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.EvalInt, 100.1, 110.0);
            this.SetSkill(SkillName.Magery, 110.1, 120.0);
            this.SetSkill(SkillName.Meditation, 100.0);
            this.SetSkill(SkillName.MagicResist, 100.0);
            this.SetSkill(SkillName.Tactics, 50.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 30.1, 100.0);

            this.Fame = 15000;
            this.Karma = 15000;

            this.VirtualArmor = 36;

            if (Core.ML && Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomPeculiarSeed(2));
        }

        public SerpentineDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override double BonusPetDamageScalar
        {
            get
            {
                return (Core.SE) ? 3.0 : 1.0;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }
        public override int Scales
        {
            get
            {
                return 6;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return (Utility.RandomBool() ? ScaleType.Black : ScaleType.White);
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
            this.AddLoot(LootPack.Gems, 2);
        }

        public override int GetIdleSound()
        {
            return 0x2C4;
        }

        public override int GetAttackSound()
        {
            return 0x2C0;
        }

        public override int GetDeathSound()
        {
            return 0x2C1;
        }

        public override int GetAngerSound()
        {
            return 0x2C4;
        }

        public override int GetHurtSound()
        {
            return 0x2C3;
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (!Core.SE && 0.2 > Utility.RandomDouble() && attacker is BaseCreature)
            {
                BaseCreature c = (BaseCreature)attacker;

                if (c.Controlled && c.ControlMaster != null)
                {
                    c.ControlTarget = c.ControlMaster;
                    c.ControlOrder = OrderType.Attack;
                    c.Combatant = c.ControlMaster;
                }
            }
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
    }
}