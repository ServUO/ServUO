using System;

namespace Server.Items
{
    public class DjinnisRing : SilverRing
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DjinnisRing()
        {
            this.Attributes.BonusInt = 5;
            this.Attributes.SpellDamage = 10;
            this.Attributes.CastSpeed = 2;
        }

        public DjinnisRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094927;
            }
        }// Djinni's Ring [Replica]
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
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