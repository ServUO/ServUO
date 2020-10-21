namespace Server.Items
{
    public static class ItemFlags
    {
        private const int StealableFlag = 0x00200000;
        private const int TakenFlag = 0x00100000;

        public static void SetStealable(Item target, bool value)
        {
            target?.SetSavedFlag(StealableFlag, value);
        }
        public static bool GetStealable(Item target)
        {
            return target != null && target.GetSavedFlag(StealableFlag);
        }

        public static void SetTaken(Item target, bool value)
        {
            target?.SetSavedFlag(TakenFlag, value);
        }
        public static bool GetTaken(Item target)
        {
            return target != null && target.GetSavedFlag(TakenFlag);
        }
    }
}
