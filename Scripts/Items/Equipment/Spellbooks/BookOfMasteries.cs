using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Spells.SkillMasteries;
using Server.Spells.Spellweaving;

namespace Server.Items
{
	[Flipable(0x225A, 0x225B)]
	public class BookOfMasteries : Spellbook
	{
		public override SpellbookType SpellbookType => SpellbookType.SkillMasteries;
		public override int BookOffset => 700;
		public override int BookCount => 45;

		private ulong _Content;

		[CommandProperty(AccessLevel.GameMaster)]
		public new ulong Content
		{
			get
			{
				return _Content;
			}
			set
			{
				if (_Content != value)
				{
					_Content = value;

					InvalidateProperties();
				}
			}
		}

		[Constructable]
		public BookOfMasteries() : this(0x1FFFFFFFFFFF)
		{
		}

		[Constructable]
		public BookOfMasteries(ulong content) : base(content, 0x225A)
		{
			_Content = content;
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);
			var isDiscordanceMasteryActive = from.Skills.CurrentMastery == SkillName.Discordance;

			var switchMasteryClilocReference = 1151948;

			// Documentation says you can't change while the skill is active
			// I don't know we really need that bit, seems like a qol improvement not having it
			List<int> clilocReferences = new List<int>
			{
				switchMasteryClilocReference
			};

			if (isDiscordanceMasteryActive)
			{
				clilocReferences.AddRange(new List<int>
				{
					1151800,
					1151801,
					1151802,
					1151803,
					1151804
				});
			}

			var currentElementType = GetDespairElementType(from);

			clilocReferences.ForEach(clilocReference =>
			{
				var isSwitchMastery = clilocReference == switchMasteryClilocReference;

				SimpleContextMenuEntry menuEntry;

				if (isSwitchMastery)
				{
					menuEntry = new SimpleContextMenuEntry(from, clilocReference, m =>
					{
						if (m is PlayerMobile && IsChildOf(m.Backpack) && CheckCooldown(m))
							BaseGump.SendGump(new MasterySelectionGump(m as PlayerMobile, this));
					});
				} else
				{
					var elementType = GetElementType(clilocReference);
					var elementName = GetElementName(elementType);
					var isEnabled = elementType != currentElementType;
					menuEntry = new SimpleContextMenuEntry(from, clilocReference, m =>
					{
						if (m is PlayerMobile && IsChildOf(m.Backpack) && CheckCooldown(m))
						{
							UpdateDespairElementType(m, elementType);
							m.SendMessage($"Despair Element Type set to: {elementName}");
							PlaySwitchMasteryEffects(from);
						}
					}, -1, isEnabled);
				}

				if (!IsChildOf(from.Backpack) || !CheckCooldown(from))
					menuEntry.Enabled = false;

				list.Add(menuEntry);
			});
		}

		private void PlaySwitchMasteryEffects(Mobile from)
		{
			from.PlaySound(0x0F5);
			from.PlaySound(0x1ED);

			from.FixedParticles(0x375A, 1, 30, 9966, 33, 2, EffectLayer.Head);
			from.FixedParticles(0x37B9, 1, 30, 9502, 43, 3, EffectLayer.Head);
		}

		private string GetElementName(ElementType type)
		{
			switch(type)
			{
				case ElementType.Fire:
					return "Fire";
				case ElementType.Poison:
					return "Poison";
				case ElementType.Energy:
					return "Energy";
				case ElementType.Cold:
					return "Cold";
				case ElementType.Physical:
					return "Physical";
				default:
					return "Physical";
			}
		}

		public static ElementType GetElementType(int clilocReference)
		{
			switch (clilocReference)
			{
				case 1151800:
					return ElementType.Physical;
				case 1151801:
					return ElementType.Fire;
				case 1151802:
					return ElementType.Cold;
				case 1151803:
					return ElementType.Poison;
				case 1151804:
					return ElementType.Energy;
				default:
					return ElementType.Physical;
			}
		}

		private static Dictionary<Mobile, DateTime> m_Cooldown = new Dictionary<Mobile, DateTime>();
		private static Dictionary<Mobile, ElementType> m_DespairElementType = new Dictionary<Mobile, ElementType>();

		public static void UpdateDespairElementType(Mobile from, ElementType type)
		{
			m_DespairElementType[from] = type;
		}

		public static ElementType GetDespairElementType(Mobile from)
		{
			ElementType element;
			if (m_DespairElementType.TryGetValue(from, out element))
				return element;
			else
				return ElementType.Physical;
		}

		public static void AddToCooldown(Mobile from)
		{
			if (m_Cooldown == null)
				m_Cooldown = new Dictionary<Mobile, DateTime>();

			m_Cooldown[from] = DateTime.UtcNow + TimeSpan.FromMinutes(10);
		}

		public static bool CheckCooldown(Mobile from)
		{
			if (from.AccessLevel > AccessLevel.Player)
				return true;

			if (m_Cooldown != null && m_Cooldown.ContainsKey(from))
			{
				if (m_Cooldown[from] < DateTime.UtcNow)
				{
					m_Cooldown.Remove(from);
					return true;
				}

				return false;
			}

			return true;
		}

		public override void AddProperty(ObjectPropertyList list)
		{
			base.AddProperty(list);

			if (RootParent is Mobile)
			{
				SkillName sk = ((Mobile)RootParent).Skills.CurrentMastery;

				if (sk > 0)
				{
					list.Add(MasteryInfo.GetLocalization(sk));

					if (sk == SkillName.Spellweaving)
					{
						list.Add(1060485, ArcanistSpell.GetMasteryFocusLevel((Mobile)RootParent).ToString()); // strength bonus ~1_val~
					}
				}
			}
		}

		public BookOfMasteries(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(1); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}
}