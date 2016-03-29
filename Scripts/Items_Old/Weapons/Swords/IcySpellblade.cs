using System;

namespace Server.Items
{
    public class IcySpellblade : ElvenSpellblade
    {
        [Constructable]
        public IcySpellblade()
        {
            this.WeaponAttributes.ResistColdBonus = 5;
        }

        public IcySpellblade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073514;
            }
        }// icy spellblade
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