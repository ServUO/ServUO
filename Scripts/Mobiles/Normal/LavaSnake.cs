using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lava snake corpse")]
    public class LavaSnake : BaseCreature
    {
        [Constructable]
        public LavaSnake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lava snake";
            Body = 52;
            Hue = Utility.RandomList(0x647, 0x650, 0x659, 0x662, 0x66B, 0x674);
            BaseSoundID = 0xDB;

            SetStr(43, 55);
            SetDex(16, 25);
            SetInt(6, 10);

            SetHits(28, 32);
            SetMana(0);

            SetDamage(1, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 15.1, 20.0);
            SetSkill(SkillName.Tactics, 19.3, 34.0);
            SetSkill(SkillName.Wrestling, 19.3, 34.0);

            Fame = 600;
            Karma = -600;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public LavaSnake(Serial serial)
            : base(serial)
        {
        }

        public override bool DeathAdderCharmable => true;

        public override int Meat => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            AddLoot(LootPack.LootItem<SulfurousAsh>());
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
