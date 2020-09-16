using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class VeriteElemental : BaseCreature
    {
        [Constructable]
        public VeriteElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a verite elemental";
            Body = 113;
            BaseSoundID = 268;

            SetStr(226, 255);
            SetDex(126, 145);
            SetInt(71, 92);

            SetHits(136, 153);

            SetDamage(9, 16);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 3500;
            Karma = -3500;
        }

        public VeriteElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel => true;
        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 1;

        public static void OnHit(Mobile defender, Item item, int damage)
        {
            IWearableDurability dur = item as IWearableDurability;

            if (dur == null || dur.MaxHitPoints == 0 || item.LootType == LootType.Blessed || item.Insured)
            {
                return;
            }

            if (damage < 10)
                damage = 10;

            if (dur.HitPoints > 0)
            {
                dur.HitPoints = Math.Max(0, dur.HitPoints - damage);
            }
            else
            {
                defender.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                dur.MaxHitPoints = Math.Max(0, dur.MaxHitPoints - damage);

                if (!item.Deleted && dur.MaxHitPoints == 0)
                {
                    item.Delete();
                }
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.LootItem<VeriteOre>(25));
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
