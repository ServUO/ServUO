using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class HaochisKatanaGenerator : Item
    {
        [Constructable]
        public HaochisKatanaGenerator()
            : base(0x1B7B)
        {
            this.Visible = false;
            this.Name = "Haochi's katana generator";
            this.Movable = false;
        }

        public HaochisKatanaGenerator(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            PlayerMobile player = m as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is HaochisTrialsQuest)
                {
                    if (HaochisTrialsQuest.HasLostHaochisKatana(player))
                    {
                        Item katana = new HaochisKatana();

                        if (!player.PlaceInBackpack(katana))
                        {
                            katana.Delete();
                            player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                        }
                    }
                    else
                    {
                        QuestObjective obj = qs.FindObjective(typeof(FifthTrialIntroObjective));

                        if (obj != null && !obj.Completed)
                        {
                            Item katana = new HaochisKatana();

                            if (player.PlaceInBackpack(katana))
                            {
                                obj.Complete();
                            }
                            else
                            {
                                katana.Delete();
                                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                            }
                        }
                    }
                }
            }

            return base.OnMoveOver(m);
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