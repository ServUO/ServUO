using System;
using Server;
using Server.Items;
using Server.Targets;

namespace Server.Kiasta
{
    public abstract class KiastasBaseWeapon : BaseMeleeWeapon
    {
        public override int ArtifactRarity { get { return Settings.BaseEquipmentAttribute.EquipmentArtifactRarity; } }

        public KiastasBaseWeapon(int itemID) : base(itemID)
        {
            Resource = Settings.BaseEquipmentAttribute.ResourceType[Utility.Random(6)];
        }

        public KiastasBaseWeapon(Serial serial) : base(serial)
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

        /*public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("What do you want to use this item on?");
            from.Target = new BladedItemTarget(this);
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);
            if (!Core.AOS && Poison != null && PoisonCharges > 0)
            {
                --PoisonCharges;
                if (Utility.RandomDouble() >= 0.5) // 50% chance to poison
                    defender.ApplyPoison(attacker, Poison);
            }
        }*/
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
                    Attributes[(AosAttribute)attribute] += modifier;
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeSuccessBool, from, attributeName, modifier, max);
                    return true;
                }
                else
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, from, attributeName, modifier, max);
                    return false;
                }
            }
            else if (attribute is AosArmorAttribute)
            {
                if ((AosArmorAttribute)attribute != AosArmorAttribute.DurabilityBonus
                    && (AosArmorAttribute)attribute != AosArmorAttribute.SelfRepair)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, from, attributeName, modifier, max);
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else if (attribute is AosWeaponAttribute)
            {
                if (WeaponAttributes[(AosWeaponAttribute)attribute] >= max && max > 1)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeMax, from, attributeName, modifier, max);
                    return false;
                }
                else if (WeaponAttributes[(AosWeaponAttribute)attribute] >= max && max <= 1)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeMaxBool, from, attributeName, modifier, max);
                    return false;
                }
                else if (WeaponAttributes[(AosWeaponAttribute)attribute] < max && max > 1)
                {
                    WeaponAttributes[(AosWeaponAttribute)attribute] += modifier;
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeSuccess, from, attributeName, modifier, max);
                    return true;
                }
                else if (WeaponAttributes[(AosWeaponAttribute)attribute] < max && max <= 1)
                {
                    WeaponAttributes[(AosWeaponAttribute)attribute] += modifier;
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeSuccessBool, from, attributeName, modifier, max);
                    return true;
                }
                else
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.AttributeError, from, attributeName, modifier, max);
                    return false;
                }
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
                if ((Slayer != SlayerName.None || Slayer2 != SlayerName.None) && (SlayerName)attribute == SlayerName.None)
                {
                    Slayer = (SlayerName)attribute;
                    Slayer2 = (SlayerName)attribute;
                    new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameSuccess, from, attributeName, modifier, max);
                    return true;
                }
                else if ((Slayer == SlayerName.None && Slayer2 == SlayerName.None) && (SlayerName)attribute == SlayerName.None)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameRemovalError, from, attributeName, modifier, max);
                    return false;
                }
                else if (Slayer == SlayerName.None && Slayer2 == SlayerName.None)
                {
                    Slayer = (SlayerName)attribute;
                    new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameSuccess, from, attributeName, modifier, max);
                    return true;
                }
                else if (Slayer != SlayerName.None && Slayer2 == SlayerName.None)
                {
                    if (Slayer != (SlayerName)attribute && Slayer2 != (SlayerName)attribute)
                    {
                        Slayer2 = (SlayerName)attribute;
                        new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameSuccess, from, attributeName, modifier, max);
                        return true;
                    }
                    else
                    {
                        new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameExists, from, attributeName, modifier, max);
                        return false;
                    }
                }
                else if (Slayer == SlayerName.None && Slayer2 != SlayerName.None)
                {
                    if (Slayer != (SlayerName)attribute && Slayer2 != (SlayerName)attribute)
                    {
                        Slayer = Slayer2;
                        Slayer = (SlayerName)attribute;
                        new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameSuccess, from, attributeName, modifier, max);
                        return true;
                    }
                    else
                    {
                        new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameExists, from, attributeName, modifier, max);
                        return false;
                    }
                }
                else if ((SlayerName)attribute == Slayer || (SlayerName)attribute == Slayer2)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameExists, from, attributeName, modifier, max);
                    return false;
                }
                else if (Slayer != SlayerName.None && Slayer2 != SlayerName.None)
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameMax, from, attributeName, modifier, max);
                    return false;
                }
                else
                {
                    new Settings.ErrorMessage(Settings.ErrorMessageType.SlayerNameError, from, attributeName, modifier, max);
                    return false;
                }
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