using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a thrasher corpse")]
    public class Thrasher : BaseCreature
    {
        [Constructable]
        public Thrasher()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.2)
        {
            Name = "Thrasher";
            Body = 0xCE;
            Hue = 0x497;
            BaseSoundID = 0x294;

            SetStr(93, 327);
            SetDex(7, 201);
            SetInt(15, 67);

            SetHits(260, 984);

            SetDamage(15, 25);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 53, 55);
            SetResistance(ResistanceType.Fire, 25, 29);
            SetResistance(ResistanceType.Poison, 25, 28);

            SetSkill(SkillName.Wrestling, 101.2, 118.3);
            SetSkill(SkillName.Tactics, 99.1, 117.3);
            SetSkill(SkillName.MagicResist, 102.4, 118.6);

            Fame = 22400;
            Karma = -22400;

            SetWeaponAbility(WeaponAbility.ArmorIgnore);
        }

        public Thrasher(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact => true;
        public override int Hides => 48;
        public override int Meat => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 4);
            AddLoot(LootPack.ArcanistScrolls);
            AddLoot(LootPack.LootItem<ThrashersTail>());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
