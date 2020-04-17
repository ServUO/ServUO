using Server.Gumps;
using Server.Items;
namespace Server.Engines.Quests
{
    public class QuestOfSingularity : BaseQuest
    {
        public QuestOfSingularity() : base()
        {
            AddObjective(new QuestionAndAnswerObjective(4, m_EntryTable));
        }

        public override bool DoneOnce => true;
        public override bool ShowDescription => false;

        //La Insep Om
        public override object Title => 1112681;

        /*Repeating the mantra, you gradually enter a state of enlightened meditation.<br><br>
         * As you contemplate your worthiness, an image of the Book of Circles comes into focus.<br><br>
         * Perhaps you are ready for La Insep Om?<br>
         */
        public override object Description => 1112682;

        //You feel as if you should return when you are worthy.
        public override object Refuse => 1112683;

        //Focusing more upon the Book of Circles, you realize that you must now show your mastery of its contents.<br>
        public override object Uncomplete => 1112684;

        /*Answering the last question correctly, you feel a strange energy wash over you.<br><br>
         * You don't understand how you know, but you are absolutely certain that the guardians will 
         * no longer bar you from entering the Stygian Abyss.<br><br>It seems you have proven yourself 
         * worthy of La Insep Om.
         */
        public override object Complete => 1112700;

        /*
         * You realize that is not the correct answer.<br><br>You vow to study the Book of Circles again 
         * so that you might understand all that is required of you. Perhaps meditating again soon will bring 
         * the wisdom that you seek.<br>
         */
        public override object FailedMsg => 1112680;

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
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public static void Configure()
        {
            m_EntryTable[0] = new QuestionAndAnswerEntry(1112601, new object[] { 1112656 /*Control*/ }, new object[] { 1112661 /*Persistence*/, 1112664 /*Precision*/, 1112660 /*Feeling*/ }); //Which of these is a Principle?
            m_EntryTable[1] = new QuestionAndAnswerEntry(1112602, new object[] { 1112656 /*Control*/ }, new object[] { 1112662 /*Balance*/, 1112658 /*Diligence*/, 1112657 /*Passion*/ }); //From what Principle does Direction spring?
            m_EntryTable[2] = new QuestionAndAnswerEntry(1112603, new object[] { 1112660 /*Feeling*/ }, new object[] { 1112663 /*Achievement*/, 1112661 /*Persistence*/, 1112656 /*Control*/ }); //From Passion springs which Virtue?
            m_EntryTable[3] = new QuestionAndAnswerEntry(1112604, new object[] { 1112661 /*Persistence*/ }, new object[] { 1112667 /*Singularity*/, 1112656 /*Control*/, 1112666 /*Order*/ }); //From Diligence springs which Virtue?
            m_EntryTable[4] = new QuestionAndAnswerEntry(1112605, new object[] { 1112669 /*No*/ }, new object[] { 1112652 /*Spirituality is the most important*/, 1112645 /*All but chaos are important*/, 1112644 /*Order is more important*/ }); //Is any Virtue more important than another?
            m_EntryTable[5] = new QuestionAndAnswerEntry(1112606, new object[] { 1112649 /*All are equal*/ }, new object[] { 1112646 /*Singularity is more imporant than all others*/, 1112669 /*No*/, 1112644 /*Order is more important*/  }); //Are each of the Virtues considered to be equal?
            m_EntryTable[6] = new QuestionAndAnswerEntry(1112607, new object[] { 1112668 /*Eight*/ }, new object[] { 1112653 /*Seven*/, 1112654 /*Ten*/, 1112655 /*Twelve*/ }); //Amongst all else, of how many Virtues does the Circle consist?
            m_EntryTable[7] = new QuestionAndAnswerEntry(1112608, new object[] { 1112662 /*Balance*/ }, new object[] { 1112663 /*Achievement*/, 1112664 /*Precision*/, 1112665 /*Chaos*/ }); //Passion combined with Control yields which Virtue?
            m_EntryTable[8] = new QuestionAndAnswerEntry(1112609, new object[] { 1112658, 1112657/*Diligence, Passion*/ }, new object[] { 1112656 /*Control*/, 1112661 /*Persistence*/ }); //Achievement is created in part by which Principle?
            m_EntryTable[9] = new QuestionAndAnswerEntry(1112610, new object[] { 1112664 /*Precision*/ }, new object[] { 1112665 /*Chaos*/, 1112663 /*Achievement*/, 1112662 /*Balance*/ }); //If you join Diligence with Control, which Virtue is provided?
            m_EntryTable[10] = new QuestionAndAnswerEntry(1112611, new object[] { 1112665 /*Chaos*/ }, new object[] { 1112666 /*Order*/, 1112663 /*Achievement*/, 1112664 /*Precision*/ }); //The absence of the Principles is called what?
            m_EntryTable[11] = new QuestionAndAnswerEntry(1112612, new object[] { 1112666 /*Order*/ }, new object[] { 1112663 /*Achievement*/, 1112664 /*Precision*/, 1112662 /*Balance*/ }); //The existence of Chaos points to which Virtue?
            m_EntryTable[12] = new QuestionAndAnswerEntry(1112613, new object[] { 1112667 /*Singularity*/ }, new object[] { 1112659 /*Direction*/, 1112661 /*Persistence*/, 1112656 /*Control*/ }); //Unifying the three Principles forms what?
            m_EntryTable[13] = new QuestionAndAnswerEntry(1112614, new object[] { 1112667 /*Singularity*/ }, new object[] { 1112647 /*Nothing*/, 1112663 /*Achievement*/, 1112659 /*Direction*/ }); //Which is the eighth Virtue?
            m_EntryTable[14] = new QuestionAndAnswerEntry(1112615, new object[] { 1112667 /*Singularity*/ }, new object[] { 1112663 /*Achievement*/, 1112659 /*Direction*/, 1112647 /*Nothing*/ }); //Which is the first Virtue?
            m_EntryTable[15] = new QuestionAndAnswerEntry(1112616, new object[] { 1112667 /*Singularity*/ }, new object[] { 1112661 /*Persistence*/, 1112660 /*Feeling*/, 1112658 /*Diligence*/ }); //In what can you find all of the Principles, and thus all of the Virtues?
            m_EntryTable[16] = new QuestionAndAnswerEntry(1112617, new object[] { 1112669 /*No*/ }, new object[] { 1112670 /*Yes*/ }); //Does the Circle have an end?
            m_EntryTable[17] = new QuestionAndAnswerEntry(1112618, new object[] { 1112670 /*Yes*/ }, new object[] { 1112669 /*No*/ }); //Are each of the Principles considered equal?
            m_EntryTable[18] = new QuestionAndAnswerEntry(1112619, new object[] { 1112671 /*Forever*/ }, new object[] { 1112672 /*As all things have a beginning, all things must have an end*/, 1112673 /*After the Great Breaking has come*/, 1112647 /*Nothing*/ }); //As with the Circle, how long does our society continue?
            m_EntryTable[19] = new QuestionAndAnswerEntry(1112620, new object[] { 1112656 /*Control*/ }, new object[] { 1112665 /*Chaos*/, 1112666 /*Order*/, 1112664 /*Precision*/ }); //Which of these is one of the Virtues?
        }

        private static readonly QuestionAndAnswerEntry[] m_EntryTable = new QuestionAndAnswerEntry[20];
        public static QuestionAndAnswerEntry[] EntryTable => m_EntryTable;
    }
}
