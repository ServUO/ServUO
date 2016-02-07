using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Naturalist
{
    public class StudyOfSolenQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(StudyNestsObjective),
            typeof(ReturnToNaturalistObjective),
            typeof(DontOfferConversation),
            typeof(AcceptConversation),
            typeof(NaturalistDuringStudyConversation),
            typeof(EndConversation),
            typeof(SpecialEndConversation),
            typeof(FullBackpackConversation)
        };
        private Naturalist m_Naturalist;
        public StudyOfSolenQuest(PlayerMobile from, Naturalist naturalist)
            : base(from)
        {
            this.m_Naturalist = naturalist;
        }

        // Serialization
        public StudyOfSolenQuest()
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
                // "Study of the Solen Hive"
                return 1054041;
            }
        }
        public override object OfferMessage
        {
            get
            {
                /* <I>The Naturalist looks up from his notes, regarding you with a hopeful
                * look in his eyes.</I><BR><BR>
                * 
                * Um..yes..excuse me. I was wondering if you could offer me a bit of assistance.
                * You see, I'm a naturalist of some repute - a gentleman and a scholar if you
                * will - primarily interested in the study of insects and arachnids. While I've
                * written a few interesting books on the marvelous Terathan race and their bizarre
                * culture, now I've heard tales of a truly significant new discovery!<BR><BR>
                * 
                * Apparently a race of ant-like creatures known as the Solen have appeared in
                * our world, scuttling up from some previously hidden home. Can you believe it?
                * Truly these are amazing times! To a scholar such as myself this is indeed
                * an exciting opportunity.<BR><BR>
                * 
                * That said, while I may be a genius of some reknown, sharp as a tack and quick
                * with the quill, I'm afraid I'm not much of the adventuring type. Though I have
                * gained assistance before, I still have many unanswered questions.<BR><BR>
                * 
                * I am particularly interested in the Solen Egg Nests that are studiously
                * protected by the Solen workers. If you would be so kind as to assist me,
                * I would ask that you travel into the Solen Hive and inspect each of the
                * Solen Egg Nests that reside within. You will have to spend some time examining
                * each Nest before you have gathered enough information. Once you are done,
                * report back to me and I will reward you as best as I can for your valiant
                * efforts!<BR><BR>
                * 
                * Will you accept my offer?
                */
                return 1054042;
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public override bool IsTutorial
        {
            get
            {
                return false;
            }
        }
        public override int Picture
        {
            get
            {
                return 0x15C7;
            }
        }
        public Naturalist Naturalist
        {
            get
            {
                return this.m_Naturalist;
            }
        }
        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_Naturalist = (Naturalist)reader.ReadMobile();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((Mobile)this.m_Naturalist);
        }

        public override void Accept()
        {
            base.Accept();

            if (this.m_Naturalist != null)
                this.m_Naturalist.PlaySound(0x431);

            this.AddConversation(new AcceptConversation());
        }
    }
}