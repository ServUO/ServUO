using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Harvest;

namespace Server.Items
{
    public abstract class BasePoleArm : BaseMeleeWeapon, IHarvestTool
    {
        public BasePoleArm(int itemID)
            : base(itemID)
        {
        }

        public BasePoleArm(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound => 0x237;
        public override int DefMissSound => 0x238;

        public override SkillName DefSkill => SkillName.Swords;

        public override WeaponType DefType => WeaponType.Polearm;

        public override WeaponAnimation DefAnimation => WeaponAnimation.Slash2H;
        
        public virtual HarvestSystem HarvestSystem => Lumberjacking.System;
       
        public override void OnDoubleClick(Mobile from)
        {
            if (this.HarvestSystem == null)
                return;

            if (this.IsChildOf(from.Backpack) || this.Parent == from)
                this.HarvestSystem.BeginHarvesting(from, this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (this.HarvestSystem != null)
                BaseHarvestTool.AddContextMenuEntries(from, this, list, this.HarvestSystem);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)3); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}