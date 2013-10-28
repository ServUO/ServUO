using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Items;
using Server.Targeting;

namespace Server.Engines.Craft
{
    public class AlterItem
    {
        public static Dictionary<Type, Type> TypeList = new Dictionary<Type, Type>();
        public static void Configure()
        {
            // Weapons
            TypeList.Add(typeof(VikingSword), typeof(StoneWarSword));
            TypeList.Add(typeof(BoneHarvester), typeof(GlassSword));
            TypeList.Add(typeof(ElvenMachete), typeof(GlassSword));
            TypeList.Add(typeof(Katana), typeof(GargishKatana));
            TypeList.Add(typeof(Cutlass), typeof(GlassSword));
            TypeList.Add(typeof(Cleaver), typeof(GargishCleaver));
            TypeList.Add(typeof(RadiantScimitar), typeof(GlassSword));
            TypeList.Add(typeof(Scimitar), typeof(GlassSword));
            TypeList.Add(typeof(Longsword), typeof(DreadSword));
            TypeList.Add(typeof(Broadsword), typeof(BloodBlade));
            TypeList.Add(typeof(Bardiche), typeof(GargishBardiche));
            TypeList.Add(typeof(Halberd), typeof(GargishTalwar));
            TypeList.Add(typeof(NoDachi), typeof(GargishTalwar));
            TypeList.Add(typeof(Daisho), typeof(GargishDaisho));
            TypeList.Add(typeof(RuneBlade), typeof(GargishTalwar));
            TypeList.Add(typeof(CrescentBlade), typeof(GargishTalwar));
            TypeList.Add(typeof(Scythe), typeof(GargishScythe));
            TypeList.Add(typeof(WarFork), typeof(GargishWarFork));
            TypeList.Add(typeof(WarCleaver), typeof(GargishCleaver));
            TypeList.Add(typeof(AssassinSpike), typeof(Shortblade));
            TypeList.Add(typeof(Kryss), typeof(GargishKryss));
            TypeList.Add(typeof(WarHammer), typeof(GargishWarHammer));
            TypeList.Add(typeof(WarMace), typeof(DiscMace));
            TypeList.Add(typeof(Mace), typeof(DiscMace));
            TypeList.Add(typeof(Maul), typeof(GargishMaul));
            TypeList.Add(typeof(Scepter), typeof(DiscMace));
            TypeList.Add(typeof(Club), typeof(DiscMace));
            TypeList.Add(typeof(HammerPick), typeof(DiscMace));
            TypeList.Add(typeof(DiamondMace), typeof(DiscMace));
            TypeList.Add(typeof(WildStaff), typeof(GargishGnarledStaff));
            TypeList.Add(typeof(WarAxe), typeof(DiscMace));
            TypeList.Add(typeof(Tetsubo), typeof(GlassStaff));
            TypeList.Add(typeof(Tessen), typeof(GargishTessen));
            TypeList.Add(typeof(BlackStaff), typeof(GlassStaff));
            TypeList.Add(typeof(QuarterStaff), typeof(GlassStaff));
            TypeList.Add(typeof(GnarledStaff), typeof(GargishGnarledStaff));
            TypeList.Add(typeof(OrnateAxe), typeof(GargishBattleAxe));
            TypeList.Add(typeof(DoubleAxe), typeof(DualShortAxes));
            TypeList.Add(typeof(ExecutionersAxe), typeof(DualShortAxes));
            TypeList.Add(typeof(Axe), typeof(GargishAxe));
            TypeList.Add(typeof(TwoHandedAxe), typeof(DualShortAxes));
            TypeList.Add(typeof(LargeBattleAxe), typeof(GargishBattleAxe));
            TypeList.Add(typeof(Spear), typeof(DualPointedSpear));
            TypeList.Add(typeof(Kama), typeof(DualPointedSpear));
            TypeList.Add(typeof(Sai), typeof(DualPointedSpear));
            TypeList.Add(typeof(ShortSpear), typeof(DualPointedSpear));
            TypeList.Add(typeof(BladedStaff), typeof(DualPointedSpear));
            TypeList.Add(typeof(DoubleBladedStaff), typeof(DualPointedSpear));
            TypeList.Add(typeof(Dagger), typeof(GargishDagger));
            TypeList.Add(typeof(SkinningKnife), typeof(GargishButcherKnife));

            // Shields
            TypeList.Add(typeof(MetalKiteShield), typeof(GargishKiteShield));
            TypeList.Add(typeof(WoodenKiteShield), typeof(LargeStoneShield));
            TypeList.Add(typeof(Buckler), typeof(SmallPlateShield));
            TypeList.Add(typeof(BronzeShield), typeof(SmallPlateShield));
            TypeList.Add(typeof(HeaterShield), typeof(LargePlateShield));
            TypeList.Add(typeof(WoodenShield), typeof(GargishWoodenShield));
            TypeList.Add(typeof(MetalShield), typeof(MediumPlateShield));
            TypeList.Add(typeof(ChaosShield), typeof(GargishChaosShield));
            TypeList.Add(typeof(OrderShield), typeof(GargishOrderShield));

            // Leather Armor
            TypeList.Add(typeof(FemaleLeatherChest), typeof(FemaleGargishLeatherChest));
            TypeList.Add(typeof(LeatherShorts), typeof(GargishLeatherKilt));
            TypeList.Add(typeof(LeatherSkirt), typeof(FemaleGargishLeatherKilt));
            TypeList.Add(typeof(LeatherBustierArms), typeof(FemaleGargishLeatherArms));
            TypeList.Add(typeof(LeatherChest), typeof(GargishLeatherChest));
            TypeList.Add(typeof(LeatherArms), typeof(GargishLeatherArms));
            TypeList.Add(typeof(LeatherLegs), typeof(GargishLeatherLegs));

            // Wooden Armor
            TypeList.Add(typeof(WoodlandChest), typeof(GargishStoneChest));
            TypeList.Add(typeof(WoodlandArms), typeof(GargishStoneArms));
            TypeList.Add(typeof(WoodlandLegs), typeof(GargishStoneLegs));
        }

        public static void BeginTarget(Mobile from, CraftSystem system, BaseTool tool)
        {
            from.Target = new AlterItemTarget(system, tool);
            from.SendMessage("Choose an item to Alter");
        }

        public static bool TryToAlter(Mobile from, Item olditem)
        {
            if (!TypeList.ContainsKey(olditem.GetType()))
                return false;

            Item newitem = CreateItem(TypeList[olditem.GetType()]);

            if (newitem == null)
                return false;

            if (olditem is BaseWeapon && newitem is BaseWeapon)
            {
                BaseWeapon oldweapon = (BaseWeapon)olditem;
                BaseWeapon newweapon = (BaseWeapon)newitem;

                newweapon.Attributes = new AosAttributes(oldweapon, newweapon.Attributes);
                //newweapon.ElementDamages = new AosElementAttributes( oldweapon, newweapon.ElementDamages ); To Do
                newweapon.SkillBonuses = new AosSkillBonuses(oldweapon, newweapon.SkillBonuses);
                newweapon.WeaponAttributes = new AosWeaponAttributes(oldweapon, newweapon.WeaponAttributes);
                newweapon.AbsorptionAttributes = new SAAbsorptionAttributes(oldweapon, newweapon.AbsorptionAttributes);
            }
            else if (olditem is BaseArmor && newitem is BaseArmor)
            {
                BaseArmor oldarmor = (BaseArmor)olditem;
                BaseArmor newarmor = (BaseArmor)newitem;

                newarmor.Attributes = new AosAttributes(oldarmor, newarmor.Attributes);
                newarmor.ArmorAttributes = new AosArmorAttributes(oldarmor, newarmor.ArmorAttributes);
                newarmor.SkillBonuses = new AosSkillBonuses(oldarmor, newarmor.SkillBonuses);
                newarmor.AbsorptionAttributes = new SAAbsorptionAttributes(oldarmor, newarmor.AbsorptionAttributes);
            }
            else if (olditem is BaseShield && newitem is BaseShield)
            {
                BaseShield oldshield = (BaseShield)olditem;
                BaseShield newshield = (BaseShield)newitem;

                newshield.Attributes = new AosAttributes(oldshield, newshield.Attributes);
                newshield.SkillBonuses = new AosSkillBonuses(oldshield, newshield.SkillBonuses);
                newshield.AbsorptionAttributes = new SAAbsorptionAttributes(oldshield, newshield.AbsorptionAttributes);
            }
            else
            {
                return false;
            }

            olditem.Delete();

            if (from.Backpack == null)
                newitem.MoveToWorld(from.Location, from.Map);
            else
                from.Backpack.DropItem(newitem);

            return true;
        }

        public static Item CreateItem(Type t)
        {
            try
            {
                return (Item)Activator.CreateInstance(t);
            }
            catch
            {
                return null;
            }
        }
    }

    public class AlterItemTarget : Target
    {
        private readonly CraftSystem m_System;
        private readonly BaseTool m_Tool;
        /// mod
        public AlterItemTarget(CraftSystem system, BaseTool tool)
            : base(1, false, TargetFlags.None)
        {
            this.m_System = system;
            this.m_Tool = tool;// mod
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (!(o is Item))
            {
                from.SendMessage("You cannot convert people into gargoyles using this.");
            }
            else if (o is BaseWeapon)
            {
                BaseWeapon bw = (BaseWeapon)o;

                this.CheckResource(from, bw, bw.Resource);
            }
            else if (o is BaseArmor)
            {
                BaseArmor ba = (BaseArmor)o;

                this.CheckResource(from, ba, ba.Resource);
            }
            else if (o is BaseShield)
            {
                BaseShield bs = (BaseShield)o;

                this.CheckResource(from, bs, bs.Resource);
            }
        }

        private void CheckResource(Mobile from, Item item, CraftResource res)
        {
            bool completed = false;

            if (this.m_System is DefTailoring)//
            {
                switch (res)
                {
                    case CraftResource.RegularLeather:
                    case CraftResource.SpinedLeather:
                    case CraftResource.HornedLeather:
                    case CraftResource.BarbedLeather:

                        //default:
                        completed = AlterItem.TryToAlter(from, item);
                        break;
                }
            }
            else if (this.m_System is DefBlacksmithy)
            {
                switch (res)
                {
                    // default: // Not listing ores, it's the only logical remainder.
                    case CraftResource.Iron:
                    case CraftResource.DullCopper:
                    case CraftResource.ShadowIron:
                    case CraftResource.Copper:
                    case CraftResource.Bronze:
                    case CraftResource.Gold:
                    case CraftResource.Agapite:
                    case CraftResource.Verite:
                    case CraftResource.Valorite:

                        //if (m_Tool is SmithHammer)
                        completed = AlterItem.TryToAlter(from, item);
                        break;
                }
            }
            else if (this.m_System is DefCarpentry)
            {
                switch (res)
                {
                    case CraftResource.RegularWood:
                    case CraftResource.OakWood:
                    case CraftResource.AshWood:
                    case CraftResource.YewWood:
                    case CraftResource.Heartwood:
                    case CraftResource.Bloodwood:
                    case CraftResource.Frostwood:

                        //if (m_Tool is Hammer)
                        completed = AlterItem.TryToAlter(from, item);
                        break;
                }
            }
            else
            {
                from.SendMessage("You cannot use this to alter that");
            }

            if (completed)
                from.SendMessage("The item has been turned into a gargish item.");
            else
                from.SendMessage("You cannot use this to alter that");
        }
    }
}

namespace Server.Commands
{
    public class AlterItemCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("AlterItem", AccessLevel.GameMaster, new CommandEventHandler(AlterItem_OnCommand));
        }

        [Description("Converts a human/elf item into a a gargoyle item.")]
        public static void AlterItem_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(10, false, TargetFlags.None, new TargetCallback(AlterItem_CallBack));
        }

        public static void AlterItem_CallBack(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                if (AlterItem.TryToAlter(from, (Item)targeted))
                    from.SendMessage("The item has been turned into a gargish item.");
                else
                    from.SendMessage("That could not be altered.");
            }
            else
                from.SendMessage("That is not an item.");
        }
    }
}