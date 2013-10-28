#region Header
// **********
// ServUO - SaveData.cs
// **********
#endregion

#region References
using System;

using Server;
#endregion

namespace CustomsFramework
{
	public class SaveData : ICustomsEntity, IComparable<SaveData>, ISerializable
	{
		#region CompareTo
		public int CompareTo(ICustomsEntity other)
		{
			if (other == null)
			{
				return -1;
			}

			return _Serial.CompareTo(other.Serial);
		}

		public int CompareTo(SaveData other)
		{
			return CompareTo((ICustomsEntity)other);
		}

		public int CompareTo(object other)
		{
			if (other == null || other is ICustomsEntity)
			{
				return CompareTo((ICustomsEntity)other);
			}

			throw new ArgumentException();
		}
		#endregion

		internal int _TypeID;

		int ISerializable.TypeReference { get { return _TypeID; } }

		int ISerializable.SerialIdentity { get { return _Serial; } }

		private bool _Deleted;
		private CustomSerial _Serial;

		[CommandProperty(AccessLevel.Developer)]
		public bool Deleted { get { return _Deleted; } set { _Deleted = value; } }

		[CommandProperty(AccessLevel.Developer)]
		public CustomSerial Serial { get { return _Serial; } set { _Serial = value; } }

		public virtual string Name { get { return @"Save Data"; } }

		public SaveData(CustomSerial serial)
		{
			_Serial = serial;

			Type dataType = GetType();
			_TypeID = World._DataTypes.IndexOf(dataType);

			if (_TypeID == -1)
			{
				World._DataTypes.Add(dataType);
				_TypeID = World._DataTypes.Count - 1;
			}
		}

		public SaveData()
		{
			_Serial = CustomSerial.NewCustom;

			World.AddData(this);

			Type dataType = GetType();
			_TypeID = World._DataTypes.IndexOf(dataType);

			if (_TypeID == -1)
			{
				World._DataTypes.Add(dataType);
				_TypeID = World._DataTypes.Count - 1;
			}
		}

		public virtual void Prep()
		{ }

		public virtual void Delete()
		{ }

		public virtual void Serialize(GenericWriter writer)
		{
			writer.WriteVersion(0);

			// Version 0
			writer.Write(_Deleted);
		}

		public virtual void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						_Deleted = reader.ReadBool();
						break;
					}
			}
		}
	}
}