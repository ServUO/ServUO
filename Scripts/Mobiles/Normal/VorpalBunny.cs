using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vorpal bunny corpse")]
    public class VorpalBunny : BaseCreature
    {
        [Constructable]
        public VorpalBunny()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a vorpal bunny";
            this.Body = 205;
            this.Hue = 0x480;

            this.SetStr(15);
            this.SetDex(2000);
            this.SetInt(1000);

            this.SetHits(2000);
            this.SetStam(500);
            this.SetMana(0);

            this.SetDamage(1);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetSkill(SkillName.MagicResist, 200.0);
            this.SetSkill(SkillName.Tactics, 5.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.Fame = 1000;
            this.Karma = 0;

            this.VirtualArmor = 4;

            int carrots = Utility.RandomMinMax(5, 10);
            this.PackItem(new Carrot(carrots));

            if (Utility.Random(5) == 0)
                this.PackItem(new BrightlyColoredEggs());

            this.PackStatue();

            this.DelayBeginTunnel();
        }

        public VorpalBunny(Serial serial)
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
        public override int Hides
        {
            get
            {
                return 1;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich, 2);
        }

        public virtual void DelayBeginTunnel()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(3.0), new TimerCallback(BeginTunnel));
        }

        public virtual void BeginTunnel()
        {
            if (this.Deleted)
                return;

            new BunnyHole().MoveToWorld(this.Location, this.Map);

            this.Frozen = true;
            this.Say("* The bunny begins to dig a tunnel back to its underground lair *");
            this.PlaySound(0x247);

            Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));
        }

        public override int GetAttackSound()
        {
            return 0xC9;
        }

        public override int GetHurtSound()
        {
            return 0xCA;
        }

        public override int GetDeathSound()
        {
            return 0xCB;
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

            this.DelayBeginTunnel();
        }

        public class BunnyHole : Item
        {
            public BunnyHole()
                : base(0x913)
            {
                this.Movable = false;
                this.Hue = 1;
                this.Name = "a mysterious rabbit hole";

                Timer.DelayCall(TimeSpan.FromSeconds(40.0), new TimerCallback(Delete));
            }

            public BunnyHole(Serial serial)
                : base(serial)
            {
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

                this.Delete();
            }
        }
    }
}