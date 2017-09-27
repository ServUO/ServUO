using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class BouraBouraQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }

        public override Type NextQuest
        {
            get { return typeof(RaptorliciousQuest); }
        }

        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromMinutes(30); }
        }

        /* Boura, Boura */
        public override object Title { get { return 1112784; } }

        /* Greetings.
		 * 
		 * Ever since one of those nasty boura crushed my leg, I haven't been up to my usual
		 * duties. My wife thinks that I'm getting to be too old to be out in the woods and
		 * down in the abyss as much as I was when I was younger. I'm a hunter, you see, and
		 * keeping dangerous creatures at bay is my responsibility. You look like you are strong
		 * enough to cull the boura herd. Perhaps you can start with some of the easier boura
		 * found across the river to the east. In exchange for your help, I can offer you some
		 * of my wife's stock of imbuing ingredients. */
        public override object Description { get { return 1112798; } }

        /* Are you sure you are not up to the hunt? Those boura may seem passive, but they have
		 * been known to maim unwary travelers who threaten them... or worse. */
        public override object Refuse { get { return 1112799; } }

        /* You need to cull more of the boura herd than that. They are found to the east, across
		 * the river. */
        public override object Uncomplete { get { return 1112800; } }

        /* I see that you have returned successful in your task. That is enough for today, but
		 * I will have more work for you in a bit if you wish to come back. */
        public override object Complete { get { return 1112801; } }

        public BouraBouraQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(RuddyBoura), "ruddy bouras", 20));
            AddObjective(new SlayObjective(typeof(LowlandBoura), "lowland bouras", 15));


            AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
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

    public class RaptorliciousQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }

        public override Type NextQuest
        {
            get { return typeof(TheSlithWarsQuest); }
        }

        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromMinutes(30); }
        }

        /* Raptorlicious */
        public override object Title { get { return 1112785; } }

        /* Greetings. Ever since one of those nasty boura crushed my leg, I haven't been
		 * up to my usual duties. My wife thinks that I'm getting to be too old to be out in the
		 * woods and down in the abyss as much as I was when I was younger. I'm a hunter, you see,
		 * and keeping dangerous creatures at bay is my responsibility. Do you think you are strong
		 * enough to take on the packs of wild raptors that roam to the south outside of the city?
		 * Those raptor packs are getting out of control and have been attacking travelers to the
		 * Holy City. In exchange for your help, I can offer you some of my wife's stock of imbuing
		 * ingredients. */
        public override object Description { get { return 1112803; } }

        /* Are you sure you can't help? The route to the Holy City will be swarming with hungry
		 * raptors soon. */
        public override object Refuse { get { return 1112804; } }

        /* You're going to have to do better than that. You can find the raptors to the south, on
		 * the island before you get to the Holy City. */
        public override object Uncomplete { get { return 1112805; } }

        /* I see that you have returned successful in your task. That is enough for today,
		 * here is your reward. I will have more work for you in a bit if you wish to come back. */
        public override object Complete { get { return 1112806; } }

        public RaptorliciousQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Raptor), "raptors", 20));

            AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
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

    public class TheSlithWarsQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }
        public override Type NextQuest
        {
            get { return typeof(BouraBouraAndMoreBouraQuest); }
        }

        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromMinutes(30); }
        }

        /* The Slith Wars */
        public override object Title { get { return 1112786; } }

        /* Greetings.
		 * 
		 * Ever since one of those nasty boura crushed my leg, I haven't been up to my usual
		 * duties. My wife thinks that I'm getting to be too old to be out in the woods and 
		 * down in the abyss as much as I was when I was younger. I'm a hunter, you see, and
		 * keeping dangerous creatures at bay is my responsibility. The slith aren't particularly
		 * aggressive, but if we let them get out of hand, they'll start pushing the other
		 * wildlife out of the area. In exchange for your help killing some of them, I can offer
		 * you some of my wife's stock of imbuing ingredients. */
        public override object Description { get { return 1112807; } }

        /* Hunting slith may not be a story you'll be happy to tell your grandchildren about,
		 * but it needs to be done nonetheless. */
        public override object Refuse { get { return 1112808; } }

        /* There are still too many slith wandering around out there. Return when you've killed
		 * enough to make a difference. */
        public override object Uncomplete { get { return 1112809; } }

        /* I see that you have returned successful in your task. That is enough for today,
		 * here is your reward. I will have more work for you in a bit if you wish to come back. */
        public override object Complete { get { return 1112810; } }

        public TheSlithWarsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Slith), "sliths", 20));

            AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
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

    public class BouraBouraAndMoreBouraQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }
        public override Type NextQuest
        {
            get { return typeof(RevengeOfTheSlithQuest); }
        }
        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromHours(2); }
        }

        /* Boura, Boura, and more Boura */
        public override object Title { get { return 1112787; } }

        /* Greetings, hunter.
		 * 
		 * You've proven yourself to be a capable hunter, able to hunt the lesser creatures
		 * without fail. My injury prevents me from coming with you, so I ask that you be
		 * careful with your next task. High plains boura are not to be taken lightly. They
		 * are territorial and will not hesitate to attack you on sight, so be prepared when
		 * you face them. You can find them roaming the high plains to the north of the Holy
		 * City. As usual, I will reward you with something from my wife's stock of imbuing
		 * ingredients. Do you think you are you ready? */
        public override object Description { get { return 1112823; } }

        /* The herd needs culling, are you sure you cannot do this? */
        public override object Refuse { get { return 1112824; } }

        /* You will need to kill more high plains boura than that. */
        public override object Uncomplete { get { return 1112826; } }

        /* I see that you are back, and apparently in one piece. This is good. Here is your reward,
		 * please return in a couple hours for another task. */
        public override object Complete { get { return 1112826; } }

        public BouraBouraAndMoreBouraQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(HighPlainsBoura), "high plains bouras", 20));

            AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
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

    public class RevengeOfTheSlithQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }
        public override Type NextQuest
        {
            get { return typeof(WeveGotAnAntProblemQuest); }
        }
        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromHours(2); }
        }

        /* Revenge of the Slith */
        public override object Title { get { return 1112788; } }

        /* Greetings, hunter.
		 * 
		 * You've proven yourself to be a capable hunter, able to hunt the lesser creatures
		 * without fail. My injury prevents me from coming with you, so I ask that you be
		 * careful with your next task. Toxic slith are quite dangerous foes, and will not
		 * hesitate to poison you if they are able. They can be found on the islands to the
		 * east and northeast. As usual, I will reward you with something from my wife's stock
		 * of imbuing ingredients. Do you think you are you ready? */
        public override object Description { get { return 1112827; } }

        /* It is not shameful, exactly, to admit that you cannot face such foes. If you
		 * change your mind, come back to me. */
        public override object Refuse { get { return 1112828; } }

        /* You can find the toxic slith on the islands to the east and northeast. Please be careful. */
        public override object Uncomplete { get { return 1112829; } }

        /* I see that you are back, and apparently in one piece. This is good. Here is your reward,
		 * please return in a couple hours for another task. */
        public override object Complete { get { return 1112830; } }

        public RevengeOfTheSlithQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(ToxicSlith), "toxic sliths", 20));

            AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
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

    public class WeveGotAnAntProblemQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }
        public override Type NextQuest
        {
            get { return typeof(AmbushingTheAmbushersQuest); }
        }
        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromHours(2); }
        }

        /* We've Got an Ant Problem */
        public override object Title { get { return 1112789; } }

        /* Greetings, hunter.
		 * 
		 * You have proven yourself to be a capable hunter, able to hunt the lesser creatures without fail.
		 * My injury prevents me from coming with you, so I ask that you be careful with your next task.
		 * There is a creature known as the fire ant, and it is a foe you will not want to take lightly.
		 * The insides of the insect react soon after contact with the air and burst into flame. As usual,
		 * I will reward you with something from my wife's stock of imbuing ingredients. Do you think you
		 * are you ready? */
        public override object Description { get { return 1112831; } }

        /* I understand that these fire ants make seem to be too much for you, but I'm confident that you 
		 * will return eventually to help. */
        public override object Refuse { get { return 1112832; } }

        /* You'll need to slay more fire ants than that. */
        public override object Uncomplete { get { return 1112833; } }

        /* I see that you are back, and apparently in one piece. This is good. Here is your reward,
		 * please return in a couple hours for another task. */
        public override object Complete { get { return 1112834; } }

        public WeveGotAnAntProblemQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(FireAnt), "fire ants", 20));

            AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
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

    public class AmbushingTheAmbushersQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }
        public override Type NextQuest
        {
            get { return typeof(ItMakesMeSickQuest); }
        }

        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromHours(2); }
        }

        /* Ambushing the Ambushers */
        public override object Title { get { return 1112790; } }

        /* Greetings, hunter.
		 * 
		 * You have proven yourself to be a capable hunter, able to hunt the lesser creatures without
		 * fail. My injury prevents me from coming with you, so I ask that you be careful with your
		 * next task. To hunt a kepetch ambusher requires great skill and patience, as they spend most
		 * of their time lurking in the shadows awaiting unsuspecting prey. When you have reached an
		 * area where they are known to be, walk carefully, and listen carefully, and you will be
		 * successful. As usual, I will reward you with something from my wife's stock of imbuing
		 * ingredients. Do you think you are you ready? */
        public override object Description { get { return 1112835; } }

        /* Hunting that which cannot be easily seen is difficult. Return to me when you are ready. */
        public override object Refuse { get { return 1112836; } }

        /* You need to slay more kepetch ambushers for your task to be complete. */
        public override object Uncomplete { get { return 1112837; } }

        /* I see that you are back, and apparently in one piece. This is good. Here is your reward,
		 * please return in a couple hours for another task. */
        public override object Complete { get { return 1112838; } }

        public AmbushingTheAmbushersQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(KepetchAmbusher), "kepetch ambushers", 20));

            AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
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

    public class ItMakesMeSickQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }

        public override Type NextQuest
        {
            get { return typeof(ItsAMadMadWorldQuest); }
        }

        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromHours(12); }
        }

        /* It Makes Me Sick */
        public override object Title { get { return 1112791; } }

        /* Greetings, my friend.
		 * 
		 * You have proven yourself to be a worthy hunter, ready to stalk the greatest of creatures and
		 * succeed. My injury is not healing this time, and I fear that my wife is right and my time on
		 * the hunt is near the end. Yet, I still have knowledge of what needs to be done, so I am
		 * pleased to continue to guide you if you so choose. Putrid undead gargoyles are not to be
		 * underestimated. Very capable at melee, they also posses nearly overwhelming magical ability.
		 * If that were not enough, any that approach them are overwhelmed with disgust and will violently
		 * retch many times.
		 * 
		 * I say this not to dissuade you from this task, but so that you can go prepared. Of course, I
		 * will reward you with ingredients from my wife's stock, but for this task they will be of the
		 * most valuable kind. Are you willing to slay these foul creatures? */
        public override object Description { get { return 1112839; } }

        /* I am sure that you have your reasons for delaying. Please return to me when you are ready. */
        public override object Refuse { get { return 1112840; } }

        /* I know it is difficult, but you must persevere in the face of this great threat. Can you
		 * imagine if these putrid undead gargoyles grew in number so as to overwhelm the guards and
		 * pour forth into Ter Mur? */
        public override object Uncomplete { get { return 1112841; } }

        /* You have returned, my friend, and from the look of you, I see that you have been successful
		 * in the hunt. I am pleased to offer you this reward. Please return tomorrow, after you've taken
		 * some time to rest and recuperate from this hunt. I will no doubt have something more for you to
		 * do then. */
        public override object Complete { get { return 1112842; } }

        public ItMakesMeSickQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(PutridUndeadGargoyle), "putrid undead gargoyles", 20));

            AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
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

    public class ItsAMadMadWorldQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }

        public override Type NextQuest
        {
            get { return typeof(TheDreamersQuest); }
        }

        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromHours(12); }
        }

        /* It's a Mad, Mad World */
        public override object Title { get { return 1112792; } }

        /* Greetings, my friend.
		 * 
		 * You have proven yourself to be a worthy hunter, ready to stalk the greatest of creatures and
		 * succeed. My injury is not healing this time, and I fear that my wife is right and my time on
		 * the hunt is near the end. Yet, I still have knowledge of what needs to be done, so I am
		 * pleased to continue to guide you if you so choose. The maddening horror is aptly named, and
		 * it is not to be underestimated. Magical in nature, it also has the ability to drain you of
		 * your mana. Of course, I will reward you with ingredients from my wife's stock, but for this
		 * task they will be of the most valuable kind. Are you willing to slay these foul creatures? */
        public override object Description { get { return 1112843; } }

        /* These maddening horrors need to be kept contained, so please return to me when you are ready. */
        public override object Refuse { get { return 1112844; } }

        /* You need to slay more maddening horrors. Please come back to me when you are finished. */
        public override object Uncomplete { get { return 1112845; } }

        /* You have returned, my friend, and from the look of you, I see that you have been successful
		 * in the hunt. I am pleased to offer you this reward. Please return tomorrow, after you've taken
		 * some time to rest and recuperate from this hunt. I will no doubt have something more for you
		 * to do then. */
        public override object Complete { get { return 1112846; } }

        public ItsAMadMadWorldQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(MaddeningHorror), "maddening horrors", 20));

            AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
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

    public class TheDreamersQuest : BaseQuest
    {
        public override QuestChain ChainID
        {
            get { return QuestChain.PercolemTheHunter; }
        }
        public override TimeSpan RestartDelay
        {
            get { return TimeSpan.FromHours(12); }
        }

        /* The Dreamers */
        public override object Title { get { return 1112793; } }

        /* Greetings, my friend.
		 * 
		 * You have proven yourself to be a worthy hunter, ready to stalk the greatest of creatures and
		 * succeed. My injury is not healing this time, and I fear that my wife is right and my time on
		 * the hunt is near the end. Yet, I still have knowledge of what needs to be done, so I am pleased
		 * to continue to guide you if you so choose. The dream wraith is a particularly difficult foe,
		 * as not only does it posses knowledge of magic and of death, it can invade your mind and turn
		 * the world into a living nightmare, freezing you with fear. It is also difficult to find, as
		 * it exists partially in the world of dreams, and thus is not always easy to see. Of course, I
		 * will reward you with ingredients from my wife's stock, but for this task they will be of the
		 * most valuable kind. Are you willing to slay these foul creatures? */
        public override object Description { get { return 1112847; } }

        /* The nightmarish creatures must be kept at bay, or we will find ourselves living our worst
		 * nightmares. Please return when you are able. */
        public override object Refuse { get { return 1112848; } }

        /* More of these dreadful beings need to be slain for your task to be complete. */
        public override object Uncomplete { get { return 1112849; } }

        /* You have returned, my friend, and from the look of you, I see that you have been successful
		 * in the hunt. I am pleased to offer you this reward. Please return tomorrow, after you've taken
		 * some time to rest and recuperate from this hunt. I will no doubt have something more for you
		 * to do then. */
        public override object Complete { get { return 1112850; } }

        public TheDreamersQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(DreamWraith), "dream wraiths", 20));

            AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
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

    public class Percolem : MondainQuester
    {
        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(BouraBouraQuest) };
            }
        }

        [Constructable]
        public Percolem()
            : base("Percolem", "the Hunter")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Gargoyle;

            Hue = 0x86EA;
            HairItemID = 0x4261;
            HairHue = 0x31E;
        }

        public override void InitOutfit()
        {
            AddItem(new GlassStaff());
            AddItem(new GargishClothKilt(0x6CE));
            AddItem(new GargishClothChest(0x664));
            AddItem(new GargishClothArms(0x4DF));
        }

        public Percolem(Serial serial)
            : base(serial)
        {
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