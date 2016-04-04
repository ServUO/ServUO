using System;
using Server.Items;

namespace Server.Mobiles
{
    public class TavaraSewel : BaseCreature
    {
        [Constructable]
        public TavaraSewel()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Title = "the Cursed";

            this.Hue = 0x8838;
            this.Female = true;
            this.Body = 0x191;
            this.Name = "Tavara Sewel";

            this.AddItem(new Kilt(0x59C));
            this.AddItem(new Sandals(0x599));

            Kryss kryss = new Kryss();
            Buckler buckler = new Buckler();
            RingmailGloves gloves = new RingmailGloves();
            FemalePlateChest chest = new FemalePlateChest();

            kryss.Hue = 0x96F;
            kryss.Movable = false;
            buckler.Hue = 0x96F;
            buckler.Movable = false;
            gloves.Hue = 0x599;
            chest.Hue = 0x96F;

            this.AddItem(kryss);
            this.AddItem(buckler);
            this.AddItem(gloves);
            this.AddItem(chest);

            this.SetStr(111, 120);
            this.SetDex(111, 120);
            this.SetInt(111, 120);

            this.SetHits(180, 207);
            this.SetStam(126, 150);
            this.SetMana(0);

            this.SetDamage(13, 16);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.Fencing, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 80.1, 90.0);
            this.SetSkill(SkillName.Anatomy, 90.1, 100.0);

            this.Fame = 5000;
            this.Karma = -1000;
        }

        public TavaraSewel(Serial serial)
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
            return 0x27F;
        }

        public override int GetAngerSound()
        {
            return 0x258;
        }

        public override int GetDeathSound()
        {
            return 0x25B;
        }

        public override int GetHurtSound()
        {
            return 0x257;
        }

        public override bool OnBeforeDeath()
        {
            Gold gold = new Gold(Utility.RandomMinMax(190, 230));
            gold.MoveToWorld(this.Location, this.Map);

            if (Utility.Random(3) == 0)
            {
                BaseBook journal = Loot.RandomTavarasJournal();
                journal.MoveToWorld(this.Location, this.Map);
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