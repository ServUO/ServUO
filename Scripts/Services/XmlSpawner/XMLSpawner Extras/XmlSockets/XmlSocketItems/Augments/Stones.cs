using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

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
        public GlimmeringBloodrock()
            : base(0x1779)
        {
            Name = "Glimmering Bloodrock";
            Hue = 85;
        }

        public override int IconXOffset { get { return 5; } }

        public override int IconYOffset { get { return 20; } }

        public GlimmeringBloodrock(Serial serial)
            : base(serial)
        {
        }

        public override string OnIdentify(Mobile from)
        {

            return "Armor, Jewelry: +5 Healing";
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
                        a.SkillBonuses.SetValues(i, SkillName.Healing, 5.0);
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
                            a.SkillBonuses.SetValues(i, SkillName.Healing, 5.0);
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

		public class GlimmeringRockNite : BaseSocketAugmentation
    {

        [Constructable]
        public GlimmeringRockNite() : base(0x1779)
        {
            Name = "Glimmering RockNite";
            Hue = 1158;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public GlimmeringRockNite( Serial serial ) : base( serial )
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
			// BREAK ADDEDNEW SOCKETS HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH
        }
			public class GlimmeringDiamond : BaseSocketAugmentation
			{

				[Constructable]
				public GlimmeringDiamond() : base(0x1779)
				{
					Name = "Glimmering Diamond";
					Hue = 2042;
				}

				public override int IconXOffset { get { return 5; } }

				public override int IconYOffset { get { return 20; } }

				public GlimmeringDiamond(Serial serial) : base(serial)
				{
				}

				public override string OnIdentify(Mobile from)
				{

					return "Armor, Jewelry: +5 Swordsmanship";
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
			
			// ******************************************************************
            }
			public class GlimmeringKimberlite : BaseSocketAugmentation
			{

				[Constructable]
				public GlimmeringKimberlite() : base(0x1779)
				{
					Name = "Glimmering Kimberlite";
					Hue = 2006;
				}

				public override int IconXOffset { get { return 5; } }

				public override int IconYOffset { get { return 20; } }

				public GlimmeringKimberlite(Serial serial) : base(serial)
				{
				}

				public override string OnIdentify(Mobile from)
				{

					return "Armor, Jewelry: +5 Mace";
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

			//****************************************************************************************

			// ******************************************************************

			public class GlimmeringEnderbite : BaseSocketAugmentation
			{

				[Constructable]
				public GlimmeringEnderbite() : base(0x1779)
				{
					Name = "Glimmering Enderbite";
					Hue = 1208;
				}

				public override int IconXOffset { get { return 5; } }

				public override int IconYOffset { get { return 20; } }

				public GlimmeringEnderbite(Serial serial) : base(serial)
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

			//*****************************************************************************
			// ******************************************************************

			public class GlimmeringEssexite : BaseSocketAugmentation
			{

				[Constructable]
				public GlimmeringEssexite() : base(0x1779)
				{
					Name = "Glimmering Essexite";
					Hue = 1924;
				}

				public override int IconXOffset { get { return 5; } }

				public override int IconYOffset { get { return 20; } }

				public GlimmeringEssexite(Serial serial) : base(serial)
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

			// ******************************************************************

			public class GlimmeringGranophyre : BaseSocketAugmentation
			{

				[Constructable]
				public GlimmeringGranophyre() : base(0x1779)
				{
					Name = "Glimmering Granophyre";
					Hue = 33;
				}

				public override int IconXOffset { get { return 5; } }

				public override int IconYOffset { get { return 20; } }

				public GlimmeringGranophyre(Serial serial) : base(serial)
				{
				}

				public override string OnIdentify(Mobile from)
				{

					return "Armor, Jewelry: +5 Magic Resist";
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

			// ******************************************************************

			public class GlimmeringDunite : BaseSocketAugmentation
			{

				[Constructable]
				public GlimmeringDunite() : base(0x1779)
				{
					Name = "Glimmering Dunite";
					Hue = 1714;
				}

				public override int IconXOffset { get { return 5; } }

				public override int IconYOffset { get { return 20; } }

				public GlimmeringDunite(Serial serial) : base(serial)
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

			// ******************************************************************

			public class GlimmeringAluminum : BaseSocketAugmentation
			{

				[Constructable]
				public GlimmeringAluminum() : base(0x1779)
				{
					Name = "Glimmering Aluminum";
					Hue = 917;
				}

				public override int IconXOffset { get { return 5; } }

				public override int IconYOffset { get { return 20; } }

				public GlimmeringAluminum(Serial serial) : base(serial)
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

			// ******************************************************************

			public class GlimmeringBoxite : BaseSocketAugmentation
			{

				[Constructable]
				public GlimmeringBoxite() : base(0x1779)
				{
					Name = "Glimmering Boxite";
					Hue = 1148;
				}

				public override int IconXOffset { get { return 5; } }

				public override int IconYOffset { get { return 20; } }

				public GlimmeringBoxite(Serial serial) : base(serial)
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

				// ******************************************************************
            }
				public class GlimmeringCoesite : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringCoesite() : base(0x1779)
					{
						Name = "Glimmering Coesite";
						Hue = 1152;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringCoesite(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringKetite : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringKetite() : base(0x1779)
					{
						Name = "Glimmering Ketite";
						Hue = 168;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringKetite(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringPhantomQuartz : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringPhantomQuartz() : base(0x1779)
					{
						Name = "Glimmering Phantom Quartz";
						Hue = 1760;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringPhantomQuartz(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringAmertrine : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringAmertrine() : base(0x1779)
					{
						Name = "Glimmering Amertrine";
						Hue = 2308;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringAmertrine(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringCarnelian : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringCarnelian() : base(0x1779)
					{
						Name = "Glimmering Carnelian";
						Hue = 975;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringCarnelian(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringMacroCrystaline : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringMacroCrystaline() : base(0x1779)
					{
						Name = "Glimmering Macrocrystaline";
						Hue = 310;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringMacroCrystaline(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringBlueChalcedonny : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringBlueChalcedonny() : base(0x1779)
					{
						Name = "Glimmering Blue Chalcedonny";
						Hue = 268;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringBlueChalcedonny(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringCryoprase : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringCryoprase() : base(0x1779)
					{
						Name = "Glimmering Cryoprase";
						Hue = 778;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringCryoprase(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringDendritic : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringDendritic() : base(0x1779)
					{
						Name = "Glimmering Dendritic";
						Hue = 1645;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringDendritic(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringLemonQuartz : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringLemonQuartz() : base(0x1779)
					{
						Name = "Glimmering Lemon Quartz";
						Hue = 2532;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringLemonQuartz(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringDiorite : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringDiorite() : base(0x1779)
					{
						Name = "Glimmering Diorite";
						Hue = 2006;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringDiorite(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringHornblendite : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringHornblendite() : base(0x1779)
					{
						Name = "Glimmering Hornblendite";
						Hue = 1126;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringHornblendite(Serial serial) : base(serial)
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

				// ******************************************************************

				public class GlimmeringRhydocite : BaseSocketAugmentation
				{

					[Constructable]
					public GlimmeringRhydocite() : base(0x1779)
					{
						Name = "Glimmering Rhydocite";
						Hue = 2726;
					}

					public override int IconXOffset { get { return 5; } }

					public override int IconYOffset { get { return 20; } }

					public GlimmeringRhydocite(Serial serial) : base(serial)
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
			}

		
    

