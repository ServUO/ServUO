using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Items;

namespace Server.Spells.SkillMasteries
{
    public class BodyGuardSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Body Guard", "",
                -1,
                9002
            );
 
        public override int RequiredMana { get { return 40; } }
        public override int DisruptMessage { get { return 1156103; } } // Bodyguard has expired.
        public override bool BlocksMovement { get { return false; } }
        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(1.0); } }

        public override SkillName CastSkill { get { return SkillName.Parry; } }
 
		private double _Block;
 
        public BodyGuardSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }
		
		public override bool CheckCast()
		{
            if (_Table != null && _Table.ContainsKey(Caster))
            {
                RemoveGumpTimer(_Table[Caster], Caster);
                return false;
            }

			BodyGuardSpell spell = GetSpell(Caster, this.GetType()) as BodyGuardSpell;
			
			if(spell != null)
			{
				spell.Expire(true);
				return false;
			}

            if (!HasShield())
                return false;
			
			return base.CheckCast();
		}
 
        public override void OnCast()
        {
            Caster.BeginTarget(8, false, Server.Targeting.TargetFlags.None, (m, o) =>
                {
                    if (!Caster.CanSee(o))
                        Caster.SendLocalizedMessage(500237); // Target can not be seen.
                    else
                    {
                        SpellHelper.Turn(Caster, o);
                        OnTarget(o);
                    }
                });
        }
		
		protected override void OnTarget(object o)
		{
            if (!HasShield())
                return;

            Mobile protectee = o as Mobile;
            Mobile master = null;

            if (protectee is BaseCreature && !((BaseCreature)protectee).Summoned && ((BaseCreature)protectee).GetMaster() is PlayerMobile)
                master = ((BaseCreature)protectee).GetMaster();

            if (protectee != null && Caster is PlayerMobile)
			{
                BodyGuardSpell spell = GetSpell(s => s.GetType() == typeof(BodyGuardSpell) && s.Target == protectee) as BodyGuardSpell;
				
				if(spell != null)
				{
					Caster.SendLocalizedMessage(1156094); // Your target is already under the effect of this ability.
				}
                if (!protectee.Alive)
				{
					Caster.SendLocalizedMessage( 501857 ); // This spell won't work on that!
				}
				else if ( !Caster.CanBeBeneficial( protectee, true ))
				{
                    Caster.SendLocalizedMessage(1001017); // You cannot perform beneficial acts on your target.
				}
                else if (protectee != Caster)
				{
                    Mobile responsible = master != null ? master : protectee;

                    Caster.FixedParticles( 0x376A, 9, 32, 5030, 1168, 0, EffectLayer.Waist, 0 );

                    if (Caster.Player)
                    {
                        Caster.PlaySound(Caster.Female ? 0x338 : 0x44A);
                    }
                    else if (Caster is BaseCreature)
                    {
                        Caster.PlaySound(((BaseCreature)Caster).GetAngerSound());
                    }

                    protectee.SendGump(new AcceptBodyguardGump(Caster as PlayerMobile, protectee, this));

                    AddGumpTimer(responsible, Caster);
				}
			}
		}

		public void AcceptBodyGuard(Mobile toGuard, BodyGuardSpell spell)
		{
            RemoveGumpTimer(toGuard, Caster);

			if(CheckBSequence(toGuard))
			{
				_Block = ((Caster.Skills[CastSkill].Value + GetWeaponSkill() + (GetMasteryLevel() * 40)) / 3) / 2.4;
                Target = toGuard;

				Expires = DateTime.UtcNow + TimeSpan.FromSeconds(90);
				BeginTimer();

                Caster.SendLocalizedMessage(1049452, "\t" + toGuard.Name); // You are now protecting ~2_NAME~.
                toGuard.SendLocalizedMessage(1049451, Caster.Name); // You are now being protected by ~1_NAME~.

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Bodyguard, 1155924, 1156061, TimeSpan.FromSeconds(90), Caster, String.Format("{0}\t{1}\t{2}\t{3}", Caster.Name, (_Block + 5).ToString(), toGuard.Name, _Block.ToString())));
                BuffInfo.AddBuff(toGuard, new BuffInfo(BuffIcon.Bodyguard, 1155924, 1156061, TimeSpan.FromSeconds(90), toGuard, String.Format("{0}\t{1}\t{2}\t{3}", Caster.Name, (_Block + 5).ToString(), toGuard.Name, _Block.ToString())));
                //~1_NAME~ receives ~2_DAMAGE~% of all damage dealt to ~3_NAME~. All damage dealt to ~3_NAME~ will be reduced by ~4_DAMAGE~%. Body guard must be within 2 tiles. 
			}

            FinishSequence();
		}

        private bool HasShield()
        {
            if (!Caster.Player)
                return true;

            BaseShield shield = Caster.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

            if (shield == null)
            {
                Caster.SendLocalizedMessage(1156096); // You must be wielding a shield to use this ability!
                return false;
            }

            return true;
        }

        public override bool OnTick()
        {
            if (!HasShield())
            {
                Expire();
                return false;
            }

            return base.OnTick();
        }

        public override void EndEffects()
        {
            if (Target != null)
            {
                Target.SendLocalizedMessage(1156103); // Bodyguard has expired.
                BuffInfo.RemoveBuff(Target, BuffIcon.Bodyguard);
            }

            Caster.SendLocalizedMessage(1156103); // Bodyguard has expired.
            BuffInfo.RemoveBuff(Caster, BuffIcon.Bodyguard);
        }

        public void DeclineBodyGuard(Mobile protectee)
        {
            Caster.SendLocalizedMessage(1049454, "\t" + protectee.Name); // ~2_NAME~ has declined your protection.
            protectee.SendLocalizedMessage(1049453, Caster.Name); // You have declined protection from ~1_NAME~.

            RemoveGumpTimer(protectee, Caster);
            FinishSequence();
        }

        public override void OnTargetDamaged(Mobile attacker, Mobile defender, DamageType type, ref int damage)
		{
            if (defender == Target && Caster.InRange(defender, 2))
			{
				double mod = (double)PropertyBonus() / 100.0;
				
				damage = damage - (int)((double)damage * mod);
                int casterDamage = damage - (int)((double)damage * (mod - .05));

                if (type >= DamageType.Spell)
                    casterDamage /= 2;

                Caster.Damage(casterDamage, attacker);
			}
		}

        public static void RemoveGumpTimer(Mobile m, Mobile caster)
        {
            if (_Table != null && _Table.ContainsKey(caster))
                _Table.Remove(caster);

            if (m.HasGump(typeof(AcceptBodyguardGump)))
            {
                m.CloseGump(typeof(AcceptBodyguardGump));
                m.SendLocalizedMessage(1156103); // Bodyguard has expired.
                caster.SendLocalizedMessage(1156103); // Bodyguard has expired.
            }
        }

        public static void AddGumpTimer(Mobile m, Mobile caster)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, Mobile>();

            _Table[caster] = m;

            Server.Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
            {
                RemoveGumpTimer(m, caster);
            });
        }

        private static Dictionary<Mobile, Mobile> _Table;
		
		public override int PropertyBonus()
		{
			return (int)_Block;
		}
    }

    public class AcceptBodyguardGump : Gump
	{
		private Mobile m_Protector;
		private Mobile m_Protectee;
        private BodyGuardSpell m_Spell;

		public AcceptBodyguardGump( Mobile protector, Mobile protectee, BodyGuardSpell spell ) : base( 150, 50 )
		{
			m_Protector = protector;
			m_Protectee = protectee;
            m_Spell = spell;

			Closable = false;

			AddPage( 0 );

			AddBackground( 0, 0, 396, 218, 3600 );

			AddImageTiled( 15, 15, 365, 190, 2624 );
			AddAlphaRegion( 15, 15, 365, 190 );

			AddHtmlLocalized( 30, 20, 360, 25, 1156099, 0x7FFF, false, false ); // Another player is offering to bodyguard you:
			AddLabel( 90, 55, 1153, String.Format("{0} will body guard {1}", protector.Name, protectee.Name));

			AddImage( 50, 45, 9005 );
			AddImageTiled( 80, 80, 200, 1, 9107 );
			AddImageTiled( 95, 82, 200, 1, 9157 );

			AddRadio( 30, 110, 9727, 9730, true, 1 );
			AddHtmlLocalized( 65, 115, 300, 25, 1049444, 0x7FFF, false, false ); // Yes, I would like their protection.

			AddRadio( 30, 145, 9727, 9730, false, 0 );
			AddHtmlLocalized( 65, 148, 300, 25, 1049445, 0x7FFF, false, false ); // No thanks, I can take care of myself.

			AddButton( 160, 175, 247, 248, 2, GumpButtonType.Reply, 0 );

			AddImage( 215, 0, 50581 );

			AddImageTiled( 15, 14, 365, 1, 9107 );
			AddImageTiled( 380, 14, 1, 190, 9105 );
			AddImageTiled( 15, 205, 365, 1, 9107 );
			AddImageTiled( 15, 14, 1, 190, 9105 );
			AddImageTiled( 0, 0, 395, 1, 9157 );
			AddImageTiled( 394, 0, 1, 217, 9155 );
			AddImageTiled( 0, 216, 395, 1, 9157 );
			AddImageTiled( 0, 0, 1, 217, 9155 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 2 )
			{
				bool okay = info.IsSwitched( 1 );

                if(okay)
                    m_Spell.AcceptBodyGuard(m_Protectee, m_Spell);
                else
                    m_Spell.DeclineBodyGuard(m_Protectee);
				/*if ( okay )
					JusticeVirtue.OnVirtueAccepted( m_Protector, m_Protectee );
				else
					JusticeVirtue.OnVirtueRejected( m_Protector, m_Protectee );*/
			}
		}
	}
}