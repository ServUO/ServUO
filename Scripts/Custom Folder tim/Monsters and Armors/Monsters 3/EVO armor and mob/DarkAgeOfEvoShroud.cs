using System;
using Server;

namespace Server.Items
{
	public class DarkAgeOfEvoShroud : HoodedShroudOfShadows
	{
		public override int ArtifactRarity{ get{ return 13; } }

		public override int BasePhysicalResistance{ get{ return 15; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }
        private Mobile m_Owner;
		[Constructable]
		public DarkAgeOfEvoShroud()
		{
            Name = "<BASEFONT COLOR=#2E9AFE>Evo Shroud";
			Hue = 1910;
                        Attributes.LowerManaCost = 10;
                        Attributes.LowerRegCost = 15;
			Attributes.BonusHits = 10;
                        Attributes.BonusMana = 10;
                        Attributes.BonusStam = 10;
			Attributes.Luck = 150;
			Attributes.ReflectPhysical = 15;
		}

        public override bool OnEquip(Mobile from)
        {
            // set owner if not already set -- this is only done the first time.
            if (m_Owner == null)
            {
                m_Owner = from;
                this.Name = m_Owner.Name.ToString() + "'s Shroud";
                from.SendMessage("You feel the robe grow fond of you.");
                return base.OnEquip(from);
            }
            else
            {
                if (m_Owner != from)
                {
                    from.SendMessage("Sorry but this robe does not belong to you.");
                    return false;
                }

                return base.OnEquip(from);
            }
        }

		public DarkAgeOfEvoShroud( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

		}
	}
}