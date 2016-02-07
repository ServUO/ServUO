using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Collector
{
    public enum Theater
    {
        Britain,
        Nujelm,
        Jhelom
    }

    public enum CaptureResponse
    {
        Valid,
        AlreadyDone,
        Invalid
    }

    public class FishPearlsObjective : QuestObjective
    {
        public FishPearlsObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Fish up shellfish from Lake Haven and collect rainbow pearls.
                return 1055084;
            }
        }
        public override int MaxProgress
        {
            get
            {
                return 6;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                // Rainbow pearls collected:
                gump.AddHtmlObject(70, 260, 270, 100, 1055085, BaseQuestGump.Blue, false, false);

                gump.AddLabel(70, 280, 0x64, this.CurProgress.ToString());
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, this.MaxProgress.ToString());
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new ReturnPearlsObjective());
        }
    }

    public class ReturnPearlsObjective : QuestObjective
    {
        public ReturnPearlsObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* You've collected enough rainbow pearls. Speak to
                * Elwood to give them to him and get your next task.
                */
                return 1055088;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new ReturnPearlsConversation());
        }
    }

    public class FindAlbertaObjective : QuestObjective
    {
        public FindAlbertaObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Go to Vesper and speak to Alberta Giacco at the Colored Canvas.
                return 1055091;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new AlbertaPaintingConversation());
        }
    }

    public class SitOnTheStoolObjective : QuestObjective
    {
        private static readonly Point3D m_StoolLocation = new Point3D(2899, 706, 0);
        private static readonly Map m_StoolMap = Map.Trammel;
        private DateTime m_Begin;
        public SitOnTheStoolObjective()
        {
            this.m_Begin = DateTime.MaxValue;
        }

        public override object Message
        {
            get
            {
                /* Sit on the stool in front of Alberta's easel so that she can
                * paint your portrait. You'll need to sit there for about 30 seconds.
                */
                return 1055093;
            }
        }
        public override void CheckProgress()
        {
            PlayerMobile pm = this.System.From;

            if (pm.Map == m_StoolMap && pm.Location == m_StoolLocation)
            {
                if (this.m_Begin == DateTime.MaxValue)
                {
                    this.m_Begin = DateTime.UtcNow;
                }
                else if (DateTime.UtcNow - this.m_Begin > TimeSpan.FromSeconds(30.0))
                {
                    this.Complete();
                }
            }
            else if (this.m_Begin != DateTime.MaxValue)
            {
                this.m_Begin = DateTime.MaxValue;
                pm.SendLocalizedMessage(1055095, "", 0x26); // You must remain seated on the stool until the portrait is complete. Alberta will now have to start again with a fresh canvas.
            }
        }

        public override void OnComplete()
        {
            this.System.AddConversation(new AlbertaEndPaintingConversation());
        }
    }

    public class ReturnPaintingObjective : QuestObjective
    {
        public ReturnPaintingObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Return to Elwood and let him know that the painting is complete.
                return 1055099;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new ReturnPaintingConversation());
        }
    }

    public class FindGabrielObjective : QuestObjective
    {
        public FindGabrielObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Go to Britain and obtain the autograph of renowned
                * minstrel, Gabriel Piete. He is often found at the Conservatory of Music.
                */
                return 1055101;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new GabrielAutographConversation());
        }
    }

    public class FindSheetMusicObjective : QuestObjective
    {
        private Theater m_Theater;
        public FindSheetMusicObjective(bool init)
        {
            if (init)
                this.InitTheater();
        }

        public FindSheetMusicObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Find some sheet music for one of Gabriel's songs.
                * Try speaking to an impresario from one of the theaters in the land.
                */
                return 1055104;
            }
        }
        public void InitTheater()
        {
            switch ( Utility.Random(3) )
            {
                case 1:
                    this.m_Theater = Theater.Britain;
                    break;
                case 2:
                    this.m_Theater = Theater.Nujelm;
                    break;
                default:
                    this.m_Theater = Theater.Jhelom;
                    break;
            }
        }

        public bool IsInRightTheater()
        {
            PlayerMobile player = this.System.From;

            Region region = Region.Find(player.Location, player.Map);

            if (region == null)
                return false;

            switch ( this.m_Theater )
            {
                case Theater.Britain:
                    return region.IsPartOf("Britain");
                case Theater.Nujelm:
                    return region.IsPartOf("Nujel'm");
                case Theater.Jhelom:
                    return region.IsPartOf("Jhelom");

                default:
                    return false;
            }
        }

        public override void OnComplete()
        {
            this.System.AddConversation(new GetSheetMusicConversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_Theater = (Theater)reader.ReadEncodedInt();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.WriteEncodedInt((int)this.m_Theater);
        }
    }

    public class ReturnSheetMusicObjective : QuestObjective
    {
        public ReturnSheetMusicObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Speak to Gabriel to have him autograph the sheet music.
                return 1055110;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new GabrielSheetMusicConversation());
        }
    }

    public class ReturnAutographObjective : QuestObjective
    {
        public ReturnAutographObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Speak to Elwood to give him the autographed sheet music.
                return 1055114;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new ReturnAutographConversation());
        }
    }

    public class FindTomasObjective : QuestObjective
    {
        public FindTomasObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Go to Trinsic and speak to Tomas O'Neerlan, the famous toymaker.
                return 1055117;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new TomasToysConversation());
        }
    }

    public class CaptureImagesObjective : QuestObjective
    {
        private ImageType[] m_Images;
        private bool[] m_Done;
        public CaptureImagesObjective(bool init)
        {
            if (init)
            {
                this.m_Images = ImageTypeInfo.RandomList(4);
                this.m_Done = new bool[4];
            }
        }

        public CaptureImagesObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Use the enchanted paints to capture the image of all of the creatures listed below.
                return 1055120;
            }
        }
        public override bool Completed
        {
            get
            {
                for (int i = 0; i < this.m_Done.Length; i++)
                {
                    if (!this.m_Done[i])
                        return false;
                }

                return true;
            }
        }
        public override bool IgnoreYoungProtection(Mobile from)
        {
            if (this.Completed)
                return false;

            Type fromType = from.GetType();

            for (int i = 0; i < this.m_Images.Length; i++)
            {
                ImageTypeInfo info = ImageTypeInfo.Get(this.m_Images[i]);

                if (info.Type == fromType)
                    return true;
            }

            return false;
        }

        public CaptureResponse CaptureImage(Type type, out ImageType image)
        {
            for (int i = 0; i < this.m_Images.Length; i++)
            {
                ImageTypeInfo info = ImageTypeInfo.Get(this.m_Images[i]);

                if (info.Type == type)
                {
                    image = this.m_Images[i];

                    if (this.m_Done[i])
                    {
                        return CaptureResponse.AlreadyDone;
                    }
                    else
                    {
                        this.m_Done[i] = true;

                        this.CheckCompletionStatus();

                        return CaptureResponse.Valid;
                    }
                }
            }

            image = (ImageType)0;
            return CaptureResponse.Invalid;
        }

        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                for (int i = 0; i < this.m_Images.Length; i++)
                {
                    ImageTypeInfo info = ImageTypeInfo.Get(this.m_Images[i]);

                    gump.AddHtmlObject(70, 260 + 20 * i, 200, 100, info.Name, BaseQuestGump.Blue, false, false);
                    gump.AddLabel(200, 260 + 20 * i, 0x64, " : ");
                    gump.AddHtmlObject(220, 260 + 20 * i, 100, 100, this.m_Done[i] ? 1055121 : 1055122, BaseQuestGump.Blue, false, false);
                }
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new ReturnImagesObjective());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            int count = reader.ReadEncodedInt();

            this.m_Images = new ImageType[count];
            this.m_Done = new bool[count];

            for (int i = 0; i < count; i++)
            {
                this.m_Images[i] = (ImageType)reader.ReadEncodedInt();
                this.m_Done[i] = reader.ReadBool();
            }
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.WriteEncodedInt((int)this.m_Images.Length);

            for (int i = 0; i < this.m_Images.Length; i++)
            {
                writer.WriteEncodedInt((int)this.m_Images[i]);
                writer.Write((bool)this.m_Done[i]);
            }
        }
    }

    public class ReturnImagesObjective : QuestObjective
    {
        public ReturnImagesObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* You now have all of the creature images you need.
                * Return to Tomas O'Neerlan so that he can make the toy figurines.
                */
                return 1055128;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new ReturnImagesConversation());
        }
    }

    public class ReturnToysObjective : QuestObjective
    {
        public ReturnToysObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Return to Elwood with news that the toy figurines will be delivered when ready.
                return 1055132;
            }
        }
    }

    public class MakeRoomObjective : QuestObjective
    {
        public MakeRoomObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Return to Elwood for your reward when you have some room in your backpack.
                return 1055136;
            }
        }
    }
}