#region Header
// **********
// ServUO - GuardAI.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;

using Server.Factions.AI;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Second;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Targeting;
#endregion

namespace Server.Factions
{
	public enum GuardAI
	{
		Bless = 0x01, // heal, cure, +stats
		Curse = 0x02, // poison, -stats
		Melee = 0x04, // weapons
		Magic = 0x08, // damage spells
		Smart = 0x10 // smart weapons/damage spells
	}

	public class ComboEntry
	{
		private readonly Type m_Spell;
		private readonly TimeSpan m_Hold;
		private readonly int m_Chance;

		public ComboEntry(Type spell)
			: this(spell, 100, TimeSpan.Zero)
		{ }

		public ComboEntry(Type spell, int chance)
			: this(spell, chance, TimeSpan.Zero)
		{ }

		public ComboEntry(Type spell, int chance, TimeSpan hold)
		{
			m_Spell = spell;
			m_Chance = chance;
			m_Hold = hold;
		}

		public Type Spell { get { return m_Spell; } }
		public TimeSpan Hold { get { return m_Hold; } }
		public int Chance { get { return m_Chance; } }
	}

	public class SpellCombo
	{
		public static readonly SpellCombo Simple = new SpellCombo(
			50,
			new ComboEntry(typeof(ParalyzeSpell), 20),
			new ComboEntry(typeof(ExplosionSpell), 100, TimeSpan.FromSeconds(2.8)),
			new ComboEntry(typeof(PoisonSpell), 30),
			new ComboEntry(typeof(EnergyBoltSpell)));

		public static readonly SpellCombo Strong = new SpellCombo(
			90,
			new ComboEntry(typeof(ParalyzeSpell), 20),
			new ComboEntry(typeof(ExplosionSpell), 50, TimeSpan.FromSeconds(2.8)),
			new ComboEntry(typeof(PoisonSpell), 30),
			new ComboEntry(typeof(ExplosionSpell), 100, TimeSpan.FromSeconds(2.8)),
			new ComboEntry(typeof(EnergyBoltSpell)),
			new ComboEntry(typeof(PoisonSpell), 30),
			new ComboEntry(typeof(EnergyBoltSpell)));

		private readonly int m_Mana;
		private readonly ComboEntry[] m_Entries;

		public SpellCombo(int mana, params ComboEntry[] entries)
		{
			m_Mana = mana;
			m_Entries = entries;
		}

		public int Mana { get { return m_Mana; } }
		public ComboEntry[] Entries { get { return m_Entries; } }

		public static Spell Process(Mobile mob, Mobile targ, ref SpellCombo combo, ref int index, ref DateTime releaseTime)
		{
			while (++index < combo.m_Entries.Length)
			{
				ComboEntry entry = combo.m_Entries[index];

				if (entry.Spell == typeof(PoisonSpell) && targ.Poisoned)
				{
					continue;
				}

				if (entry.Chance > Utility.Random(100))
				{
					releaseTime = DateTime.UtcNow + entry.Hold;
					return (Spell)Activator.CreateInstance(entry.Spell, new object[] {mob, null});
				}
			}

			combo = null;
			index = -1;
			return null;
		}
	}

	public class FactionGuardAI : BaseAI
	{
		private const int ManaReserve = 30;
		private readonly BaseFactionGuard m_Guard;
		private BandageContext m_Bandage;
		private DateTime m_BandageStart;
		private SpellCombo m_Combo;
		private int m_ComboIndex = -1;
		private DateTime m_ReleaseTarget;

		public FactionGuardAI(BaseFactionGuard guard)
			: base(guard)
		{
			m_Guard = guard;
		}

		public bool IsDamaged { get { return (m_Guard.Hits < m_Guard.HitsMax); } }
		public bool IsPoisoned { get { return m_Guard.Poisoned; } }

		public TimeSpan TimeUntilBandage
		{
			get
			{
				if (m_Bandage != null && m_Bandage.Timer == null)
				{
					m_Bandage = null;
				}

				if (m_Bandage == null)
				{
					return TimeSpan.MaxValue;
				}

				TimeSpan ts = (m_BandageStart + m_Bandage.Timer.Delay) - DateTime.UtcNow;

				if (ts < TimeSpan.FromSeconds(-1.0))
				{
					m_Bandage = null;
					return TimeSpan.MaxValue;
				}

				if (ts < TimeSpan.Zero)
				{
					ts = TimeSpan.Zero;
				}

				return ts;
			}
		}

		public bool IsAllowed(GuardAI flag)
		{
			return ((m_Guard.GuardAI & flag) == flag);
		}

		public bool DequipWeapon()
		{
			Container pack = m_Guard.Backpack;

			if (pack == null)
			{
				return false;
			}

			Item weapon = m_Guard.Weapon as Item;

			if (weapon != null && weapon.Parent == m_Guard && !(weapon is Fists))
			{
				pack.DropItem(weapon);
				return true;
			}

			return false;
		}

		public bool EquipWeapon()
		{
			Container pack = m_Guard.Backpack;

			if (pack == null)
			{
				return false;
			}

			Item weapon = pack.FindItemByType(typeof(BaseWeapon));

			if (weapon == null)
			{
				return false;
			}

			return m_Guard.EquipItem(weapon);
		}

		public bool StartBandage()
		{
			m_Bandage = null;

			Container pack = m_Guard.Backpack;

			if (pack == null)
			{
				return false;
			}

			Item bandage = pack.FindItemByType(typeof(Bandage));

			if (bandage == null)
			{
				return false;
			}

			m_Bandage = BandageContext.BeginHeal(m_Guard, m_Guard);
			m_BandageStart = DateTime.UtcNow;
			return (m_Bandage != null);
		}

		public bool UseItemByType(Type type)
		{
			Container pack = m_Guard.Backpack;

			if (pack == null)
			{
				return false;
			}

			Item item = pack.FindItemByType(type);

			if (item == null)
			{
				return false;
			}

			bool requip = DequipWeapon();

			item.OnDoubleClick(m_Guard);

			if (requip)
			{
				EquipWeapon();
			}

			return true;
		}

		public int GetStatMod(Mobile mob, StatType type)
		{
			int offset = 0;
			StatMod buff = mob.GetStatMod(String.Format("[Magic] {0} Buff", type));
			StatMod curse = mob.GetStatMod(String.Format("[Magic] {0} Curse", type));

			if (buff != null)
				offset += buff.Offset;
			if (curse != null)
				offset += buff.Offset;

			return offset;
		}

		public Spell RandomOffenseSpell()
		{
			int maxCircle = (int)((m_Guard.Skills.Magery.Value + 20.0) / (100.0 / 7.0));

			if (maxCircle < 1)
			{
				maxCircle = 1;
			}

			switch (Utility.Random(maxCircle * 2))
			{
				case 0:
				case 1:
					return new MagicArrowSpell(m_Guard, null);
				case 2:
				case 3:
					return new HarmSpell(m_Guard, null);
				case 4:
				case 5:
					return new FireballSpell(m_Guard, null);
				case 6:
				case 7:
					return new LightningSpell(m_Guard, null);
				case 8:
					return new MindBlastSpell(m_Guard, null);
				case 9:
					return new ParalyzeSpell(m_Guard, null);
				case 10:
					return new EnergyBoltSpell(m_Guard, null);
				case 11:
					return new ExplosionSpell(m_Guard, null);
				default:
					return new FlameStrikeSpell(m_Guard, null);
			}
		}

		public Mobile FindDispelTarget(bool activeOnly)
		{
			if (m_Mobile.Deleted || m_Mobile.Int < 95 || CanDispel(m_Mobile) || m_Mobile.AutoDispel)
			{
				return null;
			}

			if (activeOnly)
			{
				var aggressed = m_Mobile.Aggressed;
				var aggressors = m_Mobile.Aggressors;

				Mobile active = null;
				double activePrio = 0.0;

				Mobile comb = m_Mobile.Combatant;

				if (comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && m_Mobile.InRange(comb, 12) &&
					CanDispel(comb))
				{
					active = comb;
					activePrio = m_Mobile.GetDistanceToSqrt(comb);

					if (activePrio <= 2)
					{
						return active;
					}
				}

				for (int i = 0; i < aggressed.Count; ++i)
				{
					AggressorInfo info = aggressed[i];
					Mobile m = info.Defender;

					if (m != comb && m.Combatant == m_Mobile && m_Mobile.InRange(m, 12) && CanDispel(m))
					{
						double prio = m_Mobile.GetDistanceToSqrt(m);

						if (active == null || prio < activePrio)
						{
							active = m;
							activePrio = prio;

							if (activePrio <= 2)
							{
								return active;
							}
						}
					}
				}

				for (int i = 0; i < aggressors.Count; ++i)
				{
					AggressorInfo info = aggressors[i];
					Mobile m = info.Attacker;

					if (m != comb && m.Combatant == m_Mobile && m_Mobile.InRange(m, 12) && CanDispel(m))
					{
						double prio = m_Mobile.GetDistanceToSqrt(m);

						if (active == null || prio < activePrio)
						{
							active = m;
							activePrio = prio;

							if (activePrio <= 2)
							{
								return active;
							}
						}
					}
				}

				return active;
			}
			else
			{
				Map map = m_Mobile.Map;

				if (map != null)
				{
					Mobile active = null, inactive = null;
					double actPrio = 0.0, inactPrio = 0.0;

					Mobile comb = m_Mobile.Combatant;

					if (comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && CanDispel(comb))
					{
						active = inactive = comb;
						actPrio = inactPrio = m_Mobile.GetDistanceToSqrt(comb);
					}

					foreach (Mobile m in m_Mobile.GetMobilesInRange(12))
					{
						if (m != m_Mobile && CanDispel(m))
						{
							double prio = m_Mobile.GetDistanceToSqrt(m);

							if (!activeOnly && (inactive == null || prio < inactPrio))
							{
								inactive = m;
								inactPrio = prio;
							}

							if ((m_Mobile.Combatant == m || m.Combatant == m_Mobile) && (active == null || prio < actPrio))
							{
								active = m;
								actPrio = prio;
							}
						}
					}

					return active != null ? active : inactive;
				}
			}

			return null;
		}

		public bool CanDispel(Mobile m)
		{
			return (m is BaseCreature && ((BaseCreature)m).Summoned && m_Mobile.CanBeHarmful(m, false) &&
					!((BaseCreature)m).IsAnimatedDead);
		}

		public void RunTo(Mobile m)
		{
			/*if ( m.Paralyzed || m.Frozen )
            {
            if ( m_Mobile.InRange( m, 1 ) )
            RunFrom( m );
            else if ( !m_Mobile.InRange( m, m_Mobile.RangeFight > 2 ? m_Mobile.RangeFight : 2 ) && !MoveTo( m, true, 1 ) )
            OnFailedMove();
            }
            else
            {*/
			if (!m_Mobile.InRange(m, m_Mobile.RangeFight))
			{
				if (!MoveTo(m, true, 1))
				{
					OnFailedMove();
				}
			}
			else if (m_Mobile.InRange(m, m_Mobile.RangeFight - 1))
			{
				RunFrom(m);
			}
			/*}*/
		}

		public void RunFrom(Mobile m)
		{
			Run((Direction)((int)m_Mobile.GetDirectionTo(m) - 4) & Direction.Mask);
		}

		public void OnFailedMove()
		{
			/*if ( !m_Mobile.DisallowAllMoves && 20 > Utility.Random( 100 ) && IsAllowed( GuardAI.Magic ) )
            {
            if ( m_Mobile.Target != null )
            m_Mobile.Target.Cancel( m_Mobile, TargetCancelType.Canceled );
            new TeleportSpell( m_Mobile, null ).Cast();
            m_Mobile.DebugSay( "I am stuck, I'm going to try teleporting away" );
            }
            else*/
			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				if (m_Mobile.Debug)
				{
					m_Mobile.DebugSay("My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name);
				}

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				m_Mobile.DebugSay("I am stuck");
			}
		}

		public void Run(Direction d)
		{
			if ((m_Mobile.Spell != null && m_Mobile.Spell.IsCasting) || m_Mobile.Paralyzed || m_Mobile.Frozen ||
				m_Mobile.DisallowAllMoves)
			{
				return;
			}

			m_Mobile.Direction = d | Direction.Running;

			if (!DoMove(m_Mobile.Direction, true))
			{
				OnFailedMove();
			}
		}

		public override bool Think()
		{
			if (m_Mobile.Deleted)
			{
				return false;
			}

			Mobile combatant = m_Guard.Combatant;

			if (combatant == null || combatant.Deleted || !combatant.Alive || combatant.IsDeadBondedPet ||
				!m_Mobile.CanSee(combatant) || !m_Mobile.CanBeHarmful(combatant, false) || combatant.Map != m_Mobile.Map)
			{
				// Our combatant is deleted, dead, hidden, or we cannot hurt them
				// Try to find another combatant
				if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
				{
					m_Mobile.Combatant = combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else
				{
					m_Mobile.Combatant = combatant = null;
				}
			}

			if (combatant != null && (!m_Mobile.InLOS(combatant) || !m_Mobile.InRange(combatant, 12)))
			{
				if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
				{
					m_Mobile.Combatant = combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if (!m_Mobile.InRange(combatant, 36))
				{
					m_Mobile.Combatant = combatant = null;
				}
			}

			Mobile dispelTarget = FindDispelTarget(true);

			if (m_Guard.Target != null && m_ReleaseTarget == DateTime.MinValue)
			{
				m_ReleaseTarget = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);
			}

			if (m_Guard.Target != null && DateTime.UtcNow > m_ReleaseTarget)
			{
				Target targ = m_Guard.Target;

				Mobile toHarm = (dispelTarget == null ? combatant : dispelTarget);

				if ((targ.Flags & TargetFlags.Harmful) != 0 && toHarm != null)
				{
					if (m_Guard.Map == toHarm.Map && (targ.Range < 0 || m_Guard.InRange(toHarm, targ.Range)) && m_Guard.CanSee(toHarm) &&
						m_Guard.InLOS(toHarm))
					{
						targ.Invoke(m_Guard, toHarm);
					}
					else if (targ is DispelSpell.InternalTarget)
					{
						targ.Cancel(m_Guard, TargetCancelType.Canceled);
					}
				}
				else if ((targ.Flags & TargetFlags.Beneficial) != 0)
				{
					targ.Invoke(m_Guard, m_Guard);
				}
				else
				{
					targ.Cancel(m_Guard, TargetCancelType.Canceled);
				}

				m_ReleaseTarget = DateTime.MinValue;
			}

			if (dispelTarget != null)
			{
				if (Action != ActionType.Combat)
				{
					Action = ActionType.Combat;
				}

				m_Guard.Warmode = true;

				RunFrom(dispelTarget);
			}
			else if (combatant != null)
			{
				if (Action != ActionType.Combat)
				{
					Action = ActionType.Combat;
				}

				m_Guard.Warmode = true;

				RunTo(combatant);
			}
			else if (m_Guard.Orders.Movement != MovementType.Stand)
			{
				Mobile toFollow = null;

				if (m_Guard.Town != null && m_Guard.Orders.Movement == MovementType.Follow)
				{
					toFollow = m_Guard.Orders.Follow;

					if (toFollow == null)
					{
						toFollow = m_Guard.Town.Sheriff;
					}
				}

				if (toFollow != null && toFollow.Map == m_Guard.Map && toFollow.InRange(m_Guard, m_Guard.RangePerception * 3) &&
					Town.FromRegion(toFollow.Region) == m_Guard.Town)
				{
					if (Action != ActionType.Combat)
					{
						Action = ActionType.Combat;
					}

					if (m_Mobile.CurrentSpeed != m_Mobile.ActiveSpeed)
					{
						m_Mobile.CurrentSpeed = m_Mobile.ActiveSpeed;
					}

					m_Guard.Warmode = true;

					RunTo(toFollow);
				}
				else
				{
					if (Action != ActionType.Wander)
					{
						Action = ActionType.Wander;
					}

					if (m_Mobile.CurrentSpeed != m_Mobile.PassiveSpeed)
					{
						m_Mobile.CurrentSpeed = m_Mobile.PassiveSpeed;
					}

					m_Guard.Warmode = false;

					WalkRandomInHome(2, 2, 1);
				}
			}
			else
			{
				if (Action != ActionType.Wander)
				{
					Action = ActionType.Wander;
				}

				m_Guard.Warmode = false;
			}

			if ((IsDamaged || IsPoisoned) && m_Guard.Skills.Healing.Base > 20.0)
			{
				TimeSpan ts = TimeUntilBandage;

				if (ts == TimeSpan.MaxValue)
				{
					StartBandage();
				}
			}

			if (m_Mobile.Spell == null && Core.TickCount >= m_Mobile.NextSpellTime)
			{
				Spell spell = null;

				DateTime toRelease = DateTime.MinValue;

				if (IsPoisoned)
				{
					Poison p = m_Guard.Poison;

					TimeSpan ts = TimeUntilBandage;

					if (p != Poison.Lesser || ts == TimeSpan.MaxValue || TimeUntilBandage < TimeSpan.FromSeconds(1.5) ||
						(m_Guard.HitsMax - m_Guard.Hits) > Utility.Random(250))
					{
						if (IsAllowed(GuardAI.Bless))
						{
							spell = new CureSpell(m_Guard, null);
						}
						else
						{
							UseItemByType(typeof(BaseCurePotion));
						}
					}
				}
				else if (IsDamaged && (m_Guard.HitsMax - m_Guard.Hits) > Utility.Random(200))
				{
					if (IsAllowed(GuardAI.Magic) && ((m_Guard.Hits * 100) / Math.Max(m_Guard.HitsMax, 1)) < 10 &&
						m_Guard.Home != Point3D.Zero && !Utility.InRange(m_Guard.Location, m_Guard.Home, 15) && m_Guard.Mana >= 11)
					{
						spell = new RecallSpell(m_Guard, null, new RunebookEntry(m_Guard.Home, m_Guard.Map, "Guard's Home", null), null);
					}
					else if (IsAllowed(GuardAI.Bless))
					{
						if (m_Guard.Mana >= 11 && (m_Guard.Hits + 30) < m_Guard.HitsMax)
						{
							spell = new GreaterHealSpell(m_Guard, null);
						}
						else if ((m_Guard.Hits + 10) < m_Guard.HitsMax &&
								 (m_Guard.Mana < 11 || (m_Guard.NextCombatTime - Core.TickCount) > 2000))
						{
							spell = new HealSpell(m_Guard, null);
						}
					}
					else if (m_Guard.CanBeginAction(typeof(BaseHealPotion)))
					{
						UseItemByType(typeof(BaseHealPotion));
					}
				}
				else if (dispelTarget != null && (IsAllowed(GuardAI.Magic) || IsAllowed(GuardAI.Bless) || IsAllowed(GuardAI.Curse)))
				{
					if (!dispelTarget.Paralyzed && m_Guard.Mana > (ManaReserve + 20) && 40 > Utility.Random(100))
					{
						spell = new ParalyzeSpell(m_Guard, null);
					}
					else
					{
						spell = new DispelSpell(m_Guard, null);
					}
				}

				if (combatant != null)
				{
					if (m_Combo != null)
					{
						if (spell == null)
						{
							spell = SpellCombo.Process(m_Guard, combatant, ref m_Combo, ref m_ComboIndex, ref toRelease);
						}
						else
						{
							m_Combo = null;
							m_ComboIndex = -1;
						}
					}
					else if (20 > Utility.Random(100) && IsAllowed(GuardAI.Magic))
					{
						if (80 > Utility.Random(100))
						{
							m_Combo = (IsAllowed(GuardAI.Smart) ? SpellCombo.Simple : SpellCombo.Strong);
							m_ComboIndex = -1;

							if (m_Guard.Mana >= (ManaReserve + m_Combo.Mana))
							{
								spell = SpellCombo.Process(m_Guard, combatant, ref m_Combo, ref m_ComboIndex, ref toRelease);
							}
							else
							{
								m_Combo = null;

								if (m_Guard.Mana >= (ManaReserve + 40))
								{
									spell = RandomOffenseSpell();
								}
							}
						}
						else if (m_Guard.Mana >= (ManaReserve + 40))
						{
							spell = RandomOffenseSpell();
						}
					}

					if (spell == null && 2 > Utility.Random(100) && m_Guard.Mana >= (ManaReserve + 10))
					{
						int strMod = GetStatMod(m_Guard, StatType.Str);
						int dexMod = GetStatMod(m_Guard, StatType.Dex);
						int intMod = GetStatMod(m_Guard, StatType.Int);

						var types = new List<Type>();

						if (strMod <= 0)
						{
							types.Add(typeof(StrengthSpell));
						}

						if (dexMod <= 0 && IsAllowed(GuardAI.Melee))
						{
							types.Add(typeof(AgilitySpell));
						}

						if (intMod <= 0 && IsAllowed(GuardAI.Magic))
						{
							types.Add(typeof(CunningSpell));
						}

						if (IsAllowed(GuardAI.Bless))
						{
							if (types.Count > 1)
							{
								spell = new BlessSpell(m_Guard, null);
							}
							else if (types.Count == 1)
							{
								spell = (Spell)Activator.CreateInstance(types[0], new object[] {m_Guard, null});
							}
						}
						else if (types.Count > 0)
						{
							if (types[0] == typeof(StrengthSpell))
							{
								UseItemByType(typeof(BaseStrengthPotion));
							}
							else if (types[0] == typeof(AgilitySpell))
							{
								UseItemByType(typeof(BaseAgilityPotion));
							}
						}
					}

					if (spell == null && 2 > Utility.Random(100) && m_Guard.Mana >= (ManaReserve + 10) && IsAllowed(GuardAI.Curse))
					{
						if (!combatant.Poisoned && 40 > Utility.Random(100))
						{
							spell = new PoisonSpell(m_Guard, null);
						}
						else
						{
							int strMod = GetStatMod(combatant, StatType.Str);
							int dexMod = GetStatMod(combatant, StatType.Dex);
							int intMod = GetStatMod(combatant, StatType.Int);

							var types = new List<Type>();

							if (strMod >= 0)
							{
								types.Add(typeof(WeakenSpell));
							}

							if (dexMod >= 0 && IsAllowed(GuardAI.Melee))
							{
								types.Add(typeof(ClumsySpell));
							}

							if (intMod >= 0 && IsAllowed(GuardAI.Magic))
							{
								types.Add(typeof(FeeblemindSpell));
							}

							if (types.Count > 1)
							{
								spell = new CurseSpell(m_Guard, null);
							}
							else if (types.Count == 1)
							{
								spell = (Spell)Activator.CreateInstance(types[0], new object[] {m_Guard, null});
							}
						}
					}
				}

				if (spell != null && (m_Guard.HitsMax - m_Guard.Hits + 10) > Utility.Random(100))
				{
					Type type = null;

					if (spell is GreaterHealSpell)
					{
						type = typeof(BaseHealPotion);
					}
					else if (spell is CureSpell)
					{
						type = typeof(BaseCurePotion);
					}
					else if (spell is StrengthSpell)
					{
						type = typeof(BaseStrengthPotion);
					}
					else if (spell is AgilitySpell)
					{
						type = typeof(BaseAgilityPotion);
					}

					if (type == typeof(BaseHealPotion) && !m_Guard.CanBeginAction(type))
					{
						type = null;
					}

					if (type != null && m_Guard.Target == null && UseItemByType(type))
					{
						if (spell is GreaterHealSpell)
						{
							if ((m_Guard.Hits + 30) > m_Guard.HitsMax && (m_Guard.Hits + 10) < m_Guard.HitsMax)
							{
								spell = new HealSpell(m_Guard, null);
							}
						}
						else
						{
							spell = null;
						}
					}
				}
				else if (spell == null && m_Guard.Stam < (m_Guard.StamMax / 3) && IsAllowed(GuardAI.Melee))
				{
					UseItemByType(typeof(BaseRefreshPotion));
				}

				if (spell == null || !spell.Cast())
				{
					EquipWeapon();
				}
			}
			else if (m_Mobile.Spell is Spell && ((Spell)m_Mobile.Spell).State == SpellState.Sequencing)
			{
				EquipWeapon();
			}

			return true;
		}
	}
}