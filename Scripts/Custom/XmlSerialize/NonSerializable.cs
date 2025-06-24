using System;
using System.Collections.Generic;
using System.Reflection;
using Server.Engines.Quests;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.XmlSerialize
{
    public static class NonSerializable
    {
        private static List<FieldInfo> m_RestrictedFields;

        static NonSerializable()
        {
            CreateRestrictedFields();
        }

        public static bool IsNonSerializable( Type type )
        {
            if ( type == typeof ( Fists ) )
                return true;
            if ( typeof ( Packet ).IsAssignableFrom( type ) )
                return true;
            if ( typeof ( Timer ).IsAssignableFrom( type ) )
                return true;
            //if ( type == typeof ( TrashLooter ) )
            //    return true;
            if ( typeof ( BaseAI ).IsAssignableFrom( type ) )
                return true;
            return false;
        }

        private static void CreateRestrictedFields()
        {
            const BindingFlags flags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            m_RestrictedFields = new List<FieldInfo>
                                     {
                                         typeof ( Item ).GetField( "m_Serial", flags ),
                                         typeof ( Mobile ).GetField( "m_Serial", flags ),
                                         typeof ( Item ).GetField( "m_TypeRef", flags ),
                                         typeof ( Mobile ).GetField( "m_TypeRef", flags ),
                                         typeof ( BaseGuild ).GetField( "m_Id", flags ),
                                         typeof ( Skill ).GetField( "m_Owner", flags ),
                                         typeof ( MultiComponentList ).GetField( "m_Tiles", flags ),
                                         //typeof ( XmlSpawner ).GetField( "m_Regions", flags ),
                                         //typeof ( XmlSpawner ).GetField( "PropertyInfoList", flags ),
                                         //typeof ( XmlSpawner ).GetField( "sectorList", flags ),
                                         typeof ( SWPlayerMobile ).GetField( "m_Town", flags ),
                                         typeof ( SWPlayerMobile ).GetField( "m_BuffTable", flags ),
                                         //typeof ( BaseMykonit ).GetField( "m_TrapSpell", flags ),
                                         typeof ( QuestObjective ).GetField( "m_System", flags ),
                                         typeof ( QuestConversation ).GetField( "m_System", flags ),
                                         typeof ( BaseVendor ).GetField( "m_ArmorBuyInfo", flags ),
                                         typeof ( BaseVendor ).GetField( "m_ArmorSellInfo", flags ),
                                         typeof ( Region ).GetField( "m_Players", flags ),
                                         typeof ( Region ).GetField( "m_Mobiles", flags )
                                     };
        }

        public static bool IsRestrictedField( FieldInfo fieldInfo )
        {
            if ( typeof ( BaseVendor ).IsAssignableFrom( fieldInfo.DeclaringType ) && fieldInfo.Name == "m_SBInfos" )
                return true;
            return m_RestrictedFields.Contains( fieldInfo );
        }
    }
}