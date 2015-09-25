using System;
using System.Reflection;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class LightCannonDeed : Item
    {
        [Constructable]
        public LightCannonDeed()
            : base(0x14F0)
        {
            Hue = 0x488;
            Weight = 1.0;
            LootType = LootType.Blessed;
            Name = "a deed for a Light Ship Cannon";
        }

        public LightCannonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            LootType = LootType.Blessed;
        }

        public bool ValidatePlacement(Mobile from, Point3D loc)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return false;
            }
			

            Map map = from.Map;

            if (map == null)
                return false;

            BaseGalleon galleon = BaseGalleon.FindGalleonAt(loc, map);

            /* Removed for now, but will be reintroduced later for Boat Security
			*if (galleon != null && !galleon.IsFriend(from))
            *{
            *    from.SendLocalizedMessage(500269); // You cannot build that there.
            *    return false;
            *}
			*/

            return true;
        }

        public void BeginPlace(Mobile from)
        {
            from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Placement_OnTarget));
        }

        public void Placement_OnTarget(Mobile from, object targeted)
        {
			#region HS Ships
			bool singleCannon = false;
			bool multiCannon = false;
			if (targeted is SingleCannonPlace)
				singleCannon = true;
			else if (targeted is MultiCannonPlace)
				multiCannon = true;
			
				
			if ((singleCannon == false) && (multiCannon == false))
				return;
			#endregion
		
            IPoint3D p = targeted as IPoint3D;

            if (p == null)
                return;

            Point3D loc = new Point3D(p);

            if (p is StaticTarget)
                loc.Z -= TileData.ItemTable[((StaticTarget)p).ItemID & 0x3FFF].CalcHeight; /* NOTE: OSI does not properly normalize Z positioning here.
            * A side affect is that you can only place on floors (due to the CanFit call).
            * That functionality may be desired. And so, it's included in this script.
            */
			
			#region SmoothMulti
			SingleCannonPlace targetedSingleCannonPlace = null;
			MultiCannonPlace targetedMultiCannonPlace = null;
			if (targeted is SingleCannonPlace)
				targetedSingleCannonPlace = (SingleCannonPlace)targeted;
			else if (targeted is MultiCannonPlace)
				targetedMultiCannonPlace = (MultiCannonPlace)targeted;
				
			int direction = 0;
			
			if (singleCannon)
			{
				if (targetedSingleCannonPlace.Galleon is TokunoGalleon)
				{
					loc.Z += 8;

					object objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
					SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
					SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
					SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
					SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
					SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;				
				
				
					if (targetedSingleCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 1;
							loc.Z += 7;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 2;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 2;
							loc.Z += 7;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 3;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 3;
							loc.Z += 7;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 0;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 0;
							loc.Z += 7;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 1;
					}
				}
				else if (targetedSingleCannonPlace.Galleon is BritainGalleon)
				{
					loc.Z += 19;
					object objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
					SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
					SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
					SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSupSx");
					SingleCannonPlace CannonMidSupSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSupDx");
					SingleCannonPlace CannonMidSupDx = objValue as SingleCannonPlace;	
					
					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidInfSx");
					SingleCannonPlace CannonMidInfSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidInfDx");
					SingleCannonPlace CannonMidInfDx = objValue as SingleCannonPlace;					
					
					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
					SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
					SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;				
				
				
					if (targetedSingleCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 1;	
							loc.Z += 5;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidSupSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidSupDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidInfSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidInfDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 2;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 2;
							loc.Z += 5;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidSupSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidSupDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidInfSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidInfDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 3;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 3;
							loc.Z += 5;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidSupSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidSupDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidInfSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidInfDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 0;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 0;
							loc.Z += 5;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidSupSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidSupDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidInfSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidInfDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 1;
					}
				}
				else if (targetedSingleCannonPlace.Galleon is GargoyleGalleon)
				{
					loc.Z += 15;
					object objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
					SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
					SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
					SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	
					
					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSx");
					SingleCannonPlace CannonMidSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidDx");
					SingleCannonPlace CannonMidDx = objValue as SingleCannonPlace;

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
					SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
					SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;				
				
				
					if (targetedSingleCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 1;
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 2;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 2;
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 3;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 3;
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 0;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 0;
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 1;
					}
				}
				else if (targetedSingleCannonPlace.Galleon is OrcGalleon) 	
				{
					loc.Z += 14;
					object objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
					SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;		
					
					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSx");
					SingleCannonPlace CannonMidSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidDx");
					SingleCannonPlace CannonMidDx = objValue as SingleCannonPlace;				
				
				
					if (targetedSingleCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 2;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 3;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 0;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 1;
					}	
				}
			}
			else if (multiCannon)
			{
				if (targetedMultiCannonPlace.Galleon is OrcGalleon) 	
				{
					loc.Z += 14;

					object objValue = targetedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
					MultiCannonPlace CannonSupSx = objValue as MultiCannonPlace;	

					objValue = targetedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
					MultiCannonPlace CannonSupDx = objValue as MultiCannonPlace;	

					objValue = targetedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
					MultiCannonPlace CannonInfSx = objValue as MultiCannonPlace;	

					objValue = targetedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
					MultiCannonPlace CannonInfDx = objValue as MultiCannonPlace;				
				
				
					if (targetedMultiCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedMultiCannonPlace == CannonSupSx)
							direction = 0;
						if (targetedMultiCannonPlace == CannonSupDx)
							direction = 2;
						if (targetedMultiCannonPlace == CannonInfSx)
							direction = 0;
						if (targetedMultiCannonPlace == CannonInfDx)
							direction = 2;
					}
					else if (targetedMultiCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedMultiCannonPlace == CannonSupSx)
							direction = 1;
						if (targetedMultiCannonPlace == CannonSupDx)
							direction = 3;
						if (targetedMultiCannonPlace == CannonInfSx)
							direction = 1;
						if (targetedMultiCannonPlace == CannonInfDx)
							direction = 3;
					}
					else if (targetedMultiCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedMultiCannonPlace == CannonSupSx)
							direction = 2;
						if (targetedMultiCannonPlace == CannonSupDx)
							direction = 0;
						if (targetedMultiCannonPlace == CannonInfSx)
							direction = 2;
						if (targetedMultiCannonPlace == CannonInfDx)
							direction = 0;
					}
					else if (targetedMultiCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedMultiCannonPlace == CannonSupSx)
							direction = 3;
						if (targetedMultiCannonPlace == CannonSupDx)
							direction = 1;
						if (targetedMultiCannonPlace == CannonInfSx)
							direction = 3;
						if (targetedMultiCannonPlace == CannonInfDx)
							direction = 1;
					}
				}
			}
				
			#endregion
	
			if (singleCannon)
				if (ValidatePlacement(from, loc))
					EndPlace(from, loc, direction, targetedSingleCannonPlace.Galleon, targetedSingleCannonPlace, null);
					
			if (multiCannon)
				if (ValidatePlacement(from, loc))
					EndPlace(from, loc, direction, targetedMultiCannonPlace.Galleon, null, targetedMultiCannonPlace);
        }

        public void EndPlace(Mobile from, Point3D loc, int direction, BaseGalleon galleon, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
        {
            if (from == null)
                return;

            Delete();
            LightShipCannon cannon = new LightShipCannon(direction, galleon, 0x4218, loc, targetedSingleCannonPlace, targetedMultiCannonPlace);

            cannon.Location = loc;
            cannon.Map = from.Map;
			#region SmoothMulti#
			//cannon.Facing = direction;
			#endregion
				
        }

        public override void OnDoubleClick(Mobile from)
        {
            BeginPlace(from);
        }
    }
	
    public class HeavyCannonDeed : Item
    {
        [Constructable]
        public HeavyCannonDeed()
            : base(0x14F0)
        {
            Hue = 0x488;
            Weight = 1.0;
            LootType = LootType.Blessed;
            Name = "a deed for a Heavy Ship Cannon";
        }

        public HeavyCannonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            LootType = LootType.Blessed;
        }

        public bool ValidatePlacement(Mobile from, Point3D loc)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return false;
            }

            Map map = from.Map;

            if (map == null)
                return false;

            BaseGalleon galleon = BaseGalleon.FindGalleonAt(loc, map);

            /* Removed for now, but will be reintroduced later for Boat Security
			*if (galleon != null && !galleon.IsFriend(from))
            *{
            *    from.SendLocalizedMessage(500269); // You cannot build that there.
            *    return false;
            *}
			*/

            return true;
        }

        public void BeginPlace(Mobile from)
        {
            from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Placement_OnTarget));
        }

        public void Placement_OnTarget(Mobile from, object targeted)
        {
			#region SmoothMulti
			bool singleCannon = false;
			bool multiCannon = false;
			if (targeted is SingleCannonPlace)
				singleCannon = true;
			else if (targeted is MultiCannonPlace)
				multiCannon = true;
			
				
			if ((singleCannon == false) && (multiCannon == false))
				return;
			#endregion
		
            IPoint3D p = targeted as IPoint3D;

            if (p == null)
                return;

            Point3D loc = new Point3D(p);

            if (p is StaticTarget)
                loc.Z -= TileData.ItemTable[((StaticTarget)p).ItemID & 0x3FFF].CalcHeight; /* NOTE: OSI does not properly normalize Z positioning here.
            * A side affect is that you can only place on floors (due to the CanFit call).
            * That functionality may be desired. And so, it's included in this script.
            */
			
			#region SmoothMulti
			SingleCannonPlace targetedSingleCannonPlace = null;
			MultiCannonPlace targetedMultiCannonPlace = null;
			if (targeted is SingleCannonPlace)
				targetedSingleCannonPlace = (SingleCannonPlace)targeted;
			else if (targeted is MultiCannonPlace)
				targetedMultiCannonPlace = (MultiCannonPlace)targeted;
				
			int direction = 0;
			
			if (singleCannon)
			{
				if (targetedSingleCannonPlace.Galleon is TokunoGalleon)
				{
					loc.Z += 8;

					object objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
					SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
					SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
					SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
					SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
					SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;				
				
				
					if (targetedSingleCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 1;
							loc.Z += 7;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 2;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 2;
							loc.Z += 7;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 3;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 3;
							loc.Z += 7;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 0;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 0;
							loc.Z += 7;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 1;
					}
				}
				else if (targetedSingleCannonPlace.Galleon is BritainGalleon)
				{
					loc.Z += 19;
					object objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
					SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
					SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
					SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSupSx");
					SingleCannonPlace CannonMidSupSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSupDx");
					SingleCannonPlace CannonMidSupDx = objValue as SingleCannonPlace;	
					
					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidInfSx");
					SingleCannonPlace CannonMidInfSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidInfDx");
					SingleCannonPlace CannonMidInfDx = objValue as SingleCannonPlace;					
					
					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
					SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
					SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;				
				
				
					if (targetedSingleCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 1;	
							loc.Z += 5;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidSupSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidSupDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidInfSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidInfDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 2;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 2;
							loc.Z += 5;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidSupSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidSupDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidInfSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidInfDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 3;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 3;
							loc.Z += 5;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidSupSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidSupDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidInfSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidInfDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 0;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedSingleCannonPlace == CannonCenter)
						{
							direction = 0;
							loc.Z += 5;
						}
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidSupSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidSupDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidInfSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidInfDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 1;
					}
				}
				else if (targetedSingleCannonPlace.Galleon is GargoyleGalleon)
				{
					loc.Z += 15;
					object objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
					SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
					SingleCannonPlace CannonSupSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
					SingleCannonPlace CannonSupDx = objValue as SingleCannonPlace;	
					
					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSx");
					SingleCannonPlace CannonMidSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidDx");
					SingleCannonPlace CannonMidDx = objValue as SingleCannonPlace;

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
					SingleCannonPlace CannonInfSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
					SingleCannonPlace CannonInfDx = objValue as SingleCannonPlace;				
				
				
					if (targetedSingleCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 1;
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 2;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 2;
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 3;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 3;
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 0;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 0;
						if (targetedSingleCannonPlace == CannonSupSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonSupDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonInfSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonInfDx)
							direction = 1;
					}
				}
				else if (targetedSingleCannonPlace.Galleon is OrcGalleon) 	
				{
					loc.Z += 14;
					object objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonCenter");
					SingleCannonPlace CannonCenter = objValue as SingleCannonPlace;		
					
					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidSx");
					SingleCannonPlace CannonMidSx = objValue as SingleCannonPlace;	

					objValue = targetedSingleCannonPlace.Galleon.GetPropertyValue("CannonMidDx");
					SingleCannonPlace CannonMidDx = objValue as SingleCannonPlace;				
				
				
					if (targetedSingleCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 2;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 1;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 3;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 2;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 0;
					}
					else if (targetedSingleCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedSingleCannonPlace == CannonCenter)
							direction = 0;
						if (targetedSingleCannonPlace == CannonMidSx)
							direction = 3;
						if (targetedSingleCannonPlace == CannonMidDx)
							direction = 1;
					}	
				}
			}
			else if (multiCannon)
			{
				if (targetedMultiCannonPlace.Galleon is OrcGalleon) 	
				{
					loc.Z += 14;

					object objValue = targetedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupSx");
					MultiCannonPlace CannonSupSx = objValue as MultiCannonPlace;	

					objValue = targetedMultiCannonPlace.Galleon.GetPropertyValue("CannonSupDx");
					MultiCannonPlace CannonSupDx = objValue as MultiCannonPlace;	

					objValue = targetedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfSx");
					MultiCannonPlace CannonInfSx = objValue as MultiCannonPlace;	

					objValue = targetedMultiCannonPlace.Galleon.GetPropertyValue("CannonInfDx");
					MultiCannonPlace CannonInfDx = objValue as MultiCannonPlace;				
				
				
					if (targetedMultiCannonPlace.Galleon.Facing == Direction.North)
					{
						if (targetedMultiCannonPlace == CannonSupSx)
							direction = 0;
						if (targetedMultiCannonPlace == CannonSupDx)
							direction = 2;
						if (targetedMultiCannonPlace == CannonInfSx)
							direction = 0;
						if (targetedMultiCannonPlace == CannonInfDx)
							direction = 2;
					}
					else if (targetedMultiCannonPlace.Galleon.Facing == Direction.East)
					{
						if (targetedMultiCannonPlace == CannonSupSx)
							direction = 1;
						if (targetedMultiCannonPlace == CannonSupDx)
							direction = 3;
						if (targetedMultiCannonPlace == CannonInfSx)
							direction = 1;
						if (targetedMultiCannonPlace == CannonInfDx)
							direction = 3;
					}
					else if (targetedMultiCannonPlace.Galleon.Facing == Direction.South)
					{
						if (targetedMultiCannonPlace == CannonSupSx)
							direction = 2;
						if (targetedMultiCannonPlace == CannonSupDx)
							direction = 0;
						if (targetedMultiCannonPlace == CannonInfSx)
							direction = 2;
						if (targetedMultiCannonPlace == CannonInfDx)
							direction = 0;
					}
					else if (targetedMultiCannonPlace.Galleon.Facing == Direction.West)
					{
						if (targetedMultiCannonPlace == CannonSupSx)
							direction = 3;
						if (targetedMultiCannonPlace == CannonSupDx)
							direction = 1;
						if (targetedMultiCannonPlace == CannonInfSx)
							direction = 3;
						if (targetedMultiCannonPlace == CannonInfDx)
							direction = 1;
					}
				}
			}
				
			#endregion
	
			if (singleCannon)
				if (ValidatePlacement(from, loc))
					EndPlace(from, loc, direction, targetedSingleCannonPlace.Galleon, targetedSingleCannonPlace, null);
					
			if (multiCannon)
				if (ValidatePlacement(from, loc))
					EndPlace(from, loc, direction, targetedMultiCannonPlace.Galleon, null, targetedMultiCannonPlace);
        }

        public void EndPlace(Mobile from, Point3D loc, int direction, BaseGalleon galleon, SingleCannonPlace targetedSingleCannonPlace, MultiCannonPlace targetedMultiCannonPlace)
        {
            if (from == null)
                return;

            Delete();
            HeavyShipCannon cannon = new HeavyShipCannon(direction, galleon, 0x421C, loc, targetedSingleCannonPlace, targetedMultiCannonPlace);

            cannon.Location = loc;
            cannon.Map = from.Map;
			#region HS Ships
			//cannon.Facing = direction;
			#endregion
				
        }

        public override void OnDoubleClick(Mobile from)
        {
            BeginPlace(from);
        }
    }
	
	public static class PropertyHelper
	{
		public static object GetPropertyValue<T>(this T classInstance, string propertyName)
		{
			PropertyInfo property = classInstance.GetType().GetProperty(propertyName);
			if (property != null)
				return property.GetValue(classInstance, null);
			return null;
		}
	}
	
}