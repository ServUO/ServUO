using Server.Items;


namespace Server.Multis
{
    public class NewSmallBoat : NewBaseBoat
    {		
        protected override int NorthID { get { return 0x0; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedNewBoat DockedNewBoat { get { return new DockedNewSmallBoat(this); } }		
		
        [Constructable]
        public NewSmallBoat()
            : base(0x0, 100, new Point3D(1, 4, 0), new Point3D(-2, 0, 0), new Point3D(2, 0, 0), new Point3D(0, -4, 0))
        {
        }

        public NewSmallBoat(Serial serial)
            : base(serial)
        {
        }
		
        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 1, 3);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

	public class NewSmallBoatDeed : NewBaseBoatDeed
	{
		public override NewBaseBoat Boat{ get{ return new NewSmallBoat(); } }

		[Constructable]
		public NewSmallBoatDeed() : base( 0x0, new Point3D( 0, -1, 0 ) )
		{
			Name = "New Small Boat Deed";
		}

		public NewSmallBoatDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}
	}
	
	public class DockedNewSmallBoat : BaseDockedNewBoat
	{
		public override NewBaseBoat Boat{ get{ return new NewSmallBoat(); } }

		public DockedNewSmallBoat( NewBaseBoat boat ) : base( 0x0, Point3D.Zero, boat )
		{
		}

		public DockedNewSmallBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}	

    public class NewSmallDragonBoat : NewBaseBoat
    {
        protected override int NorthID { get { return 0x4; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedNewBoat DockedNewBoat { get { return new DockedNewSmallDragonBoat(this); } }		
		
        [Constructable]
        public NewSmallDragonBoat()
            : base(0x4, 100, new Point3D(1, 4, 0), new Point3D(-2, 0, 0), new Point3D(2, 0, 0), new Point3D(0, -4, 0))
        {
        }

        public NewSmallDragonBoat(Serial serial)
            : base(serial)
        {
        }
		
        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 1, 3);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class NewSmallDragonBoatDeed : NewBaseBoatDeed
    {
        public override NewBaseBoat Boat { get { return new NewSmallDragonBoat(); } }

        [Constructable]
        public NewSmallDragonBoatDeed()
            : base(0x4, new Point3D(0, -1, 0))
        {
            Name = "New Small Dragon Boat Deed";
        }

        public NewSmallDragonBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }
	
	public class DockedNewSmallDragonBoat : BaseDockedNewBoat
	{
		public override NewBaseBoat Boat{ get{ return new NewSmallDragonBoat(); } }

		public DockedNewSmallDragonBoat( NewBaseBoat boat ) : base( 0x4, Point3D.Zero, boat )
		{
		}

		public DockedNewSmallDragonBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}		

    public class NewMediumBoat : NewBaseBoat
    {
        protected override int NorthID { get { return 0x8; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedNewBoat DockedNewBoat { get { return new DockedNewMediumBoat(this); } }			
		
        [Constructable]
        public NewMediumBoat()
            : base(0x8, 100, new Point3D(1, 5, 0), new Point3D(-2, 0, 0), new Point3D(2, 0, 0), new Point3D(0, -4, 0))
        {
        }

        public NewMediumBoat(Serial serial)
            : base(serial)
        {
        }
		
        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 1, 3);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class NewMediumBoatDeed : NewBaseBoatDeed
    {
        public override NewBaseBoat Boat { get { return new NewMediumBoat(); } }

        [Constructable]
        public NewMediumBoatDeed()
            : base(0x8, new Point3D(0, -1, 0))
        {
            Name = "New Medium Boat Deed";
        }

        public NewMediumBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }
	
	public class DockedNewMediumBoat : BaseDockedNewBoat
	{
		public override NewBaseBoat Boat{ get{ return new NewMediumBoat(); } }

		public DockedNewMediumBoat( NewBaseBoat boat ) : base( 0x8, Point3D.Zero, boat )
		{
		}

		public DockedNewMediumBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}		

    public class NewMediumDragonBoat : NewBaseBoat
    {
        protected override int NorthID { get { return 0xC; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedNewBoat DockedNewBoat { get { return new DockedNewMediumDragonBoat(this); } }		
		
        [Constructable]
        public NewMediumDragonBoat()
            : base(0xC, 100, new Point3D(1, 5, 0), new Point3D(-2, 0, 0), new Point3D(2, 0, 0), new Point3D(0, -4, 0))
        {
        }

        public NewMediumDragonBoat(Serial serial)
            : base(serial)
        {
        }

        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 1, 3);
            }
        }		
		
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class NewMediumDragonBoatDeed : NewBaseBoatDeed
    {
        public override NewBaseBoat Boat { get { return new NewMediumDragonBoat(); } }

        [Constructable]
        public NewMediumDragonBoatDeed()
            : base(0xC, new Point3D(0, -1, 0))
        {
            Name = "New Medium Dragon Boat Deed";
        }

        public NewMediumDragonBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }
	
	public class DockedNewMediumDragonBoat : BaseDockedNewBoat
	{
		public override NewBaseBoat Boat{ get{ return new NewMediumDragonBoat(); } }

		public DockedNewMediumDragonBoat( NewBaseBoat boat ) : base( 0xC, Point3D.Zero, boat )
		{
		}

		public DockedNewMediumDragonBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}		

    public class NewLargeBoat : NewBaseBoat
    {
        protected override int NorthID { get { return 0x10; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedNewBoat DockedNewBoat { get { return new DockedNewLargeBoat(this); } }		
		
        [Constructable]
        public NewLargeBoat()
            : base(0x10, 100, new Point3D(1, 5, 0), new Point3D(-2, -1, 0), new Point3D(2, -1, 0), new Point3D(0, -5, 0))
        {
        }

        public NewLargeBoat(Serial serial)
            : base(serial)
        {
        }
		
        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 0, 3);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class NewLargeBoatDeed : NewBaseBoatDeed
    {
        public override NewBaseBoat Boat { get { return new NewLargeBoat(); } }

        [Constructable]
        public NewLargeBoatDeed()
            : base(0x10, new Point3D(0, -1, 0))
        {
            Name = "New Large Boat Deed";
        }

        public NewLargeBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }
	
	public class DockedNewLargeBoat : BaseDockedNewBoat
	{
		public override NewBaseBoat Boat{ get{ return new NewLargeBoat(); } }

		public DockedNewLargeBoat( NewBaseBoat boat ) : base( 0x10, Point3D.Zero, boat )
		{
		}

		public DockedNewLargeBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}		

    public class NewLargeDragonBoat : NewBaseBoat
    {
        protected override int NorthID { get { return 0x14; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedNewBoat DockedNewBoat { get { return new DockedNewLargeDragonBoat(this); } }
		
        [Constructable]
        public NewLargeDragonBoat()
            : base(0x14, 100, new Point3D(1, 5, 0), new Point3D(-2, -1, 0), new Point3D(2, -1, 0), new Point3D(0, -5, 0))
        {
        }

        public NewLargeDragonBoat(Serial serial)
            : base(serial)
        {
        }
		
        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 0, 3);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class NewLargeDragonBoatDeed : NewBaseBoatDeed
    {
        public override NewBaseBoat Boat { get { return new NewLargeDragonBoat(); } }

        [Constructable]
        public NewLargeDragonBoatDeed()
            : base(0x14, new Point3D(0, -1, 0))
        {
            Name = "New Large Dragon Boat Deed";
        }

        public NewLargeDragonBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }
	
	public class DockedNewLargeDragonBoat : BaseDockedNewBoat
	{
		public override NewBaseBoat Boat{ get{ return new NewLargeDragonBoat(); } }

		public DockedNewLargeDragonBoat( NewBaseBoat boat ) : base( 0x14, Point3D.Zero, boat )
		{
		}

		public DockedNewLargeDragonBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}		
}
