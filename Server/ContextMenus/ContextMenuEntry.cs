#region Header
// **********
// ServUO - ContextMenuEntry.cs
// **********
#endregion

#region References
using Server.Network;
#endregion

namespace Server.ContextMenus
{
	/// <summary>
	///     Represents a single entry of a <see cref="ContextMenu">context menu</see>.
	///     <seealso cref="ContextMenu" />
	/// </summary>
	public class ContextMenuEntry
	{
		/// <summary>
		///     Gets or sets additional <see cref="CMEFlags">flags</see> used in client communication.
		/// </summary>
		public CMEFlags Flags { get; set; }

		/// <summary>
		///     Gets or sets the <see cref="ContextMenu" /> that owns this entry.
		/// </summary>
		public ContextMenu Owner { get; set; }

		/// <summary>
		///     Gets or sets the localization number containing the name of this entry.
		/// </summary>
		public int Number { get; set; }

		/// <summary>
		///     Gets or sets the maximum range at which this entry may be used, in tiles. A value of -1 signifies no maximum range.
		/// </summary>
		public int Range { get; set; }

		/// <summary>
		///     Gets or sets the color for this entry. Format is A1-R5-G5-B5.
		/// </summary>
		public int Color { get; set; }

		/// <summary>
		///     Gets or sets whether this entry is enabled. When false, the entry will appear in a gray hue and <see cref="OnClick" /> will never be invoked.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		///     Gets a value indicating if non local use of this entry is permitted.
		/// </summary>
		public virtual bool NonLocalUse { get { return false; } }

		/// <summary>
		///     Instantiates a new ContextMenuEntry with a given <see cref="Number">localization number</see> (
		///     <paramref
		///         name="number" />
		///     ). No <see cref="Range">maximum range</see> is used.
		/// </summary>
		/// <param name="number">
		///     The localization number containing the name of this entry.
		///     <seealso cref="Number" />
		/// </param>
		public ContextMenuEntry(int number)
			: this(number, -1)
		{ }

		/// <summary>
		///     Instantiates a new ContextMenuEntry with a given <see cref="Number">localization number</see> (
		///     <paramref
		///         name="number" />
		///     ) and <see cref="Range">maximum range</see> (<paramref name="range" />).
		/// </summary>
		/// <param name="number">
		///     The localization number containing the name of this entry.
		///     <seealso cref="Number" />
		/// </param>
		/// <param name="range">
		///     The maximum range at which this entry can be used.
		///     <seealso cref="Range" />
		/// </param>
		public ContextMenuEntry(int number, int range)
		{
			if (number <= 0x7FFF) // Legacy code support
			{
				Number = 3000000 + number;
			}
			else
			{
				Number = number;
			}

			Range = range;
			Enabled = true;
			Color = 0xFFFF;
		}

		/// <summary>
		///     Overridable. Virtual event invoked when the entry is clicked.
		/// </summary>
		public virtual void OnClick()
		{ }
	}
}