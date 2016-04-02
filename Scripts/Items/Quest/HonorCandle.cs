using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class HonorCandle : CandleLong
    {
        private static readonly TimeSpan LitDuration = TimeSpan.FromSeconds(20.0);
        [Constructable]
        public HonorCandle()
        {
            this.Movable = false;
            this.Duration = LitDuration;
        }

        public HonorCandle(Serial serial)
            : base(serial)
        {
        }

        public override int LitSound
        {
            get
            {
                return 0;
            }
        }
        public override int UnlitSound
        {
            get
            {
                return 0;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            bool wasBurning = this.Burning;

            base.OnDoubleClick(from);

            if (!wasBurning && this.Burning)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                QuestSystem qs = player.Quest;

                if (qs != null && qs is HaochisTrialsQuest)
                {
                    QuestObjective obj = qs.FindObjective(typeof(SixthTrialIntroObjective));

                    if (obj != null && !obj.Completed)
                        obj.Complete();

                    this.SendLocalizedMessageTo(from, 1063251); // You light a candle in honor.
                }
            }
        }

        public override void Burn()
        {
            this.Douse();
        }

        public override void Douse()
        {
            base.Douse();

            this.Duration = LitDuration;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}