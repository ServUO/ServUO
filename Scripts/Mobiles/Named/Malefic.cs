using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Malefic corpse")]
    public class Malefic : DreadSpider
    {
        [Constructable]
        public Malefic()
        {
            Name = "Malefic";
            Hue = 0x455;

            SetStr(210, 284);
            SetDex(153, 197);
            SetInt(349, 390);

            SetHits(600, 747);
            SetStam(153, 197);
            SetMana(349, 390);

            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Poison, 80);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 49);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 41, 48);

            SetSkill(SkillName.Wrestling, 96.9, 112.4);
            SetSkill(SkillName.Tactics, 91.3, 105.4);
            SetSkill(SkillName.MagicResist, 79.8, 95.1);
            SetSkill(SkillName.Magery, 103.0, 118.6);
            SetSkill(SkillName.EvalInt, 105.7, 119.6);
            SetSkill(SkillName.Meditation, 0);

            Fame = 21000;
            Karma = -21000;

            Tamable = false;

            SetWeaponAbility(WeaponAbility.Dismount);
        }

        public Malefic(Serial serial)
            : base(serial)
        {
        }
		
        public override bool CanBeParagon => false;
        public override bool GivesMLMinorArtifact => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.ArcanistScrolls);
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
