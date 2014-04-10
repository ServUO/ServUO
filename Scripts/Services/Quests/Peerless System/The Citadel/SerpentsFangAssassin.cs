using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order mage corpse")]
    public class SerpentsFangAssassin : BaseCreature
    {
        [Constructable]
        public SerpentsFangAssassin()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Black Order Assassin";
            this.Title = "of the Serpent's Fang Sect";
            this.Female = Utility.RandomBool();
            this.Race = Race.Human;
            this.Hue = this.Race.RandomSkinHue();
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);

            this.AddItem(new ThighBoots(0x51D));
            this.AddItem(new FancyShirt(0x51D));
            this.AddItem(new StuddedMempo());
            this.AddItem(new JinBaori(0x2A));

            Item item;

            item = new StuddedGloves();
            item.Hue = 0x2A;
            this.AddItem(item);

            item = new LeatherNinjaPants();
            item.Hue = 0x51D;
            this.AddItem(item);

            item = new LightPlateJingasa();
            item.Hue = 0x51D;
            this.AddItem(item);

            item = new Sai();
            item.Hue = 0x51D;
            this.AddItem(item);

            this.SetStr(440, 460);
            this.SetDex(160, 175);
            this.SetInt(160, 175);

            this.SetHits(440, 460);

            this.SetDamage(13, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 55, 60);
            this.SetResistance(ResistanceType.Poison, 30, 50);
            this.SetResistance(ResistanceType.Energy, 30, 50);

            this.SetSkill(SkillName.MagicResist, 80.0, 100.0);
            this.SetSkill(SkillName.Tactics, 115.0, 130.0);
            this.SetSkill(SkillName.Wrestling, 95.0, 120.0);
            this.SetSkill(SkillName.Anatomy, 105.0, 120.0);
            this.SetSkill(SkillName.Fencing, 78.0, 100.0);
            this.SetSkill(SkillName.Swords, 90.1, 105.0);
            this.SetSkill(SkillName.Ninjitsu, 90.0, 120.0);
            this.SetSkill(SkillName.Hiding, 100.0, 120.0);
            this.SetSkill(SkillName.Stealth, 100.0, 120.0);

            this.Fame = 13000;
            this.Karma = -13000;

            this.VirtualArmor = 58;
        }

        public SerpentsFangAssassin(Serial serial)
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
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosFilthyRich, 4);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new SerpentFangSectBadge());
            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new SerpentFangKey());
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