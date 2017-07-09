#region Header
// **********
// ServUO - BaseFamiliar.cs
// **********
#endregion

#region References
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public abstract class BaseFamiliar : BaseCreature
	{
		public BaseFamiliar()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, -1, -1)
		{ }

		public BaseFamiliar(Serial serial)
			: base(serial)
		{ }

		public override bool BardImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override bool Commandable { get { return false; } }
		public override bool PlayerRangeSensitive { get { return false; } }
		public virtual bool AttacksMastersTarget { get { return false; } }

		public override void OnThink()
		{
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from.Alive && Controlled && from == ControlMaster && from.InRange(this, 14))
			{
				list.Add(new ReleaseEntry(from, this));
			}
		}

		public virtual void BeginRelease(Mobile from)
		{
			if (!Deleted && Controlled && from == ControlMaster && from.CheckAlive())
			{
				EndRelease(from);
			}
		}

		public virtual void EndRelease(Mobile from)
		{
			if (from == null || (!Deleted && Controlled && from == ControlMaster && from.CheckAlive()))
			{
				Effects.SendLocationParticles(
					EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 2100, 3, 5042, 0);
				PlaySound(0x201);
				Delete();
			}
		}

		public virtual void DropPackContents()
		{
			Map map = Map;
			Container pack = Backpack;

			if (map != null && map != Map.Internal && pack != null)
			{
				var list = new List<Item>(pack.Items);

				for (int i = 0; i < list.Count; ++i)
				{
					list[i].MoveToWorld(Location, map);
				}
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

			ValidationQueue<BaseFamiliar>.Add(this);
		}

		public void Validate()
		{
			DropPackContents();
			Delete();
		}

		private class ReleaseEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseFamiliar m_Familiar;

			public ReleaseEntry(Mobile from, BaseFamiliar familiar)
				: base(6118, 14)
			{
				m_From = from;
				m_Familiar = familiar;
			}

			public override void OnClick()
			{
				if (!m_Familiar.Deleted && m_Familiar.Controlled && m_From == m_Familiar.ControlMaster && m_From.CheckAlive())
				{
					m_Familiar.BeginRelease(m_From);
				}
			}
		}
	}
}
