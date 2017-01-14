using System;
using Server.Mobiles;

namespace Server.Items
{ 
    public class WizardsCurio : BaseTalisman
    {
        [Constructable]
        public WizardsCurio()
            : base(0x2F58)
        {
            this.Hue = 1912;
            this.SkillBonuses.SetValues(0, SkillName.EvalInt, 10.0);
            this.Attributes.RegenMana = 1;
            this.Attributes.LowerRegCost = 10;
            this.Attributes.SpellDamage = 5;
        }

        public WizardsCurio(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154729;
            }
        }// Wizard's Curio
        public override bool ForceShowName
        {
            get
            {
                return true;
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