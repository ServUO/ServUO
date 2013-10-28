using System;
using Server.Items;

namespace Server.Mobiles
{
    public class GrimmochDrummel : BaseCreature
    {
        [Constructable]
        public GrimmochDrummel()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Title = "the Cursed";

            this.Hue = 0x8596;
            this.Body = 0x190;
            this.Name = "Grimmoch Drummel";

            this.HairItemID = 0x204A;	//Krisna

            Bow bow = new Bow();
            bow.Movable = false;
            this.AddItem(bow);

            this.AddItem(new Boots(0x8A4));
            this.AddItem(new BodySash(0x8A4));

            Backpack backpack = new Backpack();
            backpack.Movable = false;
            this.AddItem(backpack);

            LeatherGloves gloves = new LeatherGloves();
            LeatherChest chest = new LeatherChest();
            gloves.Hue = 0x96F;
            chest.Hue = 0x96F;

            this.AddItem(gloves);
            this.AddItem(chest);

            this.SetStr(111, 120);
            this.SetDex(151, 160);
            this.SetInt(41, 50);

            this.SetHits(180, 207);
            this.SetMana(0);

            this.SetDamage(13, 16);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 45, 55);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 20, 25);

            this.SetSkill(SkillName.Archery, 90.1, 110.0);
            this.SetSkill(SkillName.Swords, 60.1, 70.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 60.1, 70.0);
            this.SetSkill(SkillName.Anatomy, 90.1, 100.0);

            this.Fame = 5000;
            this.Karma = -1000;

            this.PackItem(new Arrow(40));

            if (3 > Utility.Random(100))
                this.PackItem(new FireHorn());

            if (1 > Utility.Random(3))
                this.PackItem(Loot.RandomGrimmochJournal());
        }

        public GrimmochDrummel(Serial serial)
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
            return 0x178;
        }

        public override int GetAngerSound()
        {
            return 0x1AC;
        }

        public override int GetDeathSound()
        {
            return 0x27E;
        }

        public override int GetHurtSound()
        {
            return 0x177;
        }

        public override bool OnBeforeDeath()
        {
            Gold gold = new Gold(Utility.RandomMinMax(190, 230));
            gold.MoveToWorld(this.Location, this.Map);

            Container pack = this.Backpack;
            if (pack != null)
            {
                pack.Movable = true;
                pack.MoveToWorld(this.Location, this.Map);
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