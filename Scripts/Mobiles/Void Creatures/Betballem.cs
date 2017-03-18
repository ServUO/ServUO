using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a betballem corpse")]
    public class Betballem : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.Killing; } }
        public override int Stage { get { return 1; } }

        [Constructable]
        public Betballem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a betballem";
            this.Body = 776;
            this.Hue = 2071;
            this.BaseSoundID = 357;

            this.SetStr(270);
            this.SetDex(890);
            this.SetInt(80);

            this.SetHits(90, 100);
            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Fire, 100);

            this.SetSkill(SkillName.MagicResist, 40.0, 50.0);
            this.SetSkill(SkillName.Tactics, 20.1, 30.0);
            this.SetSkill(SkillName.Wrestling, 30.1, 40.0);
            this.SetSkill(SkillName.Anatomy, 0.0, 10.0);

            this.Fame = 500;
            this.Karma = -500;

            this.VirtualArmor = 38;

            this.AddItem(new LightSource());

            this.PackItem(new FertileDirt(Utility.RandomMinMax(1, 4)));
            this.PackItem(new DaemonBone(5));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.10)
            {
                c.DropItem(new AncientPotteryFragments());
            }
        }

        public Betballem(Serial serial)
            : base(serial)
        {
        }

        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }

        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.Gems);
        }

        public override int GetIdleSound()
        {
            return 338;
        }

        public override int GetAngerSound()
        {
            return 338;
        }

        public override int GetDeathSound()
        {
            return 338;
        }

        public override int GetAttackSound()
        {
            return 406;
        }

        public override int GetHurtSound()
        {
            return 194;
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