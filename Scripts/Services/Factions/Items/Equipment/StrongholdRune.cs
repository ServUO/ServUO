using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Factions
{
    public class StrongholdRune : Item, IFactionItem
    {
        public override int LabelNumber { get { return 1094700; } } // Faction Stronghold Rune

        #region Factions
        private FactionItem m_FactionState;

        public FactionItem FactionItemState
        {
            get { return m_FactionState; }
            set
            {
                m_FactionState = value;

                Hue = m_FactionState != null ? m_FactionState.Faction.Definition.HuePrimary : 0;
            }
        }
        #endregion

        public StrongholdRune()
            : base(0x1F14)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                if (FactionEquipment.CanUse(this, m))
                {
                    if (!IsInCooldown(m))
                    {
                        Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(1), Warp, m);
                    }
                    else
                    {
                        m.SendLocalizedMessage(501789); // You must wait before trying again.
                    }
                }
                else
                {
                    m.SendMessage("You are not the proper faction to use this item.");
                    //TODO: Message
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            FactionEquipment.AddFactionProperties(this, list);
        }

        private void Warp(Mobile m)
        {
            Point3D p = GetStoneLocation();
            Map map = Faction.Facet;

            int x = p.X;
            int y = p.Y;
            int z = p.Z;

            if (p != Point3D.Zero)
            {
                for (int i = 0; i < 20; i++)
                {
                    x = p.X + Utility.RandomMinMax(-4, 4);
                    y = p.Y + Utility.RandomMinMax(-4, 4);
                    z = map.GetAverageZ(x, y);

                    Point3D temp = new Point3D(x, y, z);

                    if (map.CanSpawnMobile(temp))
                    {
                        p = temp;
                        break;
                    }
                }

                m.PlaySound(0x1FC);
                m.MoveToWorld(p, map);
                m.PlaySound(0x1FC);

                m.SendLocalizedMessage(1094706); // Your faction stronghold rune has disappeared.

                if (m.AccessLevel == AccessLevel.Player)
                    Delete();

                AddToCooldown(m);
            }
        }

        private Point3D GetStoneLocation()
        {
            if (FactionItemState == null)
            {
                return Point3D.Zero;
            }

            return FactionItemState.Faction.Definition.Stronghold.FactionStone;
        }

        public static List<Mobile> Cooldown = new List<Mobile>();

        public static bool IsInCooldown(Mobile m)
        {
            return Cooldown.Contains(m);
        }

        public static void AddToCooldown(Mobile m)
        {
            Cooldown.Add(m);

            int minutes = 30;

            PlayerState ps = PlayerState.Find(m);

            if(ps != null)
                minutes = 30 - (ps.Rank.Rank * 2);

            Timer.DelayCall<Mobile>(TimeSpan.FromMinutes(minutes), mob =>
                {
                    Cooldown.Remove(mob);
                }, m);
        }

        public StrongholdRune(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}