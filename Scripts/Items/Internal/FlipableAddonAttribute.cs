using Server.Multis;
using System;
using System.Reflection;

namespace Server.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FlipableAddonAttribute : Attribute
    {
        private static readonly string m_MethodName = "Flip";
        private static readonly Type[] m_Params = new Type[]
        {
            typeof(Mobile), typeof(Direction)
        };
        private readonly Direction[] m_Directions;
        public FlipableAddonAttribute(params Direction[] directions)
        {
            m_Directions = directions;
        }

        public Direction[] Directions => m_Directions;
        public virtual void Flip(Mobile from, Item addon)
        {
            if (m_Directions != null && m_Directions.Length > 1)
            {
                try
                {
                    MethodInfo flipMethod = addon.GetType().GetMethod(m_MethodName, m_Params);

                    if (flipMethod != null)
                    {
                        int index = 0;

                        for (int i = 0; i < m_Directions.Length; i++)
                        {
                            if (addon.Direction == m_Directions[i])
                            {
                                index = i + 1;
                                break;
                            }
                        }

                        if (index >= m_Directions.Length)
                            index = 0;

                        ClearComponents(addon);

                        flipMethod.Invoke(addon, new object[2] { from, m_Directions[index] });

                        BaseHouse house = null;
                        AddonFitResult result = AddonFitResult.Valid;

                        addon.Map = Map.Internal;

                        if (addon is BaseAddon)
                            result = ((BaseAddon)addon).CouldFit(addon.Location, from.Map, from, ref house);
                        else if (addon is BaseAddonContainer)
                            result = ((BaseAddonContainer)addon).CouldFit(addon.Location, from.Map, from, ref house);

                        addon.Map = from.Map;

                        if (result != AddonFitResult.Valid)
                        {
                            if (index == 0)
                                index = m_Directions.Length - 1;
                            else
                                index -= 1;

                            ClearComponents(addon);

                            flipMethod.Invoke(addon, new object[2] { from, m_Directions[index] });

                            if (result == AddonFitResult.Blocked)
                                from.SendLocalizedMessage(500269); // You cannot build that there.
                            else if (result == AddonFitResult.NotInHouse)
                                from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                            else if (result == AddonFitResult.DoorTooClose)
                                from.SendLocalizedMessage(500271); // You cannot build near the door.
                            else if (result == AddonFitResult.NoWall)
                                from.SendLocalizedMessage(500268); // This object needs to be mounted on something.
                        }

                        addon.Direction = m_Directions[index];
                    }
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }
            }
        }

        private void ClearComponents(Item item)
        {
            if (item is BaseAddon)
            {
                BaseAddon addon = (BaseAddon)item;

                foreach (AddonComponent c in addon.Components)
                {
                    c.Addon = null;
                    c.Delete();
                }

                addon.Components.Clear();
            }
            else if (item is BaseAddonContainer)
            {
                BaseAddonContainer addon = (BaseAddonContainer)item;

                foreach (AddonContainerComponent c in addon.Components)
                {
                    c.Addon = null;
                    c.Delete();
                }

                addon.Components.Clear();
            }
        }
    }
}
