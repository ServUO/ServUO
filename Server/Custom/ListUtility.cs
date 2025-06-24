using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Server
{
    public static class ListUtility
    {
        public const char CsvDelimiter = ';';
        public const char CsvQuote = '~';

        public static T[] AddItem<T>( T[] array, T item )
        {
            int length = array == null ? 0 : array.Length;

            T[] result = new T[length + 1];

            if( array != null )
                array.CopyTo( result, 0 );

            result[length] = item;

            return result;
        }

        public static bool ArrayEquals<T>( this T[] a, T[] b ) where T : IEquatable<T>
        {
            if( a.Length != b.Length )
                return false;

            for( int i = 0; i < a.Length; i++ )
            {
                if( Equals( a[i], default( T ) ) && Equals( b[i], default( T ) ) )
                    return false;

                if( !a[i].Equals( b[i] ) )
                    return false;
            }
            return true;
        }

        public static void SortDescending<T>( T[] array ) where T : IComparable
        {
            Array.Sort( array, DescendingCompare );
        }

        private static int DescendingCompare<T>( T object1, T object2 ) where T : IComparable
        {
            return -object1.CompareTo( object2 );
        }

        /// <summary>
        ///   eliminates duplicate items in <paramref name = "list" />. 
        ///   It does change <paramref name = "list" /> and removes duplicated items which have the lower index
        /// </summary>
        /// <typeparam name = "T">the type of the items of the list</typeparam>
        /// <param name = "list">the list from which duplicate items should be removed</param>
        /// <returns>the <paramref name = "list" /> w/o duplicates</returns>
        public static IList<T> EliminateDuplicates<T>( IList<T> list )
        {
            for( int i = list.Count - 1; i > 0; --i )
            {
                for( int j = 0; j < i; ++j )
                {
                    if( !list[i].Equals( list[j] ) )
                        continue;

                    list.RemoveAt( j-- );
                    --i;
                }
            }
            return list;
        }

        public static T[] ToArray<T>( this ICollection<T> collection )
        {
            T[] array = new T[collection.Count];
            collection.CopyTo( array, 0 );
            return array;
        }

        public static TResult[] PropertyToArray<TResult, TOwner>( this IList<TOwner> list, Func<TOwner, TResult> accessor, int index, int count )
        {
            TResult[] result = new TResult[count];
            for( int i = 0; i < count; i++ )
                result[i] = accessor( list[index + i] );
            return result;
        }

        public static TResult[] PropertyToArray<TResult, TOwner>( this ICollection<TOwner> collection, Func<TOwner, TResult> accessor )
        {
            int i = 0;
            TResult[] result = new TResult[collection.Count];
            foreach( TOwner item in collection )
                result[i++] = accessor( item );
            return result;
        }

        public static TResult[] PropertyToArray<TResult, TOwner>( this IEnumerable<TOwner> enumerable, Func<TOwner, TResult> accessor )
        {
            List<TOwner> result = new List<TOwner>( enumerable );
            return result.PropertyToArray( accessor );
        }

        public static void TreeDepthFirst<T>( this IEnumerable<T> units, Func<T, IEnumerable<T>> getList, Action<T> method )
        {
            foreach( T unit in units )
            {
                TreeDepthFirst( getList( unit ), getList, method );

                method( unit );
            }
        }

        public static void TreeDepthLast<T>( this IEnumerable<T> units, Func<T, IEnumerable<T>> getList, Action<T> method )
        {
            foreach( T unit in units )
            {
                method( unit );

                TreeDepthLast( getList( unit ), getList, method );
            }
        }

        public static void Remove<T>( this IList<T> collection, Predicate<T> predicate )
        {
            for( int i = collection.Count - 1; i >= 0; --i )
            {
                if( predicate( collection[i] ) )
                    collection.RemoveAt( i );
            }
        }

        public static int IndexOf<T>( this IList<T> list, Predicate<T> predicate )
        {
            for( int i = 0; i < list.Count; i++ )
            {
                if( predicate( list[i] ) )
                    return i;
            }
            return -1;
        }

        public static string ReadCsvLine( TextReader reader, char quote = CsvQuote )
        {
            string line = reader.ReadLine();
            if( line == null )
                return null;

            int quoteCount = line.CountChar( quote );
            while( quoteCount % 2 == 1 && reader.Peek() != -1 ) // while odd and not EOF
            {
                string append = reader.ReadLine();
                line += append;
                quoteCount += append.CountChar( quote );
            }
            return line;
        }

        /// <summary>
        ///   To create a RFC 4180 compatible single line of CSV
        /// </summary>
        /// <param name = "values">The list of data in this line</param>
        /// <param name = "delimiter">The delimiter of an element</param>
        /// <param name = "quote">The quoting character to encapsulate fields</param>
        /// <returns>CSV string</returns>
        public static string ToCsv( IEnumerable values, char delimiter = CsvDelimiter, char quote = CsvQuote )
        {
            string internalDelimiter = delimiter.ToString();
            string internalQuote = quote.ToString();
            string doubleQuote = new string( quote, 2 );

            StringBuilder sb = new StringBuilder();
            IEnumerator enumerator = values.GetEnumerator();
            bool hasNext = enumerator.MoveNext(); // init
            while( hasNext )
            {
                if( enumerator.Current != null )
                {
                    string str = enumerator.Current.ToString();
                    if( str == string.Empty )
                        sb.Append( doubleQuote );
                    else if( str.Contains( internalDelimiter ) || str.Contains( internalQuote ) )
                    {
                        str = str.Replace( internalQuote, doubleQuote );
                        sb.Append( string.Concat( internalQuote, str, internalQuote ) );
                    }
                    else
                        sb.Append( str );
                }

                hasNext = enumerator.MoveNext();
                if( hasNext )
                    sb.Append( internalDelimiter );
            }
            return sb.ToString();
        }

        /// <summary>
        ///   To read a RFC 4180 compatible single line of CSV
        /// </summary>
        /// <typeparam name = "T">Type of a constructable list, which will be filled with srings</typeparam>
        /// <param name = "str">The CSV line</param>
        /// <param name = "delimiter">The delimiter of an element</param>
        /// <param name = "quote">The quoting character to encapsulate fields</param>
        /// <returns>A filled list</returns>
        public static T FromCsv<T>( string str, char delimiter = CsvDelimiter, char quote = CsvQuote ) where T : IList, new()
        {
            T stringCollection = new T();
            FromCsv( stringCollection, str, delimiter, quote );
            return stringCollection;
        }

        /// <summary>
        ///   To read a RFC 4180 compatible single line of CSV
        /// </summary>
        /// <param name = "stringCollection">The collection to add to</param>
        /// <param name = "str">The CSV line</param>
        /// <param name = "delimiter">The delimiter of an element</param>
        /// <param name = "quote">The quoting character to encapsulate fields</param>
        /// <returns>A filled list</returns>
        public static void FromCsv( IList stringCollection, string str, char delimiter = CsvDelimiter, char quote = CsvQuote )
        {
            string internalDelimiter = delimiter.ToString();
            string internalQuote = quote.ToString();
            string doubleQuote = new string( quote, 2 );
            string[] split = str.Split( new[] { delimiter } );
            bool concatNext = false;
            string s = null;
            foreach( string t in split )
            {
                if( concatNext )
                    s = string.Concat( s, internalDelimiter, t );
                else
                    s = t == string.Empty ? null : t;

                if( !string.IsNullOrEmpty( s ) && s[0] == quote )
                {
                    concatNext = true;
                    for( int i = s.Length - 1; i > 0 && s[i] == quote; i-- )
                        concatNext = !concatNext;

                    if( !concatNext )
                    {
                        s = s.Substring( 1, s.Length - 2 );
                        stringCollection.Add( s.Replace( doubleQuote, internalQuote ) );
                    }
                }
                else
                {
                    concatNext = false;
                    stringCollection.Add( s );
                }
            }

            if( concatNext ) // missing finish...
                stringCollection.Add( s );
        }
    }
}