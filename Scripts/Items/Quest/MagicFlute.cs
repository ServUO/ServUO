using Server.Mobiles;

namespace Server.Engines.Quests.Hag
{
    public class MagicFlute : Item
    {
        [Constructable]
        public MagicFlute()
            : base(0x1421)
        {
            Hue = 0x8AB;
        }

        public MagicFlute(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1055051;// magic flute
        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                SendLocalizedMessageTo(from, 1042292); // You must have the object in your backpack to use it.
                return;
            }

            from.PlaySound(0x3D);

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is WitchApprenticeQuest)
                {
                    FindZeefzorpulObjective obj = qs.FindObjective(typeof(FindZeefzorpulObjective)) as FindZeefzorpulObjective;

                    if (obj != null && !obj.Completed)
                    {
                        if ((player.Map != Map.Trammel && player.Map != Map.Felucca) || !player.InRange(obj.ImpLocation, 8))
                        {
                            player.SendLocalizedMessage(1055053); // Nothing happens. Zeefzorpul must not be hiding in this area.
                        }
                        else if (player.InRange(obj.ImpLocation, 4))
                        {
                            Delete();

                            obj.Complete();
                        }
                        else
                        {
                            player.SendLocalizedMessage(1055052); // The flute sparkles. Zeefzorpul must be in a good hiding place nearby.
                        }
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}