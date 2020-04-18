using Server.Items;

namespace Server.Engines.Quests
{
    public class Nedrick : HeritageQuester
    {
        [Constructable]
        public Nedrick()
            : base("Nedrick", "The Iron Worker")
        {
        }

        public Nedrick(Serial serial)
            : base(serial)
        {
        }

        public override object ConfirmMessage => 1073643;// Are you sure you wish to embrace your humanity?
        public override object IncompleteMessage => 1074412;// You have made a good start but have more yet to do.	
        public override void InitBody()
        {
            Female = false;
            Race = Race.Human;

            Hue = 0x8419;
            HairItemID = 0x203B;
            HairHue = 0x464;
            HairItemID = 0x203F;
            HairHue = 0x464;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new ThighBoots());
            AddItem(new LongPants(0x528));
            AddItem(new FancyShirt(0x4C3));
            AddItem(new Tunic(0x3));
        }

        public override void Initialize()
        {
            Quests.Add(new HeritageQuestInfo(typeof(IngenuityQuest), 1074350));
            Quests.Add(new HeritageQuestInfo(typeof(HeaveHoQuest), 1074351));
            Quests.Add(new HeritageQuestInfo(typeof(ResponsibilityQuest), 1074352));
            Quests.Add(new HeritageQuestInfo(typeof(AllSeasonAdventurerQuest), 1074353));

            Objectives.Add(1074403); // Greetings, traveler and welcome.
            Objectives.Add(1074404); // Perhaps you have heard of the service I offer?  Perhaps you wish to avail yourself of the opportunity I lay before you.
            Objectives.Add(1074405); // Elves and humans; we lived together once in peace.  Mighty relics that attest to our friendship remain, of course.  Yet, memories faded when the Gem was shattered and the world torn asunder.  Alone in The Heartwood, our elven brothers and sisters wondered what terrible evil had befallen Sosaria.
            Objectives.Add(1074406); // Violent change marked the sundering of our ties.  We are different -- elves and humans.  And yet we are much alike.  I can give an elf the chance to walk as a human upon Sosaria.  I can undertake the transformation.
            Objectives.Add(1074407); // But you must prove yourself to me.  Humans possess a strength of character and back.  Humans are quick-witted and able to pick up a smattering of nearly any talent.  Humans are tough both mentally and physically.  And of course, humans defend their own -- sometimes with their own lives.
            Objectives.Add(1074408); // Seek Sledge the Versatile and learn about human ingenuity and creativity.  Seek Patricus and demonstrate your integrity and strength.
            Objectives.Add(1074409); // Seek out a human in need and prove your worth as a defender of humanity.  Seek Belulah in Nu'Jelm and heartily challenge the elements in a display of toughness to rival any human.
            Objectives.Add(1074411); // Or turn away and embrace your heritage.  It matters not to me.

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