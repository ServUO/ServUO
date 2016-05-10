using System;
namespace Server.Items
{
    class SpinningLatheComponent : SpecialVeteranAddonComponent
    {        
        [Constructable]
        public SpinningLatheComponent()
            : base()
        {
        }

        public SpinningLatheComponent(Serial serial)
            : base(serial)
        {
        }

        public override int[] ItemIDs
        {
            get { 
                return new[] { 
                    0x9C26, 0x9C1C, // East, South still
                    0x9C25, 0x9C1B  // East, South with animation
                };  
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1123986;
            }
        }// spinning lathe

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

    public class SpinningLatheBoxComponent : SpecialVeteranAddonComponentBox
    {
        [Constructable]
        public SpinningLatheBoxComponent()
            : base()
        {
        }

        public SpinningLatheBoxComponent(Serial serial)
            : base(serial)
        {
        }

        public override Type[] AllowedTools {
            get
            {
                return new Type[] { 
                    typeof(Saw), typeof(DovetailSaw), typeof(Scorp), typeof(DrawKnife), typeof(Froe), typeof(Inshave), 
                    typeof(JointingPlane), typeof(MouldingPlane), typeof(SmoothingPlane) 
                };
            }
        }

        public override int[] ItemIDs
        {
            get { 
                return new[] { 0x9C47, 0x9C46 };  // East, South
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1124030;
            }
        }// bucket

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
