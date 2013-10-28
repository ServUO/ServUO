using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class UnfadingMemoriesOneQuest : BaseQuest
    { 
        public UnfadingMemoriesOneQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(PrismaticAmber), "prismatic amber", 1));		
							
            this.AddReward(new BaseReward(1075357)); // The joy of contributing to a noble artistic effort, however paltry the end product.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.UnfadingMemories;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(UnfadingMemoriesTwoQuest);
            }
        }
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        /* Unfading Memories */
        public override object Title
        {
            get
            {
                return 1075355;
            }
        }
        /* Aargh! It’s just not right! It doesn’t capture the unique color of her hair at all! If only I had some Prismatic 
        Amber. That would be perfect. They used to mine it in Malas, but alas, those veins ran dry some time ago. I hear it 
        may have been found in the Prism of Light. Oh, if only there were a bold adventurer within earshot who would go to 
        the Prism of Light and retrieve some for me! */
        public override object Description
        {
            get
            {
                return 1075356;
            }
        }
        /* Is there no one who can help a humble artist pursue his Muse? */
        public override object Refuse
        {
            get
            {
                return 1075358;
            }
        }
        /* You can find Prismatic Amber in the Prism of Light, located just north of the city of Nujel'm. */
        public override object Uncomplete
        {
            get
            {
                return 1075359;
            }
        }
        /* I knew it! See, it’s just the color I needed! Look how it brings out the highlights of her wheaten tresses! */
        public override object Complete
        {
            get
            {
                return 1075360;
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

    public class UnfadingMemoriesTwoQuest : BaseQuest
    { 
        public UnfadingMemoriesTwoQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(PortraitOfTheBride), "portrait of the bride", 1, typeof(Thalia), "Bride"));		
							
            this.AddReward(new BaseReward(1075369)); // The Artist’s gratitude.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.UnfadingMemories;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(UnfadingMemoriesThreeQuest);
            }
        }
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        /* Unfading Memories */
        public override object Title
        {
            get
            {
                return 1075367;
            }
        }
        /* Finished! With the pigment I was able to create from the Prismatic Amber you brought me, I was able to complete 
        my humble work. I should explain. Once, I loved a noble lady of gentleness and refinement, who possessed such beauty 
        that I have found myself unable to love another to this day. But it was from afar that I admired her, for it is not 
        for one so lowly as I to pay court to the likes of her. You have heard of the fair Thalia, Lady of Nujel'm? No? Well, 
        she was my Muse, my inspiration, and when I heard she was to be married, I lost whatever pitiful talent I possessed. 
        I felt I must compose a portrait of her, my masterpiece, or I would never be able to paint again. You, my friend, have 
        helped me complete my work. Now I ask another favor of you. Will you take it to her as a wedding gift? She will probably 
        reject it, but I must make the offer. */
        public override object Description
        {
            get
            {
                return 1075368;
            }
        }
        /* Alright then, you have already helped me more than I deserved. I shall find someone else to undertake this task. */
        public override object Refuse
        {
            get
            {
                return 1075370;
            }
        }
        /* The wedding is taking place in the palace in Nujel'm. You will likely find her there. */
        public override object Uncomplete
        {
            get
            {
                return 1075371;
            }
        }
        /* I’m sorry, I’m getting ready to be married. I don’t have time to . . . what’s that you say? */
        public override object Complete
        {
            get
            {
                return 1075372;
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

    public class UnfadingMemoriesThreeQuest : BaseQuest
    { 
        public UnfadingMemoriesThreeQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(BridesLetter), "brides letter", 1, typeof(Emilio), "Artist"));		
							
            this.AddReward(new BaseReward(typeof(Bleach), 1075375)); // Bleach
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.UnfadingMemories;
            }
        }
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        /* Unfading Memories */
        public override object Title
        {
            get
            {
                return 1075373;
            }
        }
        /* Emilio painted this? It is absolutely wonderful! I used to love looking at his paintings, but I don’t remember him creating 
        anything like this before. Would you be so kind as to carry a letter to him? Fate may have it that I am to marry another, yet 
        I am compelled to reveal to him that his love was not entirely unrequited. */
        public override object Description
        {
            get
            {
                return 1075374;
            }
        }
        /* Very well, then. If you will excuse me, I need to get ready. */
        public override object Refuse
        {
            get
            {
                return 1075376;
            }
        }
        /* Take the letter back to the Artist’s Guild in Britain, if you would do me this kindness. */
        public override object Uncomplete
        {
            get
            {
                return 1075377;
            }
        }
        /* She said what? She thinks what of me? I . . . I can’t believe it! All this time, I never knew how she truly felt. Thank you, 
        my friend. I believe now I will be able to paint once again. Here, take this bleach. I was going to use it to destroy all of my 
        works. Perhaps you can find a better use for it now. */
        public override object Complete
        {
            get
            {
                return 1075378;
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