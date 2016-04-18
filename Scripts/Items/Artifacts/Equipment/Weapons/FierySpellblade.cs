using System;

namespace Server.Items
{
    public class FierySpellblade : ElvenSpellblade
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FierySpellblade()
        {
            this.WeaponAttributes.ResistFireBonus = 5;
        }

        public FierySpellblade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073515;
            }
        }// fiery spellblade
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