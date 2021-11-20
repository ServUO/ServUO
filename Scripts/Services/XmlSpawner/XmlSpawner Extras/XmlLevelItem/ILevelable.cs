using System;

namespace Server.Items
{
	/// <summary>
	/// Summary description for ILevelable.
	/// </summary>
	public interface ILevelable
	{
		int Experience{ get; set; }
		int Level{ get; set; }
		int Points{ get; set; }
		int MaxLevel { get; set; }
	}
}
