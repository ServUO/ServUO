#region Header
// **********
// ServUO - Spell.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;

using Server.Engines.ConPVP;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Spells.Bushido;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Second;
using Server.Spells.Spellweaving;
using Server.Targeting;
#endregion

namespace Server.Spells
{
	public abstract class Spell : ISpell
	{
		private readonly Mobile m_Caster;
		private readonly Item m_Scroll;
		private readonly SpellInfo m_Info;
		private SpellState m_State;
		private long m_StartCastTime;

		public SpellState State { get { return m_State; } set { m_State = value; } }
		public Mobile Caster { get { return m_Caster; } }
		public SpellInfo Info { get { return m_Info; } }
		public string Name { get { return m_Info.Name; } }
		public string Mantra { get { return m_Info.Mantra; } }
		public Type[] Reagents { get { return m_Info.Reagents; } }
		public Item Scroll { get { return m_Scroll; } }
		public long StartCastTime { get { return m_StartCastTime; } }

		private static readonly TimeSpan NextSpellDelay = TimeSpan.FromSeconds(0.75);
		private static TimeSpan AnimateDelay = TimeSpan.FromSeconds(1.5);

		public virtual SkillName CastSkill { get { return SkillName.Magery; } }
		public virtual SkillName DamageSkill { get { return SkillName.EvalInt; } }

		public virtual bool RevealOnCast { get { return true; } }
		public virtual bool ClearHandsOnCast { get { return true; } }
		public virtual bool ShowHandMovement { get { return true; } }

		public virtual bool DelayedDamage { get { return false; } }

		public virtual bool DelayedDamageStacking { get { return true; } }
		//In reality, it's ANY delayed Damage spell Post-AoS that can't stack, but, only 
		//Expo & Magic Arrow have enough delay and a short enough cast time to bring up 
		//the possibility of stacking 'em.  Note that a MA & an Explosion will stack, but
		//of course, two MA's won't.

		private static readonly Dictionary<Type, DelayedDamageContextWrapper> m_ContextTable =
			new Dictionary<Type, DelayedDamageContextWrapper>();

		private class DelayedDamageContextWrapper
		{
            private readonly Dictionary<IDamageable, Timer> m_Contexts = new Dictionary<IDamageable, Timer>();

			public void Add(IDamageable d, Timer t)
			{
				Timer oldTimer;

				if (m_Contexts.TryGetValue(d, out oldTimer))
				{
					oldTimer.Stop();
					m_Contexts.Remove(d);
				}

				m_Contexts.Add(d, t);
			}

			public void Remove(IDamageable d)
			{
				m_Contexts.Remove(d);
			}
		}

        public void StartDelayedDamageContext(IDamageable d, Timer t)
		{
			if (DelayedDamageStacking)
			{
				return; //Sanity
			}

			DelayedDamageContextWrapper contexts;

			if (!m_ContextTable.TryGetValue(GetType(), out contexts))
			{
				contexts = new DelayedDamageContextWrapper();
				m_ContextTable.Add(GetType(), contexts);
			}

			contexts.Add(d, t);
		}

		public void RemoveDelayedDamageContext(IDamageable d)
		{
			DelayedDamageContextWrapper contexts;

			if (!m_ContextTable.TryGetValue(GetType(), out contexts))
			{
				return;
			}

			contexts.Remove(d);
		}

        public void HarmfulSpell(IDamageable d)
		{
			if (d is BaseCreature)
			{
				((BaseCreature)d).OnHarmfulSpell(m_Caster);
			}
            else if (d is IDamageableItem)
            {
                ((IDamageableItem)d).OnHarmfulSpell(m_Caster);
            }
		}

		public Spell(Mobile caster, Item scroll, SpellInfo info)
		{
			m_Caster = caster;
			m_Scroll = scroll;
			m_Info = info;
		}

		public virtual int GetNewAosDamage(int bonus, int dice, int sides, IDamageable singleTarget)
		{
			if (singleTarget != null)
			{
				return GetNewAosDamage(bonus, dice, sides, (Caster.Player && singleTarget is PlayerMobile), GetDamageScalar(singleTarget as Mobile), singleTarget);
			}
			else
			{
				return GetNewAosDamage(bonus, dice, sides, false, null);
			}
		}

        public virtual int GetNewAosDamage(int bonus, int dice, int sides, bool playerVsPlayer, IDamageable damageable)
		{
			return GetNewAosDamage(bonus, dice, sides, playerVsPlayer, 1.0, damageable);
		}

		public virtual int GetNewAosDamage(int bonus, int dice, int sides, bool playerVsPlayer, double scalar, IDamageable damageable)
		{
            Mobile target = damageable as Mobile;

			int damage = Utility.Dice(dice, sides, bonus) * 100;
			int damageBonus = 0;

			int inscribeSkill = GetInscribeFixed(m_Caster);
			int inscribeBonus = (inscribeSkill + (1000 * (inscribeSkill / 1000))) / 200;
			damageBonus += inscribeBonus;

			int intBonus = Caster.Int / 10;
			damageBonus += intBonus;

			int sdiBonus = AosAttributes.GetValue(m_Caster, AosAttribute.SpellDamage);

			#region Mondain's Legacy
			sdiBonus += ArcaneEmpowermentSpell.GetSpellBonus(m_Caster, playerVsPlayer);
			#endregion

            if (target != null && RunedSashOfWarding.IsUnderEffects(target, WardingEffect.SpellDamage))
                sdiBonus -= 10;

			// PvP spell damage increase cap of 15% from an item’s magic property, 30% if spell school focused.
			if (playerVsPlayer)
			{
			    if (SpellHelper.HasSpellMastery(m_Caster) && sdiBonus > 30)
                {
                    sdiBonus = 30;
                }

                if (!SpellHelper.HasSpellMastery(m_Caster) && sdiBonus > 15)
                {
                    sdiBonus = 15;
                }
			}

			damageBonus += sdiBonus;

			damage = AOS.Scale(damage, 100 + damageBonus);

            if (target != null && Feint.Registry.ContainsKey(target) && Feint.Registry[target].Enemy == Caster)
                damage -= (int)((double)damage * ((double)Feint.Registry[target].DamageReduction / 100));

			int evalSkill = GetDamageFixed(m_Caster);
			int evalScale = 30 + ((9 * evalSkill) / 100);

			damage = AOS.Scale(damage, evalScale);

			damage = AOS.Scale(damage, (int)(scalar * 100));

			return damage / 100;
		}

		public virtual bool IsCasting { get { return m_State == SpellState.Casting; } }

        public virtual void OnCasterHurt()
        {
            CheckCasterDisruption(false, 0, 0, 0, 0, 0);
        }

        public virtual void CheckCasterDisruption(bool checkElem = false, int phys = 0, int fire = 0, int cold = 0, int pois = 0, int nrgy = 0)
        {
            if (!Caster.Player || Caster.AccessLevel > AccessLevel.Player)
            {
                return;
            }

            if (IsCasting)
            {
                object o = ProtectionSpell.Registry[m_Caster];
                bool disturb = true;

                if (o != null && o is double)
                {
                    if (((double)o) > Utility.RandomDouble() * 100.0)
                    {
                        disturb = false;
                    }
                }

                #region Stygian Abyss
                int focus = SAAbsorptionAttributes.GetValue(Caster, SAAbsorptionAttribute.CastingFocus);

                if (BaseFishPie.IsUnderEffects(m_Caster, FishPieEffect.CastFocus))
                    focus += 2;

                if (focus > 12) 
                    focus = 12;

                focus += m_Caster.Skills[SkillName.Inscribe].Value >= 50 ? GetInscribeFixed(m_Caster) / 200 : 0;

                if (focus > 0 && focus > Utility.Random(100))
                {
                    disturb = false;
                    Caster.SendLocalizedMessage(1113690); // You regain your focus and continue casting the spell.
                }
                else if (checkElem)
                {
                    int res = 0;

                    if (phys == 100)
                        res = Math.Min(40, SAAbsorptionAttributes.GetValue(m_Caster, SAAbsorptionAttribute.ResonanceKinetic));

                    else if (fire == 100)
                        res = Math.Min(40, SAAbsorptionAttributes.GetValue(m_Caster, SAAbsorptionAttribute.ResonanceFire));

                    else if (cold == 100)
                        res = Math.Min(40, SAAbsorptionAttributes.GetValue(m_Caster, SAAbsorptionAttribute.ResonanceCold));

                    else if (pois == 100)
                        res = Math.Min(40, SAAbsorptionAttributes.GetValue(m_Caster, SAAbsorptionAttribute.ResonancePoison));

                    else if (nrgy == 100)
                        res = Math.Min(40, SAAbsorptionAttributes.GetValue(m_Caster, SAAbsorptionAttribute.ResonanceEnergy));

                    if (res > Utility.Random(100))
                        disturb = false;
                }
                #endregion

                if (disturb)
                    Disturb(DisturbType.Hurt, false, true);
            }
        }

		public virtual void OnCasterKilled()
		{
			Disturb(DisturbType.Kill);
		}

		public virtual void OnConnectionChanged()
		{
			FinishSequence();
		}

		public virtual bool OnCasterMoving(Direction d)
		{
            if (IsCasting && BlocksMovement && (!(m_Caster is BaseCreature) || ((BaseCreature)m_Caster).FreezeOnCast))
			{
                if (m_Caster is BaseCreature)
                    m_Caster.Say("Trying to move...");

				m_Caster.SendLocalizedMessage(500111); // You are frozen and can not move.
				return false;
			}

			return true;
		}

		public virtual bool OnCasterEquiping(Item item)
		{
			if (IsCasting)
			{
				Disturb(DisturbType.EquipRequest);
			}

			return true;
		}

		public virtual bool OnCasterUsingObject(object o)
		{
			if (m_State == SpellState.Sequencing)
			{
				Disturb(DisturbType.UseRequest);
			}

			return true;
		}

		public virtual bool OnCastInTown(Region r)
		{
			return m_Info.AllowTown;
		}

		public virtual bool ConsumeReagents()
		{
			if (m_Scroll != null || !m_Caster.Player)
			{
				return true;
			}

			if (AosAttributes.GetValue(m_Caster, AosAttribute.LowerRegCost) > Utility.Random(100))
			{
				return true;
			}

			if (DuelContext.IsFreeConsume(m_Caster))
			{
				return true;
			}

			Container pack = m_Caster.Backpack;

			if (pack == null)
			{
				return false;
			}

			if (pack.ConsumeTotal(m_Info.Reagents, m_Info.Amounts) == -1)
			{
				return true;
			}

			return false;
		}

		public virtual double GetInscribeSkill(Mobile m)
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );
			return m.Skills[SkillName.Inscribe].Value;
		}

		public virtual int GetInscribeFixed(Mobile m)
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );
			return m.Skills[SkillName.Inscribe].Fixed;
		}

		public virtual int GetDamageFixed(Mobile m)
		{
			//m.CheckSkill( DamageSkill, 0.0, m.Skills[DamageSkill].Cap );
			return m.Skills[DamageSkill].Fixed;
		}

		public virtual double GetDamageSkill(Mobile m)
		{
			//m.CheckSkill( DamageSkill, 0.0, m.Skills[DamageSkill].Cap );
			return m.Skills[DamageSkill].Value;
		}

		public virtual double GetResistSkill(Mobile m)
		{
			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual double GetDamageScalar(Mobile target)
		{
			double scalar = 1.0;

            if (target == null)
                return scalar;

			if (!Core.AOS) //EvalInt stuff for AoS is handled elsewhere
			{
				double casterEI = m_Caster.Skills[DamageSkill].Value;
				double targetRS = target.Skills[SkillName.MagicResist].Value;

				/*
				if( Core.AOS )
				targetRS = 0;
				*/

				//m_Caster.CheckSkill( DamageSkill, 0.0, 120.0 );

				if (casterEI > targetRS)
				{
					scalar = (1.0 + ((casterEI - targetRS) / 500.0));
				}
				else
				{
					scalar = (1.0 + ((casterEI - targetRS) / 200.0));
				}

				// magery damage bonus, -25% at 0 skill, +0% at 100 skill, +5% at 120 skill
				scalar += (m_Caster.Skills[CastSkill].Value - 100.0) / 400.0;

				if (!target.Player && !target.Body.IsHuman /*&& !Core.AOS*/)
				{
					scalar *= 2.0; // Double magery damage to monsters/animals if not AOS
				}
			}

			if (target is BaseCreature)
			{
				((BaseCreature)target).AlterDamageScalarFrom(m_Caster, ref scalar);
			}

			if (m_Caster is BaseCreature)
			{
				((BaseCreature)m_Caster).AlterDamageScalarTo(target, ref scalar);
			}

			if (Core.SE)
			{
				scalar *= GetSlayerDamageScalar(target);
			}

			target.Region.SpellDamageScalar(m_Caster, target, ref scalar);

			if (Evasion.CheckSpellEvasion(target)) //Only single target spells an be evaded
			{
				scalar = 0;
			}

			return scalar;
		}

		public virtual double GetSlayerDamageScalar(Mobile defender)
		{
			Spellbook atkBook = Spellbook.FindEquippedSpellbook(m_Caster);

			double scalar = 1.0;
			if (atkBook != null)
			{
				SlayerEntry atkSlayer = SlayerGroup.GetEntryByName(atkBook.Slayer);
				SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName(atkBook.Slayer2);

				if (atkSlayer != null && atkSlayer.Slays(defender) || atkSlayer2 != null && atkSlayer2.Slays(defender))
				{
					defender.FixedEffect(0x37B9, 10, 5); //TODO: Confirm this displays on OSIs
					scalar = 2.0;
				}

				TransformContext context = TransformationSpellHelper.GetContext(defender);

				if ((atkBook.Slayer == SlayerName.Silver || atkBook.Slayer2 == SlayerName.Silver) && context != null &&
					context.Type != typeof(HorrificBeastSpell))
				{
					scalar += .25; // Every necromancer transformation other than horrific beast take an additional 25% damage
				}

				if (scalar != 1.0)
				{
					return scalar;
				}
			}

			ISlayer defISlayer = Spellbook.FindEquippedSpellbook(defender);

			if (defISlayer == null)
			{
				defISlayer = defender.Weapon as ISlayer;
			}

			if (defISlayer != null)
			{
				SlayerEntry defSlayer = SlayerGroup.GetEntryByName(defISlayer.Slayer);
				SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName(defISlayer.Slayer2);

				if (defSlayer != null && defSlayer.Group.OppositionSuperSlays(m_Caster) ||
					defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays(m_Caster))
				{
					scalar = 2.0;
				}
			}

			return scalar;
		}

		public virtual void DoFizzle()
		{
			m_Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502632); // The spell fizzles.

			if (m_Caster.Player)
			{
				if (Core.AOS)
				{
					m_Caster.FixedParticles(0x3735, 1, 30, 9503, EffectLayer.Waist);
				}
				else
				{
					m_Caster.FixedEffect(0x3735, 6, 30);
				}

				m_Caster.PlaySound(0x5C);
			}
		}

		private CastTimer m_CastTimer;
		private AnimTimer m_AnimTimer;

		public void Disturb(DisturbType type)
		{
			Disturb(type, true, false);
		}

		public virtual bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
		{
			if (resistable && m_Scroll is BaseWand)
			{
				return false;
			}

			return true;
		}

		public void Disturb(DisturbType type, bool firstCircle, bool resistable)
		{
			if (!CheckDisturb(type, firstCircle, resistable))
			{
				return;
			}

			if (m_State == SpellState.Casting)
			{
				if (!firstCircle && !Core.AOS && this is MagerySpell && ((MagerySpell)this).Circle == SpellCircle.First)
				{
					return;
				}

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb(type, true);

				if (m_CastTimer != null)
				{
					m_CastTimer.Stop();
				}

				if (m_AnimTimer != null)
				{
					m_AnimTimer.Stop();
				}

				if (Core.AOS && m_Caster.Player && type == DisturbType.Hurt)
				{
					DoHurtFizzle();
				}

				m_Caster.NextSpellTime = Core.TickCount + (int)GetDisturbRecovery().TotalMilliseconds;
			}
			else if (m_State == SpellState.Sequencing)
			{
				if (!firstCircle && !Core.AOS && this is MagerySpell && ((MagerySpell)this).Circle == SpellCircle.First)
				{
					return;
				}

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb(type, false);

				Target.Cancel(m_Caster);

				if (Core.AOS && m_Caster.Player && type == DisturbType.Hurt)
				{
					DoHurtFizzle();
				}
			}
		}

		public virtual void DoHurtFizzle()
		{
			m_Caster.FixedEffect(0x3735, 6, 30);
			m_Caster.PlaySound(0x5C);
		}

		public virtual void OnDisturb(DisturbType type, bool message)
		{
			if (message)
			{
				m_Caster.SendLocalizedMessage(500641); // Your concentration is disturbed, thus ruining thy spell.
			}
		}

		public virtual bool CheckCast()
		{
            #region High Seas
            if (Server.Multis.BaseBoat.IsDriving(m_Caster) && m_Caster.AccessLevel == AccessLevel.Player)
            {
                m_Caster.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
                return false;
            }
            #endregion

			return true;
		}

		public virtual void SayMantra()
		{
			if (m_Scroll is BaseWand)
			{
				return;
			}

			if (m_Info.Mantra != null && m_Info.Mantra.Length > 0 && (m_Caster.Player || (m_Caster is BaseCreature && ((BaseCreature)m_Caster).ShowSpellMantra)))
			{
				m_Caster.PublicOverheadMessage(MessageType.Spell, m_Caster.SpeechHue, true, m_Info.Mantra, false);
			}
		}

		public virtual bool BlockedByHorrificBeast { get { return true; } }
		public virtual bool BlockedByAnimalForm { get { return true; } }
		public virtual bool BlocksMovement { get { return true; } }

		public virtual bool CheckNextSpellTime { get { return !(m_Scroll is BaseWand); } }

		public bool Cast()
		{
			m_StartCastTime = Core.TickCount;

			if (Core.AOS && m_Caster.Spell is Spell && ((Spell)m_Caster.Spell).State == SpellState.Sequencing)
			{
				((Spell)m_Caster.Spell).Disturb(DisturbType.NewCast);
			}

			if (!m_Caster.CheckAlive())
			{
				return false;
			}
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).Peaced)
			{
				m_Caster.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
			}
			else if (m_Scroll is BaseWand && m_Caster.Spell != null && m_Caster.Spell.IsCasting)
			{
				m_Caster.SendLocalizedMessage(502643); // You can not cast a spell while frozen.
			}
			else if (m_Caster.Spell != null && m_Caster.Spell.IsCasting)
			{
				m_Caster.SendLocalizedMessage(502642); // You are already casting a spell.
			}
			else if (BlockedByHorrificBeast && TransformationSpellHelper.UnderTransformation(m_Caster, typeof(HorrificBeastSpell)) ||
					 (BlockedByAnimalForm && AnimalForm.UnderTransformation(m_Caster)))
			{
				m_Caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
			}
			else if (!(m_Scroll is BaseWand) && (m_Caster.Paralyzed || m_Caster.Frozen))
			{
				m_Caster.SendLocalizedMessage(502643); // You can not cast a spell while frozen.
			}
			else if (CheckNextSpellTime && Core.TickCount - m_Caster.NextSpellTime < 0)
			{
				m_Caster.SendLocalizedMessage(502644); // You have not yet recovered from casting a spell.
			}
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).PeacedUntil > DateTime.UtcNow)
			{
				m_Caster.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
			}
				#region Dueling
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).DuelContext != null &&
					 !((PlayerMobile)m_Caster).DuelContext.AllowSpellCast(m_Caster, this))
			{ }
				#endregion

			else if (m_Caster.Mana >= ScaleMana(GetMana()))
			{
				#region Stygian Abyss
				if (m_Caster.Race == Race.Gargoyle && m_Caster.Flying)
				{
					var tiles = Caster.Map.Tiles.GetStaticTiles(Caster.X, Caster.Y, true);
					ItemData itemData;
					bool cancast = true;

					for (int i = 0; i < tiles.Length && cancast; ++i)
					{
						itemData = TileData.ItemTable[tiles[i].ID & TileData.MaxItemValue];
						cancast = !(itemData.Name == "hover over");
					}

					if (!cancast)
					{
						if (m_Caster.IsPlayer())
						{
							m_Caster.SendLocalizedMessage(1113750); // You may not cast spells while flying over such precarious terrain.
							return false;
						}
						else
						{
							m_Caster.SendMessage("Your staff level allows you to cast while flying over precarious terrain.");
						}
					}
				}
				#endregion

				if (m_Caster.Spell == null && m_Caster.CheckSpellCast(this) && CheckCast() &&
					m_Caster.Region.OnBeginSpellCast(m_Caster, this))
				{
					m_State = SpellState.Casting;
					m_Caster.Spell = this;

					if (!(m_Scroll is BaseWand) && RevealOnCast)
					{
						m_Caster.RevealingAction();
					}

					SayMantra();

					TimeSpan castDelay = GetCastDelay();

					if (ShowHandMovement && (m_Caster.Body.IsHuman || (m_Caster.Player && m_Caster.Body.IsMonster)))
					{
						int count = (int)Math.Ceiling(castDelay.TotalSeconds / AnimateDelay.TotalSeconds);

						if (count != 0)
						{
							m_AnimTimer = new AnimTimer(this, count);
							m_AnimTimer.Start();
						}

						if (m_Info.LeftHandEffect > 0)
						{
							Caster.FixedParticles(0, 10, 5, m_Info.LeftHandEffect, EffectLayer.LeftHand);
						}

						if (m_Info.RightHandEffect > 0)
						{
							Caster.FixedParticles(0, 10, 5, m_Info.RightHandEffect, EffectLayer.RightHand);
						}
					}

					if (ClearHandsOnCast)
					{
						m_Caster.ClearHands();
					}

					if (Core.ML)
					{
						WeaponAbility.ClearCurrentAbility(m_Caster);
					}

					m_CastTimer = new CastTimer(this, castDelay);
					//m_CastTimer.Start();

					OnBeginCast();

					if (castDelay > TimeSpan.Zero)
					{
						m_CastTimer.Start();
					}
					else
					{
						m_CastTimer.Tick();
					}

					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana
			}

			return false;
		}

		public abstract void OnCast();

		public virtual void OnBeginCast()
		{ }

		public virtual void GetCastSkills(out double min, out double max)
		{
			min = max = 0; //Intended but not required for overriding.
		}

		public virtual bool CheckFizzle()
		{
			if (m_Scroll is BaseWand)
			{
				return true;
			}

			double minSkill, maxSkill;

			GetCastSkills(out minSkill, out maxSkill);

			if (DamageSkill != CastSkill)
			{
				Caster.CheckSkill(DamageSkill, 0.0, Caster.Skills[DamageSkill].Cap);
			}

			return Caster.CheckSkill(CastSkill, minSkill, maxSkill);
		}

		public abstract int GetMana();

		public virtual int ScaleMana(int mana)
		{
			double scalar = 1.0;

            if (ManaPhasingOrb.IsInManaPhase(Caster))
            {
                ManaPhasingOrb.RemoveFromTable(Caster);
                return 0;
            }

			if (!MindRotSpell.GetMindRotScalar(Caster, ref scalar))
			{
				scalar = 1.0;
			}

			// Lower Mana Cost = 40%
			int lmc = AosAttributes.GetValue(m_Caster, AosAttribute.LowerManaCost);

			if (lmc > 40)
			{
				lmc = 40;
			}

            lmc += BaseArmor.GetInherentLowerManaCost(m_Caster);

			scalar -= (double)lmc / 100;

			return (int)(mana * scalar);
		}

		public virtual TimeSpan GetDisturbRecovery()
		{
			if (Core.AOS)
			{
				return TimeSpan.Zero;
			}

			double delay = 1.0 - Math.Sqrt((Core.TickCount - m_StartCastTime) / 1000.0 / GetCastDelay().TotalSeconds);

			if (delay < 0.2)
			{
				delay = 0.2;
			}

			return TimeSpan.FromSeconds(delay);
		}

		public virtual int CastRecoveryBase { get { return 6; } }
		public virtual int CastRecoveryFastScalar { get { return 1; } }
		public virtual int CastRecoveryPerSecond { get { return 4; } }
		public virtual int CastRecoveryMinimum { get { return 0; } }

		public virtual TimeSpan GetCastRecovery()
		{
			if (!Core.AOS)
			{
				return NextSpellDelay;
			}

			int fcr = AosAttributes.GetValue(m_Caster, AosAttribute.CastRecovery);

			int fcrDelay = -(CastRecoveryFastScalar * fcr);

			int delay = CastRecoveryBase + fcrDelay;

			if (delay < CastRecoveryMinimum)
			{
				delay = CastRecoveryMinimum;
			}

			return TimeSpan.FromSeconds((double)delay / CastRecoveryPerSecond);
		}

		public abstract TimeSpan CastDelayBase { get; }

		public virtual double CastDelayFastScalar { get { return 1; } }
		public virtual double CastDelaySecondsPerTick { get { return 0.25; } }
		public virtual TimeSpan CastDelayMinimum { get { return TimeSpan.FromSeconds(0.25); } }

		//public virtual int CastDelayBase{ get{ return 3; } }
		//public virtual int CastDelayFastScalar{ get{ return 1; } }
		//public virtual int CastDelayPerSecond{ get{ return 4; } }
		//public virtual int CastDelayMinimum{ get{ return 1; } }

		public virtual TimeSpan GetCastDelay()
		{
			if (m_Scroll is BaseWand)
			{
				return Core.ML ? CastDelayBase : TimeSpan.Zero; // TODO: Should FC apply to wands?
			}

			// Faster casting cap of 2 (if not using the protection spell) 
			// Faster casting cap of 0 (if using the protection spell) 
			// Paladin spells are subject to a faster casting cap of 4 
			// Paladins with magery of 70.0 or above are subject to a faster casting cap of 2 
			int fcMax = 4;

			if (CastSkill == SkillName.Magery || CastSkill == SkillName.Necromancy ||
                (CastSkill == SkillName.Chivalry && (m_Caster.Skills[SkillName.Magery].Value >= 70.0 || m_Caster.Skills[SkillName.Mysticism].Value >= 70.0)))
			{
				fcMax = 2;
			}

			int fc = AosAttributes.GetValue(m_Caster, AosAttribute.CastSpeed);

			if (fc > fcMax)
			{
				fc = fcMax;
			}

            if (ProtectionSpell.Registry.ContainsKey(m_Caster) /*|| EodonianPotion.IsUnderEffects(m, PotionEffect.Urali)*/)
            {
                fc = Math.Min(fcMax - 2, fc - 2);
            }

			TimeSpan baseDelay = CastDelayBase;

			TimeSpan fcDelay = TimeSpan.FromSeconds(-(CastDelayFastScalar * fc * CastDelaySecondsPerTick));

			//int delay = CastDelayBase + circleDelay + fcDelay;
			TimeSpan delay = baseDelay + fcDelay;

			if (delay < CastDelayMinimum)
			{
				delay = CastDelayMinimum;
			}

			#region Mondain's Legacy
			if (DreadHorn.IsUnderInfluence(m_Caster))
			{
				delay.Add(delay);
			}
			#endregion

			//return TimeSpan.FromSeconds( (double)delay / CastDelayPerSecond );
			return delay;
		}

		public virtual void FinishSequence()
		{
			m_State = SpellState.None;

			if (m_Caster.Spell == this)
			{
				m_Caster.Spell = null;
			}
		}

		public virtual int ComputeKarmaAward()
		{
			return 0;
		}

		public virtual bool CheckSequence()
		{
			int mana = ScaleMana(GetMana());

			if (m_Caster.Deleted || !m_Caster.Alive || m_Caster.Spell != this || m_State != SpellState.Sequencing)
			{
				DoFizzle();
			}
			else if (m_Scroll != null && !(m_Scroll is Runebook) &&
					 (m_Scroll.Amount <= 0 || m_Scroll.Deleted || m_Scroll.RootParent != m_Caster ||
					  (m_Scroll is BaseWand && (((BaseWand)m_Scroll).Charges <= 0 || m_Scroll.Parent != m_Caster))))
			{
				DoFizzle();
			}
			else if (!ConsumeReagents())
			{
				m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502630); // More reagents are needed for this spell.
			}
			else if (m_Caster.Mana < mana)
			{
				m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana for this spell.
			}
			else if (Core.AOS && (m_Caster.Frozen || m_Caster.Paralyzed))
			{
				m_Caster.SendLocalizedMessage(502646); // You cannot cast a spell while frozen.
				DoFizzle();
			}
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).PeacedUntil > DateTime.UtcNow)
			{
				m_Caster.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
				DoFizzle();
			}
			else if (CheckFizzle())
			{
				m_Caster.Mana -= mana;

				if (m_Scroll is SpellScroll)
				{
					m_Scroll.Consume();
				}
					#region SA
				else if (m_Scroll is SpellStone)
				{
					// The SpellScroll check above isn't removing the SpellStones for some reason.
					m_Scroll.Delete();
				}
					#endregion

				else if (m_Scroll is BaseWand)
				{
					((BaseWand)m_Scroll).ConsumeCharge(m_Caster);
					m_Caster.RevealingAction();
				}

				if (m_Scroll is BaseWand)
				{
					bool m = m_Scroll.Movable;

					m_Scroll.Movable = false;

					if (ClearHandsOnCast)
					{
						m_Caster.ClearHands();
					}

					m_Scroll.Movable = m;
				}
				else
				{
					if (ClearHandsOnCast)
					{
						m_Caster.ClearHands();
					}
				}

				int karma = ComputeKarmaAward();

				if (karma != 0)
				{
					Titles.AwardKarma(Caster, karma, true);
				}

				if (TransformationSpellHelper.UnderTransformation(m_Caster, typeof(VampiricEmbraceSpell)))
				{
					bool garlic = false;

					for (int i = 0; !garlic && i < m_Info.Reagents.Length; ++i)
					{
						garlic = (m_Info.Reagents[i] == Reagent.Garlic);
					}

					if (garlic)
					{
						m_Caster.SendLocalizedMessage(1061651); // The garlic burns you!
						AOS.Damage(m_Caster, Utility.RandomMinMax(17, 23), 100, 0, 0, 0, 0);
					}
				}

				return true;
			}
			else
			{
				DoFizzle();
			}

			return false;
		}

		public bool CheckBSequence(Mobile target)
		{
			return CheckBSequence(target, false);
		}

		public bool CheckBSequence(Mobile target, bool allowDead)
		{
			if (!target.Alive && !allowDead)
			{
				m_Caster.SendLocalizedMessage(501857); // This spell won't work on that!
				return false;
			}
			else if (Caster.CanBeBeneficial(target, true, allowDead) && CheckSequence())
			{
				Caster.DoBeneficial(target);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool CheckHSequence(IDamageable target)
		{
			if (!target.Alive || (target is IDamageableItem && !((IDamageableItem)target).CanDamage))
			{
				m_Caster.SendLocalizedMessage(501857); // This spell won't work on that!
				return false;
			}
			else if (Caster.CanBeHarmful(target) && CheckSequence())
			{
				Caster.DoHarmful(target);
				return true;
			}
			else
			{
				return false;
			}
		}

		private class AnimTimer : Timer
		{
			private readonly Spell m_Spell;

			public AnimTimer(Spell spell, int count)
				: base(TimeSpan.Zero, AnimateDelay, count)
			{
				m_Spell = spell;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if (m_Spell.State != SpellState.Casting || m_Spell.m_Caster.Spell != m_Spell)
				{
					Stop();
					return;
				}

				if (!m_Spell.Caster.Mounted && m_Spell.m_Info.Action >= 0)
				{
					if (m_Spell.Caster.Body.IsHuman)
					{
						m_Spell.Caster.Animate(m_Spell.m_Info.Action, 7, 1, true, false, 0);
					}
					else if (m_Spell.Caster.Player && m_Spell.Caster.Body.IsMonster)
					{
						m_Spell.Caster.Animate(12, 7, 1, true, false, 0);
					}
				}

				if (!Running)
				{
					m_Spell.m_AnimTimer = null;
				}
			}
		}

		private class CastTimer : Timer
		{
			private readonly Spell m_Spell;

			public CastTimer(Spell spell, TimeSpan castDelay)
				: base(castDelay)
			{
				m_Spell = spell;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if (m_Spell == null || m_Spell.m_Caster == null)
				{
					return;
				}
				else if (m_Spell.m_State == SpellState.Casting && m_Spell.m_Caster.Spell == m_Spell)
				{
					m_Spell.m_State = SpellState.Sequencing;
					m_Spell.m_CastTimer = null;
					m_Spell.m_Caster.OnSpellCast(m_Spell);
					if (m_Spell.m_Caster.Region != null)
					{
						m_Spell.m_Caster.Region.OnSpellCast(m_Spell.m_Caster, m_Spell);
					}
					m_Spell.m_Caster.NextSpellTime = Core.TickCount + (int)m_Spell.GetCastRecovery().TotalMilliseconds;
						// Spell.NextSpellDelay;

					Target originalTarget = m_Spell.m_Caster.Target;

					m_Spell.OnCast();

					if (m_Spell.m_Caster.Player && m_Spell.m_Caster.Target != originalTarget && m_Spell.Caster.Target != null)
					{
						m_Spell.m_Caster.Target.BeginTimeout(m_Spell.m_Caster, TimeSpan.FromSeconds(30.0));
					}

					m_Spell.m_CastTimer = null;
				}
			}

			public void Tick()
			{
				OnTick();
			}
		}
	}
}