using System;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
	public class ShearingKnowledgeQuest : BaseQuest
	{
        public override QuestChain ChainID { get { return QuestChain.LaifemTheWeaver; } }///////////////////////////
        public override Type NextQuest { get { return typeof(WeavingFriendshipsQuest); } }
		public override TimeSpan RestartDelay { get { return TimeSpan.FromMinutes(30); } }
        public override bool DoneOnce { get { return true; } }
		
		/*Shearing Knowledge */
        public override object Title { get { return 1113245; } }
        /*Welcome to my little shop!<BR><BR>Don't you just love these beautiful carpet samples?
         * Look at these embroidery patterns! And the intricate knotwork! It was sure worth every
         * gold piece I paid to have these shipped from Vesper.<BR><BR>What's that? No, no, I'm 
         * sorry, these aren't for sale! I'm working towards recreating each of these gorgeous
         * styles myself, you see, and just wanted to show my future customers what they might
         * one day expect! By the skies though, how do I even begin learning these new patterns?
         * <BR><BR>I know! If you help me get started, you could be one of my first customers! 
         * Yes, that's it - I need to get into the mind of a Britannian crafter, so I need 
         * Britannian wool! Real, natural wool, mind you, none of the cheap sort you see on the 
         * vendors.<BR><BR>Maybe you could find some by shearing some of those... what do you 
         * call them? Sherp? Sheeple? */
        public override object Description { get { return 1113246; } }
        /*Oh no, really? I was hoping you could be one of my first patrons... */
        public override object Refuse { get { return 1113251; } }
        /*Creatures in Ter Mur simply won't do! And the vendors? Horrible quality! You'll have 
         *to visit Britannia and shear a few sheep by hand to obtain some authentic Britannian wool. */
        public override object Uncomplete { get { return 1113252; } }
        /*Wow! Isn't this amazing? It's so soft, so pure - surely this is the key to my efforts!<BR><BR>
         * <I>Laifem skillfully spins the wool into a beautiful ball of white yarn - before you know it, 
         * she's staring down at her first attempt to weave a Britannian carpet</I> */
        public override object Complete { get { return 1113253; } }
	
		public ShearingKnowledgeQuest() : base()

		{
            AddObjective(new ObtainObjective(typeof(BritannianWool), "Britannian Wool", 10, 0xDF8));

            AddReward(new BaseReward( 1113256 ));/*A step closer to having access to Laifem's inventory of decorative carpets. */
		}

       // public override void GiveRewards()
       // {
          // if ( Owner is PlayerMobile ) 
          // ((PlayerMobile)Owner).Exp += 50;
          //// Owner.SendMessage("You have also been awarded 5 Queens Loyalty Points!");
          // base.GiveRewards();
       // }
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}		
	}
	
	public class WeavingFriendshipsQuest : BaseQuest
	{
        public override QuestChain ChainID { get { return QuestChain.LaifemTheWeaver; } }///////////////////////////
        public override Type NextQuest { get { return typeof(NewSpinQuest); } }
		public override TimeSpan RestartDelay { get { return TimeSpan.FromMinutes(30); } }
        public override bool DoneOnce { get { return true; } }

		/* Weaving Friendships */
        public override object Title { get { return 1113254; } }
        /* <I>Laifem stares down at the ruins of her first carpet weaving attempt</I><BR><BR>Hrm... I guess I thought
         * this would be a bit easier.<BR><BR><I>She reaches up and twists on her ear a little, obviously deep in 
         * thought</I><BR><BR>You know... I think I need some professional assistance! There's a tailoring shop in
         * Vesper called, hrm, "The Spinning..." "The Spinning..." something or other. Sorry, I just don't have a head
         * for all those clever Britannian shop names.<BR><BR><I>*laughs*</I><BR><BR> I'm sure someone there could help,
         * do you think you could deliver a letter of introduction for me? */
        public override object Description { get { return 1113255; } }
        /* But I'm so close! If I can just talk to the right people we'll be in business for sure! */
        public override object Refuse { get { return 1113257; } }
        /* There should be a man, er, a human one at that, who owns a tailoring shop in Vesper. Maybe he can help me? */
        public override object Uncomplete { get { return 1113258; } }
        /* A letter? From a Gargoyle you say? */
        public override object Complete { get { return 1113259; } }
	
		public WeavingFriendshipsQuest() : base()
		{
            AddObjective(new DeliverObjective(typeof(LetterOfIntroduction), "Letter of Introduction", 1, typeof(Dermott), "Dermott (Vesper)"));

            AddReward(new BaseReward(1113256)); // A step closer to having access to Laifem's inventory of decorative carpets.
		}

       // public override void GiveRewards()
       // {
           // if (Owner is PlayerMobile)
           // ((PlayerMobile)Owner).Exp += 100;
           // Owner.SendMessage("You have also been awarded 10 Queens Loyalty Points!");
           // base.GiveRewards();
       // }
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}		
	}
	
	public class NewSpinQuest : BaseQuest
	{
        public override QuestChain ChainID { get { return QuestChain.LaifemTheWeaver; } }///////////////////////////
		public override TimeSpan RestartDelay { get { return TimeSpan.FromMinutes(30); } }
        public override bool DoneOnce { get { return true; } }

        /* A New Spin on Things */
        public override object Title { get { return 1113260; } }
        /* Oh my! Now isn't this something? A Gargoyle seeking to master the ways of our humble little industry.
         * Why, this is nothing short of inspirational!<BR><BR>I think I have just the thing for him. There's a
         * book over... oh! It's a her? My apologies, I just don't have a knack for those Gargish names you know!
         * <BR><BR>Regardless, please take this back to the young lady, if you would be so kind.<BR><BR>Best regards! */
        public override object Description { get { return 1113261; } }
        /* Oh dear, truly? I'm sure she'd be very pleased to have this, and I don't have the means to journey there
         * myself.<BR><BR>How very, very unfortunate... */
        public override object Refuse { get { return 1113262; } }
        /* Dermott wishes you to deliver the book "Mastering the Art of Weaving" to Laifem so she learn the ways of
         * Britannian weaving. */
        public override object Uncomplete { get { return 1113263; } }
        /* This is perfect! Thank you so, so much!<BR><BR><I>Laifem eagerly begins reading the book while pacing about
         * the room</I><BR><BR>Yes, yes I see. <I>*nods*</I> And the loops are done in a... with mohair knots... 
         * <I>*her fingers begin weaving idly in the air as she thinks*</I> and then to finish off the tassels I just...
         * <BR><BR>This is everything I need to begin weaving my very own decorative carpets. You see, I won't just make
         * the larger carpets, I'm going to make them in smaller pieces that can be put together to make any size or 
         * combination! Isn't that wonderful? */
        public override object Complete { get { return 1113264; } }

        public NewSpinQuest()
            : base()
		{
            AddObjective(new DeliverObjective(typeof(MasteringWeaving), "Mastering the Art of Weaving", 1, typeof(Laifem), "Laifem (Royal City)"));

            AddReward(new BaseReward(typeof(VariableCarpetAddonDeed), "Variable Carpet Deed"));
            //AddReward(new BaseReward( 1113250 )); //Access to Laifem's inventory of decorative carpets.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}		
	}

    public class Laifem : MondainQuester
	{

      /*  public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBTailor());
        }*/

		[Constructable]
		public Laifem() : base( "Laifem", "the Weaver" )
		{			
			SetSkill( SkillName.Tailoring, 80.0, 100.0 );
			SetSkill( SkillName.Focus, 60.0, 83.0 );
		}

		public Laifem( Serial serial ) : base( serial )
		{
		}

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
			{ 
				typeof( ShearingKnowledgeQuest ),
				//typeof( WeavingFriendshipsQuest ),
			};
            }
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            CantWalk = true;
            Body = 667;
            HairItemID = 16987;
            HairHue = 1801;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            AddItem(new GargishClothChest(Utility.RandomNeutralHue()));
            AddItem(new GargishClothKilt(Utility.RandomNeutralHue()));
            AddItem(new GargishClothLegs(Utility.RandomNeutralHue()));
        }
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

    public class Dermott : MondainQuester
    {
        public override Type[] Quests{ get{ return null; } }
       /* public override Type[] Quests
        {
            get
            {
                return new Type[] 
			{ 
				typeof( NewSpinQuest ),
			};
            }
        }*/

        [Constructable]
        public Dermott(): base("Dermott", "the Weaver")
        {
            SetSkill(SkillName.Tailoring, 80.0, 100.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Dermott(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;
			
            CantWalk = true;           
            HairItemID = 0x2049;
            HairHue = 0x45E;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new ThighBoots(0x901));
            AddItem(new ShortPants(0x730));
            AddItem(new Shirt(0x1BB));
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