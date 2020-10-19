using Server.Engines.Quests;
using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a navrey corpse")]
    public class Navrey : BaseCreature
    {
        private NavreysController m_Spawner;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UsedPillars { get; set; }

        private static readonly Type[] m_Artifact =
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

            SetSpecialAbility(SpecialAbility.Webbing);
        }

        public Navrey(Serial serial)
            : base(serial)
        {
        }

        public override double TeleportChance => 0;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Parasitic;
        public override Poison HitPoison => Poison.Lethal;
        public override int Meat => 1;

        public static void DistributeRandomArtifact(BaseCreature bc, Type[] typelist)
        {
            int random = Utility.Random(typelist.Length);
            Item item = Loot.Construct(typelist[random]);
            DistributeArtifact(bc.RandomPlayerWithLootingRights(), item);
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
            AddLoot(LootPack.SuperBoss, 3);
            AddLoot(LootPack.MysticScrolls, 1, 3);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_Spawner != null)
                m_Spawner.OnNavreyKilled();

            if (Utility.RandomBool())
                c.AddItem(new UntranslatedAncientTome());

            if (0.1 >= Utility.RandomDouble())
                c.AddItem(ScrollOfTranscendence.CreateRandom(30, 30));

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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(m_Spawner);
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
