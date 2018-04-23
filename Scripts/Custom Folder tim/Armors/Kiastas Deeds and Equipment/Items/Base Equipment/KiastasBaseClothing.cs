using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public abstract class KiastasBaseClothing : BaseClothing
    {
        public override int ArtifactRarity { get { return Settings.BaseEquipmentAttribute.EquipmentArtifactRarity; } }

        public KiastasBaseClothing(int itemID, Layer layer) : this (itemID, layer, 0)
        {
        }

        public KiastasBaseClothing(int itemID, Layer layer, int hue) : base(itemID, layer, hue)
        {
        }

        public KiastasBaseClothing(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
        #region ModifyAttribute Method
        public bool ModifyAttribute(Mobile from, object attribute, string attributeName, int modifier, int max)
        {
            if (attribute is AosAttribute)
            {
                if (Attributes[(AosAttribute)attribute] >= max && max > 1)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeMax, from, attributeName, modifier, max);
                    return false;
                }
                else if (Attributes[(AosAttribute)attribute] >= max && max <= 1)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeMaxBool, from, attributeName, modifier, max);
                    return false;
                }
                else if (Attributes[(AosAttribute)attribute] < max && max > 1)
                {
                    Attributes[(AosAttribute)attribute] += modifier;
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeSuccess, from, attributeName, modifier, max);
                    return true;
                }
                else if (Attributes[(AosAttribute)attribute] < max && max <= 1)
                {
                    if ((AosAttribute)attribute == AosAttribute.SpellChanneling)
                    {
                        new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, from, attributeName, modifier, max);
                        return false;
                    }
                    else
                    {
                        Attributes[(AosAttribute)attribute] += modifier;
                        new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeSuccessBool, from, attributeName, modifier, max);
                        return true;
                    }
                }
                else
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, from, attributeName, modifier, max);
                    return false;
                }
            }
            else if (attribute is AosArmorAttribute)
            {
                new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, from, attributeName, modifier, max);
                return false;
            }
            else if (attribute is AosWeaponAttribute)
            {
                if ((AosWeaponAttribute)attribute != AosWeaponAttribute.SelfRepair
                    && (AosWeaponAttribute)attribute != AosWeaponAttribute.DurabilityBonus)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, from, attributeName, modifier, max);
                }
                return false;
            }
            else if (attribute is LootType)
            {
                if (LootType == LootType.Blessed || BlessedFor == from || (Mobile.InsuranceEnabled && Insured))
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.LootTypeExists, from, attributeName, modifier, max);
                    return false;
                }
                else if (LootType != LootType.Regular)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.LootTypeError, from, attributeName, modifier, max);
                    return false;
                }
                else if (RootParent != from)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.LootTypeError, from, attributeName, modifier, max);
                    return false;
                }
                else
                {
                    LootType = (LootType)attribute;
                    new Settings.ErrorMessage(Settings.ErrorMessageType.LootTypeSuccess, from, attributeName, modifier, max);
                    return true;
                }
            }
            else if (attribute is SlayerName)
            {
                new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameError, from, attributeName, modifier, max);
                return false;
            }
            else
            {
                new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, from, attributeName, modifier, max);
                return false;
            }
        }
        #endregion
    }
}