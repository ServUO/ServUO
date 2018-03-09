using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a medusa corpse")]
    public class Medusa : BaseSABosses, ICarvable
    {
        private List<Mobile> m_TurnedToStone = new List<Mobile>();
        public List<Mobile> AffectedMobiles { get { return m_TurnedToStone; } }

        public List<Mobile> m_Helpers = new List<Mobile>();

        private int m_Scales;
        private DateTime m_GazeDelay;
        private DateTime m_StoneDelay;
        private DateTime m_NextCarve;

        [Constructable]
        public Medusa()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.1, 0.2)
        {
            Name = "Medusa";
            Body = 728;

            SetStr(1235, 1391);
            SetDex(128, 139);
            SetInt(537, 664);

            SetHits(60000);

            SetDamage(21, 28);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 55, 65);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 60, 75);

            SetSkill(SkillName.Anatomy, 110.6, 116.1);
            SetSkill(SkillName.EvalInt, 100.0, 114.4);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.Meditation, 118.2, 127.8);
            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 111.9, 134.5);
            SetSkill(SkillName.Wrestling, 119.7, 128.9);

            Fame = 22000;
            Karma = -22000;

            VirtualArmor = 60;

            PackItem(new Arrow(Utility.RandomMinMax(100, 200)));

            IronwoodCompositeBow Bow = new IronwoodCompositeBow();
            Bow.Movable = false;
            AddItem(Bow);

            m_Scales = Utility.RandomMinMax(1, 2) + 7;

            SetWeaponAbility(WeaponAbility.MortalStrike);
            SetSpecialAbility(SpecialAbility.VenomousBite);
        }

        public Medusa(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get { return new Type[] { typeof(Slither), typeof(IronwoodCompositeBow), typeof(Venom), typeof(PetrifiedSnake), typeof(StoneDragonsTooth) }; }
        }

        public override Type[] SharedSAList
        {
            get { return new Type[] { typeof(SummonersKilt) }; }
        }

        public override bool IgnoreYoungProtection { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override double AutoDispelChance { get { return 1.0; } }
        public override bool BardImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return (0.8 >= Utility.RandomDouble() ? Poison.Deadly : Poison.Lethal); } }

        public override int GetIdleSound() { return 1557; }
        public override int GetAngerSound() { return 1554; }
        public override int GetHurtSound() { return 1556; }
        public override int GetDeathSound() { return 1555; }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            int amount = Utility.Random(5) + 1;

            corpse.DropItem(new MedusaDarkScales(amount));

            if(0.20 > Utility.RandomDouble())
                corpse.DropItem(new MedusaBlood());

            base.OnCarve(from, corpse, with);

            corpse.Carved = true;
        }

        public override void OnGotMeleeAttack(Mobile m)
        {
            base.OnGotMeleeAttack(m);

            if (0.05 > Utility.RandomDouble())
                ReleaseStoneMonster();
        }

        public override void OnDamagedBySpell(Mobile m)
        {
            base.OnDamagedBySpell(m);

            if (0.05 > Utility.RandomDouble())
                ReleaseStoneMonster();
        }

        public override void OnHarmfulSpell(Mobile from)
        {
            base.OnHarmfulSpell(from);

            if (0.05 > Utility.RandomDouble())
                ReleaseStoneMonster();
        }

        public void RemoveAffectedMobiles(Mobile toRemove)
        {
            if (m_TurnedToStone.Contains(toRemove))
                m_TurnedToStone.Remove(toRemove);
        }

        public Mobile FindRandomMedusaTarget()
        {
            List<Mobile> list = new List<Mobile>();

            IPooledEnumerable eable = this.GetMobilesInRange(12);
            foreach (Mobile m in eable)
            {
                if ( m == null || m == this || m_TurnedToStone.Contains(m) || !CanBeHarmful(m) || !InLOS(m) || m.AccessLevel > AccessLevel.Player)
                    continue;

                //Pets
                if (m is BaseCreature && (((BaseCreature)m).GetMaster() is PlayerMobile))
                    list.Add(m);
                //players
                else if (m is PlayerMobile)
                    list.Add(m);
            }
            eable.Free();

            if (list.Count == 0)
                return null;
            if (list.Count == 1)
                return list[0];

            return list[Utility.Random(list.Count)];
        }

        public static bool CheckBlockGaze(Mobile m)
        {
            if (m == null)
                return false;

            Item helm = m.FindItemOnLayer(Layer.Helm);
            Item neck = m.FindItemOnLayer(Layer.Neck);
            Item ear = m.FindItemOnLayer(Layer.Earrings);
            Item shi = m.FindItemOnLayer(Layer.TwoHanded);

            bool deflect = false;
            int perc = 0;

            if (helm != null)
            {
                if (helm is BaseArmor && ((BaseArmor)helm).GorgonLenseCharges > 0)
                {
                    perc = GetScaleEffectiveness(((BaseArmor)helm).GorgonLenseType);

                    if (perc > Utility.Random(100))
                    {
                        ((BaseArmor)helm).GorgonLenseCharges--;
                        deflect = true;
                    }
                }
                else if (helm is BaseClothing && ((BaseClothing)helm).GorgonLenseCharges > 0)
                {
                    perc = GetScaleEffectiveness(((BaseClothing)helm).GorgonLenseType);

                    if (perc > Utility.Random(100))
                    {
                        ((BaseClothing)helm).GorgonLenseCharges--;
                        deflect = true;
                    }
                }
            }

            if (!deflect && shi != null && shi is BaseShield && ((BaseArmor)shi).GorgonLenseCharges > 0)
            {
                perc = GetScaleEffectiveness(((BaseArmor)shi).GorgonLenseType);

                if (perc > Utility.Random(100))
                {
                    ((BaseArmor)shi).GorgonLenseCharges--;
                    deflect = true;
                }
            }

            if (!deflect && neck != null)
            {
                if (neck is BaseArmor && ((BaseArmor)neck).GorgonLenseCharges > 0)
                {
                    perc = GetScaleEffectiveness(((BaseArmor)neck).GorgonLenseType);

                    if (perc > Utility.Random(100))
                    {
                        ((BaseArmor)neck).GorgonLenseCharges--;
                        deflect = true;
                    }
                }
                else if (neck is BaseJewel && ((BaseJewel)neck).GorgonLenseCharges > 0)
                {
                    perc = GetScaleEffectiveness(((BaseJewel)neck).GorgonLenseType);

                    if (perc > Utility.Random(100))
                    {
                        ((BaseJewel)neck).GorgonLenseCharges--;
                        deflect = true;
                    }
                }
                else if (neck is BaseClothing && ((BaseClothing)neck).GorgonLenseCharges > 0)
                {
                    perc = GetScaleEffectiveness(((BaseClothing)neck).GorgonLenseType);

                    if (perc > Utility.Random(100))
                    {
                        ((BaseClothing)neck).GorgonLenseCharges--;
                        deflect = true;
                    }
                }
            }

            if (!deflect && ear != null)
            {
                if (ear is BaseJewel && ((BaseJewel)ear).GorgonLenseCharges > 0)
                {
                    perc = GetScaleEffectiveness(((BaseJewel)ear).GorgonLenseType);

                    if (perc > Utility.Random(100))
                    {
                        ((BaseJewel)ear).GorgonLenseCharges--;
                        deflect = true;
                    }
                }
            }

            return deflect;
        }

        private static int GetScaleEffectiveness(LenseType type)
        {
            switch (type)
            {
                case LenseType.None: return 0;
                case LenseType.Enhanced: return 100;
                case LenseType.Regular: return 50;
                case LenseType.Limited: return 15;
            }

            return 0;
        }

        public bool Carve(Mobile from, Item item)
        {
            if (m_Scales > 0)
            {
                if (DateTime.UtcNow < m_NextCarve)
                {
                    from.SendLocalizedMessage(1112677); // The creature is still recovering from the previous harvest. Try again in a few seconds.
                }
                else
                {
                    int amount = Math.Min(m_Scales, Utility.RandomMinMax(2, 3));

                    m_Scales -= amount;

                    Item scales = new MedusaLightScales(amount);

                    if (from.PlaceInBackpack(scales))
                    {
                        // You harvest magical resources from the creature and place it in your bag.
                        from.SendLocalizedMessage(1112676);
                    }
                    else
                    {
                        scales.MoveToWorld(from.Location, from.Map);
                    }

                    new Blood(0x122D).MoveToWorld(Location, Map);

                    m_NextCarve = DateTime.UtcNow + TimeSpan.FromMinutes(1.0);
                    return true;
                }
            }
            else
                from.SendLocalizedMessage(1112674); // There's nothing left to harvest from this creature.

            return false;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (m_StoneDelay < DateTime.UtcNow)
                SpawnStone();

            if (m_GazeDelay < DateTime.UtcNow)
                DoGaze();
        }

        public void DoGaze()
        {
            Mobile target = FindRandomMedusaTarget();
            Map map = Map;

            if (map == null || target == null)
                return;

            if ((target is BaseCreature && ((BaseCreature)target).SummonMaster != this) || CanBeHarmful(target))
            {
                if (CheckBlockGaze(target))
                {
                    if (GorgonLense.TotalCharges(target) == 0)
                        target.SendLocalizedMessage(1112600); // Your lenses crumble. You are no longer protected from Medusa's gaze!
                    else
                        target.SendLocalizedMessage(1112599); //Your Gorgon Lens deflect Medusa's petrifying gaze!
                }
                else
                {
                    BaseCreature clone = new MedusaClone(target);

                    bool validLocation = false;
                    Point3D loc = Location;

                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        int x = X + Utility.Random(10) - 1;
                        int y = Y + Utility.Random(10) - 1;
                        int z = map.GetAverageZ(x, y);

                        if (validLocation = map.CanFit(x, y, Z, 16, false, false))
                            loc = new Point3D(x, y, Z);
                        else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                            loc = new Point3D(x, y, z);
                    }

                    Effects.SendLocationEffect(loc, target.Map, 0x37B9, 10, 5);
                    clone.Frozen = clone.Blessed = true;
                    clone.SolidHueOverride = 761;

                    target.Frozen = target.Blessed = true;
                    target.SolidHueOverride = 761;

                    //clone.MoveToWorld(loc, target.Map);
                    BaseCreature.Summon(clone, false, this, loc, 0, TimeSpan.FromMinutes(90));

                    if (target is BaseCreature && !((BaseCreature)target).Summoned && ((BaseCreature)target).GetMaster() != null)
                        ((BaseCreature)target).GetMaster().SendLocalizedMessage(1113281, null, 43); // Your pet has been petrified!
                    else
                        target.SendLocalizedMessage(1112768); // You have been turned to stone!!!

                    new GazeTimer(target, clone, this, Utility.RandomMinMax(5, 10)).Start();
                    m_GazeDelay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(45, 75));

                    m_Helpers.Add(clone);
                    m_TurnedToStone.Add(target);

                    BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.MedusaStone, 1153790, 1153825));
                    return;
                }
            }

            m_GazeDelay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 65));
        }

        public void SpawnStone()
        {
            DefragHelpers();

            Map map = Map;

            if (map == null)
                return;

            int stones = 0;

            foreach (Mobile m in m_Helpers)
            {
                if (!(m is MedusaClone))
                    ++stones;
            }

            if (stones >= 5)
                return;
            else
            {
                BaseCreature stone = GetRandomStoneMonster();

                bool validLocation = false;
                Point3D loc = Location;

                for (int j = 0; !validLocation && j < 10; ++j)
                {
                    int x = X + Utility.Random(10) - 1;
                    int y = Y + Utility.Random(10) - 1;
                    int z = map.GetAverageZ(x, y);

                    if (validLocation = map.CanFit(x, y, Z, 16, false, false))
                        loc = new Point3D(x, y, Z);
                    else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                        loc = new Point3D(x, y, z);
                }

                BaseCreature.Summon(stone, false, this, loc, 0, TimeSpan.FromMinutes(90));
                //stone.MoveToWorld(loc, map);
                stone.Frozen = stone.Blessed = true;
                stone.SolidHueOverride = 761;
                stone.Combatant = null;

                m_Helpers.Add(stone);
            }

            m_StoneDelay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 150));
        }

        private void DefragHelpers()
        {
            List<Mobile> toDelete = new List<Mobile>();

            foreach (Mobile m in m_Helpers)
            {
                if(m == null)
                    continue;

                if (!m.Alive || m.Deleted)
                    toDelete.Add(m);
            }

            foreach (Mobile m in toDelete)
            {
                if (m_Helpers.Contains(m))
                    m_Helpers.Remove(m);
            }
        }

        private BaseCreature GetRandomStoneMonster()
        {
            switch (Utility.Random(6))
            {
                default:
                case 0: return new OphidianWarrior();
                case 1: return new OphidianArchmage();
                case 2: return new WailingBanshee();
                case 3: return new OgreLord();
                case 4: return new Dragon();
                case 5: return new UndeadGargoyle();
            }
        }

        public void ReleaseStoneMonster()
        {
            List<Mobile> stones = new List<Mobile>();

            foreach (Mobile mob in m_Helpers)
            {
                if (!(mob is MedusaClone) && mob.Alive)
                    stones.Add(mob);
            }

            if(stones.Count == 0)
                return;

            Mobile m = stones[Utility.Random(stones.Count)];

            if (m != null)
            {
                m.Frozen = m.Blessed = false;
                m.SolidHueOverride = -1;
                Mobile closest = null;
                int dist = 12;

                m_Helpers.Remove(m);

                IPooledEnumerable eable = m.GetMobilesInRange(12);
                foreach (Mobile targ in eable)
                {
                    if (targ != null && targ.Player)
                    {
                        targ.SendLocalizedMessage(1112767, null, 43); // Medusa releases one of the petrified creatures!!
                        targ.Combatant = targ;
                    }

                    if (targ is PlayerMobile || (targ is BaseCreature && ((BaseCreature)targ).GetMaster() is PlayerMobile))
                    {
                        int d = (int)m.GetDistanceToSqrt(targ.Location);

                        if (d < dist)
                        {
                            dist = d;
                            closest = targ;
                        }
                    }
                }
                eable.Free();

                if (closest != null)
                    m.Combatant = closest;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 8);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new MedusaStatue());
        }

        public override void OnAfterDelete()
        {
            foreach (Mobile m in m_Helpers)
            {
                if (m != null && !m.Deleted)
                    m.Delete();
            }

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write((int)m_Scales);

            writer.Write(m_Helpers.Count);

            foreach (Mobile helper in m_Helpers)
                writer.Write(helper);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Scales = reader.ReadInt();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();

                if (m != null && m.Alive)
                    m_Helpers.Add(m);
            }
        }

        public class GazeTimer : Timer
        {
            private Mobile target;
            private Mobile clone;
            private Medusa m_Medusa;
            private int m_Count;

            public GazeTimer(Mobile m, Mobile mc, Medusa medusa, int duration)
                : base(TimeSpan.FromSeconds(duration), TimeSpan.FromSeconds(duration))
            {
                target = m;
                clone = mc;
                m_Medusa = medusa;
                m_Count = 0;
            }

            protected override void OnTick()
            {
                ++m_Count;

                if (m_Count == 1 && target != null)
                {
                    target.Frozen = false;
                    target.SolidHueOverride = -1;
                    target.Blessed = false;
                    m_Medusa.RemoveAffectedMobiles(target);

                    if (target is BaseCreature && !((BaseCreature)target).Summoned && ((BaseCreature)target).GetMaster() != null)
                        ((BaseCreature)target).GetMaster().SendLocalizedMessage(1113285, null, 43); // Beware! A statue of your pet has been created!

                    BuffInfo.RemoveBuff(target, BuffIcon.MedusaStone);
                }
                else if (m_Count == 2 && clone != null)
                {
                    clone.SolidHueOverride = -1;
                    clone.Frozen = clone.Blessed = false;
                    int dist = 12;
                    Mobile closest = null;

                    IPooledEnumerable eable = clone.GetMobilesInRange(12);
                    foreach (Mobile m in eable)
                    {
                        int d = (int)clone.GetDistanceToSqrt(m.Location);

                        if (m != null && m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile))
                        {
                            if (m.NetState != null)
                            {
                                m.Send(new RemoveMobile(clone));
                                m.NetState.Send(MobileIncoming.Create(m.NetState, m, clone));
                                m.SendLocalizedMessage(1112767); // Medusa releases one of the petrified creatures!!
                            }

                            if (d < dist)
                            {
                                dist = d;
                                closest = m;
                            }
                        }
                    }
                    eable.Free();

                    if (closest != null)
                        clone.Combatant = closest;
                }
                else
                    Stop();
            }
        }
    }

    public class MedusaClone : BaseCreature, IFreezable
    {
        public MedusaClone(Mobile m)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SolidHueOverride = 33;
            Clone(m);
        }

        public MedusaClone(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return true;
            }
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return !Frozen;
            }
        }
        public void Clone(Mobile m)
        {
            if (m == null)
            {
                Delete();
                return;
            }

            Body = m.Body;

            Str = m.Str;
            Dex = m.Dex;
            Int = m.Int;

            Hits = m.HitsMax;

            Hue = m.Hue;
            Female = m.Female;

            Name = m.Name;
            NameHue = m.NameHue;

            Title = m.Title;
            Kills = m.Kills;

            HairItemID = m.HairItemID;
            HairHue = m.HairHue;

            FacialHairItemID = m.FacialHairItemID;
            FacialHairHue = m.FacialHairHue;

            BaseSoundID = m.BaseSoundID;

            for (int i = 0; i < m.Skills.Length; ++i)
            {
                Skills[i].Base = m.Skills[i].Base;
                Skills[i].Cap = m.Skills[i].Cap;
            }

            for (int i = 0; i < m.Items.Count; i++)
            {
                if (m.Items[i].Layer != Layer.Backpack && m.Items[i].Layer != Layer.Mount && m.Items[i].Layer != Layer.Bank)
                    AddItem(CloneItem(m.Items[i]));
            }
        }

        public Item CloneItem(Item item)
        {
            Item cloned = new Item(item.ItemID);
            cloned.Layer = item.Layer;
            cloned.Name = item.Name;
            cloned.Hue = item.Hue;
            cloned.Weight = item.Weight;
            cloned.Movable = false;

            return cloned;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Frozen)
                DisplayPaperdollTo(from);
            else
                base.OnDoubleClick(from);
        }

        public void OnRequestedAnimation(Mobile from)
        {
            if (Frozen)
            {
                from.Send(new UpdateStatueAnimation(this, 1, 31, 5));
            }
        }

        public override void OnDelete()
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 15, 5042);

            base.OnDelete();
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
            Delete();
        }
    }
}

namespace Server.Commands
{
    public class AddCloneCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("addclone", AccessLevel.Seer, new CommandEventHandler(AddClone_OnCommand));
        }

        [Description("")]
        public static void AddClone_OnCommand(CommandEventArgs e)
        {
            BaseCreature clone = new MedusaClone(e.Mobile);
            clone.Frozen = clone.Blessed = true;
            clone.MoveToWorld(e.Mobile.Location, e.Mobile.Map);
        }
    }
}