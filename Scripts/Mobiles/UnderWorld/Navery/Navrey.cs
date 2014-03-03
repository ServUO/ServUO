using System;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an spider corpse")]
    public class Navrey : BaseCreature
    {
        private static readonly Type[] m_Artifact = new Type[]
        {
            typeof(NightEyes),
            typeof(Tangle),
            typeof(BladeOfBattle),
            typeof(DemonBridleRing),
            typeof(GiantSteps),
            typeof(StormCaller),
            typeof(SwordOfShatteredHopes),
            typeof(SummonersKilt),
            typeof(TokenOfHolyFavor),
            typeof(Venom),
            typeof(BreastplateOfTheBerserker),
        };
        [Constructable]
        public Navrey()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Navrey Night-Eyes";
            this.Body = 735;
            this.BaseSoundID = 389;

            this.SetStr(1000, 1500);
            this.SetDex(200, 250);
            this.SetInt(150, 200);

            this.SetHits(30000, 35000);

            this.SetDamage(25, 40);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 45, 55);
            this.SetResistance(ResistanceType.Cold, 50, 70);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 60, 80);

            this.SetSkill(SkillName.Anatomy, 50.0, 80.0);
            this.SetSkill(SkillName.EvalInt, 90.0, 100.0);
            this.SetSkill(SkillName.Magery, 90.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 100.0, 130.0);
            this.SetSkill(SkillName.Meditation, 80.0, 100.0);
            this.SetSkill(SkillName.Poisoning, 100.0);
            this.SetSkill(SkillName.Tactics, 90.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 91.6, 98.2);

            this.Fame = 30000;
            this.Karma = -30000;

            this.VirtualArmor = 90;
        }

        public Navrey(Serial serial)
            : base(serial)
        {
        }
 
 
 
 
        public override bool AlwaysMurderer
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
                return Poison.Parasitic;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public static void DistributeRandomArtifact(BaseCreature bc, Type[] typelist)
        {
            int random = Utility.Random(typelist.Length);
            Item item = Loot.Construct(typelist[random]);
            DistributeArtifact(DemonKnight.FindRandomPlayer(bc), item);
        }

        public static void DistributeArtifact(Mobile to, Item artifact)
        {
            if (to == null || artifact == null)
                return;

            Container pack = to.Backpack;

            if (pack == null || !pack.TryDropItem(to, artifact, false))
                to.BankBox.DropItem(artifact);

            to.SendLocalizedMessage(502088); // A special gift has been placed in your backpack.
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomBool())
                c.AddItem(ScrollofTranscendence.CreateRandom(30, 30));

            if (Utility.RandomBool())
                c.AddItem(new TatteredAncientScroll());

            if (Utility.RandomBool())
                c.AddItem(new UntransTome());

            if (Utility.RandomBool())
                c.AddItem(new SpiderCarapace());

            if (Utility.RandomDouble() < 0.01)
                DistributeRandomArtifact(this, m_Artifact);

            // distribute quest items for the 'Green with Envy' quest given by Vernix
            List<DamageStore> rights = BaseCreature.GetLootingRights(this.DamageEntries, this.HitsMax);
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
            if (Utility.RandomDouble() < 0.03)
                this.DoSpecialAbility();
        }

        // override so Navrey will ignore players paralyzed in webs
        public override bool CanSee(object o)
        {
            if (o is Mobile && ((Mobile)o).Paralyzed && Utility.RandomDouble() > 0.25)
                return false;

            return base.CanSee(o);
        }

        public void DoSpecialAbility()
        {
            // build target list
            List<Mobile> mlist = new List<Mobile>();
            foreach (Mobile mob in this.Map.GetMobilesInRange(this.Location, this.RangePerception))
            {
                if (null != mob && !mob.Deleted && !mob.Paralyzed && AccessLevel.Player == mob.AccessLevel)
                    mlist.Add(mob);
            }

            // pick a random target and sling the web
            if (0 != mlist.Count)
            {
                int i = Utility.Random(mlist.Count);
                Mobile m = mlist.ToArray()[i];
                this.Direction = this.GetDirectionTo(m);
                Item web = new NavreyParalyzingWeb();
                if (Utility.RandomDouble() > 0.1)
                    m.Paralyze(TimeSpan.FromSeconds(60));
                web.MoveToWorld(m.Location, this.Map);
            }
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
        }
    }
}