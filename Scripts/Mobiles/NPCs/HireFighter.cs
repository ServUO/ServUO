using System;
using Server.Items;

namespace Server.Mobiles 
{
    public class HireFighter : BaseHire 
    {
        [Constructable] 
        public HireFighter()
        {
            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool()) 
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else 
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }
            this.Title = "the fighter";
            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
            this.Race.RandomFacialHair(this);

            this.SetStr(91, 91);
            this.SetDex(91, 91);
            this.SetInt(50, 50);

            this.SetDamage(7, 14);

            this.SetSkill(SkillName.Tactics, 36, 67);
            this.SetSkill(SkillName.Magery, 22, 22);
            this.SetSkill(SkillName.Swords, 64, 100);
            this.SetSkill(SkillName.Parry, 60, 82);
            this.SetSkill(SkillName.Macing, 36, 67);
            this.SetSkill(SkillName.Focus, 36, 67);
            this.SetSkill(SkillName.Wrestling, 25, 47);

            this.Fame = 100;
            this.Karma = 100;

            switch ( Utility.Random(2)) 
            {
                case 0:
                    this.AddItem(new Shoes(Utility.RandomNeutralHue()));
                    break;
                case 1:
                    this.AddItem(new Boots(Utility.RandomNeutralHue()));
                    break;
            }
			
            this.AddItem(new Shirt());

            // Pick a random sword
            switch ( Utility.Random(5)) 
            {
                case 0:
                    this.AddItem(new Longsword());
                    break;
                case 1:
                    this.AddItem(new Broadsword());
                    break;
                case 2:
                    this.AddItem(new VikingSword());
                    break;
                case 3:
                    this.AddItem(new BattleAxe());
                    break;
                case 4:
                    this.AddItem(new TwoHandedAxe());
                    break;
            }

            // Pick a random shield
            switch ( Utility.Random(8)) 
            {
                case 0:
                    this.AddItem(new BronzeShield());
                    break;
                case 1:
                    this.AddItem(new HeaterShield());
                    break;
                case 2:
                    this.AddItem(new MetalKiteShield());
                    break;
                case 3:
                    this.AddItem(new MetalShield());
                    break;
                case 4:
                    this.AddItem(new WoodenKiteShield());
                    break;
                case 5:
                    this.AddItem(new WoodenShield());
                    break;
                case 6:
                    this.AddItem(new OrderShield());
                    break;
                case 7:
                    this.AddItem(new ChaosShield());
                    break;
            }
		  
            switch( Utility.Random(5) )
            {
                case 0:
                    break;
                case 1:
                    this.AddItem(new Bascinet());
                    break;
                case 2:
                    this.AddItem(new CloseHelm());
                    break;
                case 3:
                    this.AddItem(new NorseHelm());
                    break;
                case 4:
                    this.AddItem(new Helmet());
                    break;
            }
            // Pick some armour
            switch( Utility.Random(4) )
            {
                case 0: // Leather
                    this.AddItem(new LeatherChest());
                    this.AddItem(new LeatherArms());
                    this.AddItem(new LeatherGloves());
                    this.AddItem(new LeatherGorget());
                    this.AddItem(new LeatherLegs());
                    break;
                case 1: // Studded Leather
                    this.AddItem(new StuddedChest());
                    this.AddItem(new StuddedArms());
                    this.AddItem(new StuddedGloves());
                    this.AddItem(new StuddedGorget());
                    this.AddItem(new StuddedLegs());
                    break;
                case 2: // Ringmail
                    this.AddItem(new RingmailChest());
                    this.AddItem(new RingmailArms());
                    this.AddItem(new RingmailGloves());
                    this.AddItem(new RingmailLegs());
                    break;
                case 3: // Chain
                    this.AddItem(new ChainChest());
                    this.AddItem(new ChainCoif());
                    this.AddItem(new ChainLegs());
                    break;
            }

            this.PackGold(25, 100);
        }

        public HireFighter(Serial serial)
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
        public override void Serialize(GenericWriter writer) 
        {
            base.Serialize(writer);

            writer.Write((int)0);// version 
        }

        public override void Deserialize(GenericReader reader) 
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}