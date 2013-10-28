using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class TheAncientWorldQuest : BaseQuest
    { 
        public TheAncientWorldQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(MapFragment), "fragment of a map", 1));		
							
            this.AddReward(new BaseReward(1074876)); // Knowledge of the legendary minotaur.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.AncientWorld;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(TheGoldenHornQuest);
            }
        }
        /* The Ancient World */
        public override object Title
        {
            get
            {
                return 1074534;
            }
        }
        /* The lore of my people mentions Mondain many times. In one tale, it is 
        revealed that he created and enslaved a race -- a sort of man bull, known 
        as a 'minotaur'. The tales speak of mighty warriors who charged with 
        blood-soaked horns into the heat of battle.  But, alas, the fate of the 
        bull-men is unknown after the rupture.  Will you seek information about 
        their civilization? */
        public override object Description
        {
            get
            {
                return 1074535;
            }
        }
        /* I am disappointed, but I respect your decision. */
        public override object Refuse
        {
            get
            {
                return 1074538;
            }
        }
        /* A traveler has told me that worshippers of Mondain still exist and 
        wander the land.  Perhaps their lore speaks of whether the bull-men 
        survived.  I do not think they share their secrets gladly.  You may 
        need to be 'persuasive'. */
        public override object Uncomplete
        {
            get
            {
                return 1074539;
            }
        }
        /* What have you found? */
        public override object Complete
        {
            get
            {
                return 1074542;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Labyrinth;
        }

        public override void OnCompleted()
        {
            this.Owner.SendLocalizedMessage(1074541, null, 0x23); // You have discovered an important clue!						
            this.Owner.PlaySound(this.CompleteSound);
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

    public class TheGoldenHornQuest : BaseQuest
    { 
        public TheGoldenHornQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(MapFragment), "fragment of a map", 1, typeof(Braen), "Braen (The Heartwood)"));		
							
            this.AddReward(new BaseReward(1074876)); // Knowledge of the legendary minotaur.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.AncientWorld;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(BullishQuest);
            }
        }
        /* The Golden Horn */
        public override object Title
        {
            get
            {
                return 1074543;
            }
        }
        /* Ah ha!  You see here ... and over here ... The map fragment places the 
        city of the bull-men, Labyrinth, on that piece of Sosaria that was thrown 
        into the sky. Hmmm, I would have you go there and find any artifacts that 
        remain that help tell the story.  But, legend speaks of a mighty barrier 
        to prevent invasion of the city. Take this map to Braen and explain the 
        problem. Perhaps he can devise a solution. */
        public override object Description
        {
            get
            {
                return 1074545;
            }
        }
        /* I am disappointed, but I respect your decision. */
        public override object Refuse
        {
            get
            {
                return 1074538;
            }
        }
        /* Braen is nearby, run and speak with him. */
        public override object Uncomplete
        {
            get
            {
                return 1074547;
            }
        }
        /* Yes?  What do you want?  I'm very busy. */
        public override object Complete
        {
            get
            {
                return 1074549;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Labyrinth;
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

    public class BullishQuest : BaseQuest
    { 
        public BullishQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(GamanHorns), "gaman horns", 20, 0x1084));		
							
            this.AddReward(new BaseReward(1074876)); // Knowledge of the legendary minotaur.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.AncientWorld;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(LostCivilizationQuest);
            }
        }
        /* Bullish */
        public override object Title
        {
            get
            {
                return 1074550;
            }
        }
        /* Oh, I see. I will need some materials to infuse you with the essence of a 
        bull-man, so you can fool their defenses.  The most similar beast to the original 
        Baratarian bull that the minotaur were bred from is undoubtedly the mighty Gaman, 
        native to the Lands of the Feudal Lords.  I need horns, in great quantity to 
        undertake this magic. */
        public override object Description
        {
            get
            {
                return 1074552;
            }
        }
        /* Oh come now, don't be afraid.  The magic won't harm you. */
        public override object Refuse
        {
            get
            {
                return 1074554;
            }
        }
        /* I cannot grant you the ability to pass through the bull-men's defenses without 
        the gaman horns. */
        public override object Uncomplete
        {
            get
            {
                return 1074555;
            }
        }
        /* You've returned at last!  Give me just a moment to examine what you've brought 
        and I can perform the magic that will allow you enter the Labyrinth. */
        public override object Complete
        {
            get
            {
                return 1074556;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Labyrinth;
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

    public class LostCivilizationQuest : BaseQuest
    { 
        public LostCivilizationQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(MinotaurArtifact), "minotaur artifacts", 3));		
							
            this.AddReward(new BaseReward(typeof(RewardBox), 1072584)); // A strongbox.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.AncientWorld;
            }
        }
        /* Lost Civilization */
        public override object Title
        {
            get
            {
                return 1074823;
            }
        }
        /* *whew*  It is done!  The fierce essence of the bull has been 
        infused into your aura.  You are able now to breach the ancient 
        defenses of the city.  Go forth and seek the minotaur -- and 
        then return with wonderous tales and evidence of your visit to 
        the Labyrinth. */
        public override object Description
        {
            get
            {
                return 1074825;
            }
        }
        /* As you wish.  I can't understand why you'd pass up such a 
        remarkable opportunity.  Think of the adventures you would have. */
        public override object Refuse
        {
            get
            {
                return 1074827;
            }
        }
        /* You won't reach the minotaur city by loitering around here!  
        What are you waiting for?  You need to get to Malas and find 
        the access point for the island.  You'll be renowned for your 
        discovery! */
        public override object Uncomplete
        {
            get
            {
                return 1074828;
            }
        }
        /* Oh! You've returned at last!  I can't wait to hear the 
        tales ... but first, let me see those artifacts.  You've 
        certainly earned this reward. */
        public override object Complete
        {
            get
            {
                return 1074829;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Labyrinth;
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