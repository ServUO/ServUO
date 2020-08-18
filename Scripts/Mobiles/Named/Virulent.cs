using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Virulent corpse")]
    public class Virulent : BaseCreature
    {
        [Constructable]
        public Virulent()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Virulent";
            Body = 11;
            Hue = 0x8FF;
            BaseSoundID = 1170;

            SetStr(207, 252);
            SetDex(156, 194);
            SetInt(346, 398);

            SetHits(616, 740);
            SetStam(156, 194);
            SetMana(346, 398);

            SetDamage(15, 25);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Poison, 80);

            SetResistance(ResistanceType.Physical, 60, 68);
            SetResistance(ResistanceType.Fire, 40, 49);
            SetResistance(ResistanceType.Cold, 41, 50);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 49);

            SetSkill(SkillName.Wrestling, 92.8, 111.7);
            SetSkill(SkillName.Tactics, 91.6, 107.4);
            SetSkill(SkillName.MagicResist, 78.1, 93.3);
            SetSkill(SkillName.Poisoning, 120.0);
            SetSkill(SkillName.Magery, 104.2, 119.8);
            SetSkill(SkillName.EvalInt, 102.8, 116.8);

            Fame = 21000;
            Karma = -21000;

            SetWeaponAbility(WeaponAbility.MortalStrike);
        }

        public Virulent(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.ArcanistScrolls, 0, 1);
            AddLoot(LootPack.LootItem<SpidersSilk>(8, true));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.025)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new HunterLegs());
                        break;
                    case 1:
                        c.DropItem(new MalekisHonor());
                        break;
                }
            }

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new ParrotItem());
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
