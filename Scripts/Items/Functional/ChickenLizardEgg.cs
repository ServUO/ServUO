using System;
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public enum EggStage
	{
		New,
		Stage1,
		Stage2,
		Mature,
        Burnt
	}
	
	public enum Dryness
	{
		Moist,
		Dry,
		Parched,
		Dehydrated
	}
	
    public class ChickenLizardEgg : Item
    {
        public virtual bool CanMutate { get { return true; } }

		private DateTime m_IncubationStart;
		private TimeSpan m_TotalIncubationTime;
		private bool m_Incubating;
		private EggStage m_Stage;
		private int m_WaterLevel;
		private bool m_IsBattleChicken;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime IncubationStart 
		{ 
			get 
			{ 
				return m_IncubationStart; 
			} 
			set
			{
				m_IncubationStart = value;
			}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan TotalIncubationTime 
		{ 
			get 
			{ 
				return m_TotalIncubationTime; 
			} 
			set
			{
				m_TotalIncubationTime = value;
				m_IncubationStart = DateTime.UtcNow;
				InvalidateProperties();
			}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Incubating 
        { 
            get { return m_Incubating; }
            set
            {
                if (m_Incubating && !value)
                {
                    if (m_IncubationStart < DateTime.UtcNow)
                        TotalIncubationTime += DateTime.UtcNow - m_IncubationStart;
                }

                m_Incubating = value;
            }
        }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public EggStage Stage { get { return m_Stage; } set { m_Stage = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Dryness Dryness 
		{
			get
			{
				int v = (int)m_Stage - m_WaterLevel;
				
				if(v >= 2 && m_WaterLevel == 0)
					return Dryness.Dehydrated;
				if(v >= 2)
					return Dryness.Parched;
				if(v >= 1)
					return Dryness.Dry;
					
				return Dryness.Moist;
			}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsBattleChicken
		{
			get { return m_IsBattleChicken; }
			set { m_IsBattleChicken = value; }
		}
		
		[Constructable]
        public ChickenLizardEgg() : base(0x41BD)
        {
			m_Incubating = false;
			m_TotalIncubationTime = TimeSpan.Zero;
			m_Stage = EggStage.New;
        }
		
		public override int LabelNumber
		{
            get
            {
                int c = 1112468;

                if (m_Stage == EggStage.Mature)
                    c = m_IsBattleChicken ? 1112468 : 1112467;
                else if (m_Stage == EggStage.Burnt)
                    c = 1112466;
                else
                {
                    switch (Dryness)
                    {
                        case Dryness.Moist: c = 1112462; break;
                        case Dryness.Dry: c = 1112463; break;
                        case Dryness.Parched: c = 1112464; break;
                        case Dryness.Dehydrated: c = 1112465; break;
                    }
                }

                return c;
            }
		}

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            bool check = base.DropToMobile(from, target, p);

            if (check && m_Incubating)
                Incubating = false;

            return check;
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool check = base.DropToWorld(from, p);

            if (check && m_Incubating)
                Incubating = false;

            return check;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            bool check = base.DropToItem(from, target, p);

            if (check && !(Parent is Incubator) && m_Incubating)
                Incubating = false;

            return check;
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            if (m_Incubating)
                Incubating = false;

            base.OnItemLifted(from, item);
        }

		public void CheckStatus()
		{
            if (m_Stage == EggStage.Burnt)
                return;

            if(m_Incubating && m_IncubationStart < DateTime.UtcNow)
			    TotalIncubationTime += DateTime.UtcNow - m_IncubationStart;
				
			if(m_TotalIncubationTime > TimeSpan.FromHours(24) && m_Stage == EggStage.New)           //from new to stage 1
			{
                IncreaseStage();
                //Nothing, egg goes to stage 2 regardless if its watered or not
			}
			else if (m_TotalIncubationTime >= TimeSpan.FromHours(48) && m_Stage == EggStage.Stage1)  //from stage 1 to stage 2
			{
				if(Dryness >= Dryness.Parched)
				{
					if(Utility.RandomBool())
						BurnEgg();
				}

                IncreaseStage();
			}
			else if (m_TotalIncubationTime >= TimeSpan.FromHours(72) && m_Stage == EggStage.Stage2)  //from stage 2 to mature egg 
			{
				if(Dryness >= Dryness.Parched)
				{
					if(.25 < Utility.RandomDouble())
						BurnEgg();
				}

                IncreaseStage();
			}
            else if (m_TotalIncubationTime >= TimeSpan.FromHours(120) && m_Stage == EggStage.Mature)
            {
                BurnEgg();
                IncreaseStage();
            }
		}

		public void Pour(Mobile from, BaseBeverage bev)
		{
            if (!bev.IsEmpty && bev.Pourable && bev.Content == BeverageType.Water && bev.ValidateUse(from, false))
            {
                if (m_Stage == EggStage.Burnt)
                    from.SendMessage("You decide not to water the burnt egg.");
                else if (m_WaterLevel < (int)m_Stage)
                {
                    bev.Quantity--;

                    m_WaterLevel++;
                    from.PlaySound(0x4E);

                    InvalidateProperties();
                }
                else
                    from.SendMessage("You decide not to water the egg since it doesn't need it.");
            }
		}
		
		public void IncreaseStage()
		{
            if (m_Stage != EggStage.Burnt)
                m_Stage++;
			
			switch(m_Stage)
			{
				default:
				case EggStage.New:
				case EggStage.Stage1:
					ItemID = 0x41BE;
					break;
				case EggStage.Stage2:
                    ItemID = 0x41BF;
                    break;
                case EggStage.Mature:
                    {
                        ItemID = 0x41BF;

                        Hue = 555;

                        double chance = .10;
                        if (Dryness == Dryness.Dry)
                            chance = .05;
                        else if (Dryness == Dryness.Parched)
                            chance = .01;
                        else if (Dryness == Dryness.Dehydrated)
                            chance = 0;
                       

                        if (CanMutate && chance >= Utility.RandomDouble())
                        {
                            m_IsBattleChicken = true;
                            Hue = GetRandomHiryuHue();
                        }
                        else
                            Hue = 555;

                        break;
                    }
				case EggStage.Burnt:
					ItemID = 0x41BF;
                    Hue = 2026;
					break;
			}

            InvalidateProperties();
		}

        private int GetRandomHiryuHue()
        {
            switch (Utility.Random(12))
            {
                case 0: return 1173;  //Cyan
                case 1: return 1160;  //Strong Cyan
                case 2: return 675;   //Light Green
                case 3: return 72;    //Strong Green
                case 4: return 2213;  //Gold
                case 5: return 1463;   //Strong Yellow
                case 6: return 2425;  //Agapite
                case 7: return 26;    //Strong Purple
                case 8: return 1151;  //Ice Green
                case 9: return 1152;  //Ice Blue
                case 10: return 101;  //Light Blue
                case 11: return 1159; //yellow blue
            }

            return 0;
        }

        public void BurnEgg()
        {
            m_Stage = EggStage.Burnt;
        }
		
		public override void OnDoubleClick(Mobile from)
		{
            if (IsChildOf(from.Backpack))
            {
                if(m_Stage == EggStage.Mature)
                    from.SendGump(new ConfirmHatchGump1(from, this));
                else
                    from.SendGump(new ConfirmHatchGump2(from, this));
            }
		}
		
		public void TryHatchEgg(Mobile from)
		{
			if(m_Stage == EggStage.Mature)
				OnHatch(from);
			else
				CrumbleEgg(from);
		}
		
		public virtual void OnHatch(Mobile from)
		{
			BaseCreature bc;

            if (m_IsBattleChicken)
            {
                from.SendLocalizedMessage(1112478); //You hatch a battle chicken lizard!!
                bc = new BattleChickenLizard();
                bc.Hue = this.Hue;
            }
            else
            {
                from.SendLocalizedMessage(1112477); //You hatch a chicken lizard.
                bc = new ChickenLizard();
            }
				
			bc.MoveToWorld(from.Location, from.Map);
            Delete();
		}
		
		public void CrumbleEgg(Mobile from)
		{
            from.SendLocalizedMessage(1112447); //You hatch the egg but it crumbles in your hands!
            Delete();
		}
		
		private class ConfirmHatchGump1 : BaseConfirmGump
		{
			private ChickenLizardEgg m_Egg;
			private Mobile m_From;

            public override int TitleNumber { get { return 1112444; } }
            public override int LabelNumber { get { return 1112446; } }
			
			public ConfirmHatchGump1(Mobile from, ChickenLizardEgg egg)
			{
				m_Egg = egg;
				m_From = from;
			}
			
			public override void Confirm( Mobile from )
			{
				if(m_Egg != null)
					m_Egg.TryHatchEgg(from);
			}
		}

        private class ConfirmHatchGump2 : BaseConfirmGump
        {
            private ChickenLizardEgg m_Egg;
            private Mobile m_From;

            public override int TitleNumber { get { return 1112444; } }
            public override int LabelNumber { get { return 1112445; } }

            public ConfirmHatchGump2(Mobile from, ChickenLizardEgg egg)
            {
                m_Egg = egg;
                m_From = from;
            }

            public override void Confirm(Mobile from)
            {
                if (m_Egg != null)
                    m_Egg.TryHatchEgg(from);
            }
        }
		
        public ChickenLizardEgg(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_IncubationStart);
            writer.Write(m_TotalIncubationTime);
            writer.Write(m_Incubating);
            writer.Write((int)m_Stage);
            writer.Write(m_WaterLevel);
            writer.Write(m_IsBattleChicken);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_IncubationStart = reader.ReadDateTime();
            m_TotalIncubationTime = reader.ReadTimeSpan();
            m_Incubating = reader.ReadBool();
            m_Stage = (EggStage)reader.ReadInt();
            m_WaterLevel = reader.ReadInt();
            m_IsBattleChicken = reader.ReadBool();
        }
    }
}
