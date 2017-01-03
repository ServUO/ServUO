using Server;
using System;
using Server.Mobiles;
using Server.Items;
using System.Linq;
using Server.Engines.ResortAndCasino;
using Server.Engines.MyrmidexInvasion;

namespace Server.Engines.Quests
{
	public class TheZealotryOfZipactriotlQuest : BaseQuest
	{
		public override QuestChain ChainID { get { return QuestChain.MyrmidexAlliance; } }
        public override Type NextQuest { get { return typeof(DestructionOfZipactriotlQuest); } }
		
		public override object Title{ get{ return 1156751; } } 		 // The Zealotry of Zipactriotl
		public override object Description{ get{ return 1156752; } }   /*All hail our Myrmidex Overlords! Now that the Britannians and the rest of Eodon 
																	   have been stopped from their offensive against the Myrmidex it is time for the 
																	   Barrab to restore the Myrmidex to power!  Long ago the Kotl came to Eodon.  
																	   Their technology was far advanced and they built many creations.  Before long 
																	   they decided they needed servents to tend to them beyond the metal giants they 
																	   called "Automatons" and so they created the Myrmidex.  The Myrmidex would not stand
																	   for their oppression and rose up against their reptilian masters!  The Kotl 
																	   unleashed a great weapon called Zipactriotl to abolish the Myrmidex.  In time 
																	   the Myrmidex were reduced in numbers and the Kotl placed Zipactriotl in stasis 
																	   until such time the Myrmidex were once again a threat. The alien technology of 
																	   the Kotl has spread throughout the realm in the eons since their arrival - we 
																	   must recover that technology so we may release Zipactriotl and destroy it once 
																	   and for all!   The time is now for Myrmidex rule with the Barrabian at their 
																	   side!  All hail the Myrmidex!  All hail the Barrab!*/
																	   
		public override object Refuse{ get{ return 1156753; } }		   /*I must question your commitment to the Myrmidex - are you satisfied with these 
																	   Britannian invaders and their Eodonian allies?*/
		
		public override object Uncomplete{ get{ return 1156754; } }	   /*You must acquire the Activator from the Gemologist at the Shimmering Jewel in 
																	   Vesper, the Regulator from the Ship Captain at the Horse's Head in Jhelom, the 
																	   Stator from the Golem Maker at the Tinker's Guild in Britain, and Power Core 
																	   from the Oddities Collector at the Lycaeum in Moonglow before the Stasis Chamber
																	   can be activated and Zipactriotl can be destroyed!*/
		
		public override object Complete{ get{ return 1156755; } }	   /**Eyes widen with excitement!*  All hail the Myrmidex! All hail the Barrab! 
																	   With these components I can bring the Stasis Chamber online and Zipactriotl can 
																	   be destroyed!*/
		
		public TheZealotryOfZipactriotlQuest()
		{
			AddObjective(new ObtainObjective(typeof(StasisChamberPowerCore), "Stasis Chamber Power Core", 1));
			AddObjective(new ObtainObjective(typeof(StasisChamberActivator), "Stasis Chamber Activator", 1));
			AddObjective(new ObtainObjective(typeof(StasisChamberRegulator), "Stasis Chamber Regulator", 1));
			AddObjective(new ObtainObjective(typeof(StasisChamberStator), "Stasis Chamber Stator", 1));
			
			AddReward(new BaseReward(1156756)); // A step closer to destroying Zipactriotl...
		}

        public override bool CanOffer()
        {
            if (!MyrmidexInvasionSystem.CanRecieveQuest(Owner, Allegiance.Myrmidex))
            {
                Owner.SendLocalizedMessage(1156778); // You must pledge allegiance to the Myrmidex and defeat the Eodonians in the Myrmidex Pits before you can embark on this quest.  You may pledge allegiance by double clicking the Idol in the Barrab Village.
                return false;
            }

            return true;
        }
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class DestructionOfZipactriotlQuest : BaseQuest
	{
		public override QuestChain ChainID { get { return QuestChain.MyrmidexAlliance; } }
		
		public override object Title{ get{ return 1156757; } } 			// The Destruction of Zipactriotl
		
		public override object Description{ get{ return 1156758; } }	/*All Hail the Myrmidex! All Hail the Barrab! Muwahahah! With these components 
																		the Stasis Chamber can be reactivated and Zipactriotl shall meet DOOM! Go now 
																		to the Kotl Antechamber and destroy Zipactriotl so the Myrmidex may once again 
																		rule the Valley of Eodon with the Barrab shall be at their side!*/
		
		public override object Refuse{ get{ return 1156760; } }			/*Only those warriors brave enough to join in the destruction of Zipactriotl 
																		will reap in the glory!  All hail the Myrmidex! All hail the Barrab!*/
		
		public override object Uncomplete{ get{ return 1156761; } }		/*Zipactriotl must be destroyed...the time is NOW! Go to the Antechamber in the 
																		Great Pyramid and restore glory to the Myrmidex!*/
		
		public override object Complete{ get{ return 1156762; } }		/*MUWAHAHA! ZIPACTRIOTL IS DESTROYED! THE MYRMIDEX ARE FREE TO RECLAIM EODON! 
																		THE BARRAB SHALL JOIN THEM AND TOGETHER WE SHALL RULE THE VALLEY!*/

        public override bool CanOffer()
        {
            return MyrmidexInvasionSystem.CanRecieveQuest(Owner, Allegiance.Myrmidex);
        }

		public DestructionOfZipactriotlQuest()
		{
			AddObjective(new SlayObjective(typeof(Zipactriotl), "Zipactriotl", 1));
			
			AddReward(new BaseReward(1156756)); // A step closer to destroying Zipactriotl...
		}
		
		public override void GiveRewards()
		{
			base.GiveRewards();
			Owner.AddToBackpack(new MyrmidexRewardBag());
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class HiddenTreasuresQuest : BaseQuest
	{
		public override object Title{ get{ return 1156800; } }		 // Hidden Treasures
		
		public override object Description{ get{ return 1156801; } } /*Hello there, looking to buy? Sell? Perhaps a pearl necklace for your lover - 
																	 or maybe a fine ring to accompany your finest evening wear?  We've got it all - just let 
																	 me know what we can do for you and I'm sure we can accomodate!  Activator? Stasis Chamber! 
																	 Why I haven't an idea what you are talking about! *leans in and whispers*  Not so 
																	 loud - can't be too sure who's listening in these days.  Listen if you want to get 
																	 your hands on that item I know where you might be able to find one.  I was able to
																	 acquire a treasure map that'll get you where you need to go but I need something 
																	 first.  My supply of prized gemstones has run low - you bring me those gem stones
																	 and I'll supply you with the map to what you're seeking.*/
		
		public override object Refuse{ get{ return 1156804; } }      // Well if you don't want to do it, you shouldn't have asked. Hrmph.
		
		public override object Uncomplete{ get{ return 1156803; } }  /*Acquire the gemstones I've asked for and I'll give you a map that will take you to 
																	 the item you seek.  It's very simple - now move along I've got customers to tend to!*/
		
		public override object Complete{ get{ return 1156805; } }    // *Examines the stones closely* Ahh yes, these will do quite nicely!  As promised here's your map - good luck!
		
		public HiddenTreasuresQuest()
		{
            AddObjective(new ObtainObjective(typeof(DarkSapphire), "Dark Sapphire", 1));
            AddObjective(new ObtainObjective(typeof(Turquoise), "Turquoise", 1));
			AddObjective(new ObtainObjective(typeof(PerfectEmerald), "Perfect Emerald", 1));
            AddObjective(new ObtainObjective(typeof(EcruCitrine), "Ecru Citrine", 1));
			AddObjective(new ObtainObjective(typeof(WhitePearl), "White Pearl", 1));
            AddObjective(new ObtainObjective(typeof(FireRuby), "Fire Ruby", 1));
			AddObjective(new ObtainObjective(typeof(BrilliantAmber), "Brilliant Amber", 1));
			
			AddReward(new BaseReward(typeof(HiddenTreasuresTreasureMap), "A Special Treasure Map"));
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class TheSaltySeaQuest : BaseQuest
	{
		public override object Title{ get{ return 1156807; } }		 //The Salty Sea
		
		public override object Description{ get{ return 1156808; } } /*Arr!  The seas have become a wretched and dangerous place - a myriad o' creatures
																	 be tormentin' me ships.  Can't have it! Can't have it one bit - what's that? 
																	 Myrmidex. *He pauses and thinks for a moment as he sits back on the stool*  Maybe 
																	 I heard of em, maybe I aint...I'll tell ya what ye go and clear some o' the creature
																	 s that be plaguing the seas and maybe I tell ya what I know about them Myrmidex.*/
		
		public override object Refuse{ get{ return 1156812; } } 	 // What's the matter, aint got yer sea legs? Har Har Har!
		
		public override object Uncomplete{ get{ return 1156813; } }  /*Ye needin' to make the seas safe for Captains like me!  Make sure ye killin' the 
																	 critters in the South Britannian Sea.  The seas south of the Britannian continent 
																	 and between the far western Island of Jhelom and the Island of Valor.  Big stretch 
																	 o' ocean to be clobberin sea monsters!  Get yerself a special fishing net if ye can
																	 't find the beasts, that'd be sure to wrangle them in!*/
		
		public override object Complete{ get{ return 1156814; } }    // Shiver me timbers!  The sea be safe again!  As promised here's the Message in a Bottle!
		
		public TheSaltySeaQuest()
		{
			AddObjective(new SlayObjective(typeof(WaterElemental), "Water Elemental", 10, "South Britannian Sea"));
			AddObjective(new SlayObjective(typeof(SeaSerpent), "Sea Serpent", 10, "South Britannian Sea"));
			AddObjective(new SlayObjective(typeof(DeepSeaSerpent), "Deep Sea Serpent", 10, "South Britannian Sea"));
            AddObjective(new SlayObjective(typeof(Kraken), "Kraken", 5, "South Britannian Sea"));
			
			AddReward(new BaseReward(typeof(SaltySeaMIB), "A Special Message in a Bottle"));
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class ATinkersTaskQuest : BaseQuest
	{
		public override object Title{ get{ return 1156815; } } 		 // A Tinker's Task
		
		public override object Description{ get{ return 1156816; } } /*Ha ha...he he...hoo hoo...yes yes...its all coming together now if only...*the 
																	 tinker pauses and looks up*  Myrmidex did you say?  Oh yes, we are most certainly 
																	 on the same page here!  I assume my Barrabian colleague sent you?  Of course he 
																	 did!  I'm almost ready to begin tinkering on the Automaton - but I need some
																	 supplies first.  You collect those supplies and I'll give you the Stator you seek.  
																	 Long live the Myrmidex!  Long live the Barrab! Ha ha! He he! Hoo Hoo!*/
		
		public override object Refuse{ get{ return 1156817; } }		 // No time to waste on those uncommitted to the cause - be gone with you!
		
		public override object Uncomplete{ get{ return 1156818; } }  /*Acquire the items I require to begin work on my Automaton and I'll give you the Stator.
																	 Don't come back until you've got everything!*/
		
		public override object Complete{ get{ return 1156819; } }	 /*Ha ha! He he! Hoo hoo! These are just what I needed - the Automaton won't take long now, 
																	 soon I shall wield this Kotl Technology and our Insect overlords will rule with the Barrab
																	 at their side.  Only those loyal Britannians will be spared!  Here is the Stator as
																	 I promised...*/
		
		public ATinkersTaskQuest()
		{
			AddObjective(new ObtainObjective(typeof(IronIngot), "Iron Ingot", 500));
			AddObjective(new ObtainObjective(typeof(Gears), "Gears", 25));
			AddObjective(new ObtainObjective(typeof(PowerCrystal), "Power Crystal", 5));
			AddObjective(new ObtainObjective(typeof(ClockworkAssembly), "Clockwork assembly", 2));
			
			AddReward(new BaseReward(typeof(StasisChamberStator), "Stasis Chamber Stator"));
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	// The last piece is sold on a vendor
	
	public class ExterminatingTheInfestationQuest : BaseQuest
	{
		public override QuestChain ChainID { get { return QuestChain.EodonianAlliance; } }
        public override Type NextQuest { get { return typeof(InsecticideAndRegicideQuest); } }
		
		public override object Title{ get{ return 1156763; } } 		 // Exterminating the Infestation
		
		public override object Description{ get{ return 1156764; } } /*It is unfortunate it had to come to this.  My hope was the knowledge gained from 
																	 all the peoples and creatures of Eodon would better our understanding of life - 
																	 alas the Myrmidex are an abomination, creatures born into enslavement by an 
																	 oppressive alien race, the Kotl.  The Kotl came to Eodon to expand their empire 
																	 and in doing so upset the delicate ecosystem that is the natural wonder of this 
																	 place.  The Valley cannot survive if the Myrmidex are allowed to spread, and there
																	 is no guarantee Britannia will be safe - they must be destroyed from within.  
																	 We must quell their numbers significantly in order to force their Queen into 
																	 reproduction, we must also obtain a powerful pheromone to gain safe passage within
																	 the deepest reaches of their pits, and we must also find an insecticide powerful 
																	 enough to destroy them. Only then can we confront the Myrmidex Queen and end the 
																	 Myrmidex threat.*/
																	 
		public override object Refuse{ get{ return 1156767; } }		 /*I must question your commitment to our cause - are you satisfied with these Myrmidex 
																     invaders and their Barrabian allies?*/
																	 
		public override object Uncomplete{ get{ return 1156768; } }  /*You must obtain the Insecticide from the Gambler at the Modest Damsel in New 
																	 Magincia, the Pheremone from the Gardener in Delucia, and the Population Report 
																	 from Lt. Foxx before we can launch an offensive against the Myrmidex Queen!*/
																	 
		public override object Complete{ get{ return 1156769; } }	 /*I should be excited but I cannot help but grimace at the destruction of an entire
																	 population *she frowns a bit*  I must remain pragmatic though, this is for the 
																	 greater good of the Valley and Britannia.  The time to destroy the Myrmidex Queen 
																	 is now.*/
		
		public ExterminatingTheInfestationQuest()
		{
			AddObjective(new ObtainObjective(typeof(BottledMyrmidexPheromone), "Bottled Myrmidex Pheromone", 1));
			AddObjective(new ObtainObjective(typeof(BottleOfConcentratedInsecticide), "Bottle of Concentrated Insecticide", 1));
			AddObjective(new ObtainObjective(typeof(MyrmidexPopulationReport), "Myrmidex Population Report", 1));
			
			AddReward(new BaseReward(1156766)); // A step closer to destroying the Myrmidex Queen...
		}

        public override bool CanOffer()
        {
            if (!MyrmidexInvasionSystem.CanRecieveQuest(Owner, Allegiance.Tribes))
            {
                Owner.SendLocalizedMessage(1156779); // You must pledge allegiance to the Eodonians and defeat the Myrmidex in the Myrmidex Pits before you can embark on this quest.  You may pledge allegiance by double clicking the Chaos banner in Sir Goeffrey's Camp.
                return false;
            }

            return true;
        }
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class InsecticideAndRegicideQuest : BaseQuest
	{
		public override QuestChain ChainID { get { return QuestChain.EodonianAlliance; } }
		
		public override object Title{ get{ return 1156770; } }		  // Insecticide and Regicide
		
		public override object Description{ get{ return 1156771; } }  /*Everything is in place.  In the depths of the Myrmidex Pits are the Queen's 
																	  chambers, it is there that she spawns new Myrmidex to bolster the numbers within
																	  the colony.  No doubt she is powerful - the energy of the moonstones is sure to 
																	  have seen to that.  It pains me to say this but - take no mercy, she must be 
																	  destroyed for the safety of the peoples of Eodon.*/
																	  
		public override object Refuse{ get{ return 1156767; } } 	  /*I must question your commitment to our cause - are you satisfied with these Myrmidex
																	  invaders and their Barrabian allies?;*/
		
		public override object Uncomplete{ get{ return 1156773; } }   /*You must go into the depths of the Myrmidex Pits and find the Queen - she must 
																	  be destroyed!  The entrance to the deepest parts of the Myrmidex Pits are in the 
																	  sandy valley to the West.*/
		
		public override object Complete{ get{ return 1156774; } }     /*You are brave and courageous - the Valley of Eodon is now safe from the Myrmidex threat!;*/
		
		public InsecticideAndRegicideQuest()
		{
			AddObjective(new SlayObjective(typeof(MyrmidexQueen), "Myrmidex Queen", 1));
			
			AddReward(new BaseReward(1156766)); // A step closer to destroying the Myrmidex Queen...
		}

        public override bool CanOffer()
        {
            if (!MyrmidexInvasionSystem.CanRecieveQuest(Owner, Allegiance.Tribes))
            {
                Owner.SendLocalizedMessage(1156779); // You must pledge allegiance to the Eodonians and defeat the Myrmidex in the Myrmidex Pits before you can embark on this quest.  You may pledge allegiance by double clicking the Chaos banner in Sir Goeffrey's Camp.
                return false;
            }

            return true;
        }

		public override void GiveRewards()
		{
			base.GiveRewards();
			Owner.AddToBackpack(new EodonianRewardBag());
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class PestControlQuest : BaseQuest
	{
		public override object Title{ get{ return 1156785; } } 		 // Pest Control
		
		public override object Description{ get{ return 1156776; } } /*A significant portion of the Myrmidex population has been quelled in the surface
																	 reaches of their habitat.  Population reduction estimated to meet or exceed 
																	 parameters set forth for operational security.  Recommend proceed.*/
																	 
		public override object Refuse{ get{ return 1156787; } }		 // I knew it, you must be yellar!  Get out of my sight!
		
		public override object Uncomplete{ get{ return 1156788; } }  /*You've got to kill enough Myrmidex Larvae, Drones, and Warriors in the Valley in
																	 order to force the queen to show herself, they've got to be Myrmidex in the Valley
																	 - stay out of the Myrmidex Pits!*/
		
		public override object Complete{ get{ return 1156789; } }	 /*Well color me surprised, you did it!  Now that we've gotten rid of enough of these
																	 pests the Queen will have no choice but to reveal herself - here's the population 
																	 report you'll need for Professor Rafkin!*/
		
		public PestControlQuest()
		{
			AddObjective(new SlayObjective(typeof(MyrmidexLarvae), "Myrmidex Larvae", 20));
			AddObjective(new SlayObjective(typeof(MyrmidexDrone), "Myrmidex Drone", 15));
			AddObjective(new SlayObjective(typeof(MyrmidexWarrior), "Myrmidex Warrior", 10));
			
			AddReward(new BaseReward(typeof(MyrmidexPopulationReport), "Myrmidex Population Report"));
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class GettingEvenQuest : BaseQuest
	{
		public override object Title{ get{ return 1156790; } }       // Getting Even
		
		public override object Description{ get{ return 1156791; } } /* *Sobs* Whyyyyy couldn't I just stop, no just had to be one more hand - I knew if 
																     I could just get on a hot streak I'd make it all better...but I lost everything, 
																	 even my shirt! I loved that shirt, was so puffy and soft.  How can fortune have 
																	 cursed me so badly?  The only thing left to do is drink this poison here and - 
																	 *pauses and looks at you*  You want this poison?  *eyebrow raises a bit*  Well if 
																	 you want it so bad you've got to do me a solid, head to the Fortune's Fire Casino 
																	 and make them pay!  Bring down the house and I'll let you have the poison, at least
																	 show me SOMEONE can win in that wretched place!*/
		
		public override object Refuse{ get{ return 1156796; } }		 // Can't even be bothered to help a guy down on his luck? *sigh*
		
		public override object Uncomplete{ get{ return 1156797; } }  /*You need to go to the Fortune's Fire Casino on Fire Island and show those dealers 
																	 who's boss!  Win 3 games each of Chuckle's Luck, Hi-Mid-Lo, and Dice Rider and I'll give 
																	 you the poison!*/
		
		public override object Complete{ get{ return 1156798; } }	 /*Luck be a lady tonight! Winner-winner chicken dinner! I'm back in the game, don't 
																	 need this poison anymore - here you go!*/

		
		public GettingEvenQuest()
		{
            AddObjective(new InternalObjective(typeof(ChucklesLuck)));
            AddObjective(new InternalObjective(typeof(HiMiddleLow)));
            AddObjective(new InternalObjective(typeof(DiceRider)));

			AddReward(new BaseReward(typeof(BottleOfConcentratedInsecticide), "A Bottle Of Concentrated Insecticide"));
		}

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddHtmlObject(160, 70, 330, 16, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:

            g.AddHtmlLocalized(98, 160, 312, 16, 1156792, 0x15F90, false, false); //Win 3 Games of "Chuckle's Luck" at the Fortune's Fire Casino
            g.AddHtmlLocalized(98, 176, 312, 16, 1156793, 0x15F90, false, false); //Win 3 games of "Hi-Middle-Lo" at the Fortune's Fire Casino
            g.AddHtmlLocalized(98, 192, 312, 16, 1156794, 0x15F90, false, false); //Win 3 games of "Dice Rider" at the Fortune's Fire Casino
            return true;
        }

        public void Update(Type t)
        {
            foreach (InternalObjective obj in this.Objectives.OfType<InternalObjective>())
            {
                if (obj.Update(t))
                {
                    if(Completed)
                        OnCompleted();
                    else if(obj.Completed)
                        Owner.SendSound(UpdateSound);	
                }
            }
        }

        public class InternalObjective : BaseObjective
        {
            public Type GameType { get; set; }

            public InternalObjective(Type type)
                : base(3)
            {
                GameType = type;
            }

            public override bool Update(object o)
            {
                Type t = o as Type;

                if(t != null && t == GameType && Quest != null && Quest.Owner != null && Quest.Owner.Region.IsPartOf("FireIsleCasino"))
                {
                    if(t == typeof(ChucklesLuck))
                    {
                        CurProgress++;
                        Quest.Owner.SendSound(Quest.UpdateSound);

                        if (CurProgress <= MaxProgress)
                            Quest.Owner.SendLocalizedMessage(1156795, String.Format("{0}\t{1}\t{2}", CurProgress.ToString(), MaxProgress.ToString(), "Chuckles' Luck")); // [Quest Event: Getting Even] You have won ~1_count~ of ~2_req~ games of ~3_game~!
                        
                    }
                    else if (t == typeof(HiMiddleLow))
                    {
                        CurProgress++;
                        Quest.Owner.SendSound(Quest.UpdateSound);

                        if (CurProgress <= MaxProgress)
                            Quest.Owner.SendLocalizedMessage(1156795, String.Format("{0}\t{1}\t{2}", CurProgress.ToString(), MaxProgress.ToString(), "Hi-Middle-Low")); // [Quest Event: Getting Even] You have won ~1_count~ of ~2_req~ games of ~3_game~!

                    }
                    else if (t == typeof(DiceRider))
                    {
                        CurProgress++;
                        Quest.Owner.SendSound(Quest.UpdateSound);

                        if (CurProgress <= MaxProgress)
                            Quest.Owner.SendLocalizedMessage(1156795, String.Format("{0}\t{1}\t{2}", CurProgress.ToString(), MaxProgress.ToString(), "Dice Rider")); // [Quest Event: Getting Even] You have won ~1_count~ of ~2_req~ games of ~3_game~!

                    }

                    return true;
                }

                return false;
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int v = reader.ReadInt();
            }
        }
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class OfVorpalsAndLettacesTheGardnerQuest : BaseQuest
	{
		public override object Title{ get{ return 1156780; } }		 // Of Vorpals and Lettuces
		
		public override object Description{ get{ return 1156781; } } /*I cannot believe these wretched pests!  I can't even manage to plant a single head of lettuce
																	 without these rascals chomping them up!  *She pauses for a moment and looks at you
																	 quizzically*  Oh, hello there!  What is it I can do for you?  Roses?  Watermelons? 
																	 Lettuces?  We've got them all...well maybe not lettuces but...Myrmidex Pheremone? 
																	 Well that is certainly a special request - say, how about this?  You take care of 
																	 my rabbit problem and I'll give you the pheremone.  Sounds fair to me!*/
		
		public override object Refuse{ get{ return 1156782; } }		 // Well if you're afraid of a little bunny then you haven't much business tackling a Myrmidex.  Hrmph!
		
		public override object Uncomplete{ get{ return 1156783; } }	 /*There's no time to lose!  My lettuces won't grow until you deal with the dastardly 
																	 bunnies wreaking havoc on my gardens! Get a green thorn and plant it in the garden's dirt
																	 to drive them out!*/
		
		public override object Complete{ get{ return 1156784; } } 	 /*Well done! With these pests out of my gardens I can finally get on with growing a prized lettuce! 
																	 Here's your Pheromone as I promised!*/
		
		public OfVorpalsAndLettacesTheGardnerQuest()
		{
			AddObjective(new SlayObjective(typeof(VorpalBunny), "Vorpal Bunny", 1, "Delucia"));
			
			AddReward(new BaseReward(typeof(BottledMyrmidexPheromone), "Bottled Myrmidex Pheromone"));
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
}