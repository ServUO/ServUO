using System;
using System.Reflection;
using Server;
using Server.Items;

namespace Server.Kiasta.Deeds
{
    class ModifyItem
    {
        #region Variable Declarations
        private Mobile m_From;
        private int m_AttributeValue;
        private int m_Max;
        private object m_Modifier;
        private object[] m_Attribute;
        private Type[] m_EquipmentTypesAllowed = Settings.EquipmentTypes.AllowedItems;
        private string m_ModifyAttributeMethodName = Settings.Misc.ModifyAttributeMethodName;
        private string m_AttributeName;
        private object o_Target;
        public bool IsApplied;
        #endregion

        #region Constructors
        public ModifyItem(Mobile from, object target, object[] attribute, string attributeName, int max, object modifier)
        {
            m_From = from;
            o_Target = target;
            m_Attribute = attribute;
            m_AttributeName = attributeName;
            m_Max = max;
            m_Modifier = modifier;
            IsApplied = false;
            DoModify();
        }
        #endregion

        #region Modifier Method
        private void DoModify()
        {
            for (int i = 0; i < m_Attribute.Length; i++)
            {
                try
                {
                    for (int x = 0; x < Settings.EquipmentTypes.Length; x++)
                    {
                        if (o_Target.GetType().BaseType.Equals(m_EquipmentTypesAllowed.GetValue(x)))
                        {
                            if (Settings.Misc.Debug) { m_From.SendMessage(m_EquipmentTypesAllowed.GetValue(i).ToString()); }
                            object MethodExists = o_Target.GetType().GetMethod(m_ModifyAttributeMethodName, new Type[] { typeof(Mobile), typeof(object), typeof(string), typeof(int), typeof(int) });
                            if (MethodExists != null)
                            {
                                if (Settings.Misc.Debug) { m_From.SendMessage("M_Attribute Element: {0}", m_Attribute.GetValue(i).GetType().ToString()); }
                                object getIsApplied = o_Target.GetType().GetMethod(m_ModifyAttributeMethodName, new Type[] { typeof(Mobile), typeof(object), typeof(string), typeof(int), typeof(int) }).Invoke(o_Target, new object[] { m_From, m_Attribute.GetValue(i), m_AttributeName, m_Modifier, m_Max });
                                if (getIsApplied is bool && !Settings.Misc.Debug)
                                {
                                    if ((bool)getIsApplied)
                                    {
                                        Settings.AttributeModifiyAnimation.DoAnimation(m_From); 
                                        IsApplied = true;
                                    }
                                }
                                else 
                                {
                                    if (Settings.Misc.Debug) { m_From.SendMessage("Invoke is not returning bool"); }
                                }
                            }
                        }
                        if (o_Target.GetType().BaseType.BaseType.Equals(m_EquipmentTypesAllowed.GetValue(x)))
                        {
                            if (Settings.Misc.Debug) { m_From.SendMessage(m_EquipmentTypesAllowed.GetValue(i).ToString()); }
                            object MethodExists = o_Target.GetType().GetMethod(m_ModifyAttributeMethodName, new Type[] { typeof(Mobile), typeof(object), typeof(string), typeof(int), typeof(int) });
                            if (MethodExists != null)
                            {
                                if (Settings.Misc.Debug) { m_From.SendMessage("M_Attribute Element: {0}", m_Attribute.GetValue(i).GetType().ToString()); }
                                object getIsApplied = o_Target.GetType().GetMethod(m_ModifyAttributeMethodName, new Type[] { typeof(Mobile), typeof(object), typeof(string), typeof(int), typeof(int) }).Invoke(o_Target, new object[] { m_From, m_Attribute.GetValue(i), m_AttributeName, m_Modifier, m_Max });
                                if (getIsApplied is bool && !Settings.Misc.Debug)
                                {
                                    if ((bool)getIsApplied)
                                    {
                                        Settings.AttributeModifiyAnimation.DoAnimation(m_From);
                                        IsApplied = true;
                                    }
                                }
                                else
                                {
                                    if (Settings.Misc.Debug) { m_From.SendMessage("Invoke is not returning bool"); }
                                }
                            }
                        }
                        else if (o_Target.GetType().BaseType.BaseType.BaseType.Equals(m_EquipmentTypesAllowed.GetValue(x)))
                        {
                            if (Settings.Misc.Debug) { m_From.SendMessage(m_EquipmentTypesAllowed.GetValue(i).ToString()); }
                            object MethodExists = o_Target.GetType().GetMethod(m_ModifyAttributeMethodName, new Type[] { typeof(Mobile), typeof(object), typeof(string), typeof(int), typeof(int) });
                            if (MethodExists != null)
                            {
                                if (Settings.Misc.Debug) { m_From.SendMessage("M_Attribute Element: {0}", m_Attribute.GetValue(i).GetType().ToString()); }
                                object getIsApplied = o_Target.GetType().GetMethod(m_ModifyAttributeMethodName, new Type[] { typeof(Mobile), typeof(object), typeof(string), typeof(int), typeof(int) }).Invoke(o_Target, new object[] { m_From, m_Attribute.GetValue(i), m_AttributeName, m_Modifier, m_Max });
                                if (getIsApplied is bool && !Settings.Misc.Debug)
                                {
                                    if ((bool)getIsApplied)
                                    {
                                        Settings.AttributeModifiyAnimation.DoAnimation(m_From);
                                        IsApplied = true;
                                    }
                                }
                                else
                                {
                                    if (Settings.Misc.Debug) { m_From.SendMessage("Invoke is not returning bool"); }
                                }
                            }
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                    if (Settings.Misc.Debug) { m_From.SendMessage("{0}: NullReferenceException: {1}", this.GetType().ToString(), e.ToString()); }
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, m_From, m_AttributeName, m_Modifier, m_Max);
                }
                catch (Exception e)
                {
                    if (Settings.Misc.Debug) { m_From.SendMessage("{0}: Exception: {1}", this.GetType().ToString(), e.ToString()); }
                }
            }
        }
        #endregion
    }
}
