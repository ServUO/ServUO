using Server.Items;

namespace Server.Mobiles
{
    public class LysanderGathenwale : BaseCreature
    {
        [Constructable]
        public LysanderGathenwale()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Title = "the Cursed";

            Hue = 0x8838;
            Body = 0x190;
            Name = "Lysander Gathenwale";

            AddItem(new Boots(0x599));
            AddItem(new Cloak(0x96F));

            Spellbook spellbook = new Spellbook();
            RingmailGloves gloves = new RingmailGloves();
            StuddedChest chest = new StuddedChest();
            PlateArms arms = new PlateArms();

            spellbook.Hue = 0x599;
            gloves.Hue = 0x599;
            chest.Hue = 0x96F;
            arms.Hue = 0x599;

            AddItem(spellbook);
            AddItem(gloves);
            AddItem(chest);
            AddItem(arms);

            SetStr(111, 120);
            SetDex(71, 80);
            SetInt(121, 130);

            SetHits(180, 207);
            SetMana(227, 265);

            SetDamage(5, 13);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.Wrestling, 80.1, 90.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 80.1, 90.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.EvalInt, 95.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);

            Fame = 5000;
            Karma = -10000;
        }

        public LysanderGathenwale(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle => false;
        public override bool ShowFameTitle => false;
        public override bool DeleteCorpseOnDeath => true;
        public override bool AlwaysMurderer => true;

        public override int GetIdleSound()
        {
            return 0x1CE;
        }

        public override int GetAngerSound()
        {
            return 0x1AC;
        }

        public override int GetDeathSound()
        {
            return 0x182;
        }

        public override int GetHurtSound()
        {
            return 0x28D;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.MageryRegs, 30);
        }

        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
                return false;

            if (Backpack != null)
                Backpack.Destroy();

            if (Utility.Random(3) == 0)
            {
                BaseBook notebook = Loot.RandomLysanderNotebook();
                notebook.MoveToWorld(Location, Map);
            }

            Effects.SendLocationEffect(Location, Map, 0x376A, 10, 1);
            return true;
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
