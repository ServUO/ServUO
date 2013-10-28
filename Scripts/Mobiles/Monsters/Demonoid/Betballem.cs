using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a betballem corpse")]
    public class Betballem : BaseCreature
    {
        [Constructable]
        public Betballem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Betballem";
            this.Body = 776;
            this.Hue = 2071;
            this.BaseSoundID = 357;

            this.SetStr(16, 40);
            this.SetDex(31, 60);
            this.SetInt(11, 25);

            this.SetHits(5010, 5250);

            this.SetDamage(21, 25);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 5, 10);

            this.SetSkill(SkillName.MagicResist, 10.0);
            this.SetSkill(SkillName.Tactics, 0.1, 15.0);
            this.SetSkill(SkillName.Wrestling, 25.1, 40.0);

            this.Fame = 500;
            this.Karma = -500;

            this.VirtualArmor = 38;

            this.AddItem(new LightSource());

            this.PackItem(new FertileDirt(Utility.RandomMinMax(1, 4)));
            this.PackItem(new DaemonBone(5)); // TODO: Five small iron ore
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
        public override bool AlwaysMurderer
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

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new VoidOrb());

            if (Utility.RandomDouble() < 0.10)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        this.AddToBackpack(new VoidEssence());
                        break;
                    case 1:
                        this.AddToBackpack(new AncientPotteryFragments());
                        break;
                }
            }
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