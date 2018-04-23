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


namespace Server.Items
{

    public class WaterSprinkler : Item, ISecurable
    {
		private int m_UsesRemaining;
		private int m_StorageLimit;
        private SecureLevel m_Level;

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
		public WaterSprinkler() : base( 0xE7A )
        {
            Movable = true;
            Weight = 1.0;
            Name = "a water sprinkler";
            Hue = 291;
            StorageLimit = 1000;
        }

        [Constructable]
        public WaterSprinkler(int uses, int storageLimit)
            : base(0xE7A)
        {
            Weight = 1.0;
            Name = "a watering sprinkler";
            Hue = 291;
			m_UsesRemaining = uses;
			StorageLimit = storageLimit;
        }

		public override void AddNameProperty(ObjectPropertyList list)
        {
			base.AddNameProperty(list);

            list.Add("Water Remaining: {0}", this.m_UsesRemaining.ToString()); // potions remaining: ~1_val~
        }

        public WaterSprinkler(Serial serial)
            : base(serial)
        {
        }

        public bool CanBeWatered(PlantItem plant)
        {
            return plant.PlantStatus < PlantStatus.DecorativePlant && plant.PlantSystem.Water <= 1;
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
                        from.SendMessage("You must fill this before you can use it!");
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
                                if (CanBeWatered(plant))
                                {
									if (plant.PlantSystem.Water == 1 && m_UsesRemaining > 0)
									{
                                    plant.PlantSystem.Water = 2;
                                    found = true;
									m_UsesRemaining--;
									}
									if (plant.PlantSystem.Water == 0 && m_UsesRemaining > 1)
									{
									plant.PlantSystem.Water = 2;
                                    found = true;
									m_UsesRemaining-=2;
									}
                                }
                            }
                        }
                        if (found)
                        {
                            from.SendMessage("Your dry plants have been watered.");
                            from.PlaySound(0x12);
                        }
                        else
                            from.SendMessage("You have no plants that need watering!");
                    }
                }
                else
                    from.SendMessage("You may not access this!");
            }
        }

		public override bool OnDragDrop(Mobile from, Item item)
        {
          
				if ( item is SprinklerContainer )
				{
					int freespace = m_StorageLimit - m_UsesRemaining;
					SprinklerContainer keg = (SprinklerContainer)item;	
					 if ( keg.Quantity <= freespace )
					{
						m_UsesRemaining+=keg.Quantity;
                        from.PlaySound(0x240);
                        from.SendMessage("You place the empty tub in your backpack."); // You place the empty bottle in your backpack.
						Container pack = from.Backpack;
						pack.DropItem( new SprinklerContainer() );
                        return true;
						}
						from.SendMessage("This is not from the water tub!"); // You don't have room for the empty bottle in your backpack.
                        return false;
                    }

                        from.SendMessage("You can't fit all those water in the sprinkler!"); // You don't have room for the empty bottle in your backpack.
                        return false;

                }

        public void Pour(Mobile from, Item item)
        {
            if (item is BaseBeverage)
            {
                BaseBeverage beverage = (BaseBeverage)item;
                if (beverage.IsEmpty || !beverage.Pourable || beverage.Content != BeverageType.Water)
                {
                    from.SendMessage("You can only put water in here!");
                    return;
                }

                if (m_UsesRemaining < m_StorageLimit)
                {
                    m_UsesRemaining++;
                    beverage.Quantity--;
                    from.PlaySound(0x4E);

                    if (m_UsesRemaining == m_StorageLimit)
                        from.SendMessage("You completely fill the sprinkler.");
                    else
                        from.SendMessage("You dump some water into the sprinkler");
                }
                else
                    from.SendMessage("It's already full.");


            }
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


    public class SprinklerContainer : BaseBeverage
    {
        public override int MaxQuantity { get { return 100; } }
        public override int ComputeItemID() { return 0x0E7B; }

        [Constructable]
        public SprinklerContainer()
        {
            Weight = 5.0;
            Name = "a water tub";
        }

        [Constructable]
        public SprinklerContainer(BeverageType type)
            : base(type)
        {
            Weight = 5.0;
        }

        public SprinklerContainer(Serial serial)
            : base(serial)
        {

        }


        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            else if (Quantity > 0)
            {
                from.SendMessage("Select the sprinkler you wish to fill");
                from.Target = new SprinklerTarget(this);
            }
            else
            {
                from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Fill_OnTarget));
                SendLocalizedMessageTo(from, 500837); // Fill from what?
            }

        }

        private class SprinklerTarget : Target
        {
            private SprinklerContainer m_cont;

            public SprinklerTarget(SprinklerContainer cont)
                : base(1, false, TargetFlags.None)
            {
                m_cont = cont;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (target is WaterSprinkler)
                {
                    WaterSprinkler sprink = (WaterSprinkler)target;
                    sprink.Pour(from, m_cont);
                }
                else
                    from.SendMessage("This can only be used on sprinklers!");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            writer.Write((int)Quantity);
            writer.Write((int)Content);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Quantity = reader.ReadInt();
            Content = (BeverageType)reader.ReadInt();

        }
    }


}
