using System;

namespace Server.Items
{
    public class NoxRangersHeavyCrossbow : HeavyCrossbow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public NoxRangersHeavyCrossbow()
        {
            this.Hue = 0x58C;
            this.WeaponAttributes.HitLeechStam = 40;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 20;
            this.WeaponAttributes.ResistPoisonBonus = 10;
        }

        public NoxRangersHeavyCrossbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063485;
            }
        }
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
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            pois = 50;
            phys = 50;

            fire = cold = nrgy = chaos = direct = 0;
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