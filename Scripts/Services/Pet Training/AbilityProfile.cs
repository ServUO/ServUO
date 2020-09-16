using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [PropertyObject]
    public class AbilityProfile
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public MagicalAbility MagicalAbility { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public AreaEffect[] AreaEffects { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SpecialAbility[] SpecialAbilities { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAbility[] WeaponAbilities { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TokunoTame { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RegenHits { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RegenStam { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RegenMana { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageIndex { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature Creature { get; private set; }

        public List<object> Advancements { get; private set; }

        public AbilityProfile(BaseCreature bc)
        {
            Creature = bc;
            DamageIndex = -1;
        }

        public void OnTame()
        {
            if (Creature.ControlMaster is PlayerMobile)
            {
                Engines.Quests.TamingPetQuest.CheckTame((PlayerMobile)Creature.ControlMaster);
            }

            if (Creature.Map == Map.Tokuno)
            {
                TokunoTame = true;
            }
        }

        public bool AddAbility(MagicalAbility ability, bool advancement = true)
        {
            if (Creature.Controlled)
            {
                MagicalAbility oldAbility = MagicalAbility;

                if (IsSpecialMagicalAbility(oldAbility))
                {
                    RemoveSpecialMagicalAbility(oldAbility);
                }

                OnRemoveMagicalAbility(oldAbility, ability);

                MagicalAbility = ability;
            }
            else
            {
                MagicalAbility |= ability;
            }

            OnAddAbility(ability, advancement);
            return true;
        }

        public bool AddAbility(SpecialAbility ability, bool advancement = true)
        {
            if (SpecialAbilities == null)
            {
                SpecialAbilities = new SpecialAbility[] { ability };
            }
            else if (!SpecialAbilities.Any(a => a == ability))
            {
                SpecialAbility[] temp = SpecialAbilities;

                SpecialAbilities = new SpecialAbility[temp.Length + 1];

                for (int i = 0; i < temp.Length; i++)
                    SpecialAbilities[i] = temp[i];

                SpecialAbilities[temp.Length] = ability;
            }

            OnAddAbility(ability, advancement);

            return true;
        }

        public bool AddAbility(AreaEffect ability, bool advancement = true)
        {
            if (AreaEffects == null)
            {
                AreaEffects = new AreaEffect[] { ability };
            }
            else if (!AreaEffects.Any(a => a == ability))
            {
                AreaEffect[] temp = AreaEffects;

                AreaEffects = new AreaEffect[temp.Length + 1];

                for (int i = 0; i < temp.Length; i++)
                    AreaEffects[i] = temp[i];

                AreaEffects[temp.Length] = ability;
            }

            OnAddAbility(ability, advancement);

            return true;
        }

        public bool AddAbility(WeaponAbility ability, bool advancement = true)
        {
            if (WeaponAbilities == null)
            {
                WeaponAbilities = new WeaponAbility[] { ability };
            }
            else if (!WeaponAbilities.Any(a => a == ability))
            {
                WeaponAbility[] temp = WeaponAbilities;

                WeaponAbilities = new WeaponAbility[temp.Length + 1];

                for (int i = 0; i < temp.Length; i++)
                    WeaponAbilities[i] = temp[i];

                WeaponAbilities[temp.Length] = ability;
            }

            OnAddAbility(ability, advancement);

            return true;
        }

        public void RemoveAbility(MagicalAbility ability)
        {
            if ((MagicalAbility & ability) != 0)
            {
                MagicalAbility ^= ability;
                RemovePetAdvancement(ability);
            }
        }

        public void RemoveAbility(SpecialAbility ability)
        {
            if (SpecialAbilities == null || !SpecialAbilities.Any(a => a == ability))
                return;

            List<SpecialAbility> list = SpecialAbilities.ToList();

            list.Remove(ability);
            RemovePetAdvancement(ability);

            SpecialAbilities = list.ToArray();

            ColUtility.Free(list);
        }

        public void RemoveAbility(WeaponAbility ability)
        {
            if (WeaponAbilities == null || !WeaponAbilities.Any(a => a == ability))
                return;

            List<WeaponAbility> list = WeaponAbilities.ToList();

            list.Remove(ability);
            RemovePetAdvancement(ability);

            WeaponAbilities = list.ToArray();

            ColUtility.Free(list);
        }

        public void RemoveAbility(AreaEffect ability)
        {
            if (AreaEffects == null || !AreaEffects.Any(a => a == ability))
                return;

            List<AreaEffect> list = AreaEffects.ToList();

            list.Remove(ability);
            RemovePetAdvancement(ability);

            AreaEffects = list.ToArray();

            ColUtility.Free(list);
        }

        public bool AddAbility(SkillName skill, bool advancement = true)
        {
            OnAddAbility(skill, advancement);
            return true;
        }

        public bool CanAddAbility(object o)
        {
            if (!Creature.Controlled)
                return true;

            if (o is MagicalAbility)
                return true;

            if (o is SpecialAbility && (SpecialAbilities == null || SpecialAbilities.Length == 0))
                return true;

            if (o is AreaEffect && (AreaEffects == null || AreaEffects.Length == 0))
                return true;

            if (o is WeaponAbility && (WeaponAbilities == null || WeaponAbilities.Length < 2))
                return true;

            return false;
        }

        public void OnAddAbility(object newAbility, bool advancement)
        {
            if (advancement)
            {
                AddPetAdvancement(newAbility);
            }

            if (newAbility is MagicalAbility)
            {
                AddMagicalAbility((MagicalAbility)newAbility);
            }

            TrainingPoint trainPoint = PetTrainingHelper.GetTrainingPoint(newAbility);

            if (trainPoint != null && trainPoint.Requirements != null)
            {
                foreach (TrainingPointRequirement req in trainPoint.Requirements.Where(r => r != null))
                {
                    if (req.Requirement is SkillName)
                    {
                        double skill = Creature.Skills[(SkillName)req.Requirement].Base;
                        double toAdd = req.Cost == 100 ? 20 : 40;

                        if ((SkillName)req.Requirement == SkillName.Hiding)
                            toAdd = 100;

                        if (skill < toAdd)
                            Creature.Skills[(SkillName)req.Requirement].Base = toAdd;
                    }
                    else if (req.Requirement is WeaponAbility)
                    {
                        AddAbility((WeaponAbility)req.Requirement);
                    }
                }
            }
        }

        public void AddPetAdvancement(object o)
        {
            if (Creature.Controlled)
            {
                if (Advancements == null)
                    Advancements = new List<object>();

                if (!Advancements.Contains(o))
                {
                    Advancements.Add(o);
                }
            }
        }

        public void RemovePetAdvancement(object o)
        {
            if (Creature.Controlled && Advancements != null && Advancements.Contains(o))
            {
                Advancements.Remove(o);
            }
        }

        public bool HasAbility(object o)
        {
            if (o is MagicalAbility)
            {
                return (MagicalAbility & (MagicalAbility)o) != 0;
            }

            if (o is SpecialAbility && SpecialAbilities != null)
            {
                return SpecialAbilities.Any(a => a == (SpecialAbility)o);
            }

            if (o is AreaEffect && AreaEffects != null)
            {
                return AreaEffects.Any(a => a == (AreaEffect)o);
            }

            if (o is WeaponAbility && WeaponAbilities != null)
            {
                return WeaponAbilities.Any(a => a == (WeaponAbility)o);
            }

            return false;
        }

        public int AbilityCount()
        {
            int count = 0;

            if (MagicalAbility != MagicalAbility.None)
                count++;

            if (SpecialAbilities != null)
                count += SpecialAbilities.Where(a => !a.NaturalAbility).Count();

            if (AreaEffects != null)
                count += AreaEffects.Length;

            if (WeaponAbilities != null)
                count += WeaponAbilities.Length;

            return count;
        }

        public bool CanChooseMagicalAbility(MagicalAbility ability)
        {
            if (!Creature.Controlled)
                return true;

            if (HasSpecialMagicalAbility() &&
                IsSpecialMagicalAbility(ability) &&
                SpecialAbilities != null &&
                SpecialAbilities.Length > 0 &&
                SpecialAbilities.Any(a => !a.NaturalAbility))
            {
                return false;
            }

            return true;
        }

        public bool CanChooseSpecialAbility(SpecialAbility[] list)
        {
            if (!Creature.Controlled)
                return true;

            if (HasSpecialMagicalAbility() &&
                list.Any(abil => IsRuleBreaker(abil)) &&
                (AreaEffects == null || AreaEffects.Length == 0) &&
                (SpecialAbilities == null || SpecialAbilities.Length == 0 || SpecialAbilities.All(a => a.NaturalAbility)))
                return true;

            return !HasSpecialMagicalAbility() && (SpecialAbilities == null || SpecialAbilities.Where(a => !a.NaturalAbility).Count() == 0) && AbilityCount() < 3;
        }

        public bool IsRuleBreaker(SpecialAbility ability)
        {
            return PetTrainingHelper.RuleBreakers.Any(abil => abil == ability);
        }

        public bool CanChooseAreaEffect()
        {
            if (!Creature.Controlled)
                return true;

            if (HasSpecialMagicalAbility() && (AreaEffects == null || AreaEffects.Length == 0) && (SpecialAbilities == null || SpecialAbilities.Length == 0))
                return true;

            return !HasSpecialMagicalAbility() && (AreaEffects == null || AreaEffects.Length == 0) && AbilityCount() < 3;
        }

        public bool CanChooseWeaponAbility()
        {
            if (!Creature.Controlled)
                return true;

            return !HasSpecialMagicalAbility() && (WeaponAbilities == null || WeaponAbilities.Length < 2) && AbilityCount() < 3;
        }

        public bool HasSpecialMagicalAbility()
        {
            return (MagicalAbility & MagicalAbility.Piercing) != 0 ||
                (MagicalAbility & MagicalAbility.Bashing) != 0 ||
                (MagicalAbility & MagicalAbility.Slashing) != 0 ||
                (MagicalAbility & MagicalAbility.BattleDefense) != 0 ||
                (MagicalAbility & MagicalAbility.WrestlingMastery) != 0;
        }

        public bool IsSpecialMagicalAbility(MagicalAbility ability)
        {
            return ability != MagicalAbility.None && ability <= MagicalAbility.WrestlingMastery;
        }

        public void AddMagicalAbility(MagicalAbility ability)
        {
            if (IsSpecialMagicalAbility(ability))
            {
                //SpecialAbilities = null;
                WeaponAbilities = null;
                Creature.AI = AIType.AI_Melee;
            }

            switch (ability)
            {
                case MagicalAbility.Piercing:
                    Creature.Mastery = SkillName.Fencing;
                    break;
                case MagicalAbility.Bashing:
                    Creature.Mastery = SkillName.Macing;
                    break;
                case MagicalAbility.Slashing:
                    Creature.Mastery = SkillName.Swords;
                    break;
                case MagicalAbility.BattleDefense:
                    Creature.Mastery = SkillName.Parry;
                    break;
                case MagicalAbility.WrestlingMastery:
                    Creature.Mastery = SkillName.Wrestling;
                    break;
                case MagicalAbility.Poisoning:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Melee)
                        Creature.AI = AIType.AI_Melee;
                    break;
                case MagicalAbility.Bushido:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Samurai)
                        Creature.AI = AIType.AI_Samurai;
                    if (!HasAbility(WeaponAbility.WhirlwindAttack))
                    {
                        AddAbility(WeaponAbility.WhirlwindAttack, false);
                    }
                    break;
                case MagicalAbility.Ninjitsu:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Ninja)
                        Creature.AI = AIType.AI_Ninja;
                    if (!HasAbility(WeaponAbility.FrenziedWhirlwind))
                    {
                        AddAbility(WeaponAbility.FrenziedWhirlwind, false);
                    }
                    break;
                case MagicalAbility.Discordance:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Melee)
                        Creature.AI = AIType.AI_Melee;
                    break;
                case MagicalAbility.Magery:
                case MagicalAbility.MageryMastery:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Mage)
                        Creature.AI = AIType.AI_Mage;
                    break;
                case MagicalAbility.Mysticism:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Mystic)
                        Creature.AI = AIType.AI_Mystic;
                    break;
                case MagicalAbility.Spellweaving:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Spellweaving)
                        Creature.AI = AIType.AI_Spellweaving;
                    break;
                case MagicalAbility.Chivalry:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Paladin)
                        Creature.AI = AIType.AI_Paladin;
                    break;
                case MagicalAbility.Necromage:
                    if (Creature.Controlled && Creature.AI != AIType.AI_NecroMage)
                        Creature.AI = AIType.AI_NecroMage;
                    break;
                case MagicalAbility.Necromancy:
                    if (Creature.Controlled && Creature.AI != AIType.AI_Necro)
                        Creature.AI = AIType.AI_Necro;
                    break;
            }
        }

        public void OnRemoveMagicalAbility(MagicalAbility oldAbility, MagicalAbility newAbility)
        {
            if ((oldAbility & MagicalAbility.Bushido) != 0)
            {
                if (HasAbility(WeaponAbility.WhirlwindAttack))
                {
                    RemoveAbility(WeaponAbility.WhirlwindAttack);
                }
            }

            if ((oldAbility & MagicalAbility.Ninjitsu) != 0)
            {
                if (HasAbility(WeaponAbility.FrenziedWhirlwind))
                {
                    RemoveAbility(WeaponAbility.FrenziedWhirlwind);
                }
            }
        }

        public void RemoveSpecialMagicalAbility(MagicalAbility ability)
        {
            //SpecialAbilities = null;
            WeaponAbilities = null;

            Creature.Mastery = SkillName.Alchemy; // default
        }

        public bool HasCustomized()
        {
            return Advancements != null && Advancements.Count > 0;
        }

        public bool IsNaturalAbility(object o)
        {
            if (Advancements == null)
                return true;

            if (o is SpecialAbility)
            {
                return SpecialAbilities != null && !Advancements.Any(s => s is SpecialAbility && (SpecialAbility)s == (SpecialAbility)o);
            }

            if (o is WeaponAbility)
            {
                return WeaponAbilities != null && !Advancements.Any(s => s is WeaponAbility && (WeaponAbility)s == (WeaponAbility)o);
            }

            return false;
        }

        public IEnumerable<object> EnumerateAllAbilities()
        {
            if (MagicalAbility != MagicalAbility.None)
            {
                foreach (MagicalAbility abil in PetTrainingHelper.MagicalAbilities)
                {
                    if ((MagicalAbility & abil) != 0)
                        yield return abil;
                }
            }

            if (SpecialAbilities != null)
            {
                foreach (SpecialAbility abil in SpecialAbilities)
                {
                    yield return abil;
                }
            }

            if (AreaEffects != null)
            {
                foreach (AreaEffect effect in AreaEffects)
                {
                    yield return effect;
                }
            }

            if (WeaponAbilities != null)
            {
                foreach (WeaponAbility abil in WeaponAbilities)
                {
                    yield return abil;
                }
            }
        }

        public IEnumerable<SpecialAbility> EnumerateSpecialAbilities()
        {
            if (SpecialAbilities == null)
            {
                yield break;
            }

            foreach (SpecialAbility ability in SpecialAbilities)
            {
                yield return ability;
            }
        }

        public SpecialAbility[] GetSpecialAbilities()
        {
            return EnumerateSpecialAbilities().ToArray();
        }

        public IEnumerable<AreaEffect> EnumerateAreaEffects()
        {
            if (AreaEffects == null)
            {
                yield break;
            }

            foreach (AreaEffect ability in AreaEffects)
            {
                yield return ability;
            }
        }

        public AreaEffect[] GetAreaEffects()
        {
            return EnumerateAreaEffects().ToArray();
        }

        public override string ToString()
        {
            return "...";
        }

        public AbilityProfile(BaseCreature bc, GenericReader reader)
        {
            int version = reader.ReadInt();

            Creature = bc;

            switch (version)
            {
                case 0:
                    DamageIndex = -1;
                    break;
                case 1:
                    DamageIndex = reader.ReadInt();
                    break;
            }

            MagicalAbility = (MagicalAbility)reader.ReadInt();
            TokunoTame = reader.ReadBool();

            RegenHits = reader.ReadInt();
            RegenStam = reader.ReadInt();
            RegenMana = reader.ReadInt();

            int count = reader.ReadInt();
            SpecialAbilities = new SpecialAbility[count];

            for (int i = 0; i < count; i++)
            {
                SpecialAbilities[i] = SpecialAbility.Abilities[reader.ReadInt()];
            }

            count = reader.ReadInt();
            AreaEffects = new AreaEffect[count];

            for (int i = 0; i < count; i++)
            {
                AreaEffects[i] = AreaEffect.Effects[reader.ReadInt()];
            }

            count = reader.ReadInt();
            WeaponAbilities = new WeaponAbility[count];

            for (int i = 0; i < count; i++)
            {
                WeaponAbilities[i] = WeaponAbility.Abilities[reader.ReadInt()];
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                if (Advancements == null)
                    Advancements = new List<object>();

                switch (reader.ReadInt())
                {
                    case 1: Advancements.Add((MagicalAbility)reader.ReadInt()); break;
                    case 2: Advancements.Add(SpecialAbility.Abilities[reader.ReadInt()]); break;
                    case 3: Advancements.Add(AreaEffect.Effects[reader.ReadInt()]); break;
                    case 4: Advancements.Add(WeaponAbility.Abilities[reader.ReadInt()]); break;
                    case 5: Advancements.Add((SkillName)reader.ReadInt()); break;
                }
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(DamageIndex);

            writer.Write((int)MagicalAbility);
            writer.Write(TokunoTame);

            writer.Write(RegenHits);
            writer.Write(RegenStam);
            writer.Write(RegenMana);

            writer.Write(SpecialAbilities != null ? SpecialAbilities.Length : 0);

            if (SpecialAbilities != null)
            {
                foreach (SpecialAbility abil in SpecialAbilities)
                {
                    writer.Write(Array.IndexOf(SpecialAbility.Abilities, abil));
                }
            }

            writer.Write(AreaEffects != null ? AreaEffects.Length : 0);

            if (AreaEffects != null)
            {
                foreach (AreaEffect abil in AreaEffects)
                {
                    writer.Write(Array.IndexOf(AreaEffect.Effects, abil));
                }
            }

            writer.Write(WeaponAbilities != null ? WeaponAbilities.Length : 0);

            if (WeaponAbilities != null)
            {
                foreach (WeaponAbility abil in WeaponAbilities)
                {
                    writer.Write(Array.IndexOf(WeaponAbility.Abilities, abil));
                }
            }

            writer.Write(Advancements != null ? Advancements.Count : 0);

            if (Advancements != null)
            {
                foreach (object o in Advancements)
                {
                    if (o is MagicalAbility)
                    {
                        writer.Write(1);
                        writer.Write((int)(MagicalAbility)o);
                    }
                    else if (o is SpecialAbility)
                    {
                        writer.Write(2);
                        writer.Write(Array.IndexOf(SpecialAbility.Abilities, (SpecialAbility)o));
                    }
                    else if (o is AreaEffect)
                    {
                        writer.Write(3);
                        writer.Write(Array.IndexOf(AreaEffect.Effects, (AreaEffect)o));
                    }
                    else if (o is WeaponAbility)
                    {
                        writer.Write(4);
                        writer.Write(Array.IndexOf(WeaponAbility.Abilities, (WeaponAbility)o));
                    }
                    else if (o is SkillName)
                    {
                        writer.Write(5);
                        writer.Write((int)(SkillName)o);
                    }
                    else
                    {
                        writer.Write(0);
                    }
                }
            }
        }
    }
}
