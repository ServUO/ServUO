using System;
using System.Linq.Expressions;
using System.Reflection;


namespace Server
{
    public static class ReflectionUtility
    {
        private static MemberExpression GetMemberExpression( Expression expression )
        {
            if( expression is UnaryExpression )
                return (MemberExpression)( (UnaryExpression)expression ).Operand;
            return (MemberExpression)expression;
        }

        public static string GetPropertyName<T>( Expression<Func<T, object>> expression )
        {
            MemberExpression body = GetMemberExpression( expression.Body );
            return body.Member.Name;
        }

        public static string GetPropertyName<T1, T2>( this Expression<Func<T1, T2>> expression )
        {
            MemberExpression body = GetMemberExpression( expression.Body );
            return body.Member.Name;
        }

        public static object GetFieldValue( object obj, Type type, string fieldName )
        {
            if( obj == null )
                throw new ArgumentNullException( "obj" );
            if( type == null )
                throw new ArgumentNullException( "type" );
            if( string.IsNullOrEmpty( fieldName ) )
                throw new ArgumentException( "fieldName is null or empty.", "fieldName" );

            FieldInfo field = type.GetField( fieldName, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
            if( field != null )
                return field.GetValue( obj );
            throw new ArgumentException( string.Format( "Instance field '{0}' could not be found in class of type '{1}'", fieldName, type ) );
        }

        public static void SetFieldValue( object obj, Type type, string fieldName, object value )
        {
            if( obj == null )
                throw new ArgumentNullException( "obj" );
            if( type == null )
                throw new ArgumentNullException( "type" );
            if( string.IsNullOrEmpty( fieldName ) )
                throw new ArgumentException( "fieldName is null or empty.", "fieldName" );

            FieldInfo field = type.GetField( fieldName, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
            if( field == null )
                throw new ArgumentException( string.Format( "Instance field '{0}' could not be found in class of type '{1}'", fieldName, type ) );
            field.SetValue( obj, value );
        }

        public static object GetStaticFieldValue( Type type, string fieldName )
        {
            if( type == null )
                throw new ArgumentNullException( "type" );
            if( string.IsNullOrEmpty( fieldName ) )
                throw new ArgumentException( "fieldName is null or empty.", "fieldName" );

            FieldInfo field = type.GetField( fieldName, BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );
            if( field != null )
                return field.GetValue( null );
            throw new ArgumentException( string.Format( "Static field '{0}' could not be found in class of type '{1}'", fieldName, type ) );
        }

        public static void SetStaticFieldValue( Type type, string fieldName, object value )
        {
            if( type == null )
                throw new ArgumentNullException( "type" );
            if( string.IsNullOrEmpty( fieldName ) )
                throw new ArgumentException( "fieldName is null or empty.", "fieldName" );

            FieldInfo field = type.GetField( fieldName, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
            if( field == null )
                throw new ArgumentException( string.Format( "Static field '{0}' could not be found in class of type '{1}'", fieldName, type ) );
            field.SetValue( null, value );
        }

        public static ObjectActivator<T> GetActivator<T>( params Type[] args )
        {
            return GetSubclassActivator<T>( typeof( T ), args );
        }

        public static ObjectActivator<T> GetSubclassActivator<T>( Type type, params Type[] args )
        {
            if( type == null )
                throw new ArgumentNullException( "type" );
            if( !typeof( T ).IsAssignableFrom( type ) )
                throw new ArgumentException( string.Format( "{0} is not assignable from {1}", typeof( T ), type ) );

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            ConstructorInfo ctor = type.GetConstructor( flags, null, args, null );
            if( ctor == null )
            {
                string[] types = args.PropertyToArray(t=>t.Name);
                throw new ArgumentException( string.Format( "{0} does not define a ctor({1})", type, string.Join( ", ", types ) ) );
            }
            return GetActivator<T>( ctor );
        }

        public static ObjectActivator<T> GetActivator<T>( ConstructorInfo ctor )
        {
            if( ctor == null )
                throw new ArgumentNullException( "ctor" );

            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter( typeof( object[] ), "args" );

            Expression[] argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for( int i = 0; i < paramsInfo.Length; i++ )
            {
                Expression index = Expression.Constant( i );
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex( param, index );
                Expression paramCastExp = Expression.Convert( paramAccessorExp, paramType );
                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New( ctor, argsExp );

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda( typeof( ObjectActivator<T> ), newExp, param );

            //compile it
            ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }

        public static bool Implements<T>( this Type type )
        {
            Type t;
            return Implements( type, typeof( T ), out t );
        }

        public static bool Implements( this Type type, Type baseType )
        {
            Type t;
            return Implements( type, baseType, out t );
        }

        public static bool Implements<T>( this Type type, out Type foundType )
        {
            return Implements( type, typeof( T ), out foundType );
        }

        public static bool Implements( this Type type, Type baseType, out Type foundType )
        {
            if( type == null )
                throw new ArgumentNullException( "type" );

            if( baseType == null )
                throw new ArgumentNullException( "baseType" );

            if( CheckGenericEquals( type, baseType ) )
            {
                foundType = type;
                return true;
            }
            if( baseType.IsInterface )
            {
                foreach( Type interfaceType in type.GetInterfaces() )
                {
                    if( !CheckGenericEquals( interfaceType, baseType ) )
                        continue;

                    foundType = interfaceType;
                    return true;
                }
            }
            else
            {
                while( type.BaseType != null )
                {
                    type = type.BaseType;

                    if( !CheckGenericEquals( type, baseType ) )
                        continue;

                    foundType = type;
                    return true;
                }
            }

            foundType = null;
            return false;
        }

        private static bool CheckGenericEquals( Type type, Type baseType )
        {
            if( baseType.IsGenericTypeDefinition && type.IsGenericType )
                type = type.GetGenericTypeDefinition();

            return type == baseType;
        }
    }
}