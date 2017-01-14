using System;

namespace Server.Items
{
    public class StormCaller : Boomerang
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public StormCaller()
            : base()
        {
            this.Name = ("Storm Caller");
			this.Weight = 4;
		
            this.Hue = 456;
            this.WeaponAttributes.BattleLust = 1;
            this.Attributes.BonusStr = 5;
            this.WeaponAttributes.HitLightning = 40;
            this.WeaponAttributes.HitLowerDefend = 30;			
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 40;
            this.AosElementDamages.Physical = 20;
            this.AosElementDamages.Fire = 20;
            this.AosElementDamages.Cold = 20;
            this.AosElementDamages.Poison = 20;
            this.AosElementDamages.Energy = 20;
        }

        public StormCaller(Serial serial)
            : base(serial)
        {
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
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
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