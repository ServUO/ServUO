using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Server.Engines.Mahjong;
using Server.Engines.Plants;
using Server.Engines.Quests;
using Server.Guilds;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;


namespace Server.XmlSerialize
{
    public static class Fix
    {
        public static string Enum( Type type, string content )
        {
            if( type == typeof( FightMode ) )
                return content.Replace( "Agressor", FightMode.Aggressor.ToString() );
            if( type == typeof( AIType ) )
                return content.Replace( "AI_SmartMage", AIType.AI_Mage.ToString() );
            return content;
        }

        public static string Parsable( Type type, string content )
        {
            if( type == typeof( Map ) )
            {
                if( content == "Felucca" )
                    return "Trammel"; // wir wechseln die Front!
            }
            if( type == typeof( Rectangle2D ) )
            {
                return content.Replace( ")+(", "," ); //neues format
            }
            return content;
        }

        /// <summary>
        /// To provide a special Ctor for objects without a default constructor
        /// </summary>
        /// <param name="type">Type to create</param>
        /// <param name="stack">Object context stack</param>
        /// <returns>a new instance</returns>
        public static object Ctor( Type type, Stack stack )
        {
            if( type == typeof( Skills ) )
                return new Skills( GetParent<Mobile>( stack ) );
            if( type == typeof( Skill ) )
                return new Skill( GetParent<Skills>( stack ), null, 0, 0, 0 );
            if( type == typeof( AosSkillBonuses ) )
                return new AosSkillBonuses( GetParent<Item>( stack ) );
            if( type == typeof( AosAttributes ) )
                return new AosAttributes( GetParent<Item>( stack ) );
            if( type == typeof( AosArmorAttributes ) )
                return new AosArmorAttributes( GetParent<Item>( stack ) );
            if( type == typeof( AosElementAttributes ) )
                return new AosElementAttributes( GetParent<Item>( stack ) );
            if( type == typeof( AosWeaponAttributes ) )
                return new AosWeaponAttributes( GetParent<Item>( stack ) );
            if( type == typeof( SecureInfo ) )
                return new SecureInfo( GetParent<Container>( stack ), SecureLevel.Owner, null );
            if( type == typeof( PlantSystem ) )
                return new PlantSystem( GetParent<PlantItem>( stack ), true );
            if( type == typeof( DesignState ) )
                return new DesignState( GetParent<HouseFoundation>( stack ), MultiComponentList.Empty );
            if( type == typeof( QuestRestartInfo ) )
                return new QuestRestartInfo( null, DateTime.MinValue );
            if( type == typeof( PlayerBBMessage ) )
                return new PlayerBBMessage( DateTime.MinValue, null, null );
            if( type == typeof( MahjongDices ) )
                return new MahjongDices( GetParent<MahjongGame>( stack ) );
            if( type == typeof( MahjongPlayers ) )
                return new MahjongPlayers( GetParent<MahjongGame>( stack ), 0, 0 );
            if( type == typeof( MahjongWallBreakIndicator ) )
                return new MahjongWallBreakIndicator( GetParent<MahjongGame>( stack ), Point2D.Zero );
            if( type == typeof( StatMod ) )
                return new StatMod( StatType.Str, null, 0, TimeSpan.Zero );
            if( type == typeof( ShardPollOption ) )
                return new ShardPollOption( (string)null );

            //typeof( BOBSmallEntry )
            //typeof( BOBLargeEntry )
            return FormatterServices.GetUninitializedObject( type );
        }

        /// <summary>
        ///   Adjust an object before it gets used. Use it for a versioning issue
        /// </summary>
        /// <param name = "valueType">Type of the value to use</param>
        /// <param name = "value">value to use</param>
        /// <param name = "stack">Stack of objects</param>
        public static void Adjust( Type valueType, ref object value, Stack stack )
        {
            if( valueType == typeof( MultiTileEntry ) )
            {
                MultiTileEntry entry = (MultiTileEntry)value;
                entry.m_ItemID = (ushort)( entry.m_ItemID & ~0x4000 ); // don't allow a multi-flag
                value = entry;
            }
        }

        private static T GetParent<T>( Stack stack )
        {
            foreach( object obj in stack )
            {
                if( obj is T )
                    return (T)obj;
            }
            return default( T );
        }

        public static string Primitive( Type type, string typeName, string content )
        {
            if( typeName == "Server.Mobiles.PetLoyalty" )
                return BaseCreature.MaxLoyalty.ToString(); // enum wurde entfernt
            return content;
        }

        public static void CleanUp( BaseGuild guild )
        {
            if( !( guild is Guild ) )
                return;

            foreach( Mobile member in ( (Guild)guild ).Members )
            {
                if( member is PlayerMobile )
                    ( (PlayerMobile)member ).GuildRank = RankDefinition.Member;
            }
            if( ( (Guild)guild ).Leader is PlayerMobile )
                ( (PlayerMobile)( (Guild)guild ).Leader ).GuildRank = RankDefinition.Leader;
        }

        public static void CleanUp( IEntity entity )
        {
        }

        public static object GetDefaults( FieldInfo field, object obj )
        {
            Type type = field.FieldType;
            if( type == typeof( AosSkillBonuses ) )
                return new AosSkillBonuses( (Item)obj );
            if( type == typeof( AosAttributes ) )
                return new AosAttributes( (Item)obj );
            if( type == typeof( AosArmorAttributes ) )
                return new AosArmorAttributes( (Item)obj );
            if( type == typeof( AosElementAttributes ) )
                return new AosElementAttributes( (Item)obj );
            if( type == typeof( AosWeaponAttributes ) )
                return new AosWeaponAttributes( (Item)obj );
            if( type == typeof( PlayerMobile.ChampionTitleInfo ) )
                return new PlayerMobile.ChampionTitleInfo();
            if( type == typeof( RankDefinition ) )
                return RankDefinition.Lowest;

            Type foundType;
            if( type.Implements( typeof( List<> ), out foundType ) )
                return Activator.CreateInstance( foundType );
            if( type.Implements( typeof( Dictionary<,> ), out foundType ) )
                return Activator.CreateInstance( foundType );
            return null;
        }

        public static ValueSetter Redirect( Type type, string fieldName )
        {
            if( type == typeof( Item ) && fieldName == "m_Name" )
                return CreatePropertyRedirector<Item>( "Name" );
            if( type == typeof( Item ) && fieldName == "m_Items" )
                return new ValueSetter( "Items", typeof( List<Item> ), AquireAddItems );

            if( type == typeof( Item ) && fieldName == "m_TypeRef" )
                return new ValueSetter( fieldName, null, null ); // restricted
            if( type == typeof( Mobile ) && fieldName == "m_TypeRef" )
                return new ValueSetter( fieldName, null, null ); // restricted
            return null;
        }

        private static void AquireAddItems( object owner, object items )
        {
            if( items == null )
                return;

            Item ownerItem = (Item)owner;
            List<Item> itemList = ownerItem.AcquireItems();
            foreach( Item item in (IEnumerable)items )
                itemList.Add( item );
        }

        private static ValueSetter CreatePropertyRedirector<T>( string propertyName )
        {
            const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            PropertyInfo property = typeof( T ).GetProperty( propertyName, flags );
            if( property == null )
                throw new ArgumentException( "Property not found: " + typeof( T ).FullName + "." + propertyName, "propertyName" );

            MethodInfo setter = property.GetSetMethod();
            if( setter == null )
                throw new ArgumentException( "Property has no setter: " + typeof( T ).FullName + "." + propertyName, "propertyName" );
            return new ValueSetter( property.Name, property.PropertyType, ( o, v ) => setter.Invoke( o, new[] { v } ) );
        }

#if DEBUG
        /// <summary>
        ///   Debug helper, use it like
        ///   XmlSerialize.Fix.SetDefaultForAll(typeof(Server.Engines.CannedEvil.ChampionSpawn),"m_DamageEntries")
        /// </summary>
        /// <param name = "type"></param>
        /// <param name = "field"></param>
        /// <returns></returns>
        public static string SetDefaultForAll( Type type, string field )
        {
            if( !typeof( IEntity ).IsAssignableFrom( type ) )
                return "Not an IEntity";

            const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo fieldInfo = type.GetField( field, flags );
            if( fieldInfo == null )
                return "Field not found " + type.Name + "." + field;

            if( typeof( Item ).IsAssignableFrom( type ) )
            {
                foreach( Item item in World.Items.Values )
                {
                    if( type.IsAssignableFrom( item.GetType() ) )
                    {
                        object value = GetDefaults( fieldInfo, item );
                        if( value == null )
                            return "No default declared for " + fieldInfo.FieldType.Name + " " + type.Name + "." + field;
                        fieldInfo.SetValue( item, value );
                    }
                }
                return "Ok!";
            }
            if( typeof( Mobile ).IsAssignableFrom( type ) )
            {
                foreach( Mobile mobile in World.Mobiles.Values )
                {
                    if( type.IsAssignableFrom( mobile.GetType() ) )
                    {
                        object value = GetDefaults( fieldInfo, mobile );
                        if( value == null )
                            return "No default declared for " + fieldInfo.FieldType.Name + " " + type.Name + "." + field;
                        fieldInfo.SetValue( mobile, value );
                    }
                }
                return "Ok!";
            }
            return "!!!";
        }
#endif
    }
}
