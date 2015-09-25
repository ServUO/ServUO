using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class LightShipCannon : BaseShipWeapon, ICannon, IFacingChange
    {
	
		private SingleCannonPlace _linkedSingleCannonPlace;
		private MultiCannonPlace _linkedMultiCannonPlace;
		private bool _singleCannon;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace LinkedSingleCannonPlace { get { return _linkedSingleCannonPlace; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace LinkedMultiCannonPlace { get { return _linkedMultiCannonPlace; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SingleCannon { get { return _singleCannon; } }
	
        // facing 0
        public static int[] CannonWest = new int[] { 0x4217, 0x4217, 0x4217 };
        public static int[] CannonWestXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonWestYOffset = new int[] { 0, 0, 0 };
        // facing 1
        public static int[] CannonNorth = new int[] { 0x4218, 0x4218, 0x4218 };
        public static int[] CannonNorthXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonNorthYOffset = new int[] { 0, 0, 0 };
        // facing 2
        public static int[] CannonEast = new int[] { 0x4219, 0x4219, 0x4219 };
        public static int[] CannonEastXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonEastYOffset = new int[] { 0, 0, 0 };
        // facing 3
        public static int[] CannonSouth = new int[] { 0x4216, 0x4216, 0x4216 };
        public static int[] CannonSouthXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonSouthYOffset = new int[] { 0, 0, 0 };
        private readonly Type[] _allowedprojectiles = new Type[] { typeof(ShipCannonball) };
        
        public LightShipCannon(BaseGalleon galleon, int northItemID, Point3D initOffset, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
            : this(0, galleon, northItemID, initOffset, targetedSingleCannonPlace, targetedMultiCannonPlace)
        {
        }

        public LightShipCannon(int facing, BaseGalleon galleon, int northItemID, Point3D initOffset, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
			: base(galleon, northItemID, initOffset)
        {
		
			if (targetedSingleCannonPlace != null)
			{
				_linkedSingleCannonPlace = targetedSingleCannonPlace;
				targetedSingleCannonPlace.LinkedLightCannon = this;
				_singleCannon = true;
				_linkedMultiCannonPlace = null;
			}
			else if (targetedMultiCannonPlace != null)
			{
				_linkedSingleCannonPlace = null;
				targetedMultiCannonPlace.LinkedLightCannon = this;
				_singleCannon = false;
				_linkedMultiCannonPlace = targetedMultiCannonPlace;
			}
		
            // add the components
            /*for (int i = 0; i < CannonNorth.Length; i++)
            *{
            *    AddComponent(new ShipComponent(0, Name), 0, 0, 0);
            *}
			*/

            // assign the facing
            if (facing < 0)
                facing = 3;
            if (facing > 3)
                facing = 0;
            Facing = facing;

            // set the default props
            Name = "Light Ship Cannon";
            Weight = 50;

            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)
            XmlAttach.AttachTo(this, new XmlBoatFight(100, 10, 10, 20, 30, 0));

            // and draggable
            //XmlAttach.AttachTo(this, new XmlDrag());

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            Hue = 0;
        }

        public LightShipCannon(Serial serial)
            : base(serial)
        {
        }
		
		public void SetFacing(Direction oldFacing, Direction newFacing)
		{
		
			bool singleCannon = false;
			bool multiCannon = false;
			if (_linkedSingleCannonPlace != null)
				singleCannon = true;
			else if (_linkedMultiCannonPlace != null)
				multiCannon = true;
							
			if ((singleCannon == false) && (multiCannon == false))
				return;	
				
			if (singleCannon)
			{

				object objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
				SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
				SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
				SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	
				
				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSx");
				SingleCannonPlace CannonMidSx = objValue as SingleCannonPlace;	

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidDx");
				SingleCannonPlace CannonMidDx = objValue as SingleCannonPlace;

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
				SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
				SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;					
			
				if (newFacing == Direction.North)
				{
					if (_linkedSingleCannonPlace == CannonSupSx)
						Facing = 0;
					if (_linkedSingleCannonPlace == CannonSupDx)
						Facing = 2;
					if (_linkedSingleCannonPlace == CannonInfSx)
						Facing = 0;
					if (_linkedSingleCannonPlace == CannonInfDx)
						Facing = 2;
					if (_linkedSingleCannonPlace == CannonCenter)
						Facing = 1;						
				}
				else if (newFacing == Direction.East)
				{
					if (_linkedSingleCannonPlace == CannonSupSx)
						Facing = 1;
					if (_linkedSingleCannonPlace == CannonSupDx)
						Facing = 3;
					if (_linkedSingleCannonPlace == CannonInfSx)
						Facing = 1;
					if (_linkedSingleCannonPlace == CannonInfDx)
						Facing = 3;
					if (_linkedSingleCannonPlace == CannonCenter)
						Facing = 2;							
				}
				else if (newFacing == Direction.South)
				{
					if (_linkedSingleCannonPlace == CannonSupSx)
						Facing = 2;
					if (_linkedSingleCannonPlace == CannonSupDx)
						Facing = 0;
					if (_linkedSingleCannonPlace == CannonInfSx)
						Facing = 2;
					if (_linkedSingleCannonPlace == CannonInfDx)
						Facing = 0;
					if (_linkedSingleCannonPlace == CannonCenter)
						Facing = 3;							
				}
				else if (newFacing == Direction.West)
				{
					if (_linkedSingleCannonPlace == CannonSupSx)
						Facing = 3;
					if (_linkedSingleCannonPlace == CannonSupDx)
						Facing = 1;
					if (_linkedSingleCannonPlace == CannonInfSx)
						Facing = 3;
					if (_linkedSingleCannonPlace == CannonInfDx)
						Facing = 1;
					if (_linkedSingleCannonPlace == CannonCenter)
						Facing = 0;					
				}
			}
			else if (multiCannon)
			{	
				object objValue = _linkedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
				MultiCannonPlace CannonSupSx = objValue as MultiCannonPlace;	

				objValue = _linkedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
				MultiCannonPlace CannonSupDx = objValue as MultiCannonPlace;	

				objValue = _linkedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
				MultiCannonPlace CannonInfSx = objValue as MultiCannonPlace;	

				objValue = _linkedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
				MultiCannonPlace CannonInfDx = objValue as MultiCannonPlace;	
				
			
				if (newFacing == Direction.North)
				{
					if (_linkedMultiCannonPlace == CannonSupSx)
						Facing = 0;
					if (_linkedMultiCannonPlace == CannonSupDx)
						Facing = 2;
					if (_linkedMultiCannonPlace == CannonInfSx)
						Facing = 0;
					if (_linkedMultiCannonPlace == CannonInfDx)
						Facing = 2;						
				}
				else if (newFacing == Direction.East)
				{
					if (_linkedMultiCannonPlace == CannonSupSx)
						Facing = 1;
					if (_linkedMultiCannonPlace == CannonSupDx)
						Facing = 3;
					if (_linkedMultiCannonPlace == CannonInfSx)
						Facing = 1;
					if (_linkedMultiCannonPlace == CannonInfDx)
						Facing = 3;							
				}
				else if (newFacing == Direction.South)
				{
					if (_linkedMultiCannonPlace == CannonSupSx)
						Facing = 2;
					if (_linkedMultiCannonPlace == CannonSupDx)
						Facing = 0;
					if (_linkedMultiCannonPlace == CannonInfSx)
						Facing = 2;
					if (_linkedMultiCannonPlace == CannonInfDx)
						Facing = 0;							
				}
				else if (newFacing == Direction.West)
				{
					if (_linkedMultiCannonPlace == CannonSupSx)
						Facing = 3;
					if (_linkedMultiCannonPlace == CannonSupDx)
						Facing = 1;
					if (_linkedMultiCannonPlace == CannonInfSx)
						Facing = 3;
					if (_linkedMultiCannonPlace == CannonInfDx)
						Facing = 1;							
				}
			}
		}

        public override double WeaponLoadingDelay
        {
            get
            {
                return 15;
            }
        }// base delay for loading this weapon
        public override double WeaponStorageDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for packing away this weapon
        public override double WeaponDamageFactor
        {
            get
            {
                return base.WeaponDamageFactor * 1.2;
            }
        }// damage multiplier for the weapon
        public override double WeaponRangeFactor
        {
            get
            {
                return base.WeaponRangeFactor * 1.2;
            }
        }//  range multiplier for the weapon
        public override int MinTargetRange
        {
            get
            {
                return 1;
            }
        }// target must be further away than this
        public override int MinStorageRange
        {
            get
            {
                return 2;
            }
        }// player must be at least this close to store the weapon
        public override int MinFiringRange
        {
            get
            {
                return 3;
            }
        }// player must be at least this close to fire the weapon
        public override bool CheckLOS
        {
            get
            {
                return true;
            }
        }// whether the weapon needs to consider line of sight when selecting a target
        public override Type[] AllowedProjectiles
        {
            get
            {
                return _allowedprojectiles;
            }
        }
        public override Point3D ProjectileLaunchPoint
        {
            get
            {
                /*if (Components != null && Components.Count > 0)
                *{
                *    switch (Facing)
                *    {
                *        case 0:
                *            return new Point3D(CannonWestXOffset[0] + Location.X - 1, CannonWestYOffset[0] + Location.Y, Location.Z + 1);
                *        case 1:
                *            return new Point3D(CannonNorthXOffset[0] + Location.X - 1, CannonNorthYOffset[0] + Location.Y - 1, Location.Z + 1);
                *        case 2:
                *            return new Point3D(CannonEastXOffset[0] + Location.X, CannonEastYOffset[0] + Location.Y - 1, Location.Z + 1);
                *        case 3:
                *            return new Point3D(CannonSouthXOffset[0] + Location.X - 1, CannonSouthYOffset[0] + Location.Y, Location.Z + 1);
                *    }
                *}
				*/

                return (Location);
            }
        }
        public override void LaunchProjectile(Mobile from, Item projectile, IEntity target, Point3D targetloc, TimeSpan delay)
        {
            base.LaunchProjectile(from, projectile, target, targetloc, delay);

            // show the cannon firing animation with explosion sound
            Effects.SendLocationEffect(this, Map, 0x36B0, 16, 1);
            Effects.PlaySound(this, Map, 0x11D);
        }

        public override void UpdateDisplay()
        {
            /*if (Components != null && Components.Count > 2)
            {*/
                //int z = ((AddonComponent)Components[1]).Location.Z;

                int[] itemid = null;
                int[] xoffset = null;
                int[] yoffset = null;
				int itemidmod = 0;

                switch (Facing)
                {
                    case 0: // West
                        itemid = CannonWest;
						itemidmod = -1;
                        xoffset = CannonWestXOffset;
                        yoffset = CannonWestYOffset;
                        break;
                    case 1: // North
                        itemid = CannonNorth;
                        xoffset = CannonNorthXOffset;
                        yoffset = CannonNorthYOffset;
                        break;
                    case 2: // East
                        itemid = CannonEast;
						itemidmod = 1;
                        xoffset = CannonEastXOffset;
                        yoffset = CannonEastYOffset;
                        break;
                    case 3: // South
                        itemid = CannonSouth;
						itemidmod = -2;
                        xoffset = CannonSouthXOffset;
                        yoffset = CannonSouthYOffset;
                        break;
                }

                if (itemid != null && xoffset != null && yoffset != null /*&& Components.Count == itemid.Length*/)
                {
                    /*for (int i = 0; i < Components.Count; i++)
                    {*/
						ItemID = itemid[0];
						RefreshItemID(itemidmod);
                        //((AddonComponent)Components[i]).ItemID = itemid[i];
                        //Point3D newoffset = new Point3D(xoffset[i], yoffset[i], 0);
                        //((AddonComponent)Components[i]).Offset = newoffset;
                        //((AddonComponent)Components[i]).Location = new Point3D(newoffset.X + X, newoffset.Y + Y, z);
                    //}
                }
            //}
        }
		
        public override void OnAfterDelete()
        {

        }
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
			base.GetContextMenuEntries(from, list);
            list.Add(new BackpackEntry(from, this));
        }
		
		public virtual void OnChop(Mobile from)
        {
			BaseGalleon galleon = null;
			if (LinkedSingleCannonPlace != null)
				galleon = LinkedSingleCannonPlace.Galleon;
			
			if (LinkedMultiCannonPlace != null)
				galleon = LinkedMultiCannonPlace.Galleon;

            if (galleon != null && (galleon.Owner == from) && galleon.Contains(this))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
				
				Delete();

                LightCannonDeed deed = new LightCannonDeed();

                from.AddToBackpack(deed);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
			
			//version 1
			writer.Write(_singleCannon);
			
			if (_singleCannon)
				writer.Write(_linkedSingleCannonPlace);
			else
				writer.Write(_linkedMultiCannonPlace);
				
			
			
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch (version)
			{
				case 1 :
				{
					_singleCannon = reader.ReadBool();
					
					if (_singleCannon)
						_linkedSingleCannonPlace = reader.ReadItem() as SingleCannonPlace;
					else
						_linkedMultiCannonPlace = reader.ReadItem() as MultiCannonPlace;
						
					break;
				}
			}
        }
		
		private class BackpackEntry : ContextMenuEntry
		{
			private readonly LightShipCannon m_cannon;
			private readonly Mobile m_from;
			public BackpackEntry(Mobile from, LightShipCannon cannon)
				: base(2139)
			{
				m_cannon = cannon;
				m_from = from;
			}

			public override void OnClick()
			{
				if (m_cannon != null)
				{
					m_cannon.OnChop(m_from);
				}
			}
		}	

       public override void OnDoubleClick(Mobile from)
        {			
            if (Parent != null)
                return;

            // can't use destroyed weapons
            if (Hits == 0)
                return;

            // check the range between the player and weapon
            if (!from.InRange(Location, MinFiringRange) || from.Map != Map)
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (Storing)
            {
                from.SendMessage("{0} being stored", Name);
                return;
            }

            if (Projectile == null || Projectile.Deleted)
            {
                from.SendMessage("{0} empty", Name);
                return;
            }

            // check if the cannon is cool enough to fire
            if (NextFiring.Seconds > 0)
            {
                from.SendMessage("Not ready yet.");
                return;
            }
			
			//new way to fire
			Map map = Map;

			if ( map == null )
				return;

			int rx = 0, ry = 0;

			if ( Facing == 0 )
				rx = -1;
			else if ( Facing == 1 )
				ry = -1;
			else if ( Facing == 2 )
				rx = 1;
			else if ( Facing == 3 )
				ry = 1;

			for ( int i = 1; i <= 15; ++i )
			{
				int x = X + (i*rx);
				int y = Y + (i*ry);
				int z;

				for ( int j = -16; j <= 16; ++j )
				{
					z = from.Z + j;

					Point3D currentLocation = new Point3D( x, y, z );
					Item goldToken = new Gold(100);
					goldToken.Visible = false;
					goldToken.Map = Map;
					goldToken.Location = currentLocation;
					
					foreach (Item item in goldToken.GetItemsInRange(10))
						if (item is BaseShip)
						{
							BaseShip target = (BaseShip) item;
																	

							//Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
							// may have to reconsider the use tileloc vs target loc
							//m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);
							if (target != Galleon)
							{
								AttackTarget(from, target, target.Map.GetPoint(target, true), true);
								goldToken.Delete();
								return;
							}
							
						}

					goldToken.Delete();
						
				}

				z = map.GetAverageZ( x, y );


				Point3D currentLocation2 = new Point3D( x, y, z );
				Item goldToken2 = new Gold(100);
				goldToken2.Visible = false;
				goldToken2.Map = Map;
				goldToken2.Location = currentLocation2;
				foreach (Item item in goldToken2.GetItemsInRange(10))
					if (item is BaseShip)
					{
						BaseShip target = (BaseShip) item;
																

						//Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
						// may have to reconsider the use tileloc vs target loc
						//m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);												
						if (target != Galleon)
						{
							AttackTarget(from, target, target.Map.GetPoint(target, true), true);
							goldToken2.Delete();
							return;
						}						
					}	
					
				goldToken2.Delete();
			}			
        }		
    }
	
    public class HeavyShipCannon : BaseShipWeapon, ICannon, IFacingChange
    {
	
		private SingleCannonPlace _linkedSingleCannonPlace;
		private MultiCannonPlace _linkedMultiCannonPlace;
		private bool _singleCannon;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SingleCannonPlace LinkedSingleCannonPlace { get { return _linkedSingleCannonPlace; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MultiCannonPlace LinkedMultiCannonPlace { get { return _linkedMultiCannonPlace; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool SingleCannon { get { return _singleCannon; } }
	
        // facing 0
        public static int[] CannonWest = new int[] { 0x421B, 0x421B, 0x421B };
        public static int[] CannonWestXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonWestYOffset = new int[] { 0, 0, 0 };
        // facing 1
        public static int[] CannonNorth = new int[] { 0x421C, 0x421C, 0x421C };
        public static int[] CannonNorthXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonNorthYOffset = new int[] { 0, 0, 0 };
        // facing 2
        public static int[] CannonEast = new int[] { 0x421D, 0x421D, 0x421D };
        public static int[] CannonEastXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonEastYOffset = new int[] { 0, 0, 0 };
        // facing 3
        public static int[] CannonSouth = new int[] { 0x421A, 0x421A, 0x421A };
        public static int[] CannonSouthXOffset = new int[] { 0, 0, 0 };
        public static int[] CannonSouthYOffset = new int[] { 0, 0, 0 };
        private readonly Type[] _allowedprojectiles = new Type[] { typeof(ShipCannonball) };
        
        public HeavyShipCannon(BaseGalleon galleon, int northItemID, Point3D initOffset, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
            : this(0, galleon, northItemID, initOffset, targetedSingleCannonPlace, targetedMultiCannonPlace)
        {
        }

        public HeavyShipCannon(int facing, BaseGalleon galleon, int northItemID, Point3D initOffset, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
			: base(galleon, northItemID, initOffset)
        {
		
			if (targetedSingleCannonPlace != null)
			{
				_linkedSingleCannonPlace = targetedSingleCannonPlace;
				targetedSingleCannonPlace.LinkedHeavyCannon = this;
				_singleCannon = true;
				_linkedMultiCannonPlace = null;
			}
			else if (targetedMultiCannonPlace != null)
			{
				_linkedSingleCannonPlace = null;
				targetedMultiCannonPlace.LinkedHeavyCannon = this;
				_singleCannon = false;
				_linkedMultiCannonPlace = targetedMultiCannonPlace;
			}
		
            // add the components
            /*for (int i = 0; i < CannonNorth.Length; i++)
            *{
            *    AddComponent(new ShipComponent(0, Name), 0, 0, 0);
            *}
			*/

            // assign the facing
            if (facing < 0)
                facing = 3;
            if (facing > 3)
                facing = 0;
            Facing = facing;

            // set the default props
            Name = "Heavy Ship Cannon";
            Weight = 50;

            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)
            XmlAttach.AttachTo(this, new XmlBoatFight(100, 10, 10, 20, 30, 0));

            // and draggable
            //XmlAttach.AttachTo(this, new XmlDrag());

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            Hue = 0;
        }

        public HeavyShipCannon(Serial serial)
            : base(serial)
        {
        }
		
		public void SetFacing(Direction oldFacing, Direction newFacing)
		{
		
			bool singleCannon = false;
			bool multiCannon = false;
			if (_linkedSingleCannonPlace != null)
				singleCannon = true;
			else if (_linkedMultiCannonPlace != null)
				multiCannon = true;
							
			if ((singleCannon == false) && (multiCannon == false))
				return;	
				
			if (singleCannon)
			{

				object objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
				SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
				SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
				SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	
				
				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSx");
				SingleCannonPlace CannonMidSx = objValue as SingleCannonPlace;	

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidDx");
				SingleCannonPlace CannonMidDx = objValue as SingleCannonPlace;

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
				SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

				objValue = _linkedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
				SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;					
			
				if (newFacing == Direction.North)
				{
					if (_linkedSingleCannonPlace == CannonSupSx)
						Facing = 0;
					if (_linkedSingleCannonPlace == CannonSupDx)
						Facing = 2;
					if (_linkedSingleCannonPlace == CannonInfSx)
						Facing = 0;
					if (_linkedSingleCannonPlace == CannonInfDx)
						Facing = 2;
					if (_linkedSingleCannonPlace == CannonCenter)
						Facing = 1;						
				}
				else if (newFacing == Direction.East)
				{
					if (_linkedSingleCannonPlace == CannonSupSx)
						Facing = 1;
					if (_linkedSingleCannonPlace == CannonSupDx)
						Facing = 3;
					if (_linkedSingleCannonPlace == CannonInfSx)
						Facing = 1;
					if (_linkedSingleCannonPlace == CannonInfDx)
						Facing = 3;
					if (_linkedSingleCannonPlace == CannonCenter)
						Facing = 2;							
				}
				else if (newFacing == Direction.South)
				{
					if (_linkedSingleCannonPlace == CannonSupSx)
						Facing = 2;
					if (_linkedSingleCannonPlace == CannonSupDx)
						Facing = 0;
					if (_linkedSingleCannonPlace == CannonInfSx)
						Facing = 2;
					if (_linkedSingleCannonPlace == CannonInfDx)
						Facing = 0;
					if (_linkedSingleCannonPlace == CannonCenter)
						Facing = 3;							
				}
				else if (newFacing == Direction.West)
				{
					if (_linkedSingleCannonPlace == CannonSupSx)
						Facing = 3;
					if (_linkedSingleCannonPlace == CannonSupDx)
						Facing = 1;
					if (_linkedSingleCannonPlace == CannonInfSx)
						Facing = 3;
					if (_linkedSingleCannonPlace == CannonInfDx)
						Facing = 1;
					if (_linkedSingleCannonPlace == CannonCenter)
						Facing = 0;					
				}
			}
			else if (multiCannon)
			{	
				object objValue = _linkedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
				MultiCannonPlace CannonSupSx = objValue as MultiCannonPlace;	

				objValue = _linkedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
				MultiCannonPlace CannonSupDx = objValue as MultiCannonPlace;	

				objValue = _linkedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
				MultiCannonPlace CannonInfSx = objValue as MultiCannonPlace;	

				objValue = _linkedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
				MultiCannonPlace CannonInfDx = objValue as MultiCannonPlace;	
				
			
				if (newFacing == Direction.North)
				{
					if (_linkedMultiCannonPlace == CannonSupSx)
						Facing = 0;
					if (_linkedMultiCannonPlace == CannonSupDx)
						Facing = 2;
					if (_linkedMultiCannonPlace == CannonInfSx)
						Facing = 0;
					if (_linkedMultiCannonPlace == CannonInfDx)
						Facing = 2;						
				}
				else if (newFacing == Direction.East)
				{
					if (_linkedMultiCannonPlace == CannonSupSx)
						Facing = 1;
					if (_linkedMultiCannonPlace == CannonSupDx)
						Facing = 3;
					if (_linkedMultiCannonPlace == CannonInfSx)
						Facing = 1;
					if (_linkedMultiCannonPlace == CannonInfDx)
						Facing = 3;							
				}
				else if (newFacing == Direction.South)
				{
					if (_linkedMultiCannonPlace == CannonSupSx)
						Facing = 2;
					if (_linkedMultiCannonPlace == CannonSupDx)
						Facing = 0;
					if (_linkedMultiCannonPlace == CannonInfSx)
						Facing = 2;
					if (_linkedMultiCannonPlace == CannonInfDx)
						Facing = 0;							
				}
				else if (newFacing == Direction.West)
				{
					if (_linkedMultiCannonPlace == CannonSupSx)
						Facing = 3;
					if (_linkedMultiCannonPlace == CannonSupDx)
						Facing = 1;
					if (_linkedMultiCannonPlace == CannonInfSx)
						Facing = 3;
					if (_linkedMultiCannonPlace == CannonInfDx)
						Facing = 1;							
				}
			}
		}		

        public override double WeaponLoadingDelay
        {
            get
            {
                return 15;
            }
        }// base delay for loading this weapon
        public override double WeaponStorageDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for packing away this weapon
        public override double WeaponDamageFactor
        {
            get
            {
                return base.WeaponDamageFactor * 1.2;
            }
        }// damage multiplier for the weapon
        public override double WeaponRangeFactor
        {
            get
            {
                return base.WeaponRangeFactor * 1.2;
            }
        }//  range multiplier for the weapon
        public override int MinTargetRange
        {
            get
            {
                return 1;
            }
        }// target must be further away than this
        public override int MinStorageRange
        {
            get
            {
                return 2;
            }
        }// player must be at least this close to store the weapon
        public override int MinFiringRange
        {
            get
            {
                return 3;
            }
        }// player must be at least this close to fire the weapon
        public override bool CheckLOS
        {
            get
            {
                return true;
            }
        }// whether the weapon needs to consider line of sight when selecting a target
        public override Type[] AllowedProjectiles
        {
            get
            {
                return _allowedprojectiles;
            }
        }
        public override Point3D ProjectileLaunchPoint
        {
            get
            {
                /*if (Components != null && Components.Count > 0)
                *{
                *    switch (Facing)
                *    {
                *        case 0:
                *            return new Point3D(CannonWestXOffset[0] + Location.X - 1, CannonWestYOffset[0] + Location.Y, Location.Z + 1);
                *        case 1:
                *            return new Point3D(CannonNorthXOffset[0] + Location.X - 1, CannonNorthYOffset[0] + Location.Y - 1, Location.Z + 1);
                *        case 2:
                *            return new Point3D(CannonEastXOffset[0] + Location.X, CannonEastYOffset[0] + Location.Y - 1, Location.Z + 1);
                *        case 3:
                *            return new Point3D(CannonSouthXOffset[0] + Location.X - 1, CannonSouthYOffset[0] + Location.Y, Location.Z + 1);
                *    }
                *}
				*/

                return (Location);
            }
        }
        public override void LaunchProjectile(Mobile from, Item projectile, IEntity target, Point3D targetloc, TimeSpan delay)
        {
            base.LaunchProjectile(from, projectile, target, targetloc, delay);

            // show the cannon firing animation with explosion sound
            Effects.SendLocationEffect(this, Map, 0x36B0, 16, 1);
            Effects.PlaySound(this, Map, 0x11D);
        }

        public override void UpdateDisplay()
        {
            /*if (Components != null && Components.Count > 2)
            {*/
                //int z = ((AddonComponent)Components[1]).Location.Z;

                int[] itemid = null;
                int[] xoffset = null;
                int[] yoffset = null;
				int itemidmod = 0;

                switch (Facing)
                {
                    case 0: // West
                        itemid = CannonWest;
						itemidmod = -1;
                        xoffset = CannonWestXOffset;
                        yoffset = CannonWestYOffset;
                        break;
                    case 1: // North
                        itemid = CannonNorth;
                        xoffset = CannonNorthXOffset;
                        yoffset = CannonNorthYOffset;
                        break;
                    case 2: // East
                        itemid = CannonEast;
						itemidmod = 1;
                        xoffset = CannonEastXOffset;
                        yoffset = CannonEastYOffset;
                        break;
                    case 3: // South
                        itemid = CannonSouth;
						itemidmod = -2;
                        xoffset = CannonSouthXOffset;
                        yoffset = CannonSouthYOffset;
                        break;
                }

                if (itemid != null && xoffset != null && yoffset != null /*&& Components.Count == itemid.Length*/)
                {
                    /*for (int i = 0; i < Components.Count; i++)
                    {*/
						ItemID = itemid[0];
						RefreshItemID(itemidmod);
                        //((AddonComponent)Components[i]).ItemID = itemid[i];
                        //Point3D newoffset = new Point3D(xoffset[i], yoffset[i], 0);
                        //((AddonComponent)Components[i]).Offset = newoffset;
                        //((AddonComponent)Components[i]).Location = new Point3D(newoffset.X + X, newoffset.Y + Y, z);
                    //}
                }
            //}
        }
		
        public override void OnAfterDelete()
        {

        }		
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
			base.GetContextMenuEntries(from, list);
            list.Add(new BackpackEntry(from, this));
        }
		
		public virtual void OnChop(Mobile from)
        {
			BaseGalleon galleon = null;
			if (LinkedSingleCannonPlace != null)
				galleon = LinkedSingleCannonPlace.Galleon;
			
			if (LinkedMultiCannonPlace != null)
				galleon = LinkedMultiCannonPlace.Galleon;

            if (galleon != null && (galleon.Owner == from) && galleon.Contains(this))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
				
				Delete();

                HeavyCannonDeed deed = new HeavyCannonDeed();

                from.AddToBackpack(deed);
            }
        }
		

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
			
			//version 1
			writer.Write(_singleCannon);
			
			if (_singleCannon)
				writer.Write(_linkedSingleCannonPlace);
			else
				writer.Write(_linkedMultiCannonPlace);
				
			
			
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch (version)
			{
				case 1 :
				{
					_singleCannon = reader.ReadBool();
					
					if (_singleCannon)
						_linkedSingleCannonPlace = reader.ReadItem() as SingleCannonPlace;
					else
						_linkedMultiCannonPlace = reader.ReadItem() as MultiCannonPlace;
						
					break;
				}
			}
        }
		
		private class BackpackEntry : ContextMenuEntry
		{
			private readonly HeavyShipCannon m_cannon;
			private readonly Mobile m_from;
			public BackpackEntry(Mobile from, HeavyShipCannon cannon)
				: base(2139)
			{
				m_cannon = cannon;
				m_from = from;
			}

			public override void OnClick()
			{
				if (m_cannon != null)
				{
					m_cannon.OnChop(m_from);
				}
			}
		}	

       public override void OnDoubleClick(Mobile from)
        {			
            if (Parent != null)
                return;

            // can't use destroyed weapons
            if (Hits == 0)
                return;

            // check the range between the player and weapon
            if (!from.InRange(Location, MinFiringRange) || from.Map != Map)
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (Storing)
            {
                from.SendMessage("{0} being stored", Name);
                return;
            }

            if (Projectile == null || Projectile.Deleted)
            {
                from.SendMessage("{0} empty", Name);
                return;
            }

            // check if the cannon is cool enough to fire
            if (NextFiring.Seconds > 0)
            {
                from.SendMessage("Not ready yet.");
                return;
            }
			
			//new way to fire
			Map map = Map;

			if ( map == null )
				return;

			int rx = 0, ry = 0;

			if ( Facing == 0 )
				rx = -1;
			else if ( Facing == 1 )
				ry = -1;
			else if ( Facing == 2 )
				rx = 1;
			else if ( Facing == 3 )
				ry = 1;

			for ( int i = 1; i <= 15; ++i )
			{
				int x = X + (i*rx);
				int y = Y + (i*ry);
				int z;

				for ( int j = -16; j <= 16; ++j )
				{
					z = from.Z + j;

					Point3D currentLocation = new Point3D( x, y, z );
					Item goldToken = new Gold(100);
					goldToken.Visible = false;
					goldToken.Map = Map;
					goldToken.Location = currentLocation;
					
					foreach (Item item in goldToken.GetItemsInRange(10))
						if (item is BaseShip)
						{
							BaseShip target = (BaseShip) item;
																	

							//Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
							// may have to reconsider the use tileloc vs target loc
							//m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);
							if (target != Galleon)
							{
								AttackTarget(from, target, target.Map.GetPoint(target, true), true);
								goldToken.Delete();
								return;
							}
							
						}

					goldToken.Delete();
						
				}

				z = map.GetAverageZ( x, y );


				Point3D currentLocation2 = new Point3D( x, y, z );
				Item goldToken2 = new Gold(100);
				goldToken2.Visible = false;
				goldToken2.Map = Map;
				goldToken2.Location = currentLocation2;
				foreach (Item item in goldToken2.GetItemsInRange(10))
					if (item is BaseShip)
					{
						BaseShip target = (BaseShip) item;
																

						//Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
						// may have to reconsider the use tileloc vs target loc
						//m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);												
						if (target != Galleon)
						{
							AttackTarget(from, target, target.Map.GetPoint(target, true), true);
							goldToken2.Delete();
							return;
						}						
					}	
					
				goldToken2.Delete();
			}			
        }				
    }	
}