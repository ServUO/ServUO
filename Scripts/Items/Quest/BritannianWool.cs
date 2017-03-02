/*
 * Created by SharpDevelop.
 * User: Sharon
 * Date: 5/27/2015
 * Time: 4:54 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    public class BritannianWool : Wool
    {
        public override int LabelNumber { get { return 1113242; } } // Britannian wool

        [Constructable]
        public BritannianWool() : this(1)
        {
        }

        [Constructable]
        public BritannianWool(int amount) : base(0x101F)
        {
            Stackable = true;
            Hue = 992;
            Weight = 4.0;
            Amount = amount;
        }

        public BritannianWool(Serial serial) : base(serial)
        {
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