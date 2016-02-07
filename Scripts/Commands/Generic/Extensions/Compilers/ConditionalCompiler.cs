using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace Server.Commands.Generic
{
    public interface IConditional
    {
        bool Verify(object obj);
    }

    public interface ICondition
    {
        // Invoked during the constructor
        void Construct(TypeBuilder typeBuilder, ILGenerator il, int index);

        // Target object will be loaded on the stack
        void Compile(MethodEmitter emitter);
    }

    public sealed class TypeCondition : ICondition
    {
        public static TypeCondition Default = new TypeCondition();

        void ICondition.Construct(TypeBuilder typeBuilder, ILGenerator il, int index)
        {
        }

        void ICondition.Compile(MethodEmitter emitter)
        {
            // The object was safely cast to be the conditionals type
            // If it's null, then the type cast didn't work...
            emitter.LoadNull();
            emitter.Compare(OpCodes.Ceq);
            emitter.LogicalNot();
        }
    }

    public sealed class PropertyValue
    {
        private readonly Type m_Type;
        private object m_Value;
        private FieldInfo m_Field;

        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }

        public object Value
        {
            get
            {
                return this.m_Value;
            }
        }

        public FieldInfo Field
        {
            get
            {
                return this.m_Field;
            }
        }

        public bool HasField
        {
            get
            {
                return (this.m_Field != null);
            }
        }

        public PropertyValue(Type type, object value)
        {
            this.m_Type = type;
            this.m_Value = value;
        }

        public void Load(MethodEmitter method)
        {
            if (this.m_Field != null)
            {
                method.LoadArgument(0);
                method.LoadField(this.m_Field);
            }
            else if (this.m_Value == null)
            {
                method.LoadNull(this.m_Type);
            }
            else
            {
                if (this.m_Value is int)
                    method.Load((int)this.m_Value);
                else if (this.m_Value is long)
                    method.Load((long)this.m_Value);
                else if (this.m_Value is float)
                    method.Load((float)this.m_Value);
                else if (this.m_Value is double)
                    method.Load((double)this.m_Value);
                else if (this.m_Value is char)
                    method.Load((char)this.m_Value);
                else if (this.m_Value is bool)
                    method.Load((bool)this.m_Value);
                else if (this.m_Value is string)
                    method.Load((string)this.m_Value);
                else if (this.m_Value is Enum)
                    method.Load((Enum)this.m_Value);
                else
                    throw new InvalidOperationException("Unrecognized comparison value.");
            }
        }

        public void Acquire(TypeBuilder typeBuilder, ILGenerator il, string fieldName)
        {
            if (this.m_Value is string)
            {
                string toParse = (string)this.m_Value;

                if (!this.m_Type.IsValueType && toParse == "null")
                {
                    this.m_Value = null;
                }
                else if (this.m_Type == typeof(string))
                {
                    if (toParse == @"@""null""")
                        toParse = "null";

                    this.m_Value = toParse;
                }
                else if (this.m_Type.IsEnum)
                {
                    this.m_Value = Enum.Parse(this.m_Type, toParse, true);
                }
                else
                {
                    MethodInfo parseMethod = null;
                    object[] parseArgs = null;

                    MethodInfo parseNumber = this.m_Type.GetMethod(
                        "Parse",
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new Type[] { typeof(string), typeof(NumberStyles) },
                        null);

                    if (parseNumber != null)
                    {
                        NumberStyles style = NumberStyles.Integer;

                        if (Insensitive.StartsWith(toParse, "0x"))
                        {
                            style = NumberStyles.HexNumber;
                            toParse = toParse.Substring(2);
                        }

                        parseMethod = parseNumber;
                        parseArgs = new object[] { toParse, style };
                    }
                    else
                    {
                        MethodInfo parseGeneral = this.m_Type.GetMethod(
                            "Parse",
                            BindingFlags.Public | BindingFlags.Static,
                            null,
                            new Type[] { typeof(string) },
                            null);

                        parseMethod = parseGeneral;
                        parseArgs = new object[] { toParse };
                    }

                    if (parseMethod != null)
                    {
                        this.m_Value = parseMethod.Invoke(null, parseArgs);

                        if (!this.m_Type.IsPrimitive)
                        {
                            this.m_Field = typeBuilder.DefineField(
                                fieldName,
                                this.m_Type,
                                FieldAttributes.Private | FieldAttributes.InitOnly);

                            il.Emit(OpCodes.Ldarg_0);

                            il.Emit(OpCodes.Ldstr, toParse);

                            if (parseArgs.Length == 2) // dirty evil hack :-(
                                il.Emit(OpCodes.Ldc_I4, (int)parseArgs[1]);

                            il.Emit(OpCodes.Call, parseMethod);
                            il.Emit(OpCodes.Stfld, this.m_Field);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                "Unable to convert string \"{0}\" into type '{1}'.",
                                this.m_Value,
                                this.m_Type));
                    }
                }
            }
        }
    }

    public abstract class PropertyCondition : ICondition
    {
        protected Property m_Property;
        protected bool m_Not;

        public PropertyCondition(Property property, bool not)
        {
            this.m_Property = property;
            this.m_Not = not;
        }

        public abstract void Construct(TypeBuilder typeBuilder, ILGenerator il, int index);

        public abstract void Compile(MethodEmitter emitter);
    }

    public enum StringOperator
    {
        Equal,
        NotEqual,

        Contains,

        StartsWith,
        EndsWith
    }

    public sealed class StringCondition : PropertyCondition
    {
        private readonly StringOperator m_Operator;
        private readonly PropertyValue m_Value;

        private readonly bool m_IgnoreCase;

        public StringCondition(Property property, bool not, StringOperator op, object value, bool ignoreCase)
            : base(property, not)
        {
            this.m_Operator = op;
            this.m_Value = new PropertyValue(property.Type, value);

            this.m_IgnoreCase = ignoreCase;
        }

        public override void Construct(TypeBuilder typeBuilder, ILGenerator il, int index)
        {
            this.m_Value.Acquire(typeBuilder, il, "v" + index);
        }

        public override void Compile(MethodEmitter emitter)
        {
            bool inverse = false;

            string methodName;

            switch ( this.m_Operator )
            {
                case StringOperator.Equal:
                    methodName = "Equals";
                    break;
                case StringOperator.NotEqual:
                    methodName = "Equals";
                    inverse = true;
                    break;
                case StringOperator.Contains:
                    methodName = "Contains";
                    break;
                case StringOperator.StartsWith:
                    methodName = "StartsWith";
                    break;
                case StringOperator.EndsWith:
                    methodName = "EndsWith";
                    break;
                default:
                    throw new InvalidOperationException("Invalid string comparison operator.");
            }

            if (this.m_IgnoreCase || methodName == "Equals")
            {
                Type type = (this.m_IgnoreCase ? typeof(Insensitive) : typeof(String));

                emitter.BeginCall(
                    type.GetMethod(
                        methodName,
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new Type[]
                        {
                            typeof(string),
                            typeof(string)
                        },
                        null));

                emitter.Chain(this.m_Property);
                this.m_Value.Load(emitter);

                emitter.FinishCall();
            }
            else
            {
                Label notNull = emitter.CreateLabel();
                Label moveOn = emitter.CreateLabel();

                LocalBuilder temp = emitter.AcquireTemp(this.m_Property.Type);

                emitter.Chain(this.m_Property);

                emitter.StoreLocal(temp);
                emitter.LoadLocal(temp);

                emitter.BranchIfTrue(notNull);

                emitter.Load(false);
                emitter.Pop();
                emitter.Branch(moveOn);

                emitter.MarkLabel(notNull);
                emitter.LoadLocal(temp);

                emitter.BeginCall(
                    typeof(string).GetMethod(
                        methodName,
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[]
                        {
                            typeof(string)
                        },
                        null));

                this.m_Value.Load(emitter);

                emitter.FinishCall();

                emitter.MarkLabel(moveOn);
            }

            if (this.m_Not != inverse)
                emitter.LogicalNot();
        }
    }

    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        Greater,
        GreaterEqual,
        Lesser,
        LesserEqual
    }

    public sealed class ComparisonCondition : PropertyCondition
    {
        private readonly ComparisonOperator m_Operator;
        private readonly PropertyValue m_Value;

        public ComparisonCondition(Property property, bool not, ComparisonOperator op, object value)
            : base(property, not)
        {
            this.m_Operator = op;
            this.m_Value = new PropertyValue(property.Type, value);
        }

        public override void Construct(TypeBuilder typeBuilder, ILGenerator il, int index)
        {
            this.m_Value.Acquire(typeBuilder, il, "v" + index);
        }

        public override void Compile(MethodEmitter emitter)
        {
            emitter.Chain(this.m_Property);

            bool inverse = false;

            bool couldCompare =
                emitter.CompareTo(1, delegate()
                {
                    this.m_Value.Load(emitter);
                });

            if (couldCompare)
            {
                emitter.Load(0);

                switch ( this.m_Operator )
                {
                    case ComparisonOperator.Equal:
                        emitter.Compare(OpCodes.Ceq);
                        break;
                    case ComparisonOperator.NotEqual:
                        emitter.Compare(OpCodes.Ceq);
                        inverse = true;
                        break;
                    case ComparisonOperator.Greater:
                        emitter.Compare(OpCodes.Cgt);
                        break;
                    case ComparisonOperator.GreaterEqual:
                        emitter.Compare(OpCodes.Clt);
                        inverse = true;
                        break;
                    case ComparisonOperator.Lesser:
                        emitter.Compare(OpCodes.Clt);
                        break;
                    case ComparisonOperator.LesserEqual:
                        emitter.Compare(OpCodes.Cgt);
                        inverse = true;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid comparison operator.");
                }
            }
            else
            {
                // This type is -not- comparable
                // We can only support == and != operations
                this.m_Value.Load(emitter);

                switch ( this.m_Operator )
                {
                    case ComparisonOperator.Equal:
                        emitter.Compare(OpCodes.Ceq);
                        break;
                    case ComparisonOperator.NotEqual:
                        emitter.Compare(OpCodes.Ceq);
                        inverse = true;
                        break;
                    case ComparisonOperator.Greater:
                    case ComparisonOperator.GreaterEqual:
                    case ComparisonOperator.Lesser:
                    case ComparisonOperator.LesserEqual:
                        throw new InvalidOperationException("Property does not support relational comparisons.");

                    default:
                        throw new InvalidOperationException("Invalid operator.");
                }
            }

            if (this.m_Not != inverse)
                emitter.LogicalNot();
        }
    }

    public static class ConditionalCompiler
    {
        public static IConditional Compile(AssemblyEmitter assembly, Type objectType, ICondition[] conditions, int index)
        {
            TypeBuilder typeBuilder = assembly.DefineType(
                "__conditional" + index,
                TypeAttributes.Public,
                typeof(object));

            #region Constructor
            {
                ConstructorBuilder ctor = typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    Type.EmptyTypes);

                ILGenerator il = ctor.GetILGenerator();

                // : base()
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

                for (int i = 0; i < conditions.Length; ++i)
                    conditions[i].Construct(typeBuilder, il, i);

                // return;
                il.Emit(OpCodes.Ret);
            }
            #endregion

            #region IComparer
            typeBuilder.AddInterfaceImplementation(typeof(IConditional));

            MethodBuilder compareMethod;

            #region Compare
            {
                MethodEmitter emitter = new MethodEmitter(typeBuilder);

                emitter.Define(
                    /*  name  */ "Verify",
                    /*  attr  */ MethodAttributes.Public | MethodAttributes.Virtual,
                    /* return */ typeof(bool),
                    /* params */ new Type[] { typeof(object) });

                LocalBuilder obj = emitter.CreateLocal(objectType);
                LocalBuilder eq = emitter.CreateLocal(typeof(bool));

                emitter.LoadArgument(1);
                emitter.CastAs(objectType);
                emitter.StoreLocal(obj);

                Label done = emitter.CreateLabel();

                for (int i = 0; i < conditions.Length; ++i)
                {
                    if (i > 0)
                    {
                        emitter.LoadLocal(eq);

                        emitter.BranchIfFalse(done);
                    }

                    emitter.LoadLocal(obj);

                    conditions[i].Compile(emitter);

                    emitter.StoreLocal(eq);
                }

                emitter.MarkLabel(done);

                emitter.LoadLocal(eq);

                emitter.Return();

                typeBuilder.DefineMethodOverride(
                    emitter.Method,
                    typeof(IConditional).GetMethod(
                        "Verify",
                        new Type[]
                        {
                            typeof(object)
                        }));

                compareMethod = emitter.Method;
            }
            #endregion
            #endregion

            Type conditionalType = typeBuilder.CreateType();

            return (IConditional)Activator.CreateInstance(conditionalType);
        }
    }
}