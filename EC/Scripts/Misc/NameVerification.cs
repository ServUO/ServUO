using System;
using Server.Commands;

namespace Server.Misc
{
    public class NameVerification
    {
        public static readonly char[] SpaceDashPeriodQuote = new char[]
        {
            ' ', '-', '.', '\''
        };
        public static readonly char[] Empty = new char[0];
        private static readonly string[] m_StartDisallowed = new string[]
        {
            "seer",
            "counselor",
            "gm",
            "admin",
            "lady",
            "lord"
        };
        private static readonly string[] m_Disallowed = new string[]
        {
            "jigaboo",
            "chigaboo",
            "wop",
            "kyke",
            "kike",
            "tit",
            "spic",
            "prick",
            "piss",
            "lezbo",
            "lesbo",
            "felatio",
            "dyke",
            "dildo",
            "chinc",
            "chink",
            "cunnilingus",
            "cum",
            "cocksucker",
            "cock",
            "clitoris",
            "clit",
            "ass",
            "hitler",
            "penis",
            "nigga",
            "nigger",
            "klit",
            "kunt",
            "jiz",
            "jism",
            "jerkoff",
            "jackoff",
            "goddamn",
            "fag",
            "blowjob",
            "bitch",
            "asshole",
            "dick",
            "pussy",
            "snatch",
            "cunt",
            "twat",
            "shit",
            "fuck",
            "tailor",
            "smith",
            "scholar",
            "rogue",
            "novice",
            "neophyte",
            "merchant",
            "medium",
            "master",
            "mage",
            "lb",
            "journeyman",
            "grandmaster",
            "fisherman",
            "expert",
            "chef",
            "carpenter",
            "british",
            "blackthorne",
            "blackthorn",
            "beggar",
            "archer",
            "apprentice",
            "adept",
            "gamemaster",
            "frozen",
            "squelched",
            "invulnerable",
            "osi",
            "origin"
        };
        public static string[] StartDisallowed
        {
            get
            {
                return m_StartDisallowed;
            }
        }
        public static string[] Disallowed
        {
            get
            {
                return m_Disallowed;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("ValidateName", AccessLevel.Administrator, new CommandEventHandler(ValidateName_OnCommand));
        }

        [Usage("ValidateName")]
        [Description("Checks the result of NameValidation on the specified name.")]
        public static void ValidateName_OnCommand(CommandEventArgs e)
        {
            if (Validate(e.ArgString, 2, 16, true, false, true, 1, SpaceDashPeriodQuote))
                e.Mobile.SendMessage(0x59, "That name is considered valid.");
            else
                e.Mobile.SendMessage(0x22, "That name is considered invalid.");
        }

        public static bool Validate(string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions)
        {
            return Validate(name, minLength, maxLength, allowLetters, allowDigits, noExceptionsAtStart, maxExceptions, exceptions, m_Disallowed, m_StartDisallowed);
        }

        public static bool Validate(string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions, string[] disallowed, string[] startDisallowed)
        {
            if (name == null || name.Length < minLength || name.Length > maxLength)
                return false;

            int exceptCount = 0;

            name = name.ToLower();

            if (!allowLetters || !allowDigits || (exceptions.Length > 0 && (noExceptionsAtStart || maxExceptions < int.MaxValue)))
            {
                for (int i = 0; i < name.Length; ++i)
                {
                    char c = name[i];

                    if (c >= 'a' && c <= 'z')
                    {
                        if (!allowLetters)
                            return false;

                        exceptCount = 0;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        if (!allowDigits)
                            return false;

                        exceptCount = 0;
                    }
                    else
                    {
                        bool except = false;

                        for (int j = 0; !except && j < exceptions.Length; ++j)
                            if (c == exceptions[j])
                                except = true;

                        if (!except || (i == 0 && noExceptionsAtStart))
                            return false;

                        if (exceptCount++ == maxExceptions)
                            return false;
                    }
                }
            }

            for (int i = 0; i < disallowed.Length; ++i)
            {
                int indexOf = name.IndexOf(disallowed[i]);

                if (indexOf == -1)
                    continue;

                bool badPrefix = (indexOf == 0);

                for (int j = 0; !badPrefix && j < exceptions.Length; ++j)
                    badPrefix = (name[indexOf - 1] == exceptions[j]);

                if (!badPrefix)
                    continue;

                bool badSuffix = ((indexOf + disallowed[i].Length) >= name.Length);

                for (int j = 0; !badSuffix && j < exceptions.Length; ++j)
                    badSuffix = (name[indexOf + disallowed[i].Length] == exceptions[j]);

                if (badSuffix)
                    return false;
            }

            for (int i = 0; i < startDisallowed.Length; ++i)
            {
                if (name.StartsWith(startDisallowed[i]))
                    return false;
            }

            return true;
        }
    }
}