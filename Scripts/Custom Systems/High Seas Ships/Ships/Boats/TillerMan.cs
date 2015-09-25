using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class NewTillerMan : BaseShipItem, IFacingChange
    {	
		private NewBaseBoat _boat;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public override bool ShareHue { get { return false; } }		
		
        [CommandProperty(AccessLevel.GameMaster)]
        public NewBaseBoat Boat { get { return _boat; } }		
	
        public NewTillerMan(NewBaseBoat boat, Point3D initOffset)
            : base(boat, 0x3E4E, initOffset)
        {
			_boat = boat;
			if (_boat != null)
				_boat.TillerMan = this;
        }

        public NewTillerMan(Serial serial)
            : base(serial)
        {
        }

        public void Say(int number)
        {
            if(!_boat.IsDriven)
                PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public void Say(int number, string args)
        {
            if (!_boat.IsDriven)
                PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }
		
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is MapItem && _boat != null && _boat.CanCommand(from) && _boat.Contains(from))
            {
                _boat.AssociateMap((MapItem)dropped);
            }

            return false;
        }		

        public override void OnDoubleClick(Mobile from)
        {
			_boat.GetMovingEntities();
			
			if (_boat != null && _boat.Contains(from))
			{
				if (!_boat.IsDriven)
				{
					from.SendMessage("You are now piloting this vessel");
					_boat.TakeCommand(from);
				}    
				else
				{
					_boat.LeaveCommand(from);
					from.SendMessage("You are no longer piloting this vessel");
				}
			}
        }

        public void SetFacing(Direction oldFacing, Direction newFacing)
        {
            switch (newFacing)
            {
                case Direction.South: _boat.SetItemIDOnSmooth(this, 0x3E4B); break;
                case Direction.North: _boat.SetItemIDOnSmooth(this, 0x3E4E); break;
                case Direction.West: _boat.SetItemIDOnSmooth(this, 0x3E50); break;
                case Direction.East: _boat.SetItemIDOnSmooth(this, 0x3E55); break;
            }

            if (oldFacing == Server.Direction.North)
            {
                Location = new Point3D(X - 1, Y, Z);
            }
            else if (newFacing == Server.Direction.North)
            {
                switch (oldFacing)
                {
                    case Server.Direction.South: Location =  new Point3D(X - 1, Y, Z); break;
                    case Server.Direction.East: Location =   new Point3D(X, Y + 1, Z); break;
                    case Server.Direction.West: Location =   new Point3D(X, Y - 1, Z); break;
                }
            }
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);//version
			
			// version 1 : _boat
			writer.Write((NewBaseBoat)_boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            switch (version)
            {
                case 1:
                    {
                        _boat = reader.ReadItem() as NewBaseBoat;
						
                        break;
                    }
            }			
        }
        #endregion
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
			base.GetContextMenuEntries(from, list);
			if (_boat != null && !_boat.Contains(from))
			{
				list.Add(new DryDockEntry(from, (NewBaseBoat)this._boat));
			}
			else
			{
				list.Add(new RenameEntry(from, (NewBaseBoat)this._boat));
			}
        }		

		private class RenameEntry : ContextMenuEntry
		{			
			private readonly Mobile _from;
			private NewBaseBoat _boat;
			
			public RenameEntry(Mobile from, NewBaseBoat boat)
				: base(1111680)
			{				
				_from = from;
				_boat = boat;
			}

			public override void OnClick()
			{
				if ((_from != null) && (_boat != null))
					_boat.BeginRename(_from);			
			}
		}	
		
		private class DryDockEntry : ContextMenuEntry
		{			
			private readonly Mobile _from;
			private NewBaseBoat _boat;
			
			public DryDockEntry(Mobile from, NewBaseBoat boat)
				: base(1116520)
			{				
				_from = from;
				_boat = boat;
			}

			public override void OnClick()
			{
				if ((_from != null) && (_boat != null))
					_boat.BeginDryDock(_from);			
			}
		}

		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (_boat != null)
			{
				_boat.CheckDecay();
				
				switch ( _boat.Phase )
				{
					case DecayPhase.New:
					{
						list.Add(1043010);
						
						break;
					}

					case DecayPhase.SlightlyWorn:
					{
						list.Add(1043011);
						
						break;
					}
					
					case DecayPhase.SomewhatWorn:
					{
						list.Add(1043012);
						
						break;
					}
					
					case DecayPhase.FairlyWorn:
					{
						list.Add(1043013);
						
						break;
					}
					
					case DecayPhase.GreatlyWorn:
					{
						list.Add(1043014);
						
						break;
					}
					
					case DecayPhase.Collapsing:
					{
						list.Add(1043015);
						
						break;
					}										
				}								   
			}
        }		
    }
}