using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a savage corpse")]
    public class SavageRider : BaseCreature
    {
        [Constructable]
        public SavageRider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4)
        {
            this.Name = NameList.RandomName("savage rider");

            if (this.Female = Utility.RandomBool())
                this.Body = 186;
            else
                this.Body = 185;

            this.SetStr(151, 170);
            this.SetDex(92, 130);
            this.SetInt(51, 65);

            this.SetDamage(29, 34);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetSkill(SkillName.Fencing, 72.5, 95.0);
            this.SetSkill(SkillName.Healing, 60.3, 90.0);
            this.SetSkill(SkillName.Macing, 72.5, 95.0);
            this.SetSkill(SkillName.Poisoning, 60.0, 82.5);
            this.SetSkill(SkillName.MagicResist, 72.5, 95.0);
            this.SetSkill(SkillName.Swords, 72.5, 95.0);
            this.SetSkill(SkillName.Tactics, 72.5, 95.0);

            this.Fame = 1000;
            this.Karma = -1000;

            this.PackItem(new Bandage(Utility.RandomMinMax(1, 15)));

            if (0.1 > Utility.RandomDouble())
                this.PackItem(new BolaBall());

            this.AddItem(new TribalSpear());
            this.AddItem(new BoneArms());
            this.AddItem(new BoneLegs());
            this.AddItem(new BearMask());

            new SavageRidgeback().Rider = this;
        }

        public SavageRider(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.SavagesAndOrcs;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
