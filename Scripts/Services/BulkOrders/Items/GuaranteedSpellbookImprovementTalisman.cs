using System;
using Server.Mobiles;

namespace Server.Items
{
    public class GuaranteedSpellbookImprovementTalisman : BaseTalisman
    {
        public override int LabelNumber { get { return 1157212; } } // Guaranteed Spellbook Improvement

        [Constructable]
        public GuaranteedSpellbookImprovementTalisman()
            : this(10)
        {
        }

        [Constructable]
        public GuaranteedSpellbookImprovementTalisman(int charges)
            : base(0x9E28)
        {
            Charges = charges;

            Skill = SkillName.Inscribe;
            ExceptionalBonus = BaseTalisman.GetRandomExceptional();
            SuccessBonus = BaseTalisman.GetRandomSuccessful();
        }

        public GuaranteedSpellbookImprovementTalisman(Serial serial)
            : base(serial)
        {
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
