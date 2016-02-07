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
