using System;

namespace Server.Items
{
    [FlipableAttribute(0x2D25, 0x2D31)]
    public class BalakaisShamanStaff : WildStaff//wand version as well
    {
        [Constructable]
        public BalakaisShamanStaff()
        {
            WeaponAttributes.MageWeapon = -0;
            Attributes.SpellChanneling = 1;
            Attributes.EnhancePotions = 25;
            SkillBonuses.SetValues(0, SkillName.Meditation, 10.0);
        }

        public BalakaisShamanStaff(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1156125;
            }
        }// Balakai's Shaman Staff

        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }

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