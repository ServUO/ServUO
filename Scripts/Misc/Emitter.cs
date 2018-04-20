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
            this.m_AssemblyName = assemblyName;

            this.m_AppDomain = AppDomain.CurrentDomain;

            this.m_AssemblyBuilder = this.m_AppDomain.DefineDynamicAssembly(
                new AssemblyName(assemblyName),
                canSave ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);

            if (canSave)
            {
                this.m_ModuleBuilder = this.m_AssemblyBuilder.DefineDynamicModule(
                    assemblyName,
                    String.Format("{0}.dll", assemblyName.ToLower()),
                    false);
            }
            else
            {
                this.m_ModuleBuilder = this.m_AssemblyBuilder.DefineDynamicModule(
                    assemblyName,
                    false);
            }
        }

        public TypeBuilder DefineType(string typeName, TypeAttributes attrs, Type parentType)
        {
            return this.m_ModuleBuilder.DefineType(typeName, attrs, parentType);
        }

        public void Save()
        {
            this.m_AssemblyBuilder.Save(
                String.Format("{0}.dll", this.m_AssemblyName.ToLower()));
        }
    }

    public class MethodEmitter
    {
        private readonly TypeBuilder m_TypeBuilder;

        private MethodBuilder m_Builder;
        private ILGenerator m_Generator;

        private Type[] m_ArgumentTypes;

        public TypeBuilder Type
        {
            get
            {
                return this.m_TypeBuilder;
            }
        }

        public ILGenerator Generator
        {
            get
            {
                return this.m_Generator;
            }
        }

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

                this.parms = method.GetParameters();
            }
        }

        private readonly Stack<Type> m_Stack;
        private readonly Stack<CallInfo> m_Calls;

        private readonly Dictionary<Type, Queue<LocalBuilder>> m_Temps;

        public MethodBuilder Method
        {
            get
            {
                return this.m_Builder;
            }
        }

        public MethodEmitter(TypeBuilder typeBuilder)
        {
            this.m_TypeBuilder = typeBuilder;

            this.m_Temps = new Dictionary<Type, Queue<LocalBuilder>>();

            this.m_Stack = new Stack<Type>();
            this.m_Calls = new Stack<CallInfo>();
        }

        public void Define(string name, MethodAttributes attr, Type returnType, Type[] parms)
        {
            this.m_Builder = this.m_TypeBuilder.DefineMethod(name, attr, returnType, parms);
            this.m_Generator = this.m_Builder.GetILGenerator();

            this.m_ArgumentTypes = parms;
        }

        public LocalBuilder CreateLocal(Type localType)
        {
            return this.m_Generator.DeclareLocal(localType);
        }

        public LocalBuilder AcquireTemp(Type localType)
        {
            Queue<LocalBuilder> list;

            if (!this.m_Temps.TryGetValue(localType, out list))
                this.m_Temps[localType] = list = new Queue<LocalBuilder>();

            if (list.Count > 0)
                return list.Dequeue();

            return this.CreateLocal(localType);
        }

        public void ReleaseTemp(LocalBuilder local)
        {
            Queue<LocalBuilder> list;

            if (!this.m_Temps.TryGetValue(local.LocalType, out list))
                this.m_Temps[local.LocalType] = list = new Queue<LocalBuilder>();

            list.Enqueue(local);
        }

        public void Branch(Label label)
        {
            this.m_Generator.Emit(OpCodes.Br, label);
        }

        public void BranchIfFalse(Label label)
        {
            this.Pop(typeof(object));

            this.m_Generator.Emit(OpCodes.Brfalse, label);
        }

        public void BranchIfTrue(Label label)
        {
            this.Pop(typeof(object));

            this.m_Generator.Emit(OpCodes.Brtrue, label);
        }

        public Label CreateLabel()
        {
            return this.m_Generator.DefineLabel();
        }

        public void MarkLabel(Label label)
        {
            this.m_Generator.MarkLabel(label);
        }

        public void Pop()
        {
            this.m_Stack.Pop();
        }

        public void Pop(Type expected)
        {
            if (expected == null)
                throw new InvalidOperationException("Expected type cannot be null.");

            Type onStack = this.m_Stack.Pop();

            if (expected == typeof(bool))
                expected = typeof(int);

            if (onStack == typeof(bool))
                onStack = typeof(int);

            if (!expected.IsAssignableFrom(onStack))
                throw new InvalidOperationException("Unexpected stack state.");
        }

        public void Push(Type type)
        {
            this.m_Stack.Push(type);
        }

        public void Return()
        {
            if (this.m_Stack.Count != (this.m_Builder.ReturnType == typeof(void) ? 0 : 1))
                throw new InvalidOperationException("Stack return mismatch.");

            this.m_Generator.Emit(OpCodes.Ret);
        }

        public void LoadNull()
        {
            this.LoadNull(typeof(object));
        }

        public void LoadNull(Type type)
        {
            this.Push(type);

            this.m_Generator.Emit(OpCodes.Ldnull);
        }

        public void Load(string value)
        {
            this.Push(typeof(string));

            if (value != null)
                this.m_Generator.Emit(OpCodes.Ldstr, value);
            else
                this.m_Generator.Emit(OpCodes.Ldnull);
        }

        public void Load(Enum value)
        {
            int toLoad = ((IConvertible)value).ToInt32(null);
            this.Load(toLoad);

            this.Pop();
            this.Push(value.GetType());
        }

        public void Load(long value)
        {
            this.Push(typeof(long));

            this.m_Generator.Emit(OpCodes.Ldc_I8, value);
        }

        public void Load(float value)
        {
            this.Push(typeof(float));

            this.m_Generator.Emit(OpCodes.Ldc_R4, value);
        }

        public void Load(double value)
        {
            this.Push(typeof(double));

            this.m_Generator.Emit(OpCodes.Ldc_R8, value);
        }

        public void Load(char value)
        {
            this.Load((int)value);

            this.Pop();
            this.Push(typeof(char));
        }

        public void Load(bool value)
        {
            this.Push(typeof(bool));

            if (value)
                this.m_Generator.Emit(OpCodes.Ldc_I4_1);
            else
                this.m_Generator.Emit(OpCodes.Ldc_I4_0);
        }

        public void Load(int value)
        {
            this.Push(typeof(int));

            switch ( value )
            {
                case -1:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    this.m_Generator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                        this.m_Generator.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    else
                        this.m_Generator.Emit(OpCodes.Ldc_I4, value);

                    break;
            }
        }

        public void LoadField(FieldInfo field)
        {
            this.Pop(field.DeclaringType);

            this.Push(field.FieldType);

            this.m_Generator.Emit(OpCodes.Ldfld, field);
        }

        public void LoadLocal(LocalBuilder local)
        {
            this.Push(local.LocalType);

            int index = local.LocalIndex;

            switch ( index )
            {
                case 0:
                    this.m_Generator.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    this.m_Generator.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    this.m_Generator.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    this.m_Generator.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (index >= byte.MinValue && index <= byte.MinValue)
                        this.m_Generator.Emit(OpCodes.Ldloc_S, (byte)index);
                    else
                        this.m_Generator.Emit(OpCodes.Ldloc, (short)index);

                    break;
            }
        }

        public void StoreLocal(LocalBuilder local)
        {
            this.Pop(local.LocalType);

            this.m_Generator.Emit(OpCodes.Stloc, local);
        }

        public void LoadArgument(int index)
        {
            if (index > 0)
                this.Push(this.m_ArgumentTypes[index - 1]);
            else
                this.Push(this.m_TypeBuilder);

            switch ( index )
            {
                case 0:
                    this.m_Generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    this.m_Generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    this.m_Generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    this.m_Generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index >= byte.MinValue && index <= byte.MaxValue)
                        this.m_Generator.Emit(OpCodes.Ldarg_S, (byte)index);
                    else
                        this.m_Generator.Emit(OpCodes.Ldarg, (short)index);

                    break;
            }
        }

        public void CastAs(Type type)
        {
            this.Pop(typeof(object));
            this.Push(type);

            this.m_Generator.Emit(OpCodes.Isinst, type);
        }

        public void Neg()
        {
            this.Pop(typeof(int));

            this.Push(typeof(int));

            this.m_Generator.Emit(OpCodes.Neg);
        }

        public void Compare(OpCode opCode)
        {
            this.Pop();
            this.Pop();

            this.Push(typeof(int));

            this.m_Generator.Emit(opCode);
        }

        public void LogicalNot()
        {
            this.Pop(typeof(int));

            this.Push(typeof(int));

            this.m_Generator.Emit(OpCodes.Ldc_I4_0);
            this.m_Generator.Emit(OpCodes.Ceq);
        }

        public void Xor()
        {
            this.Pop(typeof(int));
            this.Pop(typeof(int));

            this.Push(typeof(int));

            this.m_Generator.Emit(OpCodes.Xor);
        }

        public Type Active
        {
            get
            {
                return this.m_Stack.Peek();
            }
        }

        public void Chain(Property prop)
        {
            for (int i = 0; i < prop.Chain.Length; ++i)
                this.Call(prop.Chain[i].GetGetMethod());
        }

        public void Call(MethodInfo method)
        {
            this.BeginCall(method);

            CallInfo call = this.m_Calls.Peek();

            if (call.parms.Length > 0)
                throw new InvalidOperationException("Method requires parameters.");

            this.FinishCall();
        }

        public delegate void Callback();

        public bool CompareTo(int sign, Callback argGenerator)
        {
            Type active = this.Active;

            MethodInfo compareTo = active.GetMethod("CompareTo", new Type[] { active });

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
                Type[] ifaces = active.FindInterfaces(delegate(Type type, object obj)
                {
                    return (type.IsGenericType) &&
                           (type.GetGenericTypeDefinition() == typeof(IComparable<>)) &&
                           (type.GetGenericArguments()[0].IsAssignableFrom(active));
                }, null);

                if (ifaces.Length > 0)
                {
                    compareTo = ifaces[0].GetMethod("CompareTo", new Type[] { active });
                }
                else
                {
                    ifaces = active.FindInterfaces(delegate(Type type, object obj)
                    {
                        return (type == typeof(IComparable));
                    }, null);

                    if (ifaces.Length > 0)
                        compareTo = ifaces[0].GetMethod("CompareTo", new Type[] { active });
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
                LocalBuilder aValue = this.AcquireTemp(active);
                LocalBuilder bValue = this.AcquireTemp(active);

                this.StoreLocal(aValue);

                argGenerator();

                this.StoreLocal(bValue);

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

                Label store = this.CreateLabel();

                Label aNotNull = this.CreateLabel();

                this.LoadLocal(aValue);
                this.BranchIfTrue(aNotNull);
                // if ( aValue == null )
                {
                    Label bNotNull = this.CreateLabel();

                    this.LoadLocal(bValue);
                    this.BranchIfTrue(bNotNull);
                    // if ( bValue == null )
                    {
                        this.Load(0);
                        this.Pop(typeof(int));
                        this.Branch(store);
                    }
                    this.MarkLabel(bNotNull);
                    // else
                    {
                        this.Load(sign);
                        this.Pop(typeof(int));
                        this.Branch(store);
                    }
                }
                this.MarkLabel(aNotNull);
                // else
                {
                    Label bNotNull = this.CreateLabel();

                    this.LoadLocal(bValue);
                    this.BranchIfTrue(bNotNull);
                    // bValue == null
                    {
                        this.Load(-sign);
                        this.Pop(typeof(int));
                        this.Branch(store);
                    }
                    this.MarkLabel(bNotNull);
                    // else
                    {
                        this.LoadLocal(aValue);
                        this.BeginCall(compareTo);

                        this.LoadLocal(bValue);
                        this.ArgumentPushed();

                        this.FinishCall();

                        if (sign == -1)
                            this.Neg();
                    }
                }

                this.MarkLabel(store);

                this.ReleaseTemp(aValue);
                this.ReleaseTemp(bValue);
            }
            else
            {
                this.BeginCall(compareTo);

                argGenerator();

                this.ArgumentPushed();

                this.FinishCall();

                if (sign == -1)
                    this.Neg();
            }

            return true;
        }

        public void BeginCall(MethodInfo method)
        {
            Type type;

            if ((method.CallingConvention & CallingConventions.HasThis) != 0)
                type = this.m_Stack.Peek();
            else
                type = method.DeclaringType;

            this.m_Calls.Push(new CallInfo(type, method));

            if (type.IsValueType)
            {
                LocalBuilder temp = this.AcquireTemp(type);

                this.m_Generator.Emit(OpCodes.Stloc, temp);
                this.m_Generator.Emit(OpCodes.Ldloca, temp);

                this.ReleaseTemp(temp);
            }
        }

        public void FinishCall()
        {
            CallInfo call = this.m_Calls.Pop();

            if ((call.type.IsValueType || call.type.IsByRef) && call.method.DeclaringType != call.type)
                this.m_Generator.Emit(OpCodes.Constrained, call.type);

            if (call.method.DeclaringType.IsValueType || call.method.IsStatic)
                this.m_Generator.Emit(OpCodes.Call, call.method);
            else
                this.m_Generator.Emit(OpCodes.Callvirt, call.method);

            for (int i = call.parms.Length - 1; i >= 0; --i)
                this.Pop(call.parms[i].ParameterType);

            if ((call.method.CallingConvention & CallingConventions.HasThis) != 0)
                this.Pop(call.method.DeclaringType);

            if (call.method.ReturnType != typeof(void))
                this.Push(call.method.ReturnType);
        }

        public void ArgumentPushed()
        {
            CallInfo call = this.m_Calls.Peek();

            ParameterInfo parm = call.parms[call.index++];

            Type argumentType = this.m_Stack.Peek();

            if (!parm.ParameterType.IsAssignableFrom(argumentType))
                throw new InvalidOperationException("Parameter type mismatch.");

            if (argumentType.IsValueType && !parm.ParameterType.IsValueType)
                this.m_Generator.Emit(OpCodes.Box, argumentType);
        }
    }
}