using Server.Engines.MyrmidexInvasion;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class BritannianInfantry : BaseCreature
    {
        public override double HealChance => 1.0;

        [Constructable]
        public BritannianInfantry()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .15, .3)
        {
            SpeechHue = Utility.RandomDyedHue();

            Body = 0x190;
            Hue = 33779;
            Name = NameList.RandomName("male");
            Title = "the Britannian";

            SetStr(115, 150);
            SetDex(150);
            SetInt(25, 44);

            SetDamage(12, 17);

            SetHits(2400);
            SetMana(250);

            SetResistance(ResistanceType.Physical, 25);
            SetResistance(ResistanceType.Fire, 15);
            SetResistance(ResistanceType.Cold, 10);
            SetResistance(ResistanceType.Poison, 15);
            SetResistance(ResistanceType.Energy, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Tactics, 120);
            SetSkill(SkillName.Anatomy, 120);
            SetSkill(SkillName.Swords, 120);
            SetSkill(SkillName.Fencing, 120);
            SetSkill(SkillName.Macing, 120);

            AddImmovableItem(new PlateChest());
            AddImmovableItem(new PlateLegs());
            AddImmovableItem(new PlateArms());
            AddImmovableItem(new PlateGloves());
            AddImmovableItem(new PlateGorget());
            AddImmovableItem(new Kilt(1175));
            AddImmovableItem(new BodySash(1157));
            AddImmovableItem(new Halberd());

            Fame = 7500;
            Karma = 4500;

            Utility.AssignRandomHair(this);
        }

        private void AddImmovableItem(Item item)
        {
            item.LootType = LootType.Blessed;
            SetWearable(item);
        }

        public override bool IsEnemy(Mobile m)
        {
            if (MyrmidexInvasionSystem.Active && MyrmidexInvasionSystem.IsAlliedWithEodonTribes(m))
                return false;

            if (MyrmidexInvasionSystem.Active && MyrmidexInvasionSystem.IsAlliedWithMyrmidex(m))
                return true;

            return base.IsEnemy(m);
        }

        public override bool AlwaysAttackable => Region.IsPartOf<BattleRegion>();
        public override bool ShowFameTitle => false;
        public override bool ClickTitle => false;

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
