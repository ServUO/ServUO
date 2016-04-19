using System;
namespace Server.Items
{
    class FletchingStationComponent : SpecialVeteranAddonComponent
    {        
        [Constructable]
        public FletchingStationComponent()
            : base()
        {
        }

        public FletchingStationComponent(Serial serial)
            : base(serial)
        {
        }

        public override int[] ItemIDs
        {
            get { 
                return new[] { 
                    0x9C3A, 0x9C30, // East, South still
                    0x9C39, 0x9C2F  // East, South with animation
                };  
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1124006;
            }
        }// fletching station

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

    public class FletchingStationBoxComponent : SpecialVeteranAddonComponentBox
    {
        [Constructable]
        public FletchingStationBoxComponent()
            : base()
        {
        }

        public FletchingStationBoxComponent(Serial serial)
            : base(serial)
        {
        }

        public override Type[] AllowedTools {
            get
            {
                return new Type[] { typeof(FletcherTools) };
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1124027;
            }
        }// rack

        public override int[] ItemIDs
        {
            get { 
                return new[] { 0x9C43, 0x9C43 };  // East, South
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
