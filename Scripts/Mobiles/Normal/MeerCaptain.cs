using Server.Items;
using Server.Spells;
using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a meer corpse")]
    public class MeerCaptain : BaseCreature
    {
        private DateTime m_NextAbilityTime;
        [Constructable]
        public MeerCaptain()
            : base(AIType.AI_Paladin, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            Name = "a meer captain";
            Body = 773;

            SetStr(96, 110);
            SetDex(186, 200);
            SetInt(96, 110);

            SetHits(58, 66);

            SetDamage(5, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.Archery, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 91.0, 100.0);
            SetSkill(SkillName.Swords, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 91.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.9, 89.9);

            Fame = 2000;
            Karma = 5000;

            AddItem(new Crossbow());

            m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
        }

        public MeerCaptain(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;
        public override bool InitialInnocent => true;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LootItem<Bolt>(10, 20));
            AddLoot(LootPack.LootItem<Arrow>(10, 20));
            AddLoot(LootPack.LootItemCallback(DropWeapons));
            AddLoot(LootPack.LootItemCallback(DropRegs));
        }

        private static Item DropWeapons(IEntity e)
        {
            var pack = new Backpack();

            switch (Utility.Random(6))
            {
                case 0:
                    pack.DropItem(new Broadsword());
                    break;
                case 1:
                    pack.DropItem(new Cutlass());
                    break;
                case 2:
                    pack.DropItem(new Katana());
                    break;
                case 3:
                    pack.DropItem(new Longsword());
                    break;
                case 4:
                    pack.DropItem(new Scimitar());
                    break;
                case 5:
                    pack.DropItem(new VikingSword());
                    break;
            }

            return pack;
        }

        private static Item DropRegs(IEntity e)
        {
            var bag = new Bag();

            int count = Utility.RandomMinMax(10, 20);

            for (int i = 0; i < count; ++i)
            {
                bag.DropItemStacked(Loot.RandomReagent());
            }

            return bag;
        }

        public override int GetHurtSound()
        {
            return 0x14D;
        }

        public override int GetDeathSound()
        {
            return 0x314;
        }

        public override int GetAttackSound()
        {
            return 0x75;
        }

        public override void OnThink()
        {
            if (Combatant != null && MagicDamageAbsorb < 1)
            {
                MagicDamageAbsorb = Utility.RandomMinMax(5, 7);
                FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                PlaySound(0x1E9);
            }

            if (DateTime.UtcNow >= m_NextAbilityTime)
            {
                m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));

                ArrayList list = new ArrayList();
                IPooledEnumerable eable = GetMobilesInRange(8);

                foreach (Mobile m in eable)
                {
                    if (m is MeerWarrior && IsFriend(m) && CanBeBeneficial(m) && m.Hits < m.HitsMax && !m.Poisoned && !MortalStrike.IsWounded(m))
                        list.Add(m);
                }
                eable.Free();

                for (int i = 0; i < list.Count; ++i)
                {
                    Mobile m = (Mobile)list[i];

                    DoBeneficial(m);

                    int toHeal = Utility.RandomMinMax(20, 30);

                    SpellHelper.Turn(this, m);

                    m.Heal(toHeal, this);

                    m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                    m.PlaySound(0x202);
                }
            }

            base.OnThink();
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
