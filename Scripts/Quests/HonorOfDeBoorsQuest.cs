using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class HonorOfDeBoorsQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.HonorOfDeBoors; } }
        public override Type NextQuest { get { return typeof(JackTheVillainQuest); } }
        public override bool DoneOnce { get { return true; } }

        /* The Honor of the De Boors */
        public override object Title { get { return 1075416; } }

        /*I beg your pardon, but will you listen to my story? My family, the de Boors family, have been jewel traders
         * as far back as anyone can remember. Alas, by the time I was born, we had fallen on hard times.
         * <br>To survive, I have had to sell much of my family’s property. Most of it was meaningless, but I regret
         * that a few years ago I made a terrible mistake. I pawned a shield bearing my family’s coat of arms to a
         * loan shark. That shield was borne into battle by Jaan de Boors, the founder of our house! It has no value
         * to anyone, but that blackguard won’t believe I have no money. He wants a fortune in jewels before he will 
         * return it.<br>Now I have learned that I am dying. Soon I will be gone, and my lineage with me. For the sake
         * of what little honor is left to me and my family name, I cannot bear to leave our ancestral shield in the
         * hands of that villain. Will you help me recover it?<br> */
        public override object Description { get { return 1075417; } }

        /* I know how much I am asking. Please, can you not help a dying man restore his family’s honor? */
        public override object Refuse { get { return 1075419; } }

        /* Gather them quickly. Who knows how long Derek has to live? */
        public override object Uncomplete { get { return 1075418; } }

        /* You have done it! Bless you! I do appreciate this very much! Though, will you do me one last favor? */
        public override object Complete { get { return 1075421; } }

        public HonorOfDeBoorsQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Diamond), "Diamond", 10, 0xF26));
            AddObjective(new ObtainObjective(typeof(Emerald), "Emerald", 10, 0xF10));
            AddObjective(new ObtainObjective(typeof(Ruby), "Ruby", 10, 0xF13));

            AddReward(new BaseReward(1075416)); // The Honor of the De Boors
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

    public class JackTheVillainQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.HonorOfDeBoors; } }
        public override Type NextQuest { get { return typeof(SavedHonorQuest); } }

        /* Jack the Villain */
        public override object Title { get { return 1075422; } }

        /*Will you take the jewels to the loan shark? I am not well enough to go myself, though it is not far. */
        public override object Description { get { return 1075423; } }

        /* Ah well. You have already helped me by gathering the jewels. I cannot complain. */
        public override object Refuse { get { return 1075425; } }

        /* The name of the villain is Jack, you will find him over by the port. */
        public override object Uncomplete { get { return 1075426; } }

        /* What do you want? Oh, that jewel merchant wants his shield back, eh? */
        public override object Complete { get { return 1075427; } }

        public JackTheVillainQuest()
            : base()
        {
            this.AddObjective(new DeliverObjective(typeof(BagOfJewels), "Bag of Jewels", 1, typeof(JackLoanShark), "Jack the Loan Shark"));

            AddReward(new BaseReward(1075416)); // The Honor of the De Boors
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

    public class SavedHonorQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.HonorOfDeBoors; } }

        /* Saved Honor */
        public override object Title { get { return 1075428; } }

        /* That idiot! This beat up piece of junk isn’t worth more than three gold coins, four at most!
         * Oh, well, a deal’s a deal! */
        public override object Description { get { return 1075429; } }

        /* I don’t care what you do! */
        public override object Refuse { get { return 1075431; } }

        /* Go away and never come back. */
        public override object Uncomplete { get { return 1075432; } }

        /* My shield! My family’s honor! You have my gratitude. Please, take this goblet.
         * It is small enough repayment for all you have done for me, but it is the only
         * thing of my family’s that I have left. */
        public override object Complete { get { return 1075433; } }

        public SavedHonorQuest()
            : base()
        {
            this.AddObjective(new DeliverObjective(typeof(DeBoorShield), "Ancestral Shield", 1, typeof(DerekMerchant), "Derek the Merchant"));

            AddReward(new BaseReward(typeof(GobletOfCelebration), 1075309)); // Goblet of Celebration
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
	
	public class DerekMerchant : MondainQuester
	{
        public override Type[] Quests
        {
            get
            {
                return new Type[] 
		{ 
			typeof( HonorOfDeBoorsQuest )
		};
            }
        }

        [Constructable]
        public DerekMerchant()
            : base("Derek", "the Merchant")
        {
            if (!(this is MondainQuester))

                this.Name = "Derek";
            this.Title = "the Merchant";
        }

        public DerekMerchant(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 33814;
            HairItemID = 0x203B;
            HairHue = 1141;
        }

        	public override void InitOutfit()
		{
			AddItem( new ShortPants(743) );
			AddItem( new Shirt(193) );
			AddItem( new Shoes(593) );
            AddItem(new Cap(308));

			PackGold( 100, 200 );
            Blessed = true;
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

    public class JackLoanShark : MondainQuester
	{
        public override Type[] Quests { get { return null; } }  //JackTheVillainQuest
    
        [Constructable]
        public JackLoanShark()
            : base("Jack", "the Loan Shark")
        {
            if (!(this is MondainQuester))

                this.Name = "Jack";
            this.Title = "the Loan Shark";
        }

        public JackLoanShark(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 33814;
            HairItemID = 0x203C;
            HairHue = 2413;
            FacialHairItemID = 0x2040;
            FacialHairHue = 2413;
        }

        	public override void InitOutfit()
		{
			AddItem( new LongPants(97) );
			AddItem( new FancyShirt() );
			AddItem( new Boots(742) );
			AddItem( new FeatheredHat(43) );
            AddItem( new Cloak(248) );

			PackGold( 100, 200 );
            Blessed = true;
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


