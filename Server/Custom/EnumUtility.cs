using System;


namespace Server
{
    public static class EnumUtils
    {
        public static T Parse<T>( string value )
                where T : struct, IConvertible
        {
            return (T)Enum.Parse( typeof( T ), value, true );
        }

        public static bool TryParse<T>( string value, out T enumValue )
                where T : struct, IConvertible
        {
            try
            {
                enumValue = Parse<T>( value );
            }
            catch( Exception )
            {
                enumValue = default( T );
                return false;
            }

            return true;
        }
    }
}