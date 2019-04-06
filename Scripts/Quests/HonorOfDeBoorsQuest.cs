using System;
using Server;
using Server.Items;

namespace Server.Engines.Quests
{
    public class HonorOfDeBoorsQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.HonorOfDeBoors; } }
        public override Type NextQuest { get { return typeof(JackTheVillainQuest); } }
        public override bool DoneOnce { get { return true; } }

        /* The Honor of the De Boors */
        public override object Title { get { return 1075416; } }

        /* I beg your pardon, but will you listen to my story? My family, the de Boors family, have been jewel traders
		as far back as anyone can remember. Alas, by the time I was born, we had fallen on hard times.
		To survive, I have had to sell much of my family’s property. Most of it was meaningless, but I regret that a
		few years ago I made a terrible mistake. I pawned a shield bearing my family’s coat of arms to a loan shark.
		That shield was borne into battle by Jaan de Boors, the founder of our house! It has no value to anyone, but
		that blackguard won’t believe I have no money. He wants a fortune in jewels before he will return it.
		Now I have learned that I am dying. Soon I will be gone, and my lineage with me. For the sake of what little
		honor is left to me and my family name, I cannot bear to leave our ancestral shield in the hands of that villain.
		Will you help me recover it? */
        public override object Description { get { return 1075417; } }

        /* I know how much I am asking. Please, can you not help a dying man restore his family’s honor? */
        public override object Refuse { get { return 1075419; } }

        /* Are you sure? You are very kind. Many of the monsters around here, when slain, are found to have jewels in their stomachs.
		From innocents they have eaten, no doubt. */
        public override object Uncomplete { get { return 1075420; } }

        /* You have done it! Bless you! I do appreciate this very much! Though, will you do me one last favor? */
        public override object Complete { get { return 1075421; } }

        public HonorOfDeBoorsQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Diamond), "Diamonds", 10));
            AddObjective(new ObtainObjective(typeof(Ruby), "Rubies", 10));
            AddObjective(new ObtainObjective(typeof(Emerald), "Emeralds", 10));

            AddReward(new BaseReward(1075418)); // Gather them quickly. Who knows how long Derek has to live?
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

    public class JackTheVillainQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.HonorOfDeBoors; } }
        public override Type NextQuest { get { return typeof(SavedHonorQuest); } }
        public override bool DoneOnce { get { return true; } }

        /* Jack the Villain */
        public override object Title { get { return 1075422; } }

        /*Will you take the jewels to the loan shark? I am not well enough to go myself, though it is not far. */
        public override object Description { get { return 1075423; } }

        /* Ah well. You have already helped me by gathering the jewels. I cannot complain. */
        public override object Refuse { get { return 1075425; } }

        /* The name of the villain is Jack, you will find him over by the port. */
        public override object Uncomplete { get { return 1075426; } }

        /* What do you want? Oh, that jewel merchant wants his shield back, eh? */
        public override object Complete { get { return 1075427; } }

        public JackTheVillainQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(BagOfJewels), "Bag of Jewels", 1, typeof(JackLoanShark), "Jack the Loan Shark"));

            AddReward(new BaseReward(1075424)); // Deliver the bag of jewels to the loan shark.
        }

        public override void OnCompleted()
        {
            Owner.PlaySound(CompleteSound);
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

    public class SavedHonorQuest : BaseQuest
    {
        public override QuestChain ChainID { get { return QuestChain.HonorOfDeBoors; } }
        public override bool DoneOnce { get { return true; } }

        /* Saved Honor */
        public override object Title { get { return 1075428; } }

        /* That idiot! This beat up piece of junk isn’t worth more than three gold coins, four at most!
         * Oh, well, a deal’s a deal! */
        public override object Description { get { return 1075429; } }

        /* I don’t care what you do! */
        public override object Refuse { get { return 1075431; } }

        /* Go away and never come back. */
        public override object Uncomplete { get { return 1075432; } }

        /* My shield! My family’s honor! You have my gratitude. Please, take this goblet.
         * It is small enough repayment for all you have done for me, but it is the only
         * thing of my family’s that I have left. */
        public override object Complete { get { return 1075433; } }

        public SavedHonorQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(DeBoorShield), "Ancestral Shield", 1, typeof(DerekMerchant), "Derek the Merchant"));

            AddReward(new BaseReward(typeof(GobletOfCelebration), 1075309)); // Goblet of Celebration
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

    public class DerekMerchant : MondainQuester
    {
        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof( HonorOfDeBoorsQuest ) };
            }
        }

        [Constructable]
        public DerekMerchant()
            : base("Derek", "the Merchant")
        {
        }

        public DerekMerchant(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x8406;
            HairItemID = 0x2048;
            HairHue = 0x473;
            FacialHairItemID = 0x204B;
            FacialHairHue = 0x473;
        }

        public override void InitOutfit()
        {
            AddItem(new Shoes());
            AddItem(new LongPants(0x901));
            AddItem(new FancyShirt(0x5F4));
            AddItem(new Backpack());
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

    public class JackLoanShark : MondainQuester
    {
        public override Type[] Quests { get { return null; } }  //JackTheVillainQuest

        [Constructable]
        public JackLoanShark()
            : base("Jack", "the Loan Shark")
        {
        }

        public JackLoanShark(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x83EC;
            HairItemID = 0x2045;
            HairHue = 0x464;
            FacialHairItemID = 0x204B;
            FacialHairHue = 0x464;
        }

        public override void InitOutfit()
        {
            AddItem(new Dagger());
            AddItem(new ThighBoots(0x901));
            AddItem(new LongPants(0x521));
            AddItem(new FancyShirt(0x5A7));
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