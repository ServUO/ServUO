using System;

namespace Server.Mobiles
{
    [CorpseName("a dragon wolf corpse")]
    public class DragonWolf : BaseCreature
    {
        [Constructable]
        public DragonWolf()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dragon wolf";
            Body = 719;
            BaseSoundID = 0x5ED;

            SetStr(750, 850);
            SetDex(60, 75);
            SetInt(50, 55);

            SetHits(800, 860);

            SetDamage(20, 25);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Anatomy, 60.0, 70.0);
            SetSkill(SkillName.MagicResist, 125.0, 140.0);
            SetSkill(SkillName.Tactics, 95.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 105.0);
            SetSkill(SkillName.DetectHidden, 60.0);

            Fame = 8500;
            Karma = -8500;
            
            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 102.0;
        }

        public DragonWolf(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool CanAngerOnTame { get { return true; } }

        public override int Meat { get { return 4; } }
        public override int Hides { get { return 25; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);
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
