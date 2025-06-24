using System;


namespace Server.XmlSerialize
{
    public class ValueSetter
    {
        public readonly string NewFieldName;
        public readonly Type ValueType;
        public readonly Action<object, object> Setter;

        public ValueSetter( string newFieldName, Type valueType, Action<object, object> setter )
        {
            NewFieldName = newFieldName;
            Setter = setter;
            ValueType = valueType;
        }
    }
}