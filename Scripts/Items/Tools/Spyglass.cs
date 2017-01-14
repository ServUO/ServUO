using System;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x14F5, 0x14F6)]
    public class Spyglass : Item
    {
        [Constructable]
        public Spyglass()
            : base(0x14F5)
        {
            this.Weight = 3.0;
        }

        public Spyglass(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1008155); // You peer into the heavens, seeking the moons...

            from.Send(new MessageLocalizedAffix(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 1008146 + (int)Clock.GetMoonPhase(Map.Trammel, from.X, from.Y), "", AffixType.Prepend, "Trammel : ", ""));
            from.Send(new MessageLocalizedAffix(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 1008146 + (int)Clock.GetMoonPhase(Map.Felucca, from.X, from.Y), "", AffixType.Prepend, "Felucca : ", ""));

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is WitchApprenticeQuest)
                {
                    FindIngredientObjective obj = qs.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

                    if (obj != null && !obj.Completed && obj.Ingredient == Ingredient.StarChart)
                    {
                        int hours, minutes;
                        Clock.GetTime(from.Map, from.X, from.Y, out hours, out minutes);

                        if (hours < 5 || hours > 17)
                        {
                            player.SendLocalizedMessage(1055040); // You gaze up into the glittering night sky.  With great care, you compose a chart of the most prominent star patterns.

                            obj.Complete();
                        }
                        else
                        {
                            player.SendLocalizedMessage(1055039); // You gaze up into the sky, but it is not dark enough to see any stars.
                        }
                    }
                }
            }
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