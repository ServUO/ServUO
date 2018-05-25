using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class DullCopperElemental : BaseCreature
    {
        [Constructable]
        public DullCopperElemental()
            : this(2)
        {
        }

        [Constructable]
        public DullCopperElemental(int oreAmount)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dull copper elemental";
            Body = 110;
            BaseSoundID = 268;

            SetStr(226, 255);
            SetDex(126, 145);
            SetInt(71, 92);

            SetHits(136, 153);

            SetDamage(9, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 20;

            Item ore = new DullCopperOre(oreAmount);
            ore.ItemID = 0x19B9;
            PackItem(ore);
        }

        public DullCopperElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }

        public override bool OnBeforeDeath()
        {
            if (Map == null)
                return base.OnBeforeDeath();

            FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
            PlaySound(0x307);

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 4);
            var list = new System.Collections.Generic.List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m != this && m.Alive && m.AccessLevel == AccessLevel.Player && 
                    (m is PlayerMobile || (m is BaseCreature && !((BaseCreature)m).IsMonster)))
                {
                    list.Add(m);
                }
            }

            foreach (var m in list)
            {
                Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(.5), mob =>
                    {
                        mob.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                        mob.PlaySound(0x307);
                        AOS.Damage(mob, this, Utility.RandomMinMax(25, 50), 50, 50, 0, 0, 0);
                    }, m);
            }

            ColUtility.Free(list);

            return base.OnBeforeDeath();
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, 2);
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