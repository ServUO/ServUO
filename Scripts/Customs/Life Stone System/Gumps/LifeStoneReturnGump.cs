using System;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Regions;

namespace Server.Gumps
{
    public class LifeStoneReturnGump : Gump
    {
        private Mobile m_Owner;
        private LifeStone m_Item;

        public LifeStoneReturnGump(Mobile owner, LifeStone item)
            : base(60, 60)
        {
            m_Owner = owner;
            m_Item = item;

            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            AddImage(0, 0, 0x816);
            AddButton(34, 74, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0); // OK
            AddButton(88, 74, 0x995, 0x996, 2, GumpButtonType.Reply, 0); // Cancel

            string msg = "Teleport to linked Ankh?";
            AddHtml(30, 25, 120, 40, msg, false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            PlayerMobile from = ((PlayerMobile)(state.Mobile));

            if (info.ButtonID == 1)
            {
                if (from.Alive)
                    from.SendMessage("The stone cannot transport the living!");
                else
                {
                    if (from.Player && from.Kills >= 5 && (m_Item.Map != Map.Felucca || m_Item.Map != Map.TerMur))
                    { // Comment out " || m_Item.Map != Map.TerMur" if you don't have the map.
                        from.SendLocalizedMessage(1019004);
                    }
                    else if (Factions.Sigil.ExistsOn(from) && m_Item.Map != Factions.Faction.Facet)
                    {
                        from.SendLocalizedMessage(1019004);
                    }
                    else if (!m_Item.AllowInDungeons && (from.Region is DungeonRegion))
                    {
                        from.SendLocalizedMessage(501035);
                    }
                    else
                    {
                        if (m_Item.Deleted || m_Item == null)
                        {
                            from.SendMessage("Sorry, but your Life Stone doesn't exist any more!");

                            // Remove it as a valid working Life Stone
                            for (int i = 0; i < LifeStoneCore.LifeStoneList.Count; i++)
                            {
                                if (LifeStoneCore.LifeStoneList[i].LifeStoneOwner == from)
                                {
                                    LifeStoneCore.LifeStoneList.RemoveAt(i);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            // Teleport them
                            from.Combatant = null;
                            from.Warmode = false;
                            from.Hidden = false;

                            from.MoveToWorld(m_Item.LinkedAnkh, m_Item.LinkedMap);

                            Effects.PlaySound(m_Item.LinkedAnkh, m_Item.LinkedMap, 0x1FE);
                            from.SendMessage("You have been returned to the linked Ankh.");

                            if (m_Item.TeleportCorpse)
                            {
                                from.Corpse.Location = new Point3D(m_Item.LinkedAnkh);
                                from.Corpse.Map = m_Item.LinkedMap;
                                from.SendMessage("Your corpse has been brought with you.");
                            }

                            if (m_Item.TeleportPets)
                            {
                                BaseCreature.TeleportPets(from, m_Item.LinkedAnkh, m_Item.LinkedMap);
                            }

                            if (m_Item.UsesRemaining > 1)
                                m_Item.UsesRemaining--;
                            else
                            {
                                m_Item.Delete();
                                from.SendMessage("You have used up your Life Stone!");
                            }
                        }
                    }
                }
            }
        }

    }
}
