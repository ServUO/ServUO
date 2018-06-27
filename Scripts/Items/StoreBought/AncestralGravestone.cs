using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x1173, 0x1174)]
    public class AncestralGravestone : Item
    {
        public override int LabelNumber { get { return 1071096; } } // Ancestral Gravestone

        [Constructable]
        public AncestralGravestone()
            : base(0x1173)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile m)
        {
            //TODO: Clilocs?
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsFriend(m) && (IsLockedDown || IsSecure))
            {
                if (IsInCooldown(m))
                {
                    TimeSpan tsRem = _Cooldown[m] - DateTime.UtcNow;

                    m.SendLocalizedMessage(1071505, ((int)tsRem.TotalMinutes).ToString()); // In order to get a buff again, you have to wait for at least ~1_VAL~ minutes.
                }
                else
                {
                    AddBonus(m);
                    m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                }
            }
        }

        private static Dictionary<Mobile, SkillMod> _Table;
        private static Dictionary<Mobile, DateTime> _Cooldown;
        private static Timer _Timer;

        public static bool UnderEffects(Mobile m)
        {
            return _Table != null && _Table.ContainsKey(m);
        }

        public static void AddBonus(Mobile m)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, SkillMod>();

            var mod = new DefaultSkillMod(SkillName.SpiritSpeak, true, 5.0);
            _Table[m] = mod;

            m.AddSkillMod(mod);
            AddToCooldown(m);
            
            Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(5, 40)), ExpireBonus, new object[] { m, mod });
        }

        public static void ExpireBonus(object o)
        {
            object[] objects = (object[])o;
            Mobile mob = objects[0] as Mobile;
            SkillMod sm = objects[1] as SkillMod;

            mob.RemoveSkillMod(sm);
        }

        public static bool IsInCooldown(Mobile m)
        {
            if (UnderEffects(m))
            {
                return true;
            }

            CheckCooldown();

            return _Cooldown != null && _Cooldown.ContainsKey(m);
        }

        public static void AddToCooldown(Mobile m)
        {
            if (_Cooldown == null)
                _Cooldown = new Dictionary<Mobile, DateTime>();

            _Cooldown[m] = DateTime.UtcNow + TimeSpan.FromMinutes(90);

            if (_Timer != null)
            {
                _Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), CheckCooldown);
                _Timer.Priority = TimerPriority.FiveSeconds;
            }
        }

        public static void CheckCooldown()
        {
            if (_Cooldown == null)
                return;

            List<Mobile> list = new List<Mobile>(_Cooldown.Keys);

            foreach (var m in list)
            {
                if (_Cooldown[m] < DateTime.UtcNow)
                {
                    _Cooldown.Remove(m);
                }
            }

            if (_Cooldown.Count == 0)
            {
                _Cooldown = null;

                if (_Timer != null)
                {
                    _Timer.Stop();
                    _Timer = null;
                }
            }

            ColUtility.Free(list);
        }

        public AncestralGravestone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}