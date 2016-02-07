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
        public static List<Mobile> AffectedMobiles = new List<Mobile>();
        //private readonly DateTime m_Delay;
        private int m_Scales;
        private DateTime m_GazeDelay;
        private DateTime m_DelayOne;
        private DateTime m_DelayTwo;
        [Constructable]
        public Medusa()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Medusa";
            this.Body = 728;

            this.SetStr(1235, 1391);
            this.SetDex(128, 139);
            this.SetInt(537, 664);

            this.SetHits(170000);

            this.SetDamage(21, 28);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 55, 65);
            this.SetResistance(ResistanceType.Cold, 55, 65);
            this.SetResistance(ResistanceType.Poison, 80, 90);
            this.SetResistance(ResistanceType.Energy, 60, 75);

            this.SetSkill(SkillName.Anatomy, 110.6, 116.1);
            this.SetSkill(SkillName.EvalInt, 100.0, 114.4);
            this.SetSkill(SkillName.Magery, 100.0);
            this.SetSkill(SkillName.Meditation, 118.2, 127.8);
            this.SetSkill(SkillName.MagicResist, 120.0);
            this.SetSkill(SkillName.Tactics, 111.9, 134.5);
            this.SetSkill(SkillName.Wrestling, 119.7, 128.9);

            this.Fame = 22000;
            this.Karma = -22000;

            this.VirtualArmor = 60;

            this.PackItem(new Arrow(Utility.RandomMinMax(600, 700)));

            IronwoodCompositeBow Bow = new IronwoodCompositeBow();
            Bow.Movable = false;
            this.AddItem(Bow);

            this.m_Scales = Utility.Random(5) + 1;
        }

        public Medusa(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { typeof(Slither), typeof(IronwoodCompositeBow) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(DemonBridleRing), typeof(PetrifiedSnake), typeof(StoneDragonsTooth), typeof(SummonersKilt), typeof(Venom) };
            }
        }
        public override bool IgnoreYoungProtection
        {
            get
            {
                return true;
            }
        }
        public override int Scales
        {
            get
            {
                return (Utility.Random(5) + 1);
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return ScaleType.MedusaLight;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool GivesSAArtifact
        {
            get
            {
                return true;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return false;
            }
        }
		public override bool AllureImmune
		{
			get
			{
				return true;
			}
		}
        public override bool Unprovokable
        {
            get
            {
                return false;
            }
        }
        public override bool AreaPeaceImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Deadly : Poison.Lethal);
            }
        }
        public static Mobile FindRandomMedusaTarget(Mobile from)
        {
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in from.GetMobilesInRange(12))
            {
                if (m != null && m != from && !(m is MedusaClone) && !(m is StoneMonster) && !(Medusa.AffectedMobiles.Contains(m)) && !(m is BaseCreature))
                {
                    Medusa.AffectedMobiles.Remove(m);
                    list.Add(m);
                }

                if (m != null && m != from && !(m is MedusaClone) && !(m is StoneMonster) && !(Medusa.AffectedMobiles.Contains(m)) && (m is BaseCreature))
                {
                    Medusa.AffectedMobiles.Remove(m);
                    list.Add(m);
                }
            }

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

        public override int GetIdleSound()
        {
            return 1557;
        }

        public override int GetAngerSound()
        {
            return 1554;
        }

        public override int GetHurtSound()
        {
            return 1556;
        }

        public override int GetDeathSound()
        {
            return 1555;
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            int amount = Utility.Random(5) + 1;

            corpse.DropItem(new MedusaLightScales(amount));

            if (0.20 > Utility.RandomDouble())
                corpse.DropItem(new MedusaBlood());

            base.OnCarve(from, corpse, with);
        }

        public void Carve(Mobile from, Item item)
        {
            if (this.m_Scales > 0 && from.Backpack != null)
            {
                new Blood(0x122D).MoveToWorld(this.Location, this.Map);
                from.AddToBackpack(new MedusaDarkScales(Utility.Random(3) + 1));
                from.SendLocalizedMessage(1114098); // You cut away some scales and put them in your backpack.
                this.Combatant = from;
                --this.m_Scales;
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow > this.m_DelayOne)
            {
                Medusa.AffectedMobiles.Clear();

                this.m_DelayOne = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 180));
            }

            if (DateTime.UtcNow > this.m_GazeDelay)
            {
                Mobile target = FindRandomMedusaTarget(this);
                //  Mobile target = Ability.FindRandomMedusaTarget(this);

                Map map = this.Map;

                if (map == null)
                    return;

                this.DoSpecialAbility(this);

                if ((target != null) && !(target is MedusaClone) && !(target is StoneMonster))
                {
                    if (CheckBlockGaze(target))
                    {
                        target.SendLocalizedMessage(1112599); //Your Gorgon Lens deflect Medusa's petrifying gaze!
                        return;
                    }

                    BaseCreature clone = new MedusaClone(target);

                    bool validLocation = false;
                    Point3D loc = this.Location;

                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        int x = this.X + Utility.Random(10) - 1;
                        int y = this.Y + Utility.Random(10) - 1;
                        int z = map.GetAverageZ(x, y);

                        if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
                            loc = new Point3D(x, y, this.Z);
                        else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                            loc = new Point3D(x, y, z);
                    }

                    Effects.SendLocationEffect(loc, target.Map, 0x37B9, 10, 5);
                    clone.Frozen = clone.Blessed = true;
                    clone.SolidHueOverride = 761;
                    target.Frozen = true;
                    target.SolidHueOverride = 761;
                    clone.MoveToWorld(loc, target.Map);
                    target.SendLocalizedMessage(1112768); // You have been turned to stone!!!

                    new GazeTimer(target, clone).Start();
                    this.m_GazeDelay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));

                    AffectedMobiles.Add(target);
                }
            }
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (0.8 >= Utility.RandomDouble())
                this.SpawnStone(target);

            if (0.05 >= Utility.RandomDouble())
                this.FreeStone(target);
        }

        public void SpawnStone(Mobile target)
        {
            Map map = this.Map;

            if (map == null)
                return;

            if (DateTime.UtcNow > this.m_DelayTwo)
            {
                int stones = 0;

                foreach (Mobile m in this.GetMobilesInRange(40))
                {
                    if (m is StoneMonster)
                        ++stones;
                }

                if (stones >= 3)
                {
                    return;
                }
                else
                {
                    BaseCreature stone = new StoneMonster();

                    bool validLocation = false;
                    Point3D loc = this.Location;

                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        int x = this.X + Utility.Random(10) - 1;
                        int y = this.Y + Utility.Random(10) - 1;
                        int z = map.GetAverageZ(x, y);

                        if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
                            loc = new Point3D(x, y, this.Z);
                        else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                            loc = new Point3D(x, y, z);
                    }

                    stone.MoveToWorld(loc, target.Map);
                    stone.Frozen = stone.Blessed = true;
                    stone.SolidHueOverride = 761;
                    stone.Combatant = null;
                }

                this.m_DelayTwo = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 150));
            }
        }

        public void FreeStone(Mobile target)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)), delegate()
            {
                List<Mobile> list = new List<Mobile>();

                foreach (Mobile mob in this.GetMobilesInRange(40))
                {
                    if (mob is StoneMonster)
                        list.Add(mob);
                }

                if (0 != list.Count)
                {
                    int j = Utility.Random(list.Count);
                    Mobile stone1 = list.ToArray()[j];

                    stone1.Frozen = stone1.Blessed = false;
                    stone1.SolidHueOverride = -1;

                    foreach (Mobile targ in stone1.GetMobilesInRange(40))
                    {
                        if (targ != null && targ.Player)
                        {
                            targ.SendLocalizedMessage(1112767); // Medusa releases one of the petrified creatures!!
                            stone1.Combatant = targ;
                        }
                    }
                }
            });
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 5);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.025)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new MedusaBlood());
                        break;
                    case 1:
                        c.DropItem(new MedusaStatue());
                        break;
                }
            }
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.MortalStrike;
        }

        public override void OnAfterDelete()
        {
            this.DeleteMedusaClone(this);

            for (int i = 0; i < AffectedMobiles.Count; i++)
                if (AffectedMobiles[i] != null)
                    AffectedMobiles.Clear();

            base.OnAfterDelete();
        }

        public override bool OnBeforeDeath()
        {
            this.DeleteMedusaClone(this);

            for (int i = 0; i < AffectedMobiles.Count; i++)
                if (AffectedMobiles[i] != null)
                    AffectedMobiles.Clear();

            return base.OnBeforeDeath();
        }

        public override void OnDelete()
        {
            this.DeleteMedusaClone(this);

            for (int i = 0; i < AffectedMobiles.Count; i++)
                if (AffectedMobiles[i] != null)
                    AffectedMobiles.Clear();

            base.OnDelete();
        }

        public void DeleteMedusaClone(Mobile target)
        {
            ArrayList list = new ArrayList();

            foreach (Mobile clone in this.GetMobilesInRange(40))
            {
                if (clone is MedusaClone || clone is StoneMonster)
                    list.Add(clone);
            }

            foreach (Mobile clone in list)
            {
                clone.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write((int)this.m_Scales);
            writer.Write(AffectedMobiles);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            reader.ReadInt();
            AffectedMobiles = reader.ReadStrongMobileList();

            for (int i = 0; i < AffectedMobiles.Count; i++)
                if (AffectedMobiles[i] != null)
                    AffectedMobiles.Clear();
        }

        public class GazeTimer : Timer
        {
            private readonly Mobile target;
            private readonly Mobile clone;
            private int m_Count;
            public GazeTimer(Mobile m, Mobile mc)
                : base(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20))
            {
                this.target = m;
                this.clone = mc;
                this.m_Count = 0;
            }

            protected override void OnTick()
            {
                ++this.m_Count;

                if (this.m_Count == 1 && this.target != null)
                {
                    this.target.Frozen = false;
                    this.target.SolidHueOverride = -1;

                    if (Medusa.AffectedMobiles.Contains(this.target))
                        Medusa.AffectedMobiles.Remove(this.target);
                }
                else if (this.m_Count == 2 && this.clone != null)
                {
                    this.clone.SolidHueOverride = -1;
                    this.clone.Frozen = this.clone.Blessed = false;

                    foreach (Mobile m in this.clone.GetMobilesInRange(12))
                    {
                        if ((m != null) && (m.Player) && !(m is MedusaClone) && !(m is StoneMonster))
                        {
                            m.SendLocalizedMessage(1112767); // Medusa releases one of the petrified creatures!!
                            m.Send(new RemoveMobile(this.clone));
                            m.Send(new MobileIncoming(m, this.clone));
                            this.clone.Combatant = m;
                        }
                    }
                }
                else
                    this.Stop();
            }
        }
    }

    public class MedusaClone : BaseCreature, IFreezable
    {
        public MedusaClone(Mobile m)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.SolidHueOverride = 33;
            this.Clone(m);
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
                return !this.Frozen;
            }
        }
        public void Clone(Mobile m)
        {
            if (m == null)
            {
                this.Delete();
                return;
            }

            this.Body = m.Body;

            this.Str = m.Str;
            this.Dex = m.Dex;
            this.Int = m.Int;

            this.Hits = m.HitsMax;

            this.Hue = m.Hue;
            this.Female = m.Female;

            this.Name = m.Name;
            this.NameHue = m.NameHue;

            this.Title = m.Title;
            this.Kills = m.Kills;

            this.HairItemID = m.HairItemID;
            this.HairHue = m.HairHue;

            this.FacialHairItemID = m.FacialHairItemID;
            this.FacialHairHue = m.FacialHairHue;

            this.BaseSoundID = m.BaseSoundID;

            for (int i = 0; i < m.Skills.Length; ++i)
            {
                this.Skills[i].Base = m.Skills[i].Base;
                this.Skills[i].Cap = m.Skills[i].Cap;
            }

            for (int i = 0; i < m.Items.Count; i++)
            {
                if (m.Items[i].Layer != Layer.Backpack && m.Items[i].Layer != Layer.Mount && m.Items[i].Layer != Layer.Bank)
                    this.AddItem(this.CloneItem(m.Items[i]));
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
            if (this.Frozen)
                this.DisplayPaperdollTo(from);
            else
                base.OnDoubleClick(from);
        }

        public void OnRequestedAnimation(Mobile from)
        {
            if (this.Frozen)
            {
                //if (Core.SA)
                //from.Send(new UpdateStatueAnimationSA(this, 31, 5));
                //else
                from.Send(new UpdateStatueAnimation(this, 1, 31, 5));
            }
        }

        public override void OnDelete()
        {
            Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 10, 15, 5042);

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
            this.Delete();
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