using System;
using Server.Items;

namespace Server.Commands
{
    public class GenKhaldun
    {
        private static int m_Count;
        public static void Initialize()
        {
			CommandSystem.Register("GenKhaldun", AccessLevel.Administrator, new CommandEventHandler(GenKhaldun_OnCommand));
			CommandSystem.Register("DeleteKhaldun", AccessLevel.Administrator, new CommandEventHandler(DeleteKhaldun_OnCommand));
		}

		public static bool FindMorphItem(int x, int y, int z, int inactiveItemID, int activeItemID)
        {
            IPooledEnumerable eable = Map.Felucca.GetItemsInRange(new Point3D(x, y, z), 0);

            foreach (Item item in eable)
            {
                if (item is MorphItem && item.Z == z && ((MorphItem)item).InactiveItemID == inactiveItemID && ((MorphItem)item).ActiveItemID == activeItemID)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        public static bool FindEffectController(int x, int y, int z)
        {
            IPooledEnumerable eable = Map.Felucca.GetItemsInRange(new Point3D(x, y, z), 0);

            foreach (Item item in eable)
            {
                if (item is EffectController && item.Z == z)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        public static Item TryCreateItem(int x, int y, int z, Item srcItem)
        {
            IPooledEnumerable eable = Map.Felucca.GetItemsInBounds(new Rectangle2D(x, y, 1, 1));

            foreach (Item item in eable)
            {
                if (item.GetType() == srcItem.GetType())
                {
                    eable.Free();
                    srcItem.Delete();
                    return item;
                }
            }

            eable.Free();
            srcItem.MoveToWorld(new Point3D(x, y, z), Map.Felucca);
            m_Count++;
			
            return srcItem;
        }

        public static void CreateMorphItem(int x, int y, int z, int inactiveItemID, int activeItemID, int range)
        {
            if (FindMorphItem(x, y, z, inactiveItemID, activeItemID))
                return;
			
            MorphItem item = new MorphItem(inactiveItemID, activeItemID, range, 3);
			WeakEntityCollection.Add("khaldun", item);
			
            item.MoveToWorld(new Point3D(x, y, z), Map.Felucca);
            m_Count++;
        }

        public static void CreateApproachLight(int x, int y, int z, int off, int on, LightType light)
        {
            if (FindMorphItem(x, y, z, off, on))
                return;
			
            MorphItem item = new MorphItem(off, on, 2, 3);
			WeakEntityCollection.Add("khaldun", item);
			item.Light = light;
			
            item.MoveToWorld(new Point3D(x, y, z), Map.Felucca);
            m_Count++;
        }

        public static void CreateSoundEffect(int x, int y, int z, int sound, int range)
        {
            if (FindEffectController(x, y, z))
                return;
			
            EffectController item = new EffectController();
			WeakEntityCollection.Add("khaldun", item);
			item.SoundID = sound;
            item.TriggerType = EffectTriggerType.InRange;
            item.TriggerRange = range;
			
            item.MoveToWorld(new Point3D(x, y, z), Map.Felucca);
            m_Count++;
        }

        public static void CreateBigTeleporterItem(int x, int y, bool reverse)
        {
            if (FindMorphItem(x, y, 0, reverse ? 0x17DC : 0x17EE, reverse ? 0x17EE : 0x17DC))
                return;
			
            MorphItem item = new MorphItem(reverse ? 0x17DC : 0x17EE, reverse ? 0x17EE : 0x17DC, 1, 3);
			WeakEntityCollection.Add("khaldun", item);

			item.MoveToWorld(new Point3D(x, y, 0), Map.Felucca);
            m_Count++;
        }

		public static void DeleteKhaldun_OnCommand(CommandEventArgs e)
		{
			WeakEntityCollection.Delete("khaldun");
		}


		public static void GenKhaldun_OnCommand(CommandEventArgs e)
        {
            m_Count = 0;
			
            // Generate Morph Items
            CreateMorphItem(5459, 1416, 0, 0x1D0, 0x1, 1);			
            CreateMorphItem(5460, 1416, 0, 0x1D0, 0x1, 1);
            CreateMorphItem(5459, 1416, 0, 0x1, 0x53D, 1);
            CreateMorphItem(5460, 1416, 0, 0x1, 0x53B, 1);
			
            CreateMorphItem(5459, 1425, 0, 0x1, 0x53B, 2);
            CreateMorphItem(5459, 1426, 0, 0x1, 0x53B, 2);
            CreateMorphItem(5459, 1427, 0, 0x1, 0x53B, 2);
            CreateMorphItem(5460, 1425, 0, 0x1, 0x53B, 2);
            CreateMorphItem(5460, 1426, 0, 0x1, 0x53B, 2);
            CreateMorphItem(5460, 1427, 0, 0x1, 0x53B, 2);
            CreateMorphItem(5461, 1427, 0, 0x1, 0x53B, 2);
            CreateMorphItem(5460, 1422, 0, 0x1, 0x544, 2);
            CreateMorphItem(5460, 1419, 0, 0x1, 0x545, 2);
            CreateMorphItem(5460, 1420, 0, 0x1, 0x545, 2);
            CreateMorphItem(5460, 1423, 0, 0x1, 0x545, 2);
            CreateMorphItem(5460, 1424, 0, 0x1, 0x545, 2);
            CreateMorphItem(5461, 1426, 0, 0x1, 0x545, 2);
            CreateMorphItem(5460, 1417, 0, 0x1, 0x546, 1);
            CreateMorphItem(5460, 1418, 0, 0x1, 0x546, 2);
            CreateMorphItem(5460, 1421, 0, 0x1, 0x546, 2);
            CreateMorphItem(5461, 1425, 0, 0x1, 0x548, 2);
            CreateMorphItem(5459, 1420, 0, 0x1, 0x54A, 2);
            CreateMorphItem(5459, 1421, 0, 0x1, 0x54A, 2);
            CreateMorphItem(5459, 1423, 0, 0x1, 0x54A, 2);
            CreateMorphItem(5459, 1418, 0, 0x1, 0x54B, 2);
            CreateMorphItem(5459, 1422, 0, 0x1, 0x54B, 2);
            CreateMorphItem(5459, 1417, 0, 0x1, 0x54C, 1);
            CreateMorphItem(5459, 1419, 0, 0x1, 0x54C, 2);
            CreateMorphItem(5459, 1424, 0, 0x1, 0x54C, 2);
			
            CreateMorphItem(5458, 1426, 0, 0x1, 0x1D1, 2);
            CreateMorphItem(5459, 1427, 0, 0x1, 0x1E3, 2);
            CreateMorphItem(5458, 1425, 3, 0x1, 0x1E4, 2);
            CreateMorphItem(5458, 1427, 6, 0x1, 0x1E5, 2);
            CreateMorphItem(5461, 1427, 0, 0x1, 0x1E8, 2);
            CreateMorphItem(5460, 1427, 0, 0x1, 0x1E9, 2);
            CreateMorphItem(5458, 1425, 0, 0x1, 0x1EA, 2);
            CreateMorphItem(5458, 1427, 0, 0x1, 0x1EA, 2);
            CreateMorphItem(5458, 1427, 3, 0x1, 0x1EA, 2);
			
            // Generate Approach Lights
            CreateApproachLight(5393, 1417, 0, 0x1857, 0x1858, LightType.Circle150);
            CreateApproachLight(5393, 1420, 0, 0x1857, 0x1858, LightType.Circle150);
            CreateApproachLight(5395, 1421, 0, 0x1857, 0x1858, LightType.Circle150);
            CreateApproachLight(5396, 1417, 0, 0x1857, 0x1858, LightType.Circle150);
            CreateApproachLight(5397, 1419, 0, 0x1857, 0x1858, LightType.Circle150);
			
            CreateApproachLight(5441, 1393, 5, 0x1F2B, 0x19BB, LightType.Circle225);
            CreateApproachLight(5446, 1393, 5, 0x1F2B, 0x19BB, LightType.Circle225);
			
            // Generate Sound Effects
            CreateSoundEffect(5425, 1489, 5, 0x102, 1);
            CreateSoundEffect(5425, 1491, 5, 0x102, 1);
			
            CreateSoundEffect(5449, 1499, 10, 0xF5, 1);
            CreateSoundEffect(5451, 1499, 10, 0xF5, 1);
            CreateSoundEffect(5453, 1499, 10, 0xF5, 1);
			
            CreateSoundEffect(5524, 1367, 0, 0x102, 1);
			
            CreateSoundEffect(5450, 1370, 0, 0x220, 2);
            CreateSoundEffect(5450, 1372, 0, 0x220, 2);
			
            CreateSoundEffect(5460, 1416, 0, 0x244, 2);
			
            CreateSoundEffect(5483, 1439, 5, 0x14, 3);
			
            // Generate Big Teleporter
            CreateBigTeleporterItem(5387, 1325, true);
            CreateBigTeleporterItem(5388, 1326, true);
            CreateBigTeleporterItem(5388, 1325, false);
            CreateBigTeleporterItem(5387, 1326, false);
			
            // Generate Central Khaldun entrance
            DisappearingRaiseSwitch sw = TryCreateItem(5459, 1426, 10, new DisappearingRaiseSwitch()) as DisappearingRaiseSwitch;
            RaiseSwitch lv = TryCreateItem(5403, 1359, 0, new RaiseSwitch()) as RaiseSwitch;
			
            RaisableItem stone = TryCreateItem(5403, 1360, 0, new RaisableItem(0x788, 10, 0x477, 0x475, TimeSpan.FromMinutes(1.5))) as RaisableItem;
            RaisableItem door = TryCreateItem(5524, 1367, 0, new RaisableItem(0x1D0, 20, 0x477, 0x475, TimeSpan.FromMinutes(5.0))) as RaisableItem;

			WeakEntityCollection.Add("khaldun", sw);
			WeakEntityCollection.Add("khaldun", lv);
			WeakEntityCollection.Add("khaldun", stone);
			WeakEntityCollection.Add("khaldun", door);

			sw.RaisableItem = stone;
            lv.RaisableItem = door;
			
            e.Mobile.SendMessage(String.Format("{0} dynamic Khaldun item{1} generated.", m_Count, m_Count == 1 ? "" : "s"));
        }
    }
}