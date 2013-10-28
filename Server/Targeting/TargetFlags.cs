#region Header
// **********
// ServUO - TargetFlags.cs
// **********
#endregion

namespace Server.Targeting
{
	public enum TargetFlags : byte
	{
		None = 0x00,
		Harmful = 0x01,
		Beneficial = 0x02,
	}
}