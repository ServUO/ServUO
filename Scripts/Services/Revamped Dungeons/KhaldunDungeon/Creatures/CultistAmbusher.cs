#region References
using Server.Items;
using Server.Misc;
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
    [CorpseName("an inhuman corpse")]
    public class CultistAmbusher : BaseCreature
	{
		[Constructable]
		public CultistAmbusher()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "Cultist Ambusher";
			Body = 0x190;
            Hue = 2500;
			BaseSoundID = 0x45A;

            SetStr(150, 200);
            SetDex(150);
            SetInt(25, 44);

            SetDamage(8, 18);

            SetDamageType(ResistanceType.Physical, 100);

			SetResistance(ResistanceType.Physical, 30, 50);
			SetResistance(ResistanceType.Fire, 30, 50);
			SetResistance(ResistanceType.Cold, 30, 50);
			SetResistance(ResistanceType.Poison, 30, 50);
			SetResistance(ResistanceType.Energy, 30, 50);
            
            SetSkill(SkillName.Fencing, 100.0);
            SetSkill(SkillName.Macing, 100.0);
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Swords, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Archery, 100.0);

            Fame = 5000;
			Karma = -5000;

            switch (Utility.Random(3))
            {
                case 0:
                    {
                        SetWearable(new RingmailChest(), 1510);
                        SetWearable(new ChainLegs(), 1345);
                        SetWearable(new Sandals(), 1345);
                        SetWearable(new LeatherNinjaHood(), 1345);
                        SetWearable(new LeatherGloves(), 1345);
                        SetWearable(new LeatherArms(), 1345);
                        break;
                    }
                case 1:
                    {
                        SetWearable(new Robe(2306));
                        SetWearable(new BearMask(2683));
                        break;
                    }
                case 2:
                    {
                        SetWearable(new Shirt(676));
                        SetWearable(new RingmailLegs());
                        SetWearable(new StuddedArms());
                        SetWearable(new StuddedGloves());
                        break;
                    }
                case 3:
                    {
                        SetWearable(new SkullCap(2406));
                        SetWearable(new JinBaori(1001));
                        SetWearable(new Shirt());
                        SetWearable(new ShortPants(902));
                        break;
                    }
            }

            switch (Utility.Random(3))
            {
                case 0: SetWearable(new Spear()); break;
                case 1:
                    {
                        switch (Utility.Random(4))
                        {
                            case 0: SetWearable(new Yumi()); break;
                            case 1: SetWearable(new Crossbow()); break;
                            case 2: SetWearable(new RepeatingCrossbow()); break;
                            case 3: SetWearable(new HeavyCrossbow()); break;
                        }

                        RangeFight = 3;
                        AI = AIType.AI_Archer;

                        break;
                    }
                case 2: SetWearable(new WarMace()); break;
            }            
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public CultistAmbusher(Serial serial)
			: base(serial)
		{
        }

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Rich);
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
