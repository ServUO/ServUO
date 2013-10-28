using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class GhostOfCovetousQuest : BaseQuest
    { 
        public GhostOfCovetousQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(SpiritBottle), "spirit bottle", 1, typeof(Frederic), "The Ghost of Frederic Smithson"));		
							
            this.AddReward(new BaseReward(1075284)); // Return the filled Spirit Bottle to Griswolt the Master Necromancer to receive a reward.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.CovetousGhost;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(SaveHisDadQuest);
            }
        }
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        /* A Ghost of Covetous */
        public override object Title
        {
            get
            {
                return 1075287;
            }
        }
        /* What? Oh, you startled me! Sorry, I'm a little jumpy. My master Griswolt learned that a ghost has recently taken up residence in the 
        Covetous dungeon. He sent me to capture it, but I . . . well, it terrified me, to be perfectly honest. If you think yourself courageous 
        enough, I'll give you my Spirit Bottle, and you can try to capture it yourself. I'm certain my master would reward you richly for such 
        service. */
        public override object Description
        {
            get
            {
                return 1075286;
            }
        }
        /* That's okay, I'm sure someone with more courage than either of us will come along eventually. */
        public override object Refuse
        {
            get
            {
                return 1075288;
            }
        }
        /* You'll find that ghost in the mountain pass above the Covetous dungeon.	 */
        public override object Uncomplete
        {
            get
            {
                return 1075290;
            }
        }
        /* (As you try to use the Spirit Bottle, the ghost snatches it out of your hand and smashes it on the rocks) Please, don't be frightened. 
        I need your help! */
        public override object Complete
        {
            get
            {
                return 1075291;
            }
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

    public class SaveHisDadQuest : BaseQuest
    { 
        public SaveHisDadQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(DreadSpiderSilk), "dread spider silk", 1, typeof(Leon), "Leon", 600));		
							
            this.AddReward(new BaseReward(1075339)); // Hurry! You must get the silk to Leon the Alchemist quickly, or it will crumble and become useless!
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.CovetousGhost;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(FathersGratitudeQuest);
            }
        }
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        /* Save His Dad */
        public override object Title
        {
            get
            {
                return 1075337;
            }
        }
        /* My father, Andros, is a smith in Minoc. Last week his forge overturned and he was splashed by molten steel. He was horribly burned, 
        and we feared he would die. An alchemist in Vesper promised to make a bandage that could heal him, but he needed the silk of a dread 
        spider. I came here to get some, but I was careless, and succumbed to their poison. Please, won’t you help my father? */
        public override object Description
        {
            get
            {
                return 1075338;
            }
        }
        /* Oh . . . that’s your decision . . . OooOoooOOoo . . . */
        public override object Refuse
        {
            get
            {
                return 1075340;
            }
        }
        /* Thank you! Deliver it to Leon the Alchemist in Vesper. The silk crumbles easily, and much time has already passed since I died. 
        Please! Hurry! */
        public override object Uncomplete
        {
            get
            {
                return 1075341;
            }
        }
        /* How may I help thee? You have the silk of a dread spider? Of course I can make you a bandage, but what happened to Frederic? */
        public override object Complete
        {
            get
            {
                return 1075342;
            }
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

    public class FathersGratitudeQuest : BaseQuest
    { 
        public FathersGratitudeQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(AlchemistsBandage), "alchemist's bandage", 1, typeof(Andros), "Andros"));		
							
            this.AddReward(new BaseReward(typeof(AndrosGratitude), 1075345)); // Andros’ Gratitude
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.CovetousGhost;
            }
        }
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        /* A Father’s Gratitude */
        public override object Title
        {
            get
            {
                return 1075343;
            }
        }
        /* That is simply terrible. First Andros, and now his son. Well, let’s make sure Frederic’s sacrifice wasn’t in vain. Will you take 
        the bandages to his father? You can probably deliver them faster than I can, can’t you? */
        public override object Description
        {
            get
            {
                return 1075344;
            }
        }
        /* Well I’m sorry to hear you say that. Without your help, I don’t know if I can get these to Andros quickly enough to help him. */
        public override object Refuse
        {
            get
            {
                return 1075346;
            }
        }
        /* I don’t know how much longer Andros will survive. You’d better get this to him as quick as you can. Every second counts! */
        public override object Uncomplete
        {
            get
            {
                return 1075347;
            }
        }
        /* Sorry, I’m not accepting commissions at the moment. What? You have the bandage I need from Leon? Thank you so much! But why didn’t 
        my son bring this to me himself? . . . Oh, no! You can't be serious! *sag* My Freddie, my son! Thank you for carrying out his last wish. 
        Here -- I made this for my son, to give to him when he became a journeyman. I want you to have it. */
        public override object Complete
        {
            get
            {
                return 1075348;
            }
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