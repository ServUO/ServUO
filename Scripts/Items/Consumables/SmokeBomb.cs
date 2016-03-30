#region Header
// **********
// ServUO - SmokeBomb.cs
// **********
#endregion

#region References
using Server.SkillHandlers;
#endregion

namespace Server.Items
{
	public class SmokeBomb : Item
	{
		[Constructable]
		public SmokeBomb()
			: base(0x2808)
		{
			Stackable = Core.ML;
			Weight = 1.0;
		}

		public SmokeBomb(Serial serial)
			: base(serial)
		{ }

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack))
			{
				// The item must be in your backpack to use it.
				from.SendLocalizedMessage(1060640);
			}
			else if (from.Skills.Ninjitsu.Value < 50.0)
			{
				// You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that ability.
				from.SendLocalizedMessage(1063013, "50\tNinjitsu");
			}
			else if (from.NextSkillTime > Core.TickCount)
			{
				// You must wait a few seconds before you can use that item.
				from.SendLocalizedMessage(1070772);
			}
			else if (from.Mana < 10)
			{
				// You don't have enough mana to do that.
				from.SendLocalizedMessage(1049456);
			}
			else
			{
				Hiding.CombatOverride = true;

				if (from.UseSkill(SkillName.Hiding))
				{
					from.Mana -= 10;

					from.FixedParticles(0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot);
					from.PlaySound(0x22F);

					Consume();
				}

				Hiding.CombatOverride = false;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}