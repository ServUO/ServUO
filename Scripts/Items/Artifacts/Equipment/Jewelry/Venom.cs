using System;

namespace Server.Items
{
    public class Venom : GoldBracelet
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1114783; } } // Venom
		
        [Constructable]
        public Venom()
        {
            Hue = 1371;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 2;
            Attributes.SpellDamage = 10;
            Resistances.Poison = 20;
        }

        public Venom(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}