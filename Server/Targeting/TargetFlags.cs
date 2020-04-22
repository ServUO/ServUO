using System;

namespace Server.Targeting
{
    [Flags]
    public enum TargetFlags : byte
    {
        None = 0x00,
        Harmful = 0x01,
        Beneficial = 0x02,
    }
}
