using System;
using Server;
using Server.Targeting;
using Server.Engines.VeteranRewards;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class EtherealRetouchingTool : Item, IRewardItem
    {
        public override int LabelNumber { get { return 1113814; } } // Retouching Tool

        public bool IsRewardItem
        {
            get;
            set;
        }

        [Constructable]
        public EtherealRetouchingTool()
            : base(0x42C6)
        {
            LootType = LootType.Blessed;
        }

        public EtherealRetouchingTool(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (IsRewardItem)
                list.Add(1080458); // 11th Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
                from.SendLocalizedMessage(1113815); // Target the ethereal mount you wish to retouch.
            }
        }

        private class InternalTarget : Target
        {
            private EtherealRetouchingTool m_Tool;

            public InternalTarget(EtherealRetouchingTool tool)
                : base(-1, false, TargetFlags.None)
            {
                m_Tool = tool;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Tool.IsChildOf(from.Backpack) && targeted is EtherealMount)
                {
                    EtherealMount mount = targeted as EtherealMount;

                    if (mount is GMEthereal)
                    {
                        from.SendMessage("You cannot use it on this!");
                    }
                    else if (mount.IsChildOf(from.Backpack) && RewardSystem.CheckIsUsableBy(from, m_Tool, null))
                    {
                        mount.Transparent = mount.Transparent ? false : true;
                        from.PlaySound(0x242);

                        mount.InvalidateProperties();
                    }
                }
            }
        }

        public static void AddProperty(EtherealMount mount, ObjectPropertyList list)
        {
            list.Add(1113818, mount.Transparent ? "#1078520" : "#1153298");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.Write(IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
                IsRewardItem = true;
            else
                IsRewardItem = reader.ReadBool();

            if (LootType != LootType.Blessed)
                LootType = LootType.Blessed;
        }
    }
}