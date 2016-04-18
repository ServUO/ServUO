using System;

namespace Server.Items
{
    public class TheDragonsTail : NoDachi
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TheDragonsTail()
        {
            this.LootType = LootType.Blessed;

            this.WeaponAttributes.HitLeechStam = 16;
            this.Attributes.WeaponSpeed = 10;
            this.Attributes.WeaponDamage = 25;
        }

        public TheDragonsTail(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1078015;
            }
        }// The Dragon's Tail
        public override int InitMinHits
        {
            get
            {
                return 80;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 80;
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