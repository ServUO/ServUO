using System;

namespace Server.Mobiles
{
    [CorpseName("a platinum drake corpse")]
    public class PlatinumDrake : BaseCreature
    {
        [Constructable]
        public PlatinumDrake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Platinum Drake";
            this.Body = 0x589;
            this.Female = true;
            this.BaseSoundID = 362;

            this.SetStr(400, 430);
            this.SetDex(133, 152);
            this.SetInt(101, 140);

            this.SetHits(241, 258);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 50);
            this.SetResistance(ResistanceType.Fire, 30, 50);
            this.SetResistance(ResistanceType.Cold, 30, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 30, 50);

            this.SetSkill(SkillName.MagicResist, 65.1, 80.0);
            this.SetSkill(SkillName.Tactics, 65.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 65.1, 80.0);
            this.SetSkill(SkillName.DetectHidden, 50.0, 60.0);
            this.SetSkill(SkillName.Focus, 5.0, 20.0);

            this.Fame = 5500;
            this.Karma = -5500;

            this.VirtualArmor = 46;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 85.0;

            this.PackReg(3);
        }

        public PlatinumDrake(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement { get { return true; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int TreasureMapLevel { get { return 2; } }
        public override int Meat { get { return 10; } }
        public override int DragonBlood { get { return 8; } }
        public override int Hides { get { return 22; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override int Scales { get { return 2; } }
        public override ScaleType ScaleType { get { return ScaleType.Black; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }
        public override bool CanFly { get { return true; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls, 2);
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