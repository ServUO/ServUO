using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Quests.Collector
{
    public class EnchantedPaints : QuestItem
    {
        [Constructable]
        public EnchantedPaints()
            : base(0xFC1)
        {
            this.LootType = LootType.Blessed;

            this.Weight = 1.0;
        }

        public EnchantedPaints(Serial serial)
            : base(serial)
        {
        }

        public override bool CanDrop(PlayerMobile player)
        {
            CollectorQuest qs = player.Quest as CollectorQuest;

            if (qs == null)
                return true;

            /*return !( qs.IsObjectiveInProgress( typeof( CaptureImagesObjective ) )
            || qs.IsObjectiveInProgress( typeof( ReturnImagesObjective ) ) );*/
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is CollectorQuest)
                {
                    if (qs.IsObjectiveInProgress(typeof(CaptureImagesObjective)))
                    {
                        player.SendAsciiMessage(0x59, "Target the creature whose image you wish to create.");
                        player.Target = new InternalTarget(this);

                        return;
                    }
                }
            }

            from.SendLocalizedMessage(1010085); // You cannot use this.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly EnchantedPaints m_Paints;
            public InternalTarget(EnchantedPaints paints)
                : base(-1, false, TargetFlags.None)
            {
                this.CheckLOS = false;
                this.m_Paints = paints;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Paints.Deleted || !this.m_Paints.IsChildOf(from.Backpack))
                    return;

                PlayerMobile player = from as PlayerMobile;

                if (player != null)
                {
                    QuestSystem qs = player.Quest;

                    if (qs is CollectorQuest)
                    {
                        CaptureImagesObjective obj = qs.FindObjective(typeof(CaptureImagesObjective)) as CaptureImagesObjective;

                        if (obj != null && !obj.Completed)
                        {
                            if (targeted is Mobile)
                            {
                                ImageType image;
                                CaptureResponse response = obj.CaptureImage((targeted.GetType().Name == "GreaterMongbat" ? new Mongbat().GetType() : targeted.GetType()), out image);

                                switch ( response )
                                {
                                    case CaptureResponse.Valid:
                                        {
                                            player.SendLocalizedMessage(1055125); // The enchanted paints swirl for a moment then an image begins to take shape. *Click*
                                            player.AddToBackpack(new PaintedImage(image));

                                            break;
                                        }
                                    case CaptureResponse.AlreadyDone:
                                        {
                                            player.SendAsciiMessage(0x2C, "You have already captured the image of this creature");

                                            break;
                                        }
                                    case CaptureResponse.Invalid:
                                        {
                                            player.SendLocalizedMessage(1055124); // You have no interest in capturing the image of this creature.

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                player.SendAsciiMessage(0x35, "You have no interest in that.");
                            }

                            return;
                        }
                    }
                }

                from.SendLocalizedMessage(1010085); // You cannot use this.
            }
        }
    }
}