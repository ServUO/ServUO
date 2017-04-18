using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class FrostDragon : BaseCreature
    {
        [Constructable]
        public FrostDragon() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a frost dragon";
            this.Body = Utility.RandomList(12, 59);
            this.BaseSoundID = 362;

            this.Hue = Utility.RandomMinMax(1319, 1327);

            this.SetStr(1300, 1400);
            this.SetDex(100, 125);
            this.SetInt(600, 700);

            this.SetHits(2050, 2250);

            this.SetDamage(24, 33);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Cold, 50);

            this.SetResistance(ResistanceType.Physical, 85, 92);
            this.SetResistance(ResistanceType.Fire, 55, 70);
            this.SetResistance(ResistanceType.Cold, 85, 95);
            this.SetResistance(ResistanceType.Poison, 65, 70);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetSkill(SkillName.EvalInt, 50, 60);
            this.SetSkill(SkillName.Magery, 120, 130);
            this.SetSkill(SkillName.MagicResist, 115, 135);
            this.SetSkill(SkillName.Tactics, 120, 135);
            this.SetSkill(SkillName.Wrestling, 120, 130);
            this.SetSkill(SkillName.Meditation, 1, 50);

            this.Fame = 25000;
            this.Karma = -25000;

            this.VirtualArmor = 60;

            this.Tamable = true;
            this.ControlSlots = 5;
            this.MinTameSkill = 105.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.Gems, 8);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }


        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }
        public override bool ReacquireOnMovement { get { return !Controlled; } }
        public override bool AutoDispel { get { return !Controlled; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 33; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int DragonBlood { get { return 8; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathColdDamage { get { return 100; } }
        public override int BreathEffectHue { get { return 1264; } }

        public override bool CanAreaDamage { get { return true; } }
        public override int AreaDamageRange { get { return 10; } }
        public override double AreaDamageScalar { get { return 1.0; } }
        public override double AreaDamageChance { get { return 1.0; } }
        public override TimeSpan AreaDamageDelay { get { return TimeSpan.FromSeconds(30); } }

        public override int AreaFireDamage { get { return 0; } }
        public override int AreaColdDamage { get { return 100; } }

        public override void AreaDamageEffect(Mobile m)
        {
            m.FixedParticles(0x374A, 10, 30, 5052, this.Hue, 0, EffectLayer.Waist);
            m.PlaySound(0x5C6);
        }

        public FrostDragon(Serial serial) : base(serial)
        {
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