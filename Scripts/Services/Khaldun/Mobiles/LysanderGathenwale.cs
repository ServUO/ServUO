using System;
using Server.Items;

namespace Server.Mobiles
{
    public class LysanderGathenwale : BaseCreature
    {
        [Constructable]
        public LysanderGathenwale()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Title = "the Cursed";

            this.Hue = 0x8838;
            this.Body = 0x190;
            this.Name = "Lysander Gathenwale";

            this.AddItem(new Boots(0x599));
            this.AddItem(new Cloak(0x96F));

            Spellbook spellbook = new Spellbook();
            RingmailGloves gloves = new RingmailGloves();
            StuddedChest chest = new StuddedChest();
            PlateArms arms = new PlateArms();

            spellbook.Hue = 0x599;
            gloves.Hue = 0x599;
            chest.Hue = 0x96F;
            arms.Hue = 0x599;

            this.AddItem(spellbook);
            this.AddItem(gloves);
            this.AddItem(chest);
            this.AddItem(arms);

            this.SetStr(111, 120);
            this.SetDex(71, 80);
            this.SetInt(121, 130);

            this.SetHits(180, 207);
            this.SetMana(227, 265);

            this.SetDamage(5, 13);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.Wrestling, 80.1, 90.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 80.1, 90.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.EvalInt, 95.1, 100.0);
            this.SetSkill(SkillName.Meditation, 90.1, 100.0);

            this.Fame = 5000;
            this.Karma = -10000;

            Item reags = Loot.RandomReagent();
            reags.Amount = 30;
            this.PackItem(reags);
        }

        public LysanderGathenwale(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
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
        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return true;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
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
            this.AddLoot(LootPack.MedScrolls, 2);
        }

        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
                return false;

            if (this.Backpack != null)
                this.Backpack.Destroy();

            if (Utility.Random(3) == 0)
            {
                BaseBook notebook = Loot.RandomLysanderNotebook();
                notebook.MoveToWorld(this.Location, this.Map);
            }

            Effects.SendLocationEffect(this.Location, this.Map, 0x376A, 10, 1);
            return true;
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