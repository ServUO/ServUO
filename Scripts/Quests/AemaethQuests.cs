using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class AemaethOneQuest : BaseQuest
    {
        public AemaethOneQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(BasinOfCrystalClearWater), "basin of crystal clear water", 1, typeof(Szandor), "Szandor"));

            AddReward(new BaseReward(1075323)); // Aurelia's gratitude.
        }

        public override QuestChain ChainID => QuestChain.Aemaeth;
        public override Type NextQuest => typeof(AemaethTwoQuest);
        public override bool DoneOnce => true;
        /* Aemaeth */
        public override object Title => 1075321;
        /* My father died in an accident some months ago. My mother refused to accept his death. We had a little money set by, 
        and she took it to a necromancer, who promised to restore my father to life. Well, he revived my father, all right, the 
        cheat! Now my father is a walking corpse, a travesty . . . a monster. My mother is beside herself -- she won't eat, she 
        can't sleep. I prayed at the shrine of Spirituality for guidance, and I must have fallen asleep. When I awoke, there was 
        this basin of clear water. I cannot leave my mother, for I fear what she might do to herself. Could you take this to the 
        graveyard, and give it to what is left of my father? */
        public override object Description => 1075322;
        /* Oh! Alright then. I hope someone comes along soon who can help me, or I don’t know what will become of us. */
        public override object Refuse => 1075324;
        /* My father - or what remains of him - can be found in the graveyard northwest of the city. */
        public override object Uncomplete => 1075325;
        /* What is this you give me? A basin of water? */
        public override object Complete => 1075326;
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

    public class AemaethTwoQuest : BaseQuest
    {
        public AemaethTwoQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(BasinOfCrystalClearWater), "basin of crystal clear water", 1, typeof(Aurelia), "Aurelia"));

            AddReward(new BaseReward(typeof(MirrorOfPurification), 1075329)); // Mirror of Purification
        }

        public override QuestChain ChainID => QuestChain.Aemaeth;
        public override bool DoneOnce => true;
        /* Aemaeth */
        public override object Title => 1075327;
        /* You tell me it is time to leave this flesh. I did not understand until now. I thought: I can see my wife and my daughter, 
        I can speak. Is this not life? But now, as I regard my reflection, I see what I have become. This only a mockery of life. 
        Thank you for having the courage to show me the truth. For the love I bear my wife and daughter, I know now that I must pass 
        beyond the veil. Will you return this basin to Aurelia? She will know by this that I am at rest. */
        public override object Description => 1075328;
        /* You won’t take this back to my daughter? Please, I cannot leave until she knows I am at peace. */
        public override object Refuse => 1075330;
        /* My daughter will be at my home, on the east side of the city. */
        public override object Uncomplete => 1075331;
        /* Thank goodness! Now we can honor my father for the great man he was while he lived, rather than the horror he became. */
        public override object Complete => 1075332;
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