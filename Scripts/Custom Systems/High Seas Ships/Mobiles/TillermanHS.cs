using System;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.ContextMenus;
using Server.Items;
using Server.Movement;
using Server.Network;
using Server.Multis;
using Server.Gumps;

namespace Server.Mobiles
{
    public class TillerManHS : Mobile
    {
        private BaseGalleon _galleon;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue { get; set; }		

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get { return _galleon; } }			

        public TillerManHS(BaseGalleon galleon, uint keyValue, Point3D initOffset)
            : base()
        {
			Location = initOffset;
			if (galleon != null)
			{
				galleon.Embark(this);
				galleon.TillerMan = this;
				_galleon = galleon;
			}
			KeyValue = keyValue;
            CantWalk = true;
			Blessed = true;

            InitStats(31, 41, 51);

            SpeechHue = Utility.RandomDyedHue();

            Hue = Utility.RandomSkinHue();


            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                AddItem(new Kilt(Utility.RandomDyedHue()));
                AddItem(new Shirt(Utility.RandomDyedHue()));
                AddItem(new ThighBoots());
                Title = "the tillerman";
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new ShortPants(Utility.RandomNeutralHue()));
                AddItem(new Shirt(Utility.RandomDyedHue()));
                AddItem(new Sandals());
                Title = "the tillerman";
            }

            AddItem(new Bandana(Utility.RandomDyedHue()));

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

            pack.Movable = false;

            AddItem(pack);
        }

        public TillerManHS(Serial serial)
            : base(serial)
        {
        }		

        public void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.South: Direction = Server.Direction.South; break;
                case Direction.North: Direction = Server.Direction.North; break;
                case Direction.West: Direction = Server.Direction.West; break;
                case Direction.East: Direction = Server.Direction.East; break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (_galleon != null)
			{
				_galleon.CheckDecay();
				
				switch ( _galleon.Phase )
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
												
				switch (_galleon.Condition)
				{
					case GalleonCondition.Pristine:
						{
							list.Add(1116580);

							break;
						}
						
					case GalleonCondition.SlightlyDamaged:
						{
							list.Add(1116581);

							break;
						}
						
					case GalleonCondition.ModeratelyDamaged:
						{
							list.Add(1116582);

							break;
						}
						
					case GalleonCondition.HeavilyDamaged:
						{
							list.Add(1116583);

							break;
						}

					case GalleonCondition.ExtremelyDamaged:
						{
							list.Add(1116584);

							break;
						}						
				}    
			}
        }

        public void TillerManSay(int number)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public void TillerManSay(int number, string args)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            if (_galleon != null && _galleon.ShipName != null)
                list.Add(1042884, _galleon.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.AddNameProperties(list);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (_galleon != null && _galleon.ShipName != null)
                from.SendLocalizedMessage(1042884, _galleon.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
			if (_galleon != null && !_galleon.Contains(from))
                _galleon.BeginDryDock(from);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is MapItem && _galleon != null && _galleon.CanCommand(from) && _galleon.Contains(from))
            {
                _galleon.AssociateMap((MapItem)dropped);
            }

            return false;
        }

        public override void OnAfterDelete()
        {
            if (_galleon != null)
                _galleon.Delete();
        }
		
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
			base.GetContextMenuEntries(from, list);
			
			if (_galleon != null && !_galleon.Contains(from))
			{			
				list.Add(new DryDockEntry(from, _galleon));
			}
			else
			{
				list.Add(new SecuritySettingsEntry(from, _galleon));
				list.Add(new RenameEntry(from, _galleon));			
			}
			
			if (_galleon.Durability < 33)
			{
				list.Add(new EmergencyRepairsEntry(from, _galleon));
			}
        }

		public void RefreshTillerMan()
		{
			if (_galleon != null)
				_galleon.NotifyLocationChangeOnSmooth(this, Location);
		}		

		#region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version

            writer.Write(_galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        _galleon = reader.ReadItem() as BaseGalleon;

                        if (_galleon == null)
                            Delete();

                        break;
                    }
            }
        }
		#endregion
		
		private class SecuritySettingsEntry : ContextMenuEntry
		{			
			private readonly Mobile _from;
			private BaseGalleon _galleon;
			
			public SecuritySettingsEntry(Mobile from, BaseGalleon galleon)
				: base(1116567)
			{				
				_from = from;
				_galleon = galleon;
			}

			public override void OnClick()
			{
				if (_from != null)
				{
				
					Dictionary<int, PlayerMobile> PlayersAboard = new Dictionary<int,PlayerMobile>();
					IPooledEnumerable eable = _from.Map.GetClientsInRange(_from.Location, _galleon.GetMaxUpdateRange());
					int i = 0;
					foreach (NetState state in eable)
					{
						Mobile m = state.Mobile;

						if (m is PlayerMobile)						
							if (_galleon.IsOnBoard(m))											
								PlayersAboard.Add(i++,(PlayerMobile)m);																            
					}
					eable.Free();					
				
					_from.SendGump(new SecuritySettingsGump(SecuritySettingsGumpPage.Default, _from, _galleon, PlayersAboard, 1, null));
				}
			}
		}

		private class RenameEntry : ContextMenuEntry
		{			
			private readonly Mobile _from;
			private BaseGalleon _galleon;
			
			public RenameEntry(Mobile from, BaseGalleon galleon)
				: base(1111680)
			{				
				_from = from;
				_galleon = galleon;
			}

			public override void OnClick()
			{
				if ((_from != null) && (_galleon != null))
					_galleon.BeginRename(_from);			
			}
		}	

		private class EmergencyRepairsEntry : ContextMenuEntry
		{			
			private readonly Mobile _from;
			private BaseGalleon _galleon;
			
			public EmergencyRepairsEntry(Mobile from, BaseGalleon galleon)
				: base(1116589)
			{				
				_from = from;
				_galleon = galleon;
			}

			public override void OnClick()
			{
				if ((_from != null) && (_galleon != null))
					_galleon.EmergencyRepairs();			
			}
		}	

		private class DryDockEntry : ContextMenuEntry
		{			
			private readonly Mobile _from;
			private BaseGalleon _galleon;
			
			public DryDockEntry(Mobile from, BaseGalleon boat)
				: base(1116520)
			{				
				_from = from;
				_galleon = boat;
			}

			public override void OnClick()
			{
				if ((_from != null) && (_galleon != null))
					_galleon.BeginDryDock(_from);			
			}
		}		
    }
}