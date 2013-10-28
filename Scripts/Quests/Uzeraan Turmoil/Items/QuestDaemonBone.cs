using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class QuestDaemonBone : QuestItem
    {
        [Constructable]
        public QuestDaemonBone()
            : base(0xF80)
        {
            this.Weight = 1.0;
        }

        public QuestDaemonBone(Serial serial)
            : base(serial)
        {
        }

        public override bool CanDrop(PlayerMobile player)
        {
            UzeraanTurmoilQuest qs = player.Quest as UzeraanTurmoilQuest;

            if (qs == null)
                return true;

            //return !qs.IsObjectiveInProgress( typeof( ReturnDaemonBoneObjective ) );
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