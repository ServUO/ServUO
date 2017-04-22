using System;
using System.Collections.Generic;

namespace Server
{
    public static class LogManager
    {
        private static Dictionary<Type, ILog> m_Loggers = new Dictionary<Type, ILog>( );

        public static ILog GetLogger<T>( )
        {
            return GetLogger( typeof(T) );
        }

        public static ILog GetLogger( Type type )
        {
            ILog logger;
            
            if( !m_Loggers.TryGetValue( type, out logger ) || logger == null )
                m_Loggers[type] = logger = CreateLogger( type.Name );
            
            return logger;
        }
        
        private static ILog CreateLogger( string name )
        {
            return new ConsoleLogger( name );
        }        
    }
}
