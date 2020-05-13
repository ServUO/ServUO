using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gargoyle corpse")]
    public class GargoyleDestroyer : BaseCreature
    {
        [Constructable]
        public GargoyleDestroyer()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Gargoyle Destroyer";
            Body = 0x2F3;
            BaseSoundID = 0x174;

            SetStr(760, 850);
            SetDex(102, 150);
            SetInt(152, 200);

            SetHits(482, 485);

            SetDamage(7, 14);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.Wrestling, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 120.4, 160.0);
            SetSkill(SkillName.Anatomy, 50.5, 100.0);
            SetSkill(SkillName.Swords, 90.1, 100.0);
            SetSkill(SkillName.Macing, 90.1, 100.0);
            SetSkill(SkillName.Fencing, 90.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);

            Fame = 10000;
            Karma = -10000;
        }

        public GargoyleDestroyer(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override bool CanFly => true;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.LootItem<GargoylesPickaxe>(20.0));
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            if (from != null && from.Alive && 0.4 > Utility.RandomDouble())
            {
                ThrowHatchet(from);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (attacker != null && attacker.Alive && attacker.Weapon is BaseRanged && 0.4 > Utility.RandomDouble())
            {
                ThrowHatchet(attacker);
            }
        }

        public void ThrowHatchet(Mobile to)
        {
            int damage = 50;
            MovingEffect(to, 0xF43, 10, 0, false, false);
            DoHarmful(to);
            AOS.Damage(to, this, damage, 100, 0, 0, 0, 0);
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
