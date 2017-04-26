using System;

namespace Server.Mobiles
{
    [CorpseName("a stygian drake corpse")]
    public class StygianDrake : BaseCreature
    {
        [Constructable]
        public StygianDrake()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Stygian Drake";
            this.Body = 0x58E;
            this.Hue = 32768;
            this.Female = true;
            this.BaseSoundID = 362;

            this.SetStr(790, 830);
            this.SetDex(85, 125);
            this.SetInt(400, 450);

            this.SetHits(480, 510);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 60, 70);

            this.SetSkill(SkillName.MagicResist, 95.0, 105.0);
            this.SetSkill(SkillName.Tactics, 95.0, 105.0);
            this.SetSkill(SkillName.Wrestling, 90.0, 100.0);
            this.SetSkill(SkillName.DetectHidden, 75.0);
            this.SetSkill(SkillName.Magery, 100.0);
            this.SetSkill(SkillName.EvalInt, 95.0, 105.0);

            this.Fame = 5500;
            this.Karma = -5500;

            this.VirtualArmor = 46;

            this.Tamable = true;
            this.ControlSlots = 4;
            this.MinTameSkill = 85.0;

            this.PackReg(3);
        }

        public StygianDrake(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel { get { return !this.Controlled; } }
        public override bool ReacquireOnMovement { get { return !this.Controlled; } }
        public override int TreasureMapLevel { get { return 2; } }
        public override int Meat { get { return 10; } }
        public override int DragonBlood { get { return 8; } }
        public override int Hides { get { return 22; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override int Scales { get { return 2; } }
        public override ScaleType ScaleType { get { return ScaleType.Yellow; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }
        public override bool CanFly { get { return true; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

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