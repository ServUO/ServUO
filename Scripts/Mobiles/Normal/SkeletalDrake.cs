/* Based on Skeletal Dragon, still have to get detailed info on Skeletal Drake */
using System;

namespace Server.Mobiles
{
    [CorpseName("a skeletal drake corpse")]
    public class SkeletalDrake : BaseCreature
    {
        [Constructable]
        public SkeletalDrake()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Skeletal drake";
            this.Body = 104;
            this.BaseSoundID = 0x488;

            this.SetStr(898, 1030);
            this.SetDex(68, 200);
            this.SetInt(488, 620);

            this.SetHits(558, 599);

            this.SetDamage(29, 35);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);

            this.SetResistance(ResistanceType.Physical, 75, 80);
            this.SetResistance(ResistanceType.Fire, 40, 60);
            this.SetResistance(ResistanceType.Cold, 40, 60);
            this.SetResistance(ResistanceType.Poison, 70, 80);
            this.SetResistance(ResistanceType.Energy, 40, 60);

            this.SetSkill(SkillName.EvalInt, 80.1, 100.0);
            this.SetSkill(SkillName.Magery, 80.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 100.3, 130.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 97.6, 100.0);

            this.Fame = 22500;
            this.Karma = -22500;

            this.VirtualArmor = 80;
        }

        public SkeletalDrake(Serial serial)
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
        public override int BreathFireDamage
        {
            get
            {
                return 0;
            }
        }
        public override int BreathColdDamage
        {
            get
            {
                return 100;
            }
        }
        public override int BreathEffectHue
        {
            get
            {
                return 0x480;
            }
        }
        public override double BonusPetDamageScalar
        {
            get
            {
                return (Core.SE) ? 3.0 : 1.0;
            }
        }
        // TODO: Undead summoning?
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
                return Poison.Lethal;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }// where's it hiding these? :)
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 4);
            this.AddLoot(LootPack.Gems, 5);
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