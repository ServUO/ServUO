using System;
using System.Collections;
using Server.Mobiles;

namespace Server.Engines.Quests.Naturalist
{
    public class StudyNestsObjective : QuestObjective
    {
        private readonly ArrayList m_StudiedNests;
        private NestArea m_CurrentNest;
        private DateTime m_StudyBegin;
        private StudyState m_StudyState;
        private bool m_StudiedSpecialNest;
        public StudyNestsObjective()
        {
            this.m_StudiedNests = new ArrayList();
        }

        private enum StudyState
        {
            Inactive,
            FirstStep,
            SecondStep
        }
        public override object Message
        {
            get
            {
                /* Find an entrance to the Solen Hive, and search within for the Solen
                * Egg Nests. Each Nest must be studied for some time without a break in
                * concentration in order to gather useful information.<BR><BR>
                * 
                * Once you have completed your study of the Nests, return to the Naturalist
                * who gave you this task.
                */
                return 1054044;
            }
        }
        public override int MaxProgress
        {
            get
            {
                return NestArea.NonSpecialCount;
            }
        }
        public bool StudiedSpecialNest
        {
            get
            {
                return this.m_StudiedSpecialNest;
            }
        }
        public override bool GetTimerEvent()
        {
            return true;
        }

        public override void CheckProgress()
        {
            PlayerMobile from = this.System.From;

            if (this.m_CurrentNest != null)
            {
                NestArea nest = this.m_CurrentNest;

                if ((from.Map == Map.Trammel || from.Map == Map.Felucca) && nest.Contains(from))
                {
                    if (this.m_StudyState != StudyState.Inactive)
                    {
                        TimeSpan time = DateTime.UtcNow - this.m_StudyBegin;

                        if (time > TimeSpan.FromSeconds(30.0))
                        {
                            this.m_StudiedNests.Add(nest);
                            this.m_StudyState = StudyState.Inactive;

                            if (this.m_CurrentNest.Special)
                            {
                                from.SendLocalizedMessage(1054057); // You complete your examination of this bizarre Egg Nest. The Naturalist will undoubtedly be quite interested in these notes!
                                this.m_StudiedSpecialNest = true;
                            }
                            else
                            {
                                from.SendLocalizedMessage(1054054); // You have completed your study of this Solen Egg Nest. You put your notes away.
                                this.CurProgress++;
                            }
                        }
                        else if (this.m_StudyState == StudyState.FirstStep && time > TimeSpan.FromSeconds(15.0))
                        {
                            if (!nest.Special)
                                from.SendLocalizedMessage(1054058); // You begin recording your completed notes on a bit of parchment.

                            this.m_StudyState = StudyState.SecondStep;
                        }
                    }
                }
                else
                {
                    if (this.m_StudyState != StudyState.Inactive)
                        from.SendLocalizedMessage(1054046); // You abandon your study of the Solen Egg Nest without gathering the needed information.

                    this.m_CurrentNest = null;
                }
            }
            else if (from.Map == Map.Trammel || from.Map == Map.Felucca)
            {
                NestArea nest = NestArea.Find(from);

                if (nest != null)
                {
                    this.m_CurrentNest = nest;
                    this.m_StudyBegin = DateTime.UtcNow;

                    if (this.m_StudiedNests.Contains(nest))
                    {
                        this.m_StudyState = StudyState.Inactive;

                        from.SendLocalizedMessage(1054047); // You glance at the Egg Nest, realizing you've already studied this one.
                    }
                    else
                    {
                        this.m_StudyState = StudyState.FirstStep;

                        if (nest.Special)
                            from.SendLocalizedMessage(1054056); // You notice something very odd about this Solen Egg Nest. You begin taking notes.
                        else
                            from.SendLocalizedMessage(1054045); // You begin studying the Solen Egg Nest to gather information.

                        if (from.Female)
                            from.PlaySound(0x30B);
                        else
                            from.PlaySound(0x419);
                    }
                }
            }
        }

        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                gump.AddHtmlLocalized(70, 260, 270, 100, 1054055, BaseQuestGump.Blue, false, false); // Solen Nests Studied :
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
            this.System.AddObjective(new ReturnToNaturalistObjective());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            int count = reader.ReadEncodedInt();
            for (int i = 0; i < count; i++)
            {
                NestArea nest = NestArea.GetByID(reader.ReadEncodedInt());
                this.m_StudiedNests.Add(nest);
            }

            this.m_StudiedSpecialNest = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.WriteEncodedInt((int)this.m_StudiedNests.Count);
            foreach (NestArea nest in this.m_StudiedNests)
            {
                writer.WriteEncodedInt((int)nest.ID);
            }

            writer.Write((bool)this.m_StudiedSpecialNest);
        }
    }

    public class ReturnToNaturalistObjective : QuestObjective
    {
        public ReturnToNaturalistObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* You have studied enough Solen Egg Nests to gather a fair amount of
                * useful information. Return to the Naturalist who gave you this task.
                */
                return 1054048;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            string count = NestArea.NonSpecialCount.ToString();

            gump.AddHtmlLocalized(70, 260, 270, 100, 1054055, BaseQuestGump.Blue, false, false); // Solen Nests Studied :
            gump.AddLabel(70, 280, 0x64, count);
            gump.AddLabel(100, 280, 0x64, "/");
            gump.AddLabel(130, 280, 0x64, count);
        }
    }
}