using System;
using Server.Items;

namespace Server.Mobiles
{
    public class KhaldunZealot : BaseCreature
    {
        [Constructable]
        public KhaldunZealot()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Body = 0x190;
            this.Name = "Zealot of Khaldun";
            this.Title = "the Knight";
            this.Hue = 0;

            this.SetStr(351, 400);
            this.SetDex(151, 165);
            this.SetInt(76, 100);

            this.SetHits(448, 470);

            this.SetDamage(15, 25);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Cold, 25);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.Wrestling, 70.1, 80.0);
            this.SetSkill(SkillName.Swords, 120.1, 130.0);
            this.SetSkill(SkillName.Anatomy, 120.1, 130.0);
            this.SetSkill(SkillName.MagicResist, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);

            this.Fame = 10000;
            this.Karma = -10000;
            this.VirtualArmor = 40;

            VikingSword weapon = new VikingSword();
            weapon.Hue = 0x835;
            weapon.Movable = false;
            this.AddItem(weapon);

            MetalShield shield = new MetalShield();
            shield.Hue = 0x835;
            shield.Movable = false;
            this.AddItem(shield);

            BoneHelm helm = new BoneHelm();
            helm.Hue = 0x835;
            this.AddItem(helm);

            BoneArms arms = new BoneArms();
            arms.Hue = 0x835;
            this.AddItem(arms);

            BoneGloves gloves = new BoneGloves();
            gloves.Hue = 0x835;
            this.AddItem(gloves);

            BoneChest tunic = new BoneChest();
            tunic.Hue = 0x835;
            this.AddItem(tunic);

            BoneLegs legs = new BoneLegs();
            legs.Hue = 0x835;
            this.AddItem(legs);

            this.AddItem(new Boots());
        }

        public KhaldunZealot(Serial serial)
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
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
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
            BoneKnight rm = new BoneKnight();
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