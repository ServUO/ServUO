using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Misc
{
    [Flags]
    public enum IHSFlags
    {
        None = 0x00,
        OnDamaged = 0x01,
        OnDeath = 0x02,
        OnMovement = 0x04,
        OnSpeech = 0x08,
        All = OnDamaged | OnDeath | OnMovement
    }

    // NOTE: To enable monster conversations, add " | OnSpeech" to the "All" line
    public class InhumanSpeech
    {
        private static InhumanSpeech m_RatmanSpeech;
        private static InhumanSpeech m_OrcSpeech;
        private static InhumanSpeech m_LizardmanSpeech;
        private static InhumanSpeech m_WispSpeech;
        private string[] m_Syllables;
        private string[] m_Keywords;
        private string[] m_Responses;
        private Dictionary<string, string> m_KeywordHash;
        private int m_Hue;
        private int m_Sound;
        private IHSFlags m_Flags;
        public InhumanSpeech()
        {
        }

        public static InhumanSpeech Ratman
        {
            get
            {
                if (m_RatmanSpeech == null)
                {
                    m_RatmanSpeech = new InhumanSpeech();

                    m_RatmanSpeech.Hue = 149;
                    m_RatmanSpeech.Sound = 438;

                    m_RatmanSpeech.Flags = IHSFlags.All;

                    m_RatmanSpeech.Keywords = new string[]
                    {
                        "meat", "gold", "kill", "killing", "slay",
                        "sword", "axe", "spell", "magic", "spells",
                        "swords", "axes", "mace", "maces", "monster",
                        "monsters", "food", "run", "escape", "away",
                        "help", "dead", "die", "dying", "lose",
                        "losing", "life", "lives", "death", "ghost",
                        "ghosts", "british", "blackthorn", "guild",
                        "guilds", "dragon", "dragons", "game", "games",
                        "ultima", "silly", "stupid", "dumb", "idiot",
                        "idiots", "cheesy", "cheezy", "crazy", "dork",
                        "jerk", "fool", "foolish", "ugly", "insult", "scum"
                    };

                    m_RatmanSpeech.Responses = new string[]
                    {
                        "meat", "kill", "pound", "crush", "yum yum",
                        "crunch", "destroy", "murder", "eat", "munch",
                        "massacre", "food", "monster", "evil", "run",
                        "die", "lose", "dumb", "idiot", "fool", "crazy",
                        "dinner", "lunch", "breakfast", "fight", "battle",
                        "doomed", "rip apart", "tear apart", "smash",
                        "edible?", "shred", "disembowel", "ugly", "smelly",
                        "stupid", "hideous", "smell", "tasty", "invader",
                        "attack", "raid", "plunder", "pillage", "treasure",
                        "loser", "lose", "scum"
                    };

                    m_RatmanSpeech.Syllables = new string[]
                    {
                        "skrit",
                        "ch", "ch",
                        "it", "ti", "it", "ti",
                        "ak", "ek", "ik", "ok", "uk", "yk",
                        "ka", "ke", "ki", "ko", "ku", "ky",
                        "at", "et", "it", "ot", "ut", "yt",
                        "cha", "che", "chi", "cho", "chu", "chy",
                        "ach", "ech", "ich", "och", "uch", "ych",
                        "att", "ett", "itt", "ott", "utt", "ytt",
                        "tat", "tet", "tit", "tot", "tut", "tyt",
                        "tta", "tte", "tti", "tto", "ttu", "tty",
                        "tak", "tek", "tik", "tok", "tuk", "tyk",
                        "ack", "eck", "ick", "ock", "uck", "yck",
                        "cka", "cke", "cki", "cko", "cku", "cky",
                        "rak", "rek", "rik", "rok", "ruk", "ryk",
                        "tcha", "tche", "tchi", "tcho", "tchu", "tchy",
                        "rach", "rech", "rich", "roch", "ruch", "rych",
                        "rrap", "rrep", "rrip", "rrop", "rrup", "rryp",
                        "ccka", "ccke", "ccki", "ccko", "ccku", "ccky"
                    };
                }

                return m_RatmanSpeech;
            }
        }
        public static InhumanSpeech Orc
        {
            get
            {
                if (m_OrcSpeech == null)
                {
                    m_OrcSpeech = new InhumanSpeech();

                    m_OrcSpeech.Hue = 34;
                    m_OrcSpeech.Sound = 432;

                    m_OrcSpeech.Flags = IHSFlags.All;

                    m_OrcSpeech.Keywords = new string[]
                    {
                        "meat", "gold", "kill", "killing", "slay",
                        "sword", "axe", "spell", "magic", "spells",
                        "swords", "axes", "mace", "maces", "monster",
                        "monsters", "food", "run", "escape", "away",
                        "help", "dead", "die", "dying", "lose",
                        "losing", "life", "lives", "death", "ghost",
                        "ghosts", "british", "blackthorn", "guild",
                        "guilds", "dragon", "dragons", "game", "games",
                        "ultima", "silly", "stupid", "dumb", "idiot",
                        "idiots", "cheesy", "cheezy", "crazy", "dork",
                        "jerk", "fool", "foolish", "ugly", "insult", "scum"
                    };

                    m_OrcSpeech.Responses = new string[]
                    {
                        "meat", "kill", "pound", "crush", "yum yum",
                        "crunch", "destroy", "murder", "eat", "munch",
                        "massacre", "food", "monster", "evil", "run",
                        "die", "lose", "dumb", "idiot", "fool", "crazy",
                        "dinner", "lunch", "breakfast", "fight", "battle",
                        "doomed", "rip apart", "tear apart", "smash",
                        "edible?", "shred", "disembowel", "ugly", "smelly",
                        "stupid", "hideous", "smell", "tasty", "invader",
                        "attack", "raid", "plunder", "pillage", "treasure",
                        "loser", "lose", "scum"
                    };

                    m_OrcSpeech.Syllables = new string[]
                    {
                        "bu", "du", "fu", "ju", "gu",
                        "ulg", "gug", "gub", "gur", "oog",
                        "gub", "log", "ru", "stu", "glu",
                        "ug", "ud", "og", "log", "ro", "flu",
                        "bo", "duf", "fun", "nog", "dun", "bog",
                        "dug", "gh", "ghu", "gho", "nug", "ig",
                        "igh", "ihg", "luh", "duh", "bug", "dug",
                        "dru", "urd", "gurt", "grut", "grunt",
                        "snarf", "urgle", "igg", "glu", "glug",
                        "foo", "bar", "baz", "ghat", "ab", "ad",
                        "gugh", "guk", "ag", "alm", "thu", "log",
                        "bilge", "augh", "gha", "gig", "goth",
                        "zug", "pig", "auh", "gan", "azh", "bag",
                        "hig", "oth", "dagh", "gulg", "ugh", "ba",
                        "bid", "gug", "bug", "rug", "hat", "brui",
                        "gagh", "buad", "buil", "buim", "bum",
                        "hug", "hug", "buo", "ma", "buor", "ghed",
                        "buu", "ca", "guk", "clog", "thurg", "car",
                        "cro", "thu", "da", "cuk", "gil", "cur", "dak",
                        "dar", "deak", "der", "dil", "dit", "at", "ag",
                        "dor", "gar", "dre", "tk", "dri", "gka", "rim",
                        "eag", "egg", "ha", "rod", "eg", "lat", "eichel",
                        "ek", "ep", "ka", "it", "ut", "ewk", "ba", "dagh",
                        "faugh", "foz", "fog", "fid", "fruk", "gag", "fub",
                        "fud", "fur", "bog", "fup", "hagh", "gaa", "kt",
                        "rekk", "lub", "lug", "tug", "gna", "urg", "l",
                        "gno", "gnu", "gol", "gom", "kug", "ukk", "jak",
                        "jek", "rukk", "jja", "akt", "nuk", "hok", "hrol",
                        "olm", "natz", "i", "i", "o", "u", "ikk", "ign",
                        "juk", "kh", "kgh", "ka", "hig", "ke", "ki", "klap",
                        "klu", "knod", "kod", "knu", "thnu", "krug", "nug",
                        "nar", "nag", "neg", "neh", "oag", "ob", "ogh", "oh",
                        "om", "dud", "oo", "pa", "hrak", "qo", "quad", "quil",
                        "ghig", "rur", "sag", "sah", "sg"
                    };
                }

                return m_OrcSpeech;
            }
        }
        public static InhumanSpeech Lizardman
        {
            get
            {
                if (m_LizardmanSpeech == null)
                {
                    m_LizardmanSpeech = new InhumanSpeech();

                    m_LizardmanSpeech.Hue = 58;
                    m_LizardmanSpeech.Sound = 418;

                    m_LizardmanSpeech.Flags = IHSFlags.All;

                    m_LizardmanSpeech.Keywords = new string[]
                    {
                        "meat", "gold", "kill", "killing", "slay",
                        "sword", "axe", "spell", "magic", "spells",
                        "swords", "axes", "mace", "maces", "monster",
                        "monsters", "food", "run", "escape", "away",
                        "help", "dead", "die", "dying", "lose",
                        "losing", "life", "lives", "death", "ghost",
                        "ghosts", "british", "blackthorn", "guild",
                        "guilds", "dragon", "dragons", "game", "games",
                        "ultima", "silly", "stupid", "dumb", "idiot",
                        "idiots", "cheesy", "cheezy", "crazy", "dork",
                        "jerk", "fool", "foolish", "ugly", "insult", "scum"
                    };

                    m_LizardmanSpeech.Responses = new string[]
                    {
                        "meat", "kill", "pound", "crush", "yum yum",
                        "crunch", "destroy", "murder", "eat", "munch",
                        "massacre", "food", "monster", "evil", "run",
                        "die", "lose", "dumb", "idiot", "fool", "crazy",
                        "dinner", "lunch", "breakfast", "fight", "battle",
                        "doomed", "rip apart", "tear apart", "smash",
                        "edible?", "shred", "disembowel", "ugly", "smelly",
                        "stupid", "hideous", "smell", "tasty", "invader",
                        "attack", "raid", "plunder", "pillage", "treasure",
                        "loser", "lose", "scum"
                    };

                    m_LizardmanSpeech.Syllables = new string[]
                    {
                        "ss", "sth", "iss", "is", "ith", "kth",
                        "sith", "this", "its", "sit", "tis", "tsi",
                        "ssi", "sil", "lis", "sis", "lil", "thil",
                        "lith", "sthi", "lish", "shi", "shash", "sal",
                        "miss", "ra", "tha", "thes", "ses", "sas", "las",
                        "les", "sath", "sia", "ais", "isa", "asi", "asth",
                        "stha", "sthi", "isth", "asa", "ath", "tha", "als",
                        "sla", "thth", "ci", "ce", "cy", "yss", "ys", "yth",
                        "syth", "thys", "yts", "syt", "tys", "tsy", "ssy",
                        "syl", "lys", "sys", "lyl", "thyl", "lyth", "sthy",
                        "lysh", "shy", "myss", "ysa", "sthy", "ysth"
                    };
                }

                return m_LizardmanSpeech;
            }
        }
        public static InhumanSpeech Wisp
        {
            get
            {
                if (m_WispSpeech == null)
                {
                    m_WispSpeech = new InhumanSpeech();

                    m_WispSpeech.Hue = 89;
                    m_WispSpeech.Sound = 466;

                    m_WispSpeech.Flags = IHSFlags.OnMovement;

                    m_WispSpeech.Syllables = new string[]
                    {
                        "b", "c", "d", "f", "g", "h", "i",
                        "j", "k", "l", "m", "n", "p", "r",
                        "s", "t", "v", "w", "x", "z", "c",
                        "c", "x", "x", "x", "x", "x", "y",
                        "y", "y", "y", "t", "t", "k", "k",
                        "l", "l", "m", "m", "m", "m", "z"
                    };
                }

                return m_WispSpeech;
            }
        }
        public string[] Syllables
        {
            get
            {
                return this.m_Syllables;
            }
            set
            {
                this.m_Syllables = value;
            }
        }
        public string[] Keywords
        {
            get
            {
                return this.m_Keywords;
            }
            set
            {
                this.m_Keywords = value;
                this.m_KeywordHash = new Dictionary<string, string>(this.m_Keywords.Length, StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < this.m_Keywords.Length; ++i)
                    this.m_KeywordHash[this.m_Keywords[i]] = this.m_Keywords[i];
            }
        }
        public string[] Responses
        {
            get
            {
                return this.m_Responses;
            }
            set
            {
                this.m_Responses = value;
            }
        }
        public int Hue
        {
            get
            {
                return this.m_Hue;
            }
            set
            {
                this.m_Hue = value;
            }
        }
        public int Sound
        {
            get
            {
                return this.m_Sound;
            }
            set
            {
                this.m_Sound = value;
            }
        }
        public IHSFlags Flags
        {
            get
            {
                return this.m_Flags;
            }
            set
            {
                this.m_Flags = value;
            }
        }
        public string GetRandomSyllable()
        {
            return this.m_Syllables[Utility.Random(this.m_Syllables.Length)];
        }

        public string ConstructWord(int syllableCount)
        {
            string[] syllables = new string[syllableCount];

            for (int i = 0; i < syllableCount; ++i)
                syllables[i] = this.GetRandomSyllable();

            return String.Concat(syllables);
        }

        public string ConstructSentance(int wordCount)
        {
            StringBuilder sentance = new StringBuilder();

            bool needUpperCase = true;

            for (int i = 0; i < wordCount; ++i)
            {
                if (i > 0) // not first word )
                {
                    int random = Utility.RandomMinMax(1, 15);

                    if (random < 11)
                    {
                        sentance.Append(' ');
                    }
                    else
                    {
                        needUpperCase = true;

                        if (random > 13)
                            sentance.Append("! ");
                        else
                            sentance.Append(". ");
                    }
                }

                int syllableCount;
                                                       
                if (30 > Utility.Random(100))
                    syllableCount = Utility.Random(1, 5);
                else
                    syllableCount = Utility.Random(1, 3);

                string word = this.ConstructWord(syllableCount);

                sentance.Append(word);

                if (needUpperCase)
                    sentance.Replace(word[0], Char.ToUpper(word[0]), sentance.Length - word.Length, 1);

                needUpperCase = false;
            }

            if (Utility.RandomMinMax(1, 5) == 1)
                sentance.Append('!');
            else
                sentance.Append('.');

            return sentance.ToString();
        }

        public void SayRandomTranslate(Mobile mob, params string[] sentancesInEnglish)
        {
            this.SaySentance(mob, Utility.RandomMinMax(2, 3));
            mob.Say(sentancesInEnglish[Utility.Random(sentancesInEnglish.Length)]);
        }

        public bool OnSpeech(Mobile mob, Mobile speaker, string text)
        {
            if ((this.m_Flags & IHSFlags.OnSpeech) == 0 || this.m_Keywords == null || this.m_Responses == null || this.m_KeywordHash == null)
                return false; // not enabled

            if (!speaker.Alive)
                return false;

            if (!speaker.InRange(mob, 3))
                return false;

            if ((speaker.Direction & Direction.Mask) != speaker.GetDirectionTo(mob))
                return false;

            if ((mob.Direction & Direction.Mask) != mob.GetDirectionTo(speaker))
                return false;

            string[] split = text.Split(' ');
            List<string> keywordsFound = new List<string>();

            for (int i = 0; i < split.Length; ++i)
            {
                string keyword;
                this.m_KeywordHash.TryGetValue(split[i], out keyword);

                if (keyword != null)
                    keywordsFound.Add(keyword);
            }

            if (keywordsFound.Count > 0)
            {
                string responseWord;

                if (Utility.RandomBool())
                    responseWord = this.GetRandomResponseWord(keywordsFound);
                else
                    responseWord = keywordsFound[Utility.Random(keywordsFound.Count)];

                string secondResponseWord = this.GetRandomResponseWord(keywordsFound);

                StringBuilder response = new StringBuilder();

                switch ( Utility.Random(6) )
                {
                    default:
                    case 0:
                        {
                            response.Append("Me ").Append(responseWord).Append('?');
                            break;
                        }
                    case 1:
                        {
                            response.Append(responseWord).Append(" thee!");
                            response.Replace(responseWord[0], Char.ToUpper(responseWord[0]), 0, 1);
                            break;
                        }
                    case 2:
                        {
                            response.Append(responseWord).Append('?');
                            response.Replace(responseWord[0], Char.ToUpper(responseWord[0]), 0, 1);
                            break;
                        }
                    case 3:
                        {
                            response.Append(responseWord).Append("! ").Append(secondResponseWord).Append('.');
                            response.Replace(responseWord[0], Char.ToUpper(responseWord[0]), 0, 1);
                            response.Replace(secondResponseWord[0], Char.ToUpper(secondResponseWord[0]), responseWord.Length + 2, 1);
                            break;
                        }
                    case 4:
                        {
                            response.Append(responseWord).Append('.');
                            response.Replace(responseWord[0], Char.ToUpper(responseWord[0]), 0, 1);
                            break;
                        }
                    case 5:
                        {
                            response.Append(responseWord).Append("? ").Append(secondResponseWord).Append('.');
                            response.Replace(responseWord[0], Char.ToUpper(responseWord[0]), 0, 1);
                            response.Replace(secondResponseWord[0], Char.ToUpper(secondResponseWord[0]), responseWord.Length + 2, 1);
                            break;
                        }
                }

                int maxWords = (split.Length / 2) + 1;

                if (maxWords < 2)
                    maxWords = 2;
                else if (maxWords > 6)
                    maxWords = 6;

                this.SaySentance(mob, Utility.RandomMinMax(2, maxWords));
                mob.Say(response.ToString());

                return true;
            }

            return false;
        }

        public void OnDeath(Mobile mob)
        {
            if ((this.m_Flags & IHSFlags.OnDeath) == 0)
                return; // not enabled

            if (90 > Utility.Random(100))
                return; // 90% chance to do nothing; 10% chance to talk

            this.SayRandomTranslate(mob,
                "Revenge!",
                "NOOooo!",
                "I... I...",
                "Me no die!",
                "Me die!",
                "Must... not die...",
                "Oooh, me hurt...",
                "Me dying?");
        }

        public void OnMovement(Mobile mob, Mobile mover, Point3D oldLocation)
        {
            if ((this.m_Flags & IHSFlags.OnMovement) == 0)
                return; // not enabled

            if (!mover.Player || (mover.Hidden && mover.IsStaff()))
                return;

            if (!mob.InRange(mover, 5) || mob.InRange(oldLocation, 5))
                return; // only talk when they enter 5 tile range

            if (90 > Utility.Random(100))
                return; // 90% chance to do nothing; 10% chance to talk

            this.SaySentance(mob, 6);
        }

        public void OnDamage(Mobile mob, int amount)
        {
            if ((this.m_Flags & IHSFlags.OnDamaged) == 0)
                return; // not enabled

            if (90 > Utility.Random(100))
                return; // 90% chance to do nothing; 10% chance to talk

            if (amount < 5)
            {
                this.SayRandomTranslate(mob,
                    "Ouch!",
                    "Me not hurt bad!",
                    "Thou fight bad.",
                    "Thy blows soft!",
                    "You bad with weapon!");
            }
            else
            {
                this.SayRandomTranslate(mob,
                    "Ouch! Me hurt!",
                    "No, kill me not!",
                    "Me hurt!",
                    "Away with thee!",
                    "Oof! That hurt!",
                    "Aaah! That hurt...",
                    "Good blow!");
            }
        }

        public void OnConstruct(Mobile mob)
        {
            mob.SpeechHue = this.m_Hue;
        }

        public void SaySentance(Mobile mob, int wordCount)
        {
            mob.Say(this.ConstructSentance(wordCount));
            mob.PlaySound(this.m_Sound);
        }

        private string GetRandomResponseWord(List<string> keywordsFound)
        {
            int random = Utility.Random(keywordsFound.Count + this.m_Responses.Length);

            if (random < keywordsFound.Count)
                return keywordsFound[random];

            return this.m_Responses[random - keywordsFound.Count];
        }
    }
}