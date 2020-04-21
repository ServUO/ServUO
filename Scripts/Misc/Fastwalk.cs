using System;

namespace Server.Misc
{
    // This fastwalk detection is no longer required
    // As of B36 PlayerMobile implements movement packet throttling which more reliably controls movement speeds
    public class Fastwalk
    {
        private static readonly int MaxSteps = 4;// Maximum number of queued steps until fastwalk is detected
        private static readonly bool Enabled = false;// Is fastwalk detection enabled?
        private static readonly bool UOTDOverride = false;// Should UO:TD clients not be checked for fastwalk?
        private static readonly AccessLevel AccessOverride = AccessLevel.Decorator;// Anyone with this or higher access level is not checked for fastwalk
        public static void Initialize()
        {
            Mobile.FwdMaxSteps = MaxSteps;
            Mobile.FwdEnabled = Enabled;
            Mobile.FwdUOTDOverride = UOTDOverride;
            Mobile.FwdAccessOverride = AccessOverride;

            if (Enabled)
                EventSink.FastWalk += OnFastWalk;
        }

        public static void OnFastWalk(FastWalkEventArgs e)
        {
            e.Blocked = true;//disallow this fastwalk
            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine("Client: {0}: Fast movement detected! (name={1})", e.NetState, e.NetState.Mobile.Name);
            Utility.PopColor();
        }
    }
}
