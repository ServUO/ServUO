using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Harvest
{
    public abstract class HarvestSystem
    {
        public static void Configure()
        {
            EventSink.TargetByResourceMacro += TargetByResource;
        }

        public HarvestSystem()
        {
            Definitions = new List<HarvestDefinition>();
        }

        public List<HarvestDefinition> Definitions { get; }

        public virtual bool CheckTool(Mobile from, Item tool)
        {
            bool wornOut = tool == null || tool.Deleted || (tool is IUsesRemaining && ((IUsesRemaining)tool).UsesRemaining <= 0);

            if (wornOut)
                from.SendLocalizedMessage(1044038); // You have worn out your tool!

            return !wornOut;
        }

        public virtual bool CheckHarvest(Mobile from, Item tool)
        {
            return CheckTool(from, tool);
        }

        public virtual bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            return CheckTool(from, tool);
        }

        public virtual bool CheckRange(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            bool inRange = from.Map == map && from.InRange(loc, def.MaxRange);

            if (!inRange)
                def.SendMessageTo(from, timed ? def.TimedOutOfRangeMessage : def.OutOfRangeMessage);

            return inRange;
        }

        public virtual bool CheckResources(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);
            bool available = bank != null && bank.Current >= def.ConsumedPerHarvest;

            if (!available)
                def.SendMessageTo(from, timed ? def.DoubleHarvestMessage : def.NoResourcesMessage);

            return available;
        }

        public virtual void OnBadHarvestTarget(Mobile from, Item tool, object toHarvest)
        {
        }

        public virtual object GetLock(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            /* Here we prevent multiple harvesting.
            * 
            * Some options:
            *  - 'return tool;' : This will allow the player to harvest more than once concurrently, but only if they use multiple tools. This seems to be as OSI.
            *  - 'return GetType();' : This will disallow multiple harvesting of the same type. That is, we couldn't mine more than once concurrently, but we could be both mining and lumberjacking.
            *  - 'return typeof( HarvestSystem );' : This will completely restrict concurrent harvesting.
            */
            return tool;
        }

        public virtual void OnConcurrentHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
        }

        public virtual void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
        }

        public virtual bool BeginHarvesting(Mobile from, Item tool)
        {
            if (!CheckHarvest(from, tool))
                return false;

            EventSink.InvokeResourceHarvestAttempt(new ResourceHarvestAttemptEventArgs(from, tool, this));
            from.Target = new HarvestTarget(tool, this);
            return true;
        }

        public virtual void FinishHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked)
        {
            from.EndAction(locked);

            if (!CheckHarvest(from, tool))
                return;

            if (!GetHarvestDetails(from, tool, toHarvest, out int tileID, out Map map, out Point3D loc))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            if (!def.Validate(tileID) && !def.ValidateSpecial(tileID))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            if (!CheckRange(from, tool, def, map, loc, true))
                return;
            if (!CheckResources(from, tool, def, map, loc, true))
                return;
            if (!CheckHarvest(from, tool, def, toHarvest))
                return;

            if (SpecialHarvest(from, tool, def, map, loc))
                return;

            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

            if (bank == null)
                return;

            HarvestVein vein = bank.Vein;

            if (vein != null)
                vein = MutateVein(from, tool, def, bank, toHarvest, vein);

            if (vein == null)
                return;

            HarvestResource primary = vein.PrimaryResource;
            HarvestResource fallback = vein.FallbackResource;
            HarvestResource resource = MutateResource(from, tool, def, map, loc, vein, primary, fallback);

            double skillBase = from.Skills[def.Skill].Base;

            Type type = null;

            if (CheckHarvestSkill(map, loc, from, resource, def))
            {
                type = GetResourceType(from, tool, def, map, loc, resource);

                if (type != null)
                    type = MutateType(type, from, tool, def, map, loc, resource);

                if (type != null)
                {
                    Item item = Construct(type, from, tool);

                    if (item == null)
                    {
                        type = null;
                    }
                    else
                    {
                        int amount = def.ConsumedPerHarvest;
                        int feluccaAmount = def.ConsumedPerFeluccaHarvest;

                        if (item is BaseGranite)
                            feluccaAmount = 3;

                        Caddellite.OnHarvest(from, tool, this, item);

                        //The whole harvest system is kludgy and I'm sure this is just adding to it.
                        if (item.Stackable)
                        {
                            int racialAmount = (int)Math.Ceiling(amount * 1.1);
                            int feluccaRacialAmount = (int)Math.Ceiling(feluccaAmount * 1.1);

                            bool eligableForRacialBonus = (def.RaceBonus && from.Race == Race.Human);
                            bool inFelucca = map == Map.Felucca && !Siege.SiegeShard;

                            if (eligableForRacialBonus && inFelucca && bank.Current >= feluccaRacialAmount && 0.1 > Utility.RandomDouble())
                                item.Amount = feluccaRacialAmount;
                            else if (inFelucca && bank.Current >= feluccaAmount)
                                item.Amount = feluccaAmount;
                            else if (eligableForRacialBonus && bank.Current >= racialAmount && 0.1 > Utility.RandomDouble())
                                item.Amount = racialAmount;
                            else
                                item.Amount = amount;

                            // Void Pool Rewards
                            item.Amount += WoodsmansTalisman.CheckHarvest(from, type, this);
                        }

                        if (from.AccessLevel == AccessLevel.Player)
                        {
                            bank.Consume(amount, from);
                        }

                        if (Give(from, item, def.PlaceAtFeetIfFull))
                        {
                            SendSuccessTo(from, item, resource);
                        }
                        else
                        {
                            SendPackFullTo(from, item, def, resource);
                            item.Delete();
                        }

                        BonusHarvestResource bonus = def.GetBonusResource();
                        Item bonusItem = null;

                        if (bonus != null && bonus.Type != null && skillBase >= bonus.ReqSkill)
                        {
                            if (bonus.RequiredMap == null || bonus.RequiredMap == from.Map)
                            {
                                bonusItem = Construct(bonus.Type, from, tool);
                                Caddellite.OnHarvest(from, tool, this, bonusItem);

                                if (Give(from, bonusItem, true))    //Bonuses always allow placing at feet, even if pack is full irregrdless of def
                                {
                                    bonus.SendSuccessTo(from);
                                }
                                else
                                {
                                    bonusItem.Delete();
                                }
                            }
                        }

                        EventSink.InvokeResourceHarvestSuccess(new ResourceHarvestSuccessEventArgs(from, tool, item, bonusItem, this));
                    }

                    #region High Seas
                    OnToolUsed(from, tool, item != null);
                    #endregion
                }

                // Siege rules will take into account axes and polearms used for lumberjacking
                if (tool is IUsesRemaining && (tool is BaseHarvestTool || tool is Pickaxe || tool is SturdyPickaxe || tool is GargoylesPickaxe || Siege.SiegeShard))
                {
                    IUsesRemaining toolWithUses = (IUsesRemaining)tool;

                    toolWithUses.ShowUsesRemaining = true;

                    if (toolWithUses.UsesRemaining > 0)
                        --toolWithUses.UsesRemaining;

                    if (toolWithUses.UsesRemaining < 1)
                    {
                        tool.Delete();
                        def.SendMessageTo(from, def.ToolBrokeMessage);
                    }
                }
            }

            if (type == null)
                def.SendMessageTo(from, def.FailMessage);

            OnHarvestFinished(from, tool, def, vein, bank, resource, toHarvest);
        }

        public virtual bool CheckHarvestSkill(Map map, Point3D loc, Mobile from, HarvestResource resource, HarvestDefinition def)
        {
            return from.Skills[def.Skill].Value >= resource.ReqSkill && from.CheckSkill(def.Skill, resource.MinSkill, resource.MaxSkill);
        }

        public virtual void OnToolUsed(Mobile from, Item tool, bool caughtSomething)
        {
        }

        public virtual void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
        }

        public virtual bool SpecialHarvest(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
        {
            return false;
        }

        public virtual Item Construct(Type type, Mobile from, Item tool)
        {
            try
            {
                return Activator.CreateInstance(type) as Item;
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
                return null;
            }
        }

        public virtual HarvestVein MutateVein(Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
        {
            return vein;
        }

        public virtual void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
        {
            resource.SendSuccessTo(from);
        }

        public virtual void SendPackFullTo(Mobile from, Item item, HarvestDefinition def, HarvestResource resource)
        {
            def.SendMessageTo(from, def.PackFullMessage);
        }

        public virtual bool Give(Mobile m, Item item, bool placeAtFeet)
        {
            if (m.PlaceInBackpack(item))
                return true;

            if (!placeAtFeet)
                return false;

            Map map = m.Map;

            if (map == null || map == Map.Internal)
                return false;

            List<Item> atFeet = new List<Item>();

            IPooledEnumerable eable = m.GetItemsInRange(0);

            foreach (Item obj in eable)
                atFeet.Add(obj);

            eable.Free();

            for (int i = 0; i < atFeet.Count; ++i)
            {
                Item check = atFeet[i];

                if (check.StackWith(m, item, false))
                    return true;
            }

            ColUtility.Free(atFeet);

            item.MoveToWorld(m.Location, map);
            return true;
        }

        public virtual Type MutateType(Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            return from.Region.GetResource(type);
        }

        public virtual Type GetResourceType(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            if (resource.Types.Length > 0)
                return resource.Types[Utility.Random(resource.Types.Length)];

            return null;
        }

        public virtual HarvestResource MutateResource(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestVein vein, HarvestResource primary, HarvestResource fallback)
        {
            bool racialBonus = def.RaceBonus && from.Race == Race.Elf;

            if (vein.ChanceToFallback > (Utility.RandomDouble() + (racialBonus ? .20 : 0)))
                return fallback;

            double skillValue = from.Skills[def.Skill].Value;

            if (fallback != null && (skillValue < primary.ReqSkill || skillValue < primary.MinSkill))
                return fallback;

            return primary;
        }

        public virtual bool OnHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked, bool last)
        {
            if (!CheckHarvest(from, tool))
            {
                from.EndAction(locked);
                return false;
            }

            if (!GetHarvestDetails(from, tool, toHarvest, out int tileID, out Map map, out Point3D loc))
            {
                from.EndAction(locked);
                OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }

            if (!def.Validate(tileID) && !def.ValidateSpecial(tileID))
            {
                from.EndAction(locked);
                OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }

            if (!CheckRange(from, tool, def, map, loc, true))
            {
                from.EndAction(locked);
                return false;
            }

            if (!CheckResources(from, tool, def, map, loc, true))
            {
                from.EndAction(locked);
                return false;
            }

            if (!CheckHarvest(from, tool, def, toHarvest))
            {
                from.EndAction(locked);
                return false;
            }

            DoHarvestingEffect(from, tool, def, map, loc);

            new HarvestSoundTimer(from, tool, this, def, toHarvest, locked, last).Start();

            return !last;
        }

        public virtual void DoHarvestingSound(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            if (def.EffectSounds.Length > 0)
                from.PlaySound(Utility.RandomList(def.EffectSounds));
        }

        public virtual void DoHarvestingEffect(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
        {
            from.Direction = from.GetDirectionTo(loc);

            if (!from.Mounted)
            {
                from.Animate(AnimationType.Attack, Utility.RandomList(def.EffectActions));
            }
        }

        public virtual HarvestDefinition GetDefinition(int tileID)
        {
            return GetDefinition(tileID, null);
        }

        public virtual HarvestDefinition GetDefinition(int tileID, Item tool)
        {
            HarvestDefinition def = null;

            for (int i = 0; def == null && i < Definitions.Count; ++i)
            {
                HarvestDefinition check = Definitions[i];

                if (check.Validate(tileID))
                    def = check;
            }

            return def;
        }

        #region High Seas
        public virtual HarvestDefinition GetDefinitionFromSpecialTile(int tileID)
        {
            HarvestDefinition def = null;

            for (int i = 0; def == null && i < Definitions.Count; ++i)
            {
                HarvestDefinition check = Definitions[i];

                if (check.ValidateSpecial(tileID))
                    def = check;
            }

            return def;
        }
        #endregion

        public virtual void StartHarvesting(Mobile from, Item tool, object toHarvest)
        {
            if (!CheckHarvest(from, tool))
                return;

            if (!GetHarvestDetails(from, tool, toHarvest, out int tileID, out Map map, out Point3D loc))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            HarvestDefinition def = GetDefinition(tileID, tool);

            if (def == null)
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            if (!CheckRange(from, tool, def, map, loc, false))
                return;
            if (!CheckResources(from, tool, def, map, loc, false))
                return;
            if (!CheckHarvest(from, tool, def, toHarvest))
                return;

            object toLock = GetLock(from, tool, def, toHarvest);

            if (!from.BeginAction(toLock))
            {
                OnConcurrentHarvest(from, tool, def, toHarvest);
                return;
            }

            new HarvestTimer(from, tool, this, def, toHarvest, toLock).Start();
            OnHarvestStarted(from, tool, def, toHarvest);
        }

        public virtual bool GetHarvestDetails(Mobile from, Item tool, object toHarvest, out int tileID, out Map map, out Point3D loc)
        {
            if (toHarvest is Static && !((Static)toHarvest).Movable)
            {
                Static obj = (Static)toHarvest;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = obj.Map;
                loc = obj.GetWorldLocation();
            }
            else if (toHarvest is StaticTarget)
            {
                StaticTarget obj = (StaticTarget)toHarvest;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = from.Map;
                loc = obj.Location;
            }
            else if (toHarvest is LandTarget obj)
            {
                tileID = obj.TileID;
                map = from.Map;
                loc = obj.Location;
            }
            else
            {
                tileID = 0;
                map = null;
                loc = Point3D.Zero;
                return false;
            }

            return map != null && map != Map.Internal;
        }

        #region Enhanced Client
        public static void TargetByResource(TargetByResourceMacroEventArgs e)
        {
            Mobile m = e.Mobile;
            Item tool = e.Tool;

            HarvestSystem system = null;
            HarvestDefinition def = null;

            if (tool is IHarvestTool)
            {
                system = ((IHarvestTool)tool).HarvestSystem;
            }

            if (system != null)
            {
                switch (e.ResourceType)
                {
                    case 0: // ore
                        if (system is Mining)
                            def = ((Mining)system).OreAndStone;
                        break;
                    case 1: // sand
                        if (system is Mining)
                            def = ((Mining)system).Sand;
                        break;
                    case 2: // wood
                        if (system is Lumberjacking)
                            def = ((Lumberjacking)system).Definition;
                        break;
                    case 3: // grave
                        if (TryHarvestGrave(m))
                            return;
                        break;
                    case 4: // red shrooms
                        if (TryHarvestShrooms(m))
                            return;
                        break;
                }

                if (def != null && FindValidTile(m, def, out object toHarvest))
                {
                    system.StartHarvesting(m, tool, toHarvest);
                    return;
                }

                system.OnBadHarvestTarget(m, tool, new LandTarget(new Point3D(0, 0, 0), Map.Felucca));
            }
        }

        private static bool FindValidTile(Mobile m, HarvestDefinition definition, out object toHarvest)
        {
            Map map = m.Map;
            toHarvest = null;

            if (map == null || map == Map.Internal)
                return false;

            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, false);

                    if (tiles.Length > 0)
                    {
                        foreach (StaticTile tile in tiles)
                        {
                            int id = (tile.ID & 0x3FFF) | 0x4000;

                            if (definition.Validate(id))
                            {
                                toHarvest = new StaticTarget(new Point3D(x, y, tile.Z), tile.ID);
                                return true;
                            }
                        }
                    }

                    LandTile lt = map.Tiles.GetLandTile(x, y);

                    if (definition.Validate(lt.ID))
                    {
                        toHarvest = new LandTarget(new Point3D(x, y, lt.Z), map);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryHarvestGrave(Mobile m)
        {
            Map map = m.Map;

            if (map == null)
                return false;

            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, false);

                    foreach (StaticTile tile in tiles)
                    {
                        int itemID = tile.ID;

                        if (itemID == 0xED3 || itemID == 0xEDF || itemID == 0xEE0 || itemID == 0xEE1 || itemID == 0xEE2 || itemID == 0xEE8)
                        {
                            if (m is PlayerMobile player)
                            {
                                QuestSystem qs = player.Quest;

                                if (qs is WitchApprenticeQuest)
                                {
                                    if (qs.FindObjective(typeof(FindIngredientObjective)) is FindIngredientObjective obj && !obj.Completed && obj.Ingredient == Ingredient.Bones)
                                    {
                                        player.SendLocalizedMessage(1055037); // You finish your grim work, finding some of the specific bones listed in the Hag's recipe.
                                        obj.Complete();

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool TryHarvestShrooms(Mobile m)
        {
            Map map = m.Map;

            if (map == null)
                return false;

            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, false);

                    foreach (StaticTile tile in tiles)
                    {
                        int itemID = tile.ID;

                        if (itemID == 0xD15 || itemID == 0xD16)
                        {
                            if (m is PlayerMobile player)
                            {
                                QuestSystem qs = player.Quest;

                                if (qs is WitchApprenticeQuest)
                                {
                                    if (qs.FindObjective(typeof(FindIngredientObjective)) is FindIngredientObjective obj && !obj.Completed && obj.Ingredient == Ingredient.RedMushrooms)
                                    {
                                        player.SendLocalizedMessage(1055036); // You slice a red cap mushroom from its stem.
                                        obj.Complete();

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        #endregion
    }
}

namespace Server
{
    public interface IChopable
    {
        void OnChop(Mobile from);
    }

    public interface IHarvestTool : IEntity
    {
        Engines.Harvest.HarvestSystem HarvestSystem { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FurnitureAttribute : Attribute
    {
        private static bool IsNotChoppables(Item item)
        {
            return _NotChoppables.Any(t => t == item.GetType());
        }

        private static readonly Type[] _NotChoppables =
        {
            typeof(CommodityDeedBox), typeof(ChinaCabinet), typeof(PieSafe), typeof(AcademicBookCase), typeof(JewelryBox),
            typeof(WoodenBookcase), typeof(Countertop), typeof(Mailbox), typeof(DecorativeMagesCrystalBall), typeof(DecorativeMageThrone),
            typeof(DecorativeMagicBookStand), typeof(DecorativeSpecimenShelve), typeof(Feedbag), typeof(ChestOfDrawers), typeof(BarrelMailbox),
            typeof(DolphinMailbox), typeof(ScarecrowMailbox), typeof(SquirrelMailbox), typeof(FootedChestOfDrawers), typeof(CustomizableRoundedDoorMat),
            typeof(CowStatue), typeof(DecorativeStableFencing), typeof(OrnateBedDeed), typeof(FourPostBedDeed), typeof(FormalDiningTableDeed)
        };

        public static bool Check(Item item)
        {
            if (item == null)
            {
                return false;
            }

            if (IsNotChoppables(item))
            {
                return false;
            }

            if (item.GetType().IsDefined(typeof(FurnitureAttribute), false))
            {
                return true;
            }

            if ((item as AddonComponent)?.Addon != null && ((AddonComponent)item).Addon.GetType().IsDefined(typeof(FurnitureAttribute), false))
            {
                return true;
            }

            return false;
        }
    }
}
