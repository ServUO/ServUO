using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting;

namespace Server.Mobiles
{
	[CorpseName( "a chicken corpse" )]
	public class FightingChicken : BaseCreature
	{
		private Mobile m_Owner;
		private CockFightingControlStone m_Controller;
	
		[CommandProperty( AccessLevel.Owner )]
		public Mobile Owner{ get{ return m_Owner; } set { m_Owner = value; InvalidateProperties(); } }
		
		[CommandProperty( AccessLevel.Owner )]
		public CockFightingControlStone Controller{ get{ return m_Controller; } set { m_Controller = value; InvalidateProperties(); } }
		
		[Constructable]
		public FightingChicken( Mobile from, CockFightingControlStone controller ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			this.Name = "a fighting chicken";
            this.Body = 0xD0;
            this.BaseSoundID = 0x6E;

            this.m_Owner = from;
            this.m_Controller = controller;

            this.SetStr(20, 40);
            this.SetDex(30, 45);
            this.SetInt(10, 25);

            this.SetHits(200);
            this.SetMana(0);

            this.SetDamage(4, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

			//We want them to be protected from players casting on them.
            this.SetResistance(ResistanceType.Physical, 1, 3);
            this.SetResistance(ResistanceType.Energy, 100);
            this.SetResistance(ResistanceType.Fire, 100);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Cold, 100);


            this.SetSkill(SkillName.MagicResist, 100.0);
            this.SetSkill(SkillName.Tactics, 5.0, 10.0);
            this.SetSkill(SkillName.Wrestling, 5.0, 10.0);

            this.Fame = 0;
            this.Karma = 0;

            this.VirtualArmor = Utility.Random(2, 10);

            this.Tamable = false;
            this.ControlSlots = 10;
		}

		public override int Meat{ get{ return 0; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override FoodType FavoriteFood{ get{ return FoodType.GrainsAndHay; } }
		public override int Feathers{ get{ return 0; } }
		
		public override void AddNameProperties(ObjectPropertyList list)
		{
			base.AddNameProperties(list);
			
			if(m_Owner != null && m_Owner is PlayerMobile)
				list.Add( String.Format( "Bet On By {0}", m_Owner.Name ) );
		}
		
		public override bool OnBeforeDeath()
		{
            Account acct = (Account)m_Owner.Account;
            bool CockFightBetted = Convert.ToBoolean(acct.GetTag("CockFightBetted"));

			m_Controller.DeadChickens++;
			m_Owner.SendMessage("Your chicken has died. You lose.");
            m_Controller.ChickenList.Remove(this);  
            acct.SetTag("CockFightBetted", "false");
			
			if( m_Controller.DeadChickens == (m_Controller.MaxPlayers - 1) )  //one chicken left?
				m_Controller.EndGame(m_Controller);

			return base.OnBeforeDeath();
		}
				
		public FightingChicken(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}