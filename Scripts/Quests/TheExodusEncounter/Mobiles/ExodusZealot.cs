using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vile corpse")]
    public class ExodusZealot : BaseCreature
    {
		private static string[] m_Names = new string[]
		{
			"Drakan",
			"Vakel",
			"Zarus",
			"Zavik",
			"Dyntrall",
			"Kamal",
			"Hakash"
		};

		private static string[] m_Titles = new string[]
		{
			"The Exodus Zealot"
		};
        [Constructable]
        public ExodusZealot()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Body = 401;
            this.Female = false;
            this.Hue = Race.Human.RandomSkinHue();
            this.HairItemID = Race.Human.RandomHair(this);
            this.HairHue = Race.Human.RandomHairHue();

			this.Name = m_Names[Utility.Random( m_Names.Length )];
			this.Title = m_Titles[Utility.Random( m_Titles.Length )];

            this.SetStr(150, 210);
            this.SetDex(75, 90);
            this.SetInt(255, 310);

            this.SetHits(325, 390);

            this.SetDamage(6, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 35, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Wrestling, 70.0, 100.0);
            this.SetSkill(SkillName.Tactics, 80.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 50.0, 70.0);
            this.SetSkill(SkillName.Anatomy, 70.0, 100.0);
			this.SetSkill(SkillName.Magery, 85.0, 100.0);
			this.SetSkill(SkillName.EvalInt, 80.0, 100.0);
			this.SetSkill(SkillName.Poisoning, 70.0, 100.0);

            this.Fame = 10000;
            this.Karma = -10000;
			this.VirtualArmor = 30;

            Item boots = new ThighBoots();
            boots.Movable = false;
            this.AddItem(boots);

            Item shroud = new HoodedShroudOfShadows();
            shroud.Hue = 0xA91;
            shroud.Name = "Hooded Robe";
            shroud.LootType = LootType.Blessed;
            this.AddItem(shroud);
            
			AddItem(new Spellbook());

            if (Utility.RandomDouble() < 0.1)
            {
                switch (Utility.Random(4))
                {
                    case 0:
                        PackItem(new ExodusSummoningRite());
                        break;
                    case 1:
                        PackItem(new ExodusSacrificalDagger());
                        break;
                    case 2:
                        PackItem(new RobeofRite());
                        break;
                    case 3:
                        PackItem(new ExodusSummoningAlter());
                        break;
                }
            }
        }

        public ExodusZealot(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool PropertyTitle
        {
            get
            {
                return false;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

        public override void GenerateLoot(bool spawning)
        {
            if (spawning)
                return; // No loot/backpack on spawn

            base.GenerateLoot(true);
            base.GenerateLoot(false);
        }		
		
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
			this.AddLoot(LootPack.MedScrolls);
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}