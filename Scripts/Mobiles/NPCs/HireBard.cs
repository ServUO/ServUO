using Server.Items;

namespace Server.Mobiles
{
    public class HireBard : BaseHire
    {
        [Constructable]
        public HireBard()
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                switch (Utility.Random(2))
                {
                    case 0:
                        AddItem(new Skirt(Utility.RandomDyedHue()));
                        break;
                    case 1:
                        AddItem(new Kilt(Utility.RandomNeutralHue()));
                        break;
                }
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            Title = "the bard";
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            SetStr(16, 16);
            SetDex(26, 26);
            SetInt(26, 26);

            SetDamage(5, 10);

            SetSkill(SkillName.Tactics, 35, 57);
            SetSkill(SkillName.Magery, 22, 22);
            SetSkill(SkillName.Swords, 45, 67);
            SetSkill(SkillName.Archery, 36, 67);
            SetSkill(SkillName.Parry, 45, 60);
            SetSkill(SkillName.Musicianship, 66.0, 97.5);
            SetSkill(SkillName.Peacemaking, 65.0, 87.5);

            Fame = 100;
            Karma = 100;

            AddItem(new Shoes(Utility.RandomNeutralHue()));

            switch (Utility.Random(2))
            {
                case 0:
                    AddItem(new Doublet(Utility.RandomDyedHue()));
                    break;
                case 1:
                    AddItem(new Shirt(Utility.RandomDyedHue()));
                    break;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.RandomLootItem(new System.Type[] { typeof(Harp), typeof(Lute), typeof(Drums), typeof(Tambourine) }));
            AddLoot(LootPack.LootItem<Longsword>(true));
            AddLoot(LootPack.LootItem<Bow>(true));
            AddLoot(LootPack.LootItem<Arrow>(100, true));
            AddLoot(LootPack.LootGold(10, 50));
        }

        public HireBard(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle => false;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);// version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
