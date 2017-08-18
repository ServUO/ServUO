using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    [TypeAlias("Server.Items.PetBraningIron")]
    public class PetBrandingIron : BaseEngravingTool
    {
        public override int GumpTitle { get { return 1157374; } }
        public override int LabelNumber { get { return 1157314; } }

        public override int SuccessMessage { get { return 1157382; } } // // You brand the pet.
        public override int TargetMessage { get { return 1157379; } } // Select a pet to brand.
        public override int RemoveMessage { get { return 1157380; } } // You remove the brand from the pet.
        public override int OutOfChargesMessage { get { return 1157377; } } // There are no charges left on this branding iron.	
        public override int NotAccessibleMessage { get { return 1157376; } } // The selected pet is not accessible to brand.
        public override int CannotEngraveMessage { get { return 1157375; } } // The selected pet cannot be branded by this branding iron.

        public override Type[] Engraves { get { return new Type[] { typeof(BaseCreature) }; } }

        [Constructable]
        public PetBrandingIron()
            : this(30)
        {
        }

        [Constructable]
        public PetBrandingIron(int charges)
            : base(0x9E87, charges) 
        {
        }

        public PetBrandingIron(Serial serial)
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