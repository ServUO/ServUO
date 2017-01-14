using System;

namespace Server.Items
{
    public class DreadsRevenge : Kryss, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber
        {
            get
            {
                return 1072092;
            }
        }// Dread's Revenge

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

        [Constructable]
        public DreadsRevenge()
            : base()
        {
            this.Hue = 0x3A;
			
            this.SkillBonuses.SetValues(0, SkillName.Fencing, 20.0);
			
            this.WeaponAttributes.HitPoisonArea = 30;
            this.Attributes.AttackChance = 15;
            this.Attributes.WeaponSpeed = 50;
        }

        #region Mondain's Legacy
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = nrgy = chaos = direct = 0;
            pois = 100;
        }

        #endregion

        public DreadsRevenge(Serial serial)
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