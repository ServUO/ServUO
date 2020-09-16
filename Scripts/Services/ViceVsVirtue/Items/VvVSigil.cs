using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Engines.VvV
{
    public class VvVSigil : Item, IRevealableItem
    {
        public const int OwnershipHue = 0xB;

        [CommandProperty(AccessLevel.GameMaster)]
        public VvVBattle Battle { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D HomeLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastStolen { get; set; }

        public override int LabelNumber => 1123391;  // Sigil
        public override bool HandlesOnMovement => !Visible;
        public bool CheckWhenHidden => true;

        public VvVSigil(VvVBattle battle, Point3D home)
            : base(0x99C7)
        {
            Battle = battle;
            Visible = false;

            Hue = 2721;

            LootType = LootType.Cursed;
        }

        public static bool ExistsOn(Mobile mob, bool vvvOnly = false)
        {
            if (mob == null || mob.Backpack == null)
                return false;

            Container pack = mob.Backpack;

            return ViceVsVirtueSystem.Enabled && vvvOnly && pack.FindItemByType(typeof(VvVSigil)) != null;
        }

        public void OnStolen(VvVPlayerEntry entry)
        {
            if (Battle != null && RootParentEntity == null)
            {
                Battle.SpawnPriests();
                Battle.Update(null, entry, UpdateType.Steal);

                LastStolen = DateTime.UtcNow;
                HomeLocation = Location;

                Movable = true;
            }
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (LastStolen == DateTime.MinValue)
            {
                from.SendLocalizedMessage(1005225); // You must use the stealing skill to pick up the sigil
                return false;
            }

            return base.CheckLift(from, item, ref reject);
        }

        private Mobile FindOwner(object parent)
        {
            if (parent is Item)
                return ((Item)parent).RootParent as Mobile;

            if (parent is Mobile)
                return (Mobile)parent;

            return null;
        }

        public void ReturnToHome()
        {
            MoveToWorld(HomeLocation, Map.Felucca);
            Visible = false;
            Movable = false;
        }

        public static bool CheckMovement(PlayerMobile pm, Direction d)
        {
            if (!ViceVsVirtueSystem.Enabled)
            {
                return true;
            }

            int x = pm.X;
            int y = pm.Y;

            Movement.Movement.Offset(d, ref x, ref y);

            Region r = Region.Find(new Point3D(x, y, pm.Map.GetAverageZ(x, y)), pm.Map);

            return ViceVsVirtueSystem.IsBattleRegion(r);
        }

        public bool CheckReveal(Mobile m)
        {
            if (!ViceVsVirtueSystem.IsVvV(m))
                return false;

            return Utility.Random(100) <= m.Skills[SkillName.DetectHidden].Value;
        }

        public virtual void OnRevealed(Mobile m)
        {
            Visible = true;
        }

        public virtual bool CheckPassiveDetect(Mobile m)
        {
            if (m.InRange(Location, 4))
            {
                int skill = (int)m.Skills[SkillName.DetectHidden].Value;

                if (skill >= 80 && Utility.Random(300) < skill)
                    return true;
            }

            return false;
        }

        public VvVSigil(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(LastStolen);
            writer.Write(HomeLocation);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            LastStolen = reader.ReadDateTime();
            HomeLocation = reader.ReadPoint3D();
        }
    }
}
