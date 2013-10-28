using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a juka corpse")] // Why is this 'juka' and warriors 'jukan' ? :-(
    public class JukaLord : BaseCreature
    {
        [Constructable]
        public JukaLord()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 3, 0.2, 0.4)
        {
            this.Name = "a juka lord";
            this.Body = 766;

            this.SetStr(401, 500);
            this.SetDex(81, 100);
            this.SetInt(151, 200);

            this.SetHits(241, 300);

            this.SetDamage(10, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 45, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 20, 25);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 90.1, 100.0);
            this.SetSkill(SkillName.Archery, 95.1, 100.0);
            this.SetSkill(SkillName.Healing, 80.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 120.1, 130.0);
            this.SetSkill(SkillName.Swords, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 95.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 28;

            Container pack = new Backpack();

            pack.DropItem(new Arrow(Utility.RandomMinMax(25, 35)));
            pack.DropItem(new Arrow(Utility.RandomMinMax(25, 35)));
            pack.DropItem(new Bandage(Utility.RandomMinMax(5, 15)));
            pack.DropItem(new Bandage(Utility.RandomMinMax(5, 15)));
            pack.DropItem(Loot.RandomGem());
            pack.DropItem(new ArcaneGem());

            this.PackItem(pack);

            this.AddItem(new JukaBow());
            // TODO: Bandage self
        }

        public JukaLord(Serial serial)
            : base(serial)
        {
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
                return !Core.AOS;
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && !willKill && amount > 5 && from.Player && 5 > Utility.Random(100))
            {
                string[] toSay = new string[]
                {
                    "{0}!!  You will have to do better than that!",
                    "{0}!!  Prepare to meet your doom!",
                    "{0}!!  My armies will crush you!",
                    "{0}!!  You will pay for that!"
                };

                this.Say(true, String.Format(toSay[Utility.Random(toSay.Length)], from.Name));
            }

            base.OnDamage(amount, from, willKill);
        }

        public override int GetIdleSound()
        {
            return 0x262;
        }

        public override int GetAngerSound()
        {
            return 0x263;
        }

        public override int GetHurtSound()
        {
            return 0x1D0;
        }

        public override int GetDeathSound()
        {
            return 0x28D;
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