using Server;
using System;
using Server.Mobiles;
using Server.Engines.HuntsmasterChallenge;

namespace Server.Items
{
	public class HarvestersBlade : ElvenSpellblade
	{
		public override int LabelNumber { get { return 1114096; } } // Harvester's Blade
	
		[Constructable]
		public HarvestersBlade()
		{
			Attributes.SpellChanneling = 1;
		}
	
		public HarvestersBlade(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class RangersGuildSash : BodySash
	{
		public override int LabelNumber { get { return 1155744; } } // Member of the Skara Brae Ranger's Guild
	
		[Constructable]
		public RangersGuildSash()
		{
            LootType = LootType.Blessed;
		}
	
		public RangersGuildSash(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

        public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

    public class HuntmastersRewardTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return new TextDefinition(1155727); } } // Huntmaster's Champion

        [Constructable]
        public HuntmastersRewardTitleDeed()
        {
        }

        public HuntmastersRewardTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    [Flipable(18080, 18081)]
    public class HornOfPlenty : Item, IUsesRemaining
    {
        public override int LabelNumber { get { return 1153503; } } // Horn of Plenty

        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; if (m_UsesRemaining <= 0) Delete(); else InvalidateProperties(); }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }

        [Constructable]
        public HornOfPlenty() : base(18080)
        {
            UsesRemaining = 10;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_UsesRemaining > 0)
            {
                Item item = null;

                switch (Utility.Random(6))
                {
                    case 0: item = new SweetPotatoPie(); break;
                    case 1: item = new MashedSweetPotatoes(); break;
                    case 2: item = new BasketOfRolls(); break;
                    case 3: item = new TurkeyPlatter(); break;
                    case 4:
                        BaseCreature bc = new Turkey(true);
                        if (0.10 > Utility.RandomDouble())
                            bc.Name = "Mister Gobbles";
                        bc.MoveToWorld(from.Location, from.Map);
                        from.SendLocalizedMessage(1153512); //That one's not cooked!
                        break;
                    case 5:
                        new InternalTimer(from);
                        from.Frozen = true;
                        break;
                }

                if (item != null)
                {
                    if (from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
                        item.MoveToWorld(from.Location, from.Map);

                    UsesRemaining--;
                }
            }
        }

        private class InternalTimer : Timer
        {
            private Mobile m_From;
            private int m_Ticks;

            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_From = from;
                Start();
            }

            protected override void OnTick()
            {
                m_Ticks++;

                if (m_Ticks % 3 == 0)
                    m_From.Say(1153513); // * ZzzzZzzzZzzzZ *

                if (m_Ticks == 10)
                {
                    this.Stop();
                    m_From.Frozen = false;
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if(ShowUsesRemaining)
                list.Add(1049116, m_UsesRemaining.ToString()); // [ Charges: ~1_CHARGES~ ]
        }

        public HornOfPlenty(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
            m_UsesRemaining = reader.ReadInt();
        }
    }

	/*public class HuntmastersChampionshipDeed : Item
	{
		public override int LabelNumber { get { return 1155727; } } // Huntmaster's Champion
	
		private HuntingKillEntry m_Entry;
	
		[Constructable]
        public HuntmastersChampionshipDeed(HuntingKillEntry entry) : base(5360)
		{
			m_Entry = entry;
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			
			if(m_Entry.KillIndex >= 0 && m_Entry.KillIndex < HuntingTrophyInfo.Infos.Count)
			{
				HuntingTrophyInfo info = HuntingTrophyInfo.Infos[m_Entry.KillIndex];	
				
				if(info != null)
				{
					list.Add(1155708, m_Entry.Owner != null ? m_Entry.Owner.Name : "Unknown"); // Hunter: ~1_NAME~	
					list.Add(1155709, m_Entry.DateKilled.ToShortDateString()); // Date of Kill: ~1_DATE~
					
					if(m_Entry.Location != null)
						list.Add(1061114, m_Entry.Location); // Location: ~1_val~
						
                    list.Add(1155718, info.Species.ToString());
						
					if(info.MeasuredBy == MeasuredBy.Length)
						list.Add(1155711, m_Entry.Measurement.ToString()); // Length: ~1_VAL~
					else if (info.MeasuredBy == MeasuredBy.Wingspan)
						list.Add(1155710, m_Entry.Measurement.ToString());	// Wingspan: ~1_VAL~
					else
						list.Add(1072789, m_Entry.Measurement.ToString()); // Weight: ~1_WEIGHT~
				}
			}
		}
	
		public HuntmastersChampionshipDeed(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			if(m_Entry != null)
			{
				writer.Write((int)1);
				m_Entry.Serialize(writer);
			}
			else
				writer.Write((int)0);
		}

        public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			
			if(reader.ReadInt() == 1)
				m_Entry =  new HuntingKillEntry(reader);
		}
    }*/
}