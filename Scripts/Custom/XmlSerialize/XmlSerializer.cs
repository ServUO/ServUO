using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Server.Accounting;
using Server.Guilds;


namespace Server.XmlSerialize
{
    public class XmlSerializer : IDisposable
    {
        //public static void Configure()
        //{
        //    EventSink.ServerStarted += SaveWorld;
        //}

        public static void SaveWorld()
        {
            using( XmlSerializer serializer = new XmlSerializer( "runuo1.xml" ) )
            {
                serializer.Serialize( Accounts.GetAccounts().Cast<Account>() );
                //serializer.Serialize( Region.Regions.Cast<Region>() );
                serializer.Serialize( BaseGuild.List.Values );
                serializer.Serialize( World.Mobiles.Values );
                serializer.Serialize( World.Items.Values );
            }
        }

        private readonly XmlTextWriter m_Writer;

        public XmlSerializer( Stream stream )
        {
            m_Writer = new XmlTextWriter( stream, Encoding.UTF8 );
            InitializeXmlTextWriter( m_Writer );
        }

        public XmlSerializer( string filename )
        {
            m_Writer = new XmlTextWriter( filename, Encoding.UTF8 );
            InitializeXmlTextWriter( m_Writer );
        }

        private static void InitializeXmlTextWriter( XmlTextWriter writer )
        {
            writer.Formatting = Formatting.Indented;
            writer.IndentChar = '\t';
            writer.Indentation = 1;
        }

        public void Serialize( IEnumerable<Account> accounts )
        {
            EnsureStart();
            foreach( Account account in accounts )
                account.Save( m_Writer );
        }

        //public void Serialize( IEnumerable<Region> regions )
        //{
        //    EnsureStart();
        //    foreach ( Region region in regions )
        //    {
        //        if ( !( region is SavingRegion ) )
        //            continue;
        //        // Nur SavingRegion sind auch einzeln serialisierbar, alle anderen werden per xml oder sonstiger serialisierung erzeugt

        //        Type type = region.GetType();
        //        if ( NonSerializable.IsNonSerializable( type ) )
        //            continue;

        //        m_Writer.WriteStartElement( "region" );
        //        m_Writer.WriteAttributeString( "id", GetRegionReference( region ) );
        //        WriteType( region, type );
        //        m_Writer.WriteEndElement();
        //    }
        //}

        public void Serialize( IEnumerable<IEntity> entities )
        {
            EnsureStart();
            foreach( IEntity entity in entities )
            {
                Type type = entity.GetType();
                if( NonSerializable.IsNonSerializable( type ) )
                    continue;

                m_Writer.WriteStartElement( "entity" );
                m_Writer.WriteAttributeString( "serial", String.Format( "0x{0:X8}", (int)entity.Serial ) );
                m_Writer.WriteAttributeString( "type", type.FullName );
                WriteType( entity, type );
                m_Writer.WriteEndElement();
            }
        }

        public void Serialize( IEnumerable<BaseGuild> guilds )
        {
            EnsureStart();
            foreach( BaseGuild guild in guilds )
            {
                Type type = guild.GetType();
                if( NonSerializable.IsNonSerializable( type ) )
                    continue;

                m_Writer.WriteStartElement( "guild" );
                m_Writer.WriteAttributeString( "id", guild.Id.ToString() );
                m_Writer.WriteAttributeString( "type", type.FullName );
                WriteType( guild, type );
                m_Writer.WriteEndElement();
            }
        }

        private static readonly List<Assembly> SerializableAssemblies = new List<Assembly>
                                                                            {
                                                                                    typeof( Item ).Assembly,
                                                                                    typeof( AOS ).Assembly
                                                                            };

        private void WriteType( object obj, Type type )
        {
            if( type.BaseType != null && SerializableAssemblies.Contains( type.BaseType.Assembly ) )
            {
                m_Writer.WriteStartElement( "base" );
                m_Writer.WriteAttributeString( "type", type.FullName );
                WriteType( obj, type.BaseType );
                m_Writer.WriteEndElement();
            }

            WriteFields( obj, type );
        }

        private void WriteFields( object obj, Type type )
        {
            const BindingFlags flags =
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            FieldInfo[] fields = type.GetFields( flags );
            foreach( FieldInfo fieldInfo in fields )
            {
                if( NonSerializable.IsNonSerializable( fieldInfo.FieldType ) ||
                    NonSerializable.IsRestrictedField( fieldInfo ) )
                    continue;

                m_Writer.WriteStartElement( "field" );
                m_Writer.WriteAttributeString( "name", fieldInfo.Name );
                object value = fieldInfo.GetValue( obj );
                WriteField( value );
                m_Writer.WriteEndElement();
            }
        }

#if TRACE
        private readonly Dictionary<Type, int> m_TypeCount = new Dictionary<Type, int>( 40 );
#endif

        private void WriteField( object value )
        {
            if( value == null )
            {
                m_Writer.WriteAttributeString( "ref", "null" );
                return;
            }

            Type type = value.GetType();
            m_Writer.WriteAttributeString( "type", type.FullName );

            if( type.IsPrimitive )
            {
                m_Writer.WriteString( value.ToString() );
            }
            else if( type.IsEnum )
            {
                m_Writer.WriteString( value.ToString() );
            }
            else if( type == typeof( string ) )
            {
                m_Writer.WriteString( (string)value );
            }
            else if( type == typeof( DateTime ) )
            {
                m_Writer.WriteString( XmlConvert.ToString( (DateTime)value, XmlDateTimeSerializationMode.Utc ) );
            }
            else if( type == typeof( DateTimeOffset ) )
            {
                m_Writer.WriteString( XmlConvert.ToString( (DateTimeOffset)value ) );
            }
            else if( type == typeof( TimeSpan ) )
            {
                m_Writer.WriteString( XmlConvert.ToString( (TimeSpan)value ) );
            }
            else if( type == typeof( Guid ) )
            {
                m_Writer.WriteString( XmlConvert.ToString( (Guid)value ) );
            }
            else if( value is Type )
            {
                m_Writer.WriteString( ( (Type)value ).FullName );
            }
            else if( value is BaseGuild )
            {
                m_Writer.WriteAttributeString( "ref", String.Format( "0x{0:X8}", ( (BaseGuild)value ).Id ) );
                m_Writer.WriteString( value.ToString() );
            }
            else if( type.GetCustomAttributes( typeof( ParsableAttribute ), true ).Length > 0 )
            {
                m_Writer.WriteString( value.ToString() );
            }
            else if( value is IEntity )
            {
                m_Writer.WriteAttributeString( "ref", String.Format( "0x{0:X8}", (int)( (IEntity)value ).Serial ) );
            }
            else if( value is SkillInfo )
            {
                m_Writer.WriteString( String.Format( "{0}:{1}", ( (SkillInfo)value ).SkillID, ( (SkillInfo)value ).Name ) );
            }
            else if( value is Region )
            {
                m_Writer.WriteAttributeString( "ref", GetRegionReference( (Region)value ) );
                m_Writer.WriteString( ( (Region)value ).Name );
            }
            else if( value is Account )
            {
                m_Writer.WriteAttributeString( "ref", ( (Account)value ).Username );
            }
            else if( typeof( IEnumerable ).IsAssignableFrom( type ) )
            {
                foreach( object obj in (IEnumerable)value )
                {
                    m_Writer.WriteStartElement( "element" );
                    WriteField( obj );
                    m_Writer.WriteEndElement();
                }
            }
            else
            {
#if TRACE
                int count;
                if( m_TypeCount.TryGetValue( type, out count ) )
                    m_TypeCount[type] = count + 1;
                else
                    m_TypeCount[type] = 1;
#endif

                WriteType( value, type );
            }
        }

        private static string GetRegionReference( Region region )
        {
            return String.Format( "{0}:{1}", ( region ).Map.MapID, ( region ).GoLocation );
        }

        private void EnsureStart()
        {
            if( m_Writer.WriteState == WriteState.Start )
                m_Writer.WriteStartDocument( true );
            if( m_Writer.WriteState == WriteState.Prolog )
            {
                m_Writer.WriteStartElement( "entities" );
                m_Writer.WriteAttributeString( "date",
                                               XmlConvert.ToString( DateTime.Now, XmlDateTimeSerializationMode.Utc ) );
            }
        }

        public void Dispose()
        {
            EnsureStart();
            m_Writer.WriteEndDocument();
            m_Writer.Close();
        }
    }
}