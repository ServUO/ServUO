using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;
using Server.Items;

namespace Server.Items
{

    public class GlimmeringGranite : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringGranite() : base(0x1779)
        {
            Name = "Glimmering Granite";
            Hue = 15;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringGranite( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 Alchemy";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Alchemy, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Alchemy, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }

    public class GlimmeringClay : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringClay() : base(0x1779)
        {
            Name = "Glimmering Clay";
            Hue = 25;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringClay( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 Anatomy";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Anatomy, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Anatomy, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }

    public class GlimmeringHeartstone : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringHeartstone() : base(0x1779)
        {
            Name = "Glimmering Heartstone";
            Hue = 35;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringHeartstone( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 AnimalLore";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.AnimalLore, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.AnimalLore, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }
    
    public class GlimmeringGypsum : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringGypsum() : base(0x1779)
        {
            Name = "Glimmering Gypsum";
            Hue = 45;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringGypsum( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 ItemID";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.ItemID, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.ItemID, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }
    
    public class GlimmeringIronOre : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringIronOre() : base(0x1779)
        {
            Name = "Glimmering Iron Ore";
            Hue = 55;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringIronOre( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 ArmsLore";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.ArmsLore, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.ArmsLore, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }
    
     public class GlimmeringOnyx : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringOnyx() : base(0x1779)
        {
            Name = "Glimmering Onyx";
            Hue = 2;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringOnyx( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 Parry";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Parry, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Parry, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }
    
    public class GlimmeringMarble : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringMarble() : base(0x1779)
        {
            Name = "Glimmering Marble";
            Hue = 85;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringMarble( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 Blacksmith";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Blacksmith, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Blacksmith, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }

    public class GlimmeringPetrifiedWood : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringPetrifiedWood() : base(0x1779)
        {
            Name = "Glimmering Petrified wood";
            Hue = 85;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringPetrifiedWood( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 Fletching";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Fletching, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Fletching, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }
    
    public class GlimmeringLimestone : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringLimestone() : base(0x1779)
        {
            Name = "Glimmering Limestone";
            Hue = 85;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringLimestone( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 Peacemaking";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Peacemaking, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Peacemaking, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }
    
    public class GlimmeringBloodrock : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringBloodrock() : base(0x1779)
        {
            Name = "Glimmering Bloodrock";
            Hue = 85;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringBloodrock( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 Healing";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Healing, 5.0 );
                        break;
                    }
                }
                return true;
            } else
            if(target is BaseJewel)
            {
                BaseJewel a = target as BaseJewel;
                // find a free slot
                for(int i =0; i < 5; i++)
                {
                    if(a.SkillBonuses.GetBonus(i) == 0)
                    {
                        a.SkillBonuses.SetValues( i, SkillName.Healing, 5.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseJewel)
            {
                return true;
            }

            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
	
}
public class GlimmeringAdakite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringAdakite() : base(0x1779)
	{
		Name = "Glimmering Adakite";
		Hue = 1463;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringAdakite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Archery";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Archery, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Archery, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringArkose : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringArkose() : base(0x1779)
	{
		Name = "Glimmering Arkose";
		Hue = 1467;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringArkose(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Animal Taming";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.AnimalTaming, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.AnimalTaming, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringBasanite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringBasanite() : base(0x1779)
	{
		Name = "Glimmering Basanite";
		Hue = 1471;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringBasanite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Begging";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Begging, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Begging, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringBreccia : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringBreccia() : base(0x1779)
	{
		Name = "Glimmering Breccia";
		Hue = 1472;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringBreccia(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Bushido";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Bushido, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Bushido, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringCoal : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringCoal() : base(0x1779)
	{
		Name = "Glimmering Coal";
		Hue = 1473;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringCoal(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Camping";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Camping, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Camping, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringChert : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringChert() : base(0x1779)
	{
		Name = "Glimmering Breccia";
		Hue = 1474;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringChert(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Carpentry";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Carpentry, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Carpentry, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringCoquina : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringCoquina() : base(0x1779)
	{
		Name = "Glimmering Coquina";
		Hue = 1475;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringCoquina(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Cartography";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Cartography, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Cartography, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringCarbonatite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringCarbonatite() : base(0x1779)
	{
		Name = "Glimmering Carbonatite";
		Hue = 1279;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringCarbonatite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Chivalry";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Chivalry, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Chivalry, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringCharnockite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringCharnockite() : base(0x1779)
	{
		Name = "Glimmering Charnockite";
		Hue = 1281;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringCharnockite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Cooking";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Cooking, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Cooking, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringDacite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringDacite() : base(0x1779)
	{
		Name = "Glimmering Dacite";
		Hue = 1282;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringDacite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Detect Hidden";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.DetectHidden, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.DetectHidden, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringDolomite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringDolomite() : base(0x1779)
	{
		Name = "Glimmering Dolomite";
		Hue = 1283;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringDolomite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Discordance";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Discordance, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Discordance, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringEvaporite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringEvaporite() : base(0x1779)
	{
		Name = "Glimmering Evaporite";
		Hue = 1284;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringEvaporite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Evaluate Intelligence";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.EvalInt, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.EvalInt, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringFlint : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringFlint() : base(0x1779)
	{
		Name = "Glimmering Flint";
		Hue = 1285;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringFlint(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Fencing";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Fencing, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Fencing, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringFoidolite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringFoidolite() : base(0x1779)
	{
		Name = "Glimmering Foidolite";
		Hue = 1287;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringFoidolite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Fishing";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Fishing, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Fishing, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringGreywacke : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringGreywacke() : base(0x1779)
	{
		Name = "Glimmering Greywacke";
		Hue = 1288;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringGreywacke(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Focus";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Focus, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Focus, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringGneiss : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringGneiss() : base(0x1779)
	{
		Name = "Glimmering Gneiss";
		Hue = 1780;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringGneiss(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Forensic Evaluation";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Forensics, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Forensics, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringHarzburgite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringHarzburgite() : base(0x1779)
	{
		Name = "Glimmering Harzburgite";
		Hue = 1781;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringHarzburgite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Herding";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Herding, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Herding, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringHornblendite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringHornblendite() : base(0x1779)
	{
		Name = "Glimmering Hornblendite";
		Hue = 1782;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringHornblendite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Hiding";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Hiding, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Hiding, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringIcelandite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringIcelandite() : base(0x1779)
	{
		Name = "Glimmering Icelandite";
		Hue = 1783;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringIcelandite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Imbuing";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Imbuing, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Imbuing, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringItacolumite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringItacolumite() : base(0x1779)
	{
		Name = "Glimmering Itacolumite";
		Hue = 1784;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringItacolumite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Inscription";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Inscribe, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Inscribe, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringLaterite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringLaterite() : base(0x1779)
	{
		Name = "Glimmering Laterite";
		Hue = 1785;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringLaterite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Lockpicking";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Lockpicking, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Lockpicking, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringMonzonite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringMonzonite() : base(0x1779)
	{
		Name = "Glimmering Monzonite";
		Hue = 1786;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringMonzonite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Mace Fighting";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Macing, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Macing, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringMarl : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringMarl() : base(0x1779)
	{
		Name = "Glimmering Marl";
		Hue = 1786;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringMarl(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Magery";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Magery, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Magery, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringMudstone : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringMudstone() : base(0x1779)
	{
		Name = "Glimmering Mudstone";
		Hue = 1190;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringMudstone(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Meditation";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Meditation, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Meditation, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringMigmatite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringMigmatite() : base(0x1779)
	{
		Name = "Glimmering Migmatite";
		Hue = 1909;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringMigmatite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Mining";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Mining, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Mining, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringMylonite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringMylonite() : base(0x1779)
	{
		Name = "Glimmering Mylonite";
		Hue = 1914;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringMylonite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Musicianship";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Musicianship, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Musicianship, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringMetapelite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringMetapelite() : base(0x1779)
	{
		Name = "Glimmering Metapelite";
		Hue = 1917;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringMetapelite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Mysticism";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Mysticism, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Mysticism, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringNephelinite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringNephelinite() : base(0x1779)
	{
		Name = "Glimmering Nephelinite";
		Hue = 1920;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringNephelinite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Necromancy";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Necromancy, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Necromancy, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringNorite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringNorite() : base(0x1779)
	{
		Name = "Glimmering Norite";
		Hue = 1922;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringNorite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Ninjitsu";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Ninjitsu, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Ninjitsu, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringPegmatite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringPegmatite() : base(0x1779)
	{
		Name = "Glimmering Pegmatite";
		Hue = 1935;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringPegmatite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Poisoning";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Poisoning, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Poisoning, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringPeridotite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringPeridotite() : base(0x1779)
	{
		Name = "Glimmering Peridotite";
		Hue = 1926;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringPeridotite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Provocation";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Provocation, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Provocation, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringRhyodacite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringRhyodacite() : base(0x1779)
	{
		Name = "Glimmering Rhyidacite";
		Hue = 1929;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringRhyodacite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Remove Trap";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.RemoveTrap, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.RemoveTrap, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringRhyolite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringRhyolite() : base(0x1779)
	{
		Name = "Glimmering Rhyolite";
		Hue = 1932;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringRhyolite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Resisting Spells";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.MagicResist, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.MagicResist, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringScoria : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringScoria() : base(0x1779)
	{
		Name = "Glimmering Scoria";
		Hue = 1940;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringScoria(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Snooping";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Snooping, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Snooping, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringShonkinite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringShonkinite() : base(0x1779)
	{
		Name = "Glimmering Shonkinite";
		Hue = 1943;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringShonkinite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Spellweaving";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Spellweaving, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Spellweaving, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringSovite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringSovite() : base(0x1779)
	{
		Name = "Glimmering Sovite";
		Hue = 1947;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringSovite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Spirit Speak";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.SpiritSpeak, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.SpiritSpeak, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringSyenite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringSyenite() : base(0x1779)
	{
		Name = "Glimmering Syenite";
		Hue = 1950;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringSyenite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Stealing";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Stealing, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Stealing, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringShale : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringShale() : base(0x1779)
	{
		Name = "Glimmering Shale";
		Hue = 1954;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringShale(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Stealth";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Stealth, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Stealth, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringSylvinite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringSylvinite() : base(0x1779)
	{
		Name = "Glimmering Sylvinite";
		Hue = 1958;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringSylvinite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Swordmanship";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Swords, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Swords, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringTachylyte : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringTachylyte() : base(0x1779)
	{
		Name = "Glimmering Tachylyte";
		Hue = 1962;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringTachylyte(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Tactics";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Tactics, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Tactics, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringTephrite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringTephrite() : base(0x1779)
	{
		Name = "Glimmering Tephrite";
		Hue = 1965;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringTephrite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Tailoring";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Tailoring, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Tailoring, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringTonalite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringTonalite() : base(0x1779)
	{
		Name = "Glimmering Tonalite";
		Hue = 1969;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringTonalite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Taste Identification";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.TasteID, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.TasteID, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringTrachyte : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringTrachyte() : base(0x1779)
	{
		Name = "Glimmering Trachyte";
		Hue = 1976;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringTrachyte(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Throwing";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Throwing, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Throwing, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringTroctolite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringTroctolite() : base(0x1779)
	{
		Name = "Glimmering Troctolite";
		Hue = 1980;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringTroctolite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Tinkering";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Tinkering, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Tinkering, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringTuff : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringTuff() : base(0x1779)
	{
		Name = "Glimmering Tuff";
		Hue = 1981;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringTuff(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Tracking";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Tracking, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Tracking, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringVariolite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringVariolite() : base(0x1779)
	{
		Name = "Glimmering Variolite";
		Hue = 1990;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringVariolite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Veterinary";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Veterinary, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Veterinary, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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
public class GlimmeringWehrlite : BaseSocketAugmentation
{

	[Constructable]
	public GlimmeringWehrlite() : base(0x1779)
	{
		Name = "Glimmering Wehrlite";
		Hue = 2068;
	}

	public override int IconXOffset { get { return 5; } }

	public override int IconYOffset { get { return 20; } }

	public GlimmeringWehrlite(Serial serial) : base(serial)
	{
	}

	public override string OnIdentify(Mobile from)
	{

		return "Armor, Jewelry: +5 Wrestling";
	}

	public override bool OnAugment(Mobile from, object target)
	{
		if (target is BaseArmor)
		{
			BaseArmor a = target as BaseArmor;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Wrestling, 5.0);
					break;
				}
			}
			return true;
		}
		else
		if (target is BaseJewel)
		{
			BaseJewel a = target as BaseJewel;
			// find a free slot
			for (int i = 0; i < 5; i++)
			{
				if (a.SkillBonuses.GetBonus(i) == 0)
				{
					a.SkillBonuses.SetValues(i, SkillName.Wrestling, 5.0);
					break;
				}
			}
			return true;
		}

		return false;
	}


	public override bool CanAugment(Mobile from, object target)
	{
		if (target is BaseArmor || target is BaseJewel)
		{
			return true;
		}

		return false;
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

