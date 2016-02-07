using System;
using System.Collections.Generic;
using System.IO;
using Server.Accounting;
using Server.Commands;

namespace Server.Misc
{
    /**
    * This file requires to be saved in a Unicode
    * compatible format.
    * 
    * Warning: if you change String.Format methods,
    * please note that the following character
    * is suggested before any left-to-right text
    * in order to prevent undesired formatting
    * resulting from mixing LR and RL text: ‎
    * 
    * Use this one if you need to force RL: ‏
    * 
    * If you do not see the above chars, please
    * enable showing of unicode control chars
    **/
    public class LanguageStatistics
    {
        private static readonly InternationalCode[] InternationalCodes =
        {
            new InternationalCode("ARA", "Arabic", "Saudi Arabia", "العربية", "السعودية"),
            new InternationalCode("ARI", "Arabic", "Iraq", "العربية", "العراق"),
            new InternationalCode("ARE", "Arabic", "Egypt", "العربية", "مصر"),
            new InternationalCode("ARL", "Arabic", "Libya", "العربية", "ليبيا"),
            new InternationalCode("ARG", "Arabic", "Algeria", "العربية", "الجزائر"),
            new InternationalCode("ARM", "Arabic", "Morocco", "العربية", "المغرب"),
            new InternationalCode("ART", "Arabic", "Tunisia", "العربية", "تونس"),
            new InternationalCode("ARO", "Arabic", "Oman", "العربية", "عمان"),
            new InternationalCode("ARY", "Arabic", "Yemen", "العربية", "اليمن"),
            new InternationalCode("ARS", "Arabic", "Syria", "العربية", "سورية"),
            new InternationalCode("ARJ", "Arabic", "Jordan", "العربية", "الأردن"),
            new InternationalCode("ARB", "Arabic", "Lebanon", "العربية", "لبنان"),
            new InternationalCode("ARK", "Arabic", "Kuwait", "العربية", "الكويت"),
            new InternationalCode("ARU", "Arabic", "U.A.E.", "العربية", "الامارات"),
            new InternationalCode("ARH", "Arabic", "Bahrain", "العربية", "البحرين"),
            new InternationalCode("ARQ", "Arabic", "Qatar", "العربية", "قطر"),
            new InternationalCode("BGR", "Bulgarian", "Bulgaria", "Български", "България"),
            new InternationalCode("CAT", "Catalan", "Spain", "Català", "Espanya"),
            new InternationalCode("CHT", "Chinese", "Taiwan", "台語", "臺灣"),
            new InternationalCode("CHS", "Chinese", "PRC", "中文", "中国"),
            new InternationalCode("ZHH", "Chinese", "Hong Kong", "中文", "香港"),
            new InternationalCode("ZHI", "Chinese", "Singapore", "中文", "新加坡"),
            new InternationalCode("ZHM", "Chinese", "Macau", "中文", "澳門"),
            new InternationalCode("CSY", "Czech", "Czech Republic", "Čeština", "Česká republika"),
            new InternationalCode("DAN", "Danish", "Denmark", "Dansk", "Danmark"),
            new InternationalCode("DEU", "German", "Germany", "Deutsch", "Deutschland"),
            new InternationalCode("DES", "German", "Switzerland", "Deutsch", "der Schweiz"),
            new InternationalCode("DEA", "German", "Austria", "Deutsch", "Österreich"),
            new InternationalCode("DEL", "German", "Luxembourg", "Deutsch", "Luxembourg"),
            new InternationalCode("DEC", "German", "Liechtenstein", "Deutsch", "Liechtenstein"),
            new InternationalCode("ELL", "Greek", "Greece", "Ελληνικά", "Ελλάδα"),
            new InternationalCode("ENU", "English", "United States"),
            new InternationalCode("ENG", "English", "United Kingdom"),
            new InternationalCode("ENA", "English", "Australia"),
            new InternationalCode("ENC", "English", "Canada"),
            new InternationalCode("ENZ", "English", "New Zealand"),
            new InternationalCode("ENI", "English", "Ireland"),
            new InternationalCode("ENS", "English", "South Africa"),
            new InternationalCode("ENJ", "English", "Jamaica"),
            new InternationalCode("ENB", "English", "Caribbean"),
            new InternationalCode("ENL", "English", "Belize"),
            new InternationalCode("ENT", "English", "Trinidad"),
            new InternationalCode("ENW", "English", "Zimbabwe"),
            new InternationalCode("ENP", "English", "Philippines"),
            new InternationalCode("ESP", "Spanish", "Spain (Traditional Sort)", "Español", "España (tipo tradicional)"),
            new InternationalCode("ESM", "Spanish", "Mexico", "Español", "México"),
            new InternationalCode("ESN", "Spanish", "Spain (International Sort)", "Español", "España (tipo internacional)"),
            new InternationalCode("ESG", "Spanish", "Guatemala", "Español", "Guatemala"),
            new InternationalCode("ESC", "Spanish", "Costa Rica", "Español", "Costa Rica"),
            new InternationalCode("ESA", "Spanish", "Panama", "Español", "Panama"),
            new InternationalCode("ESD", "Spanish", "Dominican Republic", "Español", "Republica Dominicana"),
            new InternationalCode("ESV", "Spanish", "Venezuela", "Español", "Venezuela"),
            new InternationalCode("ESO", "Spanish", "Colombia", "Español", "Colombia"),
            new InternationalCode("ESR", "Spanish", "Peru", "Español", "Peru"),
            new InternationalCode("ESS", "Spanish", "Argentina", "Español", "Argentina"),
            new InternationalCode("ESF", "Spanish", "Ecuador", "Español", "Ecuador"),
            new InternationalCode("ESL", "Spanish", "Chile", "Español", "Chile"),
            new InternationalCode("ESY", "Spanish", "Uruguay", "Español", "Uruguay"),
            new InternationalCode("ESZ", "Spanish", "Paraguay", "Español", "Paraguay"),
            new InternationalCode("ESB", "Spanish", "Bolivia", "Español", "Bolivia"),
            new InternationalCode("ESE", "Spanish", "El Salvador", "Español", "El Salvador"),
            new InternationalCode("ESH", "Spanish", "Honduras", "Español", "Honduras"),
            new InternationalCode("ESI", "Spanish", "Nicaragua", "Español", "Nicaragua"),
            new InternationalCode("ESU", "Spanish", "Puerto Rico", "Español", "Puerto Rico"),
            new InternationalCode("FIN", "Finnish", "Finland", "Suomi", "Suomi"),
            new InternationalCode("FRA", "French", "France", "Français", "France"),
            new InternationalCode("FRB", "French", "Belgium", "Français", "Belgique"),
            new InternationalCode("FRC", "French", "Canada", "Français", "Canada"),
            new InternationalCode("FRS", "French", "Switzerland", "Français", "Suisse"),
            new InternationalCode("FRL", "French", "Luxembourg", "Français", "Luxembourg"),
            new InternationalCode("FRM", "French", "Monaco", "Français", "Monaco"),
            new InternationalCode("HEB", "Hebrew", "Israel", "עִבְרִית", "ישׂראל"),
            new InternationalCode("HUN", "Hungarian", "Hungary", "Magyar", "Magyarország"),
            new InternationalCode("ISL", "Icelandic", "Iceland", "Íslenska", "Ísland"),
            new InternationalCode("ITA", "Italian", "Italy", "Italiano", "Italia"),
            new InternationalCode("ITS", "Italian", "Switzerland", "Italiano", "Svizzera"),
            new InternationalCode("JPN", "Japanese", "Japan", "日本語", "日本"),
            new InternationalCode("KOR", "Korean (Extended Wansung)", "Korea", "한국어", "한국"),
            new InternationalCode("NLD", "Dutch", "Netherlands", "Nederlands", "Nederland"),
            new InternationalCode("NLB", "Dutch", "Belgium", "Nederlands", "België"),
            new InternationalCode("NOR", "Norwegian", "Norway (Bokmål)", "Norsk", "Norge (Bokmål)"),
            new InternationalCode("NON", "Norwegian", "Norway (Nynorsk)", "Norsk", "Norge (Nynorsk)"),
            new InternationalCode("PLK", "Polish", "Poland", "Polski", "Polska"),
            new InternationalCode("PTB", "Portuguese", "Brazil", "Português", "Brasil"),
            new InternationalCode("PTG", "Portuguese", "Portugal", "Português", "Brasil"),
            new InternationalCode("ROM", "Romanian", "Romania", "Limba Română", "România"),
            new InternationalCode("RUS", "Russian", "Russia", "Русский", "Россия"),
            new InternationalCode("HRV", "Croatian", "Croatia", "Hrvatski", "Hrvatska"),
            new InternationalCode("SRL", "Serbian", "Serbia (Latin)", "Srpski", "Srbija i Crna Gora"),
            new InternationalCode("SRB", "Serbian", "Serbia (Cyrillic)", "Српски", "Србија и Црна Гора"),
            new InternationalCode("SKY", "Slovak", "Slovakia", "Slovenčina", "Slovensko"),
            new InternationalCode("SQI", "Albanian", "Albania", "Shqip", "Shqipëria"),
            new InternationalCode("SVE", "Swedish", "Sweden", "Svenska", "Sverige"),
            new InternationalCode("SVF", "Swedish", "Finland", "Svenska", "Finland"),
            new InternationalCode("THA", "Thai", "Thailand", "ภาษาไทย", "ประเทศไทย"),
            new InternationalCode("TRK", "Turkish", "Turkey", "Türkçe", "Türkiye"),
            new InternationalCode("URP", "Urdu", "Pakistan", "اردو", "پاکستان"),
            new InternationalCode("IND", "Indonesian", "Indonesia", "Bahasa Indonesia", "Indonesia"),
            new InternationalCode("UKR", "Ukrainian", "Ukraine", "Українська", "Украина"),
            new InternationalCode("BEL", "Belarusian", "Belarus", "Беларускі", "Беларусь"),
            new InternationalCode("SLV", "Slovene", "Slovenia", "Slovenščina", "Slovenija"),
            new InternationalCode("ETI", "Estonian", "Estonia", "Eesti", "Eesti"),
            new InternationalCode("LVI", "Latvian", "Latvia", "Latviešu", "Latvija"),
            new InternationalCode("LTH", "Lithuanian", "Lithuania", "Lietuvių", "Lietuva"),
            new InternationalCode("LTC", "Classic Lithuanian", "Lithuania", "Lietuviškai", "Lietuva"),
            new InternationalCode("FAR", "Farsi", "Iran", "فارسى", "ايران"),
            new InternationalCode("VIT", "Vietnamese", "Viet Nam", "tiếng Việt", "Việt Nam"),
            new InternationalCode("HYE", "Armenian", "Armenia", "Հայերէն", "Հայաստան"),
            new InternationalCode("AZE", "Azeri", "Azerbaijan (Latin)", "Azərbaycanca", "Azərbaycan"),
            new InternationalCode("AZE", "Azeri", "Azerbaijan (Cyrillic)", "Азәрбајҹанҹа", "Азәрбајҹан"),
            new InternationalCode("EUQ", "Basque", "Spain", "Euskera", "Espainia"),
            new InternationalCode("MKI", "Macedonian", "Macedonia", "Македонски", "Македонија"),
            new InternationalCode("AFK", "Afrikaans", "South Africa", "Afrikaans", "Republiek van Suid-Afrika"),
            new InternationalCode("KAT", "Georgian", "Georgia", "ქართული", "საკარტველო"),
            new InternationalCode("FOS", "Faeroese", "Faeroe Islands", "Føroyska", "Føroya"),
            new InternationalCode("HIN", "Hindi", "India", "हिन्दी", "भारत"),
            new InternationalCode("MSL", "Malay", "Malaysia", "Bahasa melayu", "Malaysia"),
            new InternationalCode("MSB", "Malay", "Brunei Darussalam", "Bahasa melayu", "Negara Brunei Darussalam"),
            new InternationalCode("KAZ", "Kazak", "Kazakstan", "Қазақ", "Қазақстан"),
            new InternationalCode("SWK", "Swahili", "Kenya", "Kiswahili", "Kenya"),
            new InternationalCode("UZB", "Uzbek", "Uzbekistan (Latin)", "O'zbek", "O'zbekiston"),
            new InternationalCode("UZB", "Uzbek", "Uzbekistan (Cyrillic)", "Ўзбек", "Ўзбекистон"),
            new InternationalCode("TAT", "Tatar", "Tatarstan", "Татарча", "Татарстан"),
            new InternationalCode("BEN", "Bengali", "India", "বাংলা", "ভারত"),
            new InternationalCode("PAN", "Punjabi", "India", "ਪੰਜਾਬੀ", "ਭਾਰਤ"),
            new InternationalCode("GUJ", "Gujarati", "India", "ગુજરાતી", "ભારત"),
            new InternationalCode("ORI", "Oriya", "India", "ଓଡ଼ିଆ", "ଭାରତ"),
            new InternationalCode("TAM", "Tamil", "India", "தமிழ்", "இந்தியா"),
            new InternationalCode("TEL", "Telugu", "India", "తెలుగు", "భారత"),
            new InternationalCode("KAN", "Kannada", "India", "ಕನ್ನಡ", "ಭಾರತ"),
            new InternationalCode("MAL", "Malayalam", "India", "മലയാളം", "ഭാരത"),
            new InternationalCode("ASM", "Assamese", "India", "অসমিয়া", "Bhārat"), // missing correct country name
            new InternationalCode("MAR", "Marathi", "India", "मराठी", "भारत"),
            new InternationalCode("SAN", "Sanskrit", "India", "संस्कृत", "भारतम्"),
            new InternationalCode("KOK", "Konkani", "India", "कोंकणी", "भारत")
        };
        private static readonly bool DefaultLocalNames = false;
        private static readonly bool ShowAlternatives = true;
        private static readonly bool CountAccounts = true;// will consider only first character's valid language
        public static void Initialize()
        {
            CommandSystem.Register("LanguageStatistics", AccessLevel.Administrator, new CommandEventHandler(LanguageStatistics_OnCommand));
        }

        [Usage("LanguageStatistics")]
        [Description("Generate a file containing the list of languages for each PlayerMobile.")]
        public static void LanguageStatistics_OnCommand(CommandEventArgs e)
        {
            Dictionary<string, InternationalCodeCounter> ht = new Dictionary<string, InternationalCodeCounter>();

            using (StreamWriter writer = new StreamWriter("languages.txt"))
            {
                if (CountAccounts)
                {
                    // count accounts
                    foreach (Account acc in Accounts.GetAccounts())
                    {
                        for (int i = 0; i < acc.Length; i++)
                        {
                            Mobile mob = acc[i];

                            if (mob == null)
                                continue;

                            string lang = mob.Language;

                            if (lang != null)
                            {
                                lang = lang.ToUpper();

                                if (!ht.ContainsKey(lang))
                                    ht[lang] = new InternationalCodeCounter(lang);
                                else
                                    ht[lang].Increase();

                                break;
                            }
                        }
                    }
                }
                else
                {
                    // count playermobiles
                    foreach (Mobile mob in World.Mobiles.Values)
                    {
                        if (mob.Player)
                        {
                            string lang = mob.Language;

                            if (lang != null)
                            {
                                lang = lang.ToUpper();

                                if (!ht.ContainsKey(lang))
                                    ht[lang] = new InternationalCodeCounter(lang);
                                else
                                    ht[lang].Increase();
                            }
                        }
                    }
                }

                writer.WriteLine(String.Format("Language statistics. Numbers show how many {0} use the specified language.", CountAccounts ? "accounts" : "playermobile"));
                writer.WriteLine("====================================================================================================");
                writer.WriteLine();

                // sort the list
                List<InternationalCodeCounter> list = new List<InternationalCodeCounter>(ht.Values);
                list.Sort(InternationalCodeComparer.Instance);

                foreach (InternationalCodeCounter c in list)
                    writer.WriteLine(String.Format("{0}‎ : {1}", GetFormattedInfo(c.Code), c.Count));

                e.Mobile.SendMessage("Languages list generated.");
            }
        }

        private static string GetFormattedInfo(string code)
        {
            if (code == null || code.Length != 3)
                return String.Format("Unknown code {0}", code);

            for (int i = 0; i < InternationalCodes.Length; i++)
            {
                if (code == InternationalCodes[i].Code)
                {
                    return String.Format("{0}", InternationalCodes[i].GetName());
                }
            }

            return String.Format("Unknown code {0}", code);
        }

        struct InternationalCode
        {
            readonly string m_Code;
            readonly string m_Language;
            readonly string m_Country;
            readonly string m_Language_LocalName;
            readonly string m_Country_LocalName;
            readonly bool m_HasLocalInfo;
            public InternationalCode(string code, string language, string country)
                : this(code, language, country, null, null)
            {
                this.m_HasLocalInfo = false;
            }

            public InternationalCode(string code, string language, string country, string language_localname, string country_localname)
            {
                this.m_Code = code;
                this.m_Language = language;
                this.m_Country = country;
                this.m_Language_LocalName = language_localname;
                this.m_Country_LocalName = country_localname;
                this.m_HasLocalInfo = true;
            }

            public string Code
            {
                get
                {
                    return this.m_Code;
                }
            }
            public string Language
            {
                get
                {
                    return this.m_Language;
                }
            }
            public string Country
            {
                get
                {
                    return this.m_Country;
                }
            }
            public string Language_LocalName
            {
                get
                {
                    return this.m_Language_LocalName;
                }
            }
            public string Country_LocalName
            {
                get
                {
                    return this.m_Country_LocalName;
                }
            }
            public string GetName()
            {
                string s;

                if (this.m_HasLocalInfo)
                {
                    s = String.Format("{0}‎ - {1}", DefaultLocalNames ? this.m_Language_LocalName : this.m_Language, DefaultLocalNames ? this.m_Country_LocalName : this.m_Country);

                    if (ShowAlternatives)
                        s += String.Format("‎ 【{0}‎ - {1}‎】", DefaultLocalNames ? this.m_Language : this.m_Language_LocalName, DefaultLocalNames ? this.m_Country : this.m_Country_LocalName);
                }
                else
                {
                    s = String.Format("{0}‎ - {1}", this.m_Language, this.m_Country);
                }

                return s;
            }
        }

        private class InternationalCodeCounter
        {
            private readonly string m_Code;
            private int m_Count;
            public InternationalCodeCounter(string code)
            {
                this.m_Code = code;
                this.m_Count = 1;
            }

            public string Code
            {
                get
                {
                    return this.m_Code;
                }
            }
            public int Count
            {
                get
                {
                    return this.m_Count;
                }
            }
            public void Increase()
            {
                this.m_Count++;
            }
        }

        private class InternationalCodeComparer : IComparer<InternationalCodeCounter>
        {
            public static readonly InternationalCodeComparer Instance = new InternationalCodeComparer();
            public InternationalCodeComparer()
            {
            }

            public int Compare(InternationalCodeCounter x, InternationalCodeCounter y)
            {
                string a = null, b = null;
                int ca = 0, cb = 0;

                a = x.Code;
                ca = x.Count;
                b = y.Code;
                cb = y.Count;

                if (ca > cb)
                    return -1;

                if (ca < cb)
                    return 1;

                if (a == null && b == null)
                    return 0;

                if (a == null)
                    return 1;

                if (b == null)
                    return -1;

                return a.CompareTo(b);
            }
        }
    }
}