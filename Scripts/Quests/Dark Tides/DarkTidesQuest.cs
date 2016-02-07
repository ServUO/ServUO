using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
    public class DarkTidesQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(Necro.AcceptConversation),
            typeof(Necro.AnimateMaabusCorpseObjective),
            typeof(Necro.BankerConversation),
            typeof(Necro.CashBankCheckObjective),
            typeof(Necro.FetchAbraxusScrollObjective),
            typeof(Necro.FindBankObjective),
            typeof(Necro.FindCallingScrollObjective),
            typeof(Necro.FindCityOfLightObjective),
            typeof(Necro.FindCrystalCaveObjective),
            typeof(Necro.FindMaabusCorpseObjective),
            typeof(Necro.FindMaabusTombObjective),
            typeof(Necro.FindMardothAboutKronusObjective),
            typeof(Necro.FindMardothAboutVaultObjective),
            typeof(Necro.FindMardothEndObjective),
            typeof(Necro.FindVaultOfSecretsObjective),
            typeof(Necro.FindWellOfTearsObjective),
            typeof(Necro.HorusConversation),
            typeof(Necro.LostCallingScrollConversation),
            typeof(Necro.MaabasConversation),
            typeof(Necro.MardothEndConversation),
            typeof(Necro.MardothKronusConversation),
            typeof(Necro.MardothVaultConversation),
            typeof(Necro.RadarConversation),
            typeof(Necro.ReadAbraxusScrollConversation),
            typeof(Necro.ReadAbraxusScrollObjective),
            typeof(Necro.ReanimateMaabusConversation),
            typeof(Necro.RetrieveAbraxusScrollObjective),
            typeof(Necro.ReturnToCrystalCaveObjective),
            typeof(Necro.SecondHorusConversation),
            typeof(Necro.SpeakCavePasswordObjective),
            typeof(Necro.UseCallingScrollObjective),
            typeof(Necro.VaultOfSecretsConversation),
            typeof(Necro.FindHorusAboutRewardObjective),
            typeof(Necro.HealConversation),
            typeof(Necro.HorusRewardConversation)
        };
        public DarkTidesQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public DarkTidesQuest()
        {
        }

        public override Type[] TypeReferenceTable
        {
            get
            {
                return m_TypeReferenceTable;
            }
        }
        public override object Name
        {
            get
            {
                // Dark Tides
                return 1060095;
            }
        }
        public override object OfferMessage
        {
            get
            {
                /* <I>An old man who looks to be 200 years old from the looks
                * of his translucently pale and heavily wrinkled skin, turns
                * to you and gives you a half-cocked grin that makes you
                * feel somewhat uneasy.<BR><BR>
                * 
                * After a short pause, he begins to speak to you...</I><BR><BR>
                * 
                * Hmm. What's this?  Another budding Necromancer to join the
                * ranks of Evil?  Here... let me take a look at you...  Ah
                * yes...  Very Good! I sense the forces of evil are strong
                * within you, child – but you need training so that you can
                * learn to focus your skills against those aligned against
                * our cause.  You are destined to become a legendary
                * Necromancer - with the proper training, that only I can
                * give you.<BR><BR>
                * 
                * <I>Mardoth pauses just long enough to give you a wide,
                * skin-crawling grin.</I><BR><BR>
                * 
                * Let me introduce myself. I am Mardoth, the guildmaster of
                * the Necromantic Brotherhood.  I have taken it upon myself
                * to train anyone willing to learn the dark arts of Necromancy.
                * The path of destruction, decay and obliteration is not an
                * easy one.  Only the most evil and the most dedicated can
                * hope to master the sinister art of death.<BR><BR>
                * 
                * I can lend you training and help supply you with equipment –
                * in exchange for a few services rendered by you, of course.
                * Nothing major, just a little death and destruction here and
                * there - the tasks should be easy as a tasty meat pie for one
                * as treacherous and evil as yourself.<BR><BR>
                * 
                * What do you say?  Do we have a deal?
                */
                return 1060094;
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.MaxValue;
            }
        }
        public override bool IsTutorial
        {
            get
            {
                return true;
            }
        }
        public override int Picture
        {
            get
            {
                return 0x15B5;
            }
        }
        public static bool HasLostCallingScroll(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return false;

            QuestSystem qs = pm.Quest;

            if (qs is DarkTidesQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(FindMardothAboutKronusObjective)) || qs.IsObjectiveInProgress(typeof(FindWellOfTearsObjective)) || qs.IsObjectiveInProgress(typeof(UseCallingScrollObjective)))
                {
                    Container pack = from.Backpack;

                    return (pack == null || pack.FindItemByType(typeof(KronusScroll)) == null);
                }
            }

            return false;
        }

        public override void Accept()
        {
            base.Accept();

            this.AddConversation(new AcceptConversation());
        }

        public override bool IgnoreYoungProtection(Mobile from)
        {
            if (from is SummonedPaladin)
                return true;

            return base.IgnoreYoungProtection(from);
        }
    }
}