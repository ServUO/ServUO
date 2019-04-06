using System;
using Server.Mobiles;

namespace Server.Items
{
    public class TotemOfVoid : BaseTalisman
    {
		public override bool IsArtifact { get { return true; } }
		
        [Constructable]
        public TotemOfVoid()
            : base(0x2F5B)
        {
            Hue = 0x2D0;
            MaxChargeTime = 1800;
            Blessed = GetRandomBlessed();
            Protection = GetRandomProtection(false);
            Attributes.RegenHits = 2;
            Attributes.LowerManaCost = 10;
        }

        public TotemOfVoid(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075035;
            }
        }// Totem of the Void
        public override bool ForceShowName
        {
            get
            {
                return true;
            }
        }
        public override Type GetSummoner()
        {
            return Utility.RandomBool() ? typeof(SummonedSkeletalKnight) : typeof(SummonedSheep);
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