using System;
using Server;
using Server.Items;
using System.Collections.Generic;
using Server.Spells.SkillMasteries;
using Server.Spells;
using System.Linq;
using Server.Engines.MyrmidexInvasion;

namespace Server.Mobiles
{
	public enum EodonTribe
	{
		Jukari,
		Kurak,
		Barrab,
		Barako,
		Urali,
		Sakkhra
	}
	
	public abstract class BaseEodonTribesman : BaseCreature
	{
        protected long _NextMastery;
        private bool _HasYelled;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasYelled 
        {
            get { return _HasYelled; }
            set
            {
                if (value != _HasYelled)
                {
                    _HasYelled = value;

                    if (_HasYelled)
                        Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(180, 360)), () => _HasYelled = false);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
		public EodonTribe TribeType { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public override Poison PoisonImmune 
        { 
            get 
            {
                if (TribeType == EodonTribe.Barrab)
                    return Poison.Deadly;

                return null;
            } 
        }

        public override bool InitialInnocent { get { return true; } }

        public override MasteryInfo[] Masteries
        {
            get
            {
                BaseWeapon wep = Weapon as BaseWeapon;

                if (wep == null)
                    return null;

                var infos = MasteryInfo.Infos.Where(i => i.MasterySkill == wep.DefSkill && !i.Passive).ToArray();

                if (infos != null && infos.Length > 0)
                {
                    return infos;
                }

                return null;
            }
            set { }
        }

        private void AddImmovableItem(Item item)
        {
            item.LootType = LootType.Blessed;
            SetWearable(item);
        }

		public BaseEodonTribesman(AIType ai, EodonTribe type) : base(ai, FightMode.Closest, 10, 1, .2, .4)
		{
			TribeType = type;
			
            BuildBody();
			BuildEquipment();

            switch (type)
            {
                case EodonTribe.Jukari:
                    SetResistance(ResistanceType.Physical, 20, 30);
                    SetResistance(ResistanceType.Fire, 100);
                    SetResistance(ResistanceType.Cold, 10, 20);
                    SetResistance(ResistanceType.Poison, 40, 50);
                    SetResistance(ResistanceType.Energy, 40, 50);
                    break;
                case EodonTribe.Kurak:
                    SetResistance(ResistanceType.Physical, 20, 30);
                    SetResistance(ResistanceType.Fire, 10, 20);
                    SetResistance(ResistanceType.Cold, 100);
                    SetResistance(ResistanceType.Poison, 40, 50);
                    SetResistance(ResistanceType.Energy, 40, 50);
                    break;
                case EodonTribe.Barrab:
                    SetResistance(ResistanceType.Physical, 20, 30);
                    SetResistance(ResistanceType.Fire, 40, 50);
                    SetResistance(ResistanceType.Cold, 40, 50);
                    SetResistance(ResistanceType.Poison, 100);
                    SetResistance(ResistanceType.Energy, 10, 20);
                    break;
                case EodonTribe.Barako:
                    SetResistance(ResistanceType.Physical, 20, 30);
                    SetResistance(ResistanceType.Fire, 40, 50);
                    SetResistance(ResistanceType.Cold, 40, 50);
                    SetResistance(ResistanceType.Poison, 10, 20);
                    SetResistance(ResistanceType.Energy, 40, 50);
                    break;
                case EodonTribe.Urali:
                    SetResistance(ResistanceType.Physical, 20, 30);
                    SetResistance(ResistanceType.Fire, 40, 50);
                    SetResistance(ResistanceType.Cold, 40, 50);
                    SetResistance(ResistanceType.Poison, 10, 20);
                    SetResistance(ResistanceType.Energy, 100);
                    break;
                case EodonTribe.Sakkhra:
                    SetResistance(ResistanceType.Physical, 100);
                    SetResistance(ResistanceType.Fire, 40, 50);
                    SetResistance(ResistanceType.Cold, 40, 50);
                    SetResistance(ResistanceType.Poison, 60, 70);
                    SetResistance(ResistanceType.Energy, 40, 50);
                    break;
            }
		}
		
        public abstract void BuildBody();
        public abstract void BuildEquipment();
		
        private static string[] _Names;

        public static void Initialize()
        {
            NameList list1 = NameList.GetNameList("savage");
            NameList list2 = NameList.GetNameList("savage rider");
            NameList list3 = NameList.GetNameList("savage shaman");

            List<string> names = new List<string>(list1.List);
            names.AddRange(list2.List);
            names.AddRange(list3.List);

            _Names = names.ToArray();
        }
		
        public static string GetRandomName()
        {
            return _Names[Utility.Random(_Names.Length)];
        }

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            if (this.Map == null)
                return;

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, this.RangePerception);

            foreach (Mobile m in eable)
            {
                if (m != this && m != aggressor && m is BaseEodonTribesman && ((BaseEodonTribesman)m).TribeType == this.TribeType && m.Combatant == null)
                {
                    BaseEodonTribesman tribesman = m as BaseEodonTribesman;
                    m.Warmode = true;
                    m.Combatant = aggressor;

                    if (!tribesman.HasYelled)
                    {
                        m.PublicOverheadMessage(Server.Network.MessageType.Regular, 0x47E, 1156584); // Ahhhh-OOOO! Ahhh-OOOO!
                        tribesman.HasYelled = true;
                    }
                }
            }
        }

        public override bool IsEnemy(Mobile m)
        {
            // Basically, this makes them FightMode.Agressor. More can can be added in to make htem attack others, such as other tribes, etc.
            bool valid = this.Aggressors.FirstOrDefault(a => a.Attacker == m) != null;

            if (!valid && MyrmidexInvasionSystem.Active)
                valid = MyrmidexInvasionSystem.AreEnemies(this, m);

            return valid;
        }

        public override WeaponAbility GetWeaponAbility()
        {
            BaseWeapon wep = Weapon as BaseWeapon;

            if (wep == null)
                return null;

            if (Utility.RandomBool())
                return wep.PrimaryAbility;

            return wep.SecondaryAbility;
        } 

		public BaseEodonTribesman(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			writer.Write((int)TribeType);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			TribeType = (EodonTribe)reader.ReadInt();
		}
	}
	
	public class TribeWarrior : BaseEodonTribesman
	{
		[Constructable]
		public TribeWarrior(EodonTribe type) : base(AIType.AI_Melee, type)
		{
		}
		
		public override void BuildBody()
		{
			Name = String.Format("{0} the {1} warrior", GetRandomName(), TribeType.ToString());

			SetStr(150);
			SetDex(150);
			SetInt(75);
			
			SetHits(2500);
			
			SetDamage(10, 23);
			
			SetDamageType(ResistanceType.Physical, 100);
			
			SetSkill(SkillName.Wrestling, 100, 120);
            SetSkill(SkillName.Fencing, 100, 120);
            SetSkill(SkillName.Swords, 100, 120);
            SetSkill(SkillName.Macing, 100, 120);
            SetSkill(SkillName.Archery, 100, 120);

            SetSkill(SkillName.Tactics, 100, 120);
            SetSkill(SkillName.Anatomy, 100, 120);
            SetSkill(SkillName.MagicResist, 100, 120);
            SetSkill(SkillName.Parry, 120);

            switch(TribeType)
            {
                case EodonTribe.Jukari:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0;
                    Hue = 34723; break;
		        case EodonTribe.Kurak:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0x2047;
                    Hue = 33960; break;
		        case EodonTribe.Barrab:
                    Female = false;
                    Body = 0x190;
                    HairItemID = 0x203B;
                    Hue = 34214; break;
		        case EodonTribe.Barako:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0x203C;
                    Hue = 35187; break;
		        case EodonTribe.Urali:
                    Female = true;
                    Body = 0x25E;
                    Race = Race.Elf;
                    HairItemID = 0x2FC1;
                    Hue = 35356; 
                    break;
		        case EodonTribe.Sakkhra:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0x203C;
                    Hue = 34894; 
                    RangeFight = 7; break;
            }

            Fame = 12000;
            Karma = 8000;
		}
		
		public override void BuildEquipment()
		{
            Item weapon = null;

			switch(TribeType)
			{
				default:
				case EodonTribe.Jukari:
                    weapon = new Pickaxe();
                    weapon.Hue = 1175;
                    if (Female)
                    {
                        SetWearable(new LeatherShorts(), 1175);
                        SetWearable(new LeatherBustierArms(), 1175);
                    }
                    else
                    {
                        SetWearable(new LeatherLegs(), 1175);
                        SetWearable(new BodySash(), 1175);
                    }
                    SetWearable(new Torch());
					break;
				case EodonTribe.Kurak:
                    weapon = new Tekagi();
                    SetWearable(new LeatherDo());
                    SetWearable(new PlateMempo(), 1192);
                    SetWearable(new ShortPants(), 1192);
                    SetWearable(new Sandals(), 1192);
					break;
				case EodonTribe.Barrab:
                    weapon = new Spear();
                    SetWearable(new PlateDo(), 1828);
                    SetWearable(new Obi(), 1828);
                    SetWearable(new PlateSuneate(), 1828);
                    SetWearable(new DecorativePlateKabuto(), 1834);
                    SetWearable(new SilverEarrings());
                    SetWearable(new Sandals(), 1828);
					break;
				case EodonTribe.Barako:
                    if (Female)
                    {
                        weapon = new Maul();
                        SetWearable(new DeerMask(), 2414);
                    }
                    else
                    {
                        weapon = new WarMace();
                        SetWearable(new BearMask(), 2414);
                    }
                    weapon.Hue = 2414;
                    SetWearable(new StuddedChest(), 2414);
                    SetWearable(new StuddedArms(), 2414);
                    SetWearable(new StuddedLegs(), 2414);
                    SetWearable(new StuddedGorget(), 2414);
                    SetWearable(new LeatherNinjaMitts(), 2414);
                    SetWearable(new Boots(), 2414);
					break;
				case EodonTribe.Urali:
                    SetWearable(new DragonChest(), 2576);
                    SetWearable(new LeatherJingasa(), 2576);
                    SetWearable(new MetalShield(), 2576);
                    SetWearable(new Waraji(), 2576);
                    SetWearable(new ChainLegs(), 2576);
					break;
				case EodonTribe.Sakkhra:
                    weapon = new Bow();
                    weapon.Hue = 2125;
                    if (Female)
                    {
                        SetWearable(new LeatherBustierArms(), 2128);
                        SetWearable(new LeatherSkirt(), 2125);
                    }
                    else
                    {
                        SetWearable(new LeatherChest(), 2128);
                        SetWearable(new SkullCap(), 2125);
                        SetWearable(new Kilt(), 2125);
                    }
                    SetWearable(new ThighBoots(), 2129);
					break;
			}

            if (weapon != null)
            {
                weapon.LootType = LootType.Blessed;
                SetWearable(weapon);
            }
		}

        public override bool AlwaysAttackable { get { return this.Region.IsPartOf<BattleRegion>(); } }
        public override bool ShowFameTitle { get { return false; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
        }

		public TribeWarrior(Serial serial) : base(serial)
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
	
	public class TribeShaman : BaseEodonTribesman
	{
        public override bool ShowSpellMantra { get { return true; } }

		[Constructable]
		public TribeShaman(EodonTribe type) : base(AIType.AI_Mage, type)
		{
            RangeFight = 7;

            PackGold(60, 70);
            PackReg(1, 3);

            Bandage b = new Bandage();
            b.Amount = Utility.RandomMinMax(3, 5);
            PackItem(b);
		}
		
		public override void BuildBody()
		{
            Name = String.Format("{0} the {1} shaman", GetRandomName(), TribeType.ToString());
			
			SetStr(125);
			SetDex(75, 100);
			SetInt(200, 250);
			
			SetHits(2500);
			
			SetDamage( 10, 15 );
			
			SetDamageType(ResistanceType.Physical, 100);
			
			SetSkill(SkillName.Wrestling, 100);
			SetSkill(SkillName.Fencing, 100);
			SetSkill(SkillName.Swords, 100);
			SetSkill(SkillName.Macing, 100);
			SetSkill(SkillName.Archery, 100);
			
			SetSkill(SkillName.Tactics, 100);
			SetSkill(SkillName.Anatomy, 100);
			SetSkill(SkillName.MagicResist, 100);
			SetSkill(SkillName.Magery, 120);
			SetSkill(SkillName.EvalInt, 120);

            switch (TribeType)
            {
                case EodonTribe.Jukari:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0;
                    Hue = 34723; break;
                case EodonTribe.Kurak:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = Female ? 0x203C : 0x203B;
                    Hue = 33960; break;
                case EodonTribe.Barrab:
                    Female = true;
                    Body = 0x191;
                    HairItemID = 0x203B;
                    Hue = 34214; break;
                case EodonTribe.Barako:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0x203C;
                    Hue = 35187; break;
                case EodonTribe.Urali:
                    Female = false;
                    Body = 0x25D;
                    Race = Race.Elf;
                    HairItemID = 0x2FC1;
                    Hue = 35356; 
                    break;
                case EodonTribe.Sakkhra:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0x203C;
                    Hue = 34894; 
                    RangeFight = 7; break;
            }

            Fame = 12000;
            Karma = 8000;
		}
		
		public override void BuildEquipment()
		{
			Item weapon = new WildStaff();
			
			switch(TribeType)
			{
				default:
				case EodonTribe.Jukari:
                    SetWearable(new FemaleLeatherChest(), 1933);
                    SetWearable(new LeatherSkirt(), 1933);
                    SetWearable(new Torch());
                    weapon.Hue = 1933;
					break;
				case EodonTribe.Kurak:
                    SetWearable(new LeatherDo(), 1150);
                    SetWearable(new PlateMempo(), 1150);
                    SetWearable(new TattsukeHakama(), 1150);
                    SetWearable(new Sandals(), 1150);
                    weapon.Hue = 1150;
					break;
				case EodonTribe.Barrab:
                    Robe robe = new Robe();
                    robe.ItemID = 9860;
                    SetWearable(robe, 1834);
                    SetWearable(new Obi(), 1834);
                    SetWearable(new Sandals(), 1831);
                    weapon.Hue = 1831;
					break;
				case EodonTribe.Barako:
                    SetWearable(new LeatherHiroSode(), 1518);
                    SetWearable(new LeatherGloves(), 1518);
                    SetWearable(new TribalMask(), 1518);
                    SetWearable(new LeatherNinjaPants(), 1518);
                    SetWearable(new BoneChest(), 1518);
                    SetWearable(new StuddedGorget(), 1518);
                    SetWearable(new Boots(), 1518);
                    SetWearable(new Surcoat(), 1518);
                    weapon.Hue = 1518;
					break;
				case EodonTribe.Urali:
                    SetWearable(new DragonLegs(), 2576);
                    SetWearable(new GoldEarrings());
                    SetWearable(new NinjaTabi(), 2576);
                    weapon.Hue = 2576;
					break;
				case EodonTribe.Sakkhra:
                    SetWearable(new StuddedChest(), 2118);
                    SetWearable(new LeatherArms(), 2106);
                    SetWearable(new LeatherGloves(), 2106);
                    SetWearable(new SkullCap(), 2118);
                    SetWearable(new RingmailLegs(), 2106);
                    SetWearable(new ThighBoots(), 2106);
                    weapon.Hue = 2118;
					break;
			}

            if (weapon != null)
            {
                weapon.LootType = LootType.Blessed;
                SetWearable(weapon);
            }
		}

        public override bool AlwaysAttackable { get { return this.Region.IsPartOf<BattleRegion>(); } }
        public override bool ShowFameTitle { get { return false; } }

        public TribeShaman(Serial serial) : base(serial)
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
	
	public class TribeChieftan : BaseEodonTribesman
	{
		[Constructable]
		public TribeChieftan(EodonTribe type) : base(AIType.AI_Melee, type)
		{
		}
		
		public override void BuildBody()
		{
            Name = String.Format("{0} the {1} {2}", GetRandomName(), TribeType.ToString(), Female ? "chieftess" : "chieftan");
			
			SetStr(200);
			SetDex(2000);
			SetInt(200, 250);
			
			SetHits(4500);
			
			SetDamage( 15, 28 );
			
			SetDamageType(ResistanceType.Physical, 100);
			
			//Set resistances?
			
			SetSkill(SkillName.Wrestling, 120);
            SetSkill(SkillName.Fencing, 120);
            SetSkill(SkillName.Swords, 120);
            SetSkill(SkillName.Macing, 120);
            SetSkill(SkillName.Archery, 120);

            SetSkill(SkillName.Tactics, 120);
            SetSkill(SkillName.Anatomy, 120);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Parry, 120);

            switch (TribeType)
            {
                case EodonTribe.Jukari:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0;
                    Hue = 34723; break;
                case EodonTribe.Kurak:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = Female ? 0x2046 : 0x203B;
                    Hue = 33960; break;
                case EodonTribe.Barrab:
                    Female = true;
                    Body = 0x191;
                    HairItemID = 0x203B;
                    Hue = 34214; break;
                case EodonTribe.Barako:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0x203C;
                    Hue = 35187; break;
                case EodonTribe.Urali:
                    Female = true;
                    Body = 0x25E;
                    Race = Race.Elf;
                    HairItemID = 0x2FD0;
                    Hue = 35356;
                    break;
                case EodonTribe.Sakkhra:
                    Female = Utility.RandomBool();
                    Body = Female ? 0x191 : 0x190;
                    HairItemID = 0x203C;
                    Hue = 34894;
                    RangeFight = 7; break;
            }

            Fame = 18000;
            Karma = 8000;
		}
		
		public override void BuildEquipment()
		{
            Item weapon = null;

            switch (TribeType)
            {
                default:
                case EodonTribe.Jukari:
                    SetWearable(new LeatherLegs(), 1175);
                    SetWearable(new Shirt(), 1175);
                    SetWearable(new Torch());
                    weapon = new Bokuto();
                    weapon.Hue = 1175;
                    break;
                case EodonTribe.Kurak:
                    SetWearable(new LeatherDo(), 1175);
                    SetWearable(new FancyShirt(), 1175);
                    SetWearable(new TattsukeHakama());
                    SetWearable(new Sandals(), 1175);
                    weapon = new Tekagi();
                    weapon.Hue = 1175;
                    break;
                case EodonTribe.Barrab:
                    SetWearable(new PlateDo(), 1828);
                    SetWearable(new PlateSuneate(), 1828);
                    SetWearable(new DecorativePlateKabuto(), 1834);
                    SetWearable(new Sandals(), 1828);
                    weapon = new Spear();
                    weapon.Hue = 1828;
                    break;
                case EodonTribe.Barako:
                    SetWearable(new BoneChest(), 2407);
                    SetWearable(new LeatherNinjaPants(), 2407);
                    SetWearable(new StuddedHiroSode(), 2407);
                    SetWearable(new BoneGloves(), 2407);
                    SetWearable(new StuddedGorget(), 2407);
                    SetWearable(new Boots(), 2407);
                    weapon = new Scepter();
                    weapon.Hue = 2407;
                    break;
                case EodonTribe.Urali:
                    SetWearable(new ChainLegs(), 2576);
                    SetWearable(new DragonChest(), 2576);
                    SetWearable(new DragonArms(), 2576);
                    SetWearable(new MetalShield(), 2576);
                    SetWearable(new Circlet(), 2576);
                    SetWearable(new JinBaori(), 2592);
                    SetWearable(new Waraji(), 2576);
                    break;
                case EodonTribe.Sakkhra:
                    SetWearable(new StuddedChest(), 2118);
                    SetWearable(new LeatherArms(), 2106);
                    SetWearable(new LeatherGloves(), 2106);
                    SetWearable(new SkullCap(), 2118);
                    SetWearable(new RingmailLegs(), 2106);
                    SetWearable(new ThighBoots(), 2106);
                    weapon = new Yumi();
                    weapon.Hue = 2118;
                    break;
            }

            if (weapon != null)
            {
                weapon.LootType = LootType.Blessed;
                SetWearable(weapon);
            }
		}

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

		public TribeChieftan(Serial serial) : base(serial)
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