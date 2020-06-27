using Server.ContextMenus;

namespace Server.Engines.Shadowguard
{
    public class ExitEntry : ContextMenuEntry
    {
        private readonly Mobile _From;

        public ExitEntry(Mobile from)
            : base(1156287, -1) // Exit Shadowguard
        {
            _From = from;
        }

        public override void OnClick()
        {
            ShadowguardInstance instance = ShadowguardController.GetInstance(_From.Location, _From.Map);

            if (instance != null && instance.Region.Contains(_From.Location))
            {
                ShadowguardEncounter.MovePlayer(_From, ShadowguardController.Instance.KickLocation);

                if (instance.Encounter != null)
                    instance.Encounter.CheckPlayerStatus(_From);
            }
        }
    }

    public class ExitQueueEntry : ContextMenuEntry
    {
        private readonly Mobile _From;
        private readonly ShadowguardController _Controller;

        public ExitQueueEntry(Mobile from, ShadowguardController controller)
            : base(1156247, 12) // Exit Shadowguard Queues
        {
            _From = from;
            _Controller = controller;

            Enabled = controller.IsInQueue(from);
        }

        public override void OnClick()
        {
            if (_Controller != null && _Controller.IsInQueue(_From))
            {
                if (_Controller.RemoveFromQueue(_From))
                    _From.SendLocalizedMessage(1156248); // You have been removed from all Shadowguard queues

            }
        }
    }
}
