using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Server.Spells.Mysticism
{
	public abstract class MysticTransformationSpell : MysticSpell, ITransformationSpell
	{
		public abstract int Body{ get; }
		public virtual int Hue{ get{ return 0; } }

		public virtual int PhysResistOffset{ get{ return 0; } }
		public virtual int FireResistOffset{ get{ return 0; } }
		public virtual int ColdResistOffset{ get{ return 0; } }
		public virtual int PoisResistOffset{ get{ return 0; } }
		public virtual int NrgyResistOffset{ get{ return 0; } }

		public MysticTransformationSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public override bool BlockedByHorrificBeast{ get{ return false; } }

		public override bool CheckCast()
		{
			if( !TransformationSpellHelper.CheckCast( Caster, this ) )
				return false;

			return base.CheckCast();
		}

		public override void OnCast()
		{
			TransformationSpellHelper.OnCast( Caster, this );

			FinishSequence();
		}

		public virtual double TickRate{ get{ return 1.0; } }

		public virtual void OnTick( Mobile m )
		{
		}

		public virtual void DoEffect( Mobile m )
		{
		}

		public virtual void RemoveEffect( Mobile m )
		{
		}
	}
}