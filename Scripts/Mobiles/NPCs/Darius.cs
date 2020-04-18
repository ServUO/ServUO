using Server.Items;

namespace Server.Engines.Quests
{
    public class Darius : HeritageQuester
    {
        [Constructable]
        public Darius()
            : base("Darius", "The Wise")
        {
        }

        public Darius(Serial serial)
            : base(serial)
        {
        }

        public override object ConfirmMessage => 1073642;// Are you sure you want to follow the elven ways?
        public override object IncompleteMessage => 1074002;// You have begun to walk the path of reclaiming your heritage, but you have not learned all the lessons before you.
        public override void InitBody()
        {
            Female = false;
            Race = Race.Elf;

            Hue = 0x8385;
            HairItemID = 0x2FCE;
            HairHue = 0x1BC;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new WildStaff());
            AddItem(new Sandals(0x1BB));
            AddItem(new GemmedCirclet());
            AddItem(new Tunic(0x3));
        }

        public override void Initialize()
        {
            Quests.Add(new HeritageQuestInfo(typeof(TheJoysOfLifeQuest), 1072787));
            Quests.Add(new HeritageQuestInfo(typeof(DefendingTheHerdQuest), 1072785));
            Quests.Add(new HeritageQuestInfo(typeof(CaretakerOfTheLandQuest), 1072783));
            Quests.Add(new HeritageQuestInfo(typeof(SeasonsQuest), 1072782));
            Quests.Add(new HeritageQuestInfo(typeof(TheBalanceOfNatureQuest), 1072786));
            Quests.Add(new HeritageQuestInfo(typeof(WisdomOfTheSphynxQuest), 1072784));

            Objectives.Add(1073998); // Blessings of Sosaria to you and merry met, friend.
            Objectives.Add(1073999); // I am glad for your company and wonder if you seek the heritage of your people?  I sense within you an elven bloodline -- the purity of which was lost when our brothers and sisters were exiled here in the Rupture.
            Objectives.Add(1074000); // If it is your desire to reclaim your place amongst the people, you must demonstrate that you understand and embrace the responsibilities expected of you as an elf.
            Objectives.Add(1074001); // The most basic lessons of our Sosaria are taught by her humblest children.  Seek Maul, the great bear, who understands instictively the seasons.

            Story.Add(1074004); // You have carved a path in history, sought to understand the way from our sage companions.
            Story.Add(1074005); // And now you have returned full circle to the place of your origin within the arms of Mother Sosaria. There is but one thing left to do if you truly wish to embrace your elven heritage. 
            Story.Add(1074006); // To be born once more an elf, you must strip of all worldly possessions. Nothing of man or beast much touch your skin.
            Story.Add(1074007); // Then you may step forth into history.	
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