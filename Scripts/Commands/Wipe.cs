using System;
using System.Collections.Generic;
using Server.Items;
using Server.Multis;

namespace Server.Commands
{
    public class Wipe
    {
        [Flags]
        public enum WipeType
        {
            Items = 0x01,
            Mobiles = 0x02,
            Multis = 0x04,
            All = Items | Mobiles | Multis
        }
        public static void Initialize()
        {
            CommandSystem.Register("Wipe", AccessLevel.GameMaster, new CommandEventHandler(WipeAll_OnCommand));
            CommandSystem.Register("WipeItems", AccessLevel.GameMaster, new CommandEventHandler(WipeItems_OnCommand));
            CommandSystem.Register("WipeNPCs", AccessLevel.GameMaster, new CommandEventHandler(WipeNPCs_OnCommand));
            CommandSystem.Register("WipeMultis", AccessLevel.GameMaster, new CommandEventHandler(WipeMultis_OnCommand));
        }

        public static void BeginWipe(Mobile from, WipeType type)
        {
            BoundingBoxPicker.Begin(from, new BoundingBoxCallback(WipeBox_Callback), type);
        }

        public static void DoWipe(Mobile from, Map map, Point3D start, Point3D end, WipeType type)
        {
            CommandLogging.WriteLine(from, "{0} {1} wiping from {2} to {3} in {5} ({4})", from.AccessLevel, CommandLogging.Format(from), start, end, type, map);

            bool mobiles = ((type & WipeType.Mobiles) != 0);
            bool multis = ((type & WipeType.Multis) != 0);
            bool items = ((type & WipeType.Items) != 0);

            List<IEntity> toDelete = new List<IEntity>();

            Rectangle2D rect = new Rectangle2D(start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1);

            IPooledEnumerable eable;

            if ((items || multis) && mobiles)
                eable = map.GetObjectsInBounds(rect);
            else if (items || multis)
                eable = map.GetItemsInBounds(rect);
            else if (mobiles)
                eable = map.GetMobilesInBounds(rect);
            else
                return;

            foreach (IEntity obj in eable)
            {
                if (items && (obj is Item) && !((obj is BaseMulti) || (obj is HouseSign)))
                    toDelete.Add(obj);
                else if (multis && (obj is BaseMulti))
                    toDelete.Add(obj);
                else if (mobiles && (obj is Mobile) && !((Mobile)obj).Player)
                    toDelete.Add(obj);
            }

            eable.Free();

            for (int i = 0; i < toDelete.Count; ++i)
                toDelete[i].Delete();
        }

        [Usage("Wipe")]
        [Description("Wipes all items and npcs in a targeted bounding box.")]
        private static void WipeAll_OnCommand(CommandEventArgs e)
        {
            BeginWipe(e.Mobile, WipeType.Items | WipeType.Mobiles);
        }

        [Usage("WipeItems")]
        [Description("Wipes all items in a targeted bounding box.")]
        private static void WipeItems_OnCommand(CommandEventArgs e)
        {
            BeginWipe(e.Mobile, WipeType.Items);
        }

        [Usage("WipeNPCs")]
        [Description("Wipes all npcs in a targeted bounding box.")]
        private static void WipeNPCs_OnCommand(CommandEventArgs e)
        {
            BeginWipe(e.Mobile, WipeType.Mobiles);
        }

        [Usage("WipeMultis")]
        [Description("Wipes all multis in a targeted bounding box.")]
        private static void WipeMultis_OnCommand(CommandEventArgs e)
        {
            BeginWipe(e.Mobile, WipeType.Multis);
        }

        private static void WipeBox_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            DoWipe(from, map, start, end, (WipeType)state);
        }
    }
}