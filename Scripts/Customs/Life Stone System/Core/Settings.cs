using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;

namespace Server
{
    public sealed class LifeStoneSettings
    {
        public override string ToString()
        {
            return @"Life Stone Settings";
        }

        private static bool m_TeleportCorpse;
        private static bool m_BlessNewStones;
        private static bool m_AllowUseInDungeons;
        private static int m_ItemID;
        private static int m_Hue;

        [CommandProperty(AccessLevel.Administrator)]
        public static bool TeleportCorpse { get { return m_TeleportCorpse; } set { m_TeleportCorpse = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public static bool BlessNewStones { get { return m_BlessNewStones; } set { m_BlessNewStones = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public static bool AllowInDungeons { get { return m_AllowUseInDungeons; } set { m_AllowUseInDungeons = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public static int StoneItemID { get { return m_ItemID; } set { m_ItemID = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public static int StoneHue { get { return m_Hue; } set { m_Hue = value; } }

        public LifeStoneSettings(bool teleportCorpse, bool blessNewStones, bool allowUseInDungeons, int itemID, int hue)
        {
            m_TeleportCorpse = teleportCorpse;
            m_BlessNewStones = blessNewStones;
            m_AllowUseInDungeons = allowUseInDungeons;
            m_ItemID = itemID;
            m_Hue = hue;
        }

        public LifeStoneSettings(GenericReader reader)
        {
            Deserialize(reader);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0); // Version

            // Version 0
            writer.Write(m_TeleportCorpse);
            writer.Write(m_BlessNewStones);
            writer.Write(m_AllowUseInDungeons);
            writer.Write(m_ItemID);
            writer.Write(m_Hue);
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_TeleportCorpse = reader.ReadBool();
                        m_BlessNewStones = reader.ReadBool();
                        m_AllowUseInDungeons = reader.ReadBool();
                        m_ItemID = reader.ReadInt();
                        m_Hue = reader.ReadInt();
                        break;
                    }
            }
        }
    }
}