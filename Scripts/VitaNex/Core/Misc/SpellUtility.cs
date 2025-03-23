#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Seventh;
#endregion

namespace VitaNex
{
	public static class SpellUtility
	{
		public static string[] CircleNames =
		{
			"First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Necromancy", "Chivalry", "Bushido",
			"Ninjitsu", "Spellweaving", "Mystic", "SkillMasteries", "Racial"
		};

		public static Dictionary<Type, SpellInfo> SpellsInfo { get; private set; }
		public static Dictionary<Type, int> ItemSpellIcons { get; private set; }
		public static Dictionary<string, Dictionary<Type, SpellInfo>> TreeStructure { get; private set; }

		static SpellUtility()
		{
			SpellsInfo = new Dictionary<Type, SpellInfo>();
			ItemSpellIcons = new Dictionary<Type, int>();
			TreeStructure = new Dictionary<string, Dictionary<Type, SpellInfo>>();
		}

		[CallPriority(Int16.MaxValue)]
		public static void Initialize()
		{
			for (int i = 0, j = 8320; i < SpellRegistry.Types.Length; i++, j++)
			{
				var s = SpellRegistry.NewSpell(i, null, null);

				if (s == null)
				{
					continue;
				}

				var o = s.Info;

				if (o == null)
				{
					continue;
				}

				var t = SpellRegistry.Types[i] ?? s.GetType();

				SpellsInfo[t] = new SpellInfo(o.Name, o.Mantra, o.Action, o.LeftHandEffect, o.RightHandEffect, o.AllowTown, o.Reagents);

				//if (SpellIcons.IsItemIcon(j))
				//{
				//	ItemSpellIcons[t] = j;
				//}
			}

			InvalidateTreeStructure();
		}

		public static void InvalidateTreeStructure()
		{
			TreeStructure.Clear();

			foreach (var c in CircleNames)
			{
				var d = new Dictionary<Type, SpellInfo>();

				foreach (var o in SpellsInfo.Where(o => Insensitive.StartsWith(o.Key.FullName, "Server.Spells." + c)))
				{
					d[o.Key] = o.Value;
				}

				TreeStructure[c] = d;
			}
		}

		public static SpellInfo GetSpellInfo<TSpell>() where TSpell : ISpell
		{
			return GetSpellInfo(typeof(TSpell));
		}

		public static SpellInfo GetSpellInfo(int spellID)
		{
			return SpellRegistry.Types.InBounds(spellID) ? GetSpellInfo(SpellRegistry.Types[spellID]) : null;
		}

		public static SpellInfo GetSpellInfo(ISpell s)
		{
			return s != null ? GetSpellInfo(s.GetType()) : null;
		}

		public static SpellInfo GetSpellInfo(Type type)
		{
			return SpellsInfo.GetValue(type);
		}

		public static string GetSpellName<TSpell>() where TSpell : ISpell
		{
			return GetSpellName(typeof(TSpell));
		}

		public static string GetSpellName(int spellID)
		{
			return SpellRegistry.Types.InBounds(spellID) ? GetSpellName(SpellRegistry.Types[spellID]) : String.Empty;
		}

		public static string GetSpellName(ISpell s)
		{
			return s != null ? GetSpellName(s.GetType()) : String.Empty;
		}

		public static string GetSpellName(Type type)
		{
			if (type == null)
			{
				return String.Empty;
			}

			var o = GetSpellInfo(type);

			return o != null ? o.Name : String.Empty;
		}

		public static int GetItemIcon<TSpell>() where TSpell : ISpell
		{
			return GetItemIcon(typeof(TSpell));
		}

		public static int GetItemIcon(int spellID)
		{
			return SpellRegistry.Types.InBounds(spellID) ? GetItemIcon(SpellRegistry.Types[spellID]) : 0;
		}

		public static int GetItemIcon(ISpell s)
		{
			return s != null ? GetItemIcon(s.GetType()) : 0;
		}

		public static int GetItemIcon(Type type)
		{
			return ItemSpellIcons.GetValue(type);
		}

		public static string GetCircleName<TSpell>() where TSpell : ISpell
		{
			return GetCircleName(typeof(TSpell));
		}

		public static string GetCircleName(int spellID)
		{
			return SpellRegistry.Types.InBounds(spellID) ? GetCircleName(SpellRegistry.Types[spellID]) : String.Empty;
		}

		public static string GetCircleName(ISpell s)
		{
			return s != null ? GetCircleName(s.GetType()) : String.Empty;
		}

		public static string GetCircleName(Type type)
		{
			if (type == null)
			{
				return String.Empty;
			}

			if (TreeStructure == null || TreeStructure.Count == 0)
			{
				InvalidateTreeStructure();
			}

			if (TreeStructure == null || TreeStructure.Count == 0)
			{
				return String.Empty;
			}

			return TreeStructure.FirstOrDefault(o => o.Value.ContainsKey(type)).Key ?? String.Empty;
		}

		public static int GetCircle<TSpell>() where TSpell : ISpell
		{
			return GetCircle(typeof(TSpell));
		}

		public static int GetCircle(int spellID)
		{
			return SpellRegistry.Types.InBounds(spellID) ? GetCircle(SpellRegistry.Types[spellID]) : 0;
		}

		public static int GetCircle(ISpell s)
		{
			return s != null ? GetCircle(s.GetType()) : 0;
		}

		public static int GetCircle(Type type)
		{
			if (type == null)
			{
				return 0;
			}

			var circle = GetCircleName(type);

			return !String.IsNullOrWhiteSpace(circle) ? CircleNames.IndexOf(circle) + 1 : 0;
		}

		public static IEnumerable<Type> FindCircleSpells(int circleID)
		{
			if (!CircleNames.InBounds(--circleID))
			{
				return Enumerable.Empty<Type>();
			}

			return FindCircleSpells(CircleNames[circleID]);
		}

		public static IEnumerable<Type> FindCircleSpells(string circle)
		{
			if (SpellsInfo == null || SpellsInfo.Count == 0)
			{
				yield break;
			}

			if (TreeStructure == null || TreeStructure.Count == 0)
			{
				InvalidateTreeStructure();
			}

			if (TreeStructure == null || TreeStructure.Count == 0)
			{
				yield break;
			}

			circle = TreeStructure.Keys.FirstOrDefault(c => Insensitive.EndsWith(circle, c));

			if (circle == null)
			{
				yield break;
			}

			var spells = TreeStructure[circle];

			if (spells == null || spells.Count == 0)
			{
				yield break;
			}

			foreach (var t in spells.Keys)
			{
				yield return t;
			}
		}

		public static bool AddStatOffset(Mobile m, StatType type, int offset, TimeSpan duration)
		{
			if (offset > 0)
			{
				return AddStatBonus(m, m, type, offset, duration);
			}

			if (offset < 0)
			{
				return AddStatCurse(m, m, type, -offset, duration);
			}

			return true;
		}

		public static bool RemoveStatBonus(Mobile m, StatType type)
		{
			if (type == StatType.All)
			{
				return RemoveStatBonus(m, StatType.Str) | RemoveStatBonus(m, StatType.Dex) | RemoveStatBonus(m, StatType.Int);
			}

#if ServUO
			var name = String.Format("[Magic] {0} Buff", type);
#else
			var name = String.Format("[Magic] {0} Offset", type);
#endif

			var mod = m.GetStatMod(name);

			if (mod != null && mod.Offset >= 0)
			{
				m.RemoveStatMod(mod.Name);
				return true;
			}

			return false;
		}

		public static bool AddStatBonus(Mobile caster, Mobile target, StatType type, int bonus, TimeSpan duration)
		{
			if (type == StatType.All)
			{
				return AddStatBonus(caster, target, StatType.Str, bonus, duration) | AddStatBonus(caster, target, StatType.Dex, bonus, duration) | AddStatBonus(caster, target, StatType.Int, bonus, duration);
			}

			var offset = bonus;

#if ServUO
			var name = String.Format("[Magic] {0} Buff", type);
#else
			var name = String.Format("[Magic] {0} Offset", type);
#endif

			var mod = target.GetStatMod(name);

			if (mod != null && mod.Offset < 0)
			{
				target.AddStatMod(new StatMod(type, name, mod.Offset + offset, duration));
				return true;
			}

			if (mod == null || mod.Offset < offset)
			{
				target.AddStatMod(new StatMod(type, name, offset, duration));
				return true;
			}

			return false;
		}

		public static bool RemoveStatCurse(Mobile m, StatType type)
		{
			if (type == StatType.All)
			{
				var success = RemoveStatCurse(m, StatType.Str);
				success = RemoveStatCurse(m, StatType.Dex) || success;
				success = RemoveStatCurse(m, StatType.Int) || success;
				return success;
			}

#if ServUO
			var name = String.Format("[Magic] {0} Curse", type);
#else
			var name = String.Format("[Magic] {0} Offset", type);
#endif

			var mod = m.GetStatMod(name);

			if (mod != null && mod.Offset <= 0)
			{
				m.RemoveStatMod(mod.Name);
				return true;
			}

			return false;
		}

		public static bool AddStatCurse(Mobile caster, Mobile target, StatType type, int curse, TimeSpan duration)
		{
			if (type == StatType.All)
			{
				var success = AddStatCurse(caster, target, StatType.Str, curse, duration);
				success = AddStatCurse(caster, target, StatType.Dex, curse, duration) || success;
				success = AddStatCurse(caster, target, StatType.Int, curse, duration) || success;
				return success;
			}

			var offset = -curse;

#if ServUO
			var name = String.Format("[Magic] {0} Curse", type);
#else
			var name = String.Format("[Magic] {0} Offset", type);
#endif

			var mod = target.GetStatMod(name);

			if (mod != null && mod.Offset > 0)
			{
				target.AddStatMod(new StatMod(type, name, mod.Offset + offset, duration));
				return true;
			}

			if (mod == null || mod.Offset > offset)
			{
				target.AddStatMod(new StatMod(type, name, offset, duration));
				return true;
			}

			return false;
		}

		public static void NegateAllEffects(Mobile target)
		{
			NegateEffects(target, true, true, true, true);
		}

		public static void NegateEffects(Mobile target, bool curses, bool buffs, bool damage, bool morph)
		{
			if (target == null)
			{
				return;
			}

			if (damage)
			{
				if (target.Poisoned)
				{
					var p = target.Poison;

					target.Poison = null;

					target.OnCured(target, p);
				}

				target.Frozen = false;
				target.Paralyzed = false;

				target.SetPropertyValue("Asleep", false);

				BuffInfo.RemoveBuff(target, BuffIcon.Paralyze);
				BuffInfo.RemoveBuff(target, BuffIcon.Sleep);
			}

			if (buffs)
			{
				ReactiveArmorSpell.EndArmor(target);
				MagicReflectSpell.EndReflect(target);
			}

			if (curses)
			{
				#region Pain Spike
				if (typeof(PainSpikeSpell).GetFieldValue("m_Table", out IDictionary table) && table.Contains(target))
				{
					if (table[target] is Timer t)
					{
						t.Stop();
					}

					table.Remove(target);

					BuffInfo.RemoveBuff(target, BuffIcon.PainSpike);
				}
				#endregion

				CurseSpell.RemoveEffect(target);
				EvilOmenSpell.TryEndEffect(target);
				StrangleSpell.RemoveCurse(target);
				CorpseSkinSpell.RemoveCurse(target);
				BloodOathSpell.RemoveCurse(target);
				MindRotSpell.ClearMindRotScalar(target);
			}

			if (damage)
			{
				MortalStrike.EndWound(target);
				BleedAttack.EndBleed(target, target.Alive);
			}

			if (morph)
			{
				AnimalForm.RemoveContext(target, true);

				PolymorphSpell.StopTimer(target);
				IncognitoSpell.StopTimer(target);

				target.Send(SpeedControl.Disable);

				target.EndAction(typeof(PolymorphSpell));
				target.EndAction(typeof(IncognitoSpell));

				BuffInfo.RemoveBuff(target, BuffIcon.AnimalForm);
				BuffInfo.RemoveBuff(target, BuffIcon.Polymorph);
				BuffInfo.RemoveBuff(target, BuffIcon.Incognito);
			}

			if (buffs)
			{
				RemoveStatBonus(target, StatType.All);
			}

			if (curses)
			{
				RemoveStatCurse(target, StatType.All);
			}
		}
	}
}
