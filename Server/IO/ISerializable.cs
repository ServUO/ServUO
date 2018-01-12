namespace Server
{
	public interface ISerializable
	{
		int TypeReference { get; }
		int SerialIdentity { get; }
		void Serialize(GenericWriter writer);
	}
}