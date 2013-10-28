using System;

namespace Server.Mobiles
{
    [CorpseName("a shadow wyrm corpse")]
    public class ShadowWyrm : BaseCreature
    {
        [Constructable]
        public ShadowWyrm()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a shadow wyrm";
            this.Body = 106;
            this.BaseSoundID = 362;

            this.SetStr(898, 1030);
            this.SetDex(68, 200);
            this.SetInt(488, 620);

            this.SetHits(558, 599);

            this.SetDamage(29, 35);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Cold, 25);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 45, 55);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.EvalInt, 80.1, 100.0);
            this.SetSkill(SkillName.Magery, 80.1, 100.0);
            this.SetSkill(SkillName.Meditation, 52.5, 75.0);
            this.SetSkill(SkillName.MagicResist, 100.3, 130.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 97.6, 100.0);

            this.Fame = 22500;
            this.Karma = -22500;

            this.VirtualArmor = 70;
        }

        public ShadowWyrm(Serial serial)
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
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override int Scales
        {
            get
            {
                return 10;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return ScaleType.Black;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            this.AddLoot(LootPack.Gems, 5);
        }

        public override int GetIdleSound()
        {
            return 0x2D5;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
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