using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vorpal bunny corpse")]
    public class VorpalBunny : BaseCreature
    {
        [Constructable]
        public VorpalBunny()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.06, 0.1)
        {
            Name = "a vorpal bunny";
            Body = 205;
            Hue = 0x480;

            SetStr(15);
            SetDex(2000);
            SetInt(1000);

            SetHits(2000);
            SetStam(500);
            SetMana(0);

            SetDamage(1);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 200.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);

            Fame = 1000;
            Karma = 0;

            VirtualArmor = 4;

            int carrots = Utility.RandomMinMax(5, 10);
            PackItem(new Carrot(carrots));

            if (Utility.Random(5) == 0)
                PackItem(new BrightlyColoredEggs());

            PackStatue();

            DelayBeginTunnel();
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
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich, 2);
        }

        public override IDamageable Combatant
        {
            get { return base.Combatant; }
            set
            {
                base.Combatant = value;

                if (0.10 > Utility.RandomDouble())
                    StopFlee();
                else if (!CheckFlee())
                    BeginFlee(TimeSpan.FromSeconds(10));
            }
        }

        public virtual void DelayBeginTunnel()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(3.0), new TimerCallback(BeginTunnel));
        }

        public virtual void BeginTunnel()
        {
            if (Deleted)
                return;

            new BunnyHole().MoveToWorld(Location, Map);

            Frozen = true;
            Say("* The bunny begins to dig a tunnel back to its underground lair *");
            PlaySound(0x247);

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

            DelayBeginTunnel();
        }

        public class BunnyHole : Item
        {
            public BunnyHole()
                : base(0x913)
            {
                Movable = false;
                Hue = 1;
                Name = "a mysterious rabbit hole";

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

                Delete();
            }
        }
    }
}