using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lava serpent corpse")]
    [TypeAlias("Server.Mobiles.Lavaserpant")]
    public class LavaSerpent : BaseCreature
    {
        [Constructable]
        public LavaSerpent()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lava serpent";
            Body = 90;
            BaseSoundID = 219;

            SetStr(386, 415);
            SetDex(56, 80);
            SetInt(66, 85);

            SetHits(232, 249);
            SetMana(0);

            SetDamage(10, 22);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 80);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 25.3, 70.0);
            SetSkill(SkillName.Tactics, 65.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 4500;
            Karma = -4500;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public LavaSerpent(Serial serial)
            : base(serial)
        {
        }

        public override bool DeathAdderCharmable => true;
        public override int Meat => 4;
        public override int Hides => 15;
        public override HideType HideType => HideType.Spined;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LootItem<SulfurousAsh>(100.0, 1, false, true));
            AddLoot(LootPack.LootItem<Bone>(100.0, 1, false, true));
            AddLoot(LootPack.BodyParts);
        }

        public void AuraEffect(Mobile m)
        {
            m.SendMessage("The radiating heat scorches your skin!");
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
