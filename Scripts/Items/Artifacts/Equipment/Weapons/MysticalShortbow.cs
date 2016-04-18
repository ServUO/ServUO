using System;

namespace Server.Items
{
    public class MysticalShortbow : MagicalShortbow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MysticalShortbow()
        {
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastSpeed = -1;
        }

        public MysticalShortbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073511;
            }
        }// mystical shortbow
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}