namespace Server
{
    public interface IDynamicEnum
    {
        string Value { get; set; }
        string[] Values { get; }
        bool IsValid { get; }
    }
}