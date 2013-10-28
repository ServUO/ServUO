using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class MisplacedQuest : BaseQuest
    { 
        public MisplacedQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(DisintegratingThesisNotes), "disintegrating thesis notes", 5, 0xEF5));
			
            this.AddReward(new BaseReward(typeof(LibrariansKey), 1074347));
        }

        /* Misplaced */
        public override object Title
        {
            get
            {
                return 1074438;
            }
        }
        /* Shhh!  *nervous chuckle* Oh, sorry about that.  I forget that I'm not in the library any longer -- and instead that 
        THING has taken over.  If that wasn't distressing enough, I've misplaced my thesis pages and they've been gathered up 
        by the shambling dead.  Could you retrieve them for me? */
        public override object Description
        {
            get
            {
                return 1074439;
            }
        }
        /* *tense sigh* Of course, I understand.  If you change your mind, I'll be waiting. */
        public override object Refuse
        {
            get
            {
                return 1074441;
            }
        }
        /* Most of the creatures here wouldn't be interested in my thesis.  *nervous chuckle*  Master Gnosos would argue that 
        no one is -- not even the undead.  Still, I'd wager that the more powerful undead have my pages. */
        public override object Uncomplete
        {
            get
            {
                return 1074442;
            }
        }
        /* Ah!  You've got my pages?  Oh no ... they've been damaged.  Here, take this key.  Perhaps you can find the podium 
        and gain access to the library.  My poor books are being ravaged by that horror and you'd do well to put things right. */
        public override object Complete
        {
            get
            {
                return 1074443;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
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

    public class Cohenn : MondainQuester
    {
        [Constructable]
        public Cohenn()
            : base("Master Cohenn", "the librarian")
        { 
        }

        public Cohenn(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(MisplacedQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x840C;
            this.HairItemID = 0x2045;
            this.HairHue = 0x453;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(0x74A));
            this.AddItem(new Robe(0x498));
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