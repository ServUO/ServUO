using Server.Items;
using Server.Mobiles;
using Server.Regions;
using System;
using System.Linq;

namespace Server.Engines.Quests.RitualQuest
{
    public class ScalesOfADreamSerpentQuest : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.Ritual;
        public override Type NextQuest => typeof(TearsOfASoulbinderQuest);

        public override object Title => 1151122;  // Ritual: Scales of a Dream Serpent

        public override object Description => 1151123;
        /*Greetings, adventurer.  Our  queen has need of your services again, should you be willing to come to her aid.<br><br>	
		As you may know, her Majesty has spent the past thousand years diligently researching ways in which she could defeat the
		Defiler.  I have personally assisted her in this effort and we have discovered a ritual which will magnify the cleansing
		magic that she originally used against him, potentially breaking the barrier that protects the Defiler’s body from harm.
		<br><br>	The ritual is ancient, dangerous, and requires many obscure components that are not easy to obtain. Her 
		Majesty has been successful in acquiring all but five of them over the years but, now, time is running out and she has
		asked me to task you with their acquisition. I have been provided with a list of the five remaining components and would
		like you to obtain them.<br><br>	The first component that I require is a handful of scales from a Dream Serpent.<br><br>
		Dream Serpents are mischievous but benevolent creatures that live off of magical energy.  They are extremely rare and 
		hard to catch, preferring to live in the realm of dreams rather than the physical realm. However, there is one location
		within the kingdom that you may find one.<br><br>	Journey once again to the Southwestern Flight Tower.  On your way to
		the Athenaeum, you may have noticed a clearing surrounded by four hillocks. This location is an intersection of ley lines
		which run throughout Ter Mur.  Ley lines are energized, concentrated strands of magical energy and thus they are 
		irresistible to the creatures.  If my calculations are correct, you should find a serpent and be able to interact with the
		creature. Heed my warning, adventurer: you should not seek to harm the beast. Offensive tactics must be avoided at all 
		costs, as these magical beings help keep the very energy of Ter Mur harnessed and in balance. Speak with the creature and
		ask for its aid, but do not harm it. When you have acquired the scales, return to me and I shall reward you and guide you
		towards the second component.*/

        public override object Refuse => 1151124;
        /*You do not wish to assist us? Then we shall wait for someone who does not wish to sit idly by while our people suffer. 
		Be gone from my sight, coward.*/

        public override object Uncomplete => 1151125;  // Were you able to acquire the scales? Please do not dally, adventurer!

        public override object Complete => 1151126;
        /*You have the scales? Excellent! <br><br>	We are now one step closer to completing the list of components for the ritual,
		my friend. With your assistance, I believe we will be able to acquire all of them just in time.<br><br>	Now, on to the next
		component.*/

        public ScalesOfADreamSerpentQuest()
        {
            AddObjective(new ObtainObjective(typeof(DreamSerpentScale), "Dream Serpent Scales", 1, 0x1F13, 0, 1976)); // TODO: Get ID
            AddReward(new BaseReward(1151384)); // The gratitude of the Gargoyle Queen and the next quest in the chain.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }

    public class TearsOfASoulbinderQuest : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.Ritual;
        public override Type NextQuest => typeof(PristineCrystalLotusQuest);

        public override object Title => 1151131;  // Ritual: Tears of a Soulbinder

        public override object Description => 1151132;
        /*I fear I must now send you to convene with a creature that may not prove as willing to help as the ones you have encountered 
		previously, my friend. I require the crystallized tear of a Soulbinder, a malevolent creature that prefers to roam the desolate
		edges of Ter Mur where the Void has consumed much of the area.<br><br><br>	Spawned from the depths of the Abyss, the 
		Soulbinder is a vile and disgusting creature that prefers to consume its victims alive, devouring their souls during its
		digestion. It is the spirit of its prey that nourishes the beast, and this hunger is the only thing the beast knows. I 
		know not how you will cause one of these monsters to shed a tear, but I believe you are resourceful enough to find a way.
		<br><br><br>	Journey to the northeast, and follow the twisting strips of land surrounded by the Void; you should be able
		to locate a path to the lair of this foul thing. I wish I could provide you with guidance regarding approaching the beast,
		but I have been unable to discern anything beyond its location. Be on your guard, adventurer, and be prepared to do battle
		should the beast prove hostile.<br><br><br>*/

        public override object Refuse => 1151124;
        /*You do not wish to assist us? Then we shall wait for someone who does not wish to sit idly by while our people suffer. 
		Be gone from my sight, coward.*/

        public override object Uncomplete => 1151133;  // Were you able to obtain the Soulbinder's Tear?

        public override object Complete => 1151134;
        /*You continue to amaze me, my friend. I will admit I was concerned that the Soulbinder would prove to be too much of a 
		challenge and, yet, here you are. <br><br>Are you ready to go after the next component?*/

        public TearsOfASoulbinderQuest()
        {
            AddObjective(new ObtainObjective(typeof(SoulbinderTear), "Soulbinders Tears", 1, 0xE2A, 0, 2076));
            AddReward(new BaseReward(1151384)); // The gratitude of the Gargoyle Queen and the next quest in the chain.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }

    public class PristineCrystalLotusQuest : BaseQuest
    {
        public override QuestChain ChainID => QuestChain.Ritual;
        public override Type NextQuest => null;

        public override object Title => 1151136;  // Ritual: Pristine Crystal Lotus

        public override object Description => 1151135;
        /*Now, you must traverse the northern desert. On the far edge, you will find a teleporter similar to that which you took
		to the Athenaeum Isle.  This will take you to a barren, small, and twisted piece of land that was long ago drained by the
		Void. <br><br>	The next component will be found there: a pristine crystal lotus. Long ago, what is now desert was a 
		beautiful field of these flowers. Sadly, the Defiler's magic drained the land of life and energy, causing the land to 
		become the wasteland it is today. However, one bloom remains, and it is that bloom you must acquire for me.<br><br>	In
		my research, I discovered that Queen Zhah’s predecessor, King Trajalem, had the only surviving bloom placed within a 
		protective barrier on the island I am directing you to. No one knows exactly why he did so, but I have journeyed there
		myself and seen it with my own eyes. Unfortunately, I was not able to discern how to break the barrier protecting the 
		lotus.<br><br>	The lotus stands on a pedestal surrounded by a configuration of magical tiles. I feel that these tiles
		are the key to breaking the barrier and obtaining the bloom.  However, their secret eluded me. What I was able to 
		discover through research is that you must speak the words ‘I seek the lotus’ to activate the tiles.  Additionally, if 
		you do manage to satisfy the secret of the tiles, you must state ‘Give me the lotus.’<br><br>	Journey to the area, my
		friend, and please obtain the lotus. If anyone can do it, I have faith that it is you.<br><br>	Be well, and I look
		forward to your return.<br><br>*/

        public override object Refuse => 1151124;
        /*You do not wish to assist us? Then we shall wait for someone who does not wish to sit idly by while our people suffer. 
		Be gone from my sight, coward.*/

        public override object Uncomplete => 1151137;  // Have you solved the secret of the lotus, my friend?

        public override object Complete => 1151138;
        /*Once again, you astound me with your perserverance and triumph! I cannot thank you enough. You are truly proving
		yourself a loyal friend.<br><br>Now, only two components remain.*/

        public int PuzzlesComplete { get; set; }
        public bool ReceivedLotus { get; set; }

        public PristineCrystalLotusQuest()
        {
            AddObjective(new ObtainObjective(typeof(PristineCrystalLotus), "Pristine Crystal Lotus", 1, 0x283B, 0, 1152));

            AddReward(new BaseReward(typeof(ChronicleOfTheGargoyleQueen2), 1151164)); // Chronicle of the Gargoyle Queen Vol. II
            AddReward(new BaseReward(typeof(TerMurSnowglobe), 1151172)); // Ter Mur Snowglobe
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(PuzzlesComplete);
            writer.Write(ReceivedLotus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            PuzzlesComplete = reader.ReadInt();
            ReceivedLotus = reader.ReadBool();
        }
    }

    public class CatchMeIfYouCanQuest : BaseQuest
    {
        public override object Title => 1151144;  // Catch Me If You Can!

        public override object Description => 1151145;
        /*Oh, have you come to see me?<br><br>	This is wonderful! I haven’t spoken to a mortal in so long.  It can get pretty 
		droll here in the realm of dreams, and I have no one to play with! There aren’t many of us left. <br><br>	What’s that?
		You want something? <br><br>	My scales! Ha ha! You mortals are so silly, always after something or other. <br><br>	
		Well, I’ll happily give you some scales if you agree to play a game with me! I love games, don’t you? <br><br>	The game
		is simple:  if you can hit me 6 times, then you win!  Take this stone and step into the circle here. Then, use the stone
		and it will take you to my favorite place to play! <br><br>	So, shall we play? Excellent! Be sure to put away your pets
		before you teleport with me!<br>*/

        public override object Refuse => 1151146;  // You don’t want to play? Boo! Go away!

        public override object Uncomplete => 1151147;  // Ah ha! Looks like you are not very fast. Want to play again?

        public override object Complete => 1151148;
        /*Boo, I don’t like to lose! But wasn’t that fun? <br><br>	Here are some of my scales; give them to the Gargoyle Queen with 
		my blessing and tell her she needs to come play a game with me sometime!*/

        public DreamSerpentCharm Charm { get; set; }

        public CatchMeIfYouCanQuest()
        {
            AddObjective(new InternalObjective());
            AddReward(new BaseReward(typeof(DreamSerpentScale), 1151167)); // Dream Serpents Scale
        }

        public override void OnAccept()
        {
            base.OnAccept();

            Charm = new DreamSerpentCharm();
            Owner.AddToBackpack(Charm);
        }

        public override void RemoveQuest(bool removeChain)
        {
            base.RemoveQuest(removeChain);

            if (Charm != null && !Charm.Deleted)
            {
                Charm.Delete();
                Charm = null;
            }
        }

        public class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription => 1151213;  // Hit the Dream Serpent 6 times before the time is up.

            public InternalObjective()
                : base(6)
            {
            }

            public override bool Update(object o)
            {
                CurProgress++;

                if (Quest.Completed)
                {
                    // No Gump, no message, nothing.
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
                reader.ReadInt(); // version
            }
        }

        public class BexilRegion : BaseRegion
        {
            public override bool AllowHousing(Mobile from, Point3D p)
            {
                return false;
            }

            public static void Initialize()
            {
                new BexilRegion();
            }

            public BexilRegion()
                : base("Bexil Region", Map.TerMur, DefaultPriority, new Rectangle2D(386, 3356, 35, 51))
            {
                Register();
                SetupRegion();
            }

            private void SetupRegion()
            {
                Map map = Map.TerMur;

                for (int x = 390; x < 408; x++)
                {
                    int z = map.GetAverageZ(x, 3360);

                    if (map.FindItem<Blocker>(new Point3D(x, 3360, z)) == null)
                    {
                        Blocker blocker = new Blocker();
                        blocker.MoveToWorld(new Point3D(x, 3360, z), map);
                    }
                }

                if (!GetEnumeratedMobiles().Any(m => m is BexilPunchingBag && !m.Deleted))
                {
                    BexilPunchingBag bex = new BexilPunchingBag();
                    bex.MoveToWorld(new Point3D(403, 3391, 38), Map.TerMur);
                }
            }

            public override bool CheckTravel(Mobile traveller, Point3D p, Spells.TravelCheckType type)
            {
                return false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Charm);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            Charm = reader.ReadItem() as DreamSerpentCharm;
        }
    }

    public class FilthyLifeStealersQuest : BaseQuest
    {
        public override object Title => 1151155;  // Filthy Lifestealers!

        public override object Description => 1151154;
        /*Ah, what do we have here? <br><br><br>	Bah! I was so hoping you would be something other than a human, gargoyle,
		or elf. Do you know how many of them I have eaten? Your souls have filled me but, now, they are no longer appealing.
		You are in luck that I have recently eaten, however, or else I would have devoured you anyway. <br><br><br>	You want 
		something, yes? I can smell the desire for something on your soul.  Desire used to be such a lovely dessert for me but
		now it is bland and droll. <br><br><br>	My tears, eh? Ha! Such an odd request. But you have my interest piqued. 
		<br><br><br>	I’ll tell you what;  do something for me and I will give you a bottle of tears.  <br><br><br>	In 
		the northwest, walking the lava shores, is a creature that has vexed me for centuries. An old foe, the Lifestealer 
		eats the hearts of its victims, consuming their essence in a similar fashion to how I consume their souls.   Many times
		have I lost a victim to these inane creatures, and many times have I attempted to exact my vengeance.  Unfortunately, 
		due to my corpulent nature, I am not as fast as they are and have failed in my desire to kill them.  Wouldn’t it be 
		poetic? A Soulbinder consuming a Lifestealer! <br><br><br>	Go to the lava shores in the northwest, mortal, and kill 
		the lifestealers you find there.  When you have thinned them out, return to me and I will give you what you seek.
		<br><br><br>*/

        public override object Refuse => 1151156;
        /*You dare refuse me? You are truly an imbecile. Ah well, I guess I shall eat you, after all! Just as soon as I finish 
		digesting my last meal, that is.*/

        public override object Uncomplete => 1151157;  // Have you killed the Lifestealers yet? Don’t try my patience, or I will eat you!

        public override object Complete => 1151158;
        /*I can smell their deaths upon you, mortal. Each Lifestealer you killed let loose all of the souls that they had taken, 
		and I was able to draw them here with my magic. I am so full, now! I think this will hold me over for quite some time. 
		<br><br><br>	Here, take what you came for. While you were gone, the thought of those Lifestealers dying was enough to
		make me laugh so hard I cried. I filled this bottle for you. <br><br>	Off with you, now! <br>*/

        public FilthyLifeStealersQuest()
        {
            AddObjective(new SlayObjective(typeof(Lifestealer), "Life Stealers", 10));

            AddReward(new BaseReward(typeof(SoulbinderTear), 1151170)); // Souldbinder's Tears
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }
}
