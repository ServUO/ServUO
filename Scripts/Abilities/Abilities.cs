// Created by Peoharen for the Mobile Abilities Package.
// #define ML // Used for the Bardic Peace ability
using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public partial class Ability
    {
        public static bool CanUse(Mobile from)
        {
            if (from == null)
                return false;

            return !(from.Frozen || from.Paralyzed || from.Map == null || from.Map == Map.Internal || !from.Alive);
        }

        public static bool CanUse(Mobile from, Mobile target)
        {
            return CanUse(from) && CanUse(from, target, true);
        }

        public static bool CanUse(Mobile from, Mobile target, bool harm)
        {
            if (!CanUse(from) || target == null)
                return false;
            else if (!from.CanSee(target))
                return false;
            else
                return CanTarget(from, target, harm);
        }

        #region Aura
        // Support for the old Aura permaiters
        public static void Aura(Mobile from, int min, int max, int type, int range, int poisons, string text)
        {
            ResistanceType rt = ResistanceType.Physical;
            Poison p = null;

            switch (type)
            {
                case 1:
                    rt = ResistanceType.Fire;
                    break;
                case 2:
                    rt = ResistanceType.Cold;
                    break;
                case 3:
                    rt = ResistanceType.Poison;
                    break;
                case 4:
                    rt = ResistanceType.Energy;
                    break;
            }

            switch (poisons)
            {
                case 1:
                    p = Poison.Lesser;
                    break;
                case 2:
                    p = Poison.Regular;
                    break;
                case 3:
                    p = Poison.Greater;
                    break;
                case 4:
                    p = Poison.Deadly;
                    break;
                case 5:
                    p = Poison.Lethal;
                    break;
            }

            Aura(from.Location, from.Map, from, min, max, rt, range, p, text, true, false, false, 0, 0);
        }

        // Mobile based Aura
        public static void Aura(Mobile from, int min, int max, ResistanceType type, int range, Poison poison, string text)
        {
            Aura(from.Location, from.Map, from, min, max, type, range, poison, text, true, false, false, 0, 0);
        }

        // Null based Aura
        public static void Aura(Point3D location, Map map, Mobile from, int min, int max, ResistanceType type, int range, Poison poison, string text)
        {
            Aura(location, map, from, min, max, type, range, poison, text, true, false, false, 0, 0);
        }

        // No Effects
        public static void Aura(Point3D location, Map map, Mobile from, int min, int max, ResistanceType type, int range, Poison poison, string text, bool scales, bool allownull)
        {
            Aura(location, map, from, min, max, type, range, poison, text, scales, allownull, false, 0, 0);
        }

        // Main Aura Method
        public static void Aura(Point3D location, Map map, Mobile from, int min, int max, ResistanceType type, int range, Poison poison, string text, bool scales, bool allownull, bool effects, int itemid, int hue)
        {
            if (from == null && !allownull)
                return;

            List<Mobile> targets = new List<Mobile>();

            foreach (Mobile m in Map.AllMaps[map.MapID].GetMobilesInRange(location, range))
            {
                if (CanTarget(from, m, true, false, allownull))
                    targets.Add(m);
            }

            if (effects && from != null)
                from.Animate(12, 5, 1, true, false, 0);

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile m = (Mobile)targets[i];
                m.RevealingAction();

                if (text != "")
                    m.SendMessage(text);

                int auradamage = Utility.RandomMinMax(min, max);

                if (scales)
                    auradamage = (int)((auradamage / GetDist(location, m.Location)) * range);

                if (poison != null)
                    m.ApplyPoison((from == null) ? m : from, poison);

                if (effects)
                    m.FixedParticles(itemid, 10, 15, 5030/*what the hell does this number do?*/, hue, 0, EffectLayer.Waist);

                switch (type)
                {
                    case ResistanceType.Physical:
                        AOS.Damage(m, (from == null) ? m : from, auradamage, 100, 0, 0, 0, 0);
                        break;
                    case ResistanceType.Fire:
                        AOS.Damage(m, (from == null) ? m : from, auradamage, 0, 100, 0, 0, 0);
                        break;
                    case ResistanceType.Cold:
                        AOS.Damage(m, (from == null) ? m : from, auradamage, 0, 0, 100, 0, 0);
                        break;
                    case ResistanceType.Poison:
                        AOS.Damage(m, (from == null) ? m : from, auradamage, 0, 0, 0, 100, 0);
                        break;
                    case ResistanceType.Energy:
                        AOS.Damage(m, (from == null) ? m : from, auradamage, 0, 0, 0, 0, 100);
                        break;
                }
            }

            targets.Clear();
        }

        #endregion

        #region UseBandage
        public static int UseBandage(BaseCreature from)
        {
            return UseBandage(from, false);
        }

        public static int UseBandage(BaseCreature from, bool healmaster)
        {
            if (from.IsDeadPet)
                return 12;

            int delay = (500 + (50 * ((120 - from.Dex) / 10))) / 100;

            if (delay < 3)
                delay = 3;

            if (from.Controlled && from.ControlMaster != null && from.Hits >= (from.Hits / 2) && healmaster)
            {
                if (from.InRange(from.ControlMaster, 2) && from.ControlMaster.Alive && from.ControlMaster.Hits < from.ControlMaster.HitsMax)
                    BandageContext.BeginHeal(from, from.ControlMaster);
            }
            else if (from.Hits < from.HitsMax)
            {
                BandageContext.BeginHeal(from, from);
            }

            return delay + 3;
        }

        #endregion

        #region Bard Skills
        // Warning: Untested
        public static bool CheckBarding(BaseCreature from)
        {
            BaseInstrument inst = BaseInstrument.GetInstrument(from);

            if (inst == null)
            {
                if (from.Backpack == null)
                    return false;

                inst = (BaseInstrument)from.Backpack.FindItemByType(typeof(BaseInstrument));

                if (inst == null)
                {
                    inst = new Harp();
                    inst.SuccessSound = 0x58B;
                    // inst.DiscordSound = inst.PeaceSound = 0x58B;
                    // inst.ProvocationSound = 0x58A;
                    inst.FailureSound = 0x58C;
                }
            }

            BaseInstrument.SetInstrument(from, inst);

            if (from.Skills[SkillName.Discordance].Base == 0)
                from.Skills[SkillName.Discordance].Base = 100.0;

            if (from.Skills[SkillName.Peacemaking].Base == 0)
                from.Skills[SkillName.Peacemaking].Base = 100.0;

            if (from.Skills[SkillName.Provocation].Base == 0)
                from.Skills[SkillName.Provocation].Base = 100.0;

            return true;
        }

        public static void UseDiscord(BaseCreature from)
        {
            if (from.Combatant == null || !CanUse(from) || !CheckBarding(from))
                return;

            //int effect = 0.0;

            //if ( SkillHandlers.Discordance.GetEffect( from.Combatant, ref effect ) )
            //return;

            if (!from.UseSkill(SkillName.Discordance))
                return;

            if (from.Combatant is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)from.Combatant;

                if (from.Target != null)
                    from.Target.Invoke(from, bc);
                else
                {
                    double effect = -(from.Skills[SkillName.Discordance].Value / 5.0);
                    TimeSpan duration = TimeSpan.FromSeconds(from.Skills[SkillName.Discordance].Value * 2);

                    ResistanceMod[] mods = 
                    {
                        new ResistanceMod(ResistanceType.Physical, (int)(effect * 0.01)),
                        new ResistanceMod(ResistanceType.Fire, (int)(effect * 0.01)),
                        new ResistanceMod(ResistanceType.Cold, (int)(effect * 0.01)),
                        new ResistanceMod(ResistanceType.Poison, (int)(effect * 0.01)),
                        new ResistanceMod(ResistanceType.Energy, (int)(effect * 0.01))
                    };

                    TimedResistanceMod.AddMod(bc, "Discordance", mods, duration);
                    bc.AddStatMod(new StatMod(StatType.Str, "DiscordanceStr", (int)(bc.RawStr * effect), duration));
                    bc.AddStatMod(new StatMod(StatType.Int, "DiscordanceInt", (int)(bc.RawInt * effect), duration));
                    bc.AddStatMod(new StatMod(StatType.Dex, "DiscordanceDex", (int)(bc.RawDex * effect), duration));
                }
            }
        }

        public class DiscordEffectTimer : Timer
        {
            public Mobile Mob;
            public int Count;
            public int MaxCount;

            public DiscordEffectTimer(Mobile mob, TimeSpan duration)
                : base(TimeSpan.FromSeconds(1.25), TimeSpan.FromSeconds(1.25))
            {
                this.Mob = mob;
                this.Count = 0;
                this.MaxCount = (int)((double)duration.TotalSeconds / 1.25);
            }

            protected override void OnTick()
            {
                if (this.Count >= this.MaxCount)
                    this.Stop();
                else
                {
                    this.Mob.FixedEffect(0x376A, 1, 32);
                    this.Count++;
                }
            }
        }

        public static void UsePeace(BaseCreature from)
        {
            if (from.Combatant == null || !(from.Combatant is Mobile) || !CanUse(from) || !CheckBarding(from))
                return;

            if (!from.UseSkill(SkillName.Peacemaking))
                return;

            if (from.Combatant is PlayerMobile)
            {
                #if ML
                                PlayerMobile pm = (PlayerMobile)from.Combatant;
                                if ( pm.PeacedUntil <= DateTime.UtcNow )
                                {
                                        pm.PeacedUntil = DateTime.UtcNow + TimeSpan.FromSeconds( (int)(from.Skills[SkillName.Peacemaking].Value / 5) );
                                        pm.SendLocalizedMessage( 500616 ); // You hear lovely music, and forget to continue battling!                                   
                                }
                #endif
            }
            else if (from.Target != null)
                from.Target.Invoke(from, from.Combatant);
        }

        public static void UseProvo(BaseCreature from, bool randomly)
        {
            if (((from.Combatant == null || !(from.Combatant is Mobile)) && randomly) || !CheckBarding(from))
                return;

            if (!CanUse(from))
                return;

            if (!from.UseSkill(SkillName.Provocation))
                return;

            Mobile targetone = FindRandomTarget(from, randomly);

            if (targetone == null)
                return;

            if (from.Target != null)
                from.Target.Invoke(from, targetone);

            Mobile targettwo = randomly ? FindRandomTarget(from, randomly) : from.Combatant as Mobile;

            if (targettwo == null)
                return;

            if (from.Target != null)
                from.Target.Invoke(from, targettwo);
        }

        #endregion

        #region MimicThem
        public static void MimicThem(BaseCreature from)
        {
            Mobile targ = from.Combatant as Mobile;
            MimicThem(from, false, false);
        }

        public static void MimicThem(BaseCreature from, bool allowskillchanges, bool allowAIchanges)
        {
            Mobile targ = from.Combatant as Mobile;
            MimicThem(from, targ, allowskillchanges, allowAIchanges);
        }

        public static void MimicThem(BaseCreature from, Mobile targ, bool allowskillchanges, bool allowAIchanges)
        {
            if (targ == null)
                return;

            if (from.BodyMod == 0)
            {
                from.BodyMod = targ.Body;
                from.Hue = targ.Hue;

                from.NameMod = targ.Name;
                from.Title = targ.Title;

                from.HairItemID = targ.HairItemID;
                from.FacialHairItemID = targ.FacialHairItemID;

                from.VirtualArmor = targ.VirtualArmor;

                foreach (Item item in targ.Items)
                {
                    if (item.Layer != Layer.Backpack && item.Layer != Layer.Mount)
                    {
                        /*
                        We don't dupe armor because the creatures base seed stacks with armor
                        By duping a high resistance player we shoot the creature up into the 100's in res
                        Imagine being the player facing your 400+ HP creature and EVERY attack & spell only deals 1 damage to them.
                        */
                        if (item is BaseShield)
                        {
                            Buckler shieldtomake = new Buckler();
                            shieldtomake.PoisonBonus = 0;
                            shieldtomake.ItemID = item.ItemID;
                            shieldtomake.Hue = item.Hue;
                            shieldtomake.Layer = item.Layer;
                            shieldtomake.Movable = false;
                            shieldtomake.Name = item.Name;
                            from.EquipItem(shieldtomake);
                        }
                        else if (item is BaseWeapon)
                        {
                            Broadsword weapontomake = new Broadsword();
                            weapontomake.ItemID = item.ItemID;
                            weapontomake.Hue = item.Hue;
                            weapontomake.Layer = item.Layer;
                            weapontomake.Movable = false;
                            weapontomake.Name = item.Name;

                            BaseWeapon weapon = item as BaseWeapon;
                            weapontomake.Animation = weapon.Animation;
                            weapontomake.HitSound = weapon.HitSound;
                            weapontomake.MissSound = weapon.MissSound;
                            weapontomake.MinDamage = weapon.MinDamage;
                            weapontomake.MaxDamage = weapon.MaxDamage;
                            weapontomake.Speed = weapon.Speed;
                            from.EquipItem(weapontomake);
                        }
                        else
                        {
                            Item itemtomake = new Item(item.ItemID);
                            itemtomake.Hue = item.Hue;
                            itemtomake.Layer = item.Layer;
                            itemtomake.Movable = false;
                            itemtomake.Name = item.Name;
                            from.EquipItem(itemtomake);
                        }
                    }
                }

                /*
                Duping skills can mess up the AI.
                What good is trying to melee when you have 0 tactics?
                On the other side, What good is stopping it's attack to try and cast something it can't do?
                The bool allows you to use it as a staff command or spell or make clone creatures that don't run around with the same exact skills as the others.
                */

                if (allowskillchanges)
                    for (int i = 0; i < targ.Skills.Length && i < from.Skills.Length; i++)
                        from.Skills[i].Base = targ.Skills[i].Base;
            }
            else
            {
                from.BodyMod = 0;
                from.Hue = 0;

                from.NameMod = null;
                from.Title = null;

                from.HairItemID = 0;
                from.FacialHairItemID = 0;

                from.VirtualArmor = 0;

                List<Item> list = new List<Item>(from.Items);

                foreach (Item item in list)
                {
                    if (item != null)
                        item.Delete();
                }

                if (allowskillchanges)
                {
                    for (int i = 0; i < targ.Skills.Length; ++i)
                        from.Skills[i].Base = 50.0;
                }
            }

            return;
        }

        #endregion

        #region DarkKnightAbilities
        /* Bull Rush
        He gathers energy and then slams into you from a
        distance, dealing heavy damage and physically knocking
        you back, stunning you.
        */

        public static void BullRush(Mobile from)
        {
            BullRush(from, "", 7);
        }

        public static void BullRush(Mobile from, string text, int duration)
        {
            Mobile target = from.Combatant as Mobile;

            if (target == null || CanUse(from, target))
                return;

            int dist = from.Str / 20;
            SlideAway(target, from.Location, (dist > 12) ? 12 : dist);

            if (text != "")
                target.SendMessage(text);
        }

        /* Echo Strike
        The Dark Knight teleports to one of the platforms in
        the room, and calls down lightning several times to
        strike you or your pets. The lightning is slightly
        displaced, allowing you a chance to escape, or even
        give him a taste of his own medicine.
        */

        public static void EchoStrike(Mobile from, int min, int max)
        {
            from.Paralyze(TimeSpan.FromSeconds(1));
            from.Animate(17, 5, 1, true, false, 0);

            List<Mobile> mobiles = new List<Mobile>();
            Point3D point;

            foreach (Mobile m in from.Map.GetMobilesInRange(from.Location, 14))
            {
                if (m != from && CanTarget(from, m, true, false, false))
                    mobiles.Add(m);
            }

            for (int i = 0; i < mobiles.Count; i++)
            {
                Mobile m = mobiles[i];

                if (Utility.Random(5) == 0)
                {
                    Effects.SendBoltEffect(m);
                    AOS.Damage(m, from, Utility.RandomMinMax(min, max), 0, 0, 0, 0, 100);
                    m.SendMessage("You get hit by a lightning bolt");
                }
                else
                {
                    point = RandomCloseLocation(from, 1);

                    if (from.Location == point)
                    {
                        AOS.Damage(from, from, Utility.RandomMinMax(min, max), 0, 0, 0, 0, 100);
                        Effects.SendBoltEffect(from);
                    }
                    else
                        Effects.SendBoltEffect(new Entity(Serial.Zero, point, from.Map));
                }
            }
        }

        /* Rally
        There is a chance the Dark Knight will attempt to
        heal himself of serious wounds. In this case, he
        teleports to the forefront of the room and begins a
        cycle of healing himself, and knocking back his foes.
        Strike him, or he could gain as much as 50% of his health back. 
        */

        public static void Rally(Mobile from)
        {
            Rally(from, 7);
        }

        public static void Rally(Mobile from, int delay)
        {
            from.Paralyze(TimeSpan.FromSeconds(4.0));
            from.Animate(6, 5, 1, true, false, 0);

            Timer timer = new RallyTimer(from, delay);
            timer.Start();
        }

        private class RallyTimer : Timer
        {
            private Mobile m_User;
            private int m_Count;
            private int m_MaxCount;

            public RallyTimer(Mobile user, int delay)
                : base(TimeSpan.FromMilliseconds(100.0), TimeSpan.FromMilliseconds(100.0))
            {
                this.m_User = user;
                this.m_Count = 0;
                this.m_MaxCount = delay;
            }

            protected override void OnTick()
            {
                if (this.m_Count >= (this.m_MaxCount + 1) || this.m_User == null || !this.m_User.Paralyzed)
                    this.Stop();

                if (this.m_Count == this.m_MaxCount)
                {
                    this.m_User.Heal((this.m_User.HitsMax / 2));
                    this.m_User.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                    this.m_User.PlaySound(0x202);
                }

                this.m_Count++;
            }
        }
        #endregion

        #region MiscAbilities
        public static void EtherealDrain(Mobile from, Mobile to, int type)
        {
            if (from == null || to == null)
                return;

            if (type == 1)
            {
                from.Say(1042156); //Your power is mine to use as I wish

                int amount = Utility.RandomMinMax(40, 80);
                to.Damage(amount, from);
                from.Hits += (amount / 2); //Halved to account for 50% resistance the target may have.
            }
            else if (type == 2)
            {
                from.Say(1042156); //Your power is mine to use as I wish

                int amount = (to.Mana * (100 - Utility.RandomMinMax(50, 90))) / 100;
                to.Mana -= amount;
                from.Mana += amount;
            }
            else
            {
                from.Say(1042157); //You shalt go nowhere unless I deem it be so

                int amount = (to.Stam * (100 - Utility.RandomMinMax(50, 100))) / 100;
                to.Stam -= amount;
                from.Stam += amount;
            }
        }

        public static void LowerStat(Mobile target, int minloss, int maxloss, int mintime, int maxtime, int type)
        {
            if (target.GetStatMod("LowerStats") != null)
                return;

            StatType stattype = StatType.Str;
            int offset = Utility.Random(minloss, maxloss);

            if (type <= 0 || type >= 4)
                type = Utility.RandomMinMax(1, 3);

            switch (type)
            {
                case 1:
                    stattype = StatType.Str;
                    break;
                case 2:
                    stattype = StatType.Dex;
                    break;
                case 3:
                    stattype = StatType.Int;
                    break;
            }

            target.AddStatMod(new StatMod(stattype, "LowerStats", -offset, TimeSpan.FromSeconds(Utility.Random(mintime, maxtime))));
        }

        public static void DamageArmor(Mobile target, int min, int max)
        {
            DamageArmor(target, min, max, 0);
        }

        public static void DamageArmor(Mobile target, int min, int max, int place)
        {
            double positionchance = Utility.RandomDouble();
            int ruin = Utility.RandomMinMax(min, max);

            if (place == 7 && target.Weapon is BaseWeapon)
            {
                //CS0266: Line 579: Cannot implicitly convert type 'Server.IWeapon' to 'Server
                //Items.BaseWeapon'. An explicit conversion exists (are you missing a cast?)
                //BaseWeapon weapon = target.Weapon;
                //if ( !(weapon is Fists) && !(weapon is BaseRanged) && weapon != null )
                //weapon.HitPoints -= ruin;
            }
            else
            {
                BaseArmor armor = null;

                if (positionchance < 0.7 || place == 1)
                    armor = target.NeckArmor as BaseArmor;
                else if (positionchance < 0.14 || place == 2)
                    armor = target.HandArmor as BaseArmor;
                else if (positionchance < 0.28 || place == 3)
                    armor = target.ArmsArmor as BaseArmor;
                else if (positionchance < 0.43 || place == 4)
                    armor = target.HeadArmor as BaseArmor;
                else if (positionchance < 0.65 || place == 5)
                    armor = target.LegsArmor as BaseArmor;
                else
                    armor = target.ChestArmor as BaseArmor;

                if (armor != null)
                    armor.HitPoints -= ruin;
            }
        }

        public static void TossBola(Mobile from)
        {
            if (from == null)
                return;

            Mobile target = from.Combatant as Mobile;

            if (target == null)
                return;
            else if (!target.Mounted)
                return;

            from.NonlocalOverheadMessage(MessageType.Emote, 0x3B2, 1049633, from.Name); // ~1_NAME~ begins to menacingly swing a bola...
            from.Direction = from.GetDirectionTo(target);
            from.Animate(11, 5, 1, true, false, 0);
            from.MovingEffect(target, 0x26AC, 10, 0, false, false);

            IMount mt = target.Mount;

            if (mt != null)
            {
                mt.Rider = null;
                target.SendLocalizedMessage(1040023); // You have been knocked off of your mount!
                BaseMount.SetMountPrevention(target, BlockMountType.Dazed, TimeSpan.FromSeconds(3.0));
            }
        }

        public static void TurnPet(Mobile target)
        {
            if (target is BaseCreature)
            {
                BaseCreature c = (BaseCreature)target;

                if (c.Controlled && c.ControlMaster != null)
                {
                    c.ControlTarget = c.ControlMaster;
                    c.ControlOrder = OrderType.Attack;
                    c.Combatant = c.ControlMaster;
                }
            }
        }

        private static int EnergyDrainCount = 0;

        public static void EnergyDrain(Mobile from, Mobile target)
        {
            EnergyDrain(from, target, 1, 5, true);
        }

        public static void EnergyDrain(Mobile from, Mobile target, int amount, int duration, bool skills)
        {
            if (amount < 0)
                amount = 1;

            target.AddStatMod(new StatMod(StatType.Str, "Energy Drain Str: " + EnergyDrainCount.ToString(), -amount, TimeSpan.FromMinutes(5)));
            target.AddStatMod(new StatMod(StatType.Dex, "Energy Drain Dex: " + EnergyDrainCount.ToString(), -amount, TimeSpan.FromMinutes(5)));
            target.AddStatMod(new StatMod(StatType.Int, "Energy Drain Int: " + EnergyDrainCount.ToString(), -amount, TimeSpan.FromMinutes(5)));

            if (skills)
                for (int i = 0; i < target.Skills.Length; ++i)
                    target.AddSkillMod(new TimedSkillMod((SkillName)i, true, (double)-amount, TimeSpan.FromMinutes(duration)));

            if (from != null)
                from.Hits += 5 * amount;

            EnergyDrainCount++;

            if (EnergyDrainCount > 65535)
                EnergyDrainCount = 0;
        }

        #endregion

        #region ToolHandOuts
        public static bool GiveItem(Mobile to, Item item)
        {
            return GiveItem(to, 0, item, false);
        }

        public static bool GiveItem(Mobile to, int hue, Item item)
        {
            return GiveItem(to, hue, item, false);
        }

        public static bool GiveItem(Mobile to, int hue, Item item, bool mustequip)
        {
            if (to == null && item == null)
                return false;

            if (hue != 0)
                item.Hue = hue;

            item.Movable = false;

            if (to.EquipItem(item))
                return true;

            Container pack = to.Backpack;

            if (pack != null && !mustequip)
            {
                pack.DropItem(item);
                return true;
            }
            else
                item.Delete();

            return false;
        }

        #endregion

        #region ToolTargeting
        public static Mobile FindRandomTarget(Mobile from)
        {
            return FindRandomTarget(from, true);
        }

        public static Mobile FindRandomTarget(Mobile from, bool allowcombatant)
        {
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in from.GetMobilesInRange(12))
            {
                if (m != null && m != from)
                    if (CanTarget(from, m) && from.InLOS(m))
                    {
                        if (allowcombatant && m == from.Combatant)
                            continue;
                        else
                            list.Add(m);
                    }
            }

            if (list.Count == 0)
                return null;
            if (list.Count == 1)
                return list[0];

            return list[Utility.Random(list.Count)];
        }

        public static bool CanTarget(Mobile from, Mobile to)
        {
            return CanTarget(from, to, true, false, false);
        }

        public static bool CanTarget(Mobile from, Mobile to, bool harm)
        {
            return CanTarget(from, to, harm, false, false);
        }

        public static bool CanTarget(Mobile from, Mobile to, bool harm, bool checkguildparty, bool allownull)
        {
            if (to == null)
                return false;
            else if (from == null)
                return allownull;
            else if (from == to && !harm)
                return true;
            else if ((harm && to.Blessed) || (to.AccessLevel != AccessLevel.Player && to.Hidden))
                return false;
            else if (harm)
            {
                if (!to.Alive)
                    return false;
                else if (to is BaseCreature)
                {
                    if (((BaseCreature)to).IsDeadPet)
                        return false;
                }
            }

            if (checkguildparty)
            {
                //Guilds
                Guild fromguild = GetGuild(from);
                Guild toguild = GetGuild(to);

                if (fromguild != null && toguild != null)
                    if (fromguild == toguild || fromguild.IsAlly(toguild))
                        return !harm;

                //Parties
                Party p = GetParty(from);

                if (p != null && p.Contains(to))
                    return !harm;
            }

            //Default
            if (harm)
                return (IsGoodGuy(from) && !(IsGoodGuy(to))) | (!(IsGoodGuy(from)) && IsGoodGuy(to));
            else
                return (IsGoodGuy(from) && IsGoodGuy(to)) | (!(IsGoodGuy(from)) && !(IsGoodGuy(to)));
        }

        public static bool IsGoodGuy(Mobile m)
        {
            if (m.Criminal)
                return false;

            if (m.Player && m.Kills < 5)
                return true;

            if (m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;

                if (bc.Controlled || bc.Summoned)
                {
                    if (bc.ControlMaster != null)
                        return IsGoodGuy(bc.ControlMaster);
                    else if (bc.SummonMaster != null)
                        return IsGoodGuy(bc.SummonMaster);
                }
            }

            return false;
        }

        public static Guild GetGuild(Mobile m)
        {
            Guild guild = m.Guild as Guild;

            if (guild == null && m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;
                m = bc.ControlMaster;

                if (m != null)
                    guild = m.Guild as Guild;

                m = bc.SummonMaster;

                if (m != null && guild == null)
                    guild = m.Guild as Guild;
            }

            return guild;
        }

        public static Party GetParty(Mobile m)
        {
            Party party = Party.Get(m);

            if (party == null && m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;
                m = bc.ControlMaster;

                if (m != null)
                    party = Party.Get(m);

                m = bc.SummonMaster;

                if (m != null && party == null)
                    party = Party.Get(m);
            }

            return party;
        }

        #endregion

        #region ToolPlaces
        public static double GetDist(Point3D start, Point3D end)
        {
            int xdiff = start.X - end.X;
            int ydiff = start.Y - end.Y;
            return Math.Sqrt((xdiff * xdiff) + (ydiff * ydiff));
        }

        public static void IncreaseByDirection(ref Point3D point, Direction d)
        {
            switch (d)
            {
                case (Direction)0x0:
                case (Direction)0x80:
                    point.Y--;
                    break; //North
                case (Direction)0x1:
                case (Direction)0x81:
                    {
                        point.X++;
                        point.Y--;
                        break;
                    }//Right
                case (Direction)0x2:
                case (Direction)0x82:
                    point.X++;
                    break; //East
                case (Direction)0x3:
                case (Direction)0x83:
                    {
                        point.X++;
                        point.Y++;
                        break;
                    }//Down
                case (Direction)0x4:
                case (Direction)0x84:
                    point.Y++;
                    break; //South
                case (Direction)0x5:
                case (Direction)0x85:
                    {
                        point.X--;
                        point.Y++;
                        break;
                    }//Left
                case (Direction)0x6:
                case (Direction)0x86:
                    point.X--;
                    break; //West
                case (Direction)0x7:
                case (Direction)0x87:
                    {
                        point.X--;
                        point.Y--;
                        break;
                    }//Up
                default:
                    {
                        break;
                    }
            }
        }

        public static bool TooManyCreatures(Type type, int maxcount, Mobile from)
        {
            if (from == null)
                return false;

            int count = 0;

            foreach (Mobile m in from.GetMobilesInRange(10))
            {
                if (m != null)
                    if (m.GetType() == type)
                        count++;
            }

            return count >= maxcount;
        }

        public static bool TooManyCreatures(Type[] types, int maxcount, Mobile from)
        {
            if (from == null)
                return false;

            int count = 0;

            foreach (Mobile m in from.GetMobilesInRange(10))
            {
                for (int i = 0; i < types.Length; i++)
                    if (m != null)
                        if (m.GetType() == types[i])
                            count++;
            }

            return count >= maxcount;
        }

        public static Point3D RandomCloseLocation(Mobile target)
        {
            return RandomCloseLocation(target, 1);
        }

        public static Point3D RandomCloseLocation(Mobile target, int range)
        {
            Point3D point = target.Location;
            bool canfit = false;

            for (int i = 0; !canfit && i < 10; i++)
            {
                point = target.Location;
                point.X += Utility.RandomMinMax(-range, range);
                point.Y += Utility.RandomMinMax(-range, range);
                point.Z = target.Map.GetAverageZ(point.X, point.Y);

                canfit = target.Map.CanFit(point.X, point.Y, point.Z, 16, false, false);
            }

            return (canfit) ? point : target.Location;
        }

        public static void SlideAway(Mobile target, Point3D point, int dist)
        {
            new SlideTimer(target, point, dist, true).Start();
        }

        public static void SlideTo(Mobile target, Point3D point, int dist)
        {
            new SlideTimer(target, point, dist, false).Start();
        }

        private class SlideTimer : Timer
        {
            private Mobile m_Mob;
            private Point3D m_Point;
            private int m_Dist;
            private bool m_Push;
            private int m_Count;

            public SlideTimer(Mobile mob, Point3D point, int dist, bool push)
                : base(TimeSpan.FromMilliseconds(100.0), TimeSpan.FromMilliseconds(100.0))
            {
                this.m_Mob = mob;
                this.m_Point = point;
                this.m_Dist = dist;
                this.m_Push = push;
                this.m_Count = 0;

                this.m_Mob.CantWalk = true;
            }

            protected override void OnTick()
            {
                if (this.m_Mob == null)
                {
                    this.Stop();
                    return;
                }
                else if (this.m_Count >= this.m_Dist)
                {
                    this.m_Mob.CantWalk = false;
                    this.Stop();
                    return;
                }

                Direction d = this.m_Mob.GetDirectionTo(this.m_Point);
                Point3D moveto = new Point3D(this.m_Mob.X, this.m_Mob.Y, this.m_Mob.Z);

                if (this.m_Push)
                {
                    switch (d)
                    {
                        case (Direction)0x0:
                        case (Direction)0x80:
                            d = (Direction)0x4;
                            break; // North to South
                        case (Direction)0x1:
                        case (Direction)0x81:
                            d = (Direction)0x5;
                            break; // Right to Left
                        case (Direction)0x2:
                        case (Direction)0x82:
                            d = (Direction)0x6;
                            break; // East to West
                        case (Direction)0x3:
                        case (Direction)0x83:
                            d = (Direction)0x7;
                            break; // Down to Up
                        case (Direction)0x4:
                        case (Direction)0x84:
                            d = (Direction)0x0;
                            break; // South to North
                        case (Direction)0x5:
                        case (Direction)0x85:
                            d = (Direction)0x1;
                            break; // Left to Right
                        case (Direction)0x6:
                        case (Direction)0x86:
                            d = (Direction)0x2;
                            break; // West to East
                        case (Direction)0x7:
                        case (Direction)0x87:
                            d = (Direction)0x3;
                            break; // Up to Down
                        default:
                            {
                                break;
                            }
                    }
                }

                IncreaseByDirection(ref moveto, d);
                this.m_Mob.Direction = d;

                if (this.m_Mob.Map.CanFit(moveto.X, moveto.Y, this.m_Mob.Map.GetAverageZ(moveto.X, moveto.Y), 16, false, false))
                    this.m_Mob.Location = moveto;

                this.m_Count++;
            }
        }

        #endregion

        #region ToolWeapons
        public static void Strike(Mobile from)
        {
            Strike(from, 1);
        }

        public static void Strike(Mobile from, int count)
        {
            if (from.Frozen || from.Paralyzed)
                return;

            Mobile target = from.Combatant as Mobile;

            if (target == null)
                return;

            if (from.InRange(target.Location, 1))
                if (from.Weapon != null)
                    if (from.Weapon is BaseWeapon)
                    {
                        BaseWeapon weapon = (BaseWeapon)from.Weapon;

                        for (int i = 0; i < count + 1; i++)
                            if (target != null)
                                weapon.OnHit(from, target, 1.0);
                    }
        }

        #endregion

        #region SimpleFlame
        public static void SimpleFlame(Mobile from, Mobile target)
        {
            if (!CanUse(from, target))
                return;

            from.Say("*Ul Flam*");

            Effects.SendLocationParticles(
                EffectItem.Create(new Point3D(from.X - 1, from.Y - 1, from.Z), from.Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 0, 4, 0, 0);
            Effects.SendLocationParticles(
                EffectItem.Create(new Point3D(from.X - 1, from.Y + 1, from.Z), from.Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 0, 4, 0, 0);
            Effects.SendLocationParticles(
                EffectItem.Create(new Point3D(from.X + 1, from.Y - 1, from.Z), from.Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 0, 4, 0, 0);
            Effects.SendLocationParticles(
                EffectItem.Create(new Point3D(from.X + 1, from.Y + 1, from.Z), from.Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 0, 4, 0, 0);

            new SimpleFlameTimer(from, target).Start();
        }
        #endregion
    }
}