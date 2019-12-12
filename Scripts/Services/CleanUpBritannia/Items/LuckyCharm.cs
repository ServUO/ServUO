using System;
using Server.Mobiles;

namespace Server.Items
{
    public class LuckyCharm : BaseTalisman
    {
		public override int LabelNumber { get { return 1154725; } }// Lucky Charm
		public override bool IsArtifact { get { return true; } }
		
        [Constructable]
        public LuckyCharm()
            : base(0x2F5B)
        {
            Hue = 1923;
            Attributes.RegenHits = 1;
            Attributes.RegenStam = 1;
            Attributes.RegenMana = 1;
            Attributes.Luck = 150;
        }

        public LuckyCharm(Serial serial)
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