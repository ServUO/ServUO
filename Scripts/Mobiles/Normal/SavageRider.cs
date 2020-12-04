using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a savage corpse")]
    public class SavageRider : BaseCreature
    {
        [Constructable]
        public SavageRider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4)
        {
            Name = NameList.RandomName("savage rider");

            Body = 185;

            SetStr(151, 170);
            SetDex(92, 130);
            SetInt(51, 65);

            SetDamage(29, 34);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.Fencing, 72.5, 95.0);
            SetSkill(SkillName.Healing, 60.3, 90.0);
            SetSkill(SkillName.Macing, 72.5, 95.0);
            SetSkill(SkillName.Poisoning, 60.0, 82.5);
            SetSkill(SkillName.MagicResist, 72.5, 95.0);
            SetSkill(SkillName.Swords, 72.5, 95.0);
            SetSkill(SkillName.Tactics, 72.5, 95.0);

            Fame = 1000;
            Karma = -1000;

            AddItem(new TribalSpear());
            AddItem(new BoneArms());
            AddItem(new BoneLegs());
            AddItem(new BearMask());

            new SavageRidgeback().Rider = this;
        }

        public SavageRider(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override bool AlwaysMurderer => true;
        public override bool ShowFameTitle => false;

        public override TribeType Tribe => TribeType.Savage;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LootItem<BolaBall>(10.0));
            AddLoot(LootPack.LootItem<Bandage>(1, 15, true));
            AddLoot(LootPack.LootItem<SeveredHumanEars>(75.0, 1));
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = Mount;

            if (mount != null)
                mount.Rider = null;

            if (mount is Mobile)
                ((Mobile)mount).Delete();

            return base.OnBeforeDeath();
        }

        public override bool IsEnemy(Mobile m)
        {
            if (m.BodyMod == 183 || m.BodyMod == 184)
                return false;

            return base.IsEnemy(m);
        }

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            if (aggressor.BodyMod == 183 || aggressor.BodyMod == 184)
            {
                AOS.Damage(aggressor, 50, 0, 100, 0, 0, 0);
                aggressor.BodyMod = 0;
                aggressor.HueMod = -1;
                aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                aggressor.PlaySound(0x307);
                aggressor.SendLocalizedMessage(1040008); // Your skin is scorched as the tribal paint burns away!

                if (aggressor is PlayerMobile)
                    ((PlayerMobile)aggressor).SavagePaintExpiration = TimeSpan.Zero;
            }
        }

        public override void AlterMeleeDamageTo(Mobile to, ref int damage)
        {
            if (to is Dragon || to is WhiteWyrm || to is SwampDragon || to is Drake || to is Nightmare || to is Hiryu || to is LesserHiryu || to is Daemon)
                damage *= 3;
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
