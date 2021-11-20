using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

   // ---------------------------------------------------
    // Mythic wood
    // ---------------------------------------------------

    public class MythicWood : BaseSocketAugmentation, IMythicAugment
    {

        [Constructable]
        public MythicWood() : base(0x1bdd)
        {
            Name = "Mythic wood";
            Hue = 11;
        }
        
        public override int SocketsRequired {get { return 3; } }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public MythicWood( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor: +40 Lumberjacking";
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
                        a.SkillBonuses.SetValues( i, SkillName.Lumberjacking, 40.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
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

    // ---------------------------------------------------
    // Legendary wood
    // ---------------------------------------------------

    public class LegendaryWood : BaseSocketAugmentation, ILegendaryAugment
    {

        [Constructable]
        public LegendaryWood() : base(0x1bdd)
        {
            Name = "Legendary wood";
            Hue = 12;
        }
        
        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public LegendaryWood( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor: +25 Lumberjacking";
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
                        a.SkillBonuses.SetValues( i, SkillName.Lumberjacking, 25.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
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
    

    // ---------------------------------------------------
    // Ancient wood
    // ---------------------------------------------------

    public class AncientWood : BaseSocketAugmentation, IAncientAugment
    {

        [Constructable]
        public AncientWood() : base(0x1bdd)
        {
            Name = "Ancient wood";
            Hue = 15;
        }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public AncientWood( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Armor: +10 Lumberjacking";
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
                        a.SkillBonuses.SetValues( i, SkillName.Lumberjacking, 10.0 );
                        break;
                    }
                }
                return true;
            }

            return false;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
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
