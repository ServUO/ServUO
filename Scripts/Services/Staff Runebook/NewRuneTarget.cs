/**************************************
*Script Name: Staff Runebook          *
*Author: Joeku                        *
*For use with RunUO 2.0 RC2           *
*Client Tested with: 6.0.9.2          *
*Version: 1.10                        *
*Initial Release: 11/25/07            *
*Revision Date: 02/04/09              *
**************************************/

using System;
using Server;
using Server.Targeting;

namespace Joeku.SR
{
    public class SR_NewRuneTarget : Target
    {
        public SR_RuneAccount RuneAcc;
        public SR_NewRuneTarget(SR_RuneAccount runeAcc)
            : base(12, true, TargetFlags.None)
        {
            this.RuneAcc = runeAcc;
        }

        protected override void OnTarget(Mobile mob, object targ)
        {
            Point3D loc = new Point3D(0, 0, 0);
            if (targ is LandTarget)
                loc = (targ as LandTarget).Location;
            else if (targ is StaticTarget)
                loc = (targ as StaticTarget).Location;
            else if (targ is Mobile)
                loc = (targ as Mobile).Location;
            else if (targ is Item)
                loc = (targ as Item).Location;

            mob.SendMessage("Enter a description:");
            mob.Prompt = new SR_NewRunePrompt(this.RuneAcc, loc, mob.Map);
        }
    }
}