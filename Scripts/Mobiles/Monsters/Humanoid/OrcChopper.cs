using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class OrcChopper : BaseCreature
    {
        [Constructable]
        public OrcChopper()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Orc Chopper";
            this.Body = 7;
            this.BaseSoundID = 0x45A;
            this.Hue = 0x96D;

            this.SetStr(147, 245);
            this.SetDex(101, 135);
            this.SetInt(86, 110);

            this.SetHits(97, 139);

            this.SetDamage(4, 13);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 25, 30);

            this.SetSkill(SkillName.MagicResist, 60.1, 85.0);
            this.SetSkill(SkillName.Swords, 75.1, 90.0);
            this.SetSkill(SkillName.Tactics, 60.1, 85.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 54;

            this.PackItem(new Log(Utility.RandomMinMax(1, 10)));
            this.PackItem(new Board(Utility.RandomMinMax(10, 20)));
            this.PackItem(new ExecutionersAxe());

            // TODO: Skull?
            switch (Utility.Random(7))
            {
                case 0:
                    this.PackItem(new Arrow());
                    break;
                case 1:
                    this.PackItem(new Lockpick());
                    break;
                case 2:
                    this.PackItem(new Shaft());
                    break;
                case 3:
                    this.PackItem(new Ribs());
                    break;
                case 4:
                    this.PackItem(new Bandage());
                    break;
                case 5:
                    this.PackItem(new BeverageBottle(BeverageType.Wine));
                    break;
                case 6:
                    this.PackItem(new Jug(BeverageType.Cider));
                    break;
            }

            if (Core.AOS)
                this.PackItem(Loot.RandomNecromancyReagent());
        }

        public OrcChopper(Serial serial)
            : base(serial)
        {
        }

        public override InhumanSpeech SpeechType
        {
            get
            {
                return InhumanSpeech.Orc;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.SavagesAndOrcs;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            switch (Utility.Random(2))
            {
                default:
                case 1:
                    return WeaponAbility.WhirlwindAttack;
                case 2:
                    return WeaponAbility.CrushingBlow;
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new DoubleAxe());

            if (Core.ML)
            {
                if (Utility.RandomDouble() < 0.05)
                    c.DropItem(new StoutWhip());
            }

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new EvilOrcHelm());
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager, 2);
        }

        public override bool IsEnemy(Mobile m)
        {
            if (m.Player && m.FindItemOnLayer(Layer.Helm) is OrcishKinMask)
                return false;

            return base.IsEnemy(m);
        }

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            Item item = aggressor.FindItemOnLayer(Layer.Helm);

            if (item is OrcishKinMask)
            {
                AOS.Damage(aggressor, 50, 0, 100, 0, 0, 0);
                item.Delete();
                aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                aggressor.PlaySound(0x307);
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