using System;

namespace Server.Items
{
    public class Venom : GoldBracelet
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public Venom()
        {
            this.Name = ("Venom");
		
            this.Hue = 1371;
            this.Attributes.CastRecovery = 1;
            this.Attributes.CastSpeed = 2;
            this.Attributes.SpellDamage = 10;
            this.Resistances.Poison = 20;
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