#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;

using Server;

namespace Arya.Abay
{
	/// <summary>
	/// Delegate for targeting callbacks
	/// </summary>
	public delegate void AbayTargetCallback( Mobile from, object targeted );

	/// <summary>
	/// Delegate for gumps navigation
	/// </summary>
	public delegate void AbayGumpCallback( Mobile user );
}
