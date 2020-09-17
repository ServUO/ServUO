using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order assassin corpse")]
    public class SerpentsFangAssassin : BaseCreature
    {
        [Constructable]
        public SerpentsFangAssassin()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Black Order Assassin";
            Title = "of the Serpent's Fang Sect";
            Female = Utility.RandomBool();
            Race = Race.Human;
            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            AddItem(new ThighBoots(0x51D));
            AddItem(new FancyShirt(0x51D));
            AddItem(new StuddedMempo());
            AddItem(new JinBaori(0x2A));

            Item item;

            item = new StuddedGloves
            {
                Hue = 0x2A
            };
            AddItem(item);

            item = new LeatherNinjaPants
            {
                Hue = 0x51D
            };
            AddItem(item);

            item = new LightPlateJingasa
            {
                Hue = 0x51D
            };
            AddItem(item);

            item = new Sai
            {
                Hue = 0x51D
            };
            AddItem(item);

            SetStr(440, 460);
            SetDex(160, 175);
            SetInt(160, 175);

            SetHits(440, 500);

            SetDamage(13, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 55, 60);
            SetResistance(ResistanceType.Poison, 30, 50);
            SetResistance(ResistanceType.Energy, 30, 50);

            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 115.0, 130.0);
            SetSkill(SkillName.Wrestling, 95.0, 120.0);
            SetSkill(SkillName.Anatomy, 105.0, 120.0);
            SetSkill(SkillName.Fencing, 78.0, 100.0);
            SetSkill(SkillName.Swords, 90.1, 105.0);
            SetSkill(SkillName.Ninjitsu, 90.0, 120.0);
            SetSkill(SkillName.Hiding, 100.0, 120.0);
            SetSkill(SkillName.Stealth, 100.0, 120.0);

            Fame = 13000;
            Karma = -13000;
        }

        public SerpentsFangAssassin(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer => true;
        public override bool ShowFameTitle => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 4);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new SerpentFangSectBadge());
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
