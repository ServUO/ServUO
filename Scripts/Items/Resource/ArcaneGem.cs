using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public class ArcaneGem : Item, ICommodity
    {
        public const int DefaultArcaneHue = 2117;
        public override int LabelNumber => 1114115;  // Arcane Gem

        [Constructable]
        public ArcaneGem()
            : this(1)
        {
        }

        [Constructable]
        public ArcaneGem(int amount)
            : base(0x1EA7)
        {
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public ArcaneGem(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public static bool ConsumeCharges(Mobile from, int amount)
        {
            List<Item> items = from.Items;
            int avail = 0;

            for (int i = 0; i < items.Count; ++i)
            {
                Item obj = items[i];

                if (obj is IArcaneEquip)
                {
                    IArcaneEquip eq = (IArcaneEquip)obj;

                    if (eq.IsArcane)
                        avail += eq.CurArcaneCharges;
                }
            }

            if (avail < amount)
                return false;

            for (int i = 0; i < items.Count; ++i)
            {
                Item obj = items[i];

                if (obj is IArcaneEquip)
                {
                    IArcaneEquip eq = (IArcaneEquip)obj;

                    if (eq.IsArcane)
                    {
                        if (eq.CurArcaneCharges > amount)
                        {
                            eq.CurArcaneCharges -= amount;
                            break;
                        }
                        else
                        {
                            amount -= eq.CurArcaneCharges;
                            eq.CurArcaneCharges = 0;
                        }
                    }
                }
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
            }
            else
            {
                from.BeginTarget(2, false, TargetFlags.None, OnTarget);
            }
        }

        public int GetChargesFor(Mobile m)
        {
            int v = (int)(m.Skills[SkillName.Tailoring].Value / 5);

            if (v < 16)
                return 16;
            else if (v > 24)
                return 24;

            return v;
        }

        public void OnTarget(Mobile from, object obj)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
                return;
            }

            if (obj is IArcaneEquip && obj is Item)
            {
                Item item = (Item)obj;
                CraftResource resource = CraftResource.None;

                if (item is BaseClothing)
                    resource = ((BaseClothing)item).Resource;
                else if (item is BaseArmor)
                    resource = ((BaseArmor)item).Resource;
                else if (item is BaseWeapon) // Sanity, weapons cannot recieve gems...
                    resource = ((BaseWeapon)item).Resource;

                IArcaneEquip eq = (IArcaneEquip)obj;

                if (!item.IsChildOf(from.Backpack))
                {
                    from.SendMessage("You may only target items in your backpack.");
                    return;
                }

                int charges = GetChargesFor(from);

                if (eq.IsArcane)
                {
                    if (eq.CurArcaneCharges > 0)
                    {
                        from.SendMessage("This item still has charges left.");
                    }
                    else
                    {
                        item.Hue = eq.TempHue;

                        if (charges >= eq.MaxArcaneCharges)
                        {
                            eq.CurArcaneCharges = eq.MaxArcaneCharges;
                            from.SendMessage("Your skill in tailoring allows you to fully recharge the item.");
                        }
                        else
                        {
                            eq.CurArcaneCharges += charges;
                            from.SendMessage("You are only able to restore some of the charges.");

                        }

                        Consume();
                    }
                }
                else if (from.Skills[SkillName.Tailoring].Value >= 60.0)
                {
                    bool isExceptional = false;

                    if (item is BaseClothing)
                        isExceptional = ((BaseClothing)item).Quality == ItemQuality.Exceptional;
                    else if (item is BaseArmor)
                        isExceptional = ((BaseArmor)item).Quality == ItemQuality.Exceptional;
                    else if (item is BaseWeapon)
                        isExceptional = ((BaseWeapon)item).Quality == ItemQuality.Exceptional;

                    if (isExceptional)
                    {
                        if (item is BaseClothing)
                        {
                            BaseClothing cloth = item as BaseClothing;

                            cloth.Quality = ItemQuality.Normal;
                            cloth.Crafter = from;
                        }
                        else if (item is BaseArmor)
                        {
                            BaseArmor armor = item as BaseArmor;

                            if (armor.IsImbued || armor.IsArtifact || RunicReforging.GetArtifactRarity(armor) > 0)
                            {
                                from.SendLocalizedMessage(1049690); // Arcane gems cannot be used on that type of leather.
                                return;
                            }

                            armor.Quality = ItemQuality.Normal;
                            armor.Crafter = from;
                            armor.PhysicalBonus = 0;
                            armor.FireBonus = 0;
                            armor.ColdBonus = 0;
                            armor.PoisonBonus = 0;
                            armor.EnergyBonus = 0;
                        }
                        else if (item is BaseWeapon) // Sanity, weapons cannot recieve gems...
                        {
                            BaseWeapon weapon = item as BaseWeapon;

                            weapon.Quality = ItemQuality.Normal;
                            weapon.Crafter = from;
                        }

                        eq.CurArcaneCharges = eq.MaxArcaneCharges = charges;

                        item.Hue = DefaultArcaneHue;

                        if (item.LootType == LootType.Blessed)
                            item.LootType = LootType.Regular;

                        Consume();
                    }
                    else
                    {
                        from.SendMessage("You can only use this on exceptionally crafted robes, thigh boots, cloaks, or leather gloves.");
                    }
                }
                else
                {
                    from.SendMessage("You do not have enough skill in tailoring to use this.");
                }
            }
            else
            {
                from.SendMessage("You can only use this on exceptionally crafted robes, thigh boots, cloaks, or leather gloves.");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
