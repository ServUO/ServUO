using Server.Commands.Generic;
using Server.Targeting;

namespace Server.Targets
{
    public class PickMoveTarget : Target
    {
        public PickMoveTarget()
            : base(-1, false, TargetFlags.None)
        {
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (!BaseCommand.IsAccessible(from, o))
            {
                from.SendLocalizedMessage(500447); // That is not accessible.
                return;
            }

            if (o is Item)
            {
                from.SendMessage("Where do you wish to move the item?");
                from.Target = new MoveTarget(o);
            }
            else if (o is Mobile)
            {
                from.SendMessage("Where do you wish to move the mobile?");
                from.Target = new MoveTarget(o);
            }
            else
            {
                from.SendMessage("Invalid Object.");
            }
        }
    }
}
