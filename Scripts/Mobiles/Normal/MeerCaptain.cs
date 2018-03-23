using System;
using System.Collections;
using Server.Items;
using Server.Spells;

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
            this.Name = "a meer captain";
            this.Body = 773;

            this.SetStr(96, 110);
            this.SetDex(186, 200);
            this.SetInt(96, 110);

            this.SetHits(58, 66);

            this.SetDamage(5, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 35, 45);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.Archery, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 91.0, 100.0);
            this.SetSkill(SkillName.Swords, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 91.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.9, 89.9);

            this.Fame = 2000;
            this.Karma = 5000;

            this.VirtualArmor = 28;

            Container pack = new Backpack();

            pack.DropItem(new Bolt(Utility.RandomMinMax(10, 20)));
            pack.DropItem(new Bolt(Utility.RandomMinMax(10, 20)));

            switch ( Utility.Random(6) )
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

            Container bag = new Bag();

            int count = Utility.RandomMinMax(10, 20);

            for (int i = 0; i < count; ++i)
            {
                Item item = Loot.RandomReagent();

                if (item == null)
                    continue;

                if (!bag.TryDropItem(this, item, false))
                    item.Delete();
            }

            pack.DropItem(bag);

            this.AddItem(new Crossbow());
            this.PackItem(pack);

            this.m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
        }

        public MeerCaptain(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
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
            if (this.Combatant != null && this.MagicDamageAbsorb < 1)
            {
                this.MagicDamageAbsorb = Utility.RandomMinMax(5, 7);
                this.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                this.PlaySound(0x1E9);
            }

            if (DateTime.UtcNow >= this.m_NextAbilityTime)
            {
                this.m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));

                ArrayList list = new ArrayList();
                IPooledEnumerable eable = GetMobilesInRange(8);

                foreach (Mobile m in eable)
                {
                    if (m is MeerWarrior && this.IsFriend(m) && this.CanBeBeneficial(m) && m.Hits < m.HitsMax && !m.Poisoned && !MortalStrike.IsWounded(m))
                        list.Add(m);
                }
                eable.Free();

                for (int i = 0; i < list.Count; ++i)
                {
                    Mobile m = (Mobile)list[i];

                    this.DoBeneficial(m);

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}