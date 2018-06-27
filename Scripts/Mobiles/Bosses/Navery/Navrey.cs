using System;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a navrey corpse")]
    public class Navrey : BaseCreature
    {
        private NavreysController m_Spawner;
        private bool m_UsedPillars;
        private DateTime m_Delay;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UsedPillars
        {
            get { return m_UsedPillars; }
            set { m_UsedPillars = value; }
        }

        private static readonly Type[] m_Artifact = new Type[]
        {
            typeof(NightEyes),
            typeof(Tangle1)
        };		
        
        [Constructable]
        public Navrey(NavreysController spawner)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Spawner = spawner;

            Name = "Navrey Night-Eyes";
            Body = 735;
            BaseSoundID = 389;

            SetStr(1000, 1500);
            SetDex(200, 250);
            SetInt(150, 200);

            SetHits(30000, 35000);

            SetDamage(25, 40);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 25);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 65, 80);

            SetSkill(SkillName.Anatomy, 50.0, 80.0);
            SetSkill(SkillName.EvalInt, 90.0, 100.0);
            SetSkill(SkillName.Magery, 90.0, 100.0);
            SetSkill(SkillName.MagicResist, 100.0, 130.0);
            SetSkill(SkillName.Meditation, 80.0, 100.0);
            SetSkill(SkillName.Poisoning, 100.0);
            SetSkill(SkillName.Tactics, 90.0, 100.0);
            SetSkill(SkillName.Wrestling, 91.6, 98.2);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 90;

            for (int i = 0; i < Utility.RandomMinMax(1, 3); i++)
            {
                PackItem(Loot.RandomScroll(0, Loot.MysticismScrollTypes.Length, SpellbookType.Mystic));
            }
        }

        public Navrey(Serial serial)
            : base(serial)
        {
        }
 
        public override double TeleportChance { get { return 0; } }
	    public override bool AlwaysMurderer { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Parasitic; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override int Meat { get { return 1; } }

        public static void DistributeRandomArtifact(BaseCreature bc, Type[] typelist)
        {
            int random = Utility.Random(typelist.Length);
            Item item = Loot.Construct(typelist[random]);
            DistributeArtifact(DemonKnight.FindRandomPlayer(bc), item);
        }

        public static void DistributeArtifact(Mobile to, Item artifact)
        {
            if (artifact == null)
                return;

            if (to != null)
            {
                Container pack = to.Backpack;

                if (pack == null || !pack.TryDropItem(to, artifact, false))
                    to.BankBox.DropItem(artifact);

                to.SendLocalizedMessage(502088); // A special gift has been placed in your backpack.
            }
            else
            {
                artifact.Delete();
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosSuperBoss, 3);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_Spawner != null)
                m_Spawner.OnNavreyKilled();

            if (Utility.RandomBool())
                c.AddItem(new UntranslatedAncientTome());

            if (0.1 >= Utility.RandomDouble())
                c.AddItem(ScrollofTranscendence.CreateRandom(30, 30));

            if (0.1 >= Utility.RandomDouble())
                c.AddItem(new TatteredAncientScroll());

            if (Utility.RandomDouble() < 0.10)
                c.DropItem(new LuckyCoin());

            if (Utility.RandomDouble() < 0.025)
                DistributeRandomArtifact(this, m_Artifact);

            // distribute quest items for the 'Green with Envy' quest given by Vernix
            List<DamageStore> rights = GetLootingRights();
            for (int i = rights.Count - 1; i >= 0; --i)
            {
                DamageStore ds = rights[i];
                if (!ds.m_HasRight)
                    rights.RemoveAt(i);
            }

            // for each with looting rights... give an eye of navrey if they have the quest
            foreach (DamageStore d in rights)
            {
                PlayerMobile pm = d.m_Mobile as PlayerMobile;
                if (null != pm)
                {
                    foreach (BaseQuest quest in pm.Quests)
                    {
                        if (quest is GreenWithEnvyQuest)
                        {
                            Container pack = pm.Backpack;
                            Item item = new EyeOfNavrey();
                            if (pack == null || !pack.TryDropItem(pm, item, false))
                                pm.BankBox.DropItem(item);
                            pm.SendLocalizedMessage(1095155); // As Navrey Night-Eyes dies, you find and claim one of her eyes as proof of her demise.
                            break;
                        }
                    }
                }
            }
        }

        public override void OnThink()
        {
			if (DateTime.UtcNow > m_Delay)
            {
                m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 30));
				if (Utility.RandomDouble() < 0.50)
                DoSpecialAbility();
			}
        }

        // override so Navrey will ignore players paralyzed in webs
        public override bool CanSee(object o)
        {
            if (o is Mobile && ((Mobile)o).Paralyzed && Utility.RandomDouble() > 0.25)
                return false;

            return base.CanSee(o);
        }

        private static Dictionary<Mobile, NavreyParalyzingWeb> m_Table = new Dictionary<Mobile, NavreyParalyzingWeb>();
        public static Dictionary<Mobile, NavreyParalyzingWeb> Table { get { return m_Table; } }

        public void DoSpecialAbility()
        {
            // build target list
            List<Mobile> mlist = new List<Mobile>();

            IPooledEnumerable eable = this.GetMobilesInRange(12);
            foreach (Mobile mob in eable)
            {
                if (mob == null || mob == this || !mob.Alive || mob.Hidden || !CanSee(mob)|| !CanBeHarmful(mob) || mob.AccessLevel > AccessLevel.Player)
                    continue;

                if (m_Table.ContainsKey(mob))
                    continue;

                if (mob.Player)
                    mlist.Add(mob);

                else if (mob is BaseCreature && (((BaseCreature)mob).Summoned || ((BaseCreature)mob).Controlled))
                    mlist.Add(mob);
            }
            eable.Free();

            // pick a random target and sling the web
            if (mlist.Count > 0)
            {
                Mobile m = mlist[Utility.Random(mlist.Count)];

                Direction = GetDirectionTo(m);
                TimeSpan duration = TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10));

                Item web = new NavreyParalyzingWeb(duration, m);

                Effects.SendMovingParticles(this, m, web.ItemID, 12, 0, false, false, 0, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Timer.DelayCall(TimeSpan.FromSeconds(0.5), new TimerStateCallback(ThrowWeb_Callback), new object[] { web, m, duration });

                Combatant = m;
                m_Table[m] = web as NavreyParalyzingWeb;
            }
        }

        public static void RemoveFromTable(Mobile from)
        {
            if (m_Table.ContainsKey(from))
                m_Table.Remove(from);

            BuffInfo.RemoveBuff(from, BuffIcon.Webbing);
        }

        public void ThrowWeb_Callback(object o)
        {
            object[] os = (object[])o;

            Item web = os[0] as Item;
            Mobile m = os[1] as Mobile;
            TimeSpan ts = (TimeSpan)os[2];

            if (m != null && web != null)
            {
                web.MoveToWorld(m.Location, this.Map);
                m.Freeze(ts);
                m.SendMessage("You've been caught in Navrey's Web!");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write((Item)m_Spawner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_Spawner = reader.ReadItem() as NavreysController;
        }
    }
}
