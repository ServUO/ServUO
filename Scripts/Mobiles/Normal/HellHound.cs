using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hell hound corpse")]
    public class HellHound : BaseCreature
    {
        [Constructable]
        public HellHound()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a hell hound";
            Body = 98;
            BaseSoundID = 229;

            SetStr(100, 350);
            SetDex(80, 300);
            SetInt(30, 180);

            SetHits(130, 300);

            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 80);

            SetResistance(ResistanceType.Physical, 0, 56);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 0, 5);
            SetSkill(SkillName.MagicResist, 0, 75);
            SetSkill(SkillName.Tactics, 0, 80);
            SetSkill(SkillName.Wrestling, 0, 80);
            SetSkill(SkillName.Necromancy, 18);
            SetSkill(SkillName.SpiritSpeak, 18);

            Fame = 3400;
            Karma = -3400;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 85.5;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public HellHound(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Canine;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LootItem<SulfurousAsh>(5));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
