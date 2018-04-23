using System;
using System.Collections;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
    public class LockpickTrainer : LockableContainer
    {
        private bool m_Locked;

        [Constructable]
        public LockpickTrainer(): base(0x9AA)
        {
            Locked = true;
            LockLevel = 10;
            RequiredSkill = 10;
            Weight = 4.0;

        }
		
		public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add("Lockpick Trainer, Double click to set for your skill level.");
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            this.LabelTo(from, "Lockpick Trainer, Double click to set for your skill level.");
        }
		
		public override void Open(Mobile from)
		{
			double lockpicking = from.Skills[SkillName.Lockpicking].Value;
			int level = (int)(lockpicking * 0.8);
			this.RequiredSkill = level - 4;
            this.LockLevel = level - 14;
            this.MaxLockLevel = level + 35;

            if (this.LockLevel == 0)
                this.LockLevel = -1;
            else if (this.LockLevel > 95)
                this.LockLevel = 95;

            if (this.RequiredSkill > 95)
                this.RequiredSkill = 95;

            if (this.MaxLockLevel > 95)
                this.MaxLockLevel = 95;
					
			from.SendMessage("This chest has been set for "+lockpicking+" lockpicking skill!");
		}
			
		
		public override void LockPick(Mobile from)
        {
            this.Locked = true;
            from.SendMessage("The container magically relocks it self.");
        }
        public LockpickTrainer(Serial serial)
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