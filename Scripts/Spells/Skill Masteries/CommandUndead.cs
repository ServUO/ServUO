using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Spells.SkillMasteries
{
    public class CommandUndeadSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Command Undead", "In Corp Xen Por",
                204,
                9061,
                Reagent.DaemonBlood,
                Reagent.PigIron,
                Reagent.BatWing
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 0;
        public override int RequiredMana => 40;
        public override bool PartyEffects => false;

        public override SkillName CastSkill => SkillName.Necromancy;
        public override SkillName DamageSkill => SkillName.SpiritSpeak;

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(3.0);

        public CommandUndeadSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new MasteryTarget(this);
        }

        protected override void OnTarget(object o)
        {
            BaseCreature bc = o as BaseCreature;

            if (bc == null || !Caster.CanSee(bc.Location) || !Caster.InLOS(bc))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (ValidateTarget(bc))
            {
                if (Caster.Followers + 2 > Caster.FollowersMax)
                {
                    Caster.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                }
                else if (bc.Controlled || bc.Summoned)
                {
                    Caster.SendLocalizedMessage(1156015); // You cannot command that!
                }
                else if (CheckSequence())
                {
                    double difficulty = BaseInstrument.GetBaseDifficulty(bc);
                    double skill = ((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 2) + (GetMasteryLevel() * 3) + 1;

                    double chance = (skill - (difficulty - 25)) / ((difficulty + 25) - (difficulty - 25));

                    if (chance >= Utility.RandomDouble())
                    {
                        bc.ControlSlots = 2;
                        bc.Combatant = null;

                        if (Caster.Combatant == bc)
                        {
                            Caster.Combatant = null;
                            Caster.Warmode = false;
                        }

                        if (bc.SetControlMaster(Caster))
                        {
                            bc.PlaySound(0x5C4);
                            bc.Allured = true;

                            Container pack = bc.Backpack;

                            if (pack != null)
                            {
                                for (int i = pack.Items.Count - 1; i >= 0; --i)
                                {
                                    if (i >= pack.Items.Count)
                                        continue;

                                    pack.Items[i].Delete();
                                }
                            }

                            if (bc is SkeletalDragon)
                            {
                                Engines.Quests.Doom.BellOfTheDead.TryRemoveDragon((SkeletalDragon)bc);
                            }

                            Caster.PlaySound(0x5C4);
                            Caster.SendLocalizedMessage(1156013); // You command the undead to follow and protect you.
                        }
                    }
                    else
                    {
                        Caster.SendLocalizedMessage(1156014); // The undead becomes enraged by your command attempt and attacks you.
                    }
                }
            }
            else
                Caster.SendLocalizedMessage(1156015); // You cannot command that!

            //FinishSequence();
        }

        public static Type[] CommandTypes => _CommandTypes;
        public static Type[] NoCommandTypes => _NoCommandTypes;

        private static readonly Type[] _CommandTypes =
        {
            typeof(SkeletalDragon)
        };

        private static readonly Type[] _NoCommandTypes =
        {

            typeof(UnfrozenMummy),
            typeof(RedDeath),
            typeof(SirPatrick),
            typeof(LadyJennifyr),
            typeof(MasterMikael),
            typeof(MasterJonath),
            typeof(LadyMarai),
            typeof(Niporailem),
            typeof(PestilentBandage),
        };

        public static bool ValidateTarget(BaseCreature bc)
        {
            if (bc is BaseRenowned || bc is BaseChampion || bc is Engines.Shadowguard.ShadowguardBoss)
                return false;

            foreach (Type t in _CommandTypes)
            {
                if (t == bc.GetType())
                    return true;
            }

            foreach (Type t in _NoCommandTypes)
            {
                if (t == bc.GetType())
                    return false;
            }

            SlayerEntry entry = SlayerGroup.GetEntryByName(SlayerName.Silver);

            return entry != null && entry.Slays(bc);
        }
    }
}
