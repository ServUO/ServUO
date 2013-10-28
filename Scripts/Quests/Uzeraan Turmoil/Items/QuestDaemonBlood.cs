using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class QuestDaemonBlood : QuestItem
    {
        [Constructable]
        public QuestDaemonBlood()
            : base(0xF7D)
        {
            this.Weight = 1.0;
        }

        public QuestDaemonBlood(Serial serial)
            : base(serial)
        {
        }

        public override bool CanDrop(PlayerMobile player)
        {
            UzeraanTurmoilQuest qs = player.Quest as UzeraanTurmoilQuest;

            if (qs == null)
                return true;

            /*return !qs.IsObjectiveInProgress( typeof( ReturnDaemonBloodObjective ) );*/
            return false;
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
    }
}