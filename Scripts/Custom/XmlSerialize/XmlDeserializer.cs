using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Server.Accounting;
using Server.Guilds;


namespace Server.XmlSerialize
{
    public class XmlDeserializer
    {
        private static readonly ObjectActivator<Serial> SerialActivator = ReflectionUtility.GetActivator<Serial>( typeof( int ) );
        private static readonly string[] EnumSplitter = new[] { ", " };
        private readonly Mobile m_Caller;
        private readonly Dictionary<Serial, IEntity> m_Entities = new Dictionary<Serial, IEntity>( 1000 );
        private readonly Dictionary<string, int> m_FieldsNotFound = new Dictionary<string, int>();
        private readonly string m_Filepath;
        private readonly Dictionary<string, ValueSetter> m_FoundSetters = new Dictionary<string, ValueSetter>( 100 );
        private readonly Dictionary<string, int> m_InvalidSetters = new Dictionary<string, int>();
        private readonly Stack m_ObjectStack = new Stack();
        private readonly Dictionary<string, int> m_TypesNotFound = new Dictionary<string, int>();

        private XmlDeserializer( string filepath, Mobile caller )
        {
            m_Filepath = filepath;
            m_Caller = caller;
        }

        public static void Configure()
        {
            // EventSink.WorldLoad += LoadWorld;
            // EventSink.ServerStarted += Save;

        }

        public static void Save()
        {
            Console.WriteLine($"Accounts: {Accounts.Count}");
            Accounts.Save(null);
            World.Save();
            Console.WriteLine("Test Saved");
        }
        public static void LoadWorld()
        {
            if( !File.Exists( "./runuo1.xml" ) )
                return;

            XmlDeserializer deserializer = new XmlDeserializer( "runuo1.xml", null );
            deserializer.Deserialize();
        }

        private void WriteLine( string format, params object[] args )
        {
            string message = string.Format( format, args );
            if( m_Caller == null )
                Console.WriteLine( message );
            else
                m_Caller.SendMessage( message );
        }

        private void Deserialize()
        {
            if( m_Caller == null )
                Console.WriteLine();

            WriteLine( "Starting XmlDeserialize for " + m_Filepath );
            WriteLine( "Creating all entities..." );
            long count = CreateEntities();
            WriteLine( "{0} found! Start loading...", count );
            LoadData( count );
            WriteLine( "XmlDeserialize done!" );

            LogMissing( m_TypesNotFound, "MissingTypes" );
            LogMissing( m_FieldsNotFound, "MissingFields" );
            LogMissing( m_InvalidSetters, "InvalidSetters" );

            CleanUp();
            Reserialize();
        }

        /// <summary>
        ///   Reserialization is needed to create a correct state of all objects
        /// </summary>
        private void Reserialize()
        {
            WriteLine( "Reserialization is needed to create a correct state of all objects" );
            Reserialize( BaseGuild.List.Values, BaseGuild.List.Count, "Guilds" );
            Reserialize( m_Entities.Values, m_Entities.Count, "Entities" );
            Reserialize( BaseGuild.List.Values, BaseGuild.List.Count, "Guilds" );
        }

        private void Reserialize( IEnumerable<object> serializableList, int count, string name )
        {
            WriteLine( "Starting Reserialization of " + name + " for " + m_Filepath );
            int current = 0;
            DateTime lastMessage = DateTime.Now;

            foreach( ISerializable serializable in serializableList )
            {
                try
                {
                    BinaryMemoryWriter writer = new BinaryMemoryWriter();
                    serializable.Serialize( writer );
                    serializable.SerializeSW( writer );

                    Stream stream = writer.UnderlyingStream;
                    stream.Position = 0L;
                    using( BinaryReader binaryReader = new BinaryReader( stream ) )
                    {
                        BinaryFileReader reader = new BinaryFileReader( binaryReader );
                        if( serializable is Item )
                        {
                            ( (Item)serializable ).Deserialize( reader );
                            ( (Item)serializable ).DeserializeSW( reader );
                        }
                        else if( serializable is Mobile )
                        {
                            ( (Mobile)serializable ).Deserialize( reader );
                            ( (Mobile)serializable ).DeserializeSW( reader );
                        }
                        else if( serializable is Guild )
                        {
                            ( (Guild)serializable ).Deserialize( reader );
                            ( (Guild)serializable ).DeserializeSW( reader );
                        }
                    }
                }
                catch( Exception )
                {
                }
                current++;

                if( DateTime.Now - lastMessage > TimeSpan.FromSeconds( 2 ) )
                {
                    WriteLine( "{0:0.00}%", (double)current * 100 / count );
                    lastMessage = DateTime.Now;
                }
            }
        }

        private void CleanUp()
        {
            foreach( BaseGuild guild in BaseGuild.List.Values )
            {
                Fix.CleanUp( guild );
            }
            foreach( IEntity entity in m_Entities.Values )
            {
                Fix.CleanUp( entity );
            }
        }

        private long CreateEntities()
        {
            long count = 0L;
            using( StreamReader stream = new StreamReader( m_Filepath ) )
            using( XmlReader reader = XmlReader.Create( stream ) )
            {
                //List<Type> worldItemTypes = (List<Type>)ReflectionUtility.GetStaticFieldValue( typeof( World ), "m_ItemTypes" );
                //List<Type> worldMobileTypes = (List<Type>)ReflectionUtility.GetStaticFieldValue( typeof( World ), "m_MobileTypes" );

                while( reader.Read() )
                {
                    if( reader.NodeType == XmlNodeType.Element )
                    {
                        switch( reader.Name )
                        {
                            case "account":
                            case "region":
                                reader.Skip();
                                count++;
                                break;
                            case "guild":
                                using( XmlReader subReader = reader.ReadSubtree() )
                                {
                                    subReader.Read();
                                    int id = Convert.ToInt32( reader.GetAttribute( "id" ) );
                                    Type type = FindType( subReader );
                                    if( type != null )
                                    {
                                        if( BaseGuild.Find( id ) == null )
                                        {
                                            BaseGuild guild = (BaseGuild)Activator.CreateInstance( type, id );
                                            InitializeDefaults( guild.GetType(), guild );
                                        }
                                    }
                                }
                                count++;
                                break;
                            case "entity":
                                using( XmlReader subReader = reader.ReadSubtree() )
                                {
                                    subReader.Read();
                                    Serial serial = SerialActivator( ConvertHex( reader.GetAttribute( "serial" ) ) );
                                    Type type = FindType( subReader );
                                    if( type != null )
                                    {
                                        if( World.FindEntity( serial ) != null )
                                        {
                                            serial = typeof( Item ).IsAssignableFrom( type ) //
                                                             ? Serial.NewItem //
                                                             : Serial.NewMobile;
                                        }
                                        IEntity entity = (IEntity)Activator.CreateInstance( type, serial );
                                        InitializeDefaults( entity.GetType(), entity );
                                        m_Entities[serial] = entity;
                                        if( entity is Item )
                                        {
                                            World.AddItem( (Item)entity );

                                            //int typeref = worldItemTypes.IndexOf( type );
                                            //if( typeref == -1 )
                                            //{
                                            //    worldItemTypes.Add( type );
                                            //    typeref = worldItemTypes.Count - 1;
                                            //}
                                            //ReflectionUtility.SetFieldValue( entity, typeof( Item ), "m_TypeRef" );
                                        }
                                        else if( entity is Mobile )
                                        {
                                            World.AddMobile( (Mobile)entity );
                                        }
                                    }
                                }
                                count++;
                                break;
                            case "entities":
                                break;
                            default:
                                reader.Skip();
                                break;
                        }
                    }
                }
            }
            return count;
        }

        private static void InitializeDefaults( Type type, object obj )
        {
            if( type.BaseType != null )
                InitializeDefaults( type.BaseType, obj );

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            FieldInfo[] fields = type.GetFields( flags );
            foreach( FieldInfo fieldInfo in fields )
            {
                object value = Fix.GetDefaults( fieldInfo, obj );
                if( value != null )
                    fieldInfo.SetValue( obj, value );
            }
        }

        private static int SortCountDescending( KeyValuePair<string, int> x, KeyValuePair<string, int> y )
        {
            return -x.Value.CompareTo( y.Value );
        }

        private static int ConvertHex( string hex )
        {
            if( hex.StartsWith( "0x", StringComparison.CurrentCultureIgnoreCase ) )
                hex = hex.Substring( 2 );
            return int.Parse( hex, NumberStyles.HexNumber );
        }

        private Type FindType( XmlReader reader )
        {
            string typeName = reader.GetAttribute( "type" );
            if( typeName == null )
                return null;

            Type type = ScriptCompiler.FindTypeByFullName( typeName ) ?? Type.GetType( typeName );
            if( type != null && !type.IsAbstract )
                return type;

            while( reader.ReadToDescendant( "base" ) )
            {
                string oldTypeName = typeName;
                typeName = reader.GetAttribute( "type" );
                if( type == null && oldTypeName != null )
                {
                    int count;
                    if( m_TypesNotFound.TryGetValue( oldTypeName, out count ) )
                        m_TypesNotFound[oldTypeName] = count + 1;
                    else
                        m_TypesNotFound[oldTypeName] = 1;
                }
                type = ScriptCompiler.FindTypeByFullName( typeName );
                if( type != null && !type.IsAbstract )
                    return type;
            }
            return null;
        }

        private void LoadData( long count )
        {
            long current = 0L;
            DateTime lastMessage = DateTime.Now;

            using( StreamReader stream = new StreamReader( m_Filepath ) )
            using( XmlReader reader = XmlReader.Create( stream ) )
            {
                while( reader.Read() )
                {
                    if( reader.NodeType == XmlNodeType.Element )
                    {
                        switch( reader.Name )
                        {
                            case "account":
                                using( XmlReader subReader = reader.ReadSubtree() )
                                {
                                    try
                                    {
                                        XmlDocument doc = new XmlDocument();
                                        doc.Load(subReader);
                                        var acc = new Account(doc.DocumentElement);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Warning: Account instance load failed from XML Save");
                                    }

                                }
                                current++;

                                break;

                            case "region":
                                reader.Skip();
                                current++;
                                break;
                            case "guild":
                                int id = Convert.ToInt32( reader.GetAttribute( "id" ) );
                                BaseGuild guild = BaseGuild.Find( id );
                                if( guild != null )
                                    ReadType( reader, guild );
                                else
                                    reader.Skip();
                                current++;
                                break;
                            case "entity":
                                Serial serial = SerialActivator( ConvertHex( reader.GetAttribute( "serial" ) ) );
                                IEntity entity;
                                if( m_Entities.TryGetValue( serial, out entity ) )
                                {
                                    ReadType( reader, entity );
                                    if( entity is Item && entity.Map != null && ( (Item)entity ).Parent == null )
                                        entity.Map.OnEnter( (Item)entity );
                                    else if( entity is Mobile && entity.Map != null )
                                        entity.Map.OnEnter( (Mobile)entity );
                                }
                                else
                                    reader.Skip();
                                current++;
                                break;
                            case "entities":
                                break;
                            default:
                                reader.Skip();
                                break;
                        }
                    }

                    if( DateTime.Now - lastMessage > TimeSpan.FromSeconds( 2 ) )
                    {
                        WriteLine( "{0:0.00}%", (double)current * 100 / count );
                        lastMessage = DateTime.Now;
                    }
                }
            }
        }

        private void LogMissing( Dictionary<string, int> notFound, string name )
        {
            List<KeyValuePair<string, int>> sortedList = notFound.ToList();
            sortedList.Sort( SortCountDescending );
            using( StreamWriter stream = new StreamWriter( Path.GetFileName( m_Filepath ) + "." + name + ".txt", false, Encoding.UTF8 ) )
            {
                foreach( KeyValuePair<string, int> keyValuePair in sortedList )
                    stream.WriteLine( "'{0}' wasn't found ({1} times)", keyValuePair.Key, keyValuePair.Value );
            }
        }

        private void ReadType( XmlReader mainReader, object obj )
        {
            try
            {
                m_ObjectStack.Push( obj );
                using( XmlReader reader = mainReader.ReadSubtree() )
                {
                    reader.Read();
                    string typeName = reader.GetAttribute( "type" );
                    Type type = ScriptCompiler.FindTypeByFullName( typeName );

                    while( reader.Read() )
                    {
                        if( reader.NodeType == XmlNodeType.Element )
                        {
                            switch( reader.Name )
                            {
                                case "field":
                                    if( type != null && type.IsInstanceOfType( obj ) )
                                        ReadField( reader, obj, type );
                                    else
                                        reader.Skip();
                                    break;
                                case "base":
                                    ReadType( reader, obj );
                                    break;
                                default:
                                    reader.Skip();
                                    break;
                            }
                        }
                    }
                }
            }
            finally
            {
                m_ObjectStack.Pop();
            }
        }

        private Action<object, object> GetValueSetter( Type type, ref string fieldName, out Type valueType )
        {
            string fieldNameKey = type.FullName + "." + fieldName;
            ValueSetter foundSetter;
            if( !m_FoundSetters.TryGetValue( fieldNameKey, out foundSetter ) )
            {
                foundSetter = Fix.Redirect( type, fieldName );
                if( foundSetter == null )
                {
                    const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    FieldInfo field = type.GetField( fieldName, flags );
                    if( field != null )
                        foundSetter = new ValueSetter( fieldName, field.FieldType, field.SetValue );
                }
                m_FoundSetters[fieldNameKey] = foundSetter;
            }

            if( foundSetter != null )
            {
                fieldName = foundSetter.NewFieldName;
                valueType = foundSetter.ValueType;
                return foundSetter.Setter;
            }

            int count;
            if( m_FieldsNotFound.TryGetValue( fieldNameKey, out count ) )
                m_FieldsNotFound[fieldNameKey] = count + 1;
            else
                m_FieldsNotFound[fieldNameKey] = 1;
            valueType = null;
            return null;
        }

        private void ReadField( XmlReader mainReader, object obj, Type type )
        {
            try
            {
                m_ObjectStack.Push( obj );
                using( XmlReader reader = mainReader.ReadSubtree() )
                {
                    reader.Read();
                    string fieldName = reader.GetAttribute( "name" );
                    if( fieldName == null )
                        return;

                    Type valueType;
                    Action<object, object> setter = GetValueSetter( type, ref fieldName, out valueType );
                    if( setter == null )
                        return;

                    object value;
                    if( CreateValue( reader, valueType, out value ) )
                    {
                        if( value == null || valueType.IsAssignableFrom( value.GetType() ) )
                        {
                            Fix.Adjust( valueType, ref value, m_ObjectStack );
                            setter( obj, value );
                        }
                        else
                        {
                            string invalidSetterKey = type.FullName + "." + fieldName + " = " + value.GetType().Name;
                            int count;
                            if( m_InvalidSetters.TryGetValue( invalidSetterKey, out count ) )
                                m_InvalidSetters[invalidSetterKey] = count + 1;
                            else
                                m_InvalidSetters[invalidSetterKey] = 1;
                        }
                    }
                }
            }
            finally
            {
                m_ObjectStack.Pop();
            }
        }

        private bool CreateValue( XmlReader mainReader, Type type, out object value )
        {
            using( XmlReader reader = mainReader.ReadSubtree() )
            {
                reader.Read();
                string typeName = reader.GetAttribute( "type" );
                string reference = reader.GetAttribute( "ref" );
                if( typeName == null && string.Equals( reference, "null", StringComparison.InvariantCultureIgnoreCase ) )
                {
                    value = null;
                    return true;
                }

                try
                {
                    if( reference != null )
                    {
                        if( typeof( IAccount ).IsAssignableFrom( type ) )
                        {
                            value = Accounts.GetAccount( reference );
                            return value != null;
                        }
                        if( typeof( Region ).IsAssignableFrom( type ) )
                        {
                            string name = reader.ReadString();
                            value = GetReferencedRegion( reference, name );
                            return value != null;
                        }
                        if( typeof( BaseGuild ).IsAssignableFrom( type ) )
                        {
                            int id = ConvertHex( reference );
                            value = BaseGuild.Find( id );
                            return value != null;
                        }
                        if( reference.StartsWith( "0x" ) )
                        {
                            Serial serial = SerialActivator( ConvertHex( reference ) );
                            IEntity entity;
                            bool found = m_Entities.TryGetValue( serial, out entity );
                            value = entity;
                            return found;
                        }
                    }

                    if( type.IsPrimitive )
                    {
                        string content = reader.ReadString();
                        content = Fix.Primitive( type, typeName, content );
                        value = Convert.ChangeType( content, type );
                        return true;
                    }
                    if( type.IsEnum )
                    {
                        return TryParseEnum( type, reader.ReadString(), out value );
                    }
                    if( type == typeof( string ) )
                    {
                        value = reader.ReadString();
                        return true;
                    }
                    if( type == typeof( DateTime ) )
                    {
                        value = XmlConvert.ToDateTime( reader.ReadString(), XmlDateTimeSerializationMode.Utc ).ToLocalTime();
                        return true;
                    }
                    if( type == typeof( DateTimeOffset ) )
                    {
                        value = XmlConvert.ToDateTimeOffset( reader.ReadString() );
                        return true;
                    }
                    if( type == typeof( TimeSpan ) )
                    {
                        value = XmlConvert.ToTimeSpan( reader.ReadString() );
                        return true;
                    }
                    if( type == typeof( Guid ) )
                    {
                        value = XmlConvert.ToGuid( reader.ReadString() );
                        return true;
                    }
                    if( type == typeof( Type ) )
                    {
                        string searchType = reader.ReadString();
                        Type foundType = ScriptCompiler.FindTypeByFullName( searchType ) ?? Type.GetType( searchType );
                        if( foundType == null )
                        {
                            int count;
                            if( m_TypesNotFound.TryGetValue( searchType, out count ) )
                                m_TypesNotFound[searchType] = count + 1;
                            else
                                m_TypesNotFound[searchType] = 1;
                            value = null;
                            return false;
                        }
                        value = foundType;
                        return true;
                    }
                    if( type.GetCustomAttributes( typeof( ParsableAttribute ), true ).Length > 0 )
                    {
                        string content = reader.ReadString();
                        content = Fix.Parsable( type, content );
                        MethodInfo parseMethod = type.GetMethod( "Parse", new[] { typeof( string ) } );
                        value = parseMethod.Invoke( null, new object[] { content } );
                        return true;
                    }
                    if( type == typeof( SkillInfo ) )
                    {
                        string content = reader.ReadString();
                        int splitter = content.IndexOf( ':' );
                        content = content.Substring( 0, splitter );
                        int skillIndex = Convert.ToInt32( content );
                        value = SkillInfo.Table[skillIndex];
                        return true;
                    }

                    if( type.IsArray || typeof( IList ).IsAssignableFrom( type ) )
                    {
                        List<object> elements = ReadElements( reader );
                        if( elements == null )
                        {
                            value = null;
                            return true;
                        }
                        if( type.IsArray )
                        {
                            foreach( object element in elements ) // security check
                            {
                                if( element != null && !type.GetElementType().IsAssignableFrom( element.GetType() ) )
                                {
                                    value = Array.CreateInstance( element.GetType(), elements.Count ); // provoke an invalid set
                                    return true;
                                }
                            }
                            Array array = Array.CreateInstance( type.GetElementType(), elements.Count );
                            elements.ToArray().CopyTo( array, 0 );
                            value = array;
                            return true;
                        }
                        if( type.IsInterface || type.IsAbstract )
                        {
                            value = null;
                            return false;
                        }
                        //if(typeof(IDictionary).IsAssignableFrom( type ))
                        //{
                        //    ConstructorInfo dicCtor = type.GetConstructor( new Type[0] );
                        //    IDictionary dic = (IDictionary)dicCtor.Invoke( null );
                        //    foreach( object element in elements )
                        //        dic.Add( element );Hashtable

                        //}
                        if( typeof( IList ).IsAssignableFrom( type ) )
                        {
                            ConstructorInfo listCtor = type.GetConstructor( new Type[0] );
                            IList list = (IList)listCtor.Invoke( null );
                            foreach( object element in elements )
                                list.Add( element );
                            value = list;
                            return true;
                        }

                        value = null;
                        return false;
                    }

                    return CreateComplexValue( reader, out value );
                }
                catch
                {
                }

                value = null;
                return false;
            }
        }

        private bool CreateComplexValue( XmlReader mainReader, out object value )
        {
            using( XmlReader reader = mainReader.ReadSubtree() )
            {
                reader.Read();
                Type type = FindType( reader );
                if( type == null )
                {
                    value = null;
                    return false;
                }

                if( type.IsValueType )
                    value = Activator.CreateInstance( type );
                else if( type == typeof( string ) ) // specially for changed fields
                {
                    value = string.Empty;
                    return true;
                }
                else
                {
                    ConstructorInfo ctor = type.GetConstructor( new Type[0] );
                    if( ctor != null )
                        value = ctor.Invoke( null );
                    else
                        value = Fix.Ctor( type, m_ObjectStack );
                }

                if( value != null )
                {
                    ReadType( reader, value );
                    return true;
                }

                int count;
                string fieldNameKey = type.FullName + ".ctor()";
                if( m_FieldsNotFound.TryGetValue( fieldNameKey, out count ) )
                    m_FieldsNotFound[fieldNameKey] = count + 1;
                else
                    m_FieldsNotFound[fieldNameKey] = 1;
                return false;
            }
        }

        private List<object> ReadElements( XmlReader mainReader )
        {
            using( XmlReader reader = mainReader.ReadSubtree() )
            {
                reader.Read();
                if( reader.GetAttribute( "ref" ) == "null" )
                    return null;

                List<object> elements = new List<object>();
                while( reader.Read() )
                {
                    if( reader.NodeType == XmlNodeType.Element )
                    {
                        switch( reader.Name )
                        {
                            case "element":
                                using( XmlReader subReader = mainReader.ReadSubtree() )
                                {
                                    subReader.Read();
                                    Type type = FindType( subReader );
                                    object element;
                                    if( CreateValue( subReader, type, out element ) )
                                    {
                                        Fix.Adjust( type, ref element, m_ObjectStack );
                                        elements.Add( element );
                                    }
                                }
                                break;
                            default:
                                reader.Skip();
                                break;
                        }
                    }
                }
                return elements;
            }
        }

        private bool TryParseEnum( Type type, string content, out object value )
        {
            content = Fix.Enum( type, content );
            long num;
            if( long.TryParse( content, out num ) )
            {
                value = Enum.ToObject( type, num );
                return true;
            }

            bool abort = false;
            string[] contentFlags = content.Split( EnumSplitter, StringSplitOptions.None );
            string[] flags = Enum.GetNames( type );
            foreach( string contentFlag in contentFlags )
            {
                if( flags.Contains( contentFlag ) )
                    continue;

                abort = true;
                int count;
                string fieldNameKey = type.FullName + "." + contentFlag;
                if( m_FieldsNotFound.TryGetValue( fieldNameKey, out count ) )
                    m_FieldsNotFound[fieldNameKey] = count + 1;
                else
                    m_FieldsNotFound[fieldNameKey] = 1;
            }
            value = !abort ? Enum.Parse( type, content ) : null;
            return !abort;
        }

        private static Region GetReferencedRegion( string reference, string name )
        {
            int splitter = reference.IndexOf( ':' );
            int mapId = Convert.ToInt32( reference.Substring( 0, splitter ) );

            string mapName = Map.Maps[mapId].Name;
            mapName = Fix.Parsable( typeof( Map ), mapName );
            Map map = Map.Parse( mapName );

            Point3D loc = Point3D.Parse( reference.Substring( splitter + 1 ) );

            return Region.Regions.Find( r => r.GoLocation == loc && r.Map == map && r.Name == name )
                   ?? Region.Regions.Find( r => r.GoLocation == loc && r.Map == map )
                   ?? Region.Regions.Find( r => r.Map == map && r.Name == name && r.Contains( loc ) );
        }
    }
}
