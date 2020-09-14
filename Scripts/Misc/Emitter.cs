using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Server
{
    public class AssemblyEmitter
    {
        private readonly string m_AssemblyName;

        private readonly AppDomain m_AppDomain;
        private readonly AssemblyBuilder m_AssemblyBuilder;
        private readonly ModuleBuilder m_ModuleBuilder;

        public AssemblyEmitter(string assemblyName, bool canSave)
        {
            m_AssemblyName = assemblyName;

            m_AppDomain = AppDomain.CurrentDomain;

            m_AssemblyBuilder = m_AppDomain.DefineDynamicAssembly(
                new AssemblyName(assemblyName),
                canSave ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);

            if (canSave)
            {
                m_ModuleBuilder = m_AssemblyBuilder.DefineDynamicModule(
                    assemblyName,
                    string.Format("{0}.dll", assemblyName.ToLower()),
                    false);
            }
            else
            {
                m_ModuleBuilder = m_AssemblyBuilder.DefineDynamicModule(
                    assemblyName,
                    false);
            }
        }

        public TypeBuilder DefineType(string typeName, TypeAttributes attrs, Type parentType)
        {
            return m_ModuleBuilder.DefineType(typeName, attrs, parentType);
        }

        public void Save()
        {
            m_AssemblyBuilder.Save(
                string.Format("{0}.dll", m_AssemblyName.ToLower()));
        }
    }

    public class MethodEmitter
    {
        private readonly TypeBuilder m_TypeBuilder;

        private MethodBuilder m_Builder;
        private ILGenerator m_Generator;

        private Type[] m_ArgumentTypes;

        public TypeBuilder Type => m_TypeBuilder;

        public ILGenerator Generator => m_Generator;

        private class CallInfo
        {
            public readonly Type type;
            public readonly MethodInfo method;

            public int index;
            public readonly ParameterInfo[] parms;

            public CallInfo(Type type, MethodInfo method)
            {
                this.type = type;
                this.method = method;

                parms = method.GetParameters();
            }
        }

        private readonly Stack<Type> m_Stack;
        private readonly Stack<CallInfo> m_Calls;

        private readonly Dictionary<Type, Queue<LocalBuilder>> m_Temps;

        public MethodBuilder Method => m_Builder;

        public MethodEmitter(TypeBuilder typeBuilder)
        {
            m_TypeBuilder = typeBuilder;

            m_Temps = new Dictionary<Type, Queue<LocalBuilder>>();

            m_Stack = new Stack<Type>();
            m_Calls = new Stack<CallInfo>();
        }

        public void Define(string name, MethodAttributes attr, Type returnType, Type[] parms)
        {
            m_Builder = m_TypeBuilder.DefineMethod(name, attr, returnType, parms);
            m_Generator = m_Builder.GetILGenerator();

            m_ArgumentTypes = parms;
        }

        public LocalBuilder CreateLocal(Type localType)
        {
            return m_Generator.DeclareLocal(localType);
        }

        public LocalBuilder AcquireTemp(Type localType)
        {
            Queue<LocalBuilder> list;

            if (!m_Temps.TryGetValue(localType, out list))
                m_Temps[localType] = list = new Queue<LocalBuilder>();

            if (list.Count > 0)
                return list.Dequeue();

            return CreateLocal(localType);
        }

        public void ReleaseTemp(LocalBuilder local)
        {
            Queue<LocalBuilder> list;

            if (!m_Temps.TryGetValue(local.LocalType, out list))
                m_Temps[local.LocalType] = list = new Queue<LocalBuilder>();

            list.Enqueue(local);
        }

        public void Branch(Label label)
        {
            m_Generator.Emit(OpCodes.Br, label);
        }

        public void BranchIfFalse(Label label)
        {
            Pop(typeof(object));

            m_Generator.Emit(OpCodes.Brfalse, label);
        }

        public void BranchIfTrue(Label label)
        {
            Pop(typeof(object));

            m_Generator.Emit(OpCodes.Brtrue, label);
        }

        public Label CreateLabel()
        {
            return m_Generator.DefineLabel();
        }

        public void MarkLabel(Label label)
        {
            m_Generator.MarkLabel(label);
        }

        public void Pop()
        {
            m_Stack.Pop();
        }

        public void Pop(Type expected)
        {
            if (expected == null)
                throw new InvalidOperationException("Expected type cannot be null.");

            Type onStack = m_Stack.Pop();

            if (expected == typeof(bool))
                expected = typeof(int);

            if (onStack == typeof(bool))
                onStack = typeof(int);

            if (!expected.IsAssignableFrom(onStack))
                throw new InvalidOperationException("Unexpected stack state.");
        }

        public void Push(Type type)
        {
            m_Stack.Push(type);
        }

        public void Return()
        {
            if (m_Stack.Count != (m_Builder.ReturnType == typeof(void) ? 0 : 1))
                throw new InvalidOperationException("Stack return mismatch.");

            m_Generator.Emit(OpCodes.Ret);
        }

        public void LoadNull()
        {
            LoadNull(typeof(object));
        }

        public void LoadNull(Type type)
        {
            Push(type);

            m_Generator.Emit(OpCodes.Ldnull);
        }

        public void Load(string value)
        {
            Push(typeof(string));

            if (value != null)
                m_Generator.Emit(OpCodes.Ldstr, value);
            else
                m_Generator.Emit(OpCodes.Ldnull);
        }

        public void Load(Enum value)
        {
            int toLoad = ((IConvertible)value).ToInt32(null);
            Load(toLoad);

            Pop();
            Push(value.GetType());
        }

        public void Load(long value)
        {
            Push(typeof(long));

            m_Generator.Emit(OpCodes.Ldc_I8, value);
        }

        public void Load(float value)
        {
            Push(typeof(float));

            m_Generator.Emit(OpCodes.Ldc_R4, value);
        }

        public void Load(double value)
        {
            Push(typeof(double));

            m_Generator.Emit(OpCodes.Ldc_R8, value);
        }

        public void Load(char value)
        {
            Load((int)value);

            Pop();
            Push(typeof(char));
        }

        public void Load(bool value)
        {
            Push(typeof(bool));

            if (value)
                m_Generator.Emit(OpCodes.Ldc_I4_1);
            else
                m_Generator.Emit(OpCodes.Ldc_I4_0);
        }

        public void Load(int value)
        {
            Push(typeof(int));

            switch (value)
            {
                case -1:
                    m_Generator.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    m_Generator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    m_Generator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    m_Generator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    m_Generator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    m_Generator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    m_Generator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    m_Generator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    m_Generator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    m_Generator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                        m_Generator.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    else
                        m_Generator.Emit(OpCodes.Ldc_I4, value);

                    break;
            }
        }

        public void LoadField(FieldInfo field)
        {
            Pop(field.DeclaringType);

            Push(field.FieldType);

            m_Generator.Emit(OpCodes.Ldfld, field);
        }

        public void LoadLocal(LocalBuilder local)
        {
            Push(local.LocalType);

            int index = local.LocalIndex;

            switch (index)
            {
                case 0:
                    m_Generator.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    m_Generator.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    m_Generator.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    m_Generator.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (index >= byte.MinValue && index <= byte.MinValue)
                        m_Generator.Emit(OpCodes.Ldloc_S, (byte)index);
                    else
                        m_Generator.Emit(OpCodes.Ldloc, (short)index);

                    break;
            }
        }

        public void StoreLocal(LocalBuilder local)
        {
            Pop(local.LocalType);

            m_Generator.Emit(OpCodes.Stloc, local);
        }

        public void LoadArgument(int index)
        {
            if (index > 0)
                Push(m_ArgumentTypes[index - 1]);
            else
                Push(m_TypeBuilder);

            switch (index)
            {
                case 0:
                    m_Generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    m_Generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    m_Generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    m_Generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index >= byte.MinValue && index <= byte.MaxValue)
                        m_Generator.Emit(OpCodes.Ldarg_S, (byte)index);
                    else
                        m_Generator.Emit(OpCodes.Ldarg, (short)index);

                    break;
            }
        }

        public void CastAs(Type type)
        {
            Pop(typeof(object));
            Push(type);

            m_Generator.Emit(OpCodes.Isinst, type);
        }

        public void Neg()
        {
            Pop(typeof(int));

            Push(typeof(int));

            m_Generator.Emit(OpCodes.Neg);
        }

        public void Compare(OpCode opCode)
        {
            Pop();
            Pop();

            Push(typeof(int));

            m_Generator.Emit(opCode);
        }

        public void LogicalNot()
        {
            Pop(typeof(int));

            Push(typeof(int));

            m_Generator.Emit(OpCodes.Ldc_I4_0);
            m_Generator.Emit(OpCodes.Ceq);
        }

        public void Xor()
        {
            Pop(typeof(int));
            Pop(typeof(int));

            Push(typeof(int));

            m_Generator.Emit(OpCodes.Xor);
        }

        public Type Active => m_Stack.Peek();

        public void Chain(Property prop)
        {
            for (int i = 0; i < prop.Chain.Length; ++i)
                Call(prop.Chain[i].GetGetMethod());
        }

        public void Call(MethodInfo method)
        {
            BeginCall(method);

            CallInfo call = m_Calls.Peek();

            if (call.parms.Length > 0)
                throw new InvalidOperationException("Method requires parameters.");

            FinishCall();
        }

        public delegate void Callback();

        public bool CompareTo(int sign, Callback argGenerator)
        {
            Type active = Active;

            MethodInfo compareTo = active.GetMethod("CompareTo", new[] { active });

            if (compareTo == null)
            {
                /* This gets a little tricky...
                * 
                * There's a scenario where we might be trying to use CompareTo on an interface
                * which, while it doesn't explicitly implement CompareTo itself, is said to
                * extend IComparable indirectly.  The implementation is implicitly passed off
                * to implementers...
                * 
                * interface ISomeInterface : IComparable
                * {
                *    void SomeMethod();
                * }
                * 
                * class SomeClass : ISomeInterface
                * {
                *    void SomeMethod() { ... }
                *    int CompareTo( object other ) { ... }
                * }
                * 
                * In this case, calling ISomeInterface.GetMethod( "CompareTo" ) will return null.
                * 
                * Bleh.
                */
                Type[] ifaces = active.FindInterfaces(delegate (Type type, object obj)
                {
                    return (type.IsGenericType) &&
                           (type.GetGenericTypeDefinition() == typeof(IComparable<>)) &&
                           (type.GetGenericArguments()[0].IsAssignableFrom(active));
                }, null);

                if (ifaces.Length > 0)
                {
                    compareTo = ifaces[0].GetMethod("CompareTo", new[] { active });
                }
                else
                {
                    ifaces = active.FindInterfaces(delegate (Type type, object obj)
                    {
                        return (type == typeof(IComparable));
                    }, null);

                    if (ifaces.Length > 0)
                        compareTo = ifaces[0].GetMethod("CompareTo", new[] { active });
                }
            }

            if (compareTo == null)
                return false;

            if (!active.IsValueType)
            {
                /* This object is a reference type, so we have to make it behave
                * 
                * null.CompareTo( null ) =  0
                * real.CompareTo( null ) = -1
                * null.CompareTo( real ) = +1
                * 
                */
                LocalBuilder aValue = AcquireTemp(active);
                LocalBuilder bValue = AcquireTemp(active);

                StoreLocal(aValue);

                argGenerator();

                StoreLocal(bValue);

                /* if ( aValue == null )
                * {
                *    if ( bValue == null )
                *       v = 0;
                *    else
                *       v = +1;
                * }
                * else if ( bValue == null )
                * {
                *    v = -1;
                * }
                * else
                * {
                *    v = aValue.CompareTo( bValue );
                * }
                */

                Label store = CreateLabel();

                Label aNotNull = CreateLabel();

                LoadLocal(aValue);
                BranchIfTrue(aNotNull);
                // if ( aValue == null )
                {
                    Label bNotNull = CreateLabel();

                    LoadLocal(bValue);
                    BranchIfTrue(bNotNull);
                    // if ( bValue == null )
                    {
                        Load(0);
                        Pop(typeof(int));
                        Branch(store);
                    }
                    MarkLabel(bNotNull);
                    // else
                    {
                        Load(sign);
                        Pop(typeof(int));
                        Branch(store);
                    }
                }
                MarkLabel(aNotNull);
                // else
                {
                    Label bNotNull = CreateLabel();

                    LoadLocal(bValue);
                    BranchIfTrue(bNotNull);
                    // bValue == null
                    {
                        Load(-sign);
                        Pop(typeof(int));
                        Branch(store);
                    }
                    MarkLabel(bNotNull);
                    // else
                    {
                        LoadLocal(aValue);
                        BeginCall(compareTo);

                        LoadLocal(bValue);
                        ArgumentPushed();

                        FinishCall();

                        if (sign == -1)
                            Neg();
                    }
                }

                MarkLabel(store);

                ReleaseTemp(aValue);
                ReleaseTemp(bValue);
            }
            else
            {
                BeginCall(compareTo);

                argGenerator();

                ArgumentPushed();

                FinishCall();

                if (sign == -1)
                    Neg();
            }

            return true;
        }

        public void BeginCall(MethodInfo method)
        {
            Type type;

            if ((method.CallingConvention & CallingConventions.HasThis) != 0)
                type = m_Stack.Peek();
            else
                type = method.DeclaringType;

            m_Calls.Push(new CallInfo(type, method));

            if (type.IsValueType)
            {
                LocalBuilder temp = AcquireTemp(type);

                m_Generator.Emit(OpCodes.Stloc, temp);
                m_Generator.Emit(OpCodes.Ldloca, temp);

                ReleaseTemp(temp);
            }
        }

        public void FinishCall()
        {
            CallInfo call = m_Calls.Pop();

            if ((call.type.IsValueType || call.type.IsByRef) && call.method.DeclaringType != call.type)
                m_Generator.Emit(OpCodes.Constrained, call.type);

            if (call.method.DeclaringType.IsValueType || call.method.IsStatic)
                m_Generator.Emit(OpCodes.Call, call.method);
            else
                m_Generator.Emit(OpCodes.Callvirt, call.method);

            for (int i = call.parms.Length - 1; i >= 0; --i)
                Pop(call.parms[i].ParameterType);

            if ((call.method.CallingConvention & CallingConventions.HasThis) != 0)
                Pop(call.method.DeclaringType);

            if (call.method.ReturnType != typeof(void))
                Push(call.method.ReturnType);
        }

        public void ArgumentPushed()
        {
            CallInfo call = m_Calls.Peek();

            ParameterInfo parm = call.parms[call.index++];

            Type argumentType = m_Stack.Peek();

            if (!parm.ParameterType.IsAssignableFrom(argumentType))
                throw new InvalidOperationException("Parameter type mismatch.");

            if (argumentType.IsValueType && !parm.ParameterType.IsValueType)
                m_Generator.Emit(OpCodes.Box, argumentType);
        }
    }
}
