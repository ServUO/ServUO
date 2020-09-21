using System.Linq;

namespace Server.Items
{
    public class TerMurQuestRewardBook : BaseBook
    {
        public enum RewardBookEdition
        {
            None = -1,
            Collectors,
            Limited,
            First
        }

        public enum RewardBookType
        {
            None = -1,
            AliceInWonderland,
            LoreOfGargoyles,
            BookOfBaking,
            CrossbowMarksmanship,
            GoodAdvice,
            RaisingAdventure,
            KnightsOfLegendI,
            KodeksBenmontas,
            KodeksBenommani,
            KodeksDestermur,
            KodeksKir,
            KodeksRit,
            KodeksXen,
            LogbookOfTheEmpire,
            OfDreamsAndVisions,
            HotAirBalloonConstruction,
            PlantLore,
            BoardgameStrategy,
            TanglesTales,
            BookOfAdministration,
            //BookOfCircles,
            BookOfFamily,
            BookOfProsperity,
            BookOfRitual,
            BookOfSpirituality,
            BookOfTheUnderworld,
            CavernOfFreitag,
            CodexOfInfiniteWisdom,
            FirstAgeOfDarkness,
            LostBookOfMantras,
            QuestOfTheAvatar,
            SecondAgeOfDarkness,
            WizardOfOz,
            WarriorsOfDestiny,
            Windwalker,
            LostArtOfBallooning
        }

        private RewardBookType _BookType = RewardBookType.None;
        private RewardBookEdition _Edition;

        [CommandProperty(AccessLevel.GameMaster)]
        public RewardBookType BookType
        {
            get { return _BookType; }
            set
            {
                RewardBookType old = _BookType;

                if (old != value && value >= 0 && (int)value < 36)
                {
                    _BookType = value;

                    Title = BookContents[(int)_BookType][0];
                    Author = BookContents[(int)_BookType][1];
                    BookString = BookContents[(int)_BookType][2];

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public RewardBookEdition Edition
        {
            get { return _Edition; }
            set
            {
                _Edition = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public TerMurQuestRewardBook()
            : this(RandomType())
        {
        }

        public TerMurQuestRewardBook(RewardBookType type)
            : base(0xFF2, 20, false)
        {
            BookType = type;
            _Edition = RandomEdition();
        }

        public static RewardBookType RandomType()
        {
            System.Collections.Generic.List<string[]> list = BookContents.Where(strList => !string.IsNullOrEmpty(strList[2])).ToList();

            int ran = Utility.Random(list.Count);
            ColUtility.Free(list);

            return (RewardBookType)ran;
        }

        public RewardBookEdition RandomEdition()
        {
            double ran = Utility.RandomDouble();

            if (ran <= 0.01)
            {
                Hue = 1150;
                return RewardBookEdition.Collectors;
            }

            if (ran <= 0.03)
            {
                Hue = 1254;
                return RewardBookEdition.First;
            }

            if (ran <= 0.1)
            {
                Hue = 427;
                return RewardBookEdition.Limited;
            }

            Hue = Utility.RandomMinMax(200, 600);
            return RewardBookEdition.None;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Edition != RewardBookEdition.None)
            {
                list.Add(1113207 + (int)_Edition);
            }
        }

        public TerMurQuestRewardBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)_Edition);
            writer.Write((int)_BookType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Edition = (RewardBookEdition)reader.ReadInt();
            BookType = (RewardBookType)reader.ReadInt();
        }

        public static string[][] BookContents =
        {
            new string[]
                {
                    "Alice In Wonderland",
                    "Lewis Carroll",
                    "Alice saw a peculiar white rabbit one day. It was looking at its pocket watch and worrying about how late it was. Alice chased it down a rabbit hole, and fell a very long way. She found herself in a strange land. She went to a tea party there, with a mad hatter and a dormouse. She also met a strange caterpillar, and a cat that could vanish, with its grin disappearing last. The queen of hearts yelled 'Off with her head!' and her guards ran up to grab Alice - but then she awakened, and realized it had all been a dream."
                },

            new string[]
                {
                    "A Treatise On The Lore Of Gargoyles",
                    "Norlick the Elder",
                    @"Though gargoyles are considered by most to be mere legend, no records exist documenting the origins of the gargoyle 'statues' that adorn many castles. Even the towering stone guardians of the Codex of Ultimate Wisdom have many of the physical characteristics of the 'legendary' gargoyle. Nobody seems to know where they came from either. Despite the lack of hard evidence, there have been a fair number of unconfirmed reports of sightings of live gargoyles. It is the opinion of this author that daemons are a form of gargoyle. As many reliable encounters with daemons have been documented in various scholarly works, perhaps this is the best source of further information on the subject of gargoyles."
                },

            new string[]
                {
                    "Baldwin's Big Book Of Baking",
                    "Baldwin",
                    "Though some might scoff at the idea, the making of breads, pastries, pies, and cakes is one of the highest callings in life. Study this book carefully, and someday you may be prepared to take on this awesome responsibility.",
                },

            new string[]
                {
                    "Crossbow Marksmanship by Iolo Fitzowen",
                    "Iolo Fitzowen",
                    null,
                },

            new string[]
                {
                    "Dilzal's Almanac Of Good Advice",
                    "Dilzal",
                    null,
                },

            new string[]
                {
                    "Hubert's Hair Raising Adventure",
                    "Bill Peet",
                    "Hubert the Lion was haughty and vain, And especially proud of his elegant mane. But conceit of this sort is not proper at all, And Hubert the Lion was due for a fall.",
                },

            new string[]
                {
                    "Knights of Legend, Volume I",
                    "Unknown",
                    null,
                },

            new string[]
                {
                    "Kodeks Benmontas",
                    "Unknown",
                    null,
                },

            new string[]
                {
                    "Kodeks Benommani",
                    "Unknown",
                    "vas praetim, vas vidlem naksatilor kalle kodeks ante termur. ita rele vastim benommani. kodeks terle ante terort pritas, teresta re vidle pa lem nesde vasuis. vidlem terreg, monle pa naksatilor, juksarkle kodeks ku saeykt grav. sol lem ad omde vestas trak vis canle terpor ew leg kodeks. ante kodeks skrile pri ben ew ver res kui kuae. lem nes sol terpor kodeks, leg lem, ew invislok lemde monuis aptade. ku verinde vis ew ankadsa ski, tu mante est ten un, or, ew us nesle re pos apta via. ista est desintas vide murom, ew ita vide zenmur sa per kodeks visde ew bende. ista est kuauis kodeks est: re mon gargh zenmur trak ultim benommani.",
                },

            new string[]
                {
                    "Kodeks Destermur",
                    "Unknown",
                    null,
                },

            new string[]
                {
                    "Kodeks Kir",
                    "Unknown",
                    "tu rete ku qi askl: un, or, ew us. de un sal traktas. de or sal sent. de us sal tepertas. a ista qi bentas an plu mag de sekde pen: un kuporte ku or re don mistas. or kuporte ku es re don leintas. ew us int ku un re don benintas. anai de un, or, ew us est anord. ita anai de aksi vidukte trak semde bentas, ord. qi aksi priinte re in printas. ista est okde bentas, a lem mistim pri, kuauis kuante pritas sa venle tu aksi, ew ita tu bentas. kir ten an fin. lem teinte tutim, ku tu car parde mag te benfin de tutas. vide murom mis. lem mis teinte tutim, ku tu carzen, ew tu bentas, par car de priinle tutas.",
                },

            new string[]
                {
                    "Kodeks Rit",
                    "Naxatilor",
                    "vervid ben kua i, naksatilor, skri kuo i porle kodeks vide terreg ew estade kalle ante tim benommani: ku auks lorrelinlem i beninle vorteks lorrel, o kua i le vid kodeks kuater lem terinit anporle. i inle vorteks kuad re inbet grav ok orblap ew trakpor vorteks destrak termur. estatim i perle lorrel re invislor kodeks ad kuad. vorteks tanle vide terailem, vislor inle ailemde, ew kodeks porle des re perle bende pa vide zenmur.",
                },

            new string[]
                {
                    "Kodeks Xen",
                    "Unknown",
                    "kuatim betlem grespor de ov, lem inzenle anku vol. a vel de inzen lem sa ski si betlem re invas est volde au anvolde lem. anvolde lem ansa lok, ew anten skitas de volde lem. lem nes dukle. volde lem anmur, a lem kredonle ku skitas ew vis de zenmur. lem nes dukte. sek volde ew anvolde lem sal de mis ov, ew sek dete de mis xen. tu per ve pride tutas, re plu ben inten agratas trak temanitas ante vide termur.",
                },

            new string[]
                {
                    "Logbook Of The Empire",
                    "Captain Hawkins",
                    "This book appears to be the log of a ship called 'The Empire.' The last entry speaks of the burying of a great treasure, and of the growing discontentment of the crew. There's a hastily scrawled note at the end, in different handwriting, that says 'Captain Hawkins won't be makin' no more log entries.'",
                },

            new string[]
                {
                    "Of Dreams And Visions",
                    "Unknown",
                    "Some say that in our dreams our astral selves journey to other realms of existence. Others say that imps and daemons create dreams to disturb our sleep. Now let the truth be known! Dreams are messages from the spirit world. Someday we will learn to decipher them, and benefit greatly thereby.",
                },

            new string[]
                {
                    "Plans For The Construction Of A Hot Air Balloon",
                    "Unknown",
                    "First you must have a wicker balloon basket made, large enough to carry several passengers. Then you'll need a big iron cauldron, to hold a fire to generate the hot air. Next you must have a huge bag sewn out of silk, to hold the hot air in. Lastly, get enough rope to tie the balloon securely to the basket. Once you've gathered all of these together, use these plans to assemble them. When flying your balloon, you'll find that a ship's anchor makes the best ballast, and is also useful for stopping the balloon where and when you wish.",
                },

            new string[]
                {
                    "Plant Lore",
                    "Unknown",
                    "Mistletoe is easiest to find in the spring. Cut the sprigs with your left hand for greatest effectiveness. Hibiscus leaves can be used to make a tea which is excellent for sore throats. Never step on a dandelion, for it will anger any leprechaun who sees you do so.",
                },

            new string[]
                {
                    "Snilwit's Big Book Of Boardgame Strategy",
                    "Snilwit",
                    "Chess: Try to control the middle of the board with your knights, bishops, and pawns. Nine Men's Morris: Don't let any of your pieces get trapped in the corners. Draughts: Keep your pieces along the sides of the board, where they can't be captured.",
                },

            new string[]
                {
                    "Tangled Tales",
                    "Unknown",
                    null,
                },

            new string[]
                {
                    "The Book of Administration",
                    "Unknown",
                    "For countless ages, we winged ones have led the wingless ones. This is right and proper. But we must always remember that they are no less valuable than we. A body with no head cannot move. But neither can a body with no legs. All must function in unity if anything is to be achieved. So guide the wingless ones, and keep them from paths of error. But guide them with respect.",
                },

            new string[]
                {
                    "The Book of Family",
                    "Unknown",
                    "When a child hatches from his egg, he is born without wings. But even from birth one can tell whether a child will grow up to be a winged or a wingless one. The wingless ones cannot speak, and lack the intelligence of the winged ones. They must be guided. The winged ones are few, but they are entrusted with the intelligence and wisdom of the race. They must guide. Both winged and wingless ones spring from the same eggs, and both belong to the same family. All function as a single whole, to better maintain the struggle for survival in our world.",
                },

            new string[]
                {
                    "The Book of Prosperity",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The Book of Ritual",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The Book Of Spirituality",
                    "Unknown",
                    "In your travels through life, remember always that Spirituality embodies the sum of all virtues. Chant the mantra 'om' as you meditate on Spirituality, and all will become clear to you."
                },

            new string[]
                {
                    "The Book of The Underworld",
                    "Unknown",
                    "Deep below the land there is another land. In that land live many strange creatures. The most interesting of these creatures look something like our wingless ones. These daemons, however, are pale and soft. Some say that these daemons from the underworld can speak. And, to be sure, they make sounds that are similar to our language. But as everyone knows, no creature without wings is truly intelligent. Fables of talking daemons must be discredited."
                },

            new string[]
                {
                    "The Caverns of Freitag",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The Codex of Infinite Wisdom",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The First Age of Darkness",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The Lost Book Of Mantras",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The Quest of the Avatar",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The Second Age of Darkness",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The Third Age of Darkness",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "The Wizard Of Oz",
                    "Frank L. Baum",
                    "A little girl named Dorothy, from the far of land of Kansas was carried to the realm of Oz by a tornado. And her little dog, too! She met three faithful companions, who vowed to help her find a way home. There was a scarecrow, who was on a quest for truth, a man of tin, who was questing for love, and a lion, who quested for courage. Before their quest was done, Dorothy slew the wicked witch, freeing the land from her evil influence. Her friends completed their quests, and she returned home to Kansas."
                },

            new string[]
                {
                    "Warriors Of Destiny",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "Windwalker",
                    "Unknown",
                    null
                },

            new string[]
                {
                    "Ye Lost Art Of Ballooning",
                    "Unknown",
                    null
                },
        };
    }
}
