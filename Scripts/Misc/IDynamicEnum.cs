using System;

namespace Server
{
    public interface IDynamicEnum
    {
        String Value { get; set; }
        String[] Values { get; }
        Boolean IsValid { get; }
    }
}
