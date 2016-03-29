using System;
using Server;

namespace Server.Items
{
    public class EightBall : Item
    {
        private DateTime lastused = DateTime.Now;
        private TimeSpan delay = TimeSpan.FromSeconds(3);

        [Constructable]
        public EightBall() : base(0xE2E)
        {
            Weight = 1.0;
            Name = "a magic eight ball";
            Stackable = false;
            LootType = LootType.Blessed;
            Light = LightType.Circle150;
        }

        public EightBall(Serial serial) : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {

            base.OnDoubleClick(from);
            if (lastused + delay > DateTime.Now)
            {
                from.SendLocalizedMessage(501789);
                return;
            }
            else
            {
                lastused = DateTime.Now;
            }

            if (IsChildOf(from.Backpack) || from.InRange(this, 2) && from.CanSee(this))
            {
                switch (Utility.Random(8))
                {
                    default:
                    case 0: from.SendMessage("IT IS CERTAIN"); break;

                    case 1: from.SendMessage("WITHOUT A DOUBT"); break;

                    case 2: from.SendMessage("MY REPLY IS NO"); break;

                    case 3: from.SendMessage("ASK AGAIN LATER"); break;

                    case 4: from.SendMessage("VERY DOUBTFUL"); break;

                    case 5: from.SendMessage("CONCENTRATE AND ASK AGAIN"); break;

                    case 6: from.SendMessage("DON'T COUNT ON IT"); break;

                    case 7: from.SendMessage("YES"); break;
                }
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }
    }
}