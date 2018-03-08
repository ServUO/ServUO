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
        public AreaEffect AreaEffects { get; private set; }

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
            var old = MagicalAbilities;

            if (Creature.Controlled)
            {
                History.Add(ability);
                MagicalAbility = ability;
                OnAddAbility(old, ability);
                return true;
            }
            else
            {
                MagicalAbility |= ability;
                OnAddAbility(old, ability);
                return true;
            }

            return false;
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

                SpecialAbilities[temp.Length - 1] = ability;
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
                var temp = SpecialAbilities;

                AreaEffects = new AreaEffect[temp.Length + 1];

                for (int i = 0; i < temp.Length; i++)
                    AreaEffects[i] = temp[i];

                AreaEffects[temp.Length - 1] = ability;
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
            if (!Controlled)
                return true;

            if (o is MagicalAbility)
                return true;

            if (o is SpecialAbility && (SpecialAbilities == null || SpecialAbilities.Length == 0))
                return true;

            if (o is AreaEffect && AreaEffects == AreaEffect.None)
                return true;

            if (o is WeaponAbility && (WeaponAbilities == null || WeaponAbilities.Length < 2))
                return true;

            return false;
        }

        public void OnAddAbility(object oldAbility, object newAbility)
        {
            AddToHistory(newAbility);

            var trainPoint = PetTrainingHelper.GetTrainingPoint(newAbility);

            if (trainPoint != null && trainPoint.Requirements != null)
            {
                foreach (var req in trainPoint.Requirements)
                {
                    // Verified on EA that skill is not zeroed out
                    /*if (req is SkillName)
                    {
                        Creature.Skills[(SkillName)req].Base = 0;
                    }
                    else */
                    if (req is WeaponAbility)
                    {
                        AddAbility((WeaponAbility)req);
                    }
                }
            }

            if (newAbility is MagicalAbility && (MagicalAbility)newAbility <= MagicalAbility.WrestlingMastery)
            {
                AddSpecialMagicalAbility((MagicalAbility)newAbility);
            }

            var trainPoint = PetTrainingHelper.GetTrainingPoint(newAbility);

            if (trainPoint != null && trainPoint.Requirements != null)
            {
                foreach (var req in trainPoint.Requirements)
                {
                    if (req is SkillName)
                    {
                        double skill = Creature.Skills[(SkillName)req].Base;
                        double toAdd = req.Cost == 500 ? 40 : 20;

                        if (skill < 20)
                            Creature.Skills[(SkillName)req].Base = 20;
                    }
                    else if (req is WeaponAbility)
                    {
                        AddAbility((WeaponAbility)req);
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
                return (MagicalAbilities & (MagicAbility)o) != 0;
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

            if (SpecialAbility != null)
                count += SpecialAbilities.Sum(a => a != null);

            if (AreaEffect != AreaEffect.None)
                count++;

            if (WeaponAbilities != null)
                count += WeaponAbilities.Sum(a => a != null);

            return count;
        }

        public bool CanChooseSpecialAbility()
        {
            if (!Creature.Controlled)
                return true;

            return !HasSpecialMagicalAbility() && (SpecialAbilities == null || SpecialAbilities.Length == 0) && AbilityCount() < 3;
        }

        public bool CanChooseAreaEffect()
        {
            if (!Creature.Controlled)
                return true;

            return !HasSpecialMagicalAbility() && AreaEffect == AreaEffect.None && AbilityCount() < 3;
        }

        public bool CanChooseWeaponAbility()
        {
            if (!Creature.Controlled)
                return true;

            return !HasSpecialMagicalAbility() && (WeaponAbilities == null || WeaponAbilities.Length < 2) && AbilityCount() < 3;
        }

        public bool HasSpecialMagicalAbility()
        {
            return (MagicalAbilities & MagicalAbility.Piercing) != 0 ||
                (MagicalAbilities & MagicalAbility.Bashing) != 0 ||
                (MagicalAbilities & MagicalAbility.Slashing) != 0 ||
                (MagicalAbilities & MagicalAbility.BattleDefense) != 0 ||
                (MagicalAbilities & MagicalAbility.WrestlingMastery) != 0;
        }

        public void AddSpecialMagicalAbility(MagicalAbility ability)
        {
            SpecialAbilities = null;
            WeaponAbilities = null;

            switch ((MagicalAbility)ability)
            {
                case MagicalAbility.Piercing: Creature.Mastery = SkillName.Fencing; break;
                case MagicalAbility.Bashing: Creature.Mastery = SkillName.Macing; break;
                case MagicalAbility.Slashing: Creature.Mastery = SkillName.Swords; break;
                case MagicalAbility.BattleDefense: Creature.Mastery = SkillName.Parrying; break;
                case MagicalAbility.WrestlingMastery: Creature.Mastery = SkillName.Wrestling; break;
            }
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
            var profile = PetTrainingHelper.GetProfile(this);

            if (pofile == null)
            {
                yield break;
            }

            foreach (int i in Enum.GetValues(typeof(SpecialAbility)))
            {
                if (profile.HasAbility((SpecialAbility)i))
                    yield return ability;
            }
        }

        public SpecialAbility[] GetSpecialAbilities()
        {
            return EnumerateSpecialAbilities().ToArray();
        }

        public IEnumerable<AreaEffect> EnumerateAreaEffects()
        {
            var profile = PetTrainingHelper.GetProfile(this);

            if (pofile == null)
            {
                yield break;
            }

            foreach (int i in Enum.GetValues(typeof(AreaEffect)))
            {
                if (profile.HasAbility((AreaEffect)i))
                    yield return ability;
            }
        }

        public AreaEffect[] GetAreaEffects()
        {
            return EnumerateAreaEffects().ToArray();
        }

        public AbilityProfile(BaseCreature bc, GenericReader reader)
        {
            int version = reader.ReadInt();

            Creature = bc;

            MagicalAbilities = (MagicalAbility)reader.ReadInt();
            TokunoTame = reader.ReadBool();

            int count = reader.ReadInt();
            SpecialAbilities = new SpecialAbilities[count];

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
            WeaponAbilities = new WeaponAbilities[count];

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
                    case 2: History.Add((SpecialAbility)reader.ReadInt()); break;
                    case 3: History.Add((AreaEffect)reader.ReadInt()); break;
                    case 4: History.Add(WeaponAbility.Abilities[reader.ReadInt()]); break;
                }
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)MagicalAbilities);
            writer.Write(TokunoTame);

            writer.Write(SpecialAbilities != null ? SpecialAbilities.Length : 0);
            foreach (var abil in SpecialAbilities)
            {
                writer.Write(Array.IndexOf(SpecialAbility.Abilities, abil));
            }

            writer.Write(AreaEffects != null ? AreaEffects.Length : 0);
            foreach (var abil in AreaEffects)
            {
                writer.Write(Array.IndexOf(AreaEffect.Effects, abil));
            }

            writer.Write(WeaponAbilities != null ? WeaponAbilities.Length : 0);
            foreach (var abil in WeaponAbilities)
            {
                writer.Write(Array.IndexOf(WeaponAbility.Abilities, abil));
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
                        writer.Write((int)(SpecialAbility)o);
                    }
                    else if (o is AreaEffect)
                    {
                        writer.Write(3);
                        writer.Write((int)(AreaEffect)o);
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
