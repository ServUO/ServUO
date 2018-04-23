using System;
using Server;

namespace Server.Items
{
	public class femalechestofevolution: LeatherBustierArms
	{

		private int mEvolutionPoints = 0;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int EvolutionPoints { get { return mEvolutionPoints; } set { mEvolutionPoints = value; } }

		public override int ArtifactRarity{ get{ return 13; } }
        public override int BasePhysicalResistance { get { return 10; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 10; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int BaseEnergyResistance { get { return 10; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }
        private Mobile m_Owner;
		[Constructable]
		public femalechestofevolution()
        {
            Name = "<BASEFONT COLOR=#2E9AFE>Evo Female Chest";
			Hue = 1910;

            ArmorAttributes.SelfRepair = 10;
            ArmorAttributes.MageArmor = 1;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 16;
        }

        public femalechestofevolution(Serial serial)
            : base(serial)
        {
        }
           public override int OnHit( BaseWeapon weapon, int damageTaken )
        {
            if (Utility.Random(2) == 0)
            {
                Console.WriteLine("should be applying gain");
                ApplyGain();
            }

            return base.OnHit(weapon, damageTaken);
        }

        public void ApplyGain()
        {
            int expr;
            if (mEvolutionPoints < 5001) // edit this to change how high you wish the Attributes to go 10000 means max attributes will be 100
            {
                mEvolutionPoints++;
                this.Name = "Evo Female Chest (" + mEvolutionPoints.ToString() + ")";

                if ((mEvolutionPoints / 1) > 0)
                {
                    expr = mEvolutionPoints / 20;

                    this.Attributes.AttackChance = expr;
                    this.Attributes.WeaponSpeed  = expr;
                    this.Attributes.BonusHits = expr;
                    this.Attributes.BonusMana = expr;
                    this.Attributes.BonusStam = expr;
                }

                if ((mEvolutionPoints / 2) > 0)
                {
                    expr = mEvolutionPoints / 2;

                    this.Attributes.WeaponDamage = expr;
                    this.Attributes.Luck = expr;
                    this.Attributes.SpellDamage = expr;
                }

                if ((25 + (mEvolutionPoints / 2)) > 0) this.Attributes.WeaponSpeed = (25 + (mEvolutionPoints / 2));

                if ((mEvolutionPoints / 20) > 0)
                {
                    expr = mEvolutionPoints / 20;

                    this.Attributes.DefendChance = expr;
                    this.Attributes.ReflectPhysical = expr;
                    this.Attributes.BonusStr = expr;
                    this.Attributes.BonusDex = expr;
                    this.Attributes.BonusInt = expr;
                }
                InvalidateProperties();


            }
        }

        public override bool OnEquip(Mobile from)
        {
            // set owner if not already set -- this is only done the first time.
            if (m_Owner == null)
            {
                m_Owner = from;
                this.Name = m_Owner.Name.ToString() + "'s<BASEFONT COLOR=#2E9AFE> Evolution Chest";
                from.SendMessage("<BASEFONT COLOR=#2E9AFE>You feel the armor grow fond of you.");
                return base.OnEquip(from);
            }
            else
            {
                if (m_Owner != from)
                {
                    from.SendMessage("<BASEFONT COLOR=#2E9AFE>Sorry but this armor does not belong to you.");
                    return false;
                }

                return base.OnEquip(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(mEvolutionPoints);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            mEvolutionPoints = reader.ReadInt();
        }
    }
}
