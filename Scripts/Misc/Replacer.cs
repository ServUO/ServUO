using Server.Items;
using System;

namespace Server
{
    public class Replacer
    {
        public static void Replace(Item item1, Item item2)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(1), (oldItem, newItem) =>
            {
                object parent = oldItem.Parent;

                if (parent == null)
                {
                    Multis.BaseHouse house = Multis.BaseHouse.FindHouseAt(oldItem);

                    newItem.MoveToWorld(oldItem.Location, oldItem.Map);

                    newItem.IsLockedDown = oldItem.IsLockedDown;
                    newItem.IsSecure = oldItem.IsSecure;
                    newItem.Movable = oldItem.Movable;

                    if (house != null && house.LockDowns.ContainsKey(oldItem))
                    {
                        house.LockDowns.Remove(oldItem);
                        house.LockDowns.Add(newItem, house.Owner);
                    }
                    else if (house != null && house.IsSecure(oldItem))
                    {
                        house.ReleaseSecure(house.Owner, oldItem);
                        house.AddSecure(house.Owner, newItem);
                    }

                    oldItem.Delete();
                }
                else
                {
                    newItem.Movable = oldItem.Movable;

                    if (parent is Container)
                    {
                        oldItem.Delete();
                        ((Container)parent).DropItem(newItem);
                    }
                    else if (parent is Mobile)
                    {
                        oldItem.Delete();
                        ((Mobile)parent).AddItem(newItem);
                    }
                    else
                    {
                        newItem.Delete();
                        oldItem.Delete();

                        Console.WriteLine("Item replacement failed: {0}", newItem.GetType());
                    }
                }
            }, item1, item2);
        }
    }
}
