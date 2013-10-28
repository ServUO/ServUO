using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a ronin corpse")]
    public class Ronin : BaseCreature
    {
        [Constructable]
        public Ronin()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();
            this.Name = "a ronin";
            this.Body = ((this.Female = Utility.RandomBool()) ? this.Body = 0x191 : this.Body = 0x190);
			
            this.Hue = Utility.RandomSkinHue();

            this.SetStr(326, 375);
            this.SetDex(31, 45);
            this.SetInt(101, 110);

            this.SetHits(301, 400);
            this.SetMana(101, 110);

            this.SetDamage(17, 25);

            this.SetDamageType(ResistanceType.Physical, 90);
            this.SetDamageType(ResistanceType.Poison, 10);

            this.SetResistance(ResistanceType.Physical, 55, 75);
            this.SetResistance(ResistanceType.Fire, 40, 60);
            this.SetResistance(ResistanceType.Cold, 35, 55);
            this.SetResistance(ResistanceType.Poison, 50, 70);
            this.SetResistance(ResistanceType.Energy, 55, 75);

            this.SetSkill(SkillName.MagicResist, 42.6, 57.5);
            this.SetSkill(SkillName.Tactics, 115.1, 130.0);
            this.SetSkill(SkillName.Wrestling, 92.6, 107.5);
            this.SetSkill(SkillName.Anatomy, 110.1, 125.0);

            this.SetSkill(SkillName.Fencing, 92.6, 107.5);
            this.SetSkill(SkillName.Macing, 92.6, 107.5);
            this.SetSkill(SkillName.Swords, 92.6, 107.5);

            this.Fame = 8500;
            this.Karma = -8500;

            this.AddItem(new SamuraiTabi());
            this.AddItem(new LeatherHiroSode());
            this.AddItem(new LeatherDo());

            switch ( Utility.Random(4))
            {
                case 0:
                    this.AddItem(new LightPlateJingasa());
                    break;
                case 1:
                    this.AddItem(new ChainHatsuburi());
                    break;
                case 2:
                    this.AddItem(new DecorativePlateKabuto());
                    break;
                case 3:
                    this.AddItem(new LeatherJingasa());
                    break;
            }

            switch ( Utility.Random(3))
            {
                case 0:
                    this.AddItem(new StuddedHaidate());
                    break;
                case 1:
                    this.AddItem(new LeatherSuneate());
                    break;
                case 2:
                    this.AddItem(new PlateSuneate());
                    break;
            }
			
            if (Utility.RandomDouble() > .2)
                this.AddItem(new NoDachi());
            else
                this.AddItem(new Halberd());

            this.PackItem(new Wakizashi());
            this.PackItem(new Longsword());

            Utility.AssignRandomHair(this);
        }

        public Ronin(Serial serial)
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
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            c.DropItem(new BookOfBushido());
        }

        // TODO: Bushido abilities
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Gems, 2);
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