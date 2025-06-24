using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;


namespace Server
{
    public static class SWSerialize
    {
        public static void WriteVersionSW<T>( this GenericWriter writer, T me, int version )
                where T : ISerializable
        {
            for( int i = 0; i < _dictionary[typeof( T )]; i++ )
                writer.Write( 0 );
            writer.Write( version );
        }

        internal static void WriteEndOfVersionSW( GenericWriter writer, ISerializable serializable )
        {
            for( int i = 0; i < _dictionary[serializable.GetType()]; i++ )
                writer.Write( 0 );
        }

        public static int ReadVersionSW<T>( this GenericReader reader, T me )
                where T : ISerializable
        {
            for( int i = 0; i < _dictionary[typeof( T )]; i++ )
                reader.ReadInt();
            return reader.ReadInt();
        }

        internal static void ReadEndOfVersionSW( GenericReader reader, ISerializable serializable )
        {
            for( int i = 0; i < _dictionary[serializable.GetType()]; i++ )
                reader.ReadInt();
        }

        private static readonly Dictionary<Type, int> _dictionary = new Dictionary<Type, int>();
        internal static void Initialize( Type type )
        {
            if( !type.IsClass || !typeof( ISerializable ).IsAssignableFrom( type ) )
                return;

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            int covariance = 0;
            Type currentType = type;
            while( currentType != null && typeof( ISerializable ).IsAssignableFrom( currentType ) )
            {
                bool hasSerializeMethod = currentType.GetMethod( "Serialize", bindingFlags ) != null;
                bool hasSerializeSWMethod = currentType.GetMethod( "SerializeSW", bindingFlags ) != null;
                bool hasDeserializeMethod = currentType.GetMethod( "Deserialize", bindingFlags ) != null;
                bool hasDeserializeSWMethod = currentType.GetMethod( "DeserializeSW", bindingFlags ) != null;

                if( hasSerializeMethod && hasSerializeSWMethod && hasDeserializeMethod && hasDeserializeSWMethod )
                    break;

                if( hasSerializeMethod && hasDeserializeMethod )
                    covariance++;

                if( !hasSerializeMethod && hasSerializeSWMethod )
                    Console.WriteLine( "Warning: {0}\n       - Has no Serialize(...) but a SerializeSW(...)", type );
                if( !hasDeserializeMethod && hasDeserializeSWMethod )
                    Console.WriteLine( "Warning: {0}\n       - Has no Deserialize(...) but a DeserializeSW(...)", type );

                currentType = currentType.BaseType;
            }

            lock( _dictionary )
            {
                _dictionary[type] = covariance; // register
            }
        }

#if Test
        public static void Test( Assembly assembly, int times )
        {
            IEnumerable<Type> types = assembly.GetTypes().Where( t => typeof( ISerializable ).IsAssignableFrom( t ) );
            int count = types.Count();
            Stopwatch watch = Stopwatch.StartNew();
            foreach( Type type in types )
            {
                for( int i = 0; i < times; i++ )
                {
#pragma warning disable 168
                    int covariance = _dictionary[type];
#pragma warning restore 168
                }
            }
            Console.WriteLine( "SWSerialize {0} types {1} times with Dictionary: \t{2}", count, times, watch.Elapsed );
        }
#endif
    }
}