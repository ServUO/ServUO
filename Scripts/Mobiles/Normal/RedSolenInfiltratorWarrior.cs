using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a solen infiltrator corpse")]
    public class RedSolenInfiltratorWarrior : BaseCreature, IRedSolen
    {
        [Constructable]
        public RedSolenInfiltratorWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a red solen infiltrator";
            this.Body = 782;
            this.BaseSoundID = 959;

            this.SetStr(206, 230);
            this.SetDex(121, 145);
            this.SetInt(66, 90);

            this.SetHits(96, 107);

            this.SetDamage(5, 15);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Poison, 20);

            this.SetResistance(ResistanceType.Physical, 20, 35);
            this.SetResistance(ResistanceType.Fire, 20, 35);
            this.SetResistance(ResistanceType.Cold, 10, 25);
            this.SetResistance(ResistanceType.Poison, 20, 35);
            this.SetResistance(ResistanceType.Energy, 10, 25);

            this.SetSkill(SkillName.MagicResist, 80.0);
            this.SetSkill(SkillName.Tactics, 80.0);
            this.SetSkill(SkillName.Wrestling, 80.0);

            this.Fame = 3000;
            this.Karma = -3000;

            this.VirtualArmor = 40;

            SolenHelper.PackPicnicBasket(this);

            this.PackItem(new ZoogiFungus((0.05 < Utility.RandomDouble()) ? 3 : 13));
        }

        public RedSolenInfiltratorWarrior(Serial serial)
            : base(serial)
        {
        }

        public override int GetAngerSound()
        {
            return 0xB5;
        }

        public override int GetIdleSound()
        {
            return 0xB5;
        }

        public override int GetAttackSound()
        {
            return 0x289;
        }

        public override int GetHurtSound()
        {
            return 0xBC;
        }

        public override int GetDeathSound()
        {
            return 0xE4;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
            this.AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 4));
        }

        public override bool IsEnemy(Mobile m)
        {
            if (SolenHelper.CheckRedFriendship(m))
                return false;
            else
                return base.IsEnemy(m);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            SolenHelper.OnRedDamage(from);

            base.OnDamage(amount, from, willKill);
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
