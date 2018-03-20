using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Items;

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
        public BaseCreature Creature { get; private set; }

        public List<object> History { get; private set; }

        public AbilityProfile(BaseCreature bc)
        {
            Creature = bc;
        }

        public void OnTame()
        {
            if (Creature.Map == Map.Tokuno)
            {
                TokunoTame = true;
            }
        }

        public bool AddAbility(MagicalAbility ability)
        {
            if (Creature.Controlled)
            {
                var oldAbility = MagicalAbility;

                if (IsSpecialMagicalAbility(oldAbility))
                {
                    RemoveSpecialMagicalAbility(oldAbility);
                }

                AddToHistory(ability);
                MagicalAbility = ability;
                OnAddAbility(ability);
                return true;
            }
            else
            {
                MagicalAbility |= ability;
                OnAddAbility(ability);
                return true;
            }
        }

        public bool AddAbility(SpecialAbility ability)
        {
            if (SpecialAbilities == null)
            {
                SpecialAbilities = new SpecialAbility[] { ability };
            }
            else if (!SpecialAbilities.Any(a => a == ability))
            {
                var temp = SpecialAbilities;

                SpecialAbilities = new SpecialAbility[temp.Length + 1];

                for (int i = 0; i < temp.Length; i++)
                    SpecialAbilities[i] = temp[i];

                SpecialAbilities[temp.Length] = ability;
            }

            OnAddAbility(ability);

            return true;
        }

        public bool AddAbility(AreaEffect ability)
        {
            if (AreaEffects == null)
            {
                AreaEffects = new AreaEffect[] { ability };
            }
            else if (!AreaEffects.Any(a => a == ability))
            {
                var temp = AreaEffects;

                AreaEffects = new AreaEffect[temp.Length + 1];

                for (int i = 0; i < temp.Length; i++)
                    AreaEffects[i] = temp[i];

                AreaEffects[temp.Length] = ability;
            }

            OnAddAbility(ability);

            return true;
        }

        public bool AddAbility(WeaponAbility ability)
        {
            if (WeaponAbilities == null)
            {
                WeaponAbilities = new WeaponAbility[] { ability };
            }
            else if(!WeaponAbilities.Any(a => a == ability))
            {
                var temp = WeaponAbilities;

                WeaponAbilities = new WeaponAbility[temp.Length + 1];

                for (int i = 0; i < temp.Length; i++)
                    WeaponAbilities[i] = temp[i];

                WeaponAbilities[temp.Length] = ability;
            }

            OnAddAbility(ability);

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

        public void OnAddAbility(object newAbility)
        {
            AddToHistory(newAbility);

            if (newAbility is MagicalAbility)
            {
                AddMagicalAbility((MagicalAbility)newAbility);
            }

            var trainPoint = PetTrainingHelper.GetTrainingPoint(newAbility);

            if (trainPoint != null && trainPoint.Requirements != null)
            {
                foreach (var req in trainPoint.Requirements.Where(r => r != null))
                {
                    if (req.Requirement is SkillName)
                    {
                        double skill = Creature.Skills[(SkillName)req.Requirement].Base;
                        double toAdd = req.Cost == 100 ? 20 : 40;

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

        public void AddToHistory(object o)
        {
            if (Creature.Controlled)
            {
                if (History == null)
                    History = new List<object>();

                if (History.Contains(o))
                {
                    History.Remove(o);
                    History.Add(o);
                }
                else
                {
                    History.Add(o);
                }
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

        public bool CanChooseSpecialAbility()
        {
            if (!Creature.Controlled)
                return true;

            return !HasSpecialMagicalAbility() && (SpecialAbilities == null || SpecialAbilities.Where(a => !a.NaturalAbility).Count() == 0) && AbilityCount() < 3;
        }

        public bool CanChooseAreaEffect()
        {
            if (!Creature.Controlled)
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
                    if(Creature.AI != AIType.AI_Melee) 
                        Creature.AI = AIType.AI_Melee;
                    break;
                case MagicalAbility.Bushido:
                    if (Creature.AI != AIType.AI_Samurai) 
                        Creature.AI = AIType.AI_Samurai;
                    break;
                case MagicalAbility.Ninjitsu:
                    if (Creature.AI != AIType.AI_Ninja) 
                        Creature.AI = AIType.AI_Ninja;
                    break;
                case MagicalAbility.Discordance:
                    if (Creature.AI != AIType.AI_Melee) 
                        Creature.AI = AIType.AI_Melee;
                    break;
                case MagicalAbility.Magery:
                case MagicalAbility.MageryMastery:
                    if (Creature.AI != AIType.AI_Mage) 
                        Creature.AI = AIType.AI_Mage;
                    break;
                case MagicalAbility.Mysticism:
                    if (Creature.AI != AIType.AI_Mystic) 
                        Creature.AI = AIType.AI_Mystic;
                    break;
                case MagicalAbility.Spellweaving:
                    if (Creature.AI != AIType.AI_Spellweaving) 
                        Creature.AI = AIType.AI_Spellweaving;
                    break;
                case MagicalAbility.Chivalry:
                    if (Creature.AI != AIType.AI_Paladin)
                        Creature.AI = AIType.AI_Paladin;
                    break;
                case MagicalAbility.Necromage:
                    if (Creature.AI != AIType.AI_NecroMage) 
                        Creature.AI = AIType.AI_NecroMage;
                    break;
                case MagicalAbility.Necromancy:
                    if (Creature.AI != AIType.AI_Necro) 
                        Creature.AI = AIType.AI_Necro;
                    break;
            }
        }

        public void RemoveSpecialMagicalAbility(MagicalAbility ability)
        {
            //SpecialAbilities = null;
            WeaponAbilities = null;

            Creature.Mastery = SkillName.Alchemy; // default
        }

        public IEnumerable<object> EnumerateAllAbilities()
        {
            if (MagicalAbility != MagicalAbility.None)
            {
                yield return MagicalAbility;
            }

            if (SpecialAbilities != null)
            {
                foreach (var abil in SpecialAbilities)
                {
                    yield return abil;
                }
            }

            if (AreaEffects != null)
            {
                foreach (var effect in AreaEffects)
                {
                    yield return effect;
                }
            }

            if (WeaponAbilities != null)
            {
                foreach (var abil in WeaponAbilities)
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

            var profile = PetTrainingHelper.GetAbilityProfile(Creature);

            if (profile == null)
            {
                yield break;
            }

            foreach (var ability in SpecialAbilities)
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

            var profile = PetTrainingHelper.GetAbilityProfile(Creature);

            if (profile == null)
            {
                yield break;
            }

            foreach (var ability in AreaEffects)
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
                History = new List<object>();

                switch (reader.ReadInt())
                {
                    case 1: History.Add((MagicalAbility)reader.ReadInt()); break;
                    case 2: History.Add(SpecialAbility.Abilities[reader.ReadInt()]); break;
                    case 3: History.Add(AreaEffect.Effects[reader.ReadInt()]); break;
                    case 4: History.Add(WeaponAbility.Abilities[reader.ReadInt()]); break;
                }
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)MagicalAbility);
            writer.Write(TokunoTame);

            writer.Write(RegenHits);
            writer.Write(RegenStam);
            writer.Write(RegenMana);

            writer.Write(SpecialAbilities != null ? SpecialAbilities.Length : 0);

            if (SpecialAbilities != null)
            {
                foreach (var abil in SpecialAbilities)
                {
                    writer.Write(Array.IndexOf(SpecialAbility.Abilities, abil));
                }
            }

            writer.Write(AreaEffects != null ? AreaEffects.Length : 0);

            if (AreaEffects != null)
            {
                foreach (var abil in AreaEffects)
                {
                    writer.Write(Array.IndexOf(AreaEffect.Effects, abil));
                }
            }

            writer.Write(WeaponAbilities != null ? WeaponAbilities.Length : 0);

            if (WeaponAbilities != null)
            {
                foreach (var abil in WeaponAbilities)
                {
                    writer.Write(Array.IndexOf(WeaponAbility.Abilities, abil));
                }
            }

            writer.Write(History != null ? History.Count : 0);

            if (History != null)
            {
                foreach (var o in History)
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
                    else
                    {
                        writer.Write(0);
                    }
                }
            }
        }
    }
}
