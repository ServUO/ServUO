using System;

namespace Server.Items
{
    public class EmbroideryTool : BaseEngravingTool
    {
        public override int LabelNumber { get { return 1158880; } } // Embroidery Tool

        public override bool DeletedItem { get { return false; } }
        public override int LowSkillMessage { get { return 1158837; } } // Your tailoring skill is too low to fix this yourself. An NPC tailor can help you repair this for a fee.
        public override int VeteranRewardCliloc { get { return 1076220; } } // 4th Year Veteran Reward

        public override int GumpTitle { get { return 1158846; } } // <CENTER>Embroidery Tool</CENTER>

        public override int SuccessMessage { get { return 1158841; } } // You embroider the object.
        public override int TargetMessage { get { return 1158838; } } // Select an object to embroider
        public override int RemoveMessage { get { return 1158839; } } // You remove the embroidery from the object.
        public override int OutOfChargesMessage { get { return 1158844; } } // There are no charges left on this embroidery tool.
        public override int CannotEngraveMessage { get { return 1158843; } } // The selected item cannot be embroidered by this tool.
        public override int ObjectWasNotMessage { get { return 1158840; } } // The object was not embroidered.

        [Constructable]
        public EmbroideryTool()
            : base(0xA20A, 10)
        {
            Hue = 0;
        }

        public EmbroideryTool(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Engraves
        {
            get
            {
                return new Type[]
                {
                    typeof(BaseClothing)
                };
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
