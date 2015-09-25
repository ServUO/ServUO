using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public interface IHelm
    {
        void RefreshItemID(int mod);
    }

    public interface ICannonPlace
    {
        void RefreshItemID(int mod);
    }

    public interface ICannon
    {
        void RefreshItemID(int mod);
    }

    public class SingleHelm : BaseGalleonItem, IHelm
    {
        public SingleHelm(BaseGalleon galleon, int northItemID, Point3D initOffset)
            : base(galleon, northItemID, initOffset)
        {
            Name = "Wheel";
        }

        public SingleHelm(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon.IsDriven)
            {
                Galleon.LeaveCommand(from);
                from.SendLocalizedMessage(1149592); //You are no longer piloting this vessel. -Dian
            }
            else if (Galleon != null && from != null) 
			{
				if (!Galleon.Contains(from)) // -Dian
				{
					from.SendLocalizedMessage(1116724); //You cannot pilot a ship unless you are aboard it! -Dian
					return;
				}
					
				if (from.Mounted)
				{
					from.SendLocalizedMessage(1042146); //You cannot use this while mounted. -Dian
					return;
				}
					
				if (from.Flying) // -Dian
				{
					from.SendLocalizedMessage(1116615); //You cannot pilot a ship while flying! -Dian
					return;
				}																						
		
				if (from == Galleon.Owner)
				{
					from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
					Galleon.TakeCommand(from);
				}
				else if (Galleon.PlayerAccess != null)
				{
					if (Galleon.PlayerAccess.ContainsKey((PlayerMobile)from))
					{
						if ( 2 <= Galleon.PlayerAccess[(PlayerMobile)from] && Galleon.PlayerAccess[(PlayerMobile)from] <= 4)
						{
							from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
							Galleon.TakeCommand(from);
						}
						else if ((from.Guild != null) && (from.Guild == Galleon.Owner.Guild))
						{
							if ( 2 <= Galleon.Guild && Galleon.Guild <= 4)
							{
								from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
								Galleon.TakeCommand(from);
							}
							else
							{
								from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
							}
						}
						else if ((from.Party != null) && (from.Party == Galleon.Owner.Party) )
						{
							if (2 <= Galleon.Party && Galleon.Party <= 4)
							{
								from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
								Galleon.TakeCommand(from);
							}
							else
							{
								from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
							}								
						}
						else if (2 <= Galleon.Public && Galleon.Public <= 4)
						{
							from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
							Galleon.TakeCommand(from);
						}
						else
						{
							from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
						}								
					}
					else
					{
						from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
					}
				}
				else
				{
					from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
				}                           
            }
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //current version is 0


        }
        #endregion
    }

    public class MultiHelm : BaseGalleonMultiItem, IHelm
    {
        public MultiHelm(BaseGalleon galleon, int northItemID, Point3D initOffset, int sxNorthItemId, int dxNorthItemId)
            : base(galleon, northItemID, initOffset)
        {
            AddComponent(new GalleonMultiComponent(sxNorthItemId, this, new Point3D(-1, 0, 0)));
            AddComponent(new GalleonMultiComponent(dxNorthItemId, this, new Point3D(1, 0, 0)));
        }

        public MultiHelm(BaseGalleon galleon, int northItemID, Point3D initOffset, int relatedNorthItemId, Point3D relatedOffset)
            : base(galleon, northItemID, initOffset)
        {
            AddComponent(new GalleonMultiComponent(relatedNorthItemId, this, relatedOffset));
        }

        public MultiHelm(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon.IsDriven)
            {
                Galleon.LeaveCommand(from);
                from.SendLocalizedMessage(1149592); //You are no longer piloting this vessel. -Dian
            }
            else if (Galleon != null && from != null) 
			{
				if (!Galleon.Contains(from)) //added Galleon.Contains(from) -Dian
				{
					from.SendLocalizedMessage(1116724); //You cannot pilot a ship unless you are aboard it! -Dian
					return;
				}
				
				if (from.Mounted)
				{
					from.SendLocalizedMessage(1042146); //You cannot use this while mounted. -Dian
					return;
				}
				
				if (from.Flying) // Added -Dian
				{
					from.SendLocalizedMessage(1116615); //You cannot pilot a ship while flying! -Dian
					return;
				}																						
	
				if (from == Galleon.Owner)
				{
					from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
					Galleon.TakeCommand(from);
				}
				else if (Galleon.PlayerAccess != null)
				{
					if (Galleon.PlayerAccess.ContainsKey((PlayerMobile)from))
					{
						if ( 2 <= Galleon.PlayerAccess[(PlayerMobile)from] && Galleon.PlayerAccess[(PlayerMobile)from] <= 4)
						{
							from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
							Galleon.TakeCommand(from);
						}
						else if ((from.Guild != null) && (from.Guild == Galleon.Owner.Guild))
						{
							if ( 2 <= Galleon.Guild && Galleon.Guild <= 4)
							{
								from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
								Galleon.TakeCommand(from);
							}
							else
							{
								from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
							}
						}
						else if ((from.Party != null) && (from.Party == Galleon.Owner.Party) )
						{
							if (2 <= Galleon.Party && Galleon.Party <= 4)
							{
								from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
								Galleon.TakeCommand(from);
							}
							else
							{
								from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
							}								
						}
						else if (2 <= Galleon.Public && Galleon.Public <= 4)
						{
							from.SendLocalizedMessage(1116727); //You are now piloting this vessel. -Dian
							Galleon.TakeCommand(from);
						}
						else
						{
							from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
						}							
					}
					else
					{
						from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
					}
				}
				else
				{
					from.SendLocalizedMessage(1061637); //You are not allowed to access this. -Dian
				}                 
			}
        }

        #region Serialization
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //current version is 0
        }
        #endregion
    }

    public class GalleonHold : BaseGalleonMultiContainer
    {
        private int _maxWeight;

        [CommandProperty(AccessLevel.GameMaster)]
        public override int MaxWeight { get { return _maxWeight; } }
		
        public GalleonHold(BaseGalleon galleon, int northItemId, Point3D initOffset, List<Tuple<int, Point3D>> componentList, int HoldMaxWeight = 16000)
            : base(galleon, northItemId, initOffset)
        {
            Name = "Cargo Hold";
            _maxWeight = HoldMaxWeight;
			
			if (galleon != null)
				galleon.Hold = this;

            foreach (Tuple<int, Point3D> comp in componentList)
            {
                AddComponent(new GalleonMultiComponent(comp.Item1, this, comp.Item2));
            }

            LiftOverride = true;
        }

        public GalleonHold(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon == null || !Galleon.IsOnBoard(from))
            {
                if (Galleon.TillerMan != null)
                    Galleon.TillerMan.TillerManSay(502490); // You must be on the ship to open the hold.
            }
            else if (Galleon.IsMovingShip)
            {
                if (Galleon.TillerMan != null)
                    Galleon.TillerMan.TillerManSay(502491); // I can not open the hold while the ship is moving.
            }
            else
            {				
				if (Galleon != null)
				{
					if (from == Galleon.Owner)
					{
						base.OnDoubleClick(from);
					}
					else if (Galleon.PlayerAccess != null)
					{
						if (Galleon.PlayerAccess.ContainsKey((PlayerMobile)from))
						{
							if (3 <= Galleon.PlayerAccess[(PlayerMobile)from] && Galleon.PlayerAccess[(PlayerMobile)from] <= 4)
							{
								base.OnDoubleClick(from);
							}
							else if ((from.Guild == Galleon.Owner.Guild) && (from.Guild != null))
							{
								if (3 <= Galleon.Guild && Galleon.Guild <= 4)
								{
									base.OnDoubleClick(from);
								}
								else if ((from.Party == Galleon.Owner.Party) && (from.Party != null))
								{
									if (3 <= Galleon.Party && Galleon.Party <= 4)
									{
										base.OnDoubleClick(from);
									}
									else
									{
										if (3 <= Galleon.Public && Galleon.Public <= 4)
										{
											base.OnDoubleClick(from);
										}
									}
								}
							}
						}
					}
				}
			}
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // current version is 1

            // version 1
            writer.Write((int)_maxWeight);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        _maxWeight = (int)reader.ReadInt();
                        break;
                    }
            }
        }
    }

    public class SingleCannonPlace : BaseGalleonItem, ICannonPlace
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public LightShipCannon LinkedLightCannon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public HeavyShipCannon LinkedHeavyCannon { get; set; }

	
        public SingleCannonPlace(BaseGalleon galleon, int northItemId, Point3D initOffset)
            : base(galleon, northItemId, initOffset)
        {
            Name = " ";
        }

        public SingleCannonPlace(Serial serial)
            : base(serial)
        {
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            //version 1
            writer.Write(LinkedLightCannon);
			writer.Write(LinkedHeavyCannon);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
						LinkedLightCannon = reader.ReadItem() as LightShipCannon;
						LinkedHeavyCannon = reader.ReadItem() as HeavyShipCannon;
                        
                        break;
                    }
            }
        }
        #endregion
    }

    public class MultiCannonPlace : BaseGalleonMultiItem, ICannonPlace
    {
        public MultiCannonPlace(BaseGalleon galleon, int northItemId, Point3D initOffset, int sxNorthItemId)
            : base(galleon, northItemId, initOffset)
        {
            Name = " ";
            AddComponent(new GalleonMultiComponent(sxNorthItemId, this, new Point3D(0, 1, 0)));
        }

        public MultiCannonPlace(BaseGalleon galleon, int northItemId, Point3D initOffset, int sxNorthItemId, int dxNorthItemId)
            : base(galleon, northItemId, initOffset)
        {
            Name = " ";
            AddComponent(new GalleonMultiComponent(sxNorthItemId, this, new Point3D(0, 1, 0)));
            AddComponent(new GalleonMultiComponent(dxNorthItemId, this, new Point3D(0, -1, 0)));
        }

        public MultiCannonPlace(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public LightShipCannon LinkedLightCannon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public HeavyShipCannon LinkedHeavyCannon { get; set; }


        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            //version 1
			writer.Write((LightShipCannon)LinkedLightCannon);
            writer.Write((HeavyShipCannon)LinkedHeavyCannon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        LinkedLightCannon = reader.ReadItem() as LightShipCannon;
						LinkedHeavyCannon = reader.ReadItem() as HeavyShipCannon;
						
                        break;
                    }
            }
        }
        #endregion
    }

    public class MainMast : BaseGalleonItem
    {
        #region Properties
        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                if (base.Hue == value)
                    return;

                base.Hue = value;
            }
        }	

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { if (Galleon != null) return Galleon.Owner; return null; }
            set { if (Galleon != null) Galleon.Owner = value; }
        }		

        [CommandProperty(AccessLevel.GameMaster)]
        public ushort Durability
        {
            get { if (Galleon != null) return Galleon.Durability; return 0; }
            set { if (Galleon != null) Galleon.Durability = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonStatus Status
        {
            get { if (Galleon != null) return Galleon.Status; return GalleonStatus.Low; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SpeedCode CurrentSpeed
        {
            get { if (Galleon != null) return Galleon.Speed; return SpeedCode.Stop; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing
        {
            get { if (Galleon != null) return Galleon.Direction; return Direction.North; }
            set { if (Galleon != null) Galleon.Facing = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsDriven
        {
            get { if (Galleon != null) return Galleon.IsDriven; return false; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMoving
        {
            get { if (Galleon != null) return Galleon.IsMoving; return false; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Moving
        {
            get { if (Galleon != null) return Galleon.Moving; return Direction.North; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Pilot
        {
            get { if (Galleon != null) return Galleon.Pilot; return null; }
            set { if (Galleon != null) Galleon.Pilot = value; }
        }
        #endregion

        public MainMast(BaseGalleon galleon, int baseNorthId, Point3D initOffset)
            : base(galleon, baseNorthId, initOffset)
        {
        }

        public MainMast(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon != null)
                Galleon.OnDoubleClick(from);
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
        #endregion
    }

    public class BritainHull : BaseGalleonItem
    {
        private static int[,] _internalItemIDMods = new int[,]
        {
            {1536, 1260, 1812, 2088 },
            {0, -276, 276, 552 },
            {0, -276, 276, 552 },
        };

        public BritainHull(BaseGalleon galleon, int northItemId)
            : base(galleon, northItemId)
        {
        }

        public BritainHull(Serial serial)
            : base(serial)
        {
        }

        public override void RefreshItemID(int itemIDModifier)
        {
            if (Galleon is BaseGalleon)
            {
                BaseGalleon trans = (BaseGalleon)Galleon;
                ItemID = (BaseItemID + _internalItemIDMods[(int)trans.Status, (int)trans.Facing / 2]);
            }
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
        #endregion
    }

    public enum BoatRopeSide { Port, Starboard }

    public class BoatRope : BaseGalleonItem, ILockable
    {
        [CommandProperty(AccessLevel.GameMaster, true)]
        public override bool ShareHue
        {
            get { return false; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BoatRopeSide Side { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Starboard { get { return (Side == BoatRopeSide.Starboard); } }

        public BoatRope(BaseGalleon boat, int northItemID, Point3D initOffset, BoatRopeSide side, uint keyValue)
            : base(boat, northItemID, initOffset)
        {
            KeyValue = keyValue;
            Side = side;
            Locked = true;
            Movable = false;
            Name = "Mooring Line";
        }

        public BoatRope(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            base.OnDoubleClick(from);
            from.MoveToWorld(Location, Map);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon != null && !Galleon.Contains(from))
            {
				if (!(from.InRange(Location, 12)))
					return;
				
                Galleon.Refresh();

                if (from == Galleon.Owner)
                {
                    base.OnDoubleClick(from);
                    from.MoveToWorld(Location, Map);
					Galleon.Embark(from);
                }
                else if (Galleon.PlayerAccess != null)
                {
                    if (Galleon.PlayerAccess.ContainsKey((PlayerMobile)from))
                    {
                        if (1 <= Galleon.PlayerAccess[(PlayerMobile)from] && Galleon.PlayerAccess[(PlayerMobile)from] <= 4)
                        {
                            base.OnDoubleClick(from);
                            from.MoveToWorld(Location, Map);
							Galleon.Embark(from);
                        }
                        else
                        {
                            from.SendLocalizedMessage(1116617); //You do not have permission to board this ship. -Dian
                            base.OnDoubleClick(from);
                        }
                    }
                    else if ((Galleon.Owner != null) && (from.Guild == Galleon.Owner.Guild) && (from.Guild != null))
                    {
                        if (1 <= Galleon.Guild && Galleon.Guild <= 4)
                        {
                            base.OnDoubleClick(from);
                            from.MoveToWorld(Location, Map);
							Galleon.Embark(from);
                        }
                        else
                        {
                            from.SendLocalizedMessage(1116617); //You do not have permission to board this ship. -Dian
                            base.OnDoubleClick(from);
                        }
                    }
                    else if ((Galleon.Owner != null) && (from.Party == Galleon.Owner.Party) && (from.Party != null))
                    {
                        if (1 <= Galleon.Party && Galleon.Party <= 4)
                        {
                            base.OnDoubleClick(from);
                            from.MoveToWorld(Location, Map);
							Galleon.Embark(from);
                        }
                        else
                        {
                            from.SendLocalizedMessage(1116617); //You do not have permission to board this ship. -Dian
                            base.OnDoubleClick(from);
                        }
                    }
                    else if (1 <= Galleon.Public && Galleon.Public <= 4)
                    {
                        base.OnDoubleClick(from);
                        from.MoveToWorld(Location, Map);
						Galleon.Embark(from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1116617); //You do not have permission to board this ship. -Dian
                        base.OnDoubleClick(from);
                    }
                }
                else if ((Galleon.Owner != null) && (from.Guild == Galleon.Owner.Guild) && (from.Guild != null))
                {
                    if (1 <= Galleon.Guild && Galleon.Guild <= 4)
                    {
                        base.OnDoubleClick(from);
                        from.MoveToWorld(Location, Map);
						Galleon.Embark(from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1116617); //You do not have permission to board this ship. -Dian
                        base.OnDoubleClick(from);
                    }
                }
                else if ((Galleon.Owner != null) && (from.Party == Galleon.Owner.Party) && (from.Party != null))
                {
                    if (1 <= Galleon.Party && Galleon.Party <= 4)
                    {
                        base.OnDoubleClick(from);
                        from.MoveToWorld(Location, Map);
						Galleon.Embark(from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1116617); //You do not have permission to board this ship. -Dian
                        base.OnDoubleClick(from);
                    }
                }
                else if (1 <= Galleon.Public && Galleon.Public <= 4)
                {
                    base.OnDoubleClick(from);
                    from.MoveToWorld(Location, Map);
					Galleon.Embark(from);
                }
                else
                {
                    from.SendLocalizedMessage(1116617); //You do not have permission to board this ship. -Dian
                    base.OnDoubleClick(from);
                }
            }
            else if (Galleon != null && Galleon.Contains(from))
            {
				Galleon.Refresh();
				
				if (Galleon.IsDriven)
                {
					from.SendLocalizedMessage(1116610); //You can't do that while piloting a ship! -Dian
					return;
				}
				
                Map map = Map;

                if (map == null)
                    return;

                int rx = 0, ry = 0;

                if (Side == BoatRopeSide.Port)
                {
                    if (Galleon.Facing == Direction.North)
                        rx = 1;
                    else if (Galleon.Facing == Direction.South)
                        rx = -1;
                    else if (Galleon.Facing == Direction.East)
                        ry = 1;
                    else if (Galleon.Facing == Direction.West)
                        ry = -1;
                }
                else if (Side == BoatRopeSide.Starboard)
                {
                    if (Galleon.Facing == Direction.North)
                        rx = -1;
                    else if (Galleon.Facing == Direction.South)
                        rx = 1;
                    else if (Galleon.Facing == Direction.East)
                        ry = -1;
                    else if (Galleon.Facing == Direction.West)
                        ry = 1;
                }

                for (int i = 1; i <= 12; ++i)
                {
                    int x = X + (i * rx);
                    int y = Y + (i * ry);
                    int z;

                    for (int j = -16; j <= 16; ++j)
                    {
                        z = from.Z + j;

                        if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                        {
                            if (i == 1 && j >= -2 && j <= 2)
                                return;

                            from.Location = new Point3D(x, y, z);
							Galleon.Disembark(from);
                            return;
                        }
                    }

                    z = map.GetAverageZ(x, y);

                    if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                    {
                        if (i == 1)
                            return;

                        from.Location = new Point3D(x, y, z);
						Galleon.Disembark(from);
                        return;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);//version

            writer.Write((int)Side);
            writer.Write(Locked);
            writer.Write(KeyValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        Side = (BoatRopeSide)reader.ReadInt();
                        Locked = reader.ReadBool();
                        KeyValue = reader.ReadUInt();

                        if (Galleon == null)
                            Delete();

                        break;
                    }
            }
        }
    }
}
