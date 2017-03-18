using System;
using Server.Mobiles;

namespace Server.Items
{
    public class NecromancersPhylactery : BaseTalisman, ITokunoDyable
    {
        [Constructable]
        public NecromancersPhylactery()
            : base(0x2F5A)
        {
            this.Hue = 1912;
            this.SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 10.0);
            this.Attributes.RegenMana = 1;
            this.Attributes.LowerRegCost = 10;
            this.Attributes.SpellDamage = 5;
        }

        public NecromancersPhylactery(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154728;
            }
        }// Necromancer's Phylactery
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