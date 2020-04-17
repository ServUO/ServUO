using System;

namespace Server.Items
{
    public class EmbroideryTool : BaseEngravingTool
    {
        public override int LabelNumber => 1158880;  // Embroidery Tool

        public override bool DeletedItem => false;
        public override int LowSkillMessage => 1158837;  // Your tailoring skill is too low to fix this yourself. An NPC tailor can help you repair this for a fee.
        public override int VeteranRewardCliloc => 1076220;  // 4th Year Veteran Reward

        public override int GumpTitle => 1158846;  // <CENTER>Embroidery Tool</CENTER>

        public override int SuccessMessage => 1158841;  // You embroider the object.
        public override int TargetMessage => 1158838;  // Select an object to embroider
        public override int RemoveMessage => 1158839;  // You remove the embroidery from the object.
        public override int OutOfChargesMessage => 1158844;  // There are no charges left on this embroidery tool.
        public override int CannotEngraveMessage => 1158843;  // The selected item cannot be embroidered by this tool.
        public override int ObjectWasNotMessage => 1158840;  // The object was not embroidered.

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

        public override Type[] Engraves => new Type[]
                {
                    typeof(BaseClothing)
                };
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
