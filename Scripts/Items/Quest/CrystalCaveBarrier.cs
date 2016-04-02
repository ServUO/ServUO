using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
    public class CrystalCaveBarrier : Item
    {
        [Constructable]
        public CrystalCaveBarrier()
            : base(0x3967)
        {
            this.Movable = false;
        }

        public CrystalCaveBarrier(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.IsStaff())
                return true;

            bool sendMessage = m.Player;

            if (m is BaseCreature)
                m = ((BaseCreature)m).ControlMaster;

            PlayerMobile pm = m as PlayerMobile;

            if (pm != null)
            {
                QuestSystem qs = pm.Quest;

                if (qs is DarkTidesQuest)
                {
                    QuestObjective obj = qs.FindObjective(typeof(SpeakCavePasswordObjective));

                    if (obj != null && obj.Completed)
                    {
                        if (sendMessage)
                            m.SendLocalizedMessage(1060648); // With Horus' permission, you are able to pass through the barrier.

                        return true;
                    }
                }
            }

            if (sendMessage)
                m.SendLocalizedMessage(1060649, "", 0x66D); // Without the permission of the guardian Horus, the magic of the barrier prevents your passage.

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