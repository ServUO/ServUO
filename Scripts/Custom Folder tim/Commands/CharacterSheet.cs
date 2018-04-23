using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Gumps
{
	public class CharacterSheetDragon : Gump
	{
        public static void Initialize()
        {
            CommandSystem.Register("mystats", AccessLevel.Player, new CommandEventHandler(StatsDragon_OnCommand));
        }

        private static void StatsDragon_OnCommand(CommandEventArgs e)
        {
            e.Mobile.CloseGump(typeof(CharacterSheetDragon));
            e.Mobile.SendGump(new CharacterSheetDragon(e.Mobile));
        }

        public CharacterSheetDragon(Mobile m)
			: base( 208, 166 )
		{
            PlayerMobile pm = m as PlayerMobile;

            // configuration
            int LRCCap = 100;
            int LMCCap = 40;
            double BandageSpeedCap = 0.0;
            int SwingSpeedCap = 125;
            int HCICap = 45;
            int DCICap = 45;
            int FCCap = 4;
            int FCRCap = 6;
            int DamageIncreaseCap = 100;
            int SDICap = 100;
            int ReflectDamageCap = 50;
            int SSICap = 100;

            int LRC = AosAttributes.GetValue(pm, AosAttribute.LowerRegCost) > LRCCap ? LRCCap : AosAttributes.GetValue(pm, AosAttribute.LowerRegCost);
            int LMC = AosAttributes.GetValue(pm, AosAttribute.LowerManaCost) > LMCCap ? LMCCap : AosAttributes.GetValue(pm, AosAttribute.LowerManaCost);
            double BandageSpeed = (2.0 + (0.5 * ((double)(205 - pm.Dex) / 10))) < BandageSpeedCap ? BandageSpeedCap : (2.0 + (0.5 * ((double)(205 - pm.Dex) / 10)));
            TimeSpan SwingSpeed = (pm.Weapon as BaseWeapon).GetDelay(pm) > TimeSpan.FromSeconds(SwingSpeedCap) ? TimeSpan.FromSeconds(SwingSpeedCap) : (pm.Weapon as BaseWeapon).GetDelay(pm);
            int HCI = AosAttributes.GetValue(pm, AosAttribute.AttackChance) > HCICap ? HCICap : AosAttributes.GetValue(pm, AosAttribute.AttackChance);
            int DCI = AosAttributes.GetValue(pm, AosAttribute.DefendChance) > DCICap ? DCICap : AosAttributes.GetValue(pm, AosAttribute.DefendChance);
            int FC = AosAttributes.GetValue(pm, AosAttribute.CastSpeed) > FCCap ? FCCap : AosAttributes.GetValue(pm, AosAttribute.CastSpeed);
            int FCR = AosAttributes.GetValue(pm, AosAttribute.CastRecovery) > FCRCap ? FCRCap : AosAttributes.GetValue(pm, AosAttribute.CastRecovery);
            int DamageIncrease = AosAttributes.GetValue(pm, AosAttribute.WeaponDamage) > DamageIncreaseCap ? DamageIncreaseCap : AosAttributes.GetValue(pm, AosAttribute.WeaponDamage);
            int SDI = AosAttributes.GetValue(pm, AosAttribute.SpellDamage) > SDICap ? SDICap : AosAttributes.GetValue(pm, AosAttribute.SpellDamage);
            int ReflectDamage = AosAttributes.GetValue(pm, AosAttribute.ReflectPhysical) > ReflectDamageCap ? ReflectDamageCap : AosAttributes.GetValue(pm, AosAttribute.ReflectPhysical);
            int SSI = AosAttributes.GetValue(pm, AosAttribute.WeaponSpeed) > SSICap ? SSICap : AosAttributes.GetValue(pm, AosAttribute.WeaponSpeed);

            int hue = 1149;

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(56, 56, 423, 340, 9270);
            this.AddImage(443, 20, 10441);
            this.AddImage(10, 20, 10440);
            
            this.AddBackground(144, 20, 249, 42, 9270);
            this.AddLabel(173, 31, 394, @"Land of Archon Character Sheet");
                                          
            /*********** STATS *****************/
            this.AddLabel(75, 74, 394, @"Stats");
            this.AddImage(75, 94, 9277);
            this.AddImage(133, 94, 9277);
            this.AddLabel(75, 106, 155, @"Strength:");
            this.AddLabel(200, 106, hue, @"" + pm.RawStr + " + " + (pm.Str - pm.RawStr) + "");
            this.AddLabel(76, 136, 155, @"Intelligence:");
            this.AddLabel(200, 136, hue, @"" + pm.RawInt + " + " + (pm.Int - pm.RawInt) + "");
            this.AddLabel(75, 121, 155, @"Dexterity:");
            this.AddLabel(200, 121, hue, @"" + pm.RawDex + " + " + (pm.Dex - pm.RawDex) + "");

            /*********** VITALS ****************/
            this.AddLabel(75, 157, 394, @"Vitals");
            this.AddLabel(217, 157, 394, @"Regen");
            this.AddLabel(169, 157, 394, @"Bonus");
            this.AddImage(75, 178, 9277);
            this.AddImage(133, 178, 9277);
			this.AddLabel(75, 190, 155, @"HP:");
            this.AddLabel(105, 190, hue, @"" + pm.Hits + "/" + pm.HitsMax + "");
            this.AddLabel(174, 190, hue, @"(+ " + AosAttributes.GetValue(pm, AosAttribute.BonusHits) + ")");
            this.AddLabel(232, 190, hue, @"" + AosAttributes.GetValue(pm, AosAttribute.RegenHits) + "");
            this.AddLabel(75, 205, 155, @"MP:");
            this.AddLabel(105, 205, hue, @"" + pm.Mana + "/" + pm.ManaMax + "");
            this.AddLabel(174, 205, hue, @"(+ " + AosAttributes.GetValue(pm, AosAttribute.BonusMana) + ")");
            this.AddLabel(232, 205, hue, @"" + AosAttributes.GetValue(pm, AosAttribute.RegenMana) + "");
            this.AddLabel(75, 220, 155, @"SP:");
            this.AddLabel(105, 220, hue, @"" + pm.Stam + "/" + pm.StamMax + "");
            this.AddLabel(174, 220, hue, @"(+ " + AosAttributes.GetValue(pm, AosAttribute.BonusStam) + ")");
            this.AddLabel(232, 220, hue, @"" + AosAttributes.GetValue(pm, AosAttribute.RegenStam) + "");

            /************ POINTS ************/
            this.AddLabel(75, 240, 394, @"Points");
            this.AddImage(75, 260, 9277);
            this.AddImage(133, 260, 9277);
            this.AddLabel(75, 270, 155, @"Tithing Points:");
            this.AddLabel(182, 270, hue, @"" + pm.TithingPoints + "");
            this.AddLabel(75, 285, 155, @"Karma:");
            this.AddLabel(182, 285, hue, @"" + pm.Karma + "");
            this.AddLabel(75, 300, 155, @"Fame:");
            this.AddLabel(182, 300, hue, @"" + pm.Fame + "");
            this.AddLabel(75, 315, 155, @"Player Kills:");
            this.AddLabel(182, 315, hue, @"" + pm.Kills + "");
            this.AddLabel(75, 330, 155, @"Followers:");
            this.AddLabel(182, 330, hue, @"" + pm.Followers + "/" + pm.FollowersMax + "");
            // Setup Damage
            IWeapon weapon = m.Weapon;

            int min = 0, max = 0;

            if (weapon != null)
                weapon.GetStatusDamage(m, out min, out max);

            this.AddLabel(75, 345, 155, @"Damage:");
            this.AddLabel(182, 345, hue, @"" + min + " - " + max + "");
            this.AddLabel(75, 360, 155, @"Luck:");
            this.AddLabel(182, 360, hue, @"" + pm.Luck + "");


            /************* CHAR INFO *********************/
            this.AddLabel(272, 74, 394, @"Character Information");
            this.AddImage(272, 94, 9277);
            this.AddImage(330, 94, 9277);
            this.AddLabel(272, 106, 93, @"" + pm.Name + "");
			this.AddLabel(272, 126, 155, @"LRC:");
            this.AddLabel(304, 126, hue, @"" + LRC + " %");
            this.AddLabel(350, 126, 155, @"LMC:");
            this.AddLabel(385, 126, hue, @"" + LMC + " %"); 
            this.AddLabel(272, 146, 155, @"FC:");
            this.AddLabel(305, 146, hue, @"" + FC + "");
            this.AddLabel(350, 146, 155, @"FCR:");
            this.AddLabel(385, 146, hue, @"" + FCR + "");
            this.AddLabel(273, 166, 155, @"SDI:");
            this.AddLabel(305, 166, hue, @"" + SDI + " %");
            this.AddLabel(350, 166, 155, @"SSI:");
            this.AddLabel(385, 166, hue, @"" + SSI + " %");
            this.AddLabel(273, 186, 155, @"HCI:");
            this.AddLabel(305, 186, hue, @"" + HCI + " %");
            this.AddLabel(350, 186, 155, @"DCI:");
            this.AddLabel(385, 186, hue, @"" + DCI + " %");
            this.AddLabel(273, 206, 155, @"Damage Increase:");
            this.AddLabel(385, 206, hue, @"" + DamageIncrease + " %");
            this.AddLabel(273, 226, 155, @"Bandage Speed:");
            this.AddLabel(385, 226, hue, String.Format("{0:0.0}s", new DateTime(TimeSpan.FromSeconds(BandageSpeed).Ticks).ToString("s.ff")));
            this.AddLabel(273, 246, 155, @"Swing Speed:");
            this.AddLabel(385, 246, hue, String.Format("{0}s", new DateTime(SwingSpeed.Ticks).ToString("s.ff")));
            this.AddLabel(273, 266, 155, @"Reflect Damage:");
            this.AddLabel(385, 266, hue, @"" + ReflectDamage + " %");
            // Setup Weight
            int weight = Mobile.BodyWeight + m.TotalWeight;
            this.AddLabel(273, 286, 155, @"Weight:");
            this.AddLabel(385, 286, hue, @"" + weight + "/" + pm.MaxWeight + "");

            this.AddLabel(273, 306, 155, @"Skill Total:");
            this.AddLabel(385, 306, hue, @"" + pm.SkillsTotal + "");

            /************* MONEY ***************/
            //Lets Setup the Box as the Characters BankBox
            BankBox box = pm.BankBox;
            this.AddLabel(272, 331, 394, @"Loose Coins");
            this.AddLabel(362, 331, 394, @"Carried/Bank");
            this.AddImage(272, 351, 9277);
            this.AddImage(330, 351, 9277);
            this.AddLabel(276, 360, 144, @"Gold");
            this.AddLabel(330, 360, hue, @"" + pm.TotalGold + "/" + Banker.GetBalance( pm ).ToString());

		}
		

	}
}