// Created by Peoharen
using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Necromancy;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public DateTime m_NextShiftTime;
        public virtual bool m_CanShapeShift
        {
            get
            {
                return (this.m_Mobile is BaseVendor || this.m_Mobile is BaseEscortable);
            }
        }
        public void NecromancerPower()
        {
            Spell spell = null;

            spell = this.GetNecromancerSpell();

            if (spell != null)
                spell.Cast();

            return;
        }

        public Spell GetNecromancerSpell()
        {
            Spell spell = null;

            switch ( Utility.Random(12) )
            {
                case 0:
                case 1:	// Create an army for our ourselves
                    {
                        int whichone = Utility.RandomMinMax(1, 2);

                        if (whichone == 2 && this.m_Mobile.Skills[SkillName.Necromancy].Value > 50.0)
                        {
                            if (this.m_Mobile.Debug)
                                this.m_Mobile.Say(1109, "Undead: Casting Animate Dead");

                            spell = new AnimateDeadSpell(this.m_Mobile, null);
                        }
                        else if (this.m_Mobile.Followers == 0 || this.m_Mobile.Followers == 3)
                        {
                            if (this.m_Mobile.Debug)
                                this.m_Mobile.Say(1109, "Undead: Summoning Familiar ");

                            this.CreateNecromancerFamiliar();
                        }
                        else
                            goto default;

                        break;
                    }
                case 2:
                case 3: // Curse them
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.Say(1109, "Cusing Them");

                        spell = this.GetNecromancerCurseSpell();
                        break;
                    }
                case 4:
                case 5:	// Reverse combat
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.Say(1109, "Casting Blood Oath");

                        if (this.m_Mobile.Skills[SkillName.Necromancy].Value > 30.0)
                            spell = new BloodOathSpell(this.m_Mobile, null);

                        break;
                    }
                case 6: // Shapeshift
                    {
                        if (this.m_CanShapeShift)
                        {
                            if (this.m_Mobile.Debug)
                                this.m_Mobile.Say(1109, "Shapechange ");

                            spell = this.GetNecromancerShapeshiftSpell();
                        }
                        else
                            goto case 3; // Curse them

                        break;
                    }

                default: // Damage them
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.Say(1109, "Random damage spell");

                        spell = this.GetNecromancerDamageSpell();
                        break;
                    }
            }

            return spell;
        }

        public Spell GetNecromancerDamageSpell()
        {
            int maxCircle = (int)((this.m_Mobile.Skills[SkillName.Necromancy].Value + 20.0) / (100.0 / 7.0));

            if (maxCircle < 2)
                maxCircle = 2;

            switch ( Utility.Random(maxCircle + 1) )
            {
                case 0:
                case 1:
                case 2:
                    return this.CheckForCurseWeapon();
                case 3:
                case 4:
                    return new PoisonStrikeSpell(this.m_Mobile, null);
                case 5:
                case 6:
                    return new WitherSpell(this.m_Mobile, null);
                case 7:
                    return new StrangleSpell(this.m_Mobile, null);
                default:
                    return this.CheckForVengefulSpiritSpell();
            }
        }

        public Spell GetNecromancerCurseSpell()
        {
            int whichone = Utility.RandomMinMax(1, 4);

            if (whichone == 4 && this.m_Mobile.Skills[SkillName.Necromancy].Value >= 75.0)
                return new StrangleSpell(this.m_Mobile, null);
            else if (whichone == 3 && this.m_Mobile.Skills[SkillName.Necromancy].Value >= 40.0)
                return new MindRotSpell(this.m_Mobile, null);
            else if (whichone >= 2 && this.m_Mobile.Skills[SkillName.Necromancy].Value >= 30.0)
                return new EvilOmenSpell(this.m_Mobile, null);
            else
                return new CorpseSkinSpell(this.m_Mobile, null);
        }

        public Spell GetNecromancerShapeshiftSpell()
        {
            if (DateTime.UtcNow < this.m_NextShiftTime)
                return this.GetNecromancerDamageSpell();

            this.m_NextShiftTime = DateTime.UtcNow + TimeSpan.FromSeconds(130);

            if (this.m_Mobile.Skills[SkillName.Necromancy].Value > 110.0)
                return new VampiricEmbraceSpell(this.m_Mobile, null);
            else if (this.m_Mobile.Skills[SkillName.Necromancy].Value > 80.0)
                return new LichFormSpell(this.m_Mobile, null);
            else if (this.m_Mobile.Skills[SkillName.Necromancy].Value > 50.0)
                return new HorrificBeastSpell(this.m_Mobile, null);
            else if (this.m_Mobile.Skills[SkillName.Necromancy].Value > 30.0)
                return new WraithFormSpell(this.m_Mobile, null);
            else
                return null;
        }

        public Spell CheckForCurseWeapon()
        {
            if (this.m_Mobile.Skills[SkillName.Necromancy].Value > 5.0)
            {
                BaseWeapon weapon = this.m_Mobile.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;

                if (weapon == null)
                    weapon = this.m_Mobile.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

                if (weapon != null)
                {
                    if (!(weapon.Cursed))
                        return new CurseWeaponSpell(this.m_Mobile, null);
                }
            }

            return new PainSpikeSpell(this.m_Mobile, null);
        }

        public Spell CheckForVengefulSpiritSpell()
        {
            if (this.m_Mobile.Followers >= 3)
                return new PoisonStrikeSpell(this.m_Mobile, null);
            else
                return new VengefulSpiritSpell(this.m_Mobile, null);
        }

        public void CreateNecromancerFamiliar()
        {
            int whichone = Utility.RandomMinMax(1, 5);
            BaseCreature mob = null;

            if (whichone == 5 && this.m_Mobile.Skills[SkillName.Necromancy].Value >= 100.0)
                mob = new VampireBatFamiliar();
            else if (whichone >= 4 && this.m_Mobile.Skills[SkillName.Necromancy].Value >= 80.0)
                mob = new DeathAdder();
            else if (whichone >= 3 && this.m_Mobile.Skills[SkillName.Necromancy].Value >= 60.0)
                mob = new DarkWolfFamiliar();
            else if (whichone >= 2 && this.m_Mobile.Skills[SkillName.Necromancy].Value >= 50.0)
                mob = new ShadowWispFamiliar();
            else if (this.m_Mobile.Skills[SkillName.Necromancy].Value >= 30.0)
                mob = new HordeMinionFamiliar();

            if (mob != null)
            {
                BaseCreature.Summon(mob, this.m_Mobile, this.m_Mobile.Location, -1, TimeSpan.FromDays(1.0));
                mob.FixedParticles(0x3728, 1, 10, 9910, EffectLayer.Head);
                mob.PlaySound(mob.GetIdleSound());
            }

            return;
        }
    }
}