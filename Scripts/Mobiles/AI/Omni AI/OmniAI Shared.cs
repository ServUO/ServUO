// Created by Peoharen
using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Mystic;
using Server.Spells.Second;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public DateTime m_NextWeaponSwap;
        public virtual bool m_CanStun
        {
            get
            {
                return (this.m_Mobile is BaseVendor || this.m_Mobile is BaseEscortable || this.m_Mobile is BaseChampion);
            }
        }
        public static bool IsFieldSpell(int ID)
        {
            if (ID >= 14612 && ID <= 14633) //poison field
                return true;
            else if (ID >= 14695 && ID <= 14730) //paralysis field
                return true;
            else if (ID >= 14732 && ID <= 14751) //fire field
                return true;
            else
                return false;
        }

        public bool TryToHeal()
        {
            if (this.m_Mobile.Summoned)
                return false;
            else if (DateTime.UtcNow < this.m_NextHealTime)
                return false;

            int diff = this.m_Mobile.HitsMax - this.m_Mobile.Hits;
            diff = ((this.m_Mobile.HitsMax * (100 - diff)) / 100);
            diff = 100 - diff;

            if ((int)(Utility.RandomDouble() * 100.0) > diff)
                return false;

            Spell spell = null;
            this.m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds(20);

            if (this.m_CanUseMagery)
            {
                if (this.m_Mobile.Poisoned)
                    spell = new CureSpell(this.m_Mobile, null);

                spell = new GreaterHealSpell(this.m_Mobile, null);

                if (spell == null)
                    spell = new HealSpell(this.m_Mobile, null);
            }
            else if (this.m_CanUseNecromancy)
            {
                this.m_Mobile.UseSkill(SkillName.SpiritSpeak);
                this.m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            }
            else if (this.m_CanUseChivalry)
            {
                if (this.m_Mobile.Poisoned)
                    spell = new CleanseByFireSpell(this.m_Mobile, null);
                else
                    spell = new CloseWoundsSpell(this.m_Mobile, null);
            }
            else if (this.m_CanUseMystic)
            {
                spell = new CleansingWindsSpell(this.m_Mobile, null);
            }
            else if (this.m_Mobile.Skills[SkillName.Healing].Value > 10.0)
            {
                int delay = (int)(5.0 + (0.5 * ((120 - this.m_Mobile.Dex) / 10)));
                new BandageContext(this.m_Mobile, this.m_Mobile, TimeSpan.FromSeconds(delay), false);
                this.m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds(delay + 1);
                return true;
            }

            if (spell != null)
                spell.Cast();

            return true;
        }

        public void CheckArmed(bool swap)
        {
            if (DateTime.UtcNow > this.m_NextWeaponSwap)
                return;

            if (!this.m_SwapWeapons)
                return;

            Container pack = this.m_Mobile.Backpack;

            if (pack == null)
            {
                this.m_Mobile.EquipItem(new Backpack());
                pack = this.m_Mobile.Backpack;
            }

            BaseWeapon weapon = this.m_Mobile.Weapon as BaseWeapon;

            if (weapon != null)
            {
                if (!swap)
                    return;

                pack.DropItem(weapon);
                weapon = null;
            }

            if (weapon == null)
            {
                this.m_Mobile.DebugSay("Searching my pack for a weapon.");

                Item[] weapons = pack.FindItemsByType(typeof(BaseMeleeWeapon));

                if (weapons != null && weapons.Length != 0)
                {
                    int max = (weapons.Length == 1) ? 0 : (weapons.Length - 1);
                    int whichone = Utility.RandomMinMax(0, max);
                    this.m_Mobile.EquipItem(weapons[whichone]);
                }
            }

            this.m_NextWeaponSwap = DateTime.UtcNow + TimeSpan.FromSeconds(15);
        }

        public void UseWeaponStrike()
        {
            this.m_Mobile.DebugSay("Picking a weapon move");

            BaseWeapon weapon = this.m_Mobile.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;

            if (weapon == null)
                weapon = this.m_Mobile.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            if (weapon == null)
                return;

            int whichone = Utility.RandomMinMax(1, 2);

            if (whichone >= 2 && this.m_Mobile.Skills[weapon.Skill].Value >= 90.0)
                WeaponAbility.SetCurrentAbility(this.m_Mobile, weapon.PrimaryAbility);
            else if (this.m_Mobile.Skills[weapon.Skill].Value >= 60.0)
                WeaponAbility.SetCurrentAbility(this.m_Mobile, weapon.SecondaryAbility);
            else if (this.m_Mobile.Skills[SkillName.Wrestling].Value >= 60.0 && /*weapon == Fist &&*/ this.m_CanStun && !this.m_Mobile.StunReady)
                EventSink.InvokeStunRequest(new StunRequestEventArgs(this.m_Mobile));
        }

        public void CheckForFieldSpells()
        {
            if (!this.m_IsSmart)
                return;

            bool move = false;

            IPooledEnumerable eable = this.m_Mobile.Map.GetItemsInRange(this.m_Mobile.Location, 0);

            foreach (Item item in eable)
            {
                if (item == null)
                    continue;
                else if (item.Z != this.m_Mobile.Z)
                    continue;
                else
                    move = IsFieldSpell(item.ItemID);
            }
            eable.Free();

            if (move)
            {
                //TODO, make movement not so random.
                switch( Utility.Random(9) )
                {
                    case 0:
                        this.DoMove(Direction.Up);
                        break;
                    case 1:
                        this.DoMove(Direction.North);
                        break;
                    case 2:
                        this.DoMove(Direction.Left);
                        break;
                    case 3:
                        this.DoMove(Direction.West);
                        break;
                    case 5:
                        this.DoMove(Direction.Down);
                        break;
                    case 6:
                        this.DoMove(Direction.South);
                        break;
                    case 7:
                        this.DoMove(Direction.Right);
                        break;
                    case 8:
                        this.DoMove(Direction.East);
                        break;
                    default:
                        this.DoMove(this.m_Mobile.Direction);
                        break;
                }
            }
        }
    }
}