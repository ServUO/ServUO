using System;

namespace Server.Commands.Generic
{
    public sealed class WhereExtension : BaseExtension
    {
        public static ExtensionInfo ExtInfo = new ExtensionInfo(20, "Where", -1, delegate() { return new WhereExtension(); });
        private ObjectConditional m_Conditional;
        public WhereExtension()
        {
        }

        public override ExtensionInfo Info
        {
            get
            {
                return ExtInfo;
            }
        }
        public ObjectConditional Conditional
        {
            get
            {
                return this.m_Conditional;
            }
        }
        public static void Initialize()
        {
            ExtensionInfo.Register(ExtInfo);
        }

        public override void Optimize(Mobile from, Type baseType, ref AssemblyEmitter assembly)
        {
            if (baseType == null)
                throw new InvalidOperationException("Insanity.");

            this.m_Conditional.Compile(ref assembly);
        }

        public override void Parse(Mobile from, string[] arguments, int offset, int size)
        {
            if (size < 1)
                throw new Exception("Invalid condition syntax.");

            this.m_Conditional = ObjectConditional.ParseDirect(from, arguments, offset, size);
        }

        public override bool IsValid(object obj)
        {
            return this.m_Conditional.CheckCondition(obj);
        }
    }
}