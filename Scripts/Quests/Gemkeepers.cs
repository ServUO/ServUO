using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class WarriorsOfTheGemkeeperQuest : BaseQuest
    {
        public WarriorsOfTheGemkeeperQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(MapFragment), "fragment of a map", 1));

            AddReward(new BaseReward(1074876)); // Knowledge of the legendary minotaur.
        }

        public override QuestChain ChainID => QuestChain.GemkeeperWarriors;
        public override Type NextQuest => typeof(CloseEnoughQuest);
        /* Warriors of the Gemkeeper */
        public override object Title => 1074536;
        /* Here we honor the Gemkeeper's Apprentice and seek to aid her efforts against the humans responsible 
        for the death of her teacher - and the destruction of the elven way of life.  Our tales speak of a fierce 
        race of servants of the Gemkeeper, the men-bulls whose battle-skill was renowned. It is desireable to 
        discover the fate of these noble creatures after the Rupture.  Will you seek information? */
        public override object Description => 1074537;
        /* Fine then, I'm shall find another to run my errands then. */
        public override object Refuse => 1074063;
        /* I care not how you get the information.  Kill as many humans as you must ... but find the fate of the 
        minotaurs.  Perhaps another of the Gemkeeper's servants has the knowledge we seek. */
        public override object Uncomplete => 1074540;
        /* What have you found? */
        public override object Complete => 1074542;
        public override bool CanOffer()
        {
            return MondainsLegacy.Labyrinth;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1074541, null, 0x23); // You have discovered an important clue!						
            Owner.PlaySound(CompleteSound);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CloseEnoughQuest : BaseQuest
    {
        public CloseEnoughQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(MapFragment), "fragment of a map", 1, typeof(Canir), "Canir (Sanctuary)"));

            AddReward(new BaseReward(1074876)); // Knowledge of the legendary minotaur.
        }

        public override QuestChain ChainID => QuestChain.GemkeeperWarriors;
        public override Type NextQuest => typeof(TakingTheBullByTheHornsQuest);
        /* Close Enough */
        public override object Title => 1074544;
        /* Ah ha!  You see here ... and over here ... The map fragment places the city of the bull-men, Labyrinth, 
        on that piece of Sosaria that was thrown into the sky.  Hmmm, I would have you go there and seek out these 
        warriors to see if they might join our cause.  But, legend speaks of a mighty barrier to prevent invasion 
        of the city. Take this map to Canir and explain the problem. Perhaps she can devise a solution. */
        public override object Description => 1074546;
        /* Fine then, I'm shall find another to run my errands then. */
        public override object Refuse => 1074063;
        /* Canir is nearby, run and speak with her. */
        public override object Uncomplete => 1074548;
        /* Yes?  What do you want?  I'm very busy. */
        public override object Complete => 1074549;
        public override bool CanOffer()
        {
            return MondainsLegacy.Labyrinth;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TakingTheBullByTheHornsQuest : BaseQuest
    {
        public TakingTheBullByTheHornsQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(GamanHorns), "gaman horns", 20, 0x1084));

            AddReward(new BaseReward(1074876)); // Knowledge of the legendary minotaur.
        }

        public override QuestChain ChainID => QuestChain.GemkeeperWarriors;
        public override Type NextQuest => typeof(EmissaryToTheMinotaurQuest);
        /* Taking the Bull by the Horns */
        public override object Title => 1074551;
        /* Interesting.  I believe I have a way.  I will need some materials to infuse you with the essence of a 
        bull-man, so you can fool their defenses.  The most similar beast to the original Baratarian bull that the 
        minotaur were bred from is undoubtedly the mighty Gaman, native to the Lands of the Feudal Lords.  I need 
        horns, in great quantity to undertake this magic. */
        public override object Description => 1074553;
        /* Oh come now, don't be afraid.  The magic won't harm you. */
        public override object Refuse => 1074554;
        /* I cannot grant you the ability to pass through the bull-men's defenses without the gaman horns. */
        public override object Uncomplete => 1074555;
        /* You've returned at last!  Give me just a moment to examine what you've brought and I can perform the 
        magic that will allow you enter the Labyrinth. */
        public override object Complete => 1074556;
        public override bool CanOffer()
        {
            return MondainsLegacy.Labyrinth;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EmissaryToTheMinotaurQuest : BaseQuest
    {
        public EmissaryToTheMinotaurQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(MinotaurArtifact), "minotaur artifacts", 3));

            AddReward(new BaseReward(typeof(RewardBox), 1072584)); // A strongbox.
        }

        public override QuestChain ChainID => QuestChain.GemkeeperWarriors;
        /* Emissary to the Minotaur */
        public override object Title => 1074824;
        /* *whew*  It is done!  The fierce essence of the bull has been infused into your aura.  You are able 
        now to breach the ancient defenses of the city.  Go forth and seek the minotaur -- and then return 
        with wonderous tales and evidence of your visit to the Labyrinth. */
        public override object Description => 1074825;
        /* As you wish.  I can't understand why you'd pass up such a remarkable opportunity.  Think of the 
        adventures you would have. */
        public override object Refuse => 1074827;
        /* You won't reach the minotaur city by loitering around here!  What are you waiting for?  You need to 
        get to Malas and find the access point for the island.  You'll be renowned for your discovery! */
        public override object Uncomplete => 1074828;
        /* Oh! You've returned at last!  I can't wait to hear the tales ... but first, let me see those artifacts.  
        You've certainly earned this reward. */
        public override object Complete => 1074829;
        public override bool CanOffer()
        {
            return MondainsLegacy.Labyrinth;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}