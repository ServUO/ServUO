using System;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Engines.Harvest;
using Server.Regions;

namespace Server.Mobiles
{
    [CorpseName("an iron beetle corpse")]
    public class IronBeetle : BaseCreature
    {
        [Constructable]
        public IronBeetle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.25, 0.5)
        {
            Name = "an iron beetle";
            Body = 714;
            BaseSoundID = 397;

            SetStr(816, 883);
            SetDex(68, 73);
            SetInt(40, 49);

            SetHits(762, 830);

            SetDamage(15, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 60);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.Anatomy, 80.1, 85.0);
            SetSkill(SkillName.MagicResist, 125.1, 130.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 110.0);
            SetSkill(SkillName.Mining, 50.1, 70.0);

            Skills.Mining.Cap = 120;

            Fame = 15000;
            Karma = -15000;

            Tamable = true;
            MinTameSkill = 71.1;
            ControlSlots = 4;

            VirtualArmor = 38;

            m_MiningTimer = Timer.DelayCall(MiningInterval, MiningInterval, DoMining);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Gems);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Controlled)
                return;

            if (!Controlled && Utility.RandomDouble() < 0.03)
                c.DropItem(new LuckyCoin());

            if (!Controlled && Utility.RandomDouble() < 0.1)
                c.DropItem(new UndamagedIronBeetleScale());
        }

        public override bool SubdueBeforeTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override bool OverrideBondingReqs() { return true; }

        public override double GetControlChance(Mobile m, bool useBaseSkill) 
        {
            if (PetTrainingHelper.Enabled)
            {
                var profile = PetTrainingHelper.GetAbilityProfile(this);

                if (profile != null && profile.HasCustomized())
                {
                    return base.GetControlChance(m, useBaseSkill);
                }
            }

            return 1.0; 
        }

        public override int GetAngerSound() { return 0x21D; }
        public override int GetIdleSound() { return 0x21D; }
        public override int GetAttackSound() { return 0x162; }
        public override int GetHurtSound() { return 0x163; }
        public override int GetDeathSound() { return 0x21D; }

        #region Mining
        private static readonly TimeSpan MiningInterval = TimeSpan.FromSeconds(5.0);

        private Timer m_MiningTimer;
        private DateTime m_NextOreEat;

        private void GetMiningOffset(Direction d, ref int x, ref int y)
        {
            switch (d & Direction.Mask)
            {
                case Direction.North: --y; break;
                case Direction.South: ++y; break;
                case Direction.West: --x; break;
                case Direction.East: ++x; break;
                case Direction.Right: ++x; --y; break;
                case Direction.Left: --x; ++y; break;
                case Direction.Down: ++x; ++y; break;
                case Direction.Up: --x; --y; break;
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Owners.Count > 0 || m_NextOreEat > DateTime.UtcNow)
                return;

            m_NextOreEat = DateTime.UtcNow + TimeSpan.FromSeconds(3.0);

            if (0.5 > Utility.RandomDouble())
            {
                foreach (Item item in Map.GetItemsInRange(Location, 1))
                {
                    if (item is BaseOre)
                    {
                        // Epic coolness: turn to the ore hue!
                        Hue = item.Hue;

                        item.Delete();

                        return;
                    }
                }
            }
        }

        public void DoMining()
        {
            if (Map == null || Map == Map.Internal)
                return;

            // We may not mine while we are fighting
            if (Combatant != null)
                return;

            HarvestSystem system = Mining.System;
            HarvestDefinition def = Mining.System.OreAndStone;

            // Our target is the land tile under us
            Map map = Map;
            Point3D loc = Location;
            int x = 0, y = 0;
            GetMiningOffset(Direction, ref x, ref y);
            loc.X += x;
            loc.Y += y;
            int tileId = map.Tiles.GetLandTile(loc.X, loc.Y).ID & 0x3FFF;

            if (!def.Validate(tileId))
                return;

            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

            if (bank == null || bank.Current < def.ConsumedPerHarvest)
                return;

            HarvestVein vein = bank.Vein;

            if (vein == null)
                return;

            HarvestResource primary = vein.PrimaryResource;
            HarvestResource fallback = def.Resources[0];

            HarvestResource resource = system.MutateResource(this, null, def, map, loc, vein, primary, fallback);

            double skillBase = Skills[def.Skill].Base;

            Type type = null;

            if (skillBase >= resource.ReqSkill && CheckSkill(def.Skill, resource.MinSkill, resource.MaxSkill))
            {
                type = system.GetResourceType(this, null, def, map, loc, resource);

                if (type != null)
                    type = system.MutateType(type, this, null, def, map, loc, resource);

                if (type != null)
                {
                    Item item = system.Construct(type, this, null);

                    if (item == null)
                    {
                        type = null;
                    }
                    else
                    {
                        if (item.Stackable)
                        {
                            int amount = def.ConsumedPerHarvest;
                            int feluccaAmount = def.ConsumedPerFeluccaHarvest;

                            bool inFelucca = (map == Map.Felucca);

                            if (inFelucca)
                                item.Amount = feluccaAmount;
                            else
                                item.Amount = amount;
                        }

                        bank.Consume(item.Amount, this);

                        item.MoveToWorld(loc, map);

                        system.DoHarvestingEffect(this, null, def, map, loc);
                        system.DoHarvestingSound(this, null, def, null);

                        // Mine for gems
                        BonusHarvestResource bonus = def.GetBonusResource();

                        if (bonus != null && bonus.Type != null && skillBase >= bonus.ReqSkill)
                        {
                            Item bonusItem = system.Construct(bonus.Type, this, null);

                            bonusItem.MoveToWorld(loc, map);
                        }
                    }
                }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (Controlled && ControlMaster == from)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm == null)
                    return;

                ContextMenuEntry miningEntry = new ContextMenuEntry(pm.ToggleMiningStone ? 6179 : 6178);
                miningEntry.Color = 0x421F;
                list.Add(miningEntry);

                list.Add(new BaseHarvestTool.ToggleMiningStoneEntry(pm, false, false, 6176));
                list.Add(new BaseHarvestTool.ToggleMiningStoneEntry(pm, true, false, 6177));
            }
        }
        #endregion        

        public IronBeetle(Serial serial)
            : base(serial)
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

            m_MiningTimer = Timer.DelayCall(MiningInterval, MiningInterval, DoMining);
        }
    }
}