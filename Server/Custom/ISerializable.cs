namespace Server
{
    public partial interface ISerializable
    {
        void SerializeSW(GenericWriter writer);
    }
}