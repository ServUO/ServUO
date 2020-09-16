#region References
using Server.Items;
using System;
#endregion

namespace Server.Mobiles
{
    [CorpseName("an inhuman corpse")]
    public class CultistAmbusher : BaseCreature
    {
        [Constructable]
        public CultistAmbusher()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Cultist Ambusher";
            Body = 0x190;
            Hue = 2500;
            BaseSoundID = 0x45A;

            SetStr(150, 200);
            SetDex(150);
            SetInt(25, 44);

            SetHits(500, 1000);

            SetDamage(8, 18);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 10, 20);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Fencing, 100.0, 120.0);
            SetSkill(SkillName.Macing, 100.0, 120.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Swords, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Archery, 100.0, 120.0);
            SetSkill(SkillName.Parry, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);

            Fame = 8000;
            Karma = -8000;

            switch (Utility.Random(3))
            {
                case 0:
                    {
                        SetWearable(new RingmailChest(), 1510);
                        SetWearable(new ChainLegs(), 1345);
                        SetWearable(new Sandals(), 1345);
                        SetWearable(new LeatherNinjaHood(), 1345);
                        SetWearable(new LeatherGloves(), 1345);
                        SetWearable(new LeatherArms(), 1345);
                        break;
                    }
                case 1:
                    {
                        SetWearable(new Robe(2306));
                        SetWearable(new BearMask(2683));
                        break;
                    }
                case 2:
                    {
                        SetWearable(new Shirt(676));
                        SetWearable(new RingmailLegs());
                        SetWearable(new StuddedArms());
                        SetWearable(new StuddedGloves());
                        break;
                    }
                case 3:
                    {
                        SetWearable(new SkullCap(2406));
                        SetWearable(new JinBaori(1001));
                        SetWearable(new Shirt());
                        SetWearable(new ShortPants(902));
                        break;
                    }
            }

            switch (Utility.Random(2))
            {
                case 0:
                    {
                        SetWearable(Loot.Construct(new Type[] { typeof(Kryss), typeof(Spear), typeof(ShortSpear), typeof(Lance), typeof(Pike), typeof(WarMace), typeof(Mace), typeof(WarHammer), typeof(WarAxe) }));

                        break;
                    }
                case 1:
                    {
                        SetWearable(Loot.Construct(new Type[] { typeof(Yumi), typeof(Crossbow), typeof(RepeatingCrossbow), typeof(HeavyCrossbow) }));

                        RangeFight = 7;
                        AI = AIType.AI_Archer;

                        break;
                    }
            }
        }

        public override void OnBeforeDamage(Mobile from, ref int totalDamage, DamageType type)
        {
            if (Region.IsPartOf("Khaldun") && IsChampionSpawn && !Caddellite.CheckDamage(from, type))
            {
                totalDamage = 0;
            }

            base.OnBeforeDamage(from, ref totalDamage, type);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Map != null && AI == AIType.AI_Archer && 0.4 >= Utility.RandomDouble())
            {
                Point3D p = FindLocation(Map, Location, 10);
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                Location = p;
                Effects.SendLocationParticles(EffectItem.Create(p, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                PlaySound(0x1FE);
            }
        }

        private Point3D FindLocation(Map map, Point3D center, int range)
        {
            int cx = center.X;
            int cy = center.Y;

            for (int i = 0; i < 20; ++i)
            {
                int x = cx + Utility.Random(range * 2) - range;
                int y = cy + Utility.Random(range * 2) - range;

                if ((cx - x) * (cx - x) + (cy - y) * (cy - y) > range * range)
                    continue;

                int z = map.GetAverageZ(x, y);

                if (!map.CanFit(x, y, z, 6, false, false))
                    continue;

                int topZ = z;

                foreach (Item item in map.GetItemsInRange(new Point3D(x, y, z), 0))
                {
                    topZ = Math.Max(topZ, item.Z + item.ItemData.CalcHeight);
                }

                return new Point3D(x, y, topZ);
            }

            return center;
        }

        public override bool AlwaysMurderer => true;
        public override bool ShowFameTitle => false;

        public CultistAmbusher(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            BaseWeapon wep = Weapon as BaseWeapon;

            if (wep != null && !(wep is Fists))
            {
                if (Utility.RandomDouble() > 0.5)
                    return wep.PrimaryAbility;

                return wep.SecondaryAbility;
            }

            return null;
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
