using Server.Gumps;
using System;

namespace Server.Mobiles
{
    [CorpseName("an ethereal warrior corpse")]
    public class EtherealWarrior : BaseCreature
    {
        private static readonly TimeSpan ResurrectDelay = TimeSpan.FromSeconds(2.0);
        private DateTime m_NextResurrect;
        [Constructable]
        public EtherealWarrior()
            : base(AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("ethereal warrior");
            Body = 123;

            SetStr(586, 785);
            SetDex(177, 255);
            SetInt(351, 450);

            SetHits(352, 471);

            SetDamage(13, 19);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Anatomy, 50.1, 75.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 99.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);

            Fame = 7000;
            Karma = 7000;
        }

        public EtherealWarrior(Serial serial)
            : base(serial)
        {
        }

        public override bool InitialInnocent => true;
        public override int TreasureMapLevel => 5;

        public override TribeType Tribe => TribeType.Fey;

        public override int Feathers => 100;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.Gems);
        }

        public override void OnMovement(Mobile from, Point3D oldLocation)
        {
            if (!from.Alive && (from is PlayerMobile))
            {
                if (!from.Frozen && (DateTime.UtcNow >= m_NextResurrect) && InRange(from, 4) && !InRange(oldLocation, 4) && InLOS(from))
                {
                    m_NextResurrect = DateTime.UtcNow + ResurrectDelay;
                    if (!from.Criminal && (from.Kills < 5) && (from.Karma > 0))
                    {
                        if (from.Map != null && from.Map.CanFit(from.Location, 16, false, false))
                        {
                            Direction = GetDirectionTo(from);
                            from.PlaySound(0x1F2);
                            from.FixedEffect(0x376A, 10, 16);
                            from.CloseGump(typeof(ResurrectGump));
                            from.SendGump(new ResurrectGump(from, ResurrectMessage.Healer));
                        }
                    }
                }
            }
        }

        public override int GetAngerSound()
        {
            return 0x2F8;
        }

        public override int GetIdleSound()
        {
            return 0x2F8;
        }

        public override int GetAttackSound()
        {
            return Utility.Random(0x2F5, 2);
        }

        public override int GetHurtSound()
        {
            return 0x2F9;
        }

        public override int GetDeathSound()
        {
            return 0x2F7;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.25 > Utility.RandomDouble())
            {
                int toSap = Utility.RandomMinMax(20, 30);

                switch (Utility.Random(3))
                {
                    case 0:
                        defender.Damage(toSap, this);
                        Hits += toSap;
                        break;
                    case 1:
                        defender.Stam -= toSap;
                        Stam += toSap;
                        break;
                    case 2:
                        defender.Mana -= toSap;
                        Mana += toSap;
                        break;
                }
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
