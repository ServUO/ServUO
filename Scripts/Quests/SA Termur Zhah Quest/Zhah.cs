using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Quests
{				
	public class AthenaeumIsleQuest : BaseQuest
	{
        /* Journey to the Athenaeum Isle */
        public override object Title { get { return 1150929; } }

        //Greetings, adventurer. <br><br>	As you know, my people have suffered the incessant onslaught of the Void and its minions for as long as Gargish
        //history exists. Protecting Ter Mur from the darkness, and its desire to consume the land completely, is a burden passed down from one ruler to 
        //another upon ascension to the throne.  During my rule, I have been more successful than my predecessors but, now, I fear that the greatest evil both
        //myself and my people have ever faced is about to return.<br><br>	Long ago, Ter Mur was assaulted by the most formidable and horrid servant of the 
        //Void it had ever faced. Called Scelestus the Defiler, this daemon proved invincible to any weapon or spell that was utilized against him. I was unable
        //to defeat him and was forced to imprison him instead. Sadly, my own daughter was caught in the spell and stands imprisoned next to the daemon. It has
        //been this way for a thousand years now.<br><br>	I have received word that the isle which houses the daemon, Athenaeum Isle, is once again swarming
        //with daemons. Based on the description provided to me, I believe these are the minions of the Defiler himself. They have no doubt crawled out of the
        //dark in anticipation of their master’s return. In truth, the prison I placed him within will not last forever.<br><br>	I ask that you journey to 
        //the southwestern flight tower, adventurer, and head further southwest towards the shore. Near the water's edge, you will find an ancient teleport site
        //which will transport you to the isle. Once there, please slay as many of these monsters as you can. Additionally, please keep your eye out for any 
        //documents that you may discover. This isle was the former home of our Great Library and, when it fell, not all of the documents and books were able to 
        //be taken to the new location here in the Royal City.<br><br>	Slay the beasts and return to me any documents that you acquire.<br><br>	Be careful, 
        //and go with honor.
        public override object Description { get { return 1150902; } }

        //Understood. Perhaps you are not as brave as I initially thought. Be on your way, then.
        public override object Refuse { get { return 1150930; } }

        //You have returned. Did you manage to slay the beasts and obtain any documents that may be of interest?
        public override object Uncomplete { get { return 1150931; } }

        //You have returned! I cannot thank you enough for the service you have done me, adventurer. <br><br>	The documents that you have retrieved may seem 
        //unimportant to you, as they are naught but random letters and doctrines. But they each represent an echo of the past, musings of our ancestors. I had
        //always meant to return to the former library and retrieve all that I could, but I had thought they were safe, gathering dust in the ruins. I will 
        //immediately have these cleaned and placed in the Great Library here in the Royal City.<br><br>	As thanks, I offer you this book. It is the chronicle 
        //of my life, of the arrival of the Defiler, and a history of my people. In hopes that you will be granted further understanding of the impending danger 
        //we suffer, I offer it to you as a gesture of friendship and goodwill.<br><br>	Thank you again, on behalf of the Gargoyle people. I may have need of your 
        //assistance at another time, should you be willing to come to my aid again.<br><br>	Until then, farewell.
        public override object Complete { get { return 1150903; } }

        /*public override bool CanOffer()
        {
            return MondainsLegacy.TwistedWeald;
        }*/

        public AthenaeumIsleQuest()
            : base()
		{
            this.AddObjective(new SlayObjective(typeof(MinionOfScelestus), "Minion of Scelestus ", 10));

            this.AddObjective(new ObtainObjective(typeof(TheChallengeRite), "Obtain Gargish Document - Challenge Rite", 1, 0x0FF2));
            this.AddObjective(new ObtainObjective(typeof(AthenaeumDecree), "Obtain Gargish Document - Athenaeum Decree", 1, 0x14ED));
            this.AddObjective(new ObtainObjective(typeof(ALetterFromTheKing), "Obtain Gargish Document - Letter from the King", 1, 0x14ED));
            this.AddObjective(new ObtainObjective(typeof(OnTheVoid), "Obtain Gargish Document - On the Void", 1, 0x0FF2));
            this.AddObjective(new ObtainObjective(typeof(ShilaxrinarsMemorial), "Obtain Gargish Document - Shilaxrinar's Memorial", 1, 0x14ED));
            this.AddObjective(new ObtainObjective(typeof(ToTheHighScholar), "Obtain Gargish Document - To the High Scholar", 1, 0x14ED));
            this.AddObjective(new ObtainObjective(typeof(ToTheHighBroodmother), "Obtain Gargish Document - To the High Broodmother", 1, 0x14ED));
            this.AddObjective(new ObtainObjective(typeof(ReplyToTheHighScholar), "Obtain Gargish Document - Reply to the High Scholar", 1, 0x14ED));
            this.AddObjective(new ObtainObjective(typeof(AccessToTheIsle), "Obtain Gargish Document - Access to the Isle", 1, 0x14ED));
            this.AddObjective(new ObtainObjective(typeof(InMemory), "Obtain Gargish Document - In Memory", 1, 0x0FF2));

            this.AddReward(new BaseReward(typeof(ChronicleGargoyleQueenVolI), 1150914));
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

    public class Zhah : MondainQuester
	{
		public override Type[] Quests{ get{ return new Type[] 
		{ 
			//typeof(AthenaeumIsleQuest)
		}; } }
		
		[Constructable]
        public Zhah()
            : base("Zhah", "the Gargoyle Queen")
		{			
		}
		
		public Zhah( Serial serial ) : base( serial )
		{
		}
		
		public override void InitBody()
		{
            this.Race = Race.Gargoyle;
            this.InitStats(100, 100, 25);
            this.Female = true;
            this.Body = 667;
			
			this.Hue = 0x431;
			this.HairItemID = 0x42AB;
			this.HairHue = 0x21;
		}

        public Item ApplyHue(Item item, int hue)
        {
            item.Hue = hue;

            return item;
        }

		public override void InitOutfit()
		{
            this.AddItem(new Backpack());
            this.AddItem(this.ApplyHue(new PlateTalons(), 0x8AB));
            this.AddItem(this.ApplyHue(new FemaleGargishPlateKilt(), 0x8AB));
            this.AddItem(this.ApplyHue(new FemaleGargishPlateChest(), 0x8AB));
            this.AddItem(this.ApplyHue(new FemaleGargishPlateArms(), 0x8AB));
            this.AddItem(this.ApplyHue(new GargishPlateWingArmor(), 0x8AB));
            this.AddItem(this.ApplyHue(new GlassStaff(), 0x503));
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
}