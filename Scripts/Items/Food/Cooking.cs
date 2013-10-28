using System;
using Server.Targeting;

namespace Server.Items
{
    public class UtilityItem
    {
        static public int RandomChoice(int itemID1, int itemID2)
        {
            int iRet = 0;
            switch ( Utility.Random(2) )
            {
                default:
                case 0:
                    iRet = itemID1;
                    break;
                case 1:
                    iRet = itemID2;
                    break;
            }
            return iRet;
        }
    }

    // ********** Dough **********
    public class Dough : Item
    {
        [Constructable]
        public Dough()
            : base(0x103d)
        {
            this.Stackable = Core.ML;
            this.Weight = 1.0;
        }

        public Dough(Serial serial)
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
        }

        #if false
		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			from.Target = new InternalTarget( this );
		}
        #endif

        private class InternalTarget : Target
        {
            private readonly Dough m_Item;

            public InternalTarget(Dough item)
                : base(1, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item.Deleted)
                    return;

                if (targeted is Eggs)
                {
                    this.m_Item.Delete();

                    ((Eggs)targeted).Consume();

                    from.AddToBackpack(new UnbakedQuiche());
                    from.AddToBackpack(new Eggshells());
                }
                else if (targeted is CheeseWheel)
                {
                    this.m_Item.Delete();

                    ((CheeseWheel)targeted).Consume();

                    from.AddToBackpack(new CheesePizza());
                }
                else if (targeted is Sausage)
                {
                    this.m_Item.Delete();

                    ((Sausage)targeted).Consume();

                    from.AddToBackpack(new SausagePizza());
                }
                else if (targeted is Apple)
                {
                    this.m_Item.Delete();

                    ((Apple)targeted).Consume();

                    from.AddToBackpack(new UnbakedApplePie());
                }
                else if (targeted is Peach)
                {
                    this.m_Item.Delete();

                    ((Peach)targeted).Consume();

                    from.AddToBackpack(new UnbakedPeachCobbler());
                }
            }
        }
    }

    // ********** SweetDough **********
    public class SweetDough : Item
    {
        public override int LabelNumber
        {
            get
            {
                return 1041340;
            }
        }// sweet dough

        [Constructable]
        public SweetDough()
            : base(0x103d)
        {
            this.Stackable = Core.ML;
            this.Weight = 1.0;
            this.Hue = 150;
        }

        public SweetDough(Serial serial)
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

            if (this.Hue == 51)
                this.Hue = 150;
        }

        #if false
		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			from.Target = new InternalTarget( this );
		}
        #endif

        private class InternalTarget : Target
        {
            private readonly SweetDough m_Item;

            public InternalTarget(SweetDough item)
                : base(1, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item.Deleted)
                    return;

                if (targeted is BowlFlour)
                {
                    this.m_Item.Delete();
                    ((BowlFlour)targeted).Delete();

                    from.AddToBackpack(new CakeMix());
                }
                else if (targeted is Campfire)
                {
                    from.PlaySound(0x225);
                    this.m_Item.Delete();
                    InternalTimer t = new InternalTimer(from, (Campfire)targeted);
                    t.Start();
                }
            }
			
            private class InternalTimer : Timer
            {
                private readonly Mobile m_From;
                private readonly Campfire m_Campfire;
			
                public InternalTimer(Mobile from, Campfire campfire)
                    : base(TimeSpan.FromSeconds(5.0))
                {
                    this.m_From = from;
                    this.m_Campfire = campfire;
                }

                protected override void OnTick()
                {
                    if (this.m_From.GetDistanceToSqrt(this.m_Campfire) > 3)
                    {
                        this.m_From.SendLocalizedMessage(500686); // You burn the food to a crisp! It's ruined.
                        return;
                    }

                    if (this.m_From.CheckSkill(SkillName.Cooking, 0, 10))
                    {
                        if (this.m_From.AddToBackpack(new Muffins()))
                            this.m_From.PlaySound(0x57);
                    }
                    else
                    {
                        this.m_From.SendLocalizedMessage(500686); // You burn the food to a crisp! It's ruined.
                    }
                }
            }
        }
    }

    // ********** JarHoney **********
    public class JarHoney : Item
    {
        [Constructable]
        public JarHoney()
            : base(0x9ec)
        {
            this.Weight = 1.0;
            this.Stackable = true;
        }

        public JarHoney(Serial serial)
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
            this.Stackable = true;
        }

        /*public override void OnDoubleClick( Mobile from )
        {
        if ( !Movable )
        return;

        from.Target = new InternalTarget( this );
        }*/

        private class InternalTarget : Target
        {
            private readonly JarHoney m_Item;

            public InternalTarget(JarHoney item)
                : base(1, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item.Deleted)
                    return;

                if (targeted is Dough)
                {
                    this.m_Item.Delete();
                    ((Dough)targeted).Consume();

                    from.AddToBackpack(new SweetDough());
                }
				
                if (targeted is BowlFlour)
                {
                    this.m_Item.Consume();
                    ((BowlFlour)targeted).Delete();

                    from.AddToBackpack(new CookieMix());
                }
            }
        }
    }

    // ********** BowlFlour **********
    public class BowlFlour : Item
    {
        [Constructable]
        public BowlFlour()
            : base(0xa1e)
        {
            this.Weight = 1.0;
        }

        public BowlFlour(Serial serial)
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
        }
    }

    // ********** WoodenBowl **********
    public class WoodenBowl : Item
    {
        [Constructable]
        public WoodenBowl()
            : base(0x15f8)
        {
            this.Weight = 1.0;
        }

        public WoodenBowl(Serial serial)
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
        }
    }

    // ********** PitcherWater **********
    /*public class PitcherWater : Item
    {
    [Constructable]
    public PitcherWater() : base(Utility.Random( 0x1f9d, 2 ))
    {
    Weight = 1.0;
    }

    public PitcherWater( Serial serial ) : base( serial )
    {
    }

    public override void Serialize( GenericWriter writer )
    {
    base.Serialize( writer );

    writer.Write( (int) 0 ); // version
    }

    public override void Deserialize( GenericReader reader )
    {
    base.Deserialize( reader );

    int version = reader.ReadInt();
    }

    public override void OnDoubleClick( Mobile from )
    {
    if ( !Movable )
    return;

    from.Target = new InternalTarget( this );
    }

    private class InternalTarget : Target
    {
    private PitcherWater m_Item;

    public InternalTarget( PitcherWater item ) : base( 1, false, TargetFlags.None )
    {
    m_Item = item;
    }

    protected override void OnTarget( Mobile from, object targeted )
    {
    if ( m_Item.Deleted ) return;

    if ( targeted is BowlFlour )
    {
    m_Item.Delete();
    ((BowlFlour)targeted).Delete();

    from.AddToBackpack( new Dough() );
    from.AddToBackpack( new WoodenBowl() );
    }
    }
    }
    }*/

    // ********** SackFlour **********
    [TypeAlias("Server.Items.SackFlourOpen")]
    public class SackFlour : Item, IHasQuantity
    {
        private int m_Quantity;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Quantity
        {
            get
            {
                return this.m_Quantity;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 20)
                    value = 20;

                this.m_Quantity = value;

                if (this.m_Quantity == 0)
                    this.Delete();
                else if (this.m_Quantity < 20 && (this.ItemID == 0x1039 || this.ItemID == 0x1045))
                    ++this.ItemID;
            }
        }

        [Constructable]
        public SackFlour()
            : base(0x1039)
        {
            this.Weight = 5.0;
            this.m_Quantity = 20;
        }

        public SackFlour(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            writer.Write((int)this.m_Quantity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                case 1:
                    {
                        this.m_Quantity = reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        this.m_Quantity = 20;
                        break;
                    }
            }

            if (version < 2 && this.Weight == 1.0)
                this.Weight = 5.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.Movable)
                return;

            if ((this.ItemID == 0x1039 || this.ItemID == 0x1045))
                ++this.ItemID;
            #if false
			this.Delete();
			from.AddToBackpack( new SackFlourOpen() );
            #endif
        }
    }

    #if false
	// ********** SackFlourOpen **********
	public class SackFlourOpen : Item
	{
		public override int LabelNumber{ get{ return 1024166; } } // open sack of flour

		[Constructable]
		public SackFlourOpen() : base(UtilityItem.RandomChoice( 0x1046, 0x103a ))
		{
			Weight = 1.0;
		}

		public SackFlourOpen( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			from.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private SackFlourOpen m_Item;

			public InternalTarget( SackFlourOpen item ) : base( 1, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted ) return;

				if ( targeted is WoodenBowl )
				{
					m_Item.Delete();
					((WoodenBowl)targeted).Delete();

					from.AddToBackpack( new BowlFlour() );
				}
				else if ( targeted is TribalBerry )
				{
					if ( from.Skills[SkillName.Cooking].Base >= 80.0 )
					{
						m_Item.Delete();
						((TribalBerry)targeted).Delete();

						from.AddToBackpack( new TribalPaint() );

						from.SendLocalizedMessage( 1042002 ); // You combine the berry and the flour into the tribal paint worn by the savages.
					}
					else
					{
						from.SendLocalizedMessage( 1042003 ); // You don't have the cooking skill to create the body paint.
					}
				}
			}
		}
	}
    #endif

    // ********** Eggshells **********
    public class Eggshells : Item
    {
        [Constructable]
        public Eggshells()
            : base(0x9b4)
        {
            this.Weight = 0.5;
        }

        public Eggshells(Serial serial)
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
        }
    }

    public class WheatSheaf : Item
    {
        [Constructable]
        public WheatSheaf()
            : this(1)
        {
        }

        [Constructable]
        public WheatSheaf(int amount)
            : base(7869)
        {
            this.Weight = 1.0;
            this.Stackable = true;
            this.Amount = amount;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.Movable)
                return;

            from.BeginTarget(4, false, TargetFlags.None, new TargetCallback(OnTarget));
        }

        public virtual void OnTarget(Mobile from, object obj)
        {
            if (obj is AddonComponent)
                obj = (obj as AddonComponent).Addon;

            IFlourMill mill = obj as IFlourMill;

            if (mill != null)
            {
                int needs = mill.MaxFlour - mill.CurFlour;

                if (needs > this.Amount)
                    needs = this.Amount;

                mill.CurFlour += needs;
                this.Consume(needs);
            }
        }

        public WheatSheaf(Serial serial)
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
        }
    }
}