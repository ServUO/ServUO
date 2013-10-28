using System;
using Server.Items;

namespace Server.Mobiles
{
    public class KhaldunSummoner : BaseCreature
    {
        [Constructable]
        public KhaldunSummoner()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Body = 0x190;
            this.Name = "Zealot of Khaldun";
            this.Title = "the Summoner";

            this.SetStr(351, 400);
            this.SetDex(101, 150);
            this.SetInt(502, 700);

            this.SetHits(421, 480);

            this.SetDamage(5, 15);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Cold, 25);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.EvalInt, 100.0);
            this.SetSkill(SkillName.Meditation, 120.1, 130.0);

            this.VirtualArmor = 36;
            this.Fame = 10000;
            this.Karma = -10000;

            LeatherGloves gloves = new LeatherGloves();
            gloves.Hue = 0x66D;
            this.AddItem(gloves);

            BoneHelm helm = new BoneHelm();
            helm.Hue = 0x835;
            this.AddItem(helm);

            Necklace necklace = new Necklace();
            necklace.Hue = 0x66D;
            this.AddItem(necklace);

            Cloak cloak = new Cloak();
            cloak.Hue = 0x66D;
            this.AddItem(cloak);

            Kilt kilt = new Kilt();
            kilt.Hue = 0x66D;
            this.AddItem(kilt);

            Sandals sandals = new Sandals();
            sandals.Hue = 0x66D;
            this.AddItem(sandals);
        }

        public KhaldunSummoner(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override int GetIdleSound()
        {
            return 0x184;
        }

        public override int GetAngerSound()
        {
            return 0x286;
        }

        public override int GetDeathSound()
        {
            return 0x288;
        }

        public override int GetHurtSound()
        {
            return 0x19F;
        }

        public override bool OnBeforeDeath()
        {
            BoneMagi rm = new BoneMagi();
            rm.Team = this.Team;
            rm.Combatant = this.Combatant;
            rm.NoKillAwards = true;

            if (rm.Backpack == null)
            {
                Backpack pack = new Backpack();
                pack.Movable = false;
                rm.AddItem(pack);
            }

            for (int i = 0; i < 2; i++)
            {
                LootPack.FilthyRich.Generate(this, rm.Backpack, true, LootPack.GetLuckChanceForKiller(this));
                LootPack.FilthyRich.Generate(this, rm.Backpack, false, LootPack.GetLuckChanceForKiller(this));
            }

            Effects.PlaySound(this, this.Map, this.GetDeathSound());
            Effects.SendLocationEffect(this.Location, this.Map, 0x3709, 30, 10, 0x835, 0);
            rm.MoveToWorld(this.Location, this.Map);

            this.Delete();
            return false;
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