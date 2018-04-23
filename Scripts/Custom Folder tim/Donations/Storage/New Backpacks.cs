
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Items
{
	public class BloodyBackpack : Backpack
    {
		public override int DefaultGumpID { get { return 0x7762; } }
		//public int GumpID { get { return m_GumpID; } set { Delta(ref m_GumpID, 0x7763); } }
		
        [Constructable]
        public BloodyBackpack()
            : base()
        {
            this.Layer = Layer.Backpack;
            this.Weight = 3.0;
            
        }
		
		
        public BloodyBackpack(Serial serial)
            : base(serial)
        {
        }
		
		
        public override int DefaultMaxWeight
        {
            get
            {
                if (Core.ML)
                {
                    Mobile m = this.ParentEntity as Mobile;
                    if (m != null && m.Player && m.Backpack == this)
                    {
                        return 550;
                    }
                    else
                    {
                        return base.DefaultMaxWeight;
                    }
                }
                else
                {
                    return base.DefaultMaxWeight;
                }
            }
        }
        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

     
        }
    }
	
	
	public class FurryBackpack : Backpack
    {
		public override int DefaultGumpID { get { return 0x7760; } }
		
        [Constructable]
        public FurryBackpack()
            : base()
        {
            this.Layer = Layer.Backpack;
            this.Weight = 3.0;
        }
		
		
        public FurryBackpack(Serial serial)
            : base(serial)
        {
        }
		
		
        public override int DefaultMaxWeight
        {
            get
            {
                if (Core.ML)
                {
                    Mobile m = this.ParentEntity as Mobile;
                    if (m != null && m.Player && m.Backpack == this)
                    {
                        return 550;
                    }
                    else
                    {
                        return base.DefaultMaxWeight;
                    }
                }
                else
                {
                    return base.DefaultMaxWeight;
                }
            }
        }
        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

     
        }
    }
	
	
	public class StrapsBackpack : Backpack
    {
		public override int DefaultGumpID { get { return 0x775E; } }
		
        [Constructable]
        public StrapsBackpack()
            : base()
        {
            this.Layer = Layer.Backpack;
            this.Weight = 3.0;
        }
		
		
        public StrapsBackpack(Serial serial)
            : base(serial)
        {
        }
		
		
        public override int DefaultMaxWeight
        {
            get
            {
                if (Core.ML)
                {
                    Mobile m = this.ParentEntity as Mobile;
                    if (m != null && m.Player && m.Backpack == this)
                    {
                        return 550;
                    }
                    else
                    {
                        return base.DefaultMaxWeight;
                    }
                }
                else
                {
                    return base.DefaultMaxWeight;
                }
            }
        }
        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

     
        }
    }
}