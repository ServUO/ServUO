using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a demon knight corpse")]
    public class DemonKnight : BaseCreature
    {
        private bool m_InHere;

        [Constructable]
        public DemonKnight()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("demon knight");
            Title = "the Dark Father";
            Body = 318;
            BaseSoundID = 0x165;

            SetStr(500);
            SetDex(100);
            SetInt(1000);

            SetHits(30000);
            SetMana(5000);

            SetDamage(17, 21);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.Wrestling, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);

            Fame = 28000;
            Karma = -28000;

            SetWeaponAbility(WeaponAbility.CrushingBlow);
            SetWeaponAbility(WeaponAbility.WhirlwindAttack);

            ForceActiveSpeed = 0.38;
            ForcePassiveSpeed = 0.66;
        }

        public DemonKnight(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFlee => false;

        public override bool IgnoreYoungProtection => true;
        public override bool BardImmune => false;
        public override bool Unprovokable => true;
        public override bool AreaPeaceImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 5;

        public override void OnDeath(Container c)
        {
            List<DamageStore> rights = GetLootingRights();

            int top = 0;
            Item blood = null;

            foreach (Mobile m in rights.Select(x => x.m_Mobile).Distinct().Take(3))
            {
                if (top == 0)
                    blood = new BloodOfTheDarkFather(5);
                else if (top == 1)
                    blood = new BloodOfTheDarkFather(3);
                else if (top == 2)
                    blood = new BloodOfTheDarkFather(2);

                top++;

                if (m.Backpack == null || !m.Backpack.TryDropItem(m, blood, false))
                {
                    m.BankBox.DropItem(blood);
                }
            }

            base.OnDeath(c);
        }

        public override bool TeleportsTo => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
            AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(6, 60));
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && from != this && !m_InHere)
            {
                m_InHere = true;
                AOS.Damage(from, this, Utility.RandomMinMax(8, 20), 100, 0, 0, 0, 0);

                MovingEffect(from, 0xECA, 10, 0, false, false, 0, 0);
                PlaySound(0x491);

                if (0.05 > Utility.RandomDouble())
                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(CreateBones_Callback), from);

                m_InHere = false;
            }
        }

        public virtual void CreateBones_Callback(object state)
        {
            Mobile from = (Mobile)state;
            Map map = from.Map;

            if (map == null)
                return;

            int count = Utility.RandomMinMax(1, 3);

            for (int i = 0; i < count; ++i)
            {
                int x = from.X + Utility.RandomMinMax(-1, 1);
                int y = from.Y + Utility.RandomMinMax(-1, 1);
                int z = from.Z;

                if (!map.CanFit(x, y, z, 16, false, true))
                {
                    z = map.GetAverageZ(x, y);

                    if (z == from.Z || !map.CanFit(x, y, z, 16, false, true))
                        continue;
                }

                UnholyBone bone = new UnholyBone
                {
                    Hue = 0,
                    Name = "unholy bones",
                    ItemID = Utility.Random(0xECA, 9)
                };

                bone.MoveToWorld(new Point3D(x, y, z), map);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
