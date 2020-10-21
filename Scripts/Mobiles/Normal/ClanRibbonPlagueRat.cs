namespace Server.Mobiles
{
    [CorpseName("a rat corpse")]
    public class ClanRibbonPlagueRat : BaseCreature
    {
        [Constructable]
        public ClanRibbonPlagueRat()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Clan Ribbon Plague Rat";
            Body = 238;
            Hue = 52;
            BaseSoundID = 0xCC;

            SetStr(59);
            SetDex(51);
            SetInt(17);

            SetHits(92);
            SetStam(51);

            SetDamage(4, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 30.0);
            SetSkill(SkillName.Tactics, 34.0);
            SetSkill(SkillName.Wrestling, 40.0);

            Fame = 150;
            Karma = -150;

            Tamable = false;
            ControlSlots = 1;
            MinTameSkill = -0.9;
        }

        public ClanRibbonPlagueRat(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.GrainsAndHay;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
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
