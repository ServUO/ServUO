using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace Server.Commands.Generic
{
    public sealed class OrderInfo
    {
        private Property m_Property;
        private int m_Order;

        public Property Property
        {
            get
            {
                return this.m_Property;
            }
            set
            {
                this.m_Property = value;
            }
        }

        public bool IsAscending
        {
            get
            {
                return (this.m_Order > 0);
            }
            set
            {
                this.m_Order = (value ? +1 : -1);
            }
        }

        public bool IsDescending
        {
            get
            {
                return (this.m_Order < 0);
            }
            set
            {
                this.m_Order = (value ? -1 : +1);
            }
        }

        public int Sign
        {
            get
            {
                return Math.Sign(this.m_Order);
            }
            set
            {
                this.m_Order = Math.Sign(value);

                if (this.m_Order == 0)
                    throw new InvalidOperationException("Sign cannot be zero.");
            }
        }

        public OrderInfo(Property property, bool isAscending)
        {
            this.m_Property = property;

            this.IsAscending = isAscending;
        }
    }

    public static class SortCompiler
    {
        public static IComparer Compile(AssemblyEmitter assembly, Type objectType, OrderInfo[] orders)
        {
            TypeBuilder typeBuilder = assembly.DefineType(
                "__sort",
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

                // return;
                il.Emit(OpCodes.Ret);
            }
            #endregion

            #region IComparer
            typeBuilder.AddInterfaceImplementation(typeof(IComparer));

            MethodBuilder compareMethod;

            #region Compare
            {
                MethodEmitter emitter = new MethodEmitter(typeBuilder);

                emitter.Define(
                    /*  name  */ "Compare",
                    /*  attr  */ MethodAttributes.Public | MethodAttributes.Virtual,
                    /* return */ typeof(int),
                    /* params */ new Type[] { typeof(object), typeof(object) });

                LocalBuilder a = emitter.CreateLocal(objectType);
                LocalBuilder b = emitter.CreateLocal(objectType);

                LocalBuilder v = emitter.CreateLocal(typeof(int));

                emitter.LoadArgument(1);
                emitter.CastAs(objectType);
                emitter.StoreLocal(a);

                emitter.LoadArgument(2);
                emitter.CastAs(objectType);
                emitter.StoreLocal(b);

                emitter.Load(0);
                emitter.StoreLocal(v);

                Label end = emitter.CreateLabel();

                for (int i = 0; i < orders.Length; ++i)
                {
                    if (i > 0)
                    {
                        emitter.LoadLocal(v);
                        emitter.BranchIfTrue(end); // if ( v != 0 ) return v;
                    }

                    OrderInfo orderInfo = orders[i];

                    Property prop = orderInfo.Property;
                    int sign = orderInfo.Sign;

                    emitter.LoadLocal(a);
                    emitter.Chain(prop);

                    bool couldCompare =
                        emitter.CompareTo(sign, delegate()
                        {
                            emitter.LoadLocal(b);
                            emitter.Chain(prop);
                        });

                    if (!couldCompare)
                        throw new InvalidOperationException("Property is not comparable.");

                    emitter.StoreLocal(v);
                }

                emitter.MarkLabel(end);

                emitter.LoadLocal(v);
                emitter.Return();

                typeBuilder.DefineMethodOverride(
                    emitter.Method,
                    typeof(IComparer).GetMethod(
                        "Compare",
                        new Type[]
                        {
                            typeof(object),
                            typeof(object)
                        }));

                compareMethod = emitter.Method;
            }
            #endregion
            #endregion

            Type comparerType = typeBuilder.CreateType();

            return (IComparer)Activator.CreateInstance(comparerType);
        }
    }
}