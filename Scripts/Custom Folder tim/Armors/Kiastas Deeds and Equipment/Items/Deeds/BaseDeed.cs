using System;
using Server.Network;
using Server.Kiasta.Settings;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Kiasta.Deeds
{
    public class BaseDeedTarget:Target
    {
        #region Member Initializers
        private object[] m_Attribute;
        private string m_ModifyMethodName = Settings.Misc.ModifyAttributeMethodName;
        private int m_Max;
        private object m_Modifier;
        private string m_AttributeName;
        #endregion

        #region Member Getters/Setters
        [CommandProperty(AccessLevel.GameMaster)]
        public object[] Attribute { get { return m_Attribute; } set { m_Attribute = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Max { get { return m_Max; } set { m_Max = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public object Modifier { get { return m_Modifier; } set { m_Modifier = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string AttributeName { get { return m_AttributeName; } set { m_AttributeName = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ModifyMethodName { get { return m_ModifyMethodName; } }
        #endregion

        #region Constructors
        public BaseDeedTarget():base(1, false, TargetFlags.None) { }
        #endregion
    }

    public class BaseDeed:Item
    {
        #region Member Initializers
        private string m_AttributeName;
        #endregion

        #region Getters/Setters
        [CommandProperty(AccessLevel.GameMaster)]
        public string AttributeName { get { return m_AttributeName; } set { m_AttributeName = value; } }
        #endregion

        #region Constructors
        [Constructable]
        public BaseDeed(int itemID):base(itemID)
        {
            Hue = Settings.Misc.DeedHue;
            Stackable = Settings.Misc.DeedIsStackable;
            Weight = Settings.Misc.DeedWeight;
        }

        public BaseDeed(Serial serial):base(serial) { }
        #endregion

        #region Serializer/Deserializer
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
        #endregion
    }
}