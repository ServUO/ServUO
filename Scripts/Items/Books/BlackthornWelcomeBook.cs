namespace Server.Items
{
    public class BlackthornWelcomeBook : RedBook
    {
        public static readonly BookContent Content = new BookContent(
            "A Welcome", "Lord Blackthorn",
            new BookPageInfo(
                "  Greetings to you,",
                "new member of the",
                "Trusted.",
                "  You now read these",
                "words because you",
                "have been deemed",
                "worthy to join the",
                "ranks of"),
            new BookPageInfo(
                "Britannia's defenders.",
                "Some will call you a",
                "betrayer of mankind, I",
                "say they are",
                "misguided.  I call you",
                "a defender for Sosaria",
                "needs saving from",
                "itself."),
            new BookPageInfo(
                "  The forces of order",
                "once ruled our world.",
                "Like a great",
                "darkness over our",
                "lives we lived under",
                "the oppressive watch",
                "of a king who",
                "dictated our actions"),
            new BookPageInfo(
                "and passed judgement",
                "on our character.  He",
                "suppressed our way of",
                "life by denying our",
                "freedom and the",
                "ability to determine",
                "who we are and what",
                "we stand for.  Even"),
            new BookPageInfo(
                "today in the absense",
                "of this man we still",
                "see the symbol of his",
                "tyranny, we watch his",
                "personal guards patrol",
                "the cities to",
                "intimidate us, and we",
                "feel his laws like a"),
            new BookPageInfo(
                "vice on our lives.",
                "  You are here",
                "because you choose to",
                "be free.  Like many",
                "Britannians you have",
                "felt the opression of",
                "one man's ideas",
                "weighing down upon"),
            new BookPageInfo(
                "you like chains.  You",
                "have felt the embrace",
                "of fear, wondering if",
                "you face consequence",
                "for simply having",
                "ideas not in harmony",
                "with those forced upon",
                "you.  You have seen"),
            new BookPageInfo(
                "men fight and die for",
                "the principles of a",
                "zealot and wondered,",
                "'Who will fight for",
                "my principles should",
                "they be opposed?'",
                "You tire of living",
                "under the shadow of"),
            new BookPageInfo(
                "dreams that do not",
                "belong to you.  And",
                "most of all, you have",
                "wondered what you",
                "can do to live free.",
                "  Your journey to",
                "freedom begins here.",
                "  I, like you, once"),
            new BookPageInfo(
                "desired my freedom",
                "from the limits placed",
                "on me.  I watched in",
                "disgust as this world",
                "became engrossed with",
                "the preaching of",
                "virtue and none of the",
                "practice.  I held my"),
            new BookPageInfo(
                "convictions in check,",
                "fearful of the reaction",
                "of men blinded by",
                "belief.  I forced",
                "myself to bury the",
                "very idealogy that",
                "made me an individual.",
                "I was fortunate that"),
            new BookPageInfo(
                "a being of unique",
                "power and",
                "unimaginable",
                "intelligence saw",
                "through to the true",
                "person I was, the",
                "person I was meant to",
                "be.  Exodus found"),
            new BookPageInfo(
                "within me a man of",
                "free will,",
                "determination, and",
                "incomprehension for",
                "the plight of",
                "oppression forced on so",
                "many Britannians.",
                "  Exodus has also"),
            new BookPageInfo(
                "chosen you because of",
                "the strength of your",
                "character.",
                "  Many of the men",
                "who were once my",
                "peers look upon me",
                "and see a betrayer of",
                "humanity.  They"),
            new BookPageInfo(
                "claim my newfound",
                "form is unnatural and",
                "wrong.  Because they",
                "see the unknown in",
                "me, they show fear.",
                "They disapprove of",
                "my choices and in",
                "their ignorance see"),
            new BookPageInfo(
                "evil.  Yet I hide my",
                "true self no longer",
                "from these men.  My",
                "thoughts and my",
                "personal morality have",
                "been liberated in the",
                "face of the oppression",
                "that once consumed me."),
            new BookPageInfo(
                "Where they see a",
                "man no longer human.",
                "I see a man that has",
                "not betrayed his",
                "humanity but has been",
                "freed from it.  This",
                "is the power that has",
                "been granted to me by"),
            new BookPageInfo(
                "Exodus.  I have been",
                "given my freedom.  I",
                "have been released",
                "from my fears.",
                "  Exodus will give",
                "you the power to",
                "conquer your fears as",
                "well."),
            new BookPageInfo(
                "  When your fear",
                "of this world is gone,",
                "then the world truly",
                "belongs to you in a",
                "way it never has",
                "before.  You, trusted",
                "one, will soon be given",
                "a gift.  Your body,"),
            new BookPageInfo(
                "like mine, will be",
                "enlightened and raised",
                "to a level no mortal",
                "can know.  The power",
                "to control your own",
                "destiny will belong to",
                "you for the first time",
                "in your life."),
            new BookPageInfo(
                "  You will, at long",
                "last, be cleansed of",
                "fear.",
                "  Together, with the",
                "power of Exodus",
                "behind us, we shall",
                "finally wage war on",
                "the oppression that"),
            new BookPageInfo(
                "once held us back",
                "from our full",
                "potential.  We shall",
                "claim this world, and",
                "reshape it in an image",
                "of freedom for all of",
                "us.  Those who once",
                "told you who you are"),
            new BookPageInfo(
                "and how you should",
                "live will no longer be",
                "able to stand in the",
                "way of your free will.",
                "Many say you will",
                "be abandoning your",
                "humanity but in truth,",
                "you will be more than"),
            new BookPageInfo(
                "human.",
                "  You will be freed."));
        [Constructable]
        public BlackthornWelcomeBook()
            : base(false)
        {
            Hue = 0x89B;
        }

        public BlackthornWelcomeBook(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent => Content;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}