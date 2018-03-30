using System;
using Server.Items;

namespace Server.Mobiles 
{
    public class HireFighter : BaseHire 
    {
        [Constructable] 
        public HireFighter()
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool()) 
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else 
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            Title = "the fighter";
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            SetStr(91, 91);
            SetDex(91, 91);
            SetInt(50, 50);

            SetDamage(7, 14);

            SetSkill(SkillName.Tactics, 36, 67);
            SetSkill(SkillName.Magery, 22, 22);
            SetSkill(SkillName.Swords, 64, 100);
            SetSkill(SkillName.Parry, 60, 82);
            SetSkill(SkillName.Macing, 36, 67);
            SetSkill(SkillName.Focus, 36, 67);
            SetSkill(SkillName.Wrestling, 25, 47);

            Fame = 100;
            Karma = 100;

            switch ( Utility.Random(2)) 
            {
                case 0:
                    AddItem(new Shoes(Utility.RandomNeutralHue()));
                    break;
                case 1:
                    AddItem(new Boots(Utility.RandomNeutralHue()));
                    break;
            }
			
            AddItem(new Shirt());

            // Pick a random sword
            switch ( Utility.Random(5)) 
            {
                case 0:
                    AddItem(new Longsword());
                    break;
                case 1:
                    AddItem(new Broadsword());
                    break;
                case 2:
                    AddItem(new VikingSword());
                    break;
                case 3:
                    AddItem(new BattleAxe());
                    break;
                case 4:
                    AddItem(new TwoHandedAxe());
                    break;
            }

            // Pick a random shield
            if (FindItemOnLayer(Layer.TwoHanded) == null)
            {
                switch (Utility.Random(8))
                {
                    case 0:
                        AddItem(new BronzeShield());
                        break;
                    case 1:
                        AddItem(new HeaterShield());
                        break;
                    case 2:
                        AddItem(new MetalKiteShield());
                        break;
                    case 3:
                        AddItem(new MetalShield());
                        break;
                    case 4:
                        AddItem(new WoodenKiteShield());
                        break;
                    case 5:
                        AddItem(new WoodenShield());
                        break;
                    case 6:
                        AddItem(new OrderShield());
                        break;
                    case 7:
                        AddItem(new ChaosShield());
                        break;
                }
            }
		  
            switch( Utility.Random(5) )
            {
                case 0:
                    break;
                case 1:
                    AddItem(new Bascinet());
                    break;
                case 2:
                    AddItem(new CloseHelm());
                    break;
                case 3:
                    AddItem(new NorseHelm());
                    break;
                case 4:
                    AddItem(new Helmet());
                    break;
            }
            // Pick some armour
            switch( Utility.Random(4) )
            {
                case 0: // Leather
                    AddItem(new LeatherChest());
                    AddItem(new LeatherArms());
                    AddItem(new LeatherGloves());
                    AddItem(new LeatherGorget());
                    AddItem(new LeatherLegs());
                    break;
                case 1: // Studded Leather
                    AddItem(new StuddedChest());
                    AddItem(new StuddedArms());
                    AddItem(new StuddedGloves());
                    AddItem(new StuddedGorget());
                    AddItem(new StuddedLegs());
                    break;
                case 2: // Ringmail
                    AddItem(new RingmailChest());
                    AddItem(new RingmailArms());
                    AddItem(new RingmailGloves());
                    AddItem(new RingmailLegs());
                    break;
                case 3: // Chain
                    AddItem(new ChainChest());
                    //AddItem(new ChainCoif());
                    AddItem(new ChainLegs());
                    break;
            }

            PackGold(25, 100);
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