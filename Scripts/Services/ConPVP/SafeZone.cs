using System;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.ConPVP
{
    public class SafeZone : GuardedRegion
    {
        public static readonly int SafeZonePriority = HouseRegion.HousePriority + 1;
        /*public override bool AllowReds{ get{ return true; } }*/
        public SafeZone(Rectangle2D area, Point3D goloc, Map map, bool isGuarded)
            : base(null, map, SafeZonePriority, area)
        {
            this.GoLocation = goloc;

            this.Disabled = !isGuarded;

            this.Register();
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            if (from.AccessLevel < AccessLevel.GameMaster)
                return false;

            return base.AllowHousing(from, p);
        }

        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            if (m.Player && Factions.Sigil.ExistsOn(m))
            {
                m.SendMessage(0x22, "You are holding a sigil and cannot enter this zone.");
                return false;
            }

            PlayerMobile pm = m as PlayerMobile;

            if (pm == null && m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;

                if (bc.Summoned)
                    pm = bc.SummonMaster as PlayerMobile;
            }

            if (pm != null && pm.DuelContext != null && pm.DuelContext.StartedBeginCountdown)
                return true;

            if (DuelContext.CheckCombat(m))
            {
                m.SendMessage(0x22, "You have recently been in combat and cannot enter this zone.");
                return false;
            }

            return base.OnMoveInto(m, d, newLocation, oldLocation);
        }

        public override void OnEnter(Mobile m)
        {
            m.SendMessage("You have entered a dueling safezone. No combat other than duels are allowed in this zone.");
        }

        public override void OnExit(Mobile m)
        {
            m.SendMessage("You have left a dueling safezone. Combat is now unrestricted.");
        }

        public override bool CanUseStuckMenu(Mobile m)
        {
            return false;
        }
    }
}