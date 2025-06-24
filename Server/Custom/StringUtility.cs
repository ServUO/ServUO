using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;


namespace Server
{
    public static class StringExtensionMethods
    {
        /// <summary>
        ///   Indicates whether the specified regular expression finds a exact match in the specified input string.
        /// </summary>
        /// <returns>true if the regular expression finds a match; otherwise, false.</returns>
        /// <param name = "input">The string to search for a match. </param>
        /// <param name = "pattern">The regular expression pattern to match. </param>
        /// <exception cref = "T:System.ArgumentException">A regular expression parsing error occurred.</exception>
        /// <exception cref = "T:System.ArgumentNullException">
        ///   <paramref name = "input" /> or <paramref name = "pattern" /> is null. </exception>
        public static bool IsExactMatch( [NotNull] string input, [NotNull] string pattern )
        {
            if( input == null )
                throw new ArgumentNullException( "input" );
            if( pattern == null )
                throw new ArgumentNullException( "pattern" );

            return new Regex( "^" + pattern + "$" ).IsMatch( input );
        }

        public static int CountChar( this string str, char c )
        {
            int count = 0;
            foreach( char t in str )
            {
                if( t == c )
                    ++count;
            }
            return count;
        }

        /// <summary>
        ///   check whether the string contains only digits
        /// </summary>
        /// <param name = "s">ths string to check</param>
        /// <returns>true if the stringcontains only digits, otherwise false</returns>
        public static bool IsDigitString( this string s )
        {
            return !string.IsNullOrEmpty( s ) && s.All( char.IsDigit );
        }

        /// <summary>
        ///   Separates a string to blocks
        ///   E.g: "ppppbbbbbkkkkkkkkkkkk" to "pppp bbbb bkkk kkkk kkkk k"
        /// </summary>
        /// <param name = "s">The string to separate</param>
        /// <param name = "blockSize">size of each block</param>
        /// <param name = "separator">Separator value</param>
        /// <returns>A separated string</returns>
        [CanBeNull]
        public static string Separate( [CanBeNull] this string s, int blockSize, string separator )
        {
            if( s == null || s.Length < blockSize )
                return s;

            string[] blocks = new string[( s.Length + blockSize - 1 ) / blockSize];
            for( int i = 0; i < blocks.Length; i++ )
                blocks[i] = s.Substring( i * blockSize, i * blockSize + blockSize <= s.Length ? blockSize : s.Length % blockSize );
            return string.Join( separator, blocks );
        }
    }
}