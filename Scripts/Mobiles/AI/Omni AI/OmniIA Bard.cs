//Created by Peoharen
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public void BardPower()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1162, "");

            this.CheckInstrument();

            switch( Utility.Random(3) )
            {
                case 0:
                    this.UseDiscord();
                    break;
                case 1:
                    this.UseProvocation();
                    break;
                case 2:
                    this.UsePeacemaking();
                    break;
            }

            // TODO Bard Spell support

            return;
        }

        public bool CheckInstrument()
        {
            BaseInstrument inst = BaseInstrument.GetInstrument(this.m_Mobile);

            if (inst != null)
                return true;

            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1162, "I need an instrument, fixing my problem.");

            if (this.m_Mobile.Backpack == null)
                return false;

            inst = (BaseInstrument)this.m_Mobile.Backpack.FindItemByType(typeof(BaseInstrument));

            if (inst == null)
            {
                inst = new Harp();
                inst.SuccessSound = 0x58B;
                inst.FailureSound = 0x58C;
                // Got Better Music?
                // inst.DiscordSound = inst.PeaceSound = 0x58B;
                // inst.ProvocationSound = 0x58A;
            }

            BaseInstrument.SetInstrument(this.m_Mobile, inst);
            return true;
        }

        #region discord
        public void UseDiscord()
        {
            Mobile target = this.m_Mobile.Combatant;

            if (target == null)
                return;

            if (!this.m_Mobile.UseSkill(SkillName.Discordance))
                return;

            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1162, "Discording");

            // Discord's target flag is harmful so the AI already targets it's combatant.
            // However players are immune to Discord hence the following.
            if (target is PlayerMobile)
            {
                double effect = -(this.m_Mobile.Skills[SkillName.Discordance].Value / 5.0);
                TimeSpan duration = TimeSpan.FromSeconds(this.m_Mobile.Skills[SkillName.Discordance].Value * 2);

                ResistanceMod[] mods = 
                {
                    new ResistanceMod(ResistanceType.Physical, (int)(effect * 0.01)),
                    new ResistanceMod(ResistanceType.Fire, (int)(effect * 0.01)),
                    new ResistanceMod(ResistanceType.Cold, (int)(effect * 0.01)),
                    new ResistanceMod(ResistanceType.Poison, (int)(effect * 0.01)),
                    new ResistanceMod(ResistanceType.Energy, (int)(effect * 0.01))
                };
		
                TimedResistanceMod.AddMod(target, "Discordance", mods, duration);
                target.AddStatMod(new StatMod(StatType.Str, "DiscordanceStr", (int)(target.RawStr * effect), duration));
                target.AddStatMod(new StatMod(StatType.Int, "DiscordanceInt", (int)(target.RawInt * effect), duration));
                target.AddStatMod(new StatMod(StatType.Dex, "DiscordanceDex", (int)(target.RawDex * effect), duration));
                new DiscordEffectTimer(target, duration).Start();
            }
        }

        public class DiscordEffectTimer : Timer
        {
            public Mobile Mob;
            public int Count;
            public int MaxCount;

            public DiscordEffectTimer(Mobile mob, TimeSpan duration)
                : base(TimeSpan.FromSeconds(1.25), TimeSpan.FromSeconds(1.25))
            {
                this.Mob = mob;
                this.Count = 0;
                this.MaxCount = (int)((double)duration.TotalSeconds / 1.25);
            }

            protected override void OnTick()
            {
                if (this.Count >= this.MaxCount)
                    this.Stop();
                else
                {
                    this.Mob.FixedEffect(0x376A, 1, 32);
                    this.Count++;
                }
            }
        }
        #endregion

        public bool UseProvocation()
        {
            if (!this.m_Mobile.UseSkill(SkillName.Provocation))
                return false;
            else if (this.m_Mobile.Target != null)
                this.m_Mobile.Target.Cancel(this.m_Mobile, TargetCancelType.Canceled);

            Mobile target = this.m_Mobile.Combatant;

            if (this.m_Mobile.Combatant is BaseCreature)
            {
                BaseCreature bc = this.m_Mobile.Combatant as BaseCreature;
                target = bc.GetMaster();

                if (target != null && bc.CanBeHarmful(target))
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.Say(1162, "Provocation: Pet to Master");

                    bc.Provoke(this.m_Mobile, target, true);
                    return true;
                }
            }

            List<BaseCreature> list = new List<BaseCreature>();

            foreach (Mobile m in this.m_Mobile.GetMobilesInRange(5))
            {
                if (m != null && m is BaseCreature && m != this.m_Mobile)
                {
                    BaseCreature bc = m as BaseCreature;

                    if (this.m_Mobile.Controlled != bc.Controlled)
                        continue;

                    if (this.m_Mobile.Summoned != bc.Summoned)
                        continue;

                    list.Add(bc);
                }
            }

            if (list.Count == 0)
                return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CanBeHarmful(target))
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.Say(1162, "Provocation: " + list[i].Name + " to " + target.Name);

                    list[i].Provoke(this.m_Mobile, target, true);
                    return true;
                }
            }

            return false;
        }

        public void UsePeacemaking()
        {
            if (!this.m_Mobile.UseSkill(SkillName.Peacemaking))
                return;

            if (this.m_Mobile.Combatant is PlayerMobile)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1162, "Peacemaking: Player");

                PlayerMobile pm = this.m_Mobile.Combatant as PlayerMobile;

                if (pm.PeacedUntil <= DateTime.UtcNow)
                {
                    pm.PeacedUntil = DateTime.UtcNow + TimeSpan.FromSeconds((int)(this.m_Mobile.Skills[SkillName.Peacemaking].Value / 5));
                    pm.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!					
                }
            }
            else if (this.m_Mobile.Target != null)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1162, "Peacemaking");

                this.m_Mobile.Target.Invoke(this.m_Mobile, this.m_Mobile.Combatant);
            }
        }
    }
}