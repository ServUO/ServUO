using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class HaochisKatana : QuestItem
    {
        [Constructable]
        public HaochisKatana()
            : base(0x13FF)
        {
            this.Weight = 1.0;
        }

        public HaochisKatana(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063165;
            }
        }// Daimyo Haochi's Katana
        public override bool CanDrop(PlayerMobile player)
        {
            HaochisTrialsQuest qs = player.Quest as HaochisTrialsQuest;

            if (qs == null)
                return true;

            //return !qs.IsObjectiveInProgress( typeof( FifthTrialReturnObjective ) );
            return false;
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