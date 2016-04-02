using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wailing banshee corpse")]
    public class WailingBanshee : BaseCreature
    {
        [Constructable]
        public WailingBanshee()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a wailing banshee";
            this.Body = 310;
            this.BaseSoundID = 0x482;

            this.SetStr(126, 150);
            this.SetDex(76, 100);
            this.SetInt(86, 110);

            this.SetHits(76, 90);

            this.SetDamage(10, 14);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 60);
            this.SetDamageType(ResistanceType.Poison, 20);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 70.1, 95.0);
            this.SetSkill(SkillName.Tactics, 45.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 70.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 19;
        }

        public WailingBanshee(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.MortalStrike;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
        }
        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (1.0 > Utility.RandomDouble() && reg.Name == "The Lands of the Lich")
            {
                if (Utility.RandomDouble() < 0.6)
                    c.DropItem(new EssenceDirection());
            }
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