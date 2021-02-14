using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class LastWordsQuest : BaseQuest
    {
        public LastWordsQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(DenthesJournal), "lord denthe's journal", 1, typeof(Verity), "Verity (Britain)"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Last Words */
        public override object Title => 1073046;
        /* You have discovered a blood-stained journal that details the adventures of Lord Denthe.  
        The pages make for fascinating reading, telling the tale of the great man's efforts to 
        uncover the secrets of the Prism of Light.  Will you donate the journal to a library so 
        others may hear the tale of Lord Denthe's final expedition? */
        public override object Description => 1074650;
        /* Or, you could keep the journal for your private library. */
        public override object Refuse => 1074651;
        /* The librarian in Britain would be glad of your donation of Lord Denthe's journal. */
        public override object Uncomplete => 1074652;
        /* Shhhh!  What have YOU done to that book?  It's covered in ... is that blood?  *furious glare*  
        Let me see that.  Oh.  Oh, I see.  Do you realize how important this journal is?  Here, take this 
        for your trouble. */
        public override object Complete => 1074653;
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

    public class DenthesJournal : BaseQuestItem
    {
        [Constructable]
        public DenthesJournal()
            : base(0xFF2)
        {
        }

        public DenthesJournal(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(LastWordsQuest)
                };
        public override int LabelNumber => 1073240;// Lord Denthe's Journal
        public override int Lifespan => 3600;// ?
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