using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class DaemonBloodChest : MetalChest
    {
        [Constructable]
        public DaemonBloodChest()
        {
            this.Movable = false;
        }

        public DaemonBloodChest(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null && player.InRange(this.GetWorldLocation(), 2))
            {
                QuestSystem qs = player.Quest;

                if (qs is UzeraanTurmoilQuest)
                {
                    QuestObjective obj = qs.FindObjective(typeof(GetDaemonBloodObjective));

                    if ((obj != null && !obj.Completed) || UzeraanTurmoilQuest.HasLostDaemonBlood(player))
                    {
                        Item vial = new QuestDaemonBlood();

                        if (player.PlaceInBackpack(vial))
                        {
                            player.SendLocalizedMessage(1049331, "", 0x22); // You take a vial of blood from the chest and put it in your pack.

                            if (obj != null && !obj.Completed)
                                obj.Complete();
                        }
                        else
                        {
                            player.SendLocalizedMessage(1049338, "", 0x22); // You find a vial of blood, but can't pick it up because your pack is too full.  Come back when you have more room in your pack.
                            vial.Delete();
                        }

                        return;
                    }
                }
            }

            base.OnDoubleClick(from);
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