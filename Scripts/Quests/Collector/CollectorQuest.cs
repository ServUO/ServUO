using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Collector
{
    public class CollectorQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(Collector.DontOfferConversation),
            typeof(Collector.DeclineConversation),
            typeof(Collector.AcceptConversation),
            typeof(Collector.ElwoodDuringFishConversation),
            typeof(Collector.ReturnPearlsConversation),
            typeof(Collector.AlbertaPaintingConversation),
            typeof(Collector.AlbertaStoolConversation),
            typeof(Collector.AlbertaEndPaintingConversation),
            typeof(Collector.AlbertaAfterPaintingConversation),
            typeof(Collector.ElwoodDuringPainting1Conversation),
            typeof(Collector.ElwoodDuringPainting2Conversation),
            typeof(Collector.ReturnPaintingConversation),
            typeof(Collector.GabrielAutographConversation),
            typeof(Collector.GabrielNoSheetMusicConversation),
            typeof(Collector.NoSheetMusicConversation),
            typeof(Collector.GetSheetMusicConversation),
            typeof(Collector.GabrielSheetMusicConversation),
            typeof(Collector.GabrielIgnoreConversation),
            typeof(Collector.ElwoodDuringAutograph1Conversation),
            typeof(Collector.ElwoodDuringAutograph2Conversation),
            typeof(Collector.ElwoodDuringAutograph3Conversation),
            typeof(Collector.ReturnAutographConversation),
            typeof(Collector.TomasToysConversation),
            typeof(Collector.TomasDuringCollectingConversation),
            typeof(Collector.ReturnImagesConversation),
            typeof(Collector.ElwoodDuringToys1Conversation),
            typeof(Collector.ElwoodDuringToys2Conversation),
            typeof(Collector.ElwoodDuringToys3Conversation),
            typeof(Collector.FullEndConversation),
            typeof(Collector.FishPearlsObjective),
            typeof(Collector.ReturnPearlsObjective),
            typeof(Collector.FindAlbertaObjective),
            typeof(Collector.SitOnTheStoolObjective),
            typeof(Collector.ReturnPaintingObjective),
            typeof(Collector.FindGabrielObjective),
            typeof(Collector.FindSheetMusicObjective),
            typeof(Collector.ReturnSheetMusicObjective),
            typeof(Collector.ReturnAutographObjective),
            typeof(Collector.FindTomasObjective),
            typeof(Collector.CaptureImagesObjective),
            typeof(Collector.ReturnImagesObjective),
            typeof(Collector.ReturnToysObjective),
            typeof(Collector.MakeRoomObjective)
        };
        public CollectorQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public CollectorQuest()
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
                return 1020549; // Fishing for Rainbow Pearls
            }
        }
        public override object OfferMessage
        {
            get
            {
                /* <I>Elwood greets you warmly, like an old friend he's not
                * quite sure he ever had.</I><BR><BR>
                * 
                * Hello. Yes. Sit down. Please. Good. Okay, stand. Up to you.<BR><BR>
                * 
                * So, what brings you to these parts... hey, wait. Just had a thought.
                * Would you like to do me a favor? Yes, really. You know, for old
                * times sake. The good ole days. You were always one of my best
                * suppliers. Or maybe you weren't, who knows any more. Anyway,
                * could use some help supplementing my stock. You know me. Always
                * looking for something new to add to the collection. Or sometimes
                * not so new - just more of the same. But don't have to tell you that.
                * You know, don't you. Yes. Just like old times. That's what it'll be.
                * You and me - together again. Ah, it's been too long.<BR><BR>
                * 
                * So what do you think? The fee will be the same as always. I'm a fair
                * man. You know that. So what do you say?
                */
                return 1055081;
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
                return 0x15A9;
            }
        }
        public override void Accept()
        {
            base.Accept();

            this.AddConversation(new AcceptConversation());
        }

        public override void Decline()
        {
            base.Decline();

            this.AddConversation(new DeclineConversation());
        }
    }
}