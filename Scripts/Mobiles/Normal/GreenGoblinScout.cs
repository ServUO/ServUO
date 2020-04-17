#region References
using Server.Items;
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
    [CorpseName("a goblin corpse")]
    public class GreenGoblinScout : BaseCreature
    {
        [Constructable]
        public GreenGoblinScout()
            : base(AIType.AI_OrcScout, FightMode.Closest, 10, 7, 0.2, 0.4)
        {
            Name = "a green goblin scout";
            Body = 723;
            BaseSoundID = 0x600;

            SetStr(276, 309);
            SetDex(65, 79);
            SetInt(107, 146);

            SetHits(174, 198);
            SetMana(107, 146);
            SetStam(65, 79);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 41, 49);
            SetResistance(ResistanceType.Fire, 33, 39);
            SetResistance(ResistanceType.Cold, 26, 33);
            SetResistance(ResistanceType.Poison, 14, 20);
            SetResistance(ResistanceType.Energy, 11, 20);

            SetSkill(SkillName.MagicResist, 90.7, 98.8);
            SetSkill(SkillName.Tactics, 80.9, 86.3);
            SetSkill(SkillName.Wrestling, 107.7, 119.5);
            SetSkill(SkillName.Anatomy, 80.3, 88.2);

            Fame = 1500;
            Karma = -1500;
        }

        public GreenGoblinScout(Serial serial)
            : base(serial)
        { }

        public override int GetAngerSound() { return 0x600; }
        public override int GetIdleSound() { return 0x600; }
        public override int GetAttackSound() { return 0x5FD; }
        public override int GetHurtSound() { return 0x5FF; }
        public override int GetDeathSound() { return 0x5FE; }

        public override bool CanRummageCorpses => true;
        public override int TreasureMapLevel => 1;
        public override int Meat => 1;
        public override TribeType Tribe => TribeType.GreenGoblin;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.01)
                c.DropItem(new LuckyCoin());
        }

        public override void OnThink()
        {
            if (Utility.RandomDouble() < 0.2)
            {
                TryToDetectHidden();
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

        private Mobile FindTarget()
        {
            IPooledEnumerable eable = GetMobilesInRange(10);
            foreach (Mobile m in eable)
            {
                if (m.Player && m.Hidden && m.IsPlayer())
                {
                    eable.Free();
                    return m;
                }
            }

            eable.Free();
            return null;
        }

        private void TryToDetectHidden()
        {
            Mobile m = FindTarget();

            if (m != null)
            {
                if (Core.TickCount >= NextSkillTime && UseSkill(SkillName.DetectHidden))
                {
                    Target targ = Target;

                    if (targ != null)
                    {
                        targ.Invoke(this, this);
                    }

                    Effects.PlaySound(Location, Map, 0x340);
                }
            }
        }
    }
}
