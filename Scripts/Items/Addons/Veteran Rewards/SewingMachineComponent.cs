using System;
namespace Server.Items
{
    class SewingMachineComponent : SpecialVeteranAddonComponent
    {        
        [Constructable]
        public SewingMachineComponent()
            : base()
        {
        }

        public SewingMachineComponent(Serial serial)
            : base(serial)
        {
        }

        public override int[] ItemIDs
        {
            get { 
                return new[] {
                    0x9A49, 0x9A48, // East, South still
                    0x9A40, 0x9A38  // East, South with animation
                };  
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1123504;
            }
        }// sewing machine

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

    public class SewingMachineBoxComponent : SpecialVeteranAddonComponentBox
    {
        [Constructable]
        public SewingMachineBoxComponent()
            : base()
        {
        }

        public SewingMachineBoxComponent(Serial serial)
            : base(serial)
        {
        }

        public override int[] ItemIDs
        {
            get { 
                return new[] { 0x9A4A, 0x9A4A };  // East, South
            }
        }
        
        public override Type[] AllowedTools {
            get
            {
                return new Type[]{ typeof(SewingKit) };
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1123522;
            }
        }// sewing machine

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
