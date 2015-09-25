using System;
using System.Collections.Generic;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
    public class TokunoGalleon : BaseGalleon
    {
        #region Static Field
        private static int[] tokunoMultiIDs = new int[] { 0x38, 0x34, 0x30 }; // low, mid, full
        private static int[,] tokunoItemIDMods = new int[,] 
        {
            { 1330, 1195, 1470, 1610 }, // low
            { 732, 867, 462, 597 },     // mid
            { 0, 135, -270, -135 },     // full
        };

        protected override int[] multiIDs { get { return tokunoMultiIDs; } }
        protected override int[,] itemIDMods { get { return tokunoItemIDMods; } }
        #endregion

        private SingleCannonPlace _cannonCenter;
        private SingleCannonPlace _cannonSupSx;
        private SingleCannonPlace _cannonSupDx;
        private SingleCannonPlace _cannonInfSx;
        private SingleCannonPlace _cannonInfDx;
        private IHelm _helm;
        private GalleonHold _hold;
		private BoatRope _pRope, _sRope;
		private TillerManHS _tillerMan;	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedGalleon DockedGalleon { get { return new DockedTokunoGalleon(this); } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonCenter { get { return _cannonCenter; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupSx { get { return _cannonSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupDx { get { return _cannonSupDx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfSx { get { return _cannonInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfDx { get { return _cannonInfDx; } }			
	
        [Constructable]
        public TokunoGalleon()
            : base(0x30)
        {
            _cannonCenter = new SingleCannonPlace(this, 0x91CC, new Point3D(0, -9, 0));
            _cannonSupSx = new SingleCannonPlace(this, 0x91AA, new Point3D(-2, -3, 0));
            _cannonSupDx = new SingleCannonPlace(this, 0x91A6, new Point3D(2, -3, 0));
            _cannonInfSx = new SingleCannonPlace(this, 0x9187, new Point3D(-2, 2, 0));
            _cannonInfDx = new SingleCannonPlace(this, 0x9183, new Point3D(2, 2, 0));
            _helm = new SingleHelm(this, 0x9316, new Point3D(0, 7, 1));
            _hold = new GalleonHold(this, 0x9177, new Point3D(0, 4, 0),
                new List<Tuple<int, Point3D>>() 
                {
                    new Tuple<int, Point3D>(0x9170, new Point3D(0, 1, 0)),
                    new Tuple<int, Point3D>(0x9178, new Point3D(-1, 0, 0)),
                    new Tuple<int, Point3D>(0x9171, new Point3D(-1, 1, 0)),
                    new Tuple<int, Point3D>(0x9176, new Point3D(1, 0, 0)),
                    new Tuple<int, Point3D>(0x916F, new Point3D(1, 1, 0)),                    
                }, 16000);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-2, -2, 6), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(2, -2, 6), BoatRopeSide.Port, 0 ); 
			Ropes.Add(_pRope);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-2, 3, 6), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(2, 3, 6), BoatRopeSide.Port, 0 );
			Ropes.Add(_pRope);
			_tillerMan = new TillerManHS(this, 0, new Point3D(0, -2, 6) );
			Name = "a Tokuno Galleon";
			
            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)
            XmlAttach.AttachTo(this, new XmlBoatFight(100000, 10, 10, 20, 30, 0));

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            Hue = 0;			
			
        }

        public TokunoGalleon(Serial serial)
            : base(serial)
        {
        }
		
        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, -1, 7);
            }
        }

        protected override void OnStatusChange()
        {
            _cannonCenter.RefreshItemID(CurrentItemIdModifier);
			if (_cannonCenter.LinkedLightCannon != null)
				_cannonCenter.LinkedLightCannon.RefreshItemID(CannonItemIdModifier);
				
			if (_cannonCenter.LinkedHeavyCannon != null)
				_cannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
            _cannonSupSx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonSupSx.LinkedLightCannon != null)
				_cannonSupSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (_cannonSupSx.LinkedHeavyCannon != null)
				_cannonSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            _cannonSupDx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonSupDx.LinkedLightCannon != null)
				_cannonSupDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (_cannonSupDx.LinkedHeavyCannon != null)
				_cannonSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
			
            _cannonInfSx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonInfSx.LinkedLightCannon != null)
				_cannonInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (_cannonInfSx.LinkedHeavyCannon != null)
				_cannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            _cannonInfDx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonInfDx.LinkedLightCannon != null)
				_cannonInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (_cannonInfDx.LinkedHeavyCannon != null)
				_cannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
			
            _hold.RefreshItemID(CurrentItemIdModifier);

            int newModifier = 0;
            if (Status != GalleonStatus.Low)
            {
                switch (Facing)
                {
                    //case Server.Direction.North: newModifier = 0; break;
                    case Server.Direction.East: newModifier = 2; break;
                    case Server.Direction.South: newModifier = -4; break;
                    case Server.Direction.West: newModifier = -2; break;
                }
            }
            else
            {
                switch (Facing)
                {
                    //case Server.Direction.North: newModifier = 0; break;
                    case Server.Direction.East: newModifier = -3; break;
                    case Server.Direction.South: newModifier = 2; break;
                    case Server.Direction.West: newModifier = 4; break;
                }
            }

            switch (Status)
            {
                case GalleonStatus.Half: newModifier += 732; break;
                case GalleonStatus.Low: newModifier += 1494; break;
            }

            _helm.RefreshItemID(newModifier);
        }
		

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _cannonCenter = reader.ReadItem() as SingleCannonPlace;
            _cannonSupSx = reader.ReadItem() as SingleCannonPlace;
            _cannonSupDx = reader.ReadItem() as SingleCannonPlace;
            _cannonInfSx = reader.ReadItem() as SingleCannonPlace;
            _cannonInfDx = reader.ReadItem() as SingleCannonPlace;
            _helm = reader.ReadItem() as SingleHelm;
            _hold = reader.ReadItem() as GalleonHold;		
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((Item)_cannonCenter);
            writer.Write((Item)_cannonSupSx);
            writer.Write((Item)_cannonSupDx);
            writer.Write((Item)_cannonInfSx);
            writer.Write((Item)_cannonInfDx);
            writer.Write((Item)_helm);
            writer.Write((Item)_hold);
        }
        #endregion
    }
	
	public class TokunoGalleonDeed : BaseGalleonDeed
	{
        public override BaseGalleon Boat { get { return new TokunoGalleon(); } }

		[Constructable]
		public TokunoGalleonDeed() : base( 0x30, new Point3D( 0, -1, 0 ) )
		{
			Name = "Tokuno Galleon Deed";
		}

		public TokunoGalleonDeed( Serial serial ) : base( serial )
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
	
	public class DockedTokunoGalleon : BaseDockedGalleon
	{
		public override BaseGalleon Galleon{ get{ return new TokunoGalleon(); } }

		public DockedTokunoGalleon( BaseGalleon boat ) : base( 0x30, Point3D.Zero, boat )
		{
		}

		public DockedTokunoGalleon( Serial serial ) : base( serial )
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

    public class GargoyleGalleon : BaseGalleon
    {
        #region Static Field
        private static int[] gargoyleMultiIDs = new int[] { 0x2C, 0x28, 0x24 };
        private static int[,] gargoyleItemIDMods = new int[,] 
        {
            { 2076, 1092, 2378, 2680 },
            { 908, 604, -15005, 1512 },
            { 0, 303, -607, -302 },
        };

        protected override int[] multiIDs { get { return gargoyleMultiIDs; } }
        protected override int[,] itemIDMods { get { return gargoyleItemIDMods; } }
        #endregion

		
        private SingleCannonPlace _cannonCenter;
        private SingleCannonPlace _cannonSupSx;
        private SingleCannonPlace _cannonSupDx;
        private SingleCannonPlace _cannonMidSx;
        private SingleCannonPlace _cannonMidDx;
        private SingleCannonPlace _cannonInfSx;
        private SingleCannonPlace _cannonInfDx;
        private IHelm _helm;
        private GalleonHold _hold;
		private BoatRope _pRope, _sRope;	
		private TillerManHS _tillerMan;			

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedGalleon DockedGalleon { get { return new DockedGargoyleGalleon(this); } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonCenter { get { return _cannonCenter; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupSx { get { return _cannonSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupDx { get { return _cannonSupDx; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidSx { get { return _cannonMidSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidDx { get { return _cannonMidDx; } }			

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfSx { get { return _cannonInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfDx { get { return _cannonInfDx; } }	
		
        [Constructable]
        public GargoyleGalleon()
            : base(0x24)
        {
            _cannonCenter = new SingleCannonPlace(this, 0x8516, new Point3D(0, -8, 0));
            _cannonSupSx = new SingleCannonPlace(this, 0x84FD, new Point3D(-2, -5, 0));
            _cannonSupDx = new SingleCannonPlace(this, 0x84FF, new Point3D(2, -5, 0));
            _cannonMidSx = new SingleCannonPlace(this, 0x8489, new Point3D(-2, -2, 0));
            _cannonMidDx = new SingleCannonPlace(this, 0x848E, new Point3D(2, -2, 0));
            _cannonInfSx = new SingleCannonPlace(this, 0x84AA, new Point3D(-2, 1, 0));
            _cannonInfDx = new SingleCannonPlace(this, 0x84AC, new Point3D(2, 1, 0));
            _helm = new SingleHelm(this, 0x85A0, new Point3D(0, 2, 0));
            _hold = new GalleonHold(this, 0x84CA, new Point3D(0, 5, 0),
                new List<Tuple<int, Point3D>>() 
                {
                    new Tuple<int, Point3D>(0x84CC, new Point3D(-2, 0, 0)), // stiva lato sx 1/4
                    new Tuple<int, Point3D>(0x84D3, new Point3D(-2, 1, 0)), // stiva lato sx 2/4
                    new Tuple<int, Point3D>(0x84DA, new Point3D(-2, 2, 0)), // stiva lato sx 3/4
                    new Tuple<int, Point3D>(0x84E1, new Point3D(-2, 3, 0)), // stiva lato sx 4/4

                    new Tuple<int, Point3D>(0x84CB, new Point3D(-1, 0, 0)), // stiva sup sx       
                    new Tuple<int, Point3D>(0x84D2, new Point3D(-1, 1, 0)), // stiva Centro sx 1/2
                    new Tuple<int, Point3D>(0x84D9, new Point3D(-1, 2, 0)), // stiva Centro sx 2/2
                    new Tuple<int, Point3D>(0x84E0, new Point3D(-1, 3, 0)), // stiva inf sx
   
                    //new Tuple<int, Point3D>(0x84CA, new Point3D(0, 0, 0)),  // stiva sup ( container )
                    new Tuple<int, Point3D>(0x84D1, new Point3D(0, 1, 0)),  // stiva Centro 1/2
                    new Tuple<int, Point3D>(0x84D8, new Point3D(0, 2, 0)),  // stiva Centro 2/2 
                    new Tuple<int, Point3D>(0x84DF, new Point3D(0, 3, 0)),  // stiva inf

                    new Tuple<int, Point3D>(0x84D0, new Point3D(1, 0, 0)), // stiva sup dx 
                    new Tuple<int, Point3D>(0x84D7, new Point3D(1, 1, 0)), // stiva Centro dx 1/2
                    new Tuple<int, Point3D>(0x84DE, new Point3D(1, 2, 0)), // stiva Centro dx 2/2
                    new Tuple<int, Point3D>(0x84E5, new Point3D(1, 3, 0)), // stiva inf dx

                    new Tuple<int, Point3D>(0x84CE, new Point3D(2, 0, 0)), // stiva lato dx 1/4
                    new Tuple<int, Point3D>(0x84D5, new Point3D(2, 1, 0)), // stiva lato dx 2/4
                    new Tuple<int, Point3D>(0x84DC, new Point3D(2, 2, 0)), // stiva lato dx 3/4
                    new Tuple<int, Point3D>(0x84E3, new Point3D(2, 3, 0)), // stiva lato dx 4/4
                }, 12000);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-2, -4, 14), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(2, -4, 14), BoatRopeSide.Port, 0 ); 
			Ropes.Add(_pRope);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-2, -1, 14), BoatRopeSide.Starboard, 0 );
			Ropes.Add(_sRope);			
			_pRope = new BoatRope(this, 0x14F8, new Point3D(2, -1, 14), BoatRopeSide.Port, 0 );
			Ropes.Add(_pRope);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-2, 2, 14), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(2, 2, 14), BoatRopeSide.Port, 0 );
			Ropes.Add(_pRope);
			_tillerMan = new TillerManHS(this, 0, new Point3D(0, -4, 12) );
			Name = "a Gargoyle Galleon";
			
            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)
            XmlAttach.AttachTo(this, new XmlBoatFight(140000, 10, 10, 20, 30, 0));

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            Hue = 0;			
			
        }

        public GargoyleGalleon(Serial serial)
            : base(serial)
        {
        }

        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, -2, 16);
            }
        }		
		
        protected override void OnStatusChange()
        {
				
            _cannonSupSx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonSupSx.LinkedLightCannon != null)
				_cannonSupSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (_cannonSupSx.LinkedHeavyCannon != null)
				_cannonSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            _cannonSupDx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonSupDx.LinkedLightCannon != null)
				_cannonSupDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (_cannonSupDx.LinkedHeavyCannon != null)
				_cannonSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);

            if (Status == GalleonStatus.Low || (Status == GalleonStatus.Half && (Facing == Server.Direction.South)))
            {
                _cannonCenter.RefreshItemID(CurrentItemIdModifier - 6);
				if (_cannonCenter.LinkedLightCannon != null)
					_cannonCenter.LinkedLightCannon.RefreshItemID(CannonItemIdModifier);
					
				if (_cannonCenter.LinkedHeavyCannon != null)
					_cannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
			
                _cannonMidSx.RefreshItemID(CurrentItemIdModifier + 32);
				if (_cannonMidSx.LinkedLightCannon != null)
					_cannonMidSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonMidSx.LinkedHeavyCannon != null)
					_cannonMidSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonMidDx.RefreshItemID(CurrentItemIdModifier + 29);
				if (_cannonMidDx.LinkedLightCannon != null)
					_cannonMidDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonMidDx.LinkedHeavyCannon != null)
					_cannonMidDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _cannonInfSx.RefreshItemID(CurrentItemIdModifier + 20);
				if (_cannonInfSx.LinkedLightCannon != null)
					_cannonInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonInfSx.LinkedHeavyCannon != null)
					_cannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonInfDx.RefreshItemID(CurrentItemIdModifier + 20);
				if (_cannonInfDx.LinkedLightCannon != null)
					_cannonInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonInfDx.LinkedHeavyCannon != null)
					_cannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _hold.RefreshItemID(CurrentItemIdModifier + 14);
            }
            else
            {
                _cannonCenter.RefreshItemID(CurrentItemIdModifier);
				if (_cannonCenter.LinkedLightCannon != null)
					_cannonCenter.LinkedLightCannon.RefreshItemID(CannonItemIdModifier);
					
				if (_cannonCenter.LinkedHeavyCannon != null)
					_cannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
                _cannonMidSx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonMidSx.LinkedLightCannon != null)
					_cannonMidSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonMidSx.LinkedHeavyCannon != null)
					_cannonMidSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonMidDx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonMidDx.LinkedLightCannon != null)
					_cannonMidDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonMidDx.LinkedHeavyCannon != null)
					_cannonMidDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _cannonInfSx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonInfSx.LinkedLightCannon != null)
					_cannonInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonInfSx.LinkedHeavyCannon != null)
					_cannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonInfDx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonInfDx.LinkedLightCannon != null)
					_cannonInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonInfDx.LinkedHeavyCannon != null)
					_cannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _hold.RefreshItemID(CurrentItemIdModifier);
            }

            int newModifier = 0;
            switch (Status)
            {
                case GalleonStatus.Full:
                    switch (Facing)
                    {
                        //case Server.Direction.North: newModifier = 0; break;
                        case Server.Direction.East: newModifier = 302; break;
                        case Server.Direction.South: newModifier = -604; break;
                        case Server.Direction.West: newModifier = -302; break;
                    }
                    break;
                case GalleonStatus.Half:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 5004; break;
                        case Server.Direction.East: newModifier = 604; break;
                        case Server.Direction.South: newModifier = -15020; break;
                        case Server.Direction.West: newModifier = 1510; break;
                    }
                    break;
                case GalleonStatus.Low:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 2114; break;
                        case Server.Direction.East: newModifier = 1812; break;
                        case Server.Direction.South: newModifier = 2415; break;
                        case Server.Direction.West: newModifier = 2715; break;
                    }
                    break;
            }

            _helm.RefreshItemID(newModifier);
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _cannonCenter = reader.ReadItem() as SingleCannonPlace;
            _cannonSupSx = reader.ReadItem() as SingleCannonPlace;
            _cannonSupDx = reader.ReadItem() as SingleCannonPlace;
            _cannonMidSx = reader.ReadItem() as SingleCannonPlace;
            _cannonMidDx = reader.ReadItem() as SingleCannonPlace;
            _cannonInfSx = reader.ReadItem() as SingleCannonPlace;
            _cannonInfDx = reader.ReadItem() as SingleCannonPlace;
            _helm = reader.ReadItem() as SingleHelm;
            _hold = reader.ReadItem() as GalleonHold;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((Item)_cannonCenter);
            writer.Write((Item)_cannonSupSx);
            writer.Write((Item)_cannonSupDx);
            writer.Write((Item)_cannonMidSx);
            writer.Write((Item)_cannonMidDx);
            writer.Write((Item)_cannonInfSx);
            writer.Write((Item)_cannonInfDx);
            writer.Write((Item)_helm);
            writer.Write((Item)_hold);
        }
        #endregion
    }
	
	public class GargoyleGalleonDeed : BaseGalleonDeed
	{
        public override BaseGalleon Boat { get { return new GargoyleGalleon(); } }

		[Constructable]
		public GargoyleGalleonDeed() : base( 0x24, new Point3D( 0, -1, 0 ) )
		{
			Name = "Gargoyle Galleon Deed";
		}

		public GargoyleGalleonDeed( Serial serial ) : base( serial )
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
	
	public class DockedGargoyleGalleon : BaseDockedGalleon
	{
		public override BaseGalleon Galleon{ get{ return new GargoyleGalleon(); } }

		public DockedGargoyleGalleon( BaseGalleon boat ) : base( 0x24, Point3D.Zero, boat )
		{
		}

		public DockedGargoyleGalleon( Serial serial ) : base( serial )
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

    public class OrcGalleon : BaseGalleon
    {
        #region Static Field
        private static int[] orcMultiIDs = new int[] { 0x20, 0x1C, 0x18, };
        private static int[,] orcItemIDMods = new int[,] 
        {
            { 1100, 1300, 700, 901 },
            { 1900, 2100, 1500, 1700 },
            { 0, 500, -1000, -500 },
        };

        protected override int[] multiIDs { get { return orcMultiIDs; } }
        protected override int[,] itemIDMods { get { return orcItemIDMods; } }
        #endregion

        private SingleCannonPlace _cannonCenter;
        private MultiCannonPlace _cannonSupSx;
        private MultiCannonPlace _cannonSupDx;
        private SingleCannonPlace _cannonMidSx;
        private SingleCannonPlace _cannonMidDx;
        private MultiCannonPlace _cannonInfSx;
        private MultiCannonPlace _cannonInfDx;
        private IHelm _helm;
        private GalleonHold _hold;
		private BoatRope _pRope, _sRope;
		private TillerManHS _tillerMan;

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedGalleon DockedGalleon { get { return new DockedOrcGalleon(this); } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonCenter { get { return _cannonCenter; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace CannonSupSx { get { return _cannonSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace CannonSupDx { get { return _cannonSupDx; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidSx { get { return _cannonMidSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidDx { get { return _cannonMidDx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace CannonInfSx { get { return _cannonInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace CannonInfDx { get { return _cannonInfDx; } }			
		
        [Constructable]
        public OrcGalleon()
            : base(0x18)
        {
            _cannonCenter = new SingleCannonPlace(this, 0x7924, new Point3D(0, -6, 0));
            _cannonSupSx = new MultiCannonPlace(this, 0x793D, new Point3D(-2, -2, 0), 0x7944, 0x7936);
            _cannonSupDx = new MultiCannonPlace(this, 0x7941, new Point3D(2, -2, 0), 0x7948, 0x793A);
            _cannonMidSx = new SingleCannonPlace(this, 0x7959, new Point3D(-2, 2, 0));
            _cannonMidDx = new SingleCannonPlace(this, 0x795D, new Point3D(2, 2, 0));
            _cannonInfSx = new MultiCannonPlace(this, 0x796E, new Point3D(-2, 5, 0), 0x7975);
            _cannonInfDx = new MultiCannonPlace(this, 0x7972, new Point3D(2, 5, 0), 0x7979);
            _helm = new MultiHelm(this, 0x79A5, new Point3D(0, 7, 1), 0x79A4, 0x79A6);
            _hold = new GalleonHold(this, 0x798D, new Point3D(0, 9, 0),
                new List<Tuple<int, Point3D>>() 
                {
                    new Tuple<int, Point3D>(0x7994, new Point3D(0, 1, 0)),
                    new Tuple<int, Point3D>(0x798B, new Point3D(-1, 0, 0)),
                    new Tuple<int, Point3D>(0x7992, new Point3D(-1, 1, 0)),
                    new Tuple<int, Point3D>(0x7990, new Point3D(1, 0, 0)),
                    new Tuple<int, Point3D>(0x7997, new Point3D(1, 1, 0)),                    
                }, 14000);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-2, -1, 14), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(2, -1, 14), BoatRopeSide.Port, 0 ); 
			Ropes.Add(_pRope);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-2, 3, 14), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(2, 3, 14), BoatRopeSide.Port, 0 );
			Ropes.Add(_pRope);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-2, 7, 14), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(2, 7, 14), BoatRopeSide.Port, 0 );
			Ropes.Add(_pRope);
			_tillerMan = new TillerManHS(this, 0, new Point3D(0, 2, 10) );
			Name = "an Orc Galleon";
            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)
            XmlAttach.AttachTo(this, new XmlBoatFight(100000, 10, 10, 20, 30, 0));

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            Hue = 0;	
        }

        public OrcGalleon(Serial serial)
            : base(serial)
        {
        }

        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 4, 14);
            }
        }
		
        protected override void OnStatusChange()
        {
            _cannonSupSx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonSupSx.LinkedLightCannon != null)
				_cannonSupSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (_cannonSupSx.LinkedHeavyCannon != null)
				_cannonSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            _cannonSupDx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonSupDx.LinkedLightCannon != null)
				_cannonSupDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (_cannonSupDx.LinkedHeavyCannon != null)
				_cannonSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
			

            if (Status == GalleonStatus.Low || (Status == GalleonStatus.Half && (Facing == Server.Direction.South)))
            {
                _cannonCenter.RefreshItemID(CurrentItemIdModifier - 6);
				if (_cannonCenter.LinkedLightCannon != null)
					_cannonCenter.LinkedLightCannon.RefreshItemID(CannonItemIdModifier);
					
				if (_cannonCenter.LinkedHeavyCannon != null)
					_cannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
			
                _cannonMidSx.RefreshItemID(CurrentItemIdModifier + 32);
				if (_cannonMidSx.LinkedLightCannon != null)
					_cannonMidSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonMidSx.LinkedHeavyCannon != null)
					_cannonMidSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonMidDx.RefreshItemID(CurrentItemIdModifier + 29);
				if (_cannonMidDx.LinkedLightCannon != null)
					_cannonMidDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonMidDx.LinkedHeavyCannon != null)
					_cannonMidDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _cannonInfSx.RefreshItemID(CurrentItemIdModifier + 20);
				if (_cannonInfSx.LinkedLightCannon != null)
					_cannonInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonInfSx.LinkedHeavyCannon != null)
					_cannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonInfDx.RefreshItemID(CurrentItemIdModifier + 20);
				if (_cannonInfDx.LinkedLightCannon != null)
					_cannonInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonInfDx.LinkedHeavyCannon != null)
					_cannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _hold.RefreshItemID(CurrentItemIdModifier + 14);
            }
            else
            {
                _cannonCenter.RefreshItemID(CurrentItemIdModifier);
				if (_cannonCenter.LinkedLightCannon != null)
					_cannonCenter.LinkedLightCannon.RefreshItemID(CannonItemIdModifier);
					
				if (_cannonCenter.LinkedHeavyCannon != null)
					_cannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
                _cannonMidSx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonMidSx.LinkedLightCannon != null)
					_cannonMidSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonMidSx.LinkedHeavyCannon != null)
					_cannonMidSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonMidDx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonMidDx.LinkedLightCannon != null)
					_cannonMidDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonMidDx.LinkedHeavyCannon != null)
					_cannonMidDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _cannonInfSx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonInfSx.LinkedLightCannon != null)
					_cannonInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonInfSx.LinkedHeavyCannon != null)
					_cannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonInfDx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonInfDx.LinkedLightCannon != null)
					_cannonInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonInfDx.LinkedHeavyCannon != null)
					_cannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _hold.RefreshItemID(CurrentItemIdModifier);
            }

            int newModifier = 0;
            switch (Status)
            {
                case GalleonStatus.Full:
                    switch (Facing)
                    {
                        //case Server.Direction.North: newModifier = 0; break;
                        case Server.Direction.East: newModifier = 501; break;
                        case Server.Direction.South: newModifier = -1000; break;
                        case Server.Direction.West: newModifier = -500; break;
                    }
                    break;
                case GalleonStatus.Half:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 5004; break;
                        case Server.Direction.East: newModifier = 604; break;
                        case Server.Direction.South: newModifier = -15020; break;
                        case Server.Direction.West: newModifier = 1510; break;
                    }
                    break;
                case GalleonStatus.Low:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 2114; break;
                        case Server.Direction.East: newModifier = 1812; break;
                        case Server.Direction.South: newModifier = 2415; break;
                        case Server.Direction.West: newModifier = 2715; break;
                    }
                    break;
            }

            _helm.RefreshItemID(newModifier);
        }		

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _cannonCenter = reader.ReadItem() as SingleCannonPlace;
            _cannonSupSx = reader.ReadItem() as MultiCannonPlace;
            _cannonSupDx = reader.ReadItem() as MultiCannonPlace;
            _cannonMidSx = reader.ReadItem() as SingleCannonPlace;
            _cannonMidDx = reader.ReadItem() as SingleCannonPlace;
            _cannonInfSx = reader.ReadItem() as MultiCannonPlace;
            _cannonInfDx = reader.ReadItem() as MultiCannonPlace;
            _helm = reader.ReadItem() as MultiHelm;
            _hold = reader.ReadItem() as GalleonHold;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((Item)_cannonCenter);
            writer.Write((Item)_cannonSupSx);
            writer.Write((Item)_cannonSupDx);
            writer.Write((Item)_cannonMidSx);
            writer.Write((Item)_cannonMidDx);
            writer.Write((Item)_cannonInfSx);
            writer.Write((Item)_cannonInfDx);
            writer.Write((Item)_helm);
            writer.Write((Item)_hold);
        }
        #endregion
    }
	
	public class OrcGalleonDeed : BaseGalleonDeed
	{
        public override BaseGalleon Boat { get { return new OrcGalleon(); } }

		[Constructable]
		public OrcGalleonDeed() : base( 0x18, new Point3D( 0, -1, 0 ) )
		{
			Name = "Orc Galleon Deed";
		}

		public OrcGalleonDeed( Serial serial ) : base( serial )
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
	
	public class DockedOrcGalleon : BaseDockedGalleon
	{
		public override BaseGalleon Galleon{ get{ return new OrcGalleon(); } }

		public DockedOrcGalleon( BaseGalleon boat ) : base( 0x18, Point3D.Zero, boat )
		{
		}

		public DockedOrcGalleon( Serial serial ) : base( serial )
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

    public class BritainGalleon : BaseGalleon
    {
        #region Static Field
        private static int[] britainMultiIDs = new int[] { 0x44, 0x40, 0x40 };
        private static int[,] britainMultiIDsItemIDMods = new int[,] 
        {
            { 1536, 1482, 1590, 1644 },
            { 0, -54, 54, 108 },
            { 0, -54, 54, 108 },
        };

        protected override int[] multiIDs { get { return britainMultiIDs; } }
        protected override int[,] itemIDMods { get { return britainMultiIDsItemIDMods; } }
        #endregion

        private SingleCannonPlace _cannonCenter;
        private SingleCannonPlace _cannonSupSx;
        private SingleCannonPlace _cannonSupDx;
        private SingleCannonPlace _cannonMidSupSx;
        private SingleCannonPlace _cannonMidSupDx;
        private SingleCannonPlace _cannonMidInfSx;
        private SingleCannonPlace _cannonMidInfDx;
        private SingleCannonPlace _cannonInfSx;
        private SingleCannonPlace _cannonInfDx;
        private IHelm _helm;
        private GalleonHold _hold;
        private MainMast _mainMast;
        private BritainHull _hull;
		private BoatRope _pRope, _sRope;
		private TillerManHS _tillerMan;

		[CommandProperty( AccessLevel.GameMaster )]
		public override BaseDockedGalleon DockedGalleon { get { return new DockedBritainGalleon(this); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonCenter { get { return _cannonCenter; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupSx { get { return _cannonSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonSupDx { get { return _cannonSupDx; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidSupSx { get { return _cannonMidSupSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidSupDx { get { return _cannonMidSupDx; } }	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidInfSx { get { return _cannonMidInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonMidInfDx { get { return _cannonMidInfDx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfSx { get { return _cannonInfSx; } }	

		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace CannonInfDx { get { return _cannonInfDx; } }	
		
        [Constructable]
        public BritainGalleon()
            : base(0x40)
        {
            _cannonCenter = new SingleCannonPlace(this, 0x5C06, new Point3D(0, -9, 0));
            _cannonSupSx = new SingleCannonPlace(this, 0x5C19, new Point3D(-3, -5, 0));
            _cannonSupDx = new SingleCannonPlace(this, 0x5C18, new Point3D(3, -5, 0));
            _cannonMidSupSx = new SingleCannonPlace(this, 0x5C1A, new Point3D(-3, -1, 0));
            _cannonMidSupDx = new SingleCannonPlace(this, 0x5C1C, new Point3D(3, -1, 0));
            _cannonMidInfSx = new SingleCannonPlace(this, 0x5C21, new Point3D(-3, 3, 0));
            _cannonMidInfDx = new SingleCannonPlace(this, 0x5C1F, new Point3D(3, 3, 0));
            _cannonInfSx = new SingleCannonPlace(this, 0x5C25, new Point3D(-3, 7, 0));
            _cannonInfDx = new SingleCannonPlace(this, 0x5C23, new Point3D(3, 7, 0));
            _helm = new SingleHelm(this, 0x5C0C, new Point3D(0, 3, 1));
            _hold = new GalleonHold(this, 0x5C2A, new Point3D(0, 9, 0),
                new List<Tuple<int, Point3D>>() 
                {
                    new Tuple<int, Point3D>(0x5C2C, new Point3D(-1, -1, 0)),
                    new Tuple<int, Point3D>(0x5C2F, new Point3D(-1, 0, 0)),
                    new Tuple<int, Point3D>(0x5C32, new Point3D(-1, 1, 0)),

                    new Tuple<int, Point3D>(0x5C2D, new Point3D(0, 0, 0)),
                    new Tuple<int, Point3D>(0x5C30, new Point3D(0, 1, 0)),   
                 
                    new Tuple<int, Point3D>(0x5C2B, new Point3D(1, -1, 0)),
                    new Tuple<int, Point3D>(0x5C2E, new Point3D(1, 0, 0)),
                    new Tuple<int, Point3D>(0x5C31, new Point3D(1, 1, 0)),
                }, 28000);

            _mainMast = new MainMast(this, 0x5CE3, new Point3D(0, -3, 0));
            _hull = new BritainHull(this, 0x58A5);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-3, -4, 16), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(3, -4, 16), BoatRopeSide.Port, 0 ); 
			Ropes.Add(_pRope);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-3, 0, 16), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(3, 0, 16), BoatRopeSide.Port, 0 );
			Ropes.Add(_pRope);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-3, 4, 16), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(3, 4, 16), BoatRopeSide.Port, 0 );
			Ropes.Add(_pRope);
			_sRope = new BoatRope(this, 0x14F8, new Point3D(-3, 8, 16), BoatRopeSide.Starboard, 0 ); 
			Ropes.Add(_sRope);
			_pRope = new BoatRope(this, 0x14F8, new Point3D(3, 8, 16), BoatRopeSide.Port, 0 );
			Ropes.Add(_pRope);
			_tillerMan = new TillerManHS(this, 0, new Point3D(0, 7, 12) );
			Name = "a Britain Galleon";
			
            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)
            XmlAttach.AttachTo(this, new XmlBoatFight(200000, 10, 10, 20, 30, 0));

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            Hue = 0;				
			
        }

        public BritainGalleon(Serial serial)
            : base(serial)
        {
        }

        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 4, 18);
            }
        }
		
        protected override void OnStatusChange()
        {
            _cannonSupSx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonSupSx.LinkedLightCannon != null)
				_cannonSupSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
				
			if (_cannonSupSx.LinkedHeavyCannon != null)
				_cannonSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
			
            _cannonSupDx.RefreshItemID(CurrentItemIdModifier);
			if (_cannonSupDx.LinkedLightCannon != null)
				_cannonSupDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
				
			if (_cannonSupDx.LinkedHeavyCannon != null)
				_cannonSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);

            if (Status == GalleonStatus.Low || (Status == GalleonStatus.Half && (Facing == Server.Direction.South)))
            {
                _cannonCenter.RefreshItemID(CurrentItemIdModifier - 6);
				if (_cannonCenter.LinkedLightCannon != null)
					_cannonCenter.LinkedLightCannon.RefreshItemID(CannonItemIdModifier);
					
				if (_cannonCenter.LinkedHeavyCannon != null)
					_cannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
				_cannonMidSupSx.RefreshItemID(CurrentItemIdModifier + 32);
				if (_cannonMidSupSx.LinkedLightCannon != null)
					_cannonMidSupSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonMidSupSx.LinkedHeavyCannon != null)
					_cannonMidSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
				_cannonMidSupDx.RefreshItemID(CurrentItemIdModifier + 32);
				if (_cannonMidSupDx.LinkedLightCannon != null)
					_cannonMidSupDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonMidSupDx.LinkedHeavyCannon != null)
					_cannonMidSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
				_cannonMidInfSx.RefreshItemID(CurrentItemIdModifier + 29);
				if (_cannonMidInfSx.LinkedLightCannon != null)
					_cannonMidInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonMidInfSx.LinkedHeavyCannon != null)
					_cannonMidInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
				_cannonMidInfDx.RefreshItemID(CurrentItemIdModifier + 29);
				if (_cannonMidInfDx.LinkedLightCannon != null)
					_cannonMidInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonMidInfDx.LinkedHeavyCannon != null)
					_cannonMidInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _cannonInfSx.RefreshItemID(CurrentItemIdModifier + 20);
				if (_cannonInfSx.LinkedLightCannon != null)
					_cannonInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonInfSx.LinkedHeavyCannon != null)
					_cannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonInfDx.RefreshItemID(CurrentItemIdModifier + 20);
				if (_cannonInfDx.LinkedLightCannon != null)
					_cannonInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonInfDx.LinkedHeavyCannon != null)
					_cannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _hold.RefreshItemID(CurrentItemIdModifier + 14);
            }
            else
            {
                _cannonCenter.RefreshItemID(CurrentItemIdModifier);
				if (_cannonCenter.LinkedLightCannon != null)
					_cannonCenter.LinkedLightCannon.RefreshItemID(CannonItemIdModifier);
					
				if (_cannonCenter.LinkedHeavyCannon != null)
					_cannonCenter.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifier);
				
				_cannonMidSupSx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonMidSupSx.LinkedLightCannon != null)
					_cannonMidSupSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonMidSupSx.LinkedHeavyCannon != null)
					_cannonMidSupSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
				_cannonMidSupDx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonMidSupDx.LinkedLightCannon != null)
					_cannonMidSupDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonMidSupDx.LinkedHeavyCannon != null)
					_cannonMidSupDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
				_cannonMidInfSx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonMidInfSx.LinkedLightCannon != null)
					_cannonMidInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonMidInfSx.LinkedHeavyCannon != null)
					_cannonMidInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
				_cannonMidInfDx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonMidInfDx.LinkedLightCannon != null)
					_cannonMidInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonMidInfDx.LinkedHeavyCannon != null)
					_cannonMidInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _cannonInfSx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonInfSx.LinkedLightCannon != null)
					_cannonInfSx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierSx);
					
				if (_cannonInfSx.LinkedHeavyCannon != null)
					_cannonInfSx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierSx);
				
                _cannonInfDx.RefreshItemID(CurrentItemIdModifier);
				if (_cannonInfDx.LinkedLightCannon != null)
					_cannonInfDx.LinkedLightCannon.RefreshItemID(CannonItemIdModifierDx);
					
				if (_cannonInfDx.LinkedHeavyCannon != null)
					_cannonInfDx.LinkedHeavyCannon.RefreshItemID(CannonItemIdModifierDx);
				
                _hold.RefreshItemID(CurrentItemIdModifier);
            }

            int newModifier = 0;
            switch (Status)
            {
                case GalleonStatus.Full:
                    switch (Facing)
                    {
                        //case Server.Direction.North: newModifier = 0; break;
                        case Server.Direction.East: newModifier = 54; break;
                        case Server.Direction.South: newModifier = 108; break;
                        case Server.Direction.West: newModifier = -54; break;
                    }
                    break;
                case GalleonStatus.Half:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 5004; break;
                        case Server.Direction.East: newModifier = 604; break;
                        case Server.Direction.South: newModifier = -15020; break;
                        case Server.Direction.West: newModifier = 1510; break;
                    }
                    break;
                case GalleonStatus.Low:
                    switch (Facing)
                    {
                        case Server.Direction.North: newModifier = 2114; break;
                        case Server.Direction.East: newModifier = 1812; break;
                        case Server.Direction.South: newModifier = 2415; break;
                        case Server.Direction.West: newModifier = 2715; break;
                    }
                    break;
            }

            _helm.RefreshItemID(newModifier);
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _cannonCenter = reader.ReadItem() as SingleCannonPlace;
            _cannonSupSx = reader.ReadItem() as SingleCannonPlace;
            _cannonSupDx = reader.ReadItem() as SingleCannonPlace;
            _cannonMidSupSx = reader.ReadItem() as SingleCannonPlace;
            _cannonMidSupDx = reader.ReadItem() as SingleCannonPlace;
            _cannonMidInfSx = reader.ReadItem() as SingleCannonPlace;
            _cannonMidInfDx = reader.ReadItem() as SingleCannonPlace;
            _cannonInfSx = reader.ReadItem() as SingleCannonPlace;
            _cannonInfDx = reader.ReadItem() as SingleCannonPlace;
            _helm = reader.ReadItem() as SingleHelm;
            _hold = reader.ReadItem() as GalleonHold;
            _mainMast = reader.ReadItem() as MainMast;
            _hull = reader.ReadItem() as BritainHull;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((Item)_cannonCenter);
            writer.Write((Item)_cannonSupSx);
            writer.Write((Item)_cannonSupDx);
            writer.Write((Item)_cannonMidSupSx);
            writer.Write((Item)_cannonMidSupDx);
            writer.Write((Item)_cannonMidInfSx);
            writer.Write((Item)_cannonMidInfDx);
            writer.Write((Item)_cannonInfSx);
            writer.Write((Item)_cannonInfDx);
            writer.Write((Item)_helm);
            writer.Write((Item)_hold);
            writer.Write((Item)_mainMast);
            writer.Write((Item)_hull);
        }
        #endregion
    }
	
	public class BritainGalleonDeed : BaseGalleonDeed
	{
		public override BaseGalleon Boat{ get{ return new BritainGalleon(); } }

		[Constructable]
		public BritainGalleonDeed() : base( 0x40, new Point3D( 0, -1, 0 ) )
		{
			Name = "Britain Galleon Deed";
		}

		public BritainGalleonDeed( Serial serial ) : base( serial )
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
	
	public class DockedBritainGalleon : BaseDockedGalleon
	{
		public override BaseGalleon Galleon{ get{ return new BritainGalleon(); } }

		public DockedBritainGalleon( BaseGalleon boat ) : base( 0x40, Point3D.Zero, boat )
		{
		}

		public DockedBritainGalleon( Serial serial ) : base( serial )
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
