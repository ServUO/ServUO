using System;

namespace Server.Items
{
    public class BloodwoodSpirit : BaseTalisman
    {
        [Constructable]
        public BloodwoodSpirit()
            : base(0x2F5A)
        {
            this.Hue = 0x27;
            this.MaxChargeTime = 1200;

            this.Removal = TalismanRemoval.Damage;
            this.Blessed = GetRandomBlessed();
            this.Protection = GetRandomProtection(false);

            this.SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 10.0);
            this.SkillBonuses.SetValues(1, SkillName.Necromancy, 5.0);
        }

        public BloodwoodSpirit(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075034;
            }
        }// Bloodwood Spirit
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

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && (this.Protection == null || this.Protection.IsEmpty))
                this.Protection = GetRandomProtection(false);
        }
    }
}