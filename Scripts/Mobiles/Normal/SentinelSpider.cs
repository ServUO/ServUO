using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sentinel spider corpse")]
    public class SentinelSpider : BaseCreature
    {
        [Constructable]
        public SentinelSpider() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Sentinel spider";
            Body = 0x9d;
            Hue = 1141;
            BaseSoundID = 0x388;

            SetStr(95, 100);
            SetDex(140, 145);
            SetInt(40, 45);

            SetHits(260, 265);

            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 50);
            SetResistance(ResistanceType.Fire, 30, 35);
            SetResistance(ResistanceType.Cold, 30, 35);
            SetResistance(ResistanceType.Poison, 70, 75);
            SetResistance(ResistanceType.Energy, 30, 35);

            SetSkill(SkillName.Anatomy, 85.0, 90.0);
            SetSkill(SkillName.MagicResist, 88.5, 90.0);
            SetSkill(SkillName.Tactics, 102.9, 105.0);
            SetSkill(SkillName.Wrestling, 119.1, 120.0);
            SetSkill(SkillName.Poisoning, 101.0, 102.0);

            Fame = 775;
            Karma = -775;

            SetWeaponAbility(WeaponAbility.ArmorIgnore);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Poor);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (!Controlled && Utility.RandomDouble() < 0.03)
                c.DropItem(new LuckyCoin());
        }

        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Arachnid;

        public SentinelSpider(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}