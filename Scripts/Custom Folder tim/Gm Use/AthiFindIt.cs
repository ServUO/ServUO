/* Athi's AthiFindIt command
 *   Crafted By Athiredros
 *       27 - 07 - 2005
 *    Allrights Reserved
 * * * * * * * * * * * * * * *
 * Sadece izin verilen shardlarda kullanýlabilir.
 * Only can be used in shards which one have rights.
 * www.waptir.com
 * www.cewap.com
 * Special Notes / Ozel Notlar
 * Komut kullanimi loglarda bulunan seri numarasý belli olan itemler/mobilelar içindir.
 * Bu item/mobile lar ý yanýnýza çekebilirsiniz /çantanýza alabilirsiniz. Yada yanýna gidebilirsiniz. Veya direk silebilirsiniz.
 * -
 * This command for items/mobiles which one has serial number in the log files.
 * You can go to item's/mobile's location or you can get it in your backpack / get near you. Or you can delete it.
 */
using System;
using Server;
using Server.Mobiles;

namespace Server.Commands
{
	public class AthiFindIt
	{
		public static void Initialize()
		{
            CommandSystem.Register("AthiFindIt", AccessLevel.Seer, new CommandEventHandler(AthiFindIt_OnCommand));
		}

		[Usage( "AthiFindIt <Serial>" )]
		[Description( "Finds an item by serial." )]
        public static void AthiFindIt_OnCommand(CommandEventArgs e)
		{
			if ( e.Length == 1 )
			{
				int serial = e.GetInt32( 0 );
                Item item = World.FindItem(serial);
                Mobile mobil = World.FindMobile(serial);
                if (item != null)
                {
                    object root = item.RootParent;

                    if (root is Mobile)
                        e.Mobile.SendMessage("{0} [{1}]: {2} ({3})", item.GetWorldLocation(), item.Map, root.GetType().Name, ((Mobile)root).Name);
                    else
                        e.Mobile.SendMessage("{0} [{1}]: {2}", item.GetWorldLocation(), item.Map, root == null ? "(null)" : root.GetType().Name);

                }
                else if (mobil != null)
                {
                    e.Mobile.SendMessage("{0} [{1}]: {2} ", mobil.Location, mobil.Map, mobil.Name);
                }
                else
                    e.Mobile.SendMessage("There are no item/mobile with this serial number");
			}
            else if( e.Length == 2)
            {
                int serial = e.GetInt32(0);
                string property = e.GetString(1);
                Item item = World.FindItem(serial);
                if (item != null)
                    switch (property)
                    {
                        case "get":
                            {
                                object root = item.RootParent;

                                if (root is Mobile)
                                    e.Mobile.SendMessage("{0} [{1}]: {2} ({3})", item.GetWorldLocation(), item.Map, root.GetType().Name, ((Mobile)root).Name);
                                else
                                    e.Mobile.SendMessage("{0} [{1}]: {2}", item.GetWorldLocation(), item.Map, root == null ? "(null)" : root.GetType().Name);

                                if (e.Mobile.PlaceInBackpack(item))
                                    e.Mobile.SendMessage("The item has been placed in your backpack.");
                                else
                                    e.Mobile.SendMessage("Your backpack could not hold the item.");
                                break;
                            }
                        case "go":
                            {
                                object root = item.RootParent;

                                if (root is Mobile)
                                {
                                    e.Mobile.SendMessage("{0} [{1}]: {2} ({3})", item.GetWorldLocation(), item.Map, root.GetType().Name, ((Mobile)root).Name);
                                    e.Mobile.MoveToWorld(((Mobile)root).Location, ((Mobile)root).Map);
                                }
                                else
                                {
                                    e.Mobile.SendMessage("{0} [{1}]: {2}", item.GetWorldLocation(), item.Map, root == null ? "(null)" : root.GetType().Name);
                                    e.Mobile.MoveToWorld(item.GetWorldLocation(), item.Map);
                                }
                                e.Mobile.SendMessage("You have been teleported to the item's Location");
                                break;
                            }
                        case "del":
                            {
                                object root = item.RootParent;

                                if (root is Mobile)
                                    e.Mobile.SendMessage("{0} [{1}]: {2} ({3})", item.GetWorldLocation(), item.Map, root.GetType().Name, ((Mobile)root).Name);
                                else
                                    e.Mobile.SendMessage("{0} [{1}]: {2}", item.GetWorldLocation(), item.Map, root == null ? "(null)" : root.GetType().Name);
                                item.Delete();
                                e.Mobile.SendMessage("Item has been deleted.");
                                break;
                            }

                        default:
                            {
                                e.Mobile.SendMessage("Wrong Parameter try [go/get/del]");
                                break;
                            }

                    }
                else
                    e.Mobile.SendMessage("There is no item with this serial number");
            }


            else if (e.Length == 3 && e.GetString(2) == "mobile")
            {
                int serial = e.GetInt32(0);
                string property = e.GetString(1);
                Mobile mobil = World.FindMobile(serial);
                if (mobil != null)
                    switch (property)
                    {
                        case "get":
                            {

                                e.Mobile.SendMessage("{0} [{1}]: {2} ", mobil.Location, mobil.Map, mobil.Name);
                               if (mobil is  IMount)
                                   if (((BaseMount)mobil).Rider != null)
                                   {
                                       Mobiles.BaseMount.Dismount(((BaseMount)mobil).Rider);
                                   }
                                   mobil.MoveToWorld(e.Mobile.Location, e.Mobile.Map);
                                e.Mobile.SendMessage("The mobile has been found and moved to you");

                                break;
                            }
                        case "go":
                            {

                                e.Mobile.SendMessage("{0} [{1}]: {2} ", mobil.Location, mobil.Map, mobil.Name);
                                if (mobil is IMount)
                                    if (((BaseMount)mobil).Rider != null)
                                    {
                                        Mobiles.BaseMount.Dismount(((BaseMount)mobil).Rider);
                                    }
                                e.Mobile.MoveToWorld(mobil.Location, mobil.Map);
                                e.Mobile.SendMessage("You have been teleported to the mobile's Location");
                                break;
                            }
                        case "del":
                            {
                                if (mobil is PlayerMobile)
                                {
                                    e.Mobile.SendMessage("You can not delete Players with this command");
                                }
                                else
                                {
                                    mobil.Delete();
                                    e.Mobile.SendMessage("Mobile has been deleted.");
                                }
                                break;
                            }

                        default:
                            {
                                e.Mobile.SendMessage("Wrong Parameter try [go/get/del]");
                                break;
                            }

                    }
                else
                    e.Mobile.SendMessage("There is no mobile with this serial number");
            }

            else
			{
                e.Mobile.SendMessage("Format: AthiFindIt <serial int32> [go/get/del] [mobile]");
			}
		}
	}
}