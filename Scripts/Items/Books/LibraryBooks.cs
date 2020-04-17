namespace Server.Items
{
    #region A Grammar of Orcish
    public class GrammarOfOrcish : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "A Grammar of Orcish", "Yorick of Yew",
            new BookPageInfo(
                "This volume, and",
                "others in the series,",
                "are sponsored by",
                "donations from Lord",
                "Blackthorn, ever a",
                "supporter of",
                "understanding the",
                "other sentient races"),
            new BookPageInfo(
                "of Britannia.",
                "-",
                "",
                "  The Orcish tongue",
                "may fall unpleasingly",
                "'pon the ear, yet it",
                "has within it a",
                "complex grammar oft"),
            new BookPageInfo(
                "misunderstood by",
                "those who merely",
                "hear the few broken",
                "words of English our",
                "orcish brothers",
                "manage without",
                "education.",
                "  These are the basic"),
            new BookPageInfo(
                "rules of orcish:",
                "  Orcish has five",
                "tenses: present, past,",
                "future imperfect,",
                "present interjectional,",
                "and prehensile.",
                "  Examples: gugroflu,",
                "gugrofloog, gugrobo,"),
            new BookPageInfo(
                "gugroglu!, gugrogug.",
                "  All transitive verbs",
                "in the prehensile",
                "tense end in \"ug.\"",
                "  Examples:",
                "urgleighug,",
                "biggugdaghgug,",
                "curdakalmug."),
            new BookPageInfo(
                "  All present",
                "interjectional",
                "conjugations start",
                "with the letter G",
                "unless the contain the",
                "third declensive",
                "accent of the letter U.",
                "  Examples:"),
            new BookPageInfo(
                "ghothudunglug, but not",
                "azhbuugub.",
                "  The past tense can",
                "only refer to events",
                "since the last meal,",
                "but the prehensile",
                "tense can refer to",
                "any event within"),
            new BookPageInfo(
                "reach.",
                "  The present tense",
                "is conjugated like the",
                "future imperfect",
                "tense, when the",
                "interrogative mode is",
                "used by pitching the",
                "sound a quarter-tone"),
            new BookPageInfo(
                "higher.",
                "Orcish hath no",
                "concept of person, as",
                "in first person, third",
                "person, I, we, etc.",
                "  Orcish grammar",
                "relies upon the three",
                "cardinal rules of"),
            new BookPageInfo(
                "accretion, prefixing,",
                "and agglutination, in",
                "addition to pitch. In",
                "the former, phonemes",
                "combine into larger",
                "words which may",
                "contain full phrasal",
                "significance. In the"),
            new BookPageInfo(
                "second, prefixing",
                "specific phonetic",
                "sounds changes the",
                "subject of the",
                "sentence into object,",
                "interrogative,",
                "addressed individual,",
                "or dinner."),
            new BookPageInfo(
                "  Agglutination occurs",
                "whenever four of the",
                "same letter are",
                "present in a word, in",
                "which case, any two",
                "of them may be",
                "removed or slurred.",
                "  Pitch changes the"),
            new BookPageInfo(
                "phoneme value of",
                "individual syllables,",
                "thus completely",
                "altering what a word",
                "may mean. The",
                "classic example is",
                "\"Aktgluthugrot",
                "bigglogubuu"),
            new BookPageInfo(
                "dargilgaglug lublublub\"",
                "which can mean \"You",
                "are such a pretty",
                "girl,\" \"My mother ate",
                "your primroses,\" or",
                "\"Jellyfish nose paints",
                "alms potato,\"",
                "depending on pitch."),
            new BookPageInfo(
                "  Orcish poetry often",
                "relies upon repeating",
                "the same phrase in",
                "multiple pitches, even",
                "changing pitch",
                "midword. None of",
                "this great art is",
                "translatable."),
            new BookPageInfo(
                "  The orcish language",
                "uses the following",
                "vowels: ab, ad, ag, akt,",
                "at, augh, auh, azh, e,",
                "i, o, oo, u, uu. The",
                "vowel sound a is not",
                "recognized as a vowel",
                "and does not exist in"),
            new BookPageInfo(
                "their alphabet.",
                "The orcish alphabet is",
                "best learned using the",
                "classic rhyme",
                "repeated at 23",
                "different pitches:",
                "   Lugnog ghu blat",
                "suggaroglug,"),
            new BookPageInfo(
                "Gaghbuu dakdar ab",
                "highugbo,",
                "  Gothnogbuim ad",
                "gilgubbugbuilug",
                "Bilgeaugh thurggulg",
                "stuiggro!",
                "",
                "A translation of the"),
            new BookPageInfo(
                "first pitch:",
                "Eat food, the first",
                "letter is ab,",
                "Kill people, next letter",
                "is ad,",
                "I forget the rest",
                "But augh is in there",
                "somewhere!"),
            new BookPageInfo(
                "",
                "  What follows is a",
                "complete phonetic",
                "library of the orcish",
                "language:",
                "ab, ad, ag, akt, alm,",
                "at, augh, auh, azh,",
                "ba, ba, bag, bar, baz,"),
            new BookPageInfo(
                "bid, bilge, bo, bog, bog,",
                "brui, bu, buad, bug,",
                "bug, buil, buim, bum,",
                "buo, buor, buu, ca,",
                "car, clog, cro, cuk,",
                "cur, da, dagh, dagh,",
                "dak, dar, deak, der,",
                "dil, dit, dor, dre, dri,"),
            new BookPageInfo(
                "dru, du, dud, duf,",
                "dug, dug, duh, dun,",
                "eag, eg, egg, eichel,",
                "ek, ep, ewk, faugh,",
                "fid, flu, fog, foo,",
                "foz, fruk, fu, fub,",
                "fud, fun, fup, fur,",
                "gaa, gag, gagh, gan,"),
            new BookPageInfo(
                "gar, gh, gha, ghat,",
                "ghed, ghig, gho, ghu,",
                "gig, gil, gka, glu, glu,",
                "glug, gna, gno, gnu,",
                "gol, gom, goth, grunt,",
                "grut, gu, gub, gub,",
                "gug, gug, gugh, guk,",
                "guk,"));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public GrammarOfOrcish()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public GrammarOfOrcish(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region A Politic Call to Anarchy
    public class CallToAnarchy : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "A Politic Call to Anarchy", "Lord Blackthorn",
            new BookPageInfo(
                "  Let it never be said",
                "that I have aught as",
                "quarrel with my liege",
                "Lord British, for",
                "indeed we be of the",
                "best of friends,",
                "sharing amicable",
                "games of chess 'pon a"),
            new BookPageInfo(
                "winter's night, and",
                "talking at length into",
                "the wee hours of the",
                "issues that affect the",
                "realm of Britannia.",
                "  Yet true friendship",
                "doth not prevent true",
                "philosophical"),
            new BookPageInfo(
                "disagreement either.",
                "While I view with",
                "approval my lord's",
                "affection for his",
                "carefully crafted",
                "philosophy of the",
                "Eight Virtues,",
                "wherein moral"),
            new BookPageInfo(
                "behavior is",
                "encouraged in the",
                "populace, I view with",
                "less approval the",
                "expenditure of public",
                "funds upon the",
                "construction of",
                "\"shrines\" to said"),
            new BookPageInfo(
                "ideals.",
                "  The issue is not one",
                "of funds, however,",
                "but a disagreement",
                "most intellectual over",
                "the proper way of",
                "humankind in an",
                "ethical sense. Surely"),
            new BookPageInfo(
                "freedom of decision",
                "must be regarded as",
                "paramount in any",
                "such moral decision?",
                "Though none fail to",
                "censure the",
                "murderer, a subtler",
                "question arises when"),
            new BookPageInfo(
                "we ask if his",
                "behavior would be",
                "ethical if he were",
                "forced to it.",
                "  I say to thee, the",
                "reader, quite flatly,",
                "that no ethical system",
                "shall have sway over"),
            new BookPageInfo(
                "me unless it",
                "convinceth me, for",
                "that freely made",
                "choice is to me the",
                "sign that the system",
                "hath validity.",
                "  Whereas the system",
                "of \"Virtues\" that my"),
            new BookPageInfo(
                "liege espouses is",
                "indeed a compilation",
                "of commonly approved",
                "virtues, I approve of",
                "it. Where it seeks to",
                "control the populace",
                "and restrict their",
                "diversity and their"),
            new BookPageInfo(
                "range of behaviors, I",
                "quarrel with it. And",
                "thus do I issue this",
                "politic call to anarchy,",
                "whilst humbly",
                "begging forgiveness",
                "of Lord British for",
                "my impertinence:"),
            new BookPageInfo(
                "  Celebrate thy",
                "differences. Take",
                "thy actions according",
                "to thy own lights.",
                "Question from what",
                "source a law, a rule,",
                "a judge, and a virtue",
                "may arise. 'Twere"),
            new BookPageInfo(
                "possible (though I",
                "suggest it not",
                "seriously) that a",
                "daemon planted the",
                "seed of these",
                "\"Virtues\" in my Lord",
                "British's mind; 'twere",
                "possible that the"),
            new BookPageInfo(
                "Shrines were but a",
                "plan to destroy this",
                "world. Thou canst not",
                "know unless thou",
                "questioneth, doubteth,",
                "and in the end,",
                "unless thou relyest",
                "upon THYSELF and"),
            new BookPageInfo(
                "thy judgement.",
                "  I offer these words",
                "as mere philosophical",
                "musings for those",
                "who seek",
                "enlightenment, for",
                "'tis the issue that",
                "hath occupied mine"),
            new BookPageInfo(
                "interest and that of",
                "Lord British for",
                "some time now."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public CallToAnarchy()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public CallToAnarchy(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region A Primer on Arms and Weapons
    public class ArmsAndWeaponsPrimer : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "A Primer on Arms and Weapons", "Martin",
            new BookPageInfo(
                "    These are the",
                "basic elements to",
                "consider in assessing",
                "a weapon, of which",
                "all warriors who",
                "regard themselves as",
                "more than mere",
                "mercenaries should be"),
            new BookPageInfo(
                "aware.",
                "    First and most",
                "obvious is the amount",
                "of damage that the",
                "weapon may do",
                "against unprotected",
                "flesh. While 'tis this",
                "which first attracts"),
            new BookPageInfo(
                "the attention of the",
                "novice, 'tis a deadly",
                "mistake to regard it",
                "as the sole value of a",
                "weapon. While it may",
                "prove devastating",
                "indeed as a means of",
                "causing damage, a"),
            new BookPageInfo(
                "weapon must also",
                "serve as stout shield",
                "when engaged in",
                "combat.",
                "    Hence the second",
                "issue to which to pay",
                "attention is the",
                "amount of protection"),
            new BookPageInfo(
                "that a weapon may",
                "offer. Pay close",
                "attention to the guard",
                "on it, if it be a blade,",
                "or the stoutness of",
                "its wood if it is a pole",
                "arm.",
                "    Oft related to this"),
            new BookPageInfo(
                "is the weight of the",
                "weapon, for a heavy",
                "weapon is more",
                "difficult to maneuver",
                "to block with, though",
                "it may do more",
                "damage to thy",
                "opponent."),
            new BookPageInfo(
                "    If a weapon is too",
                "heavy for the wielder",
                "to move it freely,",
                "they should choose",
                "another and not",
                "attempt to prove their",
                "prowess by the size",
                "of their sword."),
            new BookPageInfo(
                "    The reach of a",
                "weapon both increases",
                "its defensive ability,",
                "and renders it more",
                "useful in open spaces",
                "as it allows attack",
                "against the opponent",
                "without the need to"),
            new BookPageInfo(
                "close. But be aware of",
                "the limitations of thy",
                "weapon! For a",
                "weapon with great",
                "reach may be useless",
                "in close quarters, for",
                "lack of space to",
                "maneuver it. Should"),
            new BookPageInfo(
                "that dagger-wielding",
                "enemy close on thee",
                "and thy halberd, 'tis",
                "best to flee.",
                "    Lastly, a factor",
                "that must always be",
                "considered is the",
                "condition of the"),
            new BookPageInfo(
                "weapon. It might be a",
                "wondrous magical",
                "blade of surpassing",
                "sharpness and it may",
                "leap to block blows",
                "with a mind of its",
                "own. It also might be",
                "of such flimsy"),
            new BookPageInfo(
                "construction, or",
                "damaged to such an",
                "extent, that the first",
                "time it clangs against",
                "steel, 'twill  shatter",
                "into useless shards.",
                "    Seek ye a good",
                "blacksmith should thy"),
            new BookPageInfo(
                "weapon become",
                "damaged, but be",
                "aware that their",
                "ministrations may",
                "simply make the",
                "matter worse.",
                "    While mages of",
                "some ability oft create"),
            new BookPageInfo(
                "magical weapons",
                "which enhance skill,",
                "are preternaturally",
                "sharp, or incinerate",
                "the enemy as they",
                "fall, to my mind the",
                "greatest gift that they",
                "can grant a stout"),
            new BookPageInfo(
                "sword is to make it",
                "resistant to damage,",
                "for thy own skill can",
                "make up the",
                "difference. Except",
                "for the fireball, but",
                "if the corpse is",
                "charred, then so will"),
            new BookPageInfo(
                "be the possessions,",
                "which maketh looting",
                "difficult!"));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public ArmsAndWeaponsPrimer()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public ArmsAndWeaponsPrimer(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region A Song of Samlethe
    public class SongOfSamlethe : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "A Song of Samlethe", "Sandra",
            new BookPageInfo(
                "The first bear did",
                "swim by day,",
                "And it did sleep by",
                "night.",
                "It kept itself within",
                "its cave",
                "and ate by starry",
                "light."),
            new BookPageInfo(
                "",
                "The second bear it did",
                "cavort",
                "'Neath canopies of",
                "trees,",
                "And danced its",
                "strange bearish sort",
                "Of joy for all to see."),
            new BookPageInfo(
                "",
                "The first bear, well,",
                "'twas hunted,",
                "And today adorns a",
                "floor.",
                "Its ruggish face has",
                "been dented",
                "By footfalls and the"),
            new BookPageInfo(
                "door.",
                "",
                "The second bear did",
                "step once",
                "Into a mushroom ring,",
                "And now does dance",
                "the dunce",
                "For wisps and"),
            new BookPageInfo(
                "unseen things.",
                "",
                "So do not dance, and",
                "do not sleep,",
                "Or else be led astray!",
                "For bears all end up",
                "six feet deep",
                "At the end of"),
            new BookPageInfo(
                "Samlethe's day."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public SongOfSamlethe()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public SongOfSamlethe(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region A Tale of Three Tribes
    public class TaleOfThreeTribes : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "A Tale of Three Tribes", "Janet, Scribe",
            new BookPageInfo(
                "  The dungeon known",
                "as Despise is in fact",
                "not a dungeon as",
                "such, but rather a",
                "large natural cave.",
                "Inhospitable and",
                "unfriendly to",
                "visitors, it is filled"),
            new BookPageInfo(
                "with damp spots",
                "where the deadly",
                "Exploding Red Spotted",
                "Toadstool grows in",
                "abundance.",
                "  According to the",
                "oldest of historical",
                "texts, in days gone"),
            new BookPageInfo(
                "by the cave was once",
                "the home of three",
                "separate tribes who",
                "had come to an",
                "accommodation with",
                "each other. Oddly",
                "enough, the three",
                "tribes were of"),
            new BookPageInfo(
                "dragons, lizard men,",
                "and rat men. While",
                "today few except",
                "extremists associated",
                "with Lord Blackthorn",
                "regard these latter",
                "two as being",
                "intelligent beings,"),
            new BookPageInfo(
                "apparently they have",
                "indeed fallen from a",
                "more evolved state",
                "over the years.",
                "  'Tis said that these",
                "three races did dwell",
                "in relative harmony",
                "within the vast cave,"),
            new BookPageInfo(
                "building when they",
                "required it, and",
                "trading amongst",
                "themselves if needed.",
                "  But over time,",
                "something happened,",
                "and they were forced",
                "to withdraw from"),
            new BookPageInfo(
                "their society, until",
                "today thou mayst",
                "find individuals of",
                "each species within",
                "the dungeon, but",
                "never again as a",
                "civilization.",
                "  'Tis also said that"),
            new BookPageInfo(
                "someday the three",
                "tribes may return to",
                "Despise, to once again",
                "inhabit it together.",
                "  Until then, nothing",
                "remains as token of",
                "this save an oddly",
                "intelligent skeleton,"),
            new BookPageInfo(
                "magically enchanted,",
                "that doth speak when",
                "questions are asked,",
                "and from whom I",
                "obtained these tales",
                "one day, when I was",
                "pursued by evil",
                "monsters and fled"),
            new BookPageInfo(
                "into his skeletal arms.",
                "  Fortunately, I",
                "escaped and lived to",
                "write it all down!"));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public TaleOfThreeTribes()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public TaleOfThreeTribes(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Beltran's Guide to Guilds
    public class GuideToGuilds : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Beltran's Guide to Guilds", "Beltran",
            new BookPageInfo(
                "  This reference",
                "work is intended",
                "merely to serve as",
                "resource for those",
                "curious as to the full",
                "range of trades and",
                "societies extant in",
                "Britannia and nearby"),
            new BookPageInfo(
                "nations. For each",
                "trade or guild, their",
                "blazon is given.",
                "",
                "  Armourer's Guild.",
                "Gold bar above black",
                "bar."),
            new BookPageInfo(
                "  Association of",
                "Warriors. Blue cross",
                "on a red field.",
                "",
                "  Barters' Guild.",
                "Green and white",
                "stripes, diagonal."),
            new BookPageInfo(
                "  Blacksmith's Guild.",
                "Gold alongside black.",
                "",
                "  Federation of",
                "Rogues and Beggars.",
                "Red above black.",
                "",
                "  Fighters and"),
            new BookPageInfo(
                "Footmen. Blue",
                "horzontal bar on red",
                "field.",
                "",
                "  Guild of Archers.",
                "A gold swath parting",
                "red and blue."),
            new BookPageInfo(
                "  Guild of",
                "Armaments. Swath of",
                "gold on black field,",
                "gold accents.",
                "",
                "  Guild of Assassins.",
                "Black and red",
                "quartered."),
            new BookPageInfo(
                "",
                "  Guild of Barbers.",
                "Red and white",
                "stripes.",
                "",
                "  Guild of Cavalry and",
                "Horse. Vertical blue",
                "on a red field."),
            new BookPageInfo(
                "",
                "  Guild of",
                "Fishermen. Blue and",
                "white, quartered.",
                "",
                "  Guild of Mages.",
                "Purple and blue, in a",
                "crossed pennant"),
            new BookPageInfo(
                "pattern.",
                "",
                "  Guild of",
                "Provisioners. White",
                "bar above green bar.",
                "",
                "  Guild of Sorcery. A",
                "field divided"),
            new BookPageInfo(
                "diagonally in blue and",
                "purple.",
                "",
                "  Healers Guild. Gold",
                "swath dividing green",
                "from purple, gold",
                "accents."),
            new BookPageInfo(
                "  Lord British's",
                "Healers of Virtue.",
                "Golden ankh on dark",
                "green.",
                "",
                "  Masters of Illusion.",
                "Blue and purple",
                "checkers."),
            new BookPageInfo(
                "",
                "  Merchants' Guild.",
                "Gold coins on green",
                "field.",
                "",
                "  Mining Cooperative.",
                "A gold cross,",
                "quartering blue and"),
            new BookPageInfo(
                "black.",
                "",
                "  Order of Engineers.",
                "Purple, gold, and blue",
                "vertical.",
                "",
                "  Sailors' Maritime",
                "Association. A white"),
            new BookPageInfo(
                "bar centered on a blue",
                "field.",
                "",
                "  Seamen's Chapter.",
                "Blue and white in a",
                "crossed pennant",
                "pattern."),
            new BookPageInfo(
                "  Society of Cooks and",
                "Chefs. White and red",
                "diagonal fields",
                "checker on green",
                "field.",
                "",
                "  Society of",
                "Shipwrights. White"),
            new BookPageInfo(
                "diagonal above blue.",
                "",
                "  Society of Thieves.",
                "Black and red diagonal",
                "stripes.",
                "",
                "  Society of",
                "Weaponsmakers. Gold"),
            new BookPageInfo(
                "diagonal above black.",
                "",
                "  Tailor's Hall. Purple",
                "above gold above red.",
                "",
                "  The Bardic",
                "Collegium. Purple and",
                "red checkers on gold"),
            new BookPageInfo(
                "field.",
                "",
                "  Traders' Guild.",
                "White bar centered",
                "down green field."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public GuideToGuilds()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public GuideToGuilds(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Birds of Britannia
    public class BirdsOfBritannia : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Birds of Britannia", "Thom the Heathen",
            new BookPageInfo(
                "   The WREN is a",
                "tiny insect-eating",
                "bird with a loud voice.",
                " The cheerful trills",
                "of Wrens are",
                "extraordinarily",
                "varied and melodious.",
                "    The SWALLOW"),
            new BookPageInfo(
                "is easily recognized",
                "by its forked tail.",
                "Swallows catch",
                "insects in flight, and",
                "have squeaky,",
                "twittering songs.",
                "    The WARBLER is",
                "an exceptional singer,"),
            new BookPageInfo(
                "whose extensive",
                "songs combine the",
                "best qualities of",
                "Wrens and Swallows.",
                "    The NUTHATCH",
                "climbs down trees",
                "head first, searching",
                "for insects in the"),
            new BookPageInfo(
                "bark.  It sings a",
                "repetitive series of",
                "notes with a nasal",
                "tone quality.",
                "    The agile",
                "CHICKADEE has a",
                "buzzy",
                "\"chick-a-dee-dee\""),
            new BookPageInfo(
                "call, from which its",
                "name is derived.  Its",
                "song is a series of",
                "whistled notes.",
                "    The THRUSH is a",
                "brown bird with a",
                "spotted breast, which",
                "eats worms and"),
            new BookPageInfo(
                "snails, and has a",
                "beautiful singing",
                "voice.  Thrushes use",
                "a stone as an anvil to",
                "smash the shells of",
                "snails.",
                "    The little",
                "NIGHTINGALE is"),
            new BookPageInfo(
                "also known for its",
                "beautiful song, which",
                "it sings even at night.",
                "    The STARLING",
                "is a small dark bird",
                "with a yellow bill and",
                "a squeaky,",
                "high-pitched song."),
            new BookPageInfo(
                "Starlings can mimic",
                "the sounds of other",
                "birds.",
                "    The SKYLARK",
                "sings a series of",
                "high-pitched",
                "melodious trills in",
                "flight."),
            new BookPageInfo(
                "    The FINCH is a",
                "small seed-eating bird",
                "with a conical beak",
                "and a musical,",
                "warbling song.",
                "    The CROSSBILL",
                "is a kind of Finch",
                "with a strange"),
            new BookPageInfo(
                "crossed bill, which it",
                "uses to extract seeds",
                "from pine cones.",
                "    The CANARY is a",
                "kind of Finch that is",
                "often kept as a pet.",
                "Miners would often",
                "take Canaries"),
            new BookPageInfo(
                "underground with",
                "them, to warn them",
                "of the presence of",
                "hazardous vapors in",
                "the air.",
                "    The SPARROW",
                "weaves a nest of",
                "grass, and has an"),
            new BookPageInfo(
                "unmusical chirp for a",
                "voice.",
                "    The TOWHEE is a",
                "kind of Sparrow that",
                "continually reminds",
                "listeners to drink",
                "their tea.",
                "    The SHRIKE is a"),
            new BookPageInfo(
                "gray bird with a",
                "hooked bill.  Shrikes",
                "have the habit of",
                "impaling their prey",
                "on thorns.",
                "    The",
                "WOODPECKER has a",
                "pointed beak that is"),
            new BookPageInfo(
                "suitable for pecking at",
                "wood to get at the",
                "insects inside.",
                "    The",
                "KINGFISHER dives",
                "for fish, which it",
                "catches with its long,",
                "pointed beak."),
            new BookPageInfo(
                "    The TERN",
                "migrates over great",
                "distances, from one",
                "end of Britannia to",
                "the other each year.",
                "Terns dive from the",
                "air to catch fish.",
                "    The PLOVER is a"),
            new BookPageInfo(
                "bird that distracts",
                "predators by",
                "pretending to have a",
                "broken wing.",
                "    The LAPWING is",
                "a kind of Plover that",
                "has a long black crest.",
                "    The HAWK is a"),
            new BookPageInfo(
                "predator that feeds on",
                "small birds, mice,",
                "squirrels, and other",
                "small animals.  Small",
                "hawks are known as",
                "Kites.",
                "    The DOVE is a",
                "seed-eating bird with"),
            new BookPageInfo(
                "a peaceful reputation.",
                " Doves have a",
                "low-pitched cooing",
                "song.",
                "    The PARROT is a",
                "brightly colored bird",
                "with a hooked bill,",
                "favored as a"),
            new BookPageInfo(
                "companion by pirates.",
                " Parrots can be",
                "taught to imitate the",
                "human voice.",
                "    The CUCKOO is a",
                "devious bird that lays",
                "eggs in the nests of",
                "Warblers and other"),
            new BookPageInfo(
                "small birds.  Cuckoos",
                "have the uncanny",
                "ability to keep track",
                "of time, singing once",
                "at the beginning of",
                "each hour.",
                "    The",
                "ROADRUNNER is"),
            new BookPageInfo(
                "an unusual bird with",
                "a long tail, which",
                "runs swiftly along",
                "the ground hunting",
                "for lizards and",
                "snakes.",
                "    The SWIFT is a",
                "very agile bird that"),
            new BookPageInfo(
                "spends nearly its",
                "entire life in the air.",
                "With their mouths",
                "wide open, Swifts",
                "capture insects in",
                "mid-flight.",
                "    The",
                "HUMMINGBIRD is a"),
            new BookPageInfo(
                "cross between a",
                "Swift and a Fairy.",
                "These tiny, brightly",
                "colored birds hover",
                "magically near",
                "flowers, and live on",
                "the nectar they",
                "provide."),
            new BookPageInfo(
                "    The OWL is a",
                "reputedly wise bird",
                "that is active at night,",
                "unlike most birds.",
                "Owls have excellent",
                "night vision and",
                "low-pitched hooting",
                "calls.  Their wings"),
            new BookPageInfo(
                "are silent in flight.",
                "    The",
                "GOATSUCKER is a",
                "strange owl-like bird",
                "that is thought to live",
                "on the milk of goats.",
                "These mysterious",
                "birds make jarring"),
            new BookPageInfo(
                "sounds at night, for",
                "which reason they",
                "are also called",
                "Nightjars.",
                "    The DUCK is a",
                "bird that swims more",
                "often than it flies,",
                "and has a nasal voice"),
            new BookPageInfo(
                "that is described as a",
                "\"quack\".",
                "    The SWAN is a",
                "kind of long-necked",
                "Duck that is all white.",
                " Swans are usually",
                "voiceless, but they",
                "are said to have an"),
            new BookPageInfo(
                "extraordinarily",
                "beautiful song."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public BirdsOfBritannia()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public BirdsOfBritannia(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Britannian Flora: A Casual Guide
    public class BritannianFlora : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Britannian Flora: A Casual Guide", "Herbert the Lost",
            new BookPageInfo(
                "  Oft 'pon rambling",
                "through the woods",
                "avoiding bears have I",
                "spotted some plant",
                "whose like I have",
                "never seen before,",
                "and concluded that I",
                "was a blithering idiot"),
            new BookPageInfo(
                "for failing to notice it",
                "in the past. Equally",
                "as oft have I",
                "concluded that I was a",
                "worse idiot for not",
                "running faster from",
                "the bear.",
                "  While not all my"),
            new BookPageInfo(
                "readers may share",
                "my proclivities for",
                "tree-climbing, it",
                "occurred to me that",
                "mayhap mine",
                "information might",
                "serve some humble",
                "purpose."),
            new BookPageInfo(
                "  The two most",
                "unique flowering",
                "plants in the",
                "Britannian",
                "countryside are the",
                "orfleur and the",
                "whiteflower, also",
                "called white horns."),
            new BookPageInfo(
                "  The orfleur is",
                "notable for its",
                "massive orange-red",
                "blossoms, which",
                "dwarf marigolds like",
                "the sun dwarfs your",
                "common fireball spell.",
                "The odor of said"),
            new BookPageInfo(
                "blooms is best",
                "described as",
                "peppermint-apple,",
                "with a dash of garlic.",
                "'Tis a popular potted",
                "plant despite, or",
                "perhaps because of,",
                "its exotic nature."),
            new BookPageInfo(
                "  Whiteflowers exude",
                "a subtle fragrance not",
                "unlike that of freshly",
                "shaven wood mixed",
                "with cool lemon ice.",
                "Their tall stands",
                "always droop with the",
                "heavy weight of the"),
            new BookPageInfo(
                "massive blooms, oft",
                "as large as a child's",
                "head.",
                "  The flowers are so",
                "large that one may",
                "scoop out the pollen in",
                "handfuls, and during",
                "the spring season"),
            new BookPageInfo(
                "many a prank hath",
                "been played by idle",
                "boys 'pon their",
                "sisters by dumping",
                "said pollen into their",
                "clothing drawers,",
                "causing sneezes for",
                "days."),
            new BookPageInfo(
                "  The most",
                "interesting native tree",
                "to Britannia is the",
                "spider tree. The",
                "reason for its naming",
                "is obscure, but may",
                "have to do with the",
                "twisted gray stalks"),
            new BookPageInfo(
                "from which the",
                "spherical canopy",
                "sprouts. 'Tis",
                "something of a",
                "misnomer to term",
                "these \"trunks\" as",
                "they are spindly and",
                "flexible. Spider trees"),
            new BookPageInfo(
                "provide a fresh,",
                "piney smell to a room",
                "and are therefore",
                "often potted.",
                "  In jungle climes,",
                "one finds the blade",
                "plant, whose sharp",
                "leaves oft collect"),
            new BookPageInfo(
                "water for the thirsty",
                "traveler, yet can",
                "draw blood easily.",
                "  The deadliest plant,",
                "if you can call a",
                "fungus such, is the",
                "Exploding Red Spotted",
                "Toadstool. No pattern"),
            new BookPageInfo(
                "can be discerned to",
                "its habitats save",
                "malice, for merely",
                "approaching results in",
                "the cap exploding",
                "with powder, noxious",
                "gas, and tiny painful",
                "pellets flying in all"),
            new BookPageInfo(
                "directions.",
                "Unfortunately, 'tis",
                "impossible to tell it",
                "apart from the",
                "Ordinary Red Spotted",
                "Toadstool save through",
                "experimentation.",
                "  Truly odd among the"),
            new BookPageInfo(
                "varied flora of",
                "Britannia, however,",
                "are those which bear",
                "names clearly alien to",
                "our tongue. Among",
                "these I name the",
                "Tuscany pine (for I",
                "have never seen a"),
            new BookPageInfo(
                "region of this world",
                "named Tuscany), the",
                "o'hii tree, whose very",
                "name sounds like",
                "some tropical isle, and",
                "the welsh poppy,",
                "which while",
                "different from the"),
            new BookPageInfo(
                "ordinary poppy in",
                "color and appearance,",
                "is prefaced with the",
                "odd word \"welsh,\"",
                "which as far as I",
                "know means to forgo",
                "paying a debt."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public BritannianFlora()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public BritannianFlora(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Classic Children's Tales, Volume 2
    public class ChildrenTalesVol2 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Classic Children's Tales, Volume 2", "Guilhem, Editor",
            new BookPageInfo(
                "Clarke's Printery",
                "is Honored to",
                "Present Tales from",
                "Ages Past!",
                "    Guilhem the",
                "Scholar Shall End",
                "EachVolume with",
                "Staid Commentary."),
            new BookPageInfo(
                "",
                "THE RHYME",
                "Dance in the Star",
                "Chamber",
                "And Dance in the Pit",
                "And Eat of your",
                "Entrees",
                "In the Glass House"),
            new BookPageInfo(
                "you Sit",
                "",
                "COMMENTARY",
                "    A common feeding",
                "rhyme for little",
                "babies, 'tis thought",
                "that this little ditty is",
                "part of the corpus of"),
            new BookPageInfo(
                "legendary tales",
                "regarding the world",
                "before Sosaria (see",
                "the wonderful fables",
                "of Fabio the Poor for",
                "fictionalized versions",
                "of these stories, also",
                "available from this"),
            new BookPageInfo(
                "same publisher).",
                "    According to these",
                "old tales, which",
                "survive mostly in the",
                "hills and remote",
                "villages where Lord",
                "British is as yet a",
                "distant and mythical"),
            new BookPageInfo(
                "ruler, the gods of old",
                "(a fanciful notion!)",
                "met to discuss the",
                "progress of creating",
                "the world in mystical",
                "rooms. A simple",
                "analysis reveals these",
                "rooms to be mere"),
            new BookPageInfo(
                "mythological",
                "generalizations.",
                "    \"The Star",
                "Chamber\" is clearly a",
                "reference to the sky.",
                "\"The Pit\" is certainly",
                "an Underworld",
                "analogous to the"),
            new BookPageInfo(
                "Snakehills of other",
                "tales, and \"the Glass",
                "House\" is no doubt the",
                "vantage point from",
                "which the gods",
                "observed their",
                "creation. All is simple",
                "when seen from this"),
            new BookPageInfo(
                "perspective, leaving",
                "only the mysterious",
                "reference to dinners.",
                "Oddly enough, the",
                "rhyme is universally",
                "used only for",
                "midnight feedings,",
                "never during the day."),
            new BookPageInfo());

        public override BookContent DefaultContent => Content;

        [Constructable]
        public ChildrenTalesVol2()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public ChildrenTalesVol2(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Classic Tales of Vesper, Volume 1
    public class TalesOfVesperVol1 : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Classic Tales of Vesper, Volume 1", "Clarke's Printery",
            new BookPageInfo(
                "'Tis an Honor to",
                "present to Thee these",
                "Tales collected from",
                "Ages Past. In this",
                "Inaugural Volume, we",
                "present this Verse",
                "oft Recited as a",
                "Lullabye for sleepy"),
            new BookPageInfo(
                "Children.",
                "",
                "Preface",
                "by Guilhem the",
                "Scholar",
                "",
                "  The meaning of this",
                "verse has oft been"),
            new BookPageInfo(
                "discussed in halls of",
                "scholarly sorts, for",
                "its mysterious",
                "singsongy melody is",
                "oddly disturbing to",
                "adult ears, though",
                "children seem to find",
                "it restful as they"),
            new BookPageInfo(
                "sleep. Perhaps it is",
                "but the remnant of a",
                "longer ballad once",
                "extant, for there are",
                "internal indications",
                "that it once told a",
                "longer story about",
                "ill-fated lovers, and a"),
            new BookPageInfo(
                "magical experiment",
                "gone awry. However,",
                "poetic license and the",
                "folk process has",
                "distorted the words",
                "until now the locale of",
                "the tale is no more",
                "than \"in the wind,\""),
            new BookPageInfo(
                "which while it serves",
                "a pleasingly",
                "metaphorical purpose,",
                "fails to inform the",
                "listener as to any real",
                "locale!",
                "  Another possibility",
                "is that this is some"),
            new BookPageInfo(
                "form of creation",
                "myth explaining the",
                "genesis of the various",
                "humanoid creatures",
                "that roam the lands of",
                "Britannia. It does not",
                "take a stretch of the",
                "imagination to name"),
            new BookPageInfo(
                "the middle verse's",
                "\"girl becomes tree\" as",
                "a possible explanation",
                "for the reaper, for in",
                "the area surrounding",
                "Minoc, reapers are",
                "oft referred to among",
                "the lumberjacking"),
            new BookPageInfo(
                "community as",
                "\"widowmakers.\" That",
                "these creatures are",
                "of arcane origin is",
                "assumed, but the",
                "verse seems to imply",
                "a long ago creator, and",
                "uses the antique"),
            new BookPageInfo(
                "magickal terminology",
                "of \"plaiting strands",
                "of ether\" that is so",
                "often found in",
                "ancient texts. In",
                "addition, the",
                "reference to",
                "\"snakehills\" may"),
            new BookPageInfo(
                "profitably be regarded",
                "as a reference to an",
                "actual location, such",
                "as perhaps a local",
                "term for the",
                "Serpent's Spine.",
                "  A commoner",
                "interpretation is that"),
            new BookPageInfo(
                "like many nursery",
                "rhymes, it is a",
                "simple explanation",
                "for death, wherein",
                "the wind snatches up",
                "boys and girls and",
                "when they sleep in",
                "order to keep the"),
            new BookPageInfo(
                "balance of the world.",
                "Notable tales have",
                "been written for",
                "children of",
                "adventures in \"the",
                "Snakehills,\" which",
                "are presumed to be an",
                "Afterworld whence"),
            new BookPageInfo(
                "the spirit lives on. A",
                "grim lullabye, to be",
                "sure, but no worse",
                "than \"lest I die before",
                "I wake\" surely.",
                "  In either case, 'tis",
                "an old favorite,",
                "herein printed for"),
            new BookPageInfo(
                "the first time for",
                "thy enjoyment and",
                "perusal!",
                "",
                "In the Wind where",
                "the Balance",
                "Is Whispered in",
                "Hallways"),
            new BookPageInfo(
                "In the Wind where",
                "the Magic",
                "Flows All through the",
                "Night",
                "There live Mages and",
                "Mages",
                "With Robes made of",
                "Whole Days"),
            new BookPageInfo(
                "Reading Books full of",
                "Doings",
                "Printed on Light",
                "",
                "In the Wind where",
                "the Lovers",
                "Are Crossed under",
                "Shadows"),
            new BookPageInfo(
                "Where they Meet and",
                "are Parted",
                "By the Orders of",
                "Fate",
                "The Girl becomes",
                "Tree,",
                "And thus becomes",
                "Widow"),
            new BookPageInfo(
                "The Boy becomes",
                "Earth",
                "And Wanders Till",
                "Late",
                "",
                "In the Wind are the",
                "Monsters",
                "First Born First"),
            new BookPageInfo(
                "Created",
                "When Chanting and",
                "Ether",
                "Mix Meddling and",
                "Nigh",
                "Fear going to Wind,",
                "Fear Finding its",
                "Plaitings,"),
            new BookPageInfo(
                "Go Not to the",
                "Snakehills",
                "Lest You Care to Die"));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public TalesOfVesperVol1()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public TalesOfVesperVol1(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Deceit: A Dungeon of Horrors
    public class DeceitDungeonOfHorror : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Deceit: A Dungeon of Horrors", "Mercenary Justin",
            new BookPageInfo(
                "  My employers have",
                "oft taken me into this",
                "den of hideous",
                "creatures, and I",
                "thought that it",
                "behooved me to write",
                "down what I know of",
                "it, now that I am"),
            new BookPageInfo(
                "retired from the life",
                "of an adventurer for",
                "hire.",
                "  Deceit was once a",
                "temple to forgotten",
                "powers of old. It was",
                "taken over by mages",
                "who eventually were"),
            new BookPageInfo(
                "driven out by the",
                "depredations of their",
                "own evil lackeys.",
                "However, many of",
                "the magical traps and",
                "devices that they",
                "placed for their",
                "defenses remain,"),
            new BookPageInfo(
                "particularly those the",
                "wizards used to",
                "protect their",
                "treasures.",
                "  The dungeon is",
                "mystically linked by",
                "crystal balls placed in",
                "different locations."),
            new BookPageInfo(
                "These magical orbs do",
                "transmit speech, and",
                "even have memory of",
                "things that have been",
                "said near them. No",
                "doubt they once",
                "served as a warning",
                "system"),
            new BookPageInfo(
                "  Be wary of a",
                "brazier that giveth",
                "warning when",
                "approached; thou canst",
                "use it to summon",
                "deadly creatures.",
                "  There be a",
                "tantalizing chest,"),
            new BookPageInfo(
                "undoubtedly full of",
                "treasure, that cannot",
                "be reached save past a",
                "complex set of",
                "pressure plates that",
                "trigger deadly spikes.",
                "As I never had",
                "sufficient folk with"),
            new BookPageInfo(
                "me to unlock the",
                "puzzle, I never",
                "obtained the riches",
                "that awaited there.",
                "  Do not investigate",
                "iron maidens too",
                "closely, for they may",
                "suck you within"),
            new BookPageInfo(
                "them!",
                "  There is one place",
                "where a deadly trap",
                "can only be disarmed",
                "by making use of a",
                "statue that cleverly",
                "conceals a lever.",
                "  Oft one encounters"),
            new BookPageInfo(
                "the deadly exploding",
                "toadstool; the ones in",
                "Deceit are deadlier",
                "than most, as they",
                "explode continually.",
                "Likewise, the very",
                "pools of water and",
                "slime on the floor"),
            new BookPageInfo(
                "may poison thee.",
                "  The most magical",
                "device in the dungeon",
                "is a mystical bridge",
                "that can only be",
                "triggered by a level",
                "embedded in the floor.",
                "Be wary however,"),
            new BookPageInfo(
                "for the bridge thus",
                "created doth burst",
                "into flame when one",
                "passeth across it!"));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public DeceitDungeonOfHorror()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public DeceitDungeonOfHorror(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Dimensional Travel, a Monograph
    public class DimensionalTravel : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Dimensional Travel, a Monograph", "Dryus Doost, Mage",
            new BookPageInfo(
                "  'Tis beyond the",
                "scope of this small",
                "monograph to discuss",
                "the details of",
                "moongates, and the",
                "manners in which",
                "they distort the",
                "fabric of reality in"),
            new BookPageInfo(
                "such a manner as to",
                "permit the passage of",
                "living flesh from",
                "place to place, world to",
                "world, or indeed from",
                "dimension to",
                "dimension.",
                "  Instead, allow me to"),
            new BookPageInfo(
                "bring thy attention,",
                "Gentle Reader, to the",
                "curious",
                "characteristics that",
                "are shared by certain",
                "individuals within",
                "our realm.",
                "  Long has it been"),
            new BookPageInfo(
                "known that the blue",
                "moongate permits",
                "travel from place to",
                "place, and none have",
                "trouble in taking this",
                "path. Yet 'tis also",
                "known, albeit only to a",
                "few, that certain"),
            new BookPageInfo(
                "individuals are unable",
                "to traverse the black",
                "moongates that permit",
                "travel from one",
                "dimension to another.",
                "  The noted mage and",
                "peer of our realm,",
                "Lord Blackthorn, once"),
            new BookPageInfo(
                "told me in",
                "conversation that his",
                "arcane research had",
                "indicated that the",
                "issue was one of",
                "conversation of ether.",
                "To wit, given the",
                "postulate that matter"),
            new BookPageInfo(
                "within a given",
                "dimension may be but",
                "a cross-section of",
                "ethereal matter that",
                "exists in multiple",
                "dimensions, it",
                "becomes obvious that",
                "said ethereal"),
            new BookPageInfo(
                "structure cannot",
                "enter dimensions in",
                "which it is already",
                "present.",
                "  Imagine an",
                "individual (and the",
                "Lord Blackthorn",
                "hinted that he was"),
            new BookPageInfo(
                "one such) who exists",
                "already in some form",
                "in multiple",
                "dimensions; said",
                "individual would not",
                "be able to cross into",
                "another dimension",
                "because HE IS"),
            new BookPageInfo(
                "ALREADY THERE.",
                "  The implications of",
                "this are staggering,",
                "and merit further",
                "study. 'Tis well",
                "known by theorists in",
                "the field that",
                "divisions in the"),
            new BookPageInfo(
                "ethereal structure of",
                "an individual are",
                "already implicit at the",
                "temporal level, as",
                "causality forces",
                "divisions upon the",
                "ether. This is the",
                "basic operating"),
            new BookPageInfo(
                "mechanism by which",
                "white moongates",
                "function, permitting",
                "time travel.",
                "  As time travel is",
                "not barred by the",
                "presence of an earlier",
                "self (though"),
            new BookPageInfo(
                "encountering said",
                "earlier self can prove",
                "arcanely perilous),",
                "there must be some",
                "rigidity to the",
                "ethereal structure",
                "that bars multiple",
                "instantiations of"),
            new BookPageInfo(
                "structures from",
                "manifesting within",
                "the same context.",
                "  If one regards time",
                "and causal bifurcation",
                "as a web, perhaps the",
                "appropriate analogy",
                "for dimensional"),
            new BookPageInfo(
                "matrices is that of a",
                "crystalline structure,",
                "with rigid linkages.",
                "The only way in",
                "which an individual",
                "such as Lord",
                "Blackthorn, who",
                "exists in multiple"),
            new BookPageInfo(
                "dimensional matrices,",
                "can cross worlds via",
                "a black moongate,",
                "would be for the",
                "entire crystalline",
                "structure of the",
                "dimension to",
                "perfectly match the"),
            new BookPageInfo(
                "ethereal resonance of",
                "the destination",
                "dimension.",
                "  The problem of why",
                "certain individuals",
                "are already replicated",
                "in multiple crystalline",
                "matrices is one that I"),
            new BookPageInfo(
                "fail to provide any",
                "schema for in these",
                "poor theories. It is",
                "my fondest hope that",
                "someday someone",
                "shall conquer that",
                "thorny problem and",
                "enlighten the world."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public DimensionalTravel()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public DimensionalTravel(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Ethical Hedonism: An Introduction
    public class EthicalHedonism : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Ethical Hedonism: An Introduction", "Richard Garriott",
            new BookPageInfo(
                "  Societies oft have",
                "common codes of",
                "conduct which it",
                "expects all its people",
                "to abide by. Now,",
                "while 'tis true that",
                "this can offer some",
                "advantages, most of"),
            new BookPageInfo(
                "the codes I see today",
                "around Britannia have",
                "fatal flaws. Let us",
                "examine them.",
                "  First, there is",
                "Blackthorn's code of",
                "Chaos or basically",
                "Anarchy. Whereas"),
            new BookPageInfo(
                "this affords the",
                "individual maximum",
                "opportunity for",
                "individuality and even",
                "pursuit of personal",
                "happiness, it does not",
                "offer even basic",
                "interpersonal conduct"),
            new BookPageInfo(
                "codes to prevent",
                "people from killing",
                "each other.",
                "  Without such basic",
                "tenets, all the people",
                "will need to spend a",
                "significant portion of",
                "their time and effort"),
            new BookPageInfo(
                "towards personal",
                "protection and thus",
                "less time towards",
                "other more beneficial",
                "pursuits.",
                "  Then there are the",
                "moral codes that are",
                "so popular today."),
            new BookPageInfo(
                "These codes are built",
                "largely on historical",
                "tradition rather than",
                "current logic and thus",
                "are also antiquated.",
                "For example many",
                "moral codes we see",
                "today include"),
            new BookPageInfo(
                "statements about not",
                "eating certain foods",
                "that once were often",
                "poisonous, but today",
                "can be prepared",
                "safely.",
                "  Many forbid contact",
                "between young people"),
            new BookPageInfo(
                "of the opposite",
                "gender, which can in",
                "fact be hazardous; but",
                "the codes often have",
                "lost the context as to",
                "why this is done,",
                "instead merely calling",
                "it amoral. In this day"),
            new BookPageInfo(
                "and age to call that a",
                "necessary moral",
                "would need a new",
                "reasoning. I put forth",
                "that tradition is not",
                "enough",
                "  Then there are",
                "Lord British's"),
            new BookPageInfo(
                "Virtues. It strikes me",
                "that while a system",
                "of virtues is",
                "wonderful as a",
                "touchstone to guide a",
                "society to good",
                "behavior, these are",
                "but shades of the"),
            new BookPageInfo(
                "underlying truth as to",
                "why one may wish to",
                "live a life according to",
                "certain rules of",
                "conduct.",
                "  On the other hand,",
                "clearly the Virtues",
                "that I have heard"),
            new BookPageInfo(
                "Lord British speak of",
                "are clearly positive",
                "codes of conduct, far",
                "better than the world",
                "of anarchy that Lord",
                "Blackthorn suggests.",
                "Yet, are not these",
                "Virtues still derived"),
            new BookPageInfo(
                "from a set of",
                "principles which",
                "though they sound",
                "good, are difficult to",
                "pin down as actual,",
                "undeniable, rational",
                "truths?",
                "  Worse yet though"),
            new BookPageInfo(
                "imagine a society",
                "who's code of",
                "conduct was based on",
                "pure survival of the",
                "strongest. While this",
                "society may function",
                "and even accomplish",
                "much, it can be"),
            new BookPageInfo(
                "fairly argued that",
                "personal happiness",
                "would suffer greatly,",
                "except for those at",
                "the top. To rule that",
                "out, however, we",
                "must first believe",
                "that people have a"),
            new BookPageInfo(
                "right to pursue",
                "happiness.",
                "  I hope is a safe",
                "assumption that all",
                "beings wish to be",
                "happy; I will broadly",
                "describe this as",
                "Hedonism. Yet, if all"),
            new BookPageInfo(
                "people did is live a",
                "life of hedonism,",
                "their hedonism might",
                "be in conflict with",
                "those near them, so I",
                "will use the term",
                "Ethics to describe",
                "limits one might put"),
            new BookPageInfo(
                "on one's hedonistic",
                "tendencies to allow",
                "others to pursue their",
                "happiness as well.",
                "  Allow me to give",
                "this example: If one",
                "were to live alone on a",
                "desert isle, one could"),
            new BookPageInfo(
                "live a life of pure",
                "hedonism, for no",
                "action one might take",
                "could interfere with",
                "another's right to",
                "pursue their",
                "happiness. Poison the",
                "lake if you like, there"),
            new BookPageInfo(
                "is no one to blame but",
                "yourself!",
                "  Now suppose two",
                "of you live on that",
                "island. Thou dost not",
                "want thy neighbor to",
                "feel free to poison the",
                "lake. Would it not be"),
            new BookPageInfo(
                "better to consider it",
                "unethical to poison the",
                "lake without first",
                "thinking of those",
                "whose pursuit of",
                "happiness might be",
                "affected by this",
                "action?"),
            new BookPageInfo(
                "  I put forth that it is",
                "the fact that we as a",
                "people choose to live in",
                "groups known as a",
                "society that causes us",
                "to compromise our",
                "pure hedonism with",
                "logical ethics."),
            new BookPageInfo(
                "Likewise we accept",
                "not being able to kill",
                "others without",
                "reason, because our",
                "own pursuit of",
                "happiness would be",
                "greatly interfered",
                "with if we feared"),
            new BookPageInfo(
                "others would do the",
                "same to us. From",
                "this basis of logic can",
                "be formed the Tenets",
                "of Ethical Hedonism.",
                "  For more on this",
                "subject, see The",
                "Tenants of Ethical"),
            new BookPageInfo(
                "Hedonism, by",
                "Richard Garriott and",
                "Herman Miller."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public EthicalHedonism()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public EthicalHedonism(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region My Story
    public class MyStory : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "My Story", "Sherry the Mouse",
            new BookPageInfo(
                "  'Twas on a chill",
                "night, when the moon",
                "shone pasty-faced",
                "above the horizon,",
                "balanced on the",
                "towers of Lord",
                "British's castle, that",
                "the events I am about"),
            new BookPageInfo(
                "to relate took place,",
                "some years ago now. I",
                "witnessed them all",
                "from my tiny",
                "mousehole.",
                "  Milords British and",
                "Blackthorn are",
                "accustomed to a game"),
            new BookPageInfo(
                "of chess 'pon an",
                "evening, over which",
                "they argue the issues",
                "that affect the course",
                "of the realm. Lord",
                "Blackthorn was on his",
                "way to Lord British's",
                "chambers, and Lord"),
            new BookPageInfo(
                "British stood by a",
                "window casement,",
                "just having finished",
                "setting the pieces",
                "upon the board.",
                "  Suddenly the",
                "shutters blew open,",
                "and Lord British fell"),
            new BookPageInfo(
                "to the ground, one",
                "hand shielding his",
                "eyes. A chill wind",
                "entered the room, and",
                "it seemed a gash was",
                "torn in the very air.",
                "Through the gash I",
                "could see stars and"),
            new BookPageInfo(
                "swirling clouds of",
                "stellar dust, and a",
                "coldness sucked all",
                "the warmth from the",
                "air. A terrible wind",
                "tossed books and",
                "blankets across the",
                "room, and furniture"),
            new BookPageInfo(
                "toppled.",
                "  From within this",
                "gash issued a great",
                "voice, unlike any I",
                "have ever heard. And",
                "these are the words it",
                "spoke (for I",
                "memorized them most"),
            new BookPageInfo(
                "carefully):",
                "  \"Greetings, Lord",
                "British. I am the",
                "Time Lord, a being",
                "from beyond your",
                "dimension, as thou",
                "art from a world",
                "other than Sosaria. I"),
            new BookPageInfo(
                "am here to bring thee",
                "warning. Dost thou",
                "recall how long ago a",
                "mysterious Stranger",
                "came to Sosaria and",
                "saved the world from",
                "the evil wizard",
                "Mondain? He"),
            new BookPageInfo(
                "shattered the Gem of",
                "Immortality, within",
                "which dwelled a",
                "perfect likeness of",
                "this world.\"",
                "  Lord British slowly",
                "stood and faced the",
                "hole in the air. \"I"),
            new BookPageInfo(
                "remember,\" he said.",
                "\"Oft have I wished",
                "that stranger would",
                "return.\"",
                "  \"He hath returned,\"",
                "spoke the voice. \"But",
                "not to here. When the",
                "Gem was shattered, a"),
            new BookPageInfo(
                "thousand shards were",
                "scattered across the",
                "dimensions, and in",
                "each shard there is a",
                "perfect likeness of",
                "this world. And thou",
                "dost live upon one",
                "such shard, for thou"),
            new BookPageInfo(
                "art not of the true",
                "world-thou art",
                "merely a reflection.\"",
                "  Lord British looked",
                "shaken by this, and I",
                "did not know what to",
                "think! Was I merely a",
                "shadow of the real"),
            new BookPageInfo(
                "me, which lives still",
                "somewhere else",
                "across uncounted",
                "universes?",
                "  \"My task is to heal",
                "this shattered world,",
                "Lord British,\" said",
                "the voice. \"And I seek"),
            new BookPageInfo(
                "to enlist thee in my",
                "cause. Be warned that",
                "in this case, healing",
                "carries with it a",
                "terrible price.\"",
                "  Concern warred",
                "with curiosity on my",
                "liege's face, but ever"),
            new BookPageInfo(
                "one to shoulder a",
                "burden, he",
                "straightened and",
                "faced the gash in the",
                "air bravely. \"Name",
                "thy price.\"",
                "  \"A shard of a",
                "universe is a"),
            new BookPageInfo(
                "powerful thing, and a",
                "universe shattered is",
                "always in danger",
                "from the powers of",
                "darkness. Already",
                "three shards were",
                "turned to evil, and",
                "sent to plague the"),
            new BookPageInfo(
                "original universe in",
                "the form of",
                "Shadowlords. Many",
                "times have I brought",
                "the Stranger back to",
                "Britannia, to preserve",
                "it from its own folly",
                "or from outside"),
            new BookPageInfo(
                "dangers. Yet as long",
                "as the world",
                "remaineth in pieces,",
                "it remaineth",
                "vulnerable. We must",
                "bring the shards into",
                "harmony, so that they",
                "resonate in such a"),
            new BookPageInfo(
                "manner that matches",
                "the original universe.",
                "Then the two",
                "universes shall",
                "merge, and be again",
                "as one.\"",
                "  \"But if we are only",
                "shadows...\" Lord"),
            new BookPageInfo(
                "British said",
                "wonderingly.",
                "  The light from the",
                "stars within the hole",
                "seemed to dim.",
                "\"Indeed, the",
                "reflections shall",
                "become one with the"),
            new BookPageInfo(
                "original. Thou wouldst",
                "cease to be as thou",
                "art, and become part",
                "of the larger you.",
                "Thou shalt not die;",
                "however, uncounted",
                "generations have",
                "passed and borne"),
            new BookPageInfo(
                "children since that",
                "day, and they have no",
                "counterparts. They",
                "would perish utterly.\"",
                "  Lord British sagged",
                "in shock, realizing",
                "the terrible price that",
                "would be paid to heal"),
            new BookPageInfo(
                "the universe. \"All of",
                "my people,\" he",
                "breathed.",
                "  \"'Tis for the greater",
                "good.\"",
                "  Lord British bowed",
                "his head.",
                "  'Twas then I saw"),
            new BookPageInfo(
                "the movement by the",
                "door, half-hid by the",
                "heavy red curtains.",
                "Lord Blackthorn stood",
                "there, concealed from",
                "the rest of the room,",
                "his face white. How",
                "long had he been"),
            new BookPageInfo(
                "listening? I cannot",
                "say, yet I suspect",
                "that he had heard all",
                "that the mysterious",
                "voice had to say.",
                "  \"How then, shall I",
                "aid thee?\" Lord",
                "British said,"),
            new BookPageInfo(
                "weariness in his",
                "voice.",
                "  \"Aid the nobilty that",
                "resideth in the",
                "human heart. Protect",
                "the Virtues that so",
                "recently came to thee",
                "in thought late at"),
            new BookPageInfo(
                "night. They are the",
                "Virtues of life, as",
                "your counterpart",
                "understands them to",
                "be. For when thy",
                "populace doth live and",
                "breathe these Virtues,",
                "shall it match the"),
            new BookPageInfo(
                "true Britannia, and",
                "thy shard shall",
                "rejoin with it.\"",
                "  The gash in the air",
                "began to close, and",
                "with it warmth stole",
                "back into the room.",
                "  \"I was going to"),
            new BookPageInfo(
                "discuss my idea with",
                "Blackthorn tonight,\"",
                "Lord British",
                "breathed. \"Have I no",
                "thoughts that are my",
                "own? Is my life but",
                "a reflection of",
                "another me?\""),
            new BookPageInfo(
                "  \"Nay,\" said the",
                "voice, smaller through",
                "the diminished",
                "opening. \"Say, rather,",
                "that you are parallel,",
                "for there is no",
                "guarantee that thou",
                "shalt accomplish what"),
            new BookPageInfo(
                "I have set thee to. I",
                "speak tonight to a",
                "thousand of thee, and",
                "ask the same of all.",
                "Perhaps not all shall",
                "seek to aid me.\" And",
                "with that, the gash",
                "closed, and the voice"),
            new BookPageInfo(
                "was gone, leaving a",
                "room that appeare",
                "tossed by a mighty",
                "storm.",
                "  \"Destroy the world",
                "to save the universe,\"",
                "Lord British said",
                "bitterly. \"I do not"),
            new BookPageInfo(
                "wonder that some",
                "may balk.\"",
                "  Lord Blackthorn",
                "collected himself, and",
                "strode into the room,",
                "a decent mimicry of",
                "surprise on his face.",
                "\"My liege! What has"),
            new BookPageInfo(
                "happened here?\" he",
                "exclaimed, feigning",
                "dismay well. But not",
                "well enough to fool",
                "his old friend, whose",
                "eyes narrowed at",
                "seeing him there.",
                "  \"How much didst"),
            new BookPageInfo(
                "thou hear?\" demanded",
                "Lord British.",
                "  \"Why, nothing,\"",
                "managed Blackthorn,",
                "his head ducked away",
                "from his friend, as",
                "he bent to retrieve the",
                "fallen chess pieces. \"I"),
            new BookPageInfo(
                "merely came for our",
                "game of chess.\"",
                "  Together they",
                "righted the pedestal",
                "table, and set the",
                "pieces upon the black",
                "and white squares.",
                "\"Such simplicity to"),
            new BookPageInfo(
                "the game, Blackthorn,\"",
                "mused Lord British,",
                "idly brushing one",
                "finger against the",
                "board. \"Black and",
                "white, each to its own",
                "color, as if life were",
                "so simple. What think"),
            new BookPageInfo(
                "you?\"",
                "  Blackthorn sat",
                "heavily on a hassock",
                "beside the chess table.",
                "\"I think that matters",
                "are never so simple,",
                "my liege. And that I",
                "would regret it deeply"),
            new BookPageInfo(
                "if someone, such as a",
                "friend, saw it thus.\"",
                "  Lord British's eyes",
                "met his. \"Yet",
                "sometimes one must",
                "sacrifice a pawn to",
                "save a king.\"",
                "  Lord Blackthorn met"),
            new BookPageInfo(
                "his gaze squarely.",
                "\"Even pawns have",
                "lives and loves at",
                "home, my lord.\" Then",
                "he reached out for a",
                "pawn, and firmly",
                "moved it forward two",
                "squares. \"Shall we"),
            new BookPageInfo(
                "play a game?\" he",
                "asked.",
                "  The chess game that",
                "night was a draw,",
                "and they played",
                "grimly.",
                "  And the next day,",
                "Lord British gathered"),
            new BookPageInfo(
                "the nobles to proclaim",
                "the idea of a new",
                "system of Virtues,",
                "and declared that",
                "shrines should be",
                "built across the land.",
                "  Lord Blackthorn",
                "opposed it bitterly,"),
            new BookPageInfo(
                "and many thought",
                "him strange for doing",
                "so, for ever had he",
                "been a noble and",
                "upright man, and",
                "ever had he and Lord",
                "British been in",
                "accord. Declaring that"),
            new BookPageInfo(
                "he should start his",
                "own shrine, he",
                "departed the castle",
                "that day to live in a",
                "tower in a lake on the",
                "north side of the",
                "city.",
                "  They are still the"),
            new BookPageInfo(
                "best of friends, yet a",
                "sadness hangs",
                "between them, as if",
                "they were forced into",
                "making choices that",
                "appealed not to them.",
                "And at night, when I",
                "creep softly from one"),
            new BookPageInfo(
                "corner of my liege's",
                "bedchamber to",
                "another, I sometimes",
                "see him take a pawn",
                "from his night table,",
                "and hold it in his",
                "hand, and quietly",
                "weep."),
            new BookPageInfo(
                "  But I am but a",
                "mouse, and none hear",
                "me. This tale goes",
                "unknown, save for",
                "my writing these",
                "enormous letters with",
                "mine ink-stained tiny",
                "paws for thee to"),
            new BookPageInfo(
                "read, for I fear",
                "indeed for our world",
                "and for our people in",
                "these perilous times."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public MyStory()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public MyStory(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region On the Diversity of Our Land
    public class DiversityOfOurLand : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "On the Diversity of Our Land", "Lord Blackthorn",
            new BookPageInfo(
                "  While I deplore the",
                "depredations of the",
                "misguided and",
                "belligerent races with",
                "which we share our",
                "fair Britannia, and",
                "alongside the populace,",
                "do mourn the needless"),
            new BookPageInfo(
                "deaths that their",
                "raids cause, I cannot",
                "countenance the policy",
                "of wholesale slaughter",
                "of these races that",
                "seems to be the habit",
                "of our soldierly",
                "element."),
            new BookPageInfo(
                "  Can we not regard",
                "the ratmen, lizard",
                "men, and orcs are",
                "fellow intelligent",
                "beings with whom we",
                "share a planet? Why",
                "must we slay them",
                "on sight, rather than"),
            new BookPageInfo(
                "attempt to engage",
                "them in dialogue?",
                "There is no policy of",
                "shooting at wisps",
                "when they grace us",
                "with their presence",
                "(not that an arrow",
                "could do much to"),
            new BookPageInfo(
                "pierce them!).",
                "  To view these",
                "creatures as vermin",
                "denies their obvious",
                "intelligence, and we",
                "cannot underestimate",
                "the repercussions",
                "that their slaughter"),
            new BookPageInfo(
                "may have. If we",
                "regard the slaying of",
                "fellow humans as a",
                "crime, so must we",
                "regard the killing of",
                "an orc.",
                "  At the same time,",
                "should a lizardman"),
            new BookPageInfo(
                "slay a human, should",
                "we not forgive their",
                "ignorance and",
                "foolishness? Let us",
                "not surrender the",
                "high moral ground by",
                "descending to",
                "bestiality."),
            new BookPageInfo(
                "  Now, I say not that",
                "we should fail to",
                "defend ourselves in",
                "case of attack, for",
                "even amongst humans",
                "we see war, we see",
                "famine, and we see",
                "assault (though we"),
            new BookPageInfo(
                "owe a debt of",
                "gratitude to our Lord",
                "British for",
                "preserving us from",
                "the worst of these!).",
                "However, incursions",
                "such as the recent",
                "tragedy which cost us"),
            new BookPageInfo(
                "the life of Japheth,",
                "Guildmaster of",
                "Trinsic's Paladins,",
                "are folly.",
                "  I had met Japheth,",
                "and like all paladins,",
                "he burned with an",
                "inner fire. Yet"),
            new BookPageInfo(
                "though I had the",
                "utmost respect for",
                "him, none could deny",
                "the hatred that",
                "flashed in his eyes at",
                "the mere mention of",
                "orcs. And thus he",
                "carried his battle to"),
            new BookPageInfo(
                "the orc camps, and",
                "died there, unable to",
                "rise above his own",
                "childhood experiences",
                "depicted in his book,",
                "\"The Burning of",
                "Trinsic.\" 'Tis a",
                "shame that even our"),
            new BookPageInfo(
                "mightiest men fall",
                "prey to this",
                "ignorance!",
                "  Are there not",
                "legends of orcs",
                "adopting human",
                "children to raise as",
                "their own? Tales of"),
            new BookPageInfo(
                "complex societies built",
                "underground by races",
                "we regard as bestial?",
                "  Let us not repeat",
                "the mistake of",
                "Japheth of the",
                "Paladins, and let us",
                "cease to persecute the"),
            new BookPageInfo(
                "nonhuman races,",
                "before we discover",
                "that we are harming",
                "ourselves in the",
                "process."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public DiversityOfOurLand()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public DiversityOfOurLand(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Quest of the Virtues
    public class QuestOfVirtues : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Quest of the Virtues", "Autenil",
            new BookPageInfo(
                "Volume 1",
                "Chapter 1: Starting out",
                "I begin my Quest in the",
                "fine bank of Skara Brae.",
                "My Quest is to travel by",
                "foot to the eight shrines",
                "of Britannia. Although",
                "this may sound easy, it"),
            new BookPageInfo(
                "is hampered because I",
                "have forsaken my abilities",
                "and worldly possessions.",
                "Instead, all that I have",
                "to live by are a set of",
                "plain clothes, a ship to",
                "sail the seas and my",
                "diary in which to record"),
            new BookPageInfo(
                "my adventures.  Onward",
                "Ho towards the Shrine of",
                "Spirituality!",
                "",
                "Chapter 2: The Road to",
                "Spirituality",
                "",
                "Since I have forsaken"),
            new BookPageInfo(
                "magic as a form of",
                "travel, I must find a way",
                "to the mainland from this",
                "island town. Fortunately,",
                "I was able to barter a",
                "ride from the ferryman.",
                "Upon reaching the",
                "mainland, I followed the"),
            new BookPageInfo(
                "road until it turned",
                "North, yet I must",
                "continue Eastward. I",
                "paused to listen to the",
                "pleasant sounds of the",
                "birds chirping and enjoyed",
                "the peace away from the",
                "busy life. Upon reaching"),
            new BookPageInfo(
                "the Hedge Maze I recalled",
                "a story about the mage",
                "Relvinian in the days of",
                "old and turned South to",
                "go around it. Possessing",
                "no weapons and having",
                "forsaken my training, I",
                "heeded the sign that said,"),
            new BookPageInfo(
                "\"Enter and Become One",
                "Among Ghosts.\" The River",
                "forced me to turn South",
                "and then continue East.",
                "",
                "Chapter 3: Spirituality",
                "",
                "Spirituality is the leader"),
            new BookPageInfo(
                "of the Virtues. It is the",
                "meditation and",
                "understanding of all other",
                "Virtues. Without",
                "Spirituality, one cannot",
                "completely follow the",
                "Virtues, for It is the",
                "dedication and adherance"),
            new BookPageInfo(
                "to them.",
                "",
                "Chapter 4: Finding Honor",
                "",
                "My next visit will be to",
                "the Shrine of Honor,",
                "which lies a fair distance",
                "to the South. There is a"),
            new BookPageInfo(
                "road to my East that I",
                "will follow to the town",
                "of Trinsic. I found a",
                "warm room at the",
                "Traveller's Inn, and woke",
                "refreshed in the morning",
                "to hear the sounds of",
                "nature as I prepared to"),
            new BookPageInfo(
                "continue my journeys. I",
                "thanked the kind innkeeper",
                "and headed out of Trinsic",
                "and South. I conversed",
                "briefly with one lucky",
                "enough to own a house in",
                "the beautiful country and",
                "he bade me good fortune"),
            new BookPageInfo(
                "on my travels. I used my",
                "rusty blade to cut",
                "through dense jungles",
                "past ruins of a forgotten",
                "realm, now inhabited only",
                "by the Undead. Many a",
                "mongbat did hamper my",
                "journey, but finally I"),
            new BookPageInfo(
                "arrived at the Shrine of",
                "Honor.",
                "",
                "Chapter 5: Honor",
                "",
                "Many link Honor and",
                "battle, but it can be",
                "used with any aspect of"),
            new BookPageInfo(
                "Life. Honor is to abide",
                "by the rules, dishonor is",
                "to cheat; to seek the",
                "unfair advantage. I vowed",
                "to always live life with",
                "Honor.",
                "",
                "Chapter 6: Seeking Valor"),
            new BookPageInfo(
                "I must now embark on my",
                "trusty small ship, the",
                "Hollandia, to the South",
                "and East, to a small",
                "island where few have",
                "travelled. I know not yet",
                "what I will encounter at",
                "sea, so I bid the Virtues"),
            new BookPageInfo(
                "grant me safety. So",
                "begins my voyage. I",
                "managed to sail unnoticed",
                "past some water",
                "elementals which took a",
                "fair bit of navigation",
                "from my tillerman.",
                "However, I arrived without"),
            new BookPageInfo(
                "incident.",
                "",
                "Chapter 7: Valor",
                "",
                "The Shrine of Valor is",
                "protected by many a",
                "beast far too poisonous",
                "and foul for myself to"),
            new BookPageInfo(
                "vanquish. One mush show",
                "Valor to approach the",
                "Shrine! Valor is often",
                "shown in one's willingness",
                "to fight what maybe a",
                "losing battle upon which",
                "he believes. It takes",
                "Valor to stand your"),
            new BookPageInfo(
                "ground against the many",
                "murderers and lawbreakers",
                "in our lands. You may",
                "lose, but you show Valor",
                "in that you fight that",
                "which must be opposed.",
                "Fight the fights you",
                "believe in, not just the"),
            new BookPageInfo(
                "fights you think you can",
                "win.",
                "",
                "Chapter 8: The Voyage to",
                "Humility",
                "",
                "My journey will continue",
                "to the East towards the"),
            new BookPageInfo(
                "Shrine of Humility. I",
                "launch my boat from the",
                "West side of the Island",
                "of Valor, where I made",
                "my daring escape from",
                "the many Giant Serpents",
                "chasing me with their",
                "poisonous venom and"),
            new BookPageInfo(
                "hissing tongues. Beautiful",
                "blue waves washed over",
                "the bow of the boat as",
                "dolphins played, merrily",
                "leading me on. The voyage",
                "is long and the water",
                "turbulent but finally land",
                "was struck. Quickly I ran,"),
            new BookPageInfo(
                "eager to find my final",
                "destination for the day.",
                "",
                "Chapter 9: Humility",
                "",
                "The Shrine of Humility is",
                "surprisingly spartan; it is",
                "merely a grove of stone"),
            new BookPageInfo(
                "pillars with an ankh and",
                "the Humility stone at its",
                "center. I would",
                "characterize Humility as",
                "this Quest; returning to",
                "my roots in this world. I",
                "have rejected my",
                "possessions and my wealth"),
            new BookPageInfo(
                "in order to rely only",
                "upon my cunning and",
                "instincts. No longer have",
                "I that which makes me",
                "Glorious to others, but I",
                "have only that which I",
                "need to survive. I may no",
                "longer rely upon myself, I"),
            new BookPageInfo(
                "must rely upon others",
                "for my survival. My",
                "journey to Humility has",
                "only made me realize even",
                "more how Blessed I have",
                "been."),
            new BookPageInfo(
                "Chapter 10: Onward to",
                "Honesty",
                "",
                "Now the journey will turn",
                "South towards the Island",
                "of Ice. However, my",
                "voyages and excursions",
                "into the jungle have made"),
            new BookPageInfo(
                "me quite tired, so I will",
                "camp here beside a river",
                "near the Shrine of",
                "Humility for the night. I",
                "awake the next morning",
                "refreshed and invigorated,",
                "but also under attack! A",
                "Headless One has noticed"),
            new BookPageInfo(
                "my rise from slumber and",
                "attacks viciously. My",
                "trusty cleaver was in my",
                "hand instantly, but my",
                "lack of skill with the",
                "weapon delayed the death",
                "of the creature. I",
                "launched the Hollandia and"),
            new BookPageInfo(
                "set sail for the Island of",
                "Ice, seeking the Shrine of",
                "Honesty. As I sail the",
                "vast oceans, I find myself",
                "desiring the company of",
                "my fellow man. Hopefully I",
                "shall meet some kind of",
                "traveller with whom I may"),
            new BookPageInfo(
                "exchange a few words.",
                "Shortly I was landing my",
                "boat on the North end of",
                "the Island of Ice. I",
                "quickly made my way",
                "through snow across the",
                "frigid tundra while trying",
                "to keep warm. The Shrine"),
            new BookPageInfo(
                "of Honesty bid me",
                "welcome as I felth the",
                "warmth radiate throughout",
                "me.",
                "",
                "Chapter 11: Honesty",
                "",
                "Honesty is to uphold and"),
            new BookPageInfo(
                "defend the truth at all",
                "times. Furthermore,",
                "Honesty requires us to",
                "be fair and true to our",
                "fellow man; not taking",
                "undue advantage. Honesty",
                "is a Virtue often lacking",
                "in today's world. It seems"),
            new BookPageInfo(
                "as though people are out",
                "to gain wealth with no",
                "regard to Honesty",
                "towards other people.",
                "Honesty is the foundation",
                "upon which trust is built.",
                "If the foundation",
                "crumbles, everything built"),
            new BookPageInfo(
                "upon it must fall. The",
                "cold environment which",
                "houses the Shrine of",
                "Honesty is a testament",
                "to its value in today's",
                "society. The symbolic cold",
                "and secluded location",
                "shows us that only the"),
            new BookPageInfo(
                "most dedicated to",
                "pursuing Honesty will",
                "achieve it. May we all be",
                "Honest with our fellow",
                "man and remember to",
                "treat them how we would",
                "like to be treated."),
            new BookPageInfo(
                "Chapter 12: The Path to",
                "Sacrifice.",
                "",
                "I made my way to the",
                "West side of the Island",
                "of Ice. From there I",
                "launch the Hollandia and",
                "sail slightly to the North"),
            new BookPageInfo(
                "and West. I land my boat",
                "just East of the Shrine",
                "of Sacrifice, so my road",
                "is West. Although I enjoy",
                "the sea, I am happy to",
                "be back on the mainland",
                "for the final three",
                "Shrines."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public QuestOfVirtues()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public QuestOfVirtues(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Regarding Llamas
    public class RegardingLlamas : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Regarding Llamas", "Simon",
            new BookPageInfo(
                "  Llamas are curious",
                "beasts, shaggy and",
                "sought after for their",
                "wool, yet of a",
                "curiously arrogant",
                "disposition reflected",
                "in their eyes. They",
                "live in mountainous"),
            new BookPageInfo(
                "areas, though who",
                "may have first tamed",
                "them is lost in the",
                "mists of history.",
                "  'Tis a well-known",
                "fact that llamas can",
                "indeed be tamed, and",
                "used as grazing"),
            new BookPageInfo(
                "animals, for their",
                "meat, and of course",
                "for their wool. Yet",
                "'tis lesser known that",
                "their ornery",
                "disposition and",
                "tendency to spit at",
                "those they dislike"),
            new BookPageInfo(
                "makes them appealing",
                "guard creatures as",
                "well, though they",
                "have little sound with",
                "which to sound an",
                "alarum."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public RegardingLlamas()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public RegardingLlamas(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Talking to Wisps
    public class TalkingToWisps : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Talking to Wisps", "Yorick ofMoonglow",
            new BookPageInfo(
                "This volume was",
                "sponsored by",
                "donations from Lord",
                "Blackthorn, ever a",
                "supporter of",
                "understanding the",
                "other sentient races",
                "of Britannia."),
            new BookPageInfo(
                "-",
                "  Wisps are the most",
                "intelligent of the",
                "nonhuman races",
                "inhabiting Britannia.",
                "'Tis claimed by the",
                "great sages that",
                "someday we shall be"),
            new BookPageInfo(
                "able to converse with",
                "them openly in our",
                "native",
                "tongue--indeed, we",
                "must hope that wisps",
                "learn our language,",
                "for it is not possible",
                "for humans to"),
            new BookPageInfo(
                "pronounce wispish!",
                "  The wispish",
                "language seems to",
                "only contain one",
                "vowel, the letter Y.",
                "However, the letters",
                "W, C, M, and L seem",
                "to be treated"),
            new BookPageInfo(
                "grammatically as",
                "vowels, and in",
                "addition every letter",
                "is followed by what",
                "sounds to the human",
                "ear like a glottal stop.",
                "It is possible that the",
                "glottal stop is"),
            new BookPageInfo(
                "considered a vowel as",
                "well.",
                "  Wisps do make use",
                "of what sound to us",
                "like pitch and",
                "emphasis shifts",
                "similar to",
                "exclamations and"),
            new BookPageInfo(
                "questions.",
                "  The average word is",
                "wispish seems to",
                "consist of three",
                "phonemes and three",
                "glottal stops, plus",
                "possibly a pitch shift.",
                "It often sounds like a"),
            new BookPageInfo(
                "fire burning or",
                "crackling. Some have",
                "speculated that what",
                "we are analyzing is",
                "in fact nothing more",
                "than the very air",
                "crackling near the",
                "wisp's glow, and not"),
            new BookPageInfo(
                "language, but this is",
                "of course unlikely."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public TalkingToWisps()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public TalkingToWisps(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Taming Dragons
    public class TamingDragons : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Taming Dragons", "Wyrd Beastmaster",
            new BookPageInfo(
                "  I have not much to",
                "tell about dragons. The",
                "sole time I approached",
                "one with an eye",
                "towards taming it,",
                "my initial attempts at",
                "calming it met with",
                "failure. It fixed a"),
            new BookPageInfo(
                "massive beady eye",
                "upon me, and began",
                "its slithering",
                "approach, intending no",
                "doubt to insert me",
                "into its maw and bear",
                "down with its teeth.",
                "  However, as I was"),
            new BookPageInfo(
                "engaged in what",
                "remains to this day",
                "the most terrifying",
                "combat of my life,",
                "the dragon suddenly",
                "whirled as if in a",
                "panic, ran a short",
                "distance, took off into"),
            new BookPageInfo(
                "the air, then",
                "transformed into a",
                "whirlwind. Lastly, it",
                "exploded, showering",
                "gouts of black blood",
                "and heaving, stinking",
                "flesh upon miles of",
                "countryside. The"),
            new BookPageInfo(
                "fireball was massive,",
                "enough to light a city,",
                "I should surmise.",
                "  I never did discover",
                "the exact cause of",
                "this strange behavior,",
                "except to assume that",
                "it was not typical for"),
            new BookPageInfo(
                "this reptilian species.",
                "My best guesses",
                "revolve around a",
                "magical fracture in",
                "the nature of reality,",
                "which is far too",
                "esoteric a territory",
                "for one of my limited"),
            new BookPageInfo(
                "scholarship.",
                "  Hence my basic",
                "advice to those who",
                "seek to tame a",
                "dragon-be sure that",
                "thou hast mastered",
                "the twin skills of",
                "taming animals, and"),
            new BookPageInfo(
                "running away very",
                "very fast."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public TamingDragons()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public TamingDragons(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region The Bold Stranger
    public class BoldStranger : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The Bold Stranger", "Old Fabio the Poor",
            new BookPageInfo(
                "    In a time before",
                "time, the Gods that Be",
                "assembled a group of",
                "artisans, craftsmen",
                "and lore masters",
                "(for, yes, even in",
                "those days, art",
                "existed) to create the"),
            new BookPageInfo(
                "world of Sosaria. To",
                "this group, the gods",
                "gave a tiny world,",
                "Rytabul, in which to",
                "test their works, to",
                "see if they were of",
                "the quality desired",
                "for the true world in"),
            new BookPageInfo(
                "which they would be",
                "placed. And though",
                "the gods were tight",
                "fisted with their gold,",
                "this small crew",
                "worked hard and long,",
                "and were happy in",
                "their tasks."),
            new BookPageInfo(
                "    A small corner of",
                "Rytabul had been",
                "claimed by the artisan",
                "Selrahc the Slow.",
                "Though he was not",
                "the fastest of the",
                "assembled workers,",
                "the gods smiled upon"),
            new BookPageInfo(
                "his work, even",
                "presenting him with",
                "a mystic talisman",
                "proclaiming his work",
                "the best among the",
                "newer artisans. And",
                "so Selrahc went about",
                "his business, creating"),
            new BookPageInfo(
                "hundreds of designs",
                "which would one day",
                "add color and variety",
                "to Sosaria.",
                "    One day a",
                "stranger appeared to",
                "Selrahc. His chest",
                "was bare and he wore"),
            new BookPageInfo(
                "trousers of the",
                "brightest green, and",
                "wherever he went,",
                "plants grew in his",
                "footsteps. This",
                "caused Selrahc no end",
                "of trouble, the",
                "stranger always"),
            new BookPageInfo(
                "looking over his",
                "shoulder, and the",
                "plants sprouting in",
                "places Selrahc",
                "required to ply his",
                "art. And so Selrahc",
                "approached the",
                "stranger and bade"),
            new BookPageInfo(
                "him speak. But this",
                "man in green",
                "remained silent.",
                "Selrahc pleaded with",
                "the stranger to give",
                "his name, and would",
                "he please leave",
                "Selrahc to his work."),
            new BookPageInfo(
                "But this mysterious",
                "stranger remained",
                "mute.",
                "    This angered",
                "Selrahc mightily. Who",
                "was this silent man,",
                "interfering with",
                "tasks the gods"),
            new BookPageInfo(
                "themselves had",
                "entrusted to Selrahc?",
                "In an attempt to",
                "embarrass this",
                "interloper, Selrahc",
                "stole his green",
                "trousers, leaving him",
                "naked and open to"),
            new BookPageInfo(
                "comments about his",
                "very manhood, and",
                "still the stranger",
                "would not speak,",
                "would not leave this",
                "tiny corner of",
                "Rytabul.",
                "    Vexed to his very"),
            new BookPageInfo(
                "limits, Selrahc took",
                "his war axe and",
                "smote the silent one",
                "mightily, again and",
                "again, until the silent",
                "stranger ran away,",
                "having never said a",
                "word, and never"),
            new BookPageInfo(
                "showed himself in",
                "Rytabul again.",
                "    Thus endeth the",
                "tale of the bold",
                "stranger."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public BoldStranger()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public BoldStranger(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region The Burning of Trinsic
    public class BurningOfTrinsic : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The Burning of Trinsic", "Japheth of Trinsic",
            new BookPageInfo(
                "    'Twas a sight to",
                "see, the sunlight",
                "falling lightly on the",
                "sandstone walls of",
                "Trinsic 'pon a",
                "morning in spring.",
                "    Children ran along",
                "the parapets and"),
            new BookPageInfo(
                "walkways, their",
                "laughter and running",
                "providing music to the",
                "daybreak, despite",
                "their oft-ragged",
                "clothing.",
                "    And I was one of",
                "those young ones,"),
            new BookPageInfo(
                "letting my joy rise",
                "up to the skies.",
                "    Little did we all",
                "know of the darker",
                "days that would lie",
                "ahead, for we were",
                "too young.",
                "    Had we but gained"),
            new BookPageInfo(
                "access to the quiet",
                "councils held in the",
                "Paladin tower as it",
                "faced the sea,",
                "councils lit by",
                "candlelight and",
                "worry, we would",
                "have learned more of"),
            new BookPageInfo(
                "the fears of",
                "imminent attack from",
                "the forest, where",
                "foul creatures born",
                "of dank caves and",
                "darkness were",
                "marauding ever more",
                "often into the lands"),
            new BookPageInfo(
                "around Trinsic's",
                "moat.",
                "    But we were",
                "children! The",
                "parapets and the moat",
                "were places to play,",
                "not stout defenses,",
                "and we gave no"),
            new BookPageInfo(
                "thought to the",
                "necessities that must",
                "have required their",
                "construction.",
                "    We used to reach",
                "the sheltered",
                "orchards on the lee",
                "side of the parapet"),
            new BookPageInfo(
                "walls, where the",
                "southern river cut",
                "through the city, by",
                "swimming across the",
                "water.",
                "    The rich folk who",
                "lived in the great",
                "manses there would"),
            new BookPageInfo(
                "shout from their",
                "windows and shake",
                "their fists, for we",
                "would run through",
                "their gardens and",
                "tear up the delicate",
                "foxgloves and",
                "orfleurs with our"),
            new BookPageInfo(
                "unshod dirty feet.",
                "Then we would dive",
                "into the water and",
                "splash merrily to the",
                "fruit trees.",
                "    The southern",
                "river lazily slid",
                "under the an ungated"),
            new BookPageInfo(
                "arch in the mighty",
                "wall, and we would",
                "lay on the grassy",
                "bank and watch it",
                "gurgle by the lily",
                "pads.",
                "    That spring that",
                "pleasant spot became"),
            new BookPageInfo(
                "the doorway through",
                "which our city of",
                "Trinsic let in the",
                "monstrous deformed",
                "humanoids that",
                "savaged us. I lay upon",
                "that grassy bank and",
                "watched them wade"),
            new BookPageInfo(
                "in, their coarse hair",
                "wet and matted, algae",
                "and muck festooning",
                "their wild brows.",
                "    They caught sight",
                "of a quicksilver girl",
                "with bright blond hair",
                "and lively eyes. Her"),
            new BookPageInfo(
                "name was Leyla, and",
                "that spring I had held",
                "fond dreams of",
                "holding her hand and",
                "sharing flavored ice",
                "while dangling our",
                "feet off the small",
                "bridge by Smugglers"),
            new BookPageInfo(
                "Gate.",
                "    And I said nothing",
                "when they caught",
                "her, and did not cry",
                "out when they",
                "dragged her off",
                "through that breach in",
                "our wall, and did not"),
            new BookPageInfo(
                "warn the city when I",
                "saw the helmeted orc",
                "captains call the",
                "charge upon the",
                "mansions.",
                "    Blame me not, for",
                "I was but a child, and",
                "one who hid in the"),
            new BookPageInfo(
                "branches of the peach",
                "trees, all a-tremble",
                "whilst I watched the",
                "smoke rise from Sean",
                "the tailor's, and fire",
                "lash out at the roof of",
                "witchy Eleanor's",
                "tavern."),
            new BookPageInfo(
                "    To this day I have",
                "had no word of",
                "Leyla, and to this",
                "day the smell of",
                "burning wood can",
                "conjure terrible",
                "dreams. Yet with the",
                "eyes of adulthood, 'tis"),
            new BookPageInfo(
                "possible to examine",
                "the flaws in the",
                "defense of Trinsic on",
                "that fateful day, and",
                "the reasons why our",
                "walls are now",
                "double-thick, and",
                "why our buildings"),
            new BookPageInfo(
                "are now built as",
                "fortresses within a",
                "somber fortified city.",
                "    While I can look",
                "out from the top of",
                "the new Paladin",
                "tower, and spy the",
                "mighty white sails"),
            new BookPageInfo(
                "across the barrier",
                "island, and can",
                "descry the small",
                "hollow south of the",
                "city where gypsies",
                "are wont to camp, I",
                "can also envision the",
                "city as it might be"),
            new BookPageInfo(
                "burning, and I bless",
                "the bargain we made:",
                "space for safety,",
                "grace for sturdiness,",
                "and wood for stone.",
                "    Whilst I live, I",
                "shall not see Trinsic",
                "burn, and no more"),
            new BookPageInfo(
                "cries of little girls",
                "will haunt the sleep",
                "of our fair citizens.",
                "    - Japheth, Paladin",
                "Guildmaster of the",
                "City of Trinsic"));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public BurningOfTrinsic()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public BurningOfTrinsic(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region The Fight
    public class TheFight : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The Fight", "M. de la Garza",
            new BookPageInfo(
                "    A cold autumn's",
                "morning with misty",
                "fog secures a dozen",
                "brave knights,",
                "supplying hidden",
                "shelter from prying",
                "eyes deep in the",
                "foothills of the"),
            new BookPageInfo(
                "vibrant valley.",
                "Dragons soar like",
                "fierce warriors,",
                "circling around and",
                "around, then roaring",
                "like thunder, rallying",
                "all that listen.  The",
                "dragons land swiftly"),
            new BookPageInfo(
                "beside the proud",
                "warriors, bending",
                "necks and extending",
                "wings, lifting black",
                "claws and allowing",
                "valiant fighters to",
                "ride forth and win an",
                "arisen battle.  The"),
            new BookPageInfo(
                "increasing winds",
                "silence the sounds of",
                "combat, and they",
                "fight, standing their",
                "ground like mothers",
                "protecting their",
                "childern, bright",
                "armor flashing as"),
            new BookPageInfo(
                "each one falls.",
                "    A cold autumn's",
                "evening with misty",
                "fog cradles a dozen",
                "battered corpses of",
                "knights, creasing",
                "them in currents of",
                "winds that run deep"),
            new BookPageInfo(
                "in the foothills of the",
                "desolate valley.",
                "Dragons glide like",
                "silent angels, circling",
                "around and around,",
                "then calling like",
                "banshees; keening",
                "cries of mourning."),
            new BookPageInfo(
                "The dragons land",
                "heavily beside the",
                "peaceful bodies,",
                "bending necks and",
                "extending wings,",
                "lifting black claws",
                "and allowing valiant",
                "fighters to ride forth"),
            new BookPageInfo(
                "and win an arisen",
                "battle.  The increasing",
                "winds silence the",
                "sounds of combat, and",
                "they fight, standing",
                "their ground like",
                "mothers protecting",
                "their childern, bright"),
            new BookPageInfo(
                "armor flashing as",
                "each one falls.",
                "    A cold autumn's",
                "evening with misty",
                "fog cradles a dozen",
                "battered corpses of",
                "knights, creasing",
                "them in currents of"),
            new BookPageInfo(
                "winds that run deep",
                "in the foothills of the",
                "desolate valley.",
                "Dragons glide like",
                "silent angels, circling",
                "around and around,",
                "then calling like",
                "banshees; keening"),
            new BookPageInfo(
                "cries of mourning.",
                "The dragons land",
                "heavily beside the",
                "peaceful bodies,",
                "bending necks and",
                "extending wings,",
                "lifting black claws",
                "and pinching the"),
            new BookPageInfo(
                "sacred ground and",
                "new eternal home.",
                "The dying winds",
                "whistle among the",
                "dead in somber",
                "procession, and they",
                "lie, grasping weapons",
                "to protect themselves"),
            new BookPageInfo(
                "like knights still in",
                "battle, shattered",
                "armor shining like",
                "newly born stars."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public TheFight()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public TheFight(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region The Life of a Travelling Minstrel
    public class LifeOfATravellingMinstrel : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The Life of a Travelling Minstrel", "Sarah of Yew",
            new BookPageInfo(
                "  While 'tis true that",
                "the musician who",
                "seeketh only to make",
                "sweet music for",
                "herself and for",
                "others needs little",
                "more than some",
                "talent, and stern"),
            new BookPageInfo(
                "practice at the chosen",
                "instrument, those of",
                "us who seek the open",
                "road shall find indeed",
                "that a greater skill is",
                "required. Herein",
                "discover those secrets",
                "which I have learned"),
            new BookPageInfo(
                "over the years as an",
                "itinerant performer...",
                "  Once I was in",
                "Jhelom, and",
                "accidentally angered a",
                "bravo of some local",
                "repute, whose blade",
                "flickered all too"),
            new BookPageInfo(
                "eagerly near my",
                "slender neck (for I",
                "was young then).",
                "After various threats",
                "to \"ruin my pretty",
                "face\" this bravo",
                "grabbed my arm in a",
                "most unseemly"),
            new BookPageInfo(
                "fashion and tossed",
                "me into a barbaric",
                "enclosure locally",
                "entitled a dueling pit.",
                "My plaintive cries",
                "for help went",
                "unheeded by the",
                "guards, for the"),
            new BookPageInfo(
                "inhabitants of Jhelom",
                "are eager indeed to",
                "measure fighting",
                "prowess at any time!",
                "  What saved me was",
                "the ability to",
                "improvise a melody",
                "and tune that"),
            new BookPageInfo(
                "satirized the",
                "proceedings, and",
                "sufficiently angered",
                "an onlooker to prod",
                "him to coming to my",
                "defense. Once that",
                "fight was underway,",
                "I was able to make"),
            new BookPageInfo(
                "good my escape.",
                "Hence, I regard the",
                "ability to incite fights",
                "as indispensable to",
                "the prudent bard.",
                "  Upon another",
                "occasion, 'twas the",
                "obverse side of that"),
            new BookPageInfo(
                "coin which saved me,",
                "for I was being held",
                "prisoner by a",
                "particularly nasty",
                "band of ruffians who",
                "had seized me",
                "unawares from the",
                "road to Vesper."),
            new BookPageInfo(
                "  They had worked",
                "themselves into a",
                "frenzy and were",
                "ready to attack and I",
                "fear, tear me limb",
                "from limb, when I",
                "began to sing",
                "frantically, tapping"),
            new BookPageInfo(
                "my falled drum with",
                "my tied up feet. The",
                "melody developed into",
                "a soothing one, and",
                "the brigands slowly",
                "calmed down to the",
                "extent of apologizing,",
                "and they let me go!"),
            new BookPageInfo(
                "  A final example I",
                "would pray you grant",
                "your attention: once I",
                "was lost upon a large",
                "isle far to the east of",
                "the mainland, well",
                "beyond Serpent's",
                "Hold, where lava"),
            new BookPageInfo(
                "made its sluggish",
                "way across the",
                "surface landscape.",
                "And this accursed",
                "land was filled with",
                "vile beasts and",
                "cunning dragons.",
                "  I was being pursued"),
            new BookPageInfo(
                "by one of said fell",
                "dragons when I found",
                "myself trapped. I",
                "quickly skirted a",
                "bubbling pool of molten",
                "rock and attempted to",
                "hide.",
                "  The dragon scented"),
            new BookPageInfo(
                "me and was",
                "preparing to skirt the",
                "pool, when I began to",
                "play a lusty tune",
                "upon my lute that",
                "attracted its attention.",
                "Mesmerized and",
                "enticed by the"),
            new BookPageInfo(
                "melody, it stepped",
                "directly toward sme,",
                "and into the",
                "lava-where its foot",
                "was so burned that it",
                "quickly hopped away,",
                "undignified and",
                "annoyed."),
            new BookPageInfo(
                "  'Tis my fond hope",
                "that other travelling",
                "minstrels shall learn",
                "from my experiences",
                "and apply themselves",
                "to practicing these",
                "skills in order to",
                "preserve life and"),
            new BookPageInfo(
                "limb."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public LifeOfATravellingMinstrel()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public LifeOfATravellingMinstrel(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region The Major Trade Associations
    public class MajorTradeAssociation : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The Major Trade Associations", "Pieter of Vesper",
            new BookPageInfo(
                "  There are ten major",
                "trade associations that",
                "operate legitimately in",
                "the lands of Britannia",
                "and among its trading",
                "partners. Many of",
                "these guilds are",
                "divided into local or"),
            new BookPageInfo(
                "specialty subguilds,",
                "who use the same",
                "colors but vary the",
                "heraldic pattern.",
                "  There are many",
                "lesser trade",
                "associations that have",
                "closed membership,"),
            new BookPageInfo(
                "and one can join them",
                "only by invitation.",
                "Beltran's Guide to",
                "Guilds is the",
                "definitive text on the",
                "full range of guilds",
                "and other associations",
                "in Britannia, and I"),
            new BookPageInfo(
                "heartily recommend",
                "it.",
                "  In what follows I",
                "have attempted to",
                "bring together the",
                "known information",
                "regarding these",
                "guilds. I offer thee"),
            new BookPageInfo(
                "the name, typical",
                "membership, heraldic",
                "colors, known",
                "specialty",
                "organizations within",
                "the larger guild, and",
                "any known",
                "affiliations to other"),
            new BookPageInfo(
                "guilds, which often",
                "occur because of",
                "trade reasons.",
                "",
                "The Guild of Arcane",
                "Arts",
                "Members: alchemists",
                "and wizards"),
            new BookPageInfo(
                "Colors: blue and purple",
                "Subguilds: Illusionists,",
                "Mages, Wizards",
                "Affiliations: Healer's",
                "Guild",
                "",
                "The Warrior's Guild",
                "Members:"),
            new BookPageInfo(
                "mercenaries,",
                "soldiery, guardsmen,",
                "weapons masters,",
                "paladins.",
                "Colors: Blue and red",
                "Subguilds: Cavalry,",
                "Fighters, Warriors",
                "Affiliations: League"),
            new BookPageInfo(
                "of Rangers",
                "",
                "League of Rangers",
                "Members: rangers,",
                "bowyers, animal",
                "trainers",
                "Colors: Red, gold and",
                "blue"),
            new BookPageInfo(
                "",
                "Guild of Healers",
                "Members: healers",
                "Colors: Green, gold,",
                "and purple",
                "Affiliations: Guild of",
                "Arcane Arts"),
            new BookPageInfo(
                "Mining Cooperative",
                "Members: miners",
                "Colors: blue and black",
                "checkers, with a gold",
                "cross",
                "Affiliations: Order of",
                "Engineers"),
            new BookPageInfo(
                "Merchants'",
                "Association",
                "Members:",
                "innkeepers,",
                "tavernkeepers,",
                "jewelers,",
                "provisioners",
                "Colors: gold coins on a"),
            new BookPageInfo(
                "green field for",
                "Merchants.  White",
                "and green for the",
                "others.",
                "Subguilds: Barters,",
                "Provisioners,",
                "Traders, Merchants"),
            new BookPageInfo(
                "Order of Engineers",
                "Members: tinkers and",
                "engineers",
                "Colors: Blue, gold, and",
                "purple vertical bars",
                "Affiliations: Mining",
                "Cooperative"),
            new BookPageInfo(
                "Society of Clothiers",
                "Members: tailors and",
                "weavers",
                "Colors: Purple, gold,",
                "and red horizontal",
                "bars",
                "",
                "Maritime Guild"),
            new BookPageInfo(
                "Members: fishermen,",
                "sailors, mapmakers,",
                "shipwrights",
                "Colors: blue and white",
                "Subguilds:",
                "Fishermen, Sailors,",
                "Shipwrights"),
            new BookPageInfo(
                "Bardic Collegium",
                "Members: bards,",
                "musicians,",
                "storytellers, and other",
                "performers",
                "Colors: Purple, red",
                "and gold checkerboard"),
            new BookPageInfo(
                "Society of Thieves",
                "Members: beggars,",
                "cutpurses, assassins,",
                "and brigands",
                "Colors: red and black",
                "Subguilds: Rogues",
                "(beggars), Assassins,",
                "Thieves"));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public MajorTradeAssociation()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public MajorTradeAssociation(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region The Rankings of Trades
    public class RankingsOfTrades : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The Rankings of Trades", "Lord Higginbotham",
            new BookPageInfo(
                "  Whilst 'tis true that",
                "within each trade, one",
                "finds differing titles",
                "and accolades granted",
                "to the members of a",
                "given guild,",
                "nonetheless for the",
                "betterment of trade"),
            new BookPageInfo(
                "and understanding,",
                "we must have a",
                "commonality of",
                "titling.",
                "  For those who may",
                "find themselves",
                "ignorant of the finer",
                "distinctions between a"),
            new BookPageInfo(
                "three-knot member of",
                "the Sailors' Maritime",
                "Association and a",
                "second thaumaturge,",
                "this book shall serve",
                "as a simple",
                "introduction to the",
                "common cant used"),
            new BookPageInfo(
                "when members of",
                "differing guilds and",
                "trade organizations",
                "must trade with each",
                "other and must",
                "establish relative",
                "credentials.",
                "  Neophyte"),
            new BookPageInfo(
                "Has shown interest",
                "in learning the craft",
                "and some meager",
                "talent.",
                "  Novice",
                "Is practicing basic",
                "skills but has not been",
                "admitted to full"),
            new BookPageInfo(
                "standing.",
                "  Apprentice",
                "A student of the",
                "discipline.",
                "  Journeyman",
                "Warranted to practice",
                "the discipline under",
                "the eyes of a tutor."),
            new BookPageInfo(
                "  Expert",
                "A full member of the",
                "guild.",
                "  Adept",
                "A member of the",
                "guild qualified to",
                "teach others.",
                "  Master"),
            new BookPageInfo(
                "Acknowledged as",
                "qualified to lead a hall",
                "or business.",
                "  Grandmaster",
                "Rarely a permanent",
                "title, granted in",
                "common parlance to",
                "those who have"),
            new BookPageInfo(
                "shown extreme",
                "mastery of their",
                "craft recently."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public RankingsOfTrades()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public RankingsOfTrades(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region The Wild Girl of the Forest
    public class WildGirlOfTheForest : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "The Wild Girl of the Forest", "Horace the Trader",
            new BookPageInfo(
                "    Her name was",
                "Leyla, she said, and",
                "her hair was braided",
                "wild with creepers",
                "and thorns. I",
                "marveled that they",
                "did not hurt her, but",
                "when I asked, she but"),
            new BookPageInfo(
                "shrugged and let her",
                "eyes roam once more",
                "across the woods.",
                "Though I had my",
                "hands securely",
                "fastened by her",
                "ropes, I itched to",
                "reach out and comb"),
            new BookPageInfo(
                "that unruly golden",
                "mane, dirtied and",
                "leaf-ridden.",
                "    Her provenance,",
                "she told me over",
                "nights illumined by",
                "campfires, was once",
                "the city of Trinsic."),
            new BookPageInfo(
                "She claimed to have",
                "been kidnapped and",
                "raised by orcs, which",
                "I judged an unlikely",
                "tale, for all know orcs",
                "delight in eating the",
                "meat of honest folk.",
                "When I told her this,"),
            new BookPageInfo(
                "she laughed a fey",
                "laugh, and gaily",
                "admitted that honest",
                "she was not, for oft",
                "had she stolen folk",
                "away from caravans",
                "to loot their",
                "possessions from an"),
            new BookPageInfo(
                "unconscious body!",
                "    At this, I began to",
                "fear for my life, and",
                "her smile seemed full",
                "of teeth sharper than",
                "a human ought to",
                "have, for the tale of",
                "orcish raising had"),
            new BookPageInfo(
                "struck fear into the",
                "marrow of my bones.",
                "\"Wilt thou eat me?\" I",
                "asked, a-tremble,",
                "fearing the answer.",
                "    And she cocked",
                "her head at me, like a",
                "wild animal facing a"),
            new BookPageInfo(
                "word that it dost not",
                "understand, and the",
                "fixity in her eyes",
                "was a glimpse into",
                "the deeper reaches of",
                "the Abyss. But she",
                "finally grunted, and",
                "said, \"Nay,\" in a"),
            new BookPageInfo(
                "voice that recalled to",
                "me a child. \"Nay,\"",
                "she said, \"for thou",
                "dost remind me of a",
                "boy I knew once,",
                "when I was a girl",
                "who played in a city",
                "of great sandstone"),
            new BookPageInfo(
                "walls, before I was",
                "taken. He had sandy",
                "hair like thee, and I",
                "dreamt as a child of",
                "holding his hand and",
                "sharing flavored ice.",
                "His name was",
                "Japheth.\""),
            new BookPageInfo(
                "    The next morning",
                "she let me go,",
                "stripped of my pouch",
                "and clothes, and bade",
                "me run through the",
                "woods, and to fear",
                "recapture, for surely",
                "her heart would not"),
            new BookPageInfo(
                "soften again. 'Twas a",
                "fearful run, and I",
                "came to the road to",
                "Yew with welts and",
                "scratches run",
                "rampant crost my",
                "skin, but I did not see",
                "her again."),
            new BookPageInfo(
                "    Oft have I",
                "wondered of the boy",
                "named Japheth, and",
                "whether he",
                "remembers a girl who",
                "lived in sandstone",
                "walls. The only",
                "Japheth I know is the"),
            new BookPageInfo(
                "Guildmaster of",
                "Paladins who died",
                "last year warring",
                "amidst the orcs, and",
                "though he had indeed",
                "sandy hair, I cannot",
                "picture him side by",
                "side with a feral girl"),
            new BookPageInfo(
                "whose tongue has",
                "tasted of human",
                "flesh.",
                "    Yet the paths of",
                "fate are strange",
                "indeed, and I suppose",
                "'tis possible that this",
                "paladin died"),
            new BookPageInfo(
                "defending his",
                "remembered lady's",
                "honor, unknowingly",
                "struck down by the",
                "orc that she called",
                "father."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public WildGirlOfTheForest()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public WildGirlOfTheForest(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Treatise on Alchemy
    public class TreatiseOnAlchemy : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Treatise on Alchemy", "Felicia Hierophant",
            new BookPageInfo(
                "    The alchemical",
                "arts are notable for",
                "their deceptive",
                "simplicity. 'Tis true",
                "that to our best",
                "knowledge currently,",
                "there are but eight",
                "valid potions that can"),
            new BookPageInfo(
                "be made (though I",
                "emphasize that new",
                "discoveries may",
                "always await).",
                "However, the delicate",
                "balance of confecting",
                "the potions is",
                "difficult indeed, and"),
            new BookPageInfo(
                "requires great skill.",
                "    To give thee an",
                "example of the",
                "simpler potions that",
                "can be created by",
                "those well-versed in",
                "the subtleties of",
                "alchemy:"),
            new BookPageInfo(
                "    Black pearl, that",
                "rare substance that is",
                "oft found lying",
                "unannounced upon the",
                "surface of the",
                "ground, when",
                "properly crushed",
                "with mortar and"),
            new BookPageInfo(
                "pestle, can yield a",
                "fine powder. Said",
                "powder in the proper",
                "proportions when",
                "mixed via the",
                "alchemical arts can",
                "yield a wonderfully",
                "refreshing drink."),
            new BookPageInfo(
                "    The revolting blood",
                "moss so gingerly",
                "scraped off of",
                "windowsills by",
                "fastidious housewives",
                "is but a tiny cousin to",
                "the wilder version,",
                "which when properly"),
            new BookPageInfo(
                "prepared yields a",
                "magical liquid that for",
                "a time can make the",
                "imbiber a more agile",
                "and dextrous",
                "individual.",
                "    However, beware",
                "of the deadly"),
            new BookPageInfo(
                "nightshade, for it",
                "yields a deceptively",
                "sweet-tasting poison",
                "that can prove highly",
                "fatal to the drinker,",
                "and in fact is also",
                "used by assassins to",
                "coat their blades."),
            new BookPageInfo(
                "Fortunately, this",
                "latter art of poisoning",
                "is little known!",
                "    There is much to",
                "reward the student of",
                "alchemy, indeed. The",
                "rumours of longtime",
                "alchemists losing"),
            new BookPageInfo(
                "their hair and",
                "acquiring an",
                "unhealthy pallor, not",
                "to mention unsightly",
                "blotches upon their",
                "once-fair skin, are",
                "unhappily, true. Yet",
                "the joys of the mind"),
            new BookPageInfo(
                "make up for the",
                "complete loss of",
                "interest that others",
                "may have in thee as",
                "an object of",
                "courtship, and I have",
                "never regretted that",
                "choice. Honestly,"),
            new BookPageInfo(
                "truly. Not once."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public TreatiseOnAlchemy()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public TreatiseOnAlchemy(Serial serial)
            : base(serial)
        {
        }

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
    #endregion

    #region Virtue
    public class VirtueBook : BaseBook
    {
        public static readonly BookContent Content = new BookContent(
            "Virtue", "Lord British",
            new BookPageInfo(
                "  Within this world",
                "live people with many",
                "different ideals, and",
                "this is good. Yet what",
                "is it within the people",
                "of our land that sorts",
                "out the good from the",
                "evil, the cherished"),
            new BookPageInfo(
                "form the disdained?",
                "Virtue, I say it is,",
                "and virtue is the",
                "logical outcome of a",
                "people who wish to",
                "live together in a",
                "bonded society.",
                "  For without Virtues"),
            new BookPageInfo(
                "as a code of conduct",
                "which people maintain",
                "in their relations",
                "with each other, the",
                "fabric of that society",
                "will become weakened.",
                "For a society to grow",
                "and prosper for all,"),
            new BookPageInfo(
                "each must grant the",
                "others a common base",
                "of consideration.",
                "  I call this base the",
                "Virtues. For though",
                "one person might gain",
                "personal advantage by",
                "breaching such a"),
            new BookPageInfo(
                "code, the society as a",
                "whole would suffer.",
                "  There are three",
                "Principle Virtues that",
                "should guide people to",
                "enlightenment. These",
                "are: Truth, Love and",
                "Courage. From all the"),
            new BookPageInfo(
                "infinite reasons one",
                "may have to found an",
                "action, such as greed",
                "or charity, envy or",
                "pity, the three",
                "Principle Virtues",
                "stand out.",
                "  In fact all other"),
            new BookPageInfo(
                "virtues and vices can",
                "be show to be built",
                "from these principles",
                "and their opposite",
                "corruption's of",
                "Falsehood, Hatred and",
                "Cowardice. These",
                "three Principles can"),
            new BookPageInfo(
                "be combined in eight",
                "ways, which I will",
                "call the eight virtues.",
                "The eight virtues",
                "which we should",
                "build our society upon",
                "follow.",
                " Truth alone becomes"),
            new BookPageInfo(
                "Honesty, for without",
                "honesty between our",
                "people, how can we",
                "build the trust which",
                "is needed to",
                "maximize our",
                "successes.",
                " Love alone becomes"),
            new BookPageInfo(
                "compassion, for at",
                "some time or another",
                "all of us will need the",
                "compassion of others,",
                "and most likely",
                "compassion will be",
                "shown to those who",
                "have shown it."),
            new BookPageInfo(
                " Courage alone",
                "becomes Valor,",
                "without valor our",
                "people will never",
                "reach into the",
                "unknown or to the",
                "risky and will never",
                "achieve."),
            new BookPageInfo(
                " Truth tempered by",
                "Love give us Justice,",
                "for only in a loving",
                "search for the truth",
                "can one dispense fair",
                "Justice, rather than",
                "create a cold and",
                "callous people."),
            new BookPageInfo(
                " Love and Courage",
                "give us Sacrifice, for",
                "a people who love each",
                "other will be willing",
                "to make personal",
                "sacrifices to help",
                "other in need, which",
                "one day, may be"),
            new BookPageInfo(
                "needed in return.",
                " Courage and Truth",
                "give us Honor, great",
                "knights know this",
                "well, that chivalric",
                "honor can be found",
                "by adhering to this",
                "code of conduct."),
            new BookPageInfo(
                " Combining Truth,",
                "Love and Courage",
                "suggest the virtue of",
                "Spirituality the virtue",
                "that causes one to be",
                "introspective, to",
                "wonder about ones",
                "place in this world"),
            new BookPageInfo(
                "and whether one's",
                "deeds will be recorded",
                "as a gift to the world",
                "or a plague.",
                " The final Virtue is",
                "more complicated. For",
                "the eighth combination",
                "is that devoid of"),
            new BookPageInfo(
                "Truth, Love or",
                "Courage which can",
                "only exist in a state",
                "of great Pride, which",
                "of course is not a",
                "virtue at all. Perhaps",
                "this trick of fate is a",
                "test to see if one can"),
            new BookPageInfo(
                "realize that the true",
                "virtue is that of",
                "Humility. I feel that",
                "the people of",
                "Magincia fail to see",
                "this to such a degree",
                "that I would not be",
                "surprised if some ill"),
            new BookPageInfo(
                "fate awaited their",
                "future.",
                " Thus from the",
                "infinite possibilities",
                "which spawned the",
                "Three Principles of",
                "Truth, Love and",
                "Courage, come the"),
            new BookPageInfo(
                "Eight Virtues of",
                "Honesty, Compassion,",
                "Valor, Justice,",
                "Sacrifice, Honor,",
                "Spirituality, and",
                "Humility."));

        public override BookContent DefaultContent => Content;

        [Constructable]
        public VirtueBook()
            : base(Utility.Random(0xFEF, 2), false)
        {
        }

        public VirtueBook(Serial serial)
            : base(serial)
        {
        }

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
    #endregion
}