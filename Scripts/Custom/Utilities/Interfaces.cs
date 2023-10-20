#region AuthorHeader
//
//	Interfaces version 1.0 - utilities version 2.0, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Interfaces
{
	//
	// This interface is implemented by clients of ShrinkTarget allowing the
	// ShrinkTarget to adjust the charges of tools without requiring they have the same base class.
	//

	public interface IShrinkTool
	{
		int ShrinkCharges { get; set; }
	}

	//
	// Used by the auction system to validate the pet referred to by a shrink item.
	//

	public interface IShrinkItem
	{
		BaseCreature ShrunkenPet{ get; }
	}

	//
	// Allows BaseEvo and BaseEvoMount to be handled without special-casing.
	//

	public interface IEvoCreature
	{
		TimeSpan RemainingTerm { get; }
		string Breed { get; }
		bool Pregnant { get; set; }
		bool HasEgg { get; set; }
		bool CanHue { get; }
		Type GetEvoDustType();
		int Ep { get; }
		int Stage { get; }
		void OnShrink( IShrinkItem shrinkItem );
	}

	//
	// For simple testing of whether an Evo is a standard evo or a guardian.
	//

	public interface IEvoGuardian
	{
	}

}