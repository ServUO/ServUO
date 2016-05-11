using System;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a corpse of Ilhenir")]
    public class Ilhenir : BaseChampion
    {
        private static Hashtable m_Table;
        private readonly DateTime m_NextDrop = DateTime.UtcNow;
        [Constructable]
        public Ilhenir()
            : base(AIType.AI_Mage)
        {
            this.Name = "Ilhenir";
            this.Title = "the Stained";
            this.Body = 0x103;

            this.BaseSoundID = 589;

            this.SetStr(1105, 1350);
            this.SetDex(82, 160);
            this.SetInt(505, 750);

            this.SetHits(9000);

            this.SetDamage(21, 28);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Poison, 20);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 55, 65);
            this.SetResistance(ResistanceType.Poison, 70, 90);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetSkill(SkillName.EvalInt, 100);
            this.SetSkill(SkillName.Magery, 100);
            this.SetSkill(SkillName.Meditation, 0);
            this.SetSkill(SkillName.Poisoning, 5.4);
            this.SetSkill(SkillName.Anatomy, 117.5);
            this.SetSkill(SkillName.MagicResist, 120.0);
            this.SetSkill(SkillName.Tactics, 119.9);
            this.SetSkill(SkillName.Wrestling, 119.9);

            this.Fame = 50000;
            this.Karma = -50000;

            this.VirtualArmor = 44;

            for (int i = 0; i < Utility.RandomMinMax(1, 3); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }

            if (Core.ML)
            {
                this.PackResources(8);
                this.PackTalismans(5);
            }
        }

        public Ilhenir(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Pain;
            }
        }
        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[]
                {
                    typeof(ANecromancerShroud),
                    typeof(LieutenantOfTheBritannianRoyalGuard),
                    typeof(OblivionsNeedle),
                    typeof(TheRobeOfBritanniaAri)
                };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { typeof(MonsterStatuette) };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[]
                {
                    MonsterStatuetteType.PlagueBeast,
                    MonsterStatuetteType.RedDeath
                };
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override bool Uncalmable
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
        //public override bool GivesMLMinorArtifact { get { return true; } } // TODO: Needs verification
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public static bool UnderCacophonicAttack(Mobile from)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            return m_Table[from] != null;
        }

        public virtual void PackResources(int amount)
        {
            for (int i = 0; i < amount; i++)
                switch( Utility.Random(6) )
                {
                    case 0:
                        this.PackItem(new Blight());
                        break;
                    case 1:
                        this.PackItem(new Scourge());
                        break;
                    case 2:
                        this.PackItem(new Taint());
                        break;
                    case 3:
                        this.PackItem(new Putrefication());
                        break;
                    case 4:
                        this.PackItem(new Corruption());
                        break;
                    case 5:
                        this.PackItem(new Muculent());
                        break;
                }
        }

        public virtual void PackItems(Item item, int amount)
        {
            for (int i = 0; i < amount; i++)
                this.PackItem(item);
        }

        public virtual void PackTalismans(int amount)
        {
            int count = Utility.Random(amount);

            for (int i = 0; i < count; i++)
                this.PackItem(new RandomTalisman());
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 8);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Core.ML)
            {
                c.DropItem(new GrizzledBones());

                // TODO: Parrots
                /*if ( Utility.RandomDouble() < 0.6 )
                c.DropItem( new ParrotItem() ); */

                if (Utility.RandomDouble() < 0.05)
                    c.DropItem(new GrizzledMareStatuette());

                if (Utility.RandomDouble() < 0.025)
                    c.DropItem(new CrimsonCincture());
                // TODO: Armor sets
                /*if ( Utility.RandomDouble() < 0.05 )
                {
                switch ( Utility.Random(5) )
                {
                case 0: c.DropItem( new GrizzleGauntlets() ); break;
                case 1: c.DropItem( new GrizzleGreaves() ); break;
                case 2: c.DropItem( new GrizzleHelm() ); break;
                case 3: c.DropItem( new GrizzleTunic() ); break;
                case 4: c.DropItem( new GrizzleVambraces() ); break;
                }
                }*/
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
                this.CacophonicAttack(defender);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Utility.RandomDouble() < 0.1)
                this.DropOoze();

            base.OnDamage(amount, from, willKill);
        }

        public override int GetAngerSound()
        {
            return 0x581;
        }

        public override int GetIdleSound()
        {
            return 0x582;
        }

        public override int GetAttackSound()
        {
            return 0x580;
        }

        public override int GetHurtSound()
        {
            return 0x583;
        }

        public override int GetDeathSound()
        {
            return 0x584;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public virtual void CacophonicAttack(Mobile to)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            if (to.Alive && to.Player && m_Table[to] == null)
            {
                to.Send(SpeedControl.WalkSpeed);
                to.SendLocalizedMessage(1072069); // A cacophonic sound lambastes you, suppressing your ability to move.
                to.PlaySound(0x584);

                m_Table[to] = Timer.DelayCall(TimeSpan.FromSeconds(30), new TimerStateCallback(EndCacophonic_Callback), to);
            }
        }

        public virtual void CacophonicEnd(Mobile from)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            m_Table[from] = null;

            from.Send(SpeedControl.Disable);
        }

        public virtual void DropOoze()
        {
            int amount = Utility.RandomMinMax(1, 3);
            bool corrosive = Utility.RandomBool();

            for (int i = 0; i < amount; i++)
            {
                Item ooze = new StainedOoze(corrosive);
                Point3D p = new Point3D(this.Location);

                for (int j = 0; j < 5; j++)
                {
                    p = this.GetSpawnPosition(2);
                    bool found = false;

                    foreach (Item item in this.Map.GetItemsInRange(p, 0))
                        if (item is StainedOoze)
                        {
                            found = true;
                            break;
                        }

                    if (!found)
                        break;
                }

                ooze.MoveToWorld(p, this.Map);
            }

            if (this.Combatant != null)
            {
                if (corrosive)
                    this.Combatant.SendLocalizedMessage(1072071); // A corrosive gas seeps out of your enemy's skin!
                else
                    this.Combatant.SendLocalizedMessage(1072072); // A poisonous gas seeps out of your enemy's skin!
            }
        }

        public virtual Point3D GetSpawnPosition(int range)
        {
            return this.GetSpawnPosition(this.Location, this.Map, range);
        }

        public virtual Point3D GetSpawnPosition(Point3D from, Map map, int range)
        {
            if (map == null)
                return from;

            Point3D loc = new Point3D((this.RandomPoint(this.X)), (this.RandomPoint(this.Y)), this.Z);

            loc.Z = this.Map.GetAverageZ(loc.X, loc.Y);

            return loc;
        }

        private void EndCacophonic_Callback(object state)
        {
            if (state is Mobile)
                this.CacophonicEnd((Mobile)state);
        }

        private int RandomPoint(int mid)
        {
            return (mid + Utility.RandomMinMax(-2, 2));
        }
    }

    public class StainedOoze : Item
    {
        private bool m_Corrosive;
        private Timer m_Timer;
        private int m_Ticks;
        [Constructable]
        public StainedOoze()
            : this(false)
        {
        }

        [Constructable]
        public StainedOoze(bool corrosive)
            : base(0x122A)
        {
            this.Movable = false;
            this.Hue = 0x95;

            this.m_Corrosive = corrosive;
            this.m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
            this.m_Ticks = 0;
        }

        public StainedOoze(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Corrosive
        {
            get
            {
                return this.m_Corrosive;
            }
            set
            {
                this.m_Corrosive = value;
            }
        }
        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
            {
                this.m_Timer.Stop();
                this.m_Timer = null;
            }
        }

        public void Damage(Mobile m)
        {
            if (this.m_Corrosive)
            {
                List<Item> items = m.Items;
                bool damaged = false;

                for (int i = 0; i < items.Count; ++i)
                {
                    IDurability wearable = items[i] as IDurability;

                    if (wearable != null && wearable.HitPoints >= 10 && Utility.RandomDouble() < 0.25)
                    {
                        wearable.HitPoints -= (wearable.HitPoints == 10) ? Utility.Random(1, 5) : 10;
                        damaged = true;
                    }
                }

                if (damaged)
                {
                    m.LocalOverheadMessage(MessageType.Regular, 0x21, 1072070); // The infernal ooze scorches you, setting you and your equipment ablaze!
                    return;
                }
            }

            AOS.Damage(m, 40, 0, 0, 0, 100, 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Corrosive);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Corrosive = reader.ReadBool();

            this.m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
            this.m_Ticks = (this.ItemID == 0x122A) ? 0 : 30;
        }

        private void OnTick()
        {
            List<Mobile> toDamage = new List<Mobile>();

            foreach (Mobile m in this.GetMobilesInRange(0))
            {
                if (m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;

                    if (!bc.Controlled && !bc.Summoned)
                        continue;
                }
                else if (!m.Player)
                {
                    continue;
                }

                if (m.Alive && !m.IsDeadBondedPet && m.CanBeDamaged())
                    toDamage.Add(m);
            }

            for (int i = 0; i < toDamage.Count; ++i)
                this.Damage(toDamage[i]);

            ++this.m_Ticks;

            if (this.m_Ticks >= 35)
                this.Delete();
            else if (this.m_Ticks == 30)
                this.ItemID = 0x122B;
        }
    }
}