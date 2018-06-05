using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a slith corpse")]
    public class StoneSlith : BaseCreature
    {
        [Constructable]
        public StoneSlith()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a stone slith";
            Body = 734;

            SetStr(250, 300);
            SetDex(76, 90);
            SetInt(34, 69);

            SetHits(154, 166);
            SetStam(76, 90);
            SetMana(34, 69);

            SetDamage(6, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 55);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 86.8, 95.1);
            SetSkill(SkillName.Tactics, 82.6, 88.6);
            SetSkill(SkillName.Wrestling, 75.8, 87.4);
            SetSkill(SkillName.Anatomy, 0.0, 2.9);

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 65.1;
            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.GraspingClaw);
            SetSpecialAbility(SpecialAbility.TailSwipe);
        }

        public StoneSlith(Serial serial)
            : base(serial)
        {
        }

        public override int DragonBlood { get { return 6; } }
        public override bool HasBreath
        {
            get { return true; }
        } // fire breath enabled

        public override int Meat
        {
            get { return 1; }
        }

        //public override int DragonBlood{ get{ return 6; } }
        public override int Hides
        {
            get { return 12; }
        }

        public override HideType HideType
        {
            get { return HideType.Spined; }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            
            if (!Controlled && Utility.RandomDouble() <= 0.005)
            {
                c.DropItem(new StoneSlithClaw());
            }

            if (!Controlled && Utility.RandomDouble() < 0.05)
            {
                c.DropItem(new SlithEye());
            }

            if (!Controlled && Utility.RandomDouble() < 0.25)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new AncientPotteryFragments());
                        break;
                    case 1:
                        c.DropItem(new TatteredAncientScroll());
                        break;
                }
            }        
        }     

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();

            if (version == 0)
            {
                SetSpecialAbility(SpecialAbility.GraspingClaw);
                SetWeaponAbility(WeaponAbility.BleedAttack);
            }
        }
    }
}
