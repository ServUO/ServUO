#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Xanthos.Interfaces;
using Xanthos.ShrinkSystem;

namespace Xanthos.Evo
{
	[CorpseName( "an evolution creature corpse" )]
	public abstract class BaseEvo : BaseCreature, IEvoCreature
	{
		private static double kOverLimitLossChance = 0.02;	// Chance that loyalty will be lost if over followers limit

		protected int m_Ep;
		protected int m_Stage;
		protected bool m_Pregnant;
		protected bool m_HasEgg;
		protected DateTime m_DeliveryDate;
		protected ShrinkItem m_ShrinkItem;

		protected int m_FinalStage;
		protected int m_MaxTrainingStage;
		protected int m_EpMinDivisor;
		protected int m_EpMaxDivisor;
		protected int m_DustMultiplier;
		protected int m_NextEpThreshold;
		protected TimeSpan m_InitialTerm;
		protected bool m_CanAttackPlayers;
		protected bool m_ProducesYoung;
		protected bool m_AlwaysHappy;
		protected DateTime m_NextHappyTime;

		protected string m_Breed;
		protected BaseAI m_ForcedAI;
		protected PregnancyTimer m_PregnancyTimer;

		// Implement these 3 in your subclass to return BaseEvoSpec & BaseEvoEgg subclasses & Dust Type
		public abstract BaseEvoSpec GetEvoSpec();
		public abstract BaseEvoEgg GetEvoEgg();
		public abstract Type GetEvoDustType();
		// Implement these 2 in your subclass to control where exp points are accumulated
		public abstract bool AddPointsOnDamage { get; }
		public abstract bool AddPointsOnMelee { get; }

		[CommandProperty( AccessLevel.Administrator )]
		public bool CanHue
		{
			get { return true; }
		}

		[CommandProperty( AccessLevel.Administrator )]
		public int Ep
		{
			get { return m_Ep; }
			set { m_Ep = value; }
		}

		[CommandProperty( AccessLevel.Administrator )]
		public int Stage
		{
			get { return m_Stage; }
		}

		[CommandProperty( AccessLevel.Administrator )]
		public DateTime DeliveryDate
		{
			get { return m_DeliveryDate; }
			set { m_DeliveryDate = value; }
		}

		[CommandProperty( AccessLevel.Administrator )]
		public TimeSpan RemainingTerm
		{
			get
			{
				return ( DateTime.MinValue == m_DeliveryDate ? m_InitialTerm : m_DeliveryDate.Subtract( DateTime.Now ));
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool ProducesYoung
		{
			get { return this is IEvoGuardian ? false : m_ProducesYoung; }
			set { m_ProducesYoung = value; }
		}

		[CommandProperty( AccessLevel.Administrator )]
		public int MaxTrainingStage
		{
			get { return m_MaxTrainingStage; }
			set { m_MaxTrainingStage = value; }
		}

		public string Breed
		{
			get
			{
				return ( null == m_Breed ? m_Breed = Xanthos.Utilities.Misc.GetFriendlyClassName( GetType().Name ) : m_Breed );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Pregnant
		{
			get { return this is IEvoGuardian ? false : m_Pregnant; }
			set
			{
				if ( !( this is IEvoGuardian ) && ( m_Pregnant = Blessed = value ) )
				{
					m_PregnancyTimer = new PregnancyTimer( this );
					DeliveryDate = DateTime.Now + m_PregnancyTimer.Delay;

				}
				else if ( null != m_PregnancyTimer )
				{
					m_PregnancyTimer.Stop();
					m_PregnancyTimer = null;
					DeliveryDate = DateTime.MinValue;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasEgg
		{
			get { return this is IEvoGuardian ? false : m_HasEgg; }
			set
			{ 
				if ( m_HasEgg = value )
					Pregnant = false;
			}
		}

		public BaseEvo( string name, AIType ai, double dActiveSpeed ) : base( ai, FightMode.Closest, 10, 1, dActiveSpeed, 0.4 )
		{
			Name = name;
			Init();
			InitAI();
		}

		public BaseEvo( Serial serial ) : base( serial )
		{
			InitAI();
		}

		protected virtual void Init()
		{
			BaseEvoSpec spec = GetEvoSpec();

			if ( null != spec && null != spec.Stages )
			{
				m_Stage = -1;
				Female = ( spec.PercentFemaleChance > Utility.RandomDouble() );
				m_ProducesYoung = spec.ProducesYoung;
				m_Pregnant = m_HasEgg = false;
				m_InitialTerm = TimeSpan.FromDays( spec.PregnancyTerm );
				m_FinalStage = spec.Stages.Length - 1;
				m_MaxTrainingStage = spec.MaxTrainingStage;
				m_DeliveryDate = DateTime.MinValue;
				Tamable = spec.Tamable;
				SetFameLevel( spec.FameLevel );
				SetKarmaLevel( spec.KarmaLevel );
				m_CanAttackPlayers = spec.CanAttackPlayers;

				if ( null != spec.Skills )
				{	
					double skillTotals = 0.0;

					for ( int i = 0;  i < spec.Skills.Length; i++ )
					{
						Skills[spec.Skills[ i ]].Cap = spec.MaxSkillValues[ i ];
						skillTotals += spec.MaxSkillValues[ i ];
						SetSkill( spec.Skills[ i ], (double)(spec.MinSkillValues[ i ]), (double)(spec.MaxSkillValues[ i ]) );
					}

					if ( ( skillTotals *= 10 ) > SkillsCap )
					{
						SkillsCap = (int)skillTotals;
					}
				}
				if ( this is IEvoGuardian )
				{
					// Go all the way
					while ( m_Stage < m_FinalStage )
					{
						m_Ep = m_NextEpThreshold;
						Evolve( false );
					}
				}
				else
					Evolve( true );	// Evolve once as a new born

				if ( spec.PackSpecialItemChance > Utility.RandomDouble() )
					PackSpecialItem();
			}
		}

		protected override BaseAI ForcedAI { get { return m_ForcedAI; } }

		private void InitAI()
		{
			switch ( AI )
			{
				case AIType.AI_Melee:
					m_ForcedAI = new EvoMeleeAI( this, m_CanAttackPlayers );
					break;
				case AIType.AI_Berserk:
					m_ForcedAI = new EvoBerserkAI( this, m_CanAttackPlayers );
					break;
				case AIType.AI_Archer:
					m_ForcedAI = new EvoArcherAI( this, m_CanAttackPlayers );
					break;
				case AIType.AI_Mage:
					m_ForcedAI = new EvoMageAI( this, m_CanAttackPlayers );
					break;
				default:
					m_ForcedAI = null;
					break;
			}
			ChangeAIType( AI );
		}

		// We don't need no stinking paragons
		public override void OnBeforeSpawn( Point3D location, Map m )
		{
			base.OnBeforeSpawn( location, m );
			Paragon.UnConvert( this );
		}

		// Use this to place a surprise in the Evo's pack randomly on creation
		protected virtual void PackSpecialItem() {}

		public override void OnThink()
		{
			base.OnThink();

			if ( this is IEvoGuardian )
				return;

			else if ( null != ControlMaster && ControlMaster.Followers > ControlMaster.FollowersMax && kOverLimitLossChance >= Utility.RandomDouble() )
			{
				ControlMaster.SendMessage( Name + " is losing confidence in your ability to control so many creatures!" );
				Say( 1043270, Name ); // * ~1_NAME~ looks around desperately *
				PlaySound( GetIdleSound() );
				if ( Loyalty > BaseCreature.MaxLoyalty / 10 )
					Loyalty--;
			}
			else if ( m_AlwaysHappy && DateTime.Now >= m_NextHappyTime && null != ControlMaster && ControlMaster.Map == Map )
			{
				Loyalty = BaseCreature.MaxLoyalty;
				m_NextHappyTime = DateTime.Now + TimeSpan.FromMinutes( 30.0 );
			}
		}
		
		public override void Damage( int amount, Mobile defender )
		{
			if ( AddPointsOnDamage )
				AddPoints( defender );

			base.Damage( amount, defender );
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			if ( AddPointsOnMelee )
				AddPoints( defender );

			base.OnGaveMeleeAttack( defender );
		}

		private void AddPoints( Mobile defender )
		{
			if ( defender == null || defender.Deleted )
				return;

			if ( defender is TrainingElemental && m_Stage >= m_MaxTrainingStage && null != ControlMaster )
			{
				Emote( "*stops fighting*" );
				Combatant = null;
				ControlTarget = ControlMaster;
				ControlOrder = OrderType.Follow;
				ControlMaster.SendMessage ( "Your pet can no longer gain experience points fighting Training Elementals!");
			}
			else if ( defender is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)defender;

				if ( bc.Controlled != true )
					m_Ep += Utility.RandomMinMax( 5 + ( bc.HitsMax ) / m_EpMinDivisor, 5 + ( bc.HitsMax ) / m_EpMaxDivisor );

				if ( m_Stage < m_FinalStage && m_Ep >= m_NextEpThreshold )
				{
					Evolve( false );
				}
			}
		}

		protected virtual void Evolve( bool hatching )
		{
			BaseEvoSpec spec = GetEvoSpec();

			if ( null != spec && null != spec.Stages )
			{
				BaseEvoStage stage = spec.Stages[ ++m_Stage ];

				if ( null != stage )
				{
					int OldControlSlots = ControlSlots;
					
					if ( null != stage.Title )		Title			= stage.Title;
					if ( 0 != stage.BaseSoundID )	BaseSoundID		= stage.BaseSoundID;
					if ( 0 != stage.BodyValue )		Body			= stage.BodyValue;
					if ( 0 != stage.VirtualArmor )	VirtualArmor	= stage.VirtualArmor;
					if ( 0 != stage.ControlSlots )	ControlSlots	= stage.ControlSlots;
					if ( 0 != stage.MinTameSkill )	MinTameSkill	= stage.MinTameSkill;
					if ( 0 != stage.EpMinDivisor )	m_EpMinDivisor	= stage.EpMinDivisor;
					if ( 0 != stage.EpMaxDivisor )	m_EpMaxDivisor	= stage.EpMaxDivisor;
					if ( 0 != stage.DustMultiplier )m_DustMultiplier= stage.DustMultiplier;
					m_NextEpThreshold = stage.NextEpThreshold;

					if ( spec.AbsoluteStatValues )
					{
						SetStr( stage.StrMin, stage.StrMax );
						SetDex( stage.DexMin, stage.DexMin );
						SetInt( stage.IntMin, stage.IntMax );
						SetHits( stage.HitsMin, stage.HitsMax );
						SetDamage( stage.DamageMin, stage.DamageMax );
					}
					else
					{
						SetStr( RawStr + Utility.RandomMinMax( stage.StrMin, stage.StrMax ));
						SetDex( RawDex + Utility.RandomMinMax( stage.DexMin, stage.DexMin ));
						SetInt( RawInt + Utility.RandomMinMax( stage.IntMin, stage.IntMax ));
						SetHits( HitsMax + Utility.RandomMinMax( stage.HitsMin, stage.HitsMax ));
						SetDamage( DamageMin + stage.DamageMin, DamageMax + stage.DamageMax );
					}

					if ( null != stage.DamagesTypes )
					{
						for ( int i = 0; i < stage.DamagesTypes.Length; i++ )
							SetDamageType( stage.DamagesTypes[ i ], Utility.RandomMinMax( stage.MinDamages[ i ], stage.MaxDamages[ i ] ));
					}
					if ( null != stage.ResistanceTypes )
					{
						for ( int i = 0; i < stage.ResistanceTypes.Length; i++ )
							SetResistance( stage.ResistanceTypes[ i ], Utility.RandomMinMax( stage.MinResistances[ i ], stage.MaxResistances[ i ] ));
					}

					Hue = GetEvoHue( spec, stage );

					if ( null != ControlMaster && stage.ControlSlots > 0 && ControlSlots > 0 )
						ControlMaster.Followers += stage.ControlSlots - OldControlSlots;

					if ( !( hatching || this is IEvoGuardian ) )
					{
						PlaySound( 665 );
						Emote( "*" + Name + " " + stage.EvolutionMessage + "*" );
					}
					Warmode = false;
				}
			}
		}

		private int GetEvoHue( BaseEvoSpec spec, BaseEvoStage stage )
		{
			if ( stage.Hue == 0 )
				return Hue;

			if ( stage.Hue == Evo.Flags.kRandomHueFlag && spec.RandomHues != null && spec.RandomHues.Length > 0 )
				return Utility.RandomList( spec.RandomHues );

			if ( stage.Hue == Evo.Flags.kUnHueFlag )
				return 0;

			return stage.Hue;
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			PlayerMobile player = from as PlayerMobile;

			if ( this is IEvoGuardian )
				return base.OnDragDrop( from, dropped );
  
			if ( null != ControlMaster && ControlMaster.Followers > ControlMaster.FollowersMax )
			{
				ControlMaster.SendMessage( Name + " is not interested in that now!" );
				return false;
			}
			
			if ( null != player && dropped.GetType() == GetEvoDustType() )
			{
				BaseEvoDust dust = dropped as BaseEvoDust;

				if ( null != dust )
				{
					int amount = ( dust.Amount * m_DustMultiplier );

					m_Ep += amount;
					PlaySound( 665 );
					dust.Delete();
					Emote( "*" + this.Name + " absorbs the " + dust.Name + " gaining " + amount + " experience points*" );
					return true;
				}
				return false;
			}
			return base.OnDragDrop( from, dropped );
		}

		public void OnShrink( IShrinkItem shrinkItem )
		{
			m_ShrinkItem = (ShrinkItem)shrinkItem;
		}

		private void MatingTarget_Callback( Mobile from, object obj )
		{
			BaseEvo evo = obj as BaseEvo;

			if ( null == evo )
				from.SendMessage( "That is not a pet!" );

			else if ( evo.Controlled == false )
				from.SendMessage( "That " + evo.Breed + " is wild and cannot be bred!" );

			else
			{
				if ( evo.Female == true )
					from.SendMessage( "That " + evo.Breed + " is not male!" );

				else if ( evo.Stage < m_FinalStage )
					from.SendMessage( "That male " + evo.Breed + " is not old enough to mate!" );

				else if ( evo.ControlMaster == from )
				{
					Pregnant = true;
				}
				else
				{
					evo.ControlMaster.SendGump( new MatingGump( from, evo.ControlMaster, this, evo ) );
					from.SendMessage( "You ask the owner of the " + evo.Breed + " if they will let your female mate with their male." );
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !ProducesYoung || this is IEvoGuardian )
				base.OnDoubleClick( from );

			else if ( Controlled == true && ControlMaster == from && Female == true )
			{
				if ( Stage < m_FinalStage )
					from.SendMessage( "This female " + Breed + " is not yet old enough to mate." );

				else if ( Pregnant == true )
					from.SendMessage( "This " + Breed + " has not yet produced an egg." );

				else if ( HasEgg == true )
				{
					HasEgg = false;
					from.AddToBackpack( GetEvoEgg() );
					from.SendMessage( "A " + Breed + " egg has been placed in your backpack." );
				}
				else
				{
					from.SendMessage( "Target a male " + Breed + " to mate with this female." );
					from.BeginTarget( -1, false, TargetFlags.Harmful, new TargetCallback( MatingTarget_Callback ) );
				}
			}
		}

		public override int GetMaxResistance( ResistanceType type )
		{
			if ( this is IEvoGuardian )
				return base.GetMaxResistance( type );

			int resistance = base.GetMaxResistance( type );

			BaseEvoSpec spec = GetEvoSpec();

			return ( spec == null ? resistance : resistance > spec.MaxEvoResistance ? spec.MaxEvoResistance : resistance );
		}

		public void LoadSpecValues()
		{
			BaseEvoSpec spec = GetEvoSpec();

			if ( null != spec && null != spec.Stages )
			{
				BaseEvoStage stage = spec.Stages[ m_Stage ];
				if ( null != stage )
				{
					m_FinalStage = spec.Stages.Length - 1;
					m_ProducesYoung = spec.ProducesYoung;
					m_InitialTerm = TimeSpan.FromDays( spec.PregnancyTerm );
					m_EpMinDivisor = stage.EpMinDivisor;
					m_EpMaxDivisor = stage.EpMaxDivisor;
					m_DustMultiplier = stage.DustMultiplier;
					m_NextEpThreshold = stage.NextEpThreshold;
					m_AlwaysHappy = spec.AlwaysHappy;
					m_NextHappyTime = DateTime.Now;
					m_MaxTrainingStage = spec.MaxTrainingStage;
					m_CanAttackPlayers = spec.CanAttackPlayers;
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			
			writer.Write( (int)2 );

			// Version 2
			writer.Write( (ShrinkItem)m_ShrinkItem );

			// Version 1

			// Version 0
			writer.Write( (int)m_Ep );
			writer.Write( (int)m_Stage );
			writer.Write( (bool)m_Pregnant );
			writer.Write( (bool)m_HasEgg );
			writer.Write( (DateTime)m_DeliveryDate );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
					m_ShrinkItem = (ShrinkItem)(reader.ReadItem());
					goto case 1;

				case 1:
					goto case 0;	// Account for Dracna's version bump

				case 0:
				{
					m_Ep = reader.ReadInt();
					m_Stage = reader.ReadInt();
					Pregnant = m_Pregnant = reader.ReadBool();	// resets the timer if pregnant
					m_HasEgg = reader.ReadBool();
					m_DeliveryDate = reader.ReadDateTime();
					break;
				}
			}
			LoadSpecValues();
		}
	}

	public class PregnancyTimer : Timer
	{
		IEvoCreature m_Evo;

		public PregnancyTimer( IEvoCreature female ) : base( female.RemainingTerm )
		{
			Priority = TimerPriority.OneMinute;
			Start();
			m_Evo = female;
		}

		protected override void OnTick()
		{
			m_Evo.HasEgg = true;
			Stop();
		}
	}
}