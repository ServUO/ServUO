using System;
using Server;
using Server.Items;
using Server.Engines.MyrmidexInvasion;

namespace Server.Mobiles
{
    public class BritannianInfantry : BaseCreature
    {
        [Constructable]
        public BritannianInfantry()
            : base(AIType.AI_Melee, FightMode.Enemy, 10, 1, .15, .3)
        {
            SpeechHue = Utility.RandomDyedHue();

            Body = 0x190;
            Name = NameList.RandomName("male");

            SetStr(115, 150);
            SetDex(150);
            SetInt(25, 44);

            SetDamage(12, 17);

            SetHits(2400);
            SetMana(250);

            SetResistance(ResistanceType.Physical, 20);
            SetResistance(ResistanceType.Fire, 20);
            SetResistance(ResistanceType.Cold, 20);
            SetResistance(ResistanceType.Poison, 20);
            SetResistance(ResistanceType.Energy, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Tactics, 120);
            SetSkill(SkillName.Anatomy, 120);
            SetSkill(SkillName.Swords, 120);
            SetSkill(SkillName.Fencing, 120);
            SetSkill(SkillName.Macing, 120);

            SetWearable(new PlateChest(), -1, 1.0);
            SetWearable(new PlateLegs(), -1, 1.0);
            SetWearable(new PlateArms(), -1, 1.0);
            SetWearable(new PlateGloves(), -1, 1.0);
            SetWearable(new PlateHelm(), -1, 1.0);

            switch (Utility.Random(5))
            {
                case 0: SetWearable(new Halberd(), -1, 1.0); break;
                case 1: SetWearable(new Bardiche(), -1, 1.0); break;
                case 2: SetWearable(new Spear(), -1, 1.0); break;
                case 3: SetWearable(new ShortSpear(), -1, 1.0); break;
                case 4: SetWearable(new WarHammer(), -1, 1.0); break;
            }

            PackGold(Utility.RandomMinMax(250, 300));

            Fame = 7500;
            Karma = 4500;
        }

        public override bool IsEnemy(Mobile m)
        {
            if (MyrmidexInvasionSystem.Active && MyrmidexInvasionSystem.IsAlliedWithEodonTribes(m))
                return false;

            if (MyrmidexInvasionSystem.Active && MyrmidexInvasionSystem.IsAlliedWithMyrmidex(m))
                return true;

            return base.IsEnemy(m);
        }

        public override bool AutoRearms { get { return true; } }
        public override bool CanHeal { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
        }	

        public override WeaponAbility GetWeaponAbility()
        {
            BaseWeapon wep = Weapon as BaseWeapon;

            if (wep != null && !(wep is Fists))
            {
                if (Utility.RandomDouble() > 0.5)
                    return wep.PrimaryAbility;

                return wep.SecondaryAbility;
            }

            return null;
        }

        public BritannianInfantry(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
	
}