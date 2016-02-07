#region Header
// **********
// ServUO - ContextMenu.cs
// **********
#endregion

#region References
using System.Collections.Generic;
#endregion

namespace Server.ContextMenus
{
	/// <summary>
	///     Represents the state of an active context menu. This includes who opened the menu, the menu's focus object, and a list of
	///     <see
	///         cref="ContextMenuEntry">
	///         entries
	///     </see>
	///     that the menu is composed of.
	///     <seealso cref="ContextMenuEntry" />
	/// </summary>
	public class ContextMenu
	{
		private readonly Mobile m_From;
		private readonly object m_Target;
		private readonly ContextMenuEntry[] m_Entries;

		/// <summary>
		///     Gets the <see cref="Mobile" /> who opened this ContextMenu.
		/// </summary>
		public Mobile From { get { return m_From; } }

		/// <summary>
		///     Gets an object of the <see cref="Mobile" /> or <see cref="Item" /> for which this ContextMenu is on.
		/// </summary>
		public object Target { get { return m_Target; } }

		/// <summary>
		///     Gets the list of <see cref="ContextMenuEntry">entries</see> contained in this ContextMenu.
		/// </summary>
		public ContextMenuEntry[] Entries { get { return m_Entries; } }

		/// <summary>
		///     Instantiates a new ContextMenu instance.
		/// </summary>
		/// <param name="from">
		///     The <see cref="Mobile" /> who opened this ContextMenu.
		///     <seealso cref="From" />
		/// </param>
		/// <param name="target">
		///     The <see cref="Mobile" /> or <see cref="Item" /> for which this ContextMenu is on.
		///     <seealso cref="Target" />
		/// </param>
		public ContextMenu(Mobile from, object target)
		{
			m_From = from;
			m_Target = target;

			var list = new List<ContextMenuEntry>();

			if (target is Mobile)
			{
				((Mobile)target).GetContextMenuEntries(from, list);
			}
			else if (target is Item)
			{
				((Item)target).GetContextMenuEntries(from, list);
			}

			//m_Entries = (ContextMenuEntry[])list.ToArray( typeof( ContextMenuEntry ) );

			m_Entries = list.ToArray();

			for (int i = 0; i < m_Entries.Length; ++i)
			{
				m_Entries[i].Owner = this;
			}
		}

		/// <summary>
		///     Returns true if this ContextMenu requires packet version 2.
		/// </summary>
		public bool RequiresNewPacket
		{
			get
			{
				for (int i = 0; i < m_Entries.Length; ++i)
				{
					if (m_Entries[i].Number < 3000000 || m_Entries[i].Number > 3032767)
					{
						return true;
					}
				}

				return false;
			}
		}
	}
}