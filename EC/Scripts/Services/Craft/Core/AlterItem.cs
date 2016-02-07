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
            //****BlackSmithy****//
            // Shields
            TypeList.Add(typeof(Buckler), typeof(SmallPlateShield));
            TypeList.Add(typeof(BronzeShield), typeof(SmallPlateShield));
            TypeList.Add(typeof(HeaterShield), typeof(LargePlateShield));
            TypeList.Add(typeof(MetalShield), typeof(MediumPlateShield));
            TypeList.Add(typeof(MetalKiteShield), typeof(GargishKiteShield));
            TypeList.Add(typeof(WoodenShield), typeof(GargishWoodenShield));
            TypeList.Add(typeof(ChaosShield), typeof(GargishChaosShield));
            TypeList.Add(typeof(OrderShield), typeof(GargishOrderShield));

            //Platemail
            TypeList.Add(typeof(FemalePlateChest), typeof(FemaleGargishPlateChest));
            TypeList.Add(typeof(PlateChest), typeof(GargishPlateChest));
            TypeList.Add(typeof(PlateArms), typeof(GargishPlateArms));
            TypeList.Add(typeof(PlateLegs), typeof(GargishPlateLegs));
            TypeList.Add(typeof(PlateDo), typeof(GargishPlateChest));
            TypeList.Add(typeof(PlateHaidate), typeof(GargishPlateLegs));
            TypeList.Add(typeof(PlateHiroSode), typeof(GargishPlateArms));
            TypeList.Add(typeof(PlateSuneate), typeof(GargishPlateLegs));
            
            // Weapons
            TypeList.Add(typeof(BattleAxe), typeof(GargishBattleAxe));
            TypeList.Add(typeof(BoneHarvester), typeof(GargishBoneHarvester));
            TypeList.Add(typeof(Katana), typeof(GargishKatana));
            TypeList.Add(typeof(Tekagi), typeof(GargishTekagi));
            TypeList.Add(typeof(Lance), typeof(GargishLance));
            TypeList.Add(typeof(Pike), typeof(GargishPike));
            TypeList.Add(typeof(Bardiche), typeof(GargishBardiche));
            TypeList.Add(typeof(Daisho), typeof(GargishDaisho));
            TypeList.Add(typeof(Scythe), typeof(GargishScythe));
            TypeList.Add(typeof(WarFork), typeof(GargishWarFork));
            TypeList.Add(typeof(Kryss), typeof(GargishKryss));
            TypeList.Add(typeof(WarHammer), typeof(GargishWarHammer));
            TypeList.Add(typeof(Maul), typeof(GargishMaul));
            TypeList.Add(typeof(Tessen), typeof(GargishTessen));
            TypeList.Add(typeof(Axe), typeof(GargishAxe));
            TypeList.Add(typeof(Dagger), typeof(GargishDagger));
            TypeList.Add(typeof(Broadsword), typeof(DreadSword));
            TypeList.Add(typeof(Longsword), typeof(DreadSword));
            TypeList.Add(typeof(CrescentBlade), typeof(GargishTalwar));
            TypeList.Add(typeof(NoDachi), typeof(GargishTalwar));
            TypeList.Add(typeof(RuneBlade), typeof(GargishTalwar));
            TypeList.Add(typeof(Halberd), typeof(GargishTalwar));
            TypeList.Add(typeof(VikingSword), typeof(StoneWarSword));
            TypeList.Add(typeof(Lajatang), typeof(DualPointedSpear));
            TypeList.Add(typeof(Kama), typeof(DualPointedSpear));
            TypeList.Add(typeof(Sai), typeof(DualPointedSpear));
            TypeList.Add(typeof(ElvenSpellblade), typeof(DualPointedSpear));
            TypeList.Add(typeof(BladedStaff), typeof(DualPointedSpear));
            TypeList.Add(typeof(DoubleBladedStaff), typeof(DualPointedSpear));
            TypeList.Add(typeof(Spear), typeof(DualPointedSpear));
            TypeList.Add(typeof(ShortSpear), typeof(DualPointedSpear));
            TypeList.Add(typeof(WarCleaver), typeof(Shortblade));
            TypeList.Add(typeof(AssassinSpike), typeof(Shortblade));
            TypeList.Add(typeof(Leafblade), typeof(BloodBlade));
            TypeList.Add(typeof(DoubleAxe), typeof(DualShortAxes));
            TypeList.Add(typeof(ExecutionersAxe), typeof(DualShortAxes));
            TypeList.Add(typeof(LargeBattleAxe), typeof(DualShortAxes));
            TypeList.Add(typeof(TwoHandedAxe), typeof(DualShortAxes));
            TypeList.Add(typeof(OrnateAxe), typeof(DualShortAxes));
            TypeList.Add(typeof(WarAxe), typeof(DiscMace));
            TypeList.Add(typeof(HammerPick), typeof(DiscMace));
            TypeList.Add(typeof(Mace), typeof(DiscMace));
            TypeList.Add(typeof(Scepter), typeof(DiscMace));
            TypeList.Add(typeof(WarMace), typeof(DiscMace));
            TypeList.Add(typeof(DiamondMace), typeof(DiscMace));

            //****Carpentry****//
            TypeList.Add(typeof(GnarledStaff), typeof(GargishGnarledStaff));
            #region Need Implementation
            //****Tailoring****//
            //Misc
            /*  TypeList.Add(typeof(BodySash), typeof(GargishSash));
                TypeList.Add(typeof(HalfApron), typeof(GargoyleHalfApron));
                TypeList.Add(typeof(Kilt), typeof(GargishClothKilt));

                //Footwear
                TypeList.Add(typeof(FurBoots), typeof(LeatherTalons));
                TypeList.Add(typeof(Boots), typeof(LeatherTalons));
                TypeList.Add(typeof(ThighBoots), typeof(LeatherTalons));
                TypeList.Add(typeof(Shoes), typeof(LeatherTalons));
                TypeList.Add(typeof(Sandals), typeof(LeatherTalons));
                TypeList.Add(typeof(NinjaTabi), typeof(LeatherTalons));
                TypeList.Add(typeof(SamuraiTabi), typeof(LeatherTalons));
                TypeList.Add(typeof(Waraji), typeof(LeatherTalons));
                TypeList.Add(typeof(ElvenBoots), typeof(LeatherTalons));

                //Quivers & Cloaks
                TypeList.Add(typeof(ElvenQuiver), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(QuiverOfBlight), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(QuiverOfFire), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(QuiverOfIce), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(QuiverOfLightning), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(QuiverOfElements), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(QuiverOfRage), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(QuiverOfInfinity), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(Cloak), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(RewardCloak), typeof(GargishClothWingArmor));
                TypeList.Add(typeof(FurCape), typeof(GargishClothWingArmor)); */
            #endregion

            //Leather Armor
            TypeList.Add(typeof(LeatherBustierArms), typeof(FemaleGargishLeatherChest));
            TypeList.Add(typeof(FemaleLeatherChest), typeof(FemaleGargishLeatherChest));
            TypeList.Add(typeof(LeatherShorts), typeof(GargishLeatherLegs));
            TypeList.Add(typeof(LeatherSkirt), typeof(GargishLeatherLegs));
            TypeList.Add(typeof(LeatherChest), typeof(GargishLeatherChest));
            TypeList.Add(typeof(LeatherLegs), typeof(GargishLeatherLegs));
            TypeList.Add(typeof(LeatherArms), typeof(GargishLeatherArms));
            TypeList.Add(typeof(FemaleLeafChest), typeof(FemaleGargishLeatherChest));
            TypeList.Add(typeof(LeafArms), typeof(GargishLeatherArms));
            TypeList.Add(typeof(LeafChest), typeof(GargishLeatherChest));
            TypeList.Add(typeof(LeafLegs), typeof(GargishLeatherLegs));
            TypeList.Add(typeof(LeafTonlet), typeof(GargishLeatherLegs));
            TypeList.Add(typeof(LeatherDo), typeof(GargishLeatherChest));
            TypeList.Add(typeof(LeatherHaidate), typeof(GargishLeatherLegs));
            TypeList.Add(typeof(LeatherHiroSode), typeof(GargishLeatherArms));
            TypeList.Add(typeof(LeatherSuneate), typeof(GargishLeatherLegs));
            TypeList.Add(typeof(LeatherNinjaJacket), typeof(GargishLeatherChest));
            TypeList.Add(typeof(LeatherNinjaPants), typeof(GargishLeatherLegs));

            //****Tinker****//
            TypeList.Add(typeof(Hatchet), typeof(DualShortAxes));
            TypeList.Add(typeof(ButcherKnife), typeof(GargishButcherKnife));
            TypeList.Add(typeof(Cleaver), typeof(GargishCleaver));
                      
        }

        public static void BeginTarget(Mobile from, CraftSystem system, BaseTool tool)
        {
            from.Target = new AlterItemTarget(system, tool);
            from.SendLocalizedMessage(1094730); //Target the item to altar
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
			olditem.OnAfterDuped(newitem);
			newitem.Parent = null;

            if (from.Backpack == null)
                newitem.MoveToWorld(from.Location, from.Map);
            else
                from.Backpack.DropItem(newitem);
				
			newitem.InvalidateProperties();

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