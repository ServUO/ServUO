using System;

namespace Server.Commands.Generic
{
    public class FacetCommandImplementor : BaseCommandImplementor
    {
        public FacetCommandImplementor()
        {
            this.Accessors = new string[] { "Facet" };
            this.SupportRequirement = CommandSupport.Area;
            this.SupportsConditionals = true;
            this.AccessLevel = AccessLevel.GameMaster;
            this.Usage = "Facet <command> [condition]";
            this.Description = "Invokes the command on all appropriate objects within your facet's map bounds. Optional condition arguments can further restrict the set of objects.";
        }

        public override void Process(Mobile from, BaseCommand command, string[] args)
        {
            AreaCommandImplementor impl = AreaCommandImplementor.Instance;

            if (impl == null)
                return;

            Map map = from.Map;

            if (map == null || map == Map.Internal)
                return;

            impl.OnTarget(from, map, Point3D.Zero, new Point3D(map.Width - 1, map.Height - 1, 0), new object[] { command, args });
        }
    }
}