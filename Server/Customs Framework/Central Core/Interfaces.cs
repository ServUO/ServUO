#region Header
// **********
// ServUO - Interfaces.cs
// **********
#endregion

#region References
using System;
#endregion

namespace CustomsFramework
{
	public interface ICustomsEntry
	{
		CustomSerial Serial { get; }
		int TypeID { get; }
		long Position { get; }
		int Length { get; }
	}

	public interface ICustomsEntity : IComparable, IComparable<ICustomsEntity>
	{
		CustomSerial Serial { get; }
		string Name { get; }
		void Delete();

		void Prep();
	}
}