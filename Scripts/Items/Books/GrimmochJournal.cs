using System;

namespace Server.Items
{
    public class GrimmochJournal1 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day One :",
                "",
                "'Tis a grand sight, this",
                "primeval tomb, I agree",
                "with Tavara on that.",
                "And we've a good crew",
                "here, they've strong",
                "backs and a good"),
            new BookPageInfo(
                "attitude.  I'm a bit",
                "concerned by those",
                "that worked as guides",
                "for us, however.  All",
                "seemed well enough",
                "until we revealed the",
                "immense stone doors",
                "of the tomb structure"),
            new BookPageInfo(
                "itself.  Seemed to send",
                "a shiver up their",
                "spines and get them all",
                "stirred up with",
                "whispering.  I'll",
                "watch the lot of them",
                "with a close eye, but",
                "I'm confident we won't"),
            new BookPageInfo(
                "have any real",
                "problems on the dig.",
                "I'm especially proud to",
                "see Thomas standing",
                "out - he was a good",
                "hire, despite the",
                "warnings from his",
                "previous employers."),
            new BookPageInfo(
                "He's drummed up the",
                "workers into a",
                "furious pace - we've",
                "nearly halved the",
                "estimate on the",
                "timeline for",
                "excavating the Tomb's",
                "entrance."));
        [Constructable]
        public GrimmochJournal1()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal1(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GrimmochJournal2 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day Two :",
                "",
                "We managed to dig out",
                "the last of the",
                "remaining rubble",
                "today, revealing the",
                "entirety of the giant",
                "stone doors that sealed"),
            new BookPageInfo(
                "ol' Khal Ankur and",
                "his folk up ages ago.",
                "Actually getting them",
                "open was another",
                "matter altogether,",
                "however.  As the",
                "workers set to the",
                "task with picks and"),
            new BookPageInfo(
                "crowbars, I could have",
                "sworn I saw Lysander",
                "Gathenwale fiddling",
                "with something in that",
                "musty old tome of his.",
                " I've no great",
                "knowledge of things",
                "magical, but the way"),
            new BookPageInfo(
                "his hand moved over",
                "that book, and the look",
                "of concentration on his",
                "face as he whispered",
                "something to himself",
                "looked like every",
                "description of an",
                "incantation I've ever"),
            new BookPageInfo(
                "heard.  The strange",
                "thing is, this set of",
                "doors that an entire",
                "crew of excavators",
                "was laboring over for",
                "hours, right when",
                "Gathenwale finishes",
                "with his mumbling..."),
            new BookPageInfo(
                "well, I swore the doors",
                "just gave open at the",
                "exact moment he",
                "spoke his last bit of",
                "whisper and shut the",
                "tome tight in his",
                "hands.  When he",
                "looked up, it was"),
            new BookPageInfo(
                "almost as if he was",
                "expecting the doors to",
                "be open, rather than",
                "shocked that they'd",
                "finally given way."));
        [Constructable]
        public GrimmochJournal2()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal2(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GrimmochJournal3 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day Three - Day Five:",
                "",
                "I might have",
                "written too hastily in",
                "my first entry - this",
                "place doesn't seem too",
                "bent on giving up any",
                "secrets.   Though the"),
            new BookPageInfo(
                "main antechamber is",
                "open to us, the main",
                "exit hall is blocked by",
                "yet another pile of",
                "rubble.  Doesn't look a",
                "bit like anything",
                "caused by a quake or",
                "instability in the"),
            new BookPageInfo(
                "stonework... I swear it",
                "looks as if someone",
                "actually piled the",
                "stones up themselves,",
                "some time after the",
                "tomb was built.  The",
                "stones aren't of the",
                "same set nor quality"),
            new BookPageInfo(
                "of the carved work",
                "that surrounds them",
                "- if anything, they",
                "resemble the grade of",
                "common rock we saw",
                "in great quantities on",
                "the trip here.  Which",
                "makes it feel all the"),
            new BookPageInfo(
                "more like someone",
                "hauled them in and",
                "deliberately covered",
                "this passage.  But then",
                "why not decorate them",
                "in the same ornate",
                "manner as the rest of",
                "the stone in this"),
            new BookPageInfo(
                "place?  Lysander",
                "wouldn't hear a word",
                "of what I had to say -",
                "to him, it was a quake",
                "some time in the",
                "history of the tomb,",
                "and that was it, shut",
                "up and move on.  So I"),
            new BookPageInfo(
                "shut up, and got back",
                "to work."));
        [Constructable]
        public GrimmochJournal3()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal3(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GrimmochJournal6 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day Six :",
                "",
                "The camp was",
                "attacked last night by",
                "a pack of, well, I don't",
                "have a clue.  I've never",
                "seen the like of these",
                "beasts anywhere."),
            new BookPageInfo(
                "Huge things, with",
                "fangs the size of your",
                "forefinger, covered in",
                "hair and with the",
                "strangest arched back",
                "I've ever seen.  And so",
                "many of them.  We",
                "were forced back into"),
            new BookPageInfo(
                "the Tomb for the",
                "night, just to keep our",
                "hides on us.  And",
                "today Gathenwale",
                "practically orders us",
                "all to move the entire",
                "exterior camp into the",
                "Tomb.  Now, I don't"),
            new BookPageInfo(
                "disagree that we'd be",
                "well off to use the",
                "place as a point of",
                "fortification... but I",
                "don't like it one bit, in",
                "any case.  I don't like",
                "the look of this place,",
                "nor the sound of it."),
            new BookPageInfo(
                "The way the wind",
                "gets into the",
                "passageways,",
                "whistling up the",
                "strangest noises.",
                "Deep, sustained echoes",
                "of the wind, not so",
                "much flute-like as..."),
            new BookPageInfo(
                "well, it sounds",
                "ridiculous.  In any",
                "case, we've set to work",
                "moving the bulk of the",
                "exterior camp into the",
                "main antechamber, so",
                "there's no use moaning",
                "about it now."));
        [Constructable]
        public GrimmochJournal6()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal6(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GrimmochJournal7 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day Seven - Day Ten:",
                "",
                "I cannot stand this",
                "place, I cannot bear it.",
                "I've got to get out.",
                "Something evil lurks",
                "in this ancient place,",
                "something best left"),
            new BookPageInfo(
                "alone.  I hear them,",
                "yet none of the others",
                "do.  And yet they",
                "must.  Hands, claws,",
                "scratching at stone,",
                "the awful scratching",
                "and the piteous cries",
                "that sound almost like"),
            new BookPageInfo(
                "laughter.  I can hear",
                "them above even the",
                "cracks of the",
                "workmen's picks, and",
                "at night they are all I",
                "can hear.  And yet the",
                "others hear nothing.",
                "We must leave this"),
            new BookPageInfo(
                "place, we must.",
                "Three workers have",
                "gone missing - Tavara",
                "expects they've",
                "abandoned us - and I",
                "count them lucky if",
                "they have.  I don't care",
                "what the others say,"),
            new BookPageInfo(
                "we must leave this",
                "place.  We must do as",
                "those before and pile",
                "up the stones, block all",
                "access to this primeval",
                "crypt, seal it up again",
                "for all eternity."));
        [Constructable]
        public GrimmochJournal7()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal7(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GrimmochJournal11 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day Eleven - Day",
                "Thirteen :",
                "",
                "Lysander is gone, and",
                "two more workers",
                "with him.  Good",
                "riddance to the first.",
                "He knows something."),
            new BookPageInfo(
                "He heard them too, I",
                "know he did - and yet",
                "he scowled at me",
                "when I mentioned",
                "them.  I cannot stop",
                "the noise in my head,",
                "the scratching, the",
                "clawing tears at my"),
            new BookPageInfo(
                "senses.  What is it?",
                "What does Lysander",
                "seek that I can only",
                "turn from?  Where",
                "has he gone?  The",
                "only answer to my",
                "questions comes as",
                "laughter from behind"),
            new BookPageInfo(
                "the stones."));
        [Constructable]
        public GrimmochJournal11()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal11(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GrimmochJournal14 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day Fourteen - Day",
                "Sixteen :",
                "",
                "We are lost... we are",
                "lost... all is lost.  The",
                "dead are piled up at",
                "my feet.  Bergen and I",
                "somehow managed in"),
            new BookPageInfo(
                "the madness to piece",
                "together a barricade,",
                "barring access to the",
                "camp antechamber.",
                "He knows as well as I",
                "that we cannot hold it",
                "forever.  The dead",
                "come.  They took"),
            new BookPageInfo(
                "Lysander before our",
                "eyes.  I pity the soul",
                "of even such a",
                "madman - no one",
                "should die in such a",
                "manner.  And yet so",
                "many have.  We're",
                "trapped here in this"),
            new BookPageInfo(
                "horror.  So many have",
                "died, and for what?",
                "What curse have we",
                "stumbled upon?  I",
                "cannot bear it, the",
                "moaning, wailing cries",
                "of the dead.  Poor",
                "Thomas, cut to pieces"),
            new BookPageInfo(
                "by their blades.  We",
                "had only an hour to",
                "properly bury those",
                "we could, before the",
                "undead legions struck",
                "again.  I cannot go on...",
                "I cannot go on."));
        [Constructable]
        public GrimmochJournal14()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal14(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GrimmochJournal17 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day Seventeen - Day",
                "Twenty-Two :",
                "",
                "The fighting never",
                "ceases... the blood",
                "never stops flowing,",
                "like a river through",
                "the bloated corpses of"),
            new BookPageInfo(
                "the dead.  And yet",
                "there are still more.",
                "Always more, with",
                "the red fire gleaming",
                "in their eyes.  My",
                "arm aches, I've taken",
                "to the sword as my",
                "bow seems to do little"),
            new BookPageInfo(
                "good... the dull ache in",
                "my arm... so many",
                "swings, cleaving a",
                "mountain of decaying",
                "flesh.  And Thomas...",
                "he was there, in the",
                "thick of it... Thomas",
                "was beside me..."),
            new BookPageInfo(
                "his face cleaved in",
                "twain - and yet beside",
                "me, fighting with us",
                "against the horde until",
                "he was cut down once",
                "again.  And I swear I",
                "see him even now,",
                "there in the dark"),
            new BookPageInfo(
                "corner of the",
                "antechamber, his eyes",
                "flickering in the last",
                "dying embers of the",
                "fire... and he stares at",
                "me, and a scream fills",
                "the vault - whether",
                "his or mine, I can no"),
            new BookPageInfo(
                "longer tell."));
        [Constructable]
        public GrimmochJournal17()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal17(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GrimmochJournal23 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The daily journal of Grimmoch Drummel", "Grimmoch",
            new BookPageInfo(
                "Day Twenty-Three :",
                "",
                "We no longer bury the",
                "dead."));
        [Constructable]
        public GrimmochJournal23()
            : base(Utility.Random(0xFF1, 2), false)
        {
        }

        public GrimmochJournal23(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}