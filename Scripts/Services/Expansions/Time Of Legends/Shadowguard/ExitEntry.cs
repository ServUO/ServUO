using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Engines.Shadowguard
{
    public class ExitEntry : ContextMenuEntry
    {
        private Mobile _From;

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

                if(instance.Encounter != null)
                    instance.Encounter.CheckPlayerStatus(_From);
            }
        }
    }
}