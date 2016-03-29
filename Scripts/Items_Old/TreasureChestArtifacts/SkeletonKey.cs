using Server;
using System;

namespace Server.Items
{
    public class Skeletonkey : Lockpick
    {
        public override int LabelNumber { get { return 1095522; } }

        public override bool IsSkeletonKey { get { return true; } }
        public override int SkillBonus { get { return 10; } }

        private int m_Uses;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Uses { get { return m_Uses; } set { m_Uses = value; InvalidateProperties(); if (m_Uses <= 0) Delete(); } }

        [Constructable]
        public Skeletonkey() : base(1)
        {
            Stackable = false;
            ItemID = 16650;
            m_Uses = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, m_Uses.ToString()); //uses remaining: ~1_val~
        }

        public override void OnUse()
        {
            Uses--;
        }

        public Skeletonkey(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Uses);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Uses = reader.ReadInt();
        }
    }
}