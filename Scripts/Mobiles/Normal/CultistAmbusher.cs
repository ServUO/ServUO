#region References
using Server.Items;
using Server.Misc;
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class CultistAmbusher : BaseCreature
	{
		[Constructable]
		public CultistAmbusher()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 7, 0.2, 0.4)
		{
			Name = "Cultist Ambusher";
			Body = 0x190;
            Hue = 2500;
			BaseSoundID = 0x45A;

            SetStr(115, 150);
            SetDex(150);
            SetInt(25, 44);

            SetHits(58, 72);

            SetDamage(8, 18);

            SetDamageType(ResistanceType.Physical, 100);

			SetResistance(ResistanceType.Physical, 1, 10);
			SetResistance(ResistanceType.Fire, 1, 10);
			SetResistance(ResistanceType.Cold, 1, 10);
			SetResistance(ResistanceType.Poison, 1, 10);
			SetResistance(ResistanceType.Energy, 1, 10);
            
            SetSkill(SkillName.Fencing, 77.6, 92.5);
            SetSkill(SkillName.Healing, 60.3, 90.0);
            SetSkill(SkillName.Macing, 77.6, 92.5);
            SetSkill(SkillName.Anatomy, 77.6, 87.5);
            SetSkill(SkillName.MagicResist, 77.6, 97.5);
            SetSkill(SkillName.Swords, 77.6, 92.5);
            SetSkill(SkillName.Tactics, 77.6, 87.5);

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

            switch (Utility.Random(5))
            {
                case 0: SetWearable(new Spear()); break;
                case 1: SetWearable(new Yumi()); break;
                case 2: SetWearable(new Crossbow()); break;
                case 3: SetWearable(new RepeatingCrossbow()); break;
                case 4: SetWearable(new WarMace()); break;
                case 5: SetWearable(new HeavyCrossbow()); break;
            }            
        }

        public override bool AlwaysMurderer { get { return true; } }

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
