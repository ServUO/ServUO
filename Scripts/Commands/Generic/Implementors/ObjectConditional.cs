using System;
using System.Collections.Generic;

namespace Server.Commands.Generic
{
    public sealed class ObjectConditional
    {
        private static readonly Type typeofItem = typeof(Item);
        private static readonly Type typeofMobile = typeof(Mobile);

        private readonly Type m_ObjectType;

        private readonly ICondition[][] m_Conditions;

        private IConditional[] m_Conditionals;

        public Type Type => m_ObjectType;

        public bool IsItem => (m_ObjectType == null || m_ObjectType == typeofItem || m_ObjectType.IsSubclassOf(typeofItem));

        public bool IsMobile => (m_ObjectType == null || m_ObjectType == typeofMobile || m_ObjectType.IsSubclassOf(typeofMobile));

        public static readonly ObjectConditional Empty = new ObjectConditional(null, null);

        public bool HasCompiled => (m_Conditionals != null);

        public void Compile(ref AssemblyEmitter emitter)
        {
            if (emitter == null)
                emitter = new AssemblyEmitter("__dynamic", false);

            m_Conditionals = new IConditional[m_Conditions.Length];

            for (int i = 0; i < m_Conditionals.Length; ++i)
                m_Conditionals[i] = ConditionalCompiler.Compile(emitter, m_ObjectType, m_Conditions[i], i);
        }

        public bool CheckCondition(object obj)
        {
            if (m_ObjectType == null)
                return true; // null type means no condition

            if (!HasCompiled)
            {
                AssemblyEmitter emitter = null;

                Compile(ref emitter);
            }

            for (int i = 0; i < m_Conditionals.Length; ++i)
            {
                if (m_Conditionals[i].Verify(obj))
                    return true;
            }

            return false; // all conditions false
        }

        public static ObjectConditional Parse(Mobile from, ref string[] args)
        {
            string[] conditionArgs = null;

            for (int i = 0; i < args.Length; ++i)
            {
                if (Insensitive.Equals(args[i], "where"))
                {
                    string[] origArgs = args;

                    args = new string[i];

                    for (int j = 0; j < args.Length; ++j)
                        args[j] = origArgs[j];

                    conditionArgs = new string[origArgs.Length - i - 1];

                    for (int j = 0; j < conditionArgs.Length; ++j)
                        conditionArgs[j] = origArgs[i + j + 1];

                    break;
                }
            }

            return ParseDirect(from, conditionArgs, 0, conditionArgs == null ? 0 : conditionArgs.Length);
        }

        public static ObjectConditional ParseDirect(Mobile from, string[] args, int offset, int size)
        {
            if (args == null || size == 0)
                return Empty;

            int index = 0;

            Type objectType = ScriptCompiler.FindTypeByName(args[offset + index], true);

            if (objectType == null)
                throw new Exception(string.Format("No type with that name ({0}) was found.", args[offset + index]));

            ++index;

            List<ICondition[]> conditions = new List<ICondition[]>();
            List<ICondition> current = new List<ICondition>();

            current.Add(TypeCondition.Default);

            while (index < size)
            {
                string cur = args[offset + index];

                bool inverse = false;

                if (Insensitive.Equals(cur, "not") || cur == "!")
                {
                    inverse = true;
                    ++index;

                    if (index >= size)
                        throw new Exception("Improperly formatted object conditional.");
                }
                else if (Insensitive.Equals(cur, "or") || cur == "||")
                {
                    if (current.Count > 1)
                    {
                        conditions.Add(current.ToArray());

                        current.Clear();
                        current.Add(TypeCondition.Default);
                    }

                    ++index;

                    continue;
                }

                string binding = args[offset + index];
                index++;

                if (index >= size)
                    throw new Exception("Improperly formatted object conditional.");

                string oper = args[offset + index];
                index++;

                if (index >= size)
                    throw new Exception("Improperly formatted object conditional.");

                string val = args[offset + index];
                index++;

                Property prop = new Property(binding);

                prop.BindTo(objectType, PropertyAccess.Read);
                prop.CheckAccess(from);

                ICondition condition = null;

                switch (oper)
                {
                    #region Equality
                    case "=":
                    case "==":
                    case "is":
                        condition = new ComparisonCondition(prop, inverse, ComparisonOperator.Equal, val);
                        break;
                    case "!=":
                        condition = new ComparisonCondition(prop, inverse, ComparisonOperator.NotEqual, val);
                        break;
                    #endregion

                    #region Relational
                    case ">":
                        condition = new ComparisonCondition(prop, inverse, ComparisonOperator.Greater, val);
                        break;
                    case "<":
                        condition = new ComparisonCondition(prop, inverse, ComparisonOperator.Lesser, val);
                        break;
                    case ">=":
                        condition = new ComparisonCondition(prop, inverse, ComparisonOperator.GreaterEqual, val);
                        break;
                    case "<=":
                        condition = new ComparisonCondition(prop, inverse, ComparisonOperator.LesserEqual, val);
                        break;
                    #endregion

                    #region Strings
                    case "==~":
                    case "~==":
                    case "=~":
                    case "~=":
                    case "is~":
                    case "~is":
                        condition = new StringCondition(prop, inverse, StringOperator.Equal, val, true);
                        break;
                    case "!=~":
                    case "~!=":
                        condition = new StringCondition(prop, inverse, StringOperator.NotEqual, val, true);
                        break;
                    case "starts":
                        condition = new StringCondition(prop, inverse, StringOperator.StartsWith, val, false);
                        break;
                    case "starts~":
                    case "~starts":
                        condition = new StringCondition(prop, inverse, StringOperator.StartsWith, val, true);
                        break;
                    case "ends":
                        condition = new StringCondition(prop, inverse, StringOperator.EndsWith, val, false);
                        break;
                    case "ends~":
                    case "~ends":
                        condition = new StringCondition(prop, inverse, StringOperator.EndsWith, val, true);
                        break;
                    case "contains":
                        condition = new StringCondition(prop, inverse, StringOperator.Contains, val, false);
                        break;
                    case "contains~":
                    case "~contains":
                        condition = new StringCondition(prop, inverse, StringOperator.Contains, val, true);
                        break;
                        #endregion
                }

                if (condition == null)
                    throw new InvalidOperationException(string.Format("Unrecognized operator (\"{0}\").", oper));

                current.Add(condition);
            }

            conditions.Add(current.ToArray());

            return new ObjectConditional(objectType, conditions.ToArray());
        }

        public ObjectConditional(Type objectType, ICondition[][] conditions)
        {
            m_ObjectType = objectType;
            m_Conditions = conditions;
        }
    }
}
