using System;

namespace Server.Items
{
    public class TrueSpellblade : ElvenSpellblade
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TrueSpellblade()
        {
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastSpeed = -1;
        }

        public TrueSpellblade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073513;
            }
        }// true spellblade
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