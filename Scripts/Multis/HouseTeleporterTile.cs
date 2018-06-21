using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Multis
{
	public class HouseTeleporterTile : HouseTeleporter
	{
        public static int MaxCharges = 1000;

		private int _Charges;

		[CommandProperty(AccessLevel.GameMaster)]
        public HouseTeleporterTile Link
		{
			get
			{
				if(Target != null && Target.Deleted)
					Target = null;

                return Target as HouseTeleporterTile;
			}
			set
			{
				Target = value;
			}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int Charges
		{
			get { return _Charges; }
			set 
            {
                _Charges = value;

                if (UsesCharges)
                {
                    if (_Charges == 0)
                    {
                        Hue = 1208;
                    }
                    else if (_Charges >= 1 && Hue != 1201)
                    {
                        Hue = 1201;
                    }
                }

                InvalidateProperties();
            } 
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UsesCharges { get; set; }

        public override int LabelNumber { get { return Link == null ? 1114916 : 1113917; } } // house teleporter (unlinked) -or- House Teleporter

        public HouseTeleporterTile(bool vetReward)
            : base(vetReward ? 0x40BB : 0x40B9, null)
        {
            UsesCharges = !vetReward;
            Movable = true;
            Weight = 2.0;
            LootType = LootType.Blessed;

            if (vetReward)
            {
                UsesCharges = false;
            }
            else
            {
                UsesCharges = true;
                Charges = MaxCharges;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (UsesCharges)
            {
                list.Add(1060741, _Charges.ToString()); // charges: ~1_val~
            }
        }

        public bool CheckBaseAccess(Mobile m)
        {
            return base.CheckAccess(m);
        }

		public override bool CheckAccess(Mobile m)
		{
			BaseHouse house = BaseHouse.FindHouseAt(this);
            BaseHouse linkHouse = Link == null ? null : BaseHouse.FindHouseAt(Link);

			if(house == null || Link == null || !IsLockedDown || !Link.IsLockedDown || linkHouse == null) // TODO: Messages for these?
			{
				return false;
			}

            if (UsesCharges && _Charges == 0)
            {
                m.SendLocalizedMessage(1115121); // There are no charges left in this teleporter.
                return false;
            }

            if (UsesCharges && Link.Charges == 0)
            {
                m.SendLocalizedMessage(1115120); // There are no more charges left in the remote teleporter.
                return false;
            }

            if (CheckBaseAccess(m) && Link.CheckBaseAccess(m))
            {
                return CheckTravel(m, Link.Location, Link.Map);
            }

			return false;
		}

        public bool CheckTravel(Mobile from, Point3D dest, Map destMap)
        {

            if (Factions.Sigil.ExistsOn(from))
            {
                from.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }
            else if (from.Criminal)
            {
                from.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (SpellHelper.CheckCombat(from))
            {
                from.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if (destMap == Map.Felucca && from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendLocalizedMessage(1049543); // You decide against traveling to Felucca while you are still young.
                return false;
            }
            else if (from.Murderer && destMap.Rules != MapRules.FeluccaRules && !Siege.SiegeShard)
            {
                from.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                return false;
            }
            else if (Server.Engines.CityLoyalty.CityTradeSystem.HasTrade(from))
            {
                from.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
                return false;
            }
            else if (from.Holding != null)
            {
                from.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                return false;
            }
            else if (from.Target != null)
            {
                from.SendLocalizedMessage(500310); // You are too busy with something else.
                return false;
            }

            return true;
        }
		
		public override void OnDoubleClick(Mobile m)
		{
			if(IsChildOf(m.Backpack))
			{
				m.SendLocalizedMessage(1114918); // Select a House Teleporter to link to.
				m.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, (from, targeted) =>
				{
					if(targeted is HouseTeleporterTile)
					{
						var tile = targeted as HouseTeleporterTile;
						
						if(tile.IsChildOf(m.Backpack))
						{
							tile.Link = this;
							Link = tile;

                            if (UsesCharges && tile.UsesCharges) //TODO:  Can you link non-charged with charged?
                            {
                                from.SendLocalizedMessage(1115119); // The two House Teleporters are now linked and the charges remaining have been rebalanced.

                                if (!UsesCharges)
                                    UsesCharges = true;

                                if (!tile.UsesCharges)
                                    tile.UsesCharges = true;

                                int charges = _Charges + tile.Charges;
                                Charges = charges / 2;
                                tile.Charges = charges / 2;
                            }
                            else if (!UsesCharges && !tile.UsesCharges)
                            {
                                from.SendLocalizedMessage(1114919); // The two House Teleporters are now linked.
                            }
                            else
                            {
                                from.SendMessage("Those cannot be linked."); // TODO: Message?
                            }
						}
						else
						{
							from.SendLocalizedMessage(1114917); // This must be in your backpack to link it.
						}
					}
				});
			}
			else
			{
				m.SendLocalizedMessage(1114917); // This must be in your backpack to link it.
			}
		}

        public override void OnAfterTeleport(Mobile m)
        {
            if (UsesCharges)
            {
                Charges = Math.Max(0, _Charges - 1);

                if (Link != null)
                {
                    Link.Charges = Math.Max(0, Link.Charges - 1);

                    if (!Link.UsesCharges)
                    {
                        Link.UsesCharges = true;
                    }
                }
            }
        }
		
		public HouseTeleporterTile(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write(_Charges);
            writer.Write(UsesCharges);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			_Charges = reader.ReadInt();
            UsesCharges = reader.ReadBool();
		}
	}

    public class HouseTeleporterTileBag : Bag
    {
        public override int LabelNumber { get { return 1113917; } }

        [Constructable]
        public HouseTeleporterTileBag()
            : this(false)
        {
        }

        [Constructable]
        public HouseTeleporterTileBag(bool reward)
        {
            Hue = 1336;

            var tele1 = new HouseTeleporterTile(reward);
            var tele2 = new HouseTeleporterTile(reward);

            tele1.Link = tele2;
            tele2.Link = tele1;

            DropItem(tele1);
            DropItem(tele2);
            DropItem(new HouseTeleporterInstructions(reward));
        }

        public HouseTeleporterTileBag(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
    }

    public class HouseTeleporterInstructions : Item
    {
        public override int LabelNumber { get { return 1115122; } } // Care Instructions

        public bool VetReward { get; set; }

        public HouseTeleporterInstructions(bool reward)
            : base(0xFF4)
        {
            VetReward = reward;
            Hue = 195;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1115123); // Congratulations on becoming the<br> owner of your very own house<br> teleporter set!
            list.Add(1115124); // To use them, lock one down in your<br> home then lock the other down in<br> the home of a trusted friend.

            if (!VetReward)
            {
                list.Add(1115125); // Drop Gate Travel scrolls onto these<br> to recharge them.
            }
        }

        public HouseTeleporterInstructions(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
            writer.Write(VetReward);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
            VetReward = reader.ReadBool();
		}
    }
}
