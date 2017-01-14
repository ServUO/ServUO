// Uncomment the following line if you have XmlMobFactions installed and you would like to allow faction gain through recipes
//#define XMLMOBFACTIONS

using System;
using System.Data;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using Server.Gumps;
using System.Text;
using Server.ContextMenus;
using Server.Engines.BulkOrders;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    public class BoxOfTransmutation : BaseTransmutationContainer
    {

        public enum Recipes
        {
            UpgradeAncientAugment,
            UpgradeLegendaryAugment,
            UpgradeCrystalAugment,
            UndeadFaction,
            AbyssFaction,
            ReptilianFaction,
            ElementalFaction,
            HumanoidFaction,
            ArachnidFaction,
            UnderworldFaction,
            SocketWeapon,
            SocketArmor,
            RecoverAugmentation,
            HammerOfRecovery,
            PowerScrollSkillChange,
            SmallBODChange,
            LargeBODChange,
            ExceptionalSocketHammer
        }

		public override int DefaultMaxWeight
		{
			get
			{
				return 420;
			}
		}

        public static void Initialize()
        {

            //
            // define the recipes and their use requirements
            //
            // ideally, you have a definition for every Recipes enum.  Although it isnt absolutely necessary,
            // if it isnt defined here, it will not be available for use

            AddRecipe(
                (int)Recipes.PowerScrollSkillChange,                                  // combines 3 powerscrolls of the same level to produce a new random ps of the same level
                50,20,50,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Alchemy },                                        //  skill requirement list
                new int [] { 100 },                                                               // minimum skill levels
                new Type [] {typeof(LegendaryDiamond), typeof(LegendaryRuby), typeof(LegendaryEmerald), typeof(PowerScroll)},                     // ingredient list
                new int [] { 1, 1, 1, 3, }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.SmallBODChange,                                  // a small bod and gold to yield a new random small bod
                50,20,50,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Alchemy },                                        //  skill requirement list
                new int [] { 100 },                                                               // minimum skill levels
                new Type [] {typeof(Gold), typeof(SmallBOD)},                     // ingredient list
                new int [] { 2000, 1 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.LargeBODChange,                                  // a large bod and gold to yield a new random large bod
                50,20,50,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Alchemy },                                        //  skill requirement list
                new int [] { 100 },                                                               // minimum skill levels
                new Type [] {typeof(Gold), typeof(LargeBOD)},                     // ingredient list
                new int [] { 20000, 1 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.HammerOfRecovery,                                  // removes an augmentation from an item
                70,30,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Blacksmith },                                        //  skill requirement list
                new int [] { 105 },                                                               // minimum skill levels
                new Type [] {typeof(AncientRuby), typeof(Hammer)},                     // ingredient list
                new int [] { 1, 1, }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.RecoverAugmentation,                                  // removes an augmentation from an item
                70,30,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Blacksmith },                                        //  skill requirement list
                new int [] { 100 },                                                               // minimum skill levels
                new Type [] {typeof(AncientRuby), typeof(Item)},                     // ingredient list
                new int [] { 1, 1, }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.UpgradeAncientAugment,                     // change 6 ancient augments into one legendary
                70,30,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Alchemy },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(IAncientAugment)},                                            // ingredient list
                new int [] { 6 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.UpgradeLegendaryAugment,                     // change 3 legendary augments into one mythic
                70,30,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Alchemy },                                        //  skill requirement list
                new int [] { 70 },                                                               // minimum skill levels
                new Type [] {typeof(ILegendaryAugment)},                                            // ingredient list
                new int [] { 3 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.UpgradeCrystalAugment,                     // change 3 crystal augments into one radiant
                70,30,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Alchemy },                                        //  skill requirement list
                new int [] { 70 },                                                               // minimum skill levels
                new Type [] {typeof(ICrystalAugment)},                                            // ingredient list
                new int [] { 3 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.UndeadFaction,                     // gain undead faction
                20,20,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.AnimalLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(GlimmeringGranite), typeof( Gold) },                                            // ingredient list
                new int [] { 1 , 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.AbyssFaction,                     // gain abyss faction
                20,20,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.AnimalLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(GlimmeringBloodrock), typeof( Gold) },                                            // ingredient list
                new int [] { 1 , 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.ReptilianFaction,                     // gain reptilian faction
                20,20,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.AnimalLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(GlimmeringHeartstone), typeof( Gold) },                                            // ingredient list
                new int [] { 1 , 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.HumanoidFaction,                     // gain humanoid faction
                20,20,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.AnimalLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(GlimmeringClay), typeof( Gold) },                                            // ingredient list
                new int [] { 1 , 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.ArachnidFaction,                     // gain arachnid faction
                20,20,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.AnimalLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(GlimmeringIronOre), typeof( Gold) },                                            // ingredient list
                new int [] { 1 , 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.ElementalFaction,                     // gain elemental faction
                20,20,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.AnimalLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(GlimmeringGypsum), typeof( Gold) },                                            // ingredient list
                new int [] { 1 , 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.UnderworldFaction,                     // gain underworld faction
                20,20,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.AnimalLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(GlimmeringMarble), typeof( Gold) },                                            // ingredient list
                new int [] { 1 , 0 }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.SocketWeapon,                                  // add a socket to any unsocketed weapon
                70,30,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Blacksmith },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(RadiantRhoCrystal), typeof(BaseWeapon)},                     // ingredient list
                new int [] { 1, 1, }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.SocketArmor,                                  // add a socket to any unsocketed armor
                70,30,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Blacksmith },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(RadiantRhoCrystal), typeof(BaseArmor)},                     // ingredient list
                new int [] { 1, 1, }                                                                 // zero indicates any quantity
            );
            AddRecipe(
                (int)Recipes.ExceptionalSocketHammer,                                  // create a single use Exceptionalsockethammer
                70,30,40,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.Blacksmith },                                        //  skill requirement list
                new int [] { 105 },                                                               // minimum skill levels
                new Type [] {typeof(RadiantRysCrystal), typeof(Hammer)},                     // ingredient list
                new int [] { 1, 1, }                                                                 // zero indicates any quantity
            );
        }

        public override void DoTransmute(Mobile from, Recipe r)
        {
            if(r == null || from == null) return;

            Recipes rid = (Recipes) r.RecipeID;
            switch(rid)
            {
                case Recipes.PowerScrollSkillChange:
                {
                    // get the target value from one of the powerscrolls
                    Item [] slist = FindItemsByType(typeof(PowerScroll), true);
                    
                    double value = 0;
                    // make sure they are all of the same value
                    foreach(Item s in slist)
                    {
                        if(value == 0)
                        {
                            value = ((PowerScroll)s).Value;
                        } else
                        {
                            if(value != ((PowerScroll)s).Value)
                            {
                                from.SendMessage("All powerscrolls must be of the same level");
                                return;
                            }
                        }
                    }

                    PowerScroll newps = PowerScroll.CreateRandom( (int)(value - 100), (int)(value - 100) );

                    // consume ingredients
                    ConsumeAll();
                    
                    // add the new powerscroll
                    DropItem(newps);

                    break;
                }
                case Recipes.SmallBODChange:
                {
                    // get the target value from one of the powerscrolls
                    Item [] slist = FindItemsByType(typeof(SmallBOD), true);
                    
                    Type t = null;
                    // make sure they are all of the same type
                    foreach(Item s in slist)
                    {
                        if(t == null)
                        {
                            t = ((SmallBOD)s).GetType();
                        } else
                        {
                            if(t != ((SmallBOD)s).GetType())
                            {
                                from.SendMessage("All BODs must be of the same type");
                                return;
                            }
                        }
                    }
                    
                    SmallBOD newbod = null;

                    if(t == typeof(SmallTailorBOD))
                    {
                    	newbod = new SmallTailorBOD();
                    } else
                    if(t == typeof(SmallSmithBOD))
                    {
                    	newbod = new SmallSmithBOD();
                    } else
                    {
                    	from.SendMessage("Cannot transmute those BODs");
                    	return;
                    }

					if(newbod == null) return;

                    // consume ingredients
                    ConsumeAll();
                    
                    // add the new powerscroll
                    DropItem(newbod);

                    break;
                }
                case Recipes.LargeBODChange:
                {
                    // get the target value from one of the powerscrolls
                    Item [] slist = FindItemsByType(typeof(LargeBOD), true);
                    
                    Type t = null;
                    // make sure they are all of the same type
                    foreach(Item s in slist)
                    {
                        if(t == null)
                        {
                            t = ((LargeBOD)s).GetType();
                        } else
                        {
                            if(t != ((LargeBOD)s).GetType())
                            {
                                from.SendMessage("All BODs must be of the same type");
                                return;
                            }
                        }
                    }
                    
                    LargeBOD newbod = null;

                    if(t == typeof(LargeTailorBOD))
                    {
                    	newbod = new LargeTailorBOD();
                    } else
                    if(t == typeof(LargeSmithBOD))
                    {
                    	newbod = new LargeSmithBOD();
                    } else
                    {
                    	from.SendMessage("Cannot transmute those BODs");
                    	return;
                    }

					if(newbod == null) return;

                    // consume ingredients
                    ConsumeAll();
                    
                    // add the new powerscroll
                    DropItem(newbod);

                    break;
                }
                case Recipes.UpgradeAncientAugment:
                {
                    // check what type of augment is being upgraded
                    BaseSocketAugmentation augment = null;
                    if(Items.Count > 0)
                    {
                        augment = Items[0] as BaseSocketAugmentation;
                    }

                    if(augment == null) return;

                    // make sure they are all the same
                    foreach(Item i in Items)
                    {
                        if(augment.GetType() != i.GetType()) return;
                    }

                    // take the ingredients
                    ConsumeAll();
                    // and add the result
                    if(augment is AncientDiamond)
                    {
                        DropItem(new LegendaryDiamond() );
                    } else
                    if(augment is AncientSapphire)
                    {
                        DropItem(new LegendarySapphire() );
                    } else
                    if(augment is AncientSkull)
                    {
                        DropItem(new LegendarySkull() );
                    } else
                    if(augment is AncientTourmaline)
                    {
                        DropItem(new LegendaryTourmaline() );
                    } else
                    if(augment is AncientWood)
                    {
                        DropItem(new LegendaryWood() );
                    } else
                    if(augment is AncientRuby)
                    {
                        DropItem(new LegendaryRuby() );
                    } else
                    if(augment is AncientEmerald)
                    {
                        DropItem(new LegendaryEmerald() );
                    }
					else
						if(augment is AncientAmethyst)
					{
						DropItem(new LegendaryAmethyst() );
					}

                    break;
                }
                case Recipes.UpgradeLegendaryAugment:
                {
                    // check what type of augment is being upgraded
                    BaseSocketAugmentation augment = null;
                    if(Items.Count > 0)
                    {
                        augment = Items[0] as BaseSocketAugmentation;
                    }

                    if(augment == null) return;

                    // make sure they are all the same
                    foreach(Item i in Items)
                    {
                        if(augment.GetType() != i.GetType()) return;
                    }

                    // take the ingredients
                    ConsumeAll();

                    // and add the result
                    if(augment is LegendaryDiamond)
                    {
                        DropItem(new MythicDiamond() );
                    } else
                    if(augment is LegendarySapphire)
                    {
                        DropItem(new MythicSapphire() );
                    } else
                    if(augment is LegendarySkull)
                    {
                        DropItem(new MythicSkull() );
                    } else
                    if(augment is LegendaryTourmaline)
                    {
                        DropItem(new MythicTourmaline() );
                    } else
                    if(augment is LegendaryWood)
                    {
                        DropItem(new MythicWood() );
                    } else
                    if(augment is LegendaryRuby)
                    {
                        DropItem(new MythicRuby() );
                    } else
                    if(augment is LegendaryEmerald)
                    {
                        DropItem(new MythicEmerald() );
                    }
					else
						if(augment is LegendaryAmethyst)
					{
						DropItem(new MythicAmethyst() );
					}

                    break;
                }
                case Recipes.UpgradeCrystalAugment:
                {
                    // check what type of augment is being upgraded
                    BaseSocketAugmentation augment = null;
                    if(Items.Count > 0)
                    {
                        augment = Items[0] as BaseSocketAugmentation;
                    }
                    
                    if(augment == null) return;
                    
                    // make sure they are all the same
                    foreach(Item i in Items)
                    {
                        if(augment.GetType() != i.GetType()) return;
                    }

                    // take the ingredients
                    ConsumeAll();

                    // and add the result
                    if(augment is RhoCrystal)
                    {
                        DropItem(new RadiantRhoCrystal() );
                    } else
                    if(augment is RysCrystal)
                    {
                        DropItem(new RadiantRysCrystal() );
                    } else
                    if(augment is WyrCrystal)
                    {
                        DropItem(new RadiantWyrCrystal() );
                    } else
                    if(augment is FreCrystal)
                    {
                        DropItem(new RadiantFreCrystal() );
                    } else
                    if(augment is TorCrystal)
                    {
                        DropItem(new RadiantTorCrystal() );
                    } else
                    if(augment is VelCrystal)
                    {
                        DropItem(new RadiantVelCrystal() );
                    } else
                    if(augment is XenCrystal)
                    {
                        DropItem(new RadiantXenCrystal() );
                    } else
                    if(augment is PolCrystal)
                    {
                        DropItem(new RadiantPolCrystal() );
                    } else
                    if(augment is WolCrystal)
                    {
                        DropItem(new RadiantWolCrystal() );
                    } else
                    if(augment is BalCrystal)
                    {
                        DropItem(new RadiantBalCrystal() );
                    } else
                    if(augment is TalCrystal)
                    {
                        DropItem(new RadiantTalCrystal() );
                    } else
                    if(augment is JalCrystal)
                    {
                        DropItem(new RadiantJalCrystal() );
                    } else
                    if(augment is RalCrystal)
                    {
                        DropItem(new RadiantRalCrystal() );
                    } else
                    if(augment is KalCrystal)
                    {
                        DropItem(new RadiantKalCrystal() );
                    }

                    break;

                }
                case Recipes.RecoverAugmentation:
                {
                    // does item have any sockets on it?
                    Item b = FindItemByType(typeof(BaseArmor), true);
                    if(b == null)
                        b = FindItemByType(typeof(BaseWeapon), true);
                    if(b == null)
                        b = FindItemByType(typeof(BaseJewel), true);

                    XmlSockets a = XmlAttach.FindAttachment(b, typeof(XmlSockets)) as XmlSockets;
                    if(a == null)
                    {
                        // if so then forget it
                        from.SendMessage("This item is not socketed.");
                        return;
                    }


                    if(a.NSockets == 0)
                    {
                        from.SendMessage("This item is has no sockets.");
                        return;
                    }

                    BaseSocketAugmentation augment = a.RecoverRandomAugmentation(from, b);

                    if(augment != null)
                    {
                        // consume the crystal
                        ConsumeTotal(typeof(AncientRuby), 1, true);

                        // put the recovered augment in the container
                        DropItem(augment);

                        from.SendMessage("Recovered a {0} augmentation.", augment.Name);
                        
                        // update the sockets gump
                        a.OnIdentify(from);

                    } else
                    {
                        from.SendMessage("Failed to recover augmentation.");
                    }


                    break;
                }
                case Recipes.HammerOfRecovery:
                {
                    // consume ingredients
                    ConsumeAll();
                    
                    // add the hammer
                    DropItem(new HammerOfRecovery(1));

                    break;
                }
                
                case Recipes.ExceptionalSocketHammer:
                {
                    // consume ingredients
                    ConsumeAll();
                    
                    // add the hammer
                    DropItem(new ExceptionalSocketHammer(1));

                    break;
                }
                
                case Recipes.SocketWeapon:
                {
                    // does the weapon already have any sockets on it?
                    Item b = FindItemByType(typeof(BaseWeapon), true);
                    XmlAttachment a = XmlAttach.FindAttachment(b, typeof(XmlSockets));
                    if(a != null)
                    {
                        // if so then forget it
                        from.SendMessage("Weapon already socketed.");
                        return;
                    }
                    // otherwise add the socket
                    XmlAttach.AttachTo(b, new XmlSockets(1));
                    // consume the crystal
                    ConsumeTotal(typeof(RadiantRhoCrystal), 1, true);
                    break;
                }
                case Recipes.SocketArmor:
                {
                    // does the armor already have any sockets on it?
                    Item b = FindItemByType(typeof(BaseArmor), true);
                    XmlAttachment a = XmlAttach.FindAttachment(b, typeof(XmlSockets));
                    if(a != null)
                    {
                        // if so then forget it
                        from.SendMessage("Armor already socketed.");
                        return;
                    }
                    // otherwise add the socket
                    XmlAttach.AttachTo(b, new XmlSockets(1));
                    // consume the crystal
                    ConsumeTotal(typeof(RadiantRhoCrystal), 1, true);
                    break;
                }
// Uncomment the follow recipes if you have XmlMobFactions installed and you would like to allow faction gain through these recipes

#if(XMLMOBFACTIONS)
                case Recipes.UndeadFaction:
                {
                    // how much gold is being offered
                    Item b = FindItemByType(typeof(Gold), true);

                    int value = 0;
                    if(b != null)
                    {
                        value = b.Amount/10;
                    }

                    // take the ingredients
                    ConsumeAll();

                    // add the faction
                    XmlAttach.AttachTo(from, new XmlAddFaction("Undead", value));
                    break;
                }
                case Recipes.AbyssFaction:
                {
                    // how much gold is being offered
                    Item b = FindItemByType(typeof(Gold), true);

                    int value = 0;
                    if(b != null)
                    {
                        value = b.Amount/10;
                    }

                    // take the ingredients
                    ConsumeAll();

                    // add the faction
                    XmlAttach.AttachTo(from, new XmlAddFaction("Abyss", value));
                    break;
                }
                case Recipes.HumanoidFaction:
                {
                    // how much gold is being offered
                    Item b = FindItemByType(typeof(Gold), true);

                    int value = 0;
                    if(b != null)
                    {
                        value = b.Amount/10;
                    }

                    // take the ingredients
                    ConsumeAll();

                    // add the faction
                    XmlAttach.AttachTo(from, new XmlAddFaction("Humanoid", value));
                    break;
                }
                case Recipes.ReptilianFaction:
                {
                    // how much gold is being offered
                    Item b = FindItemByType(typeof(Gold), true);

                    int value = 0;
                    if(b != null)
                    {
                        value = b.Amount/10;
                    }

                    // take the ingredients
                    ConsumeAll();
                    
                    // add the faction
                    XmlAttach.AttachTo(from, new XmlAddFaction("Reptilian", value));
                    break;
                }
                case Recipes.ArachnidFaction:
                {
                    // how much gold is being offered
                    Item b = FindItemByType(typeof(Gold), true);

                    int value = 0;
                    if(b != null)
                    {
                        value = b.Amount/10;
                    }

                    // take the ingredients
                    ConsumeAll();

                    // add the faction
                    XmlAttach.AttachTo(from, new XmlAddFaction("Arachnid", value));
                    break;
                }
                case Recipes.ElementalFaction:
                {
                    // how much gold is being offered
                    Item b = FindItemByType(typeof(Gold), true);

                    int value = 0;
                    if(b != null)
                    {
                        value = b.Amount/10;
                    }

                    // take the ingredients
                    ConsumeAll();
                    
                    // add the faction
                    XmlAttach.AttachTo(from, new XmlAddFaction("Elemental", value));
                    break;
                }
                case Recipes.UnderworldFaction:
                {
                    // how much gold is being offered
                    Item b = FindItemByType(typeof(Gold), true);

                    int value = 0;
                    if(b != null)
                    {
                        value = b.Amount/10;
                    }

                    // take the ingredients
                    ConsumeAll();
                    
                    // add the faction
                    XmlAttach.AttachTo(from, new XmlAddFaction("Underworld", value));
                    break;
                }

// end of commented-out XmlMobFactions recipes
#endif
            }

            // give effects for successful transmutation
            from.PlaySound(503);
            
            base.DoTransmute(from, r);
        }

        [Constructable]
		public BoxOfTransmutation() : this(-1)
		{
		}
		
		[Constructable]
		public BoxOfTransmutation(int nuses) : base(0xE80)
		{
            Name = "Box of Transmutation";
            UsesRemaining = nuses;
		}

		public BoxOfTransmutation( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

    }
}
