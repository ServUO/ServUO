using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
    public class SalvageBag : Bag
    {
        private bool m_Failure;
		
        public override int LabelNumber
        {
            get
            {
                return 1079931;
            }
        }// Salvage Bag

        [Constructable]
        public SalvageBag()
            : this(Utility.RandomBlueHue())
        {
        }

        [Constructable]
        public SalvageBag(int hue)
        {
            this.Weight = 2.0;
            this.Hue = hue;
            this.m_Failure = false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
            {
                list.Add(new SalvageIngotsEntry(this, this.IsChildOf(from.Backpack) && this.Resmeltables()));
                list.Add(new SalvageClothEntry(this, this.IsChildOf(from.Backpack) && this.Scissorables()));
                list.Add(new SalvageAllEntry(this, this.IsChildOf(from.Backpack) && this.Resmeltables() && this.Scissorables()));
            }
        }
		
        #region Checks
        private bool Resmeltables() //Where context menu checks for metal items and dragon barding deeds
        {
            foreach (Item i in this.Items)
            {
                if (i != null && !i.Deleted)
                {
                    if (i is BaseWeapon)
                    {
                        if (CraftResources.GetType(((BaseWeapon)i).Resource) == CraftResourceType.Metal)
                            return true;
                    }
                    if (i is BaseArmor)
                    {
                        if (CraftResources.GetType(((BaseArmor)i).Resource) == CraftResourceType.Metal)
                            return true;
                    }
                    if (i is DragonBardingDeed)
                        return true;
                }
            }
            return false;
        }
		
        private bool Scissorables() //Where context menu checks for Leather items and cloth items
        {
            foreach (Item i in this.Items)
            {
                if (i != null && !i.Deleted)
                {
                    if (i is IScissorable)
                    {
                        if (i is BaseClothing)
                            return true;
                        if (i is BaseArmor)
                        {
                            if (CraftResources.GetType(((BaseArmor)i).Resource) == CraftResourceType.Leather)
                                return true;
                        }
                        if ((i is Cloth) || (i is BoltOfCloth) || (i is Hides) || (i is BonePile))
                            return true;
                    }
                }
            }
            return false;
        }

        #endregion	

        #region Resmelt.cs
        private bool Resmelt(Mobile from, Item item, CraftResource resource)
        {
            try
            {
                if (CraftResources.GetType(resource) != CraftResourceType.Metal)
                    return false;

                CraftResourceInfo info = CraftResources.GetInfo(resource);

                if (info == null || info.ResourceTypes.Length == 0)
                    return false;

                CraftItem craftItem = DefBlacksmithy.CraftSystem.CraftItems.SearchFor(item.GetType());

                if (craftItem == null || craftItem.Resources.Count == 0)
                    return false;

                CraftRes craftResource = craftItem.Resources.GetAt(0);

                if (craftResource.Amount < 2)
                    return false; // Not enough metal to resmelt
					
                double difficulty = 0.0;

                switch ( resource )
                {
                    case CraftResource.DullCopper:
                        difficulty = 65.0;
                        break;
                    case CraftResource.ShadowIron:
                        difficulty = 70.0;
                        break;
                    case CraftResource.Copper:
                        difficulty = 75.0;
                        break;
                    case CraftResource.Bronze:
                        difficulty = 80.0;
                        break;
                    case CraftResource.Gold:
                        difficulty = 85.0;
                        break;
                    case CraftResource.Agapite:
                        difficulty = 90.0;
                        break;
                    case CraftResource.Verite:
                        difficulty = 95.0;
                        break;
                    case CraftResource.Valorite:
                        difficulty = 99.0;
                        break;
                }

                Type resourceType = info.ResourceTypes[0];
                Item ingot = (Item)Activator.CreateInstance(resourceType);

                if (item is DragonBardingDeed || (item is BaseArmor && ((BaseArmor)item).PlayerConstructed) || (item is BaseWeapon && ((BaseWeapon)item).PlayerConstructed) || (item is BaseClothing && ((BaseClothing)item).PlayerConstructed))
                {
                    double mining = from.Skills[SkillName.Mining].Value;
                    if (mining > 100.0)
                        mining = 100.0;
                    double amount = (((4 + mining) * craftResource.Amount - 4) * 0.0068);
                    if (amount < 2)
                        ingot.Amount = 2;
                    else
                        ingot.Amount = (int)amount;
                }
                else
                {
                    ingot.Amount = 2;
                }
				
                if (difficulty > from.Skills[SkillName.Mining].Value)
                {
                    this.m_Failure = true; 
                    ingot.Delete();
                }
                else
                    item.Delete();

                from.AddToBackpack(ingot);

                from.PlaySound(0x2A);
                from.PlaySound(0x240);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        #endregion
		
        #region Salvaging
        private void SalvageIngots(Mobile from)
        {
            Item[] tools = from.Backpack.FindItemsByType(typeof(BaseTool));

            bool ToolFound = false;
            foreach (Item tool in tools)
            {
                if (tool is BaseTool && ((BaseTool)tool).CraftSystem == DefBlacksmithy.CraftSystem)
                    ToolFound = true;
            }

            if (!ToolFound)
            {
                from.SendLocalizedMessage(1079822); // You need a blacksmithing tool in order to salvage ingots.
                return;
            }

            bool anvil, forge;
            DefBlacksmithy.CheckAnvilAndForge(from, 2, out anvil, out forge);

            if (!forge)
            {
                from.SendLocalizedMessage(1044265); // You must be near a forge.
                return;
            }

            int salvaged = 0;
            int notSalvaged = 0;
			
            Container sBag = this;
			
            List<Item> Smeltables = sBag.FindItemsByType<Item>();

            for (int i = Smeltables.Count - 1; i >= 0; i--)
            {
                Item item = Smeltables[i];
				
                if (item is BaseArmor)
                {
                    if (this.Resmelt(from, item, ((BaseArmor)item).Resource))
                        salvaged++;
                    else
                        notSalvaged++;
                }
                else if (item is BaseWeapon)
                {
                    if (this.Resmelt(from, item, ((BaseWeapon)item).Resource))
                        salvaged++;
                    else
                        notSalvaged++;
                }
                else if (item is DragonBardingDeed)
                {
                    if (this.Resmelt(from, item, ((DragonBardingDeed)item).Resource))
                        salvaged++;

                    else
                        notSalvaged++;
                }
            }
            if (this.m_Failure)
            {
                from.SendLocalizedMessage(1079975); // You failed to smelt some metal for lack of skill.
                this.m_Failure = false;
            }
            else
                from.SendLocalizedMessage(1079973, String.Format("{0}\t{1}", salvaged, salvaged + notSalvaged)); // Salvaged: ~1_COUNT~/~2_NUM~ blacksmithed items
        }

        private void SalvageCloth(Mobile from)
        {
            Scissors scissors = from.Backpack.FindItemByType(typeof(Scissors)) as Scissors;
            if (scissors == null)
            {
                from.SendLocalizedMessage(1079823); // You need scissors in order to salvage cloth.
                return;
            }

            int salvaged = 0;
            int notSalvaged = 0;
			
            Container sBag = this;
			
            List<Item> Scissorables = sBag.FindItemsByType<Item>();

            for (int i = Scissorables.Count - 1; i >= 0; i--)
            {
                Item item = Scissorables[i];
                if (item is IScissorable)
                {
                    if (((IScissorable)item).Scissor(from, scissors))
                    {
                        salvaged++;
                    }
                    else
                    {
                        notSalvaged++;
                    }
                }
            }
			
            from.SendLocalizedMessage(1079974, String.Format("{0}\t{1}", salvaged, salvaged + notSalvaged)); // Salvaged: ~1_COUNT~/~2_NUM~ tailored items
			
            Container pack = from.Backpack;
			
            foreach (Item i in ((Container)this).FindItemsByType(typeof(Item), true))
            {
                if ((i is Leather) || (i is Cloth) || (i is SpinedLeather) || (i is HornedLeather) || (i is BarbedLeather) || (i is Bandage) || (i is Bone))
                {
                    from.AddToBackpack(i);
                }
            }
        }

        private void SalvageAll(Mobile from)
        {
            this.SalvageIngots(from);

            this.SalvageCloth(from);
        }

        #endregion

        #region ContextMenuEntries
        private class SalvageAllEntry : ContextMenuEntry
        {
            private readonly SalvageBag m_Bag;

            public SalvageAllEntry(SalvageBag bag, bool enabled)
                : base(6276)
            {
                this.m_Bag = bag;

                if (!enabled)
                    this.Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (this.m_Bag.Deleted)
                    return;

                Mobile from = this.Owner.From;

                if (from.CheckAlive())
                    this.m_Bag.SalvageAll(from);
            }
        }

        private class SalvageIngotsEntry : ContextMenuEntry
        {
            private readonly SalvageBag m_Bag;

            public SalvageIngotsEntry(SalvageBag bag, bool enabled)
                : base(6277)
            {
                this.m_Bag = bag;

                if (!enabled)
                    this.Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (this.m_Bag.Deleted)
                    return;

                Mobile from = this.Owner.From;

                if (from.CheckAlive())
                    this.m_Bag.SalvageIngots(from);
            }
        }

        private class SalvageClothEntry : ContextMenuEntry
        {
            private readonly SalvageBag m_Bag;

            public SalvageClothEntry(SalvageBag bag, bool enabled)
                : base(6278)
            {
                this.m_Bag = bag;

                if (!enabled)
                    this.Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (this.m_Bag.Deleted)
                    return;

                Mobile from = this.Owner.From;

                if (from.CheckAlive())
                    this.m_Bag.SalvageCloth(from);
            }
        }
        #endregion

        #region Serialization
        public SalvageBag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }
}