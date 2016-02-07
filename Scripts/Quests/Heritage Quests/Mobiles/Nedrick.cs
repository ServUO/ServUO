using System;
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

        public override object ConfirmMessage
        {
            get
            {
                return 1073643;
            }
        }// Are you sure you wish to embrace your humanity?
        public override object IncompleteMessage
        {
            get
            {
                return 1074412;
            }
        }// You have made a good start but have more yet to do.	
        public override void InitBody()
        { 
            this.Female = false;
            this.Race = Race.Human;		
			
            this.Hue = 0x8419;
            this.HairItemID = 0x203B;
            this.HairHue = 0x464;
            this.HairItemID = 0x203F;
            this.HairHue = 0x464;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());		
            this.AddItem(new ThighBoots());
            this.AddItem(new LongPants(0x528));
            this.AddItem(new FancyShirt(0x4C3));
            this.AddItem(new Tunic(0x3));		
        }

        public override void Initialize()
        {
            this.Quests.Add(new HeritageQuestInfo(typeof(IngenuityQuest), 1074350));
            this.Quests.Add(new HeritageQuestInfo(typeof(HeaveHoQuest), 1074351));
            this.Quests.Add(new HeritageQuestInfo(typeof(ResponsibilityQuest), 1074352));
            this.Quests.Add(new HeritageQuestInfo(typeof(AllSeasonAdventurerQuest), 1074353));
				
            this.Objectives.Add(1074403); // Greetings, traveler and welcome.
            this.Objectives.Add(1074404); // Perhaps you have heard of the service I offer?  Perhaps you wish to avail yourself of the opportunity I lay before you.
            this.Objectives.Add(1074405); // Elves and humans; we lived together once in peace.  Mighty relics that attest to our friendship remain, of course.  Yet, memories faded when the Gem was shattered and the world torn asunder.  Alone in The Heartwood, our elven brothers and sisters wondered what terrible evil had befallen Sosaria.
            this.Objectives.Add(1074406); // Violent change marked the sundering of our ties.  We are different -- elves and humans.  And yet we are much alike.  I can give an elf the chance to walk as a human upon Sosaria.  I can undertake the transformation.
            this.Objectives.Add(1074407); // But you must prove yourself to me.  Humans possess a strength of character and back.  Humans are quick-witted and able to pick up a smattering of nearly any talent.  Humans are tough both mentally and physically.  And of course, humans defend their own -- sometimes with their own lives.
            this.Objectives.Add(1074408); // Seek Sledge the Versatile and learn about human ingenuity and creativity.  Seek Patricus and demonstrate your integrity and strength.
            this.Objectives.Add(1074409); // Seek out a human in need and prove your worth as a defender of humanity.  Seek Belulah in Nu'Jelm and heartily challenge the elements in a display of toughness to rival any human.
            this.Objectives.Add(1074411); // Or turn away and embrace your heritage.  It matters not to me.
		
            this.Story.Add(1074004); // You have carved a path in history, sought to understand the way from our sage companions.
            this.Story.Add(1074005); // And now you have returned full circle to the place of your origin within the arms of Mother Sosaria. There is but one thing left to do if you truly wish to embrace your elven heritage. 
            this.Story.Add(1074006); // To be born once more an elf, you must strip of all worldly possessions. Nothing of man or beast much touch your skin.
            this.Story.Add(1074007); // Then you may step forth into history.	
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