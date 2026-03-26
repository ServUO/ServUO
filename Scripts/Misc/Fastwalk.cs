using System;

namespace Server.Misc
{
    public class Fastwalk
    {
        private static readonly int MaxSteps = 2;// Maximum number of queued steps until fastwalk is detected
        private static readonly bool Enabled = true;// Is fastwalk detection enabled?
        private static readonly bool UOTDOverride = false;// Should UO:TD clients not be checked for fastwalk?
        private static readonly AccessLevel AccessOverride = AccessLevel.GameMaster;// Anyone with this or higher access level is not checked for fastwalk
        public static void Initialize()
        {
            Mobile.FwdMaxSteps = MaxSteps;
            Mobile.FwdEnabled = Enabled;
            Mobile.FwdUOTDOverride = UOTDOverride;
            Mobile.FwdAccessOverride = AccessOverride;

            // Tighten movement delays to prevent speed hacking
            // These are minimum milliseconds between steps
            Mobile.WalkFoot = 400;
            Mobile.RunFoot = 200;
            Mobile.WalkMount = 200;
            Mobile.RunMount = 100;

            if (Enabled)
                EventSink.FastWalk += new FastWalkEventHandler(OnFastWalk);
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
