using Server.ContextMenus;
using Server.Engines.Harvest;
using System.Collections.Generic;

namespace Server.Items
{
    public interface IAxe
    {
        bool Axe(Mobile from, BaseAxe axe);
    }

    public abstract class BaseAxe : BaseMeleeWeapon, IHarvestTool
    {
        public BaseAxe(int itemID)
            : base(itemID)
        {
        }

        public BaseAxe(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound => 0x232;
        public override int DefMissSound => 0x23A;

        public override SkillName DefSkill => SkillName.Swords;

        public override WeaponType DefType => WeaponType.Axe;

        public override WeaponAnimation DefAnimation => WeaponAnimation.Slash2H;

        public virtual HarvestSystem HarvestSystem => Lumberjacking.System;

        public override void OnDoubleClick(Mobile from)
        {
            if (HarvestSystem == null || Deleted)
                return;

            Point3D loc = GetWorldLocation();

            if (!from.InLOS(loc) || !from.InRange(loc, 2))
            {
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3E9, 1019045); // I can't reach that
                return;
            }
            else if (!IsAccessibleTo(from))
            {
                PublicOverheadMessage(Network.MessageType.Regular, 0x3E9, 1061637); // You are not allowed to access 
                return;
            }

            if (!(HarvestSystem is Mining))
                from.SendLocalizedMessage(1010018); // What do you want to use this item on?

            HarvestSystem.BeginHarvesting(from, this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (HarvestSystem == null)
                return;

            BaseHarvestTool.AddContextMenuEntries(from, this, list, HarvestSystem);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(3); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
