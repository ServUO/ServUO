using System;
 
namespace Server.Items
{
    public class TacticalMask : LeatherMempo
    {
        [Constructable]
        public TacticalMask() //: base( 10106 )
        {
            this.Name = "Tactical Mask";
            this.Hue = 1420;
			this.Weight = 2;

			this.SkillBonuses.SetValues(2, SkillName.Tactics, 5.0);
			this.SkillBonuses.SetValues(0, SkillName.Swords, 5);
			this.SkillBonuses.SetValues(1, SkillName.Fencing, 5);
			this.SkillBonuses.SetValues(1, SkillName.Macing, 5);

			this.Attributes.BonusDex = 5;
			this.Attributes.DefendChance = 15;
			this.Attributes.CastRecovery = 3;
			this.Attributes.CastSpeed = 2;
			this.ArmorAttributes.LowerStatReq = 5;
			this.Attributes.NightSight = 1;
			this.Attributes.WeaponSpeed = 10;

			//PhysicalBonus = 14;
			//ColdBonus = 14;
			//FireBonus = 8;
			//PoisonBonus = 15;
			//EnergyBonus = 15;

		}

		public override int BasePhysicalResistance
		{
			get
			{
				return 14;
			}
		}
		public override int BaseFireResistance
		{
			get
			{
				return 8;
			}
		}
		public override int BaseColdResistance
		{
			get
			{
				return 14;
			}
		}
		public override int BasePoisonResistance
		{
			get
			{
				return 15;
			}
		}
		public override int BaseEnergyResistance
		{
			get
			{
				return 15;
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

		public TacticalMask( Serial serial ) : base( serial )
        {
        }
 
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
 
            writer.Write( (int) 0 ); // version
        }
 
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
 
            int version = reader.ReadInt();
        }
    }
}
