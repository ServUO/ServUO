using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Gumps;
namespace Server.Engines.Quests
{
    public class QuestOfSingularity : BaseQuest
    {
        public QuestOfSingularity() : base()
        {
            AddObjective(new QuestionAndAnswerObjective(4, m_EntryTable));
        }

        public override bool DoneOnce { get { return true; } }
        public override bool ShowDescription { get { return false; } }

        //La Insep Om
        public override object Title { get { return 1112681; } }

        /*Repeating the mantra, you gradually enter a state of enlightened meditation.<br><br>
         * As you contemplate your worthiness, an image of the Book of Circles comes into focus.<br><br>
         * Perhaps you are ready for La Insep Om?<br>
         */ 
        public override object Description { get { return 1112682; } }

        //You feel as if you should return when you are worthy.
        public override object Refuse { get { return 1112683; } }

        //Focusing more upon the Book of Circles, you realize that you must now show your mastery of its contents.<br>
        public override object Uncomplete { get { return 1112684; } }

        /*Answering the last question correctly, you feel a strange energy wash over you.<br><br>
         * You don't understand how you know, but you are absolutely certain that the guardians will 
         * no longer bar you from entering the Stygian Abyss.<br><br>It seems you have proven yourself 
         * worthy of La Insep Om.
         */
        public override object Complete { get { return 1112700; } }

        /*
         * You realize that is not the correct answer.<br><br>You vow to study the Book of Circles again 
         * so that you might understand all that is required of you. Perhaps meditating again soon will bring 
         * the wisdom that you seek.<br>
         */
        public override object FailedMsg { get { return 1112680; } }

        public override void OnAccept()
        {
            base.OnAccept();
            Owner.SendGump(new QAndAGump(Owner, this));
        }

        public override void GiveRewards()
        {
            base.GiveRewards();

            Owner.PlaySound(0x1E0);
            Owner.FixedParticles(0x3709, 1, 30, 1153, 13, 3, EffectLayer.Head);
            Owner.AbyssEntry = true;
        }

        public override void OnResign(bool chain)
        {
            base.OnResign(chain);
            ShrineOfSingularity.AddToTable(Owner);
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

        public static void Configure()
        {
            m_EntryTable[0] = new QuestionAndAnswerEntry(1112601, new string[] { "control", "passion", "diligence" }, new string[] { "chaos", "order", "singularity" }); //Which of these is a Principle?
            m_EntryTable[1] = new QuestionAndAnswerEntry(1112602, new string[] { "control" }, new string[] { "passion", "diligence", "feeling" }); //Which of these is a Principle?
            m_EntryTable[2] = new QuestionAndAnswerEntry(1112603, new string[] { "feeling" }, new string[] { "direction", "persistence", "control" }); //From Passion springs which Virtue?
            m_EntryTable[3] = new QuestionAndAnswerEntry(1112604, new string[] { "persistence" }, new string[] { "feeling", "direction", "alexandria" }); //From Diligence springs which Virtue?
            m_EntryTable[4] = new QuestionAndAnswerEntry(1112605, new string[] { "no" }, new string[] { "yes" }); //Is any Virtue more important than another?
            m_EntryTable[5] = new QuestionAndAnswerEntry(1112606, new string[] { "yes" }, new string[] { "no" }); //Are each of the Virtues considered to be equal?
            m_EntryTable[6] = new QuestionAndAnswerEntry(1112607, new string[] { "eight" }, new string[] { "seven", "nine", "six", "eight" }); //Amongst all else, of how many Virtues does the Circle consist?
            m_EntryTable[7] = new QuestionAndAnswerEntry(1112608, new string[] { "balance" }, new string[] { "achievement", "precision", "chaos" }); //Passion combined with Control yields which Virtue?
            m_EntryTable[8] = new QuestionAndAnswerEntry(1112609, new string[] { "passion", "deligience" }, new string[] { "control" }); //Achievement is created in part by which Principle?
            m_EntryTable[9] = new QuestionAndAnswerEntry(1112610, new string[] { "precision" }, new string[] { "chaos", "achievement", "balance" }); //If you join Diligence with Control, which Virtue is provided?
            m_EntryTable[10] = new QuestionAndAnswerEntry(1112611, new string[] { "chaos" }, new string[] { "order", "achievement", "precision" }); //The absence of the Principles is called what?
            m_EntryTable[11] = new QuestionAndAnswerEntry(1112612, new string[] { "order" }, new string[] { "achievement", "precision", "balance" }); //The existence of Chaos points to which Virtue?
            m_EntryTable[12] = new QuestionAndAnswerEntry(1112613, new string[] { "singularity" }, new string[] { "direction", "persistence", "control" }); //Unifying the three Principles forms what?
            m_EntryTable[13] = new QuestionAndAnswerEntry(1112614, new string[] { "singularity" }, new string[] { "compassion", "justice", "honor" }); //Which is the eighth Virtue?
            m_EntryTable[14] = new QuestionAndAnswerEntry(1112615, new string[] { "singularity" }, new string[] { "compassion", "justice", "honor" }); //Which is the first Virtue?
            m_EntryTable[15] = new QuestionAndAnswerEntry(1112616, new string[] { "singularity" }, new string[] { "circle of life", "feeling", "diligence" }); //In what can you find all of the Principles, and thus all of the Virtues?
            m_EntryTable[16] = new QuestionAndAnswerEntry(1112617, new string[] { "no" }, new string[] { "yes" }); //Does the Circle have an end?
            m_EntryTable[17] = new QuestionAndAnswerEntry(1112618, new string[] { "yes" }, new string[] { "no" }); //Are each of the Principles considered equal?
            m_EntryTable[18] = new QuestionAndAnswerEntry(1112619, new string[] { "forever" }, new string[] { "when it ends", "tomorrow", "next week" }); //As with the Circle, how long does our society continue?
            m_EntryTable[19] = new QuestionAndAnswerEntry(1112620, new string[] { "control", "passion", "diligence", "direction", "persistene", "feeling", "chaos", "singularity" }, new string[] { "order", "alexandria", "OSI" }); //Which of these is one of the Virtues?
        }

        private static QuestionAndAnswerEntry[] m_EntryTable = new QuestionAndAnswerEntry[20];
        public static QuestionAndAnswerEntry[] EntryTable { get { return m_EntryTable; } }
    }
}