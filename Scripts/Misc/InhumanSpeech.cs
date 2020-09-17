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

        public static InhumanSpeech Ratman
        {
            get
            {
                if (m_RatmanSpeech == null)
                {
                    m_RatmanSpeech = new InhumanSpeech
                    {
                        Hue = 149,
                        Sound = 438,

                        Flags = IHSFlags.All,

                        Keywords = new[]
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
                    },

                        Responses = new[]
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
                    },

                        Syllables = new[]
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
                    }
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
                    m_OrcSpeech = new InhumanSpeech
                    {
                        Hue = 34,
                        Sound = 432,

                        Flags = IHSFlags.All,

                        Keywords = new[]
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
                    },

                        Responses = new[]
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
                    },

                        Syllables = new[]
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
                    }
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
                    m_LizardmanSpeech = new InhumanSpeech
                    {
                        Hue = 58,
                        Sound = 418,

                        Flags = IHSFlags.All,

                        Keywords = new[]
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
                    },

                        Responses = new[]
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
                    },

                        Syllables = new[]
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
                    }
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
                    m_WispSpeech = new InhumanSpeech
                    {
                        Hue = 89,
                        Sound = 466,

                        Flags = IHSFlags.OnMovement,

                        Syllables = new[]
                    {
                        "b", "c", "d", "f", "g", "h", "i",
                        "j", "k", "l", "m", "n", "p", "r",
                        "s", "t", "v", "w", "x", "z", "c",
                        "c", "x", "x", "x", "x", "x", "y",
                        "y", "y", "y", "t", "t", "k", "k",
                        "l", "l", "m", "m", "m", "m", "z"
                    }
                    };
                }

                return m_WispSpeech;
            }
        }
        public string[] Syllables
        {
            get
            {
                return m_Syllables;
            }
            set
            {
                m_Syllables = value;
            }
        }
        public string[] Keywords
        {
            get
            {
                return m_Keywords;
            }
            set
            {
                m_Keywords = value;
                m_KeywordHash = new Dictionary<string, string>(m_Keywords.Length, StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < m_Keywords.Length; ++i)
                    m_KeywordHash[m_Keywords[i]] = m_Keywords[i];
            }
        }
        public string[] Responses
        {
            get
            {
                return m_Responses;
            }
            set
            {
                m_Responses = value;
            }
        }
        public int Hue
        {
            get
            {
                return m_Hue;
            }
            set
            {
                m_Hue = value;
            }
        }
        public int Sound
        {
            get
            {
                return m_Sound;
            }
            set
            {
                m_Sound = value;
            }
        }
        public IHSFlags Flags
        {
            get
            {
                return m_Flags;
            }
            set
            {
                m_Flags = value;
            }
        }
        public string GetRandomSyllable()
        {
            return m_Syllables[Utility.Random(m_Syllables.Length)];
        }

        public string ConstructWord(int syllableCount)
        {
            string[] syllables = new string[syllableCount];

            for (int i = 0; i < syllableCount; ++i)
                syllables[i] = GetRandomSyllable();

            return string.Concat(syllables);
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

                string word = ConstructWord(syllableCount);

                sentance.Append(word);

                if (needUpperCase)
                    sentance.Replace(word[0], char.ToUpper(word[0]), sentance.Length - word.Length, 1);

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
            SaySentance(mob, Utility.RandomMinMax(2, 3));
            mob.Say(sentancesInEnglish[Utility.Random(sentancesInEnglish.Length)]);
        }

        public bool OnSpeech(Mobile mob, Mobile speaker, string text)
        {
            if ((m_Flags & IHSFlags.OnSpeech) == 0 || m_Keywords == null || m_Responses == null || m_KeywordHash == null)
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
                m_KeywordHash.TryGetValue(split[i], out keyword);

                if (keyword != null)
                    keywordsFound.Add(keyword);
            }

            if (keywordsFound.Count > 0)
            {
                string responseWord;

                if (Utility.RandomBool())
                    responseWord = GetRandomResponseWord(keywordsFound);
                else
                    responseWord = keywordsFound[Utility.Random(keywordsFound.Count)];

                string secondResponseWord = GetRandomResponseWord(keywordsFound);

                StringBuilder response = new StringBuilder();

                switch (Utility.Random(6))
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
                            response.Replace(responseWord[0], char.ToUpper(responseWord[0]), 0, 1);
                            break;
                        }
                    case 2:
                        {
                            response.Append(responseWord).Append('?');
                            response.Replace(responseWord[0], char.ToUpper(responseWord[0]), 0, 1);
                            break;
                        }
                    case 3:
                        {
                            response.Append(responseWord).Append("! ").Append(secondResponseWord).Append('.');
                            response.Replace(responseWord[0], char.ToUpper(responseWord[0]), 0, 1);
                            response.Replace(secondResponseWord[0], char.ToUpper(secondResponseWord[0]), responseWord.Length + 2, 1);
                            break;
                        }
                    case 4:
                        {
                            response.Append(responseWord).Append('.');
                            response.Replace(responseWord[0], char.ToUpper(responseWord[0]), 0, 1);
                            break;
                        }
                    case 5:
                        {
                            response.Append(responseWord).Append("? ").Append(secondResponseWord).Append('.');
                            response.Replace(responseWord[0], char.ToUpper(responseWord[0]), 0, 1);
                            response.Replace(secondResponseWord[0], char.ToUpper(secondResponseWord[0]), responseWord.Length + 2, 1);
                            break;
                        }
                }

                int maxWords = (split.Length / 2) + 1;

                if (maxWords < 2)
                    maxWords = 2;
                else if (maxWords > 6)
                    maxWords = 6;

                SaySentance(mob, Utility.RandomMinMax(2, maxWords));
                mob.Say(response.ToString());

                return true;
            }

            return false;
        }

        public void OnDeath(Mobile mob)
        {
            if ((m_Flags & IHSFlags.OnDeath) == 0)
                return; // not enabled

            if (90 > Utility.Random(100))
                return; // 90% chance to do nothing; 10% chance to talk

            SayRandomTranslate(mob,
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
            if ((m_Flags & IHSFlags.OnMovement) == 0)
                return; // not enabled

            if (!mover.Player || (mover.Hidden && mover.IsStaff()))
                return;

            if (!mob.InRange(mover, 5) || mob.InRange(oldLocation, 5))
                return; // only talk when they enter 5 tile range

            if (90 > Utility.Random(100))
                return; // 90% chance to do nothing; 10% chance to talk

            SaySentance(mob, 6);
        }

        public void OnDamage(Mobile mob, int amount)
        {
            if ((m_Flags & IHSFlags.OnDamaged) == 0)
                return; // not enabled

            if (90 > Utility.Random(100))
                return; // 90% chance to do nothing; 10% chance to talk

            if (amount < 5)
            {
                SayRandomTranslate(mob,
                    "Ouch!",
                    "Me not hurt bad!",
                    "Thou fight bad.",
                    "Thy blows soft!",
                    "You bad with weapon!");
            }
            else
            {
                SayRandomTranslate(mob,
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
            mob.SpeechHue = m_Hue;
        }

        public void SaySentance(Mobile mob, int wordCount)
        {
            mob.Say(ConstructSentance(wordCount));
            mob.PlaySound(m_Sound);
        }

        private string GetRandomResponseWord(List<string> keywordsFound)
        {
            int random = Utility.Random(keywordsFound.Count + m_Responses.Length);

            if (random < keywordsFound.Count)
                return keywordsFound[random];

            return m_Responses[random - keywordsFound.Count];
        }
    }
}
