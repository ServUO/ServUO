using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class WhiteNinjaQuestTeleporter : DynamicTeleporter
    {
        [Constructable]
        public WhiteNinjaQuestTeleporter()
            : base(0x51C, 0x47E)
        {
        }

        public WhiteNinjaQuestTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1026157;
            }
        }// teleporter
        public override int NotWorkingMessage
        {
            get
            {
                return 1063198;
            }
        }// You stand on the strange floor tile but nothing happens.
        public override bool GetDestination(PlayerMobile player, ref Point3D loc, ref Map map)
        {
            QuestSystem qs = player.Quest;

            if (qs is EminosUndertakingQuest)
            {
                QuestObjective obj = qs.FindObjective(typeof(SearchForSwordObjective));

                if (obj != null)
                {
                    if (!obj.Completed)
                        obj.Complete();

                    loc = new Point3D(411, 1085, 0);
                    map = Map.Malas;

                    return true;
                }
            }

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