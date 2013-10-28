using System;

namespace Server
{
    public delegate MoveResult MoveMethod(Direction d);

    public enum MoveResult
    {
        BadState,
        Blocked,
        Success,
        SuccessAutoTurn
    }
}