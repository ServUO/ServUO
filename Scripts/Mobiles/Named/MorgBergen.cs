using System;
using Server.Items;

namespace Server.Mobiles
{
    public class MorgBergen : BaseCreature
    {
        [Constructable]
        public MorgBergen()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Title = "the Cursed";

            this.Hue = 0x8596;
            this.Body = 0x190;
            this.Name = "Morg Bergen";

            this.AddItem(new ShortPants(0x59C));

            Bardiche bardiche = new Bardiche();
            LeatherGloves gloves = new LeatherGloves();
            LeatherArms arms = new LeatherArms();

            bardiche.Hue = 0x96F;
            bardiche.Movable = false;
            gloves.Hue = 0x96F;
            arms.Hue = 0x96F;

            this.AddItem(bardiche);
            this.AddItem(gloves);
            this.AddItem(arms);

            this.SetStr(111, 120);
            this.SetDex(111, 120);
            this.SetInt(51, 60);

            this.SetHits(180, 207);
            this.SetMana(0);

            this.SetDamage(9, 17);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Cold, 60);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.Swords, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 80.1, 90.0);
            this.SetSkill(SkillName.Anatomy, 90.1, 100.0);

            this.Fame = 5000;
            this.Karma = -1000;
        }

        public MorgBergen(Serial serial)
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
        public override bool DeleteCorpseOnDeath
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
        public override int GetIdleSound()
        {
            return 0x1CE;
        }

        public override int GetAngerSound()
        {
            return 0x263;
        }

        public override int GetDeathSound()
        {
            return 0x1D1;
        }

        public override int GetHurtSound()
        {
            return 0x25E;
        }

        public override bool OnBeforeDeath()
        {
            Gold gold = new Gold(Utility.RandomMinMax(190, 230));
            gold.MoveToWorld(this.Location, this.Map);

            Effects.SendLocationEffect(this.Location, this.Map, 0x376A, 10, 1);
            return true;
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