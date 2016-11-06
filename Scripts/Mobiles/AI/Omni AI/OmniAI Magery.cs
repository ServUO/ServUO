// Created by Peoharen
using System;
using Server.Spells;
using Server.Spells.Eighth;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Second;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public static int[] m_MageryMana = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };
        DateTime m_LastExplosion;
        public bool m_CanUseMagerySummon
        {
            get
            {
                return false;
            }//m_ForceUseAll; }
        }
        public void MageryPower()
        {
            Spell spell = null;

            spell = this.GetMagerySpell();

            if (spell != null)
            {
                this.m_NextCastTime = DateTime.UtcNow + spell.GetCastDelay() + spell.GetCastRecovery();
                spell.Cast();
            }

            return;
        }

        public Spell GetMagerySpell()
        {
            Spell spell = null;

            // always check for bless, per OSI
            spell = this.CheckBless();

            if (spell != null)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1156, "Blessing my self");

                return spell;
            }

            // always check for curse, per OSI
            spell = this.CheckCurse();

            if (spell != null)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1156, "Cursing my opponent");

                return spell;
            }

            // 25% chance to cast poison if needed
            if (this.m_Mobile.Combatant is Mobile && !((Mobile)m_Mobile.Combatant).Poisoned && Utility.RandomDouble() > 0.75)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1156, "Casting Poison");

                spell = new PoisonSpell(this.m_Mobile, null);
            }

            // scaling chance to drain mana based on how much of a caster the opponent is
            if (this.CheckManaDrain() > 0.75)
            {
                if (this.m_Mobile.Skills[SkillName.Magery].Value > 80.0)
                    spell = new ManaVampireSpell(this.m_Mobile, null);
                else if (this.m_Mobile.Skills[SkillName.Magery].Value > 40.0)
                    spell = new ManaDrainSpell(this.m_Mobile, null);

                if (spell != null)
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.Say(1156, "Draining mana");

                    return spell;
                }
            }

            // 10% chance to summon help
            if (this.m_CanUseMagerySummon && Utility.RandomDouble() > 0.90)
            {
                spell = this.CheckMagerySummon();

                if (spell != null)
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.Say(1156, "Summoning help");
                    return spell;
                }
            }

            // Let's just blast the hell out of them.
            return this.GetRandomMageryDamageSpell();
        }

        public Spell CheckBless()
        {
            StatMod mod = this.m_Mobile.GetStatMod("[Magic] Str Buff");

            if (mod != null)
                return null;

            if (this.m_Mobile.Skills[SkillName.Magery].Value >= 40.0)
                return new BlessSpell(this.m_Mobile, null);

            mod = this.m_Mobile.GetStatMod("[Magic] Int Buff");

            if (mod == null)
                return new CunningSpell(this.m_Mobile, null);

            mod = this.m_Mobile.GetStatMod("[Magic] Dex Buff");

            if (mod == null)
                return new AgilitySpell(this.m_Mobile, null);

            return new StrengthSpell(this.m_Mobile, null);
        }

        public Spell CheckCurse()
        {
            Mobile foe = this.m_Mobile.Combatant as Mobile;

            if (foe == null)
                return null;

            StatMod mod = foe.GetStatMod("[Magic] Int Curse");

            if (mod != null)
                return null;

            if (this.m_Mobile.Skills[SkillName.Magery].Value >= 40.0)
                return new CurseSpell(this.m_Mobile, null);

            int whichone = 1;
            Spell spell = null;

			if ((mod = foe.GetStatMod("[Magic] Str Curse")) != null)
                whichone++;

			if ((mod = this.m_Mobile.GetStatMod("[Magic] Dex Curse")) != null)
                whichone++;

            switch ( whichone )
            {
                case 1:
                    spell = new FeeblemindSpell(this.m_Mobile, null);
                    break;
                case 2:
                    spell = new WeakenSpell(this.m_Mobile, null);
                    break;
                case 3:
                    spell = new ClumsySpell(this.m_Mobile, null);
                    break;
            }

            return spell;
        }

        public double CheckManaDrain()
        {
            Mobile foe = this.m_Mobile.Combatant as Mobile;

            if (foe == null || foe.Mana < 10)
                return 0.0;

            double drain = 0.0;

            if (foe.Skills[SkillName.Bushido].Value > 35.0)
                drain += 0.1;
            if (foe.Skills[SkillName.Chivalry].Value > 35.0)
                drain += 0.1;
            if (foe.Skills[SkillName.Magery].Value > 35.0)
                drain += 0.2;
            if (foe.Skills[SkillName.Necromancy].Value > 35.0)
                drain += 0.2;
            if (foe.Skills[SkillName.Ninjitsu].Value > 35.0)
                drain += 0.1;

            if (drain == 0.0)
                return drain;
            else
                return Utility.RandomDouble() + drain;
        }

        public Spell CheckMagerySummon()
        {
            int slots = this.m_Mobile.FollowersMax - this.m_Mobile.Followers;

            if (slots < 2)
                return null;

            Spell spell = null;

            if (this.m_Mobile.Skills[SkillName.Magery].Value > 95.0 && this.m_Mobile.Mana > 50)
            {
                int whichone = Utility.Random(10);

                if (slots > 4 && whichone == 0)
                    spell = new SummonDaemonSpell(this.m_Mobile, null);
                else if (slots > 3 && whichone < 2)
                    spell = new FireElementalSpell(this.m_Mobile, null);
                else if (slots > 2 && whichone < 3)
                    spell = new WaterElementalSpell(this.m_Mobile, null);
                else if (whichone < 4)
                    spell = new AirElementalSpell(this.m_Mobile, null);
                else if (whichone < 5)
                    spell = new EarthElementalSpell(this.m_Mobile, null);
                else
                    spell = new EnergyVortexSpell(this.m_Mobile, null);
            }
            else if (this.m_Mobile.Skills[SkillName.Magery].Value > 55.0 && this.m_Mobile.Mana > 14) // 5th level
            {
                if (this.m_Mobile.InitialInnocent) // peace loving hippy using nature friendly animals
                    spell = new SummonCreatureSpell(this.m_Mobile, null);
                else
                    spell = new BladeSpiritsSpell(this.m_Mobile, null);
            }

            return spell;
        }

        public Spell GetRandomMageryDamageSpell()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1156, "Random damage spell");

            int whichone = (int)(this.m_Mobile.Skills[SkillName.Magery].Value / 14.2) - 1;

            whichone += Utility.RandomMinMax(-2, -0);

            if (whichone > 6)
                whichone = 6;
            else if (whichone < 1)
                whichone = 1;

            // instead of checking each spell level to wipe all all the mana out only 
            // lower it once. Failure means mana might build up for a better spell
            if (m_MageryMana[whichone] > this.m_Mobile.Mana)
                whichone--;

            switch( whichone )
            {
                case 6:
                    return new FlameStrikeSpell(this.m_Mobile, null);
                case 5:
                    {
                        if (DateTime.UtcNow > this.m_LastExplosion)
                        {
                            this.m_LastExplosion = DateTime.UtcNow + TimeSpan.FromSeconds(3);
                            return new ExplosionSpell(this.m_Mobile, null);
                        }
                        else
                            return new EnergyBoltSpell(this.m_Mobile, null);
                    }
                case 4:
                    return new MindBlastSpell(this.m_Mobile, null);
                case 3:
                    return new LightningSpell(this.m_Mobile, null);
                case 2:
                    return new FireballSpell(this.m_Mobile, null);
                case 1:
                    return new HarmSpell(this.m_Mobile, null);
                default:
                    return new MagicArrowSpell(this.m_Mobile, null);
            }
        }
    }
}