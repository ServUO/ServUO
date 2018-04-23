using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles
{
    [CorpseName("a battle angelic horse corpse")]
    public class BattleAngelicmare : BaseMount
    {
		public override bool InitialInnocent{ get{ return true; } }
		public override bool DeleteOnRelease{ get{ return true; } }
		
        [Constructable]
        public BattleAngelicmare()
            : this("A Battle Angelicmare")
        {
        }

        [Constructable]
        public BattleAngelicmare(string name)
            : base(name, 0x74, 0x3EA7, AIType.AI_Healer, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.BaseSoundID = Core.AOS ? 0xA8 : 0x16A;

            this.SetStr(596, 725);
            this.SetDex(286, 305);
            this.SetInt(286, 325);

            this.SetHits(1100, 1200);

            this.SetDamage(35, 40);

            this.SetDamageType(ResistanceType.Physical, 100);
            this.SetDamageType(ResistanceType.Fire, 100);
            this.SetDamageType(ResistanceType.Cold, 100);
            this.SetDamageType(ResistanceType.Poison, 100);
            this.SetDamageType(ResistanceType.Energy, 100);			

            this.SetResistance(ResistanceType.Physical, 155, 165);
            this.SetResistance(ResistanceType.Fire, 130, 140);
            this.SetResistance(ResistanceType.Cold, 130, 140);
            this.SetResistance(ResistanceType.Poison, 130, 140);
            this.SetResistance(ResistanceType.Energy, 120, 130);

            this.SetSkill(SkillName.EvalInt, 50.4, 80.0);
            this.SetSkill(SkillName.Magery, 50.4, 80.0);
            this.SetSkill(SkillName.MagicResist, 85.3, 100.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.5, 92.5);

            this.Fame = 1400;
            this.Karma = -1400;

            this.VirtualArmor = 100;

            this.Tamable = true;
            this.ControlSlots = 4;
            this.MinTameSkill = 95.1;

            switch ( Utility.Random(3) )
            {
                case 0:
                    {
                        this.BodyValue = 116;
                        this.ItemID = 16039;
                        break;
                    }
                case 1:
                    {
                        this.BodyValue = 178;
                        this.ItemID = 16041;
                        break;
                    }
                case 2:
                    {
                        this.BodyValue = 179;
                        this.ItemID = 16055;
                        break;
                    }
            }

        }

		public virtual bool HealsPlayers{ get{ return true; } }

		public virtual bool CheckResurrect( Mobile m )
		{
			if ( m.Criminal )
			{
				Say( 501222 ); // Thou art a criminal.  I shall not resurrect thee.
				return false;
			}
			else if ( m.Kills >= 5 )
			{
				Say( 501223 ); // Thou'rt not a decent and good person. I shall not resurrect thee.
				return false;
			}

			return true;
		}
		
		public virtual bool CheckHeal( Mobile m )
		{
			if ( m.Criminal )
			{
				Say( "Thou'rt not a decent and good person. I shall not heal thee." ); // Thou art a criminal.  I shall not resurrect thee.
				return false;
			}
			else if ( m.Kills >= 5 )
			{
				Say( "Thou'rt not a decent and good person. I shall not heal thee." ); // Thou'rt not a decent and good person. I shall not resurrect thee.
				return false;
			}

			return true;
		}

		private DateTime m_NextResurrect;
		private static TimeSpan ResurrectDelay = TimeSpan.FromSeconds( 2.0 );

		public virtual void OfferResurrection( Mobile m )
		{
			if (  m.Alive )
			
			Direction = GetDirectionTo( m );
			Say( 501224 ); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.

			m.PlaySound( 0x214 );
			m.FixedEffect( 0x376A, 10, 16 );

			m.CloseGump( typeof( ResurrectGump ) );
			m.SendGump( new ResurrectGump( m, ResurrectMessage.Healer ) );
			
		}

		public virtual void OfferHeal( PlayerMobile m )
		{
			Direction = GetDirectionTo( m );
			if ( m.Alive )
			{
				Say( 501229 ); // You look like you need some healing my child.

				m.PlaySound( 0x1F2 );
				m.FixedEffect( 0x376A, 9, 32 );

				m.Hits = m.HitsMax;
			}
			
			else
			{
				Say( "Thou'rt not a decent and good person. I shall not heal thee." ); 
			}
		

		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !Controlled && !m.Frozen && DateTime.Now >= m_NextResurrect && InRange( m, 4 ) && !InRange( oldLocation, 4 ) && InLOS( m ) )
			{
				if ( !m.Alive )
				{
					m_NextResurrect = DateTime.Now + ResurrectDelay;

					if ( m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) )
					{
						m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					}
					else if ( CheckResurrect( m ) )
					{
						OfferResurrection( m );
					}
				}
				else if ( this.HealsPlayers && m.Hits < m.HitsMax && m is PlayerMobile  )
				{
					OfferHeal( (PlayerMobile) m );
				}
			}
		}

		public BattleAngelicmare(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }		
        public override int Meat
        {
            get
            {
                return 5;
            }
        }
        public override int Hides
        {
            get
            {
                return 10;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.LowScrolls);
            this.AddLoot(LootPack.Potions);
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

            if (Core.AOS && this.BaseSoundID == 0x16A)
                this.BaseSoundID = 0xA8;
            else if (!Core.AOS && this.BaseSoundID == 0xA8)
                this.BaseSoundID = 0x16A;
        }
    }
}