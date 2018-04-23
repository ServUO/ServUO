// 	RunUO 2.0 SVN Version	
using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Multis;
using Server.Engines.Plants;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;



namespace Server.Items
{

    public class CureSprinkler : Item, ISecurable, IUsesRemaining
    {
        private SecureLevel m_Level;
		private int m_UsesRemaining;
		private int m_StorageLimit;

		[CommandProperty(AccessLevel.GameMaster)]
        public int StorageLimit { get { return m_StorageLimit; } set { m_StorageLimit = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

		[CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }

        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        [Constructable]
		public CureSprinkler() : base( 0xE7A )
        {
            Movable = true;
            Weight = 1.0;
            Name = "a cure sprinkler";
            Hue = 43;
            StorageLimit = 1000;
        }
        public CureSprinkler(int uses, int storageLimit)
            : base(0xE7A)
        {
            Weight = 1.0;
            Name = "a cure sprinkler";
            Hue = 43;
			m_UsesRemaining = uses;
			StorageLimit = storageLimit;
        }

		public override void AddNameProperty(ObjectPropertyList list)
        {
			base.AddNameProperty(list);

            list.Add("Potions Remaining: {0}", this.m_UsesRemaining.ToString()); // potions remaining: ~1_val~
        }

        public CureSprinkler(Serial serial)
            : base(serial)
        {
        }

		public bool CanBeCured( PlantItem plant )
		{
			PlantSystem sys = plant.PlantSystem;

			if ( plant.PlantStatus >= PlantStatus.DecorativePlant )
				return false;
			else if( sys.Fungus == 0 )
				return false;

			return true;
		}

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);
            if (house == null)
                from.SendLocalizedMessage(1005525);//That is not in your house
            else if (this.Movable)
                from.SendMessage("This must be locked down to use!");
            else
            {
                if (this.IsAccessibleTo(from))
                {
                    if (m_UsesRemaining < 1)
                        from.SendMessage("You must add cure potions to this before you can use it!");
                    else
                    {
                        Point3D p = new Point3D(this.Location);
                        Map map = this.Map;
                        IPooledEnumerable eable = map.GetItemsInRange(p, 18);
                        bool found = false;


                        foreach (Item item in eable)
                        {
                            if (house.IsInside(item) && item is PlantItem && item.IsLockedDown)
                            {
                                PlantItem plant = (PlantItem)item;
                                if (CanBeCured(plant))
                                {
									if (plant.PlantSystem.Fungus == 1 && plant.PlantSystem.CurePotion == 0 && m_UsesRemaining > 0)
									{
                                    plant.PlantSystem.CurePotion = 1;
                                    found = true;
									m_UsesRemaining--;
									}
									if (plant.PlantSystem.Fungus == 2 && plant.PlantSystem.CurePotion == 0 && m_UsesRemaining > 1)
									{
									plant.PlantSystem.CurePotion = 2;
                                    found = true;
									m_UsesRemaining-=2;
									}
									if (plant.PlantSystem.Fungus == 2 && plant.PlantSystem.CurePotion == 1 && m_UsesRemaining > 0)
									{
									plant.PlantSystem.CurePotion = 2;
                                    found = true;
									m_UsesRemaining--;
									}
                                }
                            }
                        }
                        if (found)
                        {
                            from.SendMessage("The fungus on your plants has been treated.");
                            from.PlaySound(0x12);
                        }
                        else
                            from.SendMessage("You have no plants that need curing!");
                    }
                }
                else
                    from.SendMessage("You may not access this!");
            }
        }

		public override bool OnDragDrop(Mobile from, Item item)
        {
            if (item is GreaterCurePotion && m_UsesRemaining < m_StorageLimit)
            {
						m_UsesRemaining++;
                        from.PlaySound(0x240);
                        from.SendLocalizedMessage(502237); // You place the empty bottle in your backpack.
						Container pack = from.Backpack;
						pack.DropItem( new Bottle() );
                        return true;
                    }

				if ( item is PotionKeg )
				{
					int freespace = m_StorageLimit - m_UsesRemaining;
					PotionKeg keg = (PotionKeg)item;	
					 if (keg.Type == PotionEffect.CureGreater && keg.Held <= freespace )
					{
						m_UsesRemaining+=keg.Held;
                        from.PlaySound(0x240);
                        from.SendMessage("You place the empty keg in your backpack."); // You place the empty bottle in your backpack.
						Container pack = from.Backpack;
						pack.DropItem( new PotionKeg() );
                        return true;
						}
						from.SendMessage("This is not the right kind of Potion!"); // You don't have room for the empty bottle in your backpack.
                        return false;
                    }

                        from.SendMessage("You can't fit all those potions in the sprinkler!"); // You don't have room for the empty bottle in your backpack.
                        return false;

                }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_Level);
			writer.Write((int)this.m_UsesRemaining);
			writer.Write((int)this.m_StorageLimit);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Level = (SecureLevel)reader.ReadInt();
			this.m_UsesRemaining = reader.ReadInt();
			this.m_StorageLimit = reader.ReadInt();
        }

    }

}
