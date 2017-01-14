using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class QuartzGrit : Item
    {
        public override int LabelNumber { get { return 1151808; } } // Quartz Grit

        [Constructable]
        public QuartzGrit()
            : this(1)
        {
        }

        [Constructable]
        public QuartzGrit(int amount) : base(0x423A)
        {
            this.Hue = 1151;
            this.Weight = 1;

            Stackable = true;
            Amount = amount;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            else if (from.Backpack.GetAmount(typeof(CursedOilstone)) == 0)
                from.SendLocalizedMessage(1151813, "#1151810"); // You do not have a required component: ~1_val~
            else if (from.Backpack.GetAmount(typeof(CorrosiveAsh)) == 0)
                from.SendLocalizedMessage(1151813, "#1151809"); // You do not have a required component: ~1_val~
            else
            {
                from.Backpack.ConsumeTotal(new Type[] { typeof(CursedOilstone), typeof(CorrosiveAsh) },
                                           new int[] { 1, 1 });

                this.Consume();

                from.AddToBackpack(new WhetstoneOfEnervation());
                from.SendLocalizedMessage(1151812); // You have managed to form the items into a rancid smelling, crag covered, hardened lump. In a moment of prescience, you realize what it must be named. The Whetstone of Enervation!
            }
        }

        public QuartzGrit(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}