using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Crystal Lattice Seeker corpse")]
    public class CrystalLatticeSeeker : BaseCreature
    {
        [Constructable]
        public CrystalLatticeSeeker()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Crystal Lattice Seeker";
            this.Body = 0x7B;
            this.Hue = 0x47E;

            this.SetStr(550, 850);
            this.SetDex(190, 250);
            this.SetInt(350, 450);

            this.SetHits(350, 550);

            this.SetDamage(13, 19);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 50.0, 75.0);
            this.SetSkill(SkillName.EvalInt, 90.0, 100.0);
            this.SetSkill(SkillName.Magery, 100.0, 100.0);
            this.SetSkill(SkillName.Meditation, 90.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 90.0, 100.0);
            this.SetSkill(SkillName.Tactics, 90.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.0, 100.0);

            this.Fame = 17000;
            this.Karma = -17000;

            for (int i = 0; i < Utility.RandomMinMax(0, 2); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public CrystalLatticeSeeker(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.75 )
            c.DropItem( new CrystallineFragments() );

            if ( Utility.RandomDouble() < 0.07 )
            c.DropItem( new PiecesOfCrystal() );
        }

        public override int Feathers
        {
            get
            {
                return 100;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 4);
            AddLoot( LootPack.Parrot );
            this.AddLoot(LootPack.Gems);
            this.AddLoot(LootPack.HighScrolls, 2);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.1)
                this.Drain(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Utility.RandomDouble() < 0.1)
                this.Drain(attacker);
        }

        public virtual void Drain(Mobile m)
        {
            int toDrain;

            switch ( Utility.Random(3) )
            {
                case 0:
                    {
                        this.Say(1042156); // I can grant life, and I can sap it as easily.
                        this.PlaySound(0x1E6);

                        toDrain = Utility.RandomMinMax(3, 6);
                        this.Hits += toDrain;
                        m.Hits -= toDrain;
                        break;
                    }
                case 1:
                    {
                        this.Say(1042157); // You'll go nowhere, unless I deem it should be so.
                        this.PlaySound(0x1DF);

                        toDrain = Utility.RandomMinMax(10, 25);
                        this.Stam += toDrain;
                        m.Stam -= toDrain;
                        break;
                    }
                case 2:
                    {
                        this.Say(1042155); // Your power is mine to use as I will.
                        this.PlaySound(0x1F8);

                        toDrain = Utility.RandomMinMax(15, 25);
                        this.Mana += toDrain;
                        m.Mana -= toDrain;
                        break;
                    }
            }
        }

        public override int GetAttackSound()
        {
            return 0x2F6;
        }

        public override int GetDeathSound()
        {
            return 0x2F7;
        }

        public override int GetAngerSound()
        {
            return 0x2F8;
        }

        public override int GetHurtSound()
        {
            return 0x2F9;
        }

        public override int GetIdleSound()
        {
            return 0x2FA;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}