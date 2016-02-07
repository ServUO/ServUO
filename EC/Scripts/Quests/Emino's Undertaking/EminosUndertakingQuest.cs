using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class EminosUndertakingQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(Ninja.AcceptConversation),
            typeof(Ninja.FindZoelConversation),
            typeof(Ninja.RadarConversation),
            typeof(Ninja.EnterCaveConversation),
            typeof(Ninja.SneakPastGuardiansConversation),
            typeof(Ninja.NeedToHideConversation),
            typeof(Ninja.UseTeleporterConversation),
            typeof(Ninja.GiveZoelNoteConversation),
            typeof(Ninja.LostNoteConversation),
            typeof(Ninja.GainInnInformationConversation),
            typeof(Ninja.ReturnFromInnConversation),
            typeof(Ninja.SearchForSwordConversation),
            typeof(Ninja.HallwayWalkConversation),
            typeof(Ninja.ReturnSwordConversation),
            typeof(Ninja.SlayHenchmenConversation),
            typeof(Ninja.ContinueSlayHenchmenConversation),
            typeof(Ninja.GiveEminoSwordConversation),
            typeof(Ninja.LostSwordConversation),
            typeof(Ninja.EarnGiftsConversation),
            typeof(Ninja.EarnLessGiftsConversation),
            typeof(Ninja.FindEminoBeginObjective),
            typeof(Ninja.FindZoelObjective),
            typeof(Ninja.EnterCaveObjective),
            typeof(Ninja.SneakPastGuardiansObjective),
            typeof(Ninja.UseTeleporterObjective),
            typeof(Ninja.GiveZoelNoteObjective),
            typeof(Ninja.GainInnInformationObjective),
            typeof(Ninja.ReturnFromInnObjective),
            typeof(Ninja.SearchForSwordObjective),
            typeof(Ninja.HallwayWalkObjective),
            typeof(Ninja.ReturnSwordObjective),
            typeof(Ninja.SlayHenchmenObjective),
            typeof(Ninja.GiveEminoSwordObjective)
        };
        private bool m_SentRadarConversion;
        public EminosUndertakingQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public EminosUndertakingQuest()
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
                // Emino's Undertaking
                return 1063173;
            }
        }
        public override object OfferMessage
        {
            get
            {
                // Your value as a Ninja must be proven. Find Daimyo Emino and accept the test he offers.
                return 1063174;
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
                return 0x15D5;
            }
        }
        public static bool HasLostNoteForZoel(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return false;

            QuestSystem qs = pm.Quest;

            if (qs is EminosUndertakingQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(GiveZoelNoteObjective)))
                {
                    Container pack = from.Backpack;

                    return (pack == null || pack.FindItemByType(typeof(NoteForZoel)) == null);
                }
            }

            return false;
        }

        public static bool HasLostEminosKatana(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return false;

            QuestSystem qs = pm.Quest;

            if (qs is EminosUndertakingQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(GiveEminoSwordObjective)))
                {
                    Container pack = from.Backpack;

                    return (pack == null || pack.FindItemByType(typeof(EminosKatana)) == null);
                }
            }

            return false;
        }

        public override void Accept()
        {
            base.Accept();

            this.AddConversation(new AcceptConversation());
        }

        public override void Slice()
        {
            if (!this.m_SentRadarConversion && (this.From.Map != Map.Malas || this.From.X < 407 || this.From.X > 431 || this.From.Y < 801 || this.From.Y > 830))
            {
                this.m_SentRadarConversion = true;
                this.AddConversation(new RadarConversation());
            }

            base.Slice();
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_SentRadarConversion = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_SentRadarConversion);
        }
    }
}