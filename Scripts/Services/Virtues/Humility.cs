#region References
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
#endregion

namespace Server.Services.Virtues
{
    public class HumilityVirtue
    {
        public static Dictionary<Mobile, HumilityHuntContext> HuntTable { get; set; }
        public static Dictionary<Mobile, Mobile> ActiveTable { get; set; }

        public static void Initialize()
        {
            HuntTable = new Dictionary<Mobile, HumilityHuntContext>();
            ActiveTable = new Dictionary<Mobile, Mobile>();

            VirtueGump.Register(108, OnVirtueUsed);

            EventSink.Speech += EventSink_Speech;
        }

        public static void EventSink_Speech(SpeechEventArgs e)
        {
            string speech = e.Speech;

            if (speech.IndexOf("lum lum lum", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                HumilityHunt(e.Mobile);
            }
        }

        public static void OnVirtueUsed(Mobile from)
        {
            if (VirtueHelper.GetLevel(from, VirtueName.Humility) < VirtueLevel.Seeker)
                from.SendLocalizedMessage(1155812); // You must be at least a Seeker of Humility to Invoke this ability.
            else if (from.Alive)
            {
                from.SendLocalizedMessage(1155817); // Target the pet you wish to embrace with your Humility.
                from.BeginTarget(
                    10,
                    false,
                    TargetFlags.None,
                    (m, targeted) =>
                    {
                        if (targeted is BaseCreature)
                        {
                            BaseCreature bc = (BaseCreature)targeted;

                            if (!bc.Alive)
                            {
                                from.SendLocalizedMessage(1155815); // You cannot embrace Humility on the dead!
                            }
                            else if (VirtueHelper.GetLevel(m, VirtueName.Humility) < VirtueLevel.Seeker)
                            {
                                from.SendLocalizedMessage(1155812); // You must be at least a Seeker of Humility to Invoke this ability.
                            }
                            else if (!bc.Controlled && !bc.Summoned)
                            {
                                from.SendLocalizedMessage(1155813); // You can only embrace your Humility on a pet.
                            }
                            else if (ActiveTable.ContainsKey(bc))
                            {
                                from.SendLocalizedMessage(1156047); // That pet has already embraced Humility.
                            }
                            else
                            {
                                VirtueHelper.Atrophy(from, VirtueName.Humility, 3200);

                                from.SendLocalizedMessage(1155818); // You have lost some Humility.

                                ActiveTable[bc] = from;

                                m.PrivateOverheadMessage(
                                    MessageType.Regular,
                                    1150,
                                    1155819,
                                    from.NetState); // *Your pet surges with the power of your Humility!*

                                bc.FixedEffect(0x373A, 10, 16);

                                BuffInfo.AddBuff(
                                    from,
                                    new BuffInfo(
                                        BuffIcon.Humility,
                                        1156049,
                                        1156050,
                                        TimeSpan.FromMinutes(20),
                                        from,
                                        string.Format("{0}\t{1}", bc.Name, GetRegenBonus(bc)))); // Pet: ~1_NAME~<br>+~2_VAL~ HPR<br>

                                CheckTimer();
                                bc.ResetStatTimers();

                                Timer.DelayCall(
                                    TimeSpan.FromMinutes(20),
                                    mob =>
                                    {
                                        if (mob != null && ActiveTable.ContainsKey(mob))
                                        {
                                            Mobile user = ActiveTable[mob];
                                            ActiveTable.Remove(mob);

                                            BuffInfo.RemoveBuff(user, BuffIcon.Humility);

                                            user.PrivateOverheadMessage(
                                                MessageType.Regular,
                                                1150,
                                                1155823,
                                                from.NetState); // *Your pet's power returns to normal*

                                            CheckTimer();
                                        }
                                    },
                                    bc);
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1155813); // You can only embrace your Humility on a pet.
                        }
                    });
            }
        }

        private static Timer _Timer;

        public static void CheckTimer()
        {
            if (ActiveTable == null || ActiveTable.Count == 0)
            {
                if (_Timer != null)
                {
                    _Timer.Stop();
                    _Timer = null;
                }
            }
            else
            {
                if (_Timer == null)
                {
                    _Timer = Timer.DelayCall(
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(3),
                        () =>
                        {
                            foreach (Mobile mob in ActiveTable.Keys)
                            {
                                mob.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                            }
                        });

                    _Timer.Start();
                }
            }
        }

        public static int GetRegenBonus(Mobile mobile)
        {
            if (ActiveTable == null || !ActiveTable.ContainsKey(mobile))
                return 0;

            Mobile user = ActiveTable[mobile];

            if (user != null)
            {
                if (VirtueHelper.IsKnight(user, VirtueName.Humility))
                    return 30;

                if (VirtueHelper.IsFollower(user, VirtueName.Humility))
                    return 20;

                if (VirtueHelper.IsSeeker(user, VirtueName.Humility))
                    return 10;
            }

            return 0;
        }

        public static void HumilityHunt(Mobile from)
        {
            if (!from.Alive)
                return;

            if (HuntTable.ContainsKey(from))
            {
                if (!HuntTable[from].Expiring)
                {
                    HuntTable[from].Expiring = true;

                    from.SendLocalizedMessage(1155800); // You have ended your journey on the Path of Humility.
                }
                else
                    from.SendLocalizedMessage(
                        1155796); // You have already ended your journey on the Path of Humility.  You must wait before you restart your path.
            }
            else
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm != null)
                {
                    HuntTable[pm] = new HumilityHuntContext(pm, new List<Mobile>(pm.AllFollowers));

                    pm.SendLocalizedMessage(
                        1155802,
                        "70"); // You have begun your journey on the Path of Humility.  Your resists have been debuffed by ~1_DEBUFF~.
                    pm.SendLocalizedMessage(
                        1155858); // You are now on a Humility Hunt. For each kill while you forgo the protection of resists," you shall continue on your path to Humility.  You may end your Hunt by speaking ""Lum Lum Lum"" at any time.

                    BuffInfo.AddBuff(pm, new BuffInfo(BuffIcon.HumilityDebuff, 1025327, 1155806, "70"));
                }
            }
        }

        public static void RegisterKill(Mobile attacker, BaseCreature killed, int count)
        {
            int points = Math.Min(60, Math.Max(1, (killed.Fame / 5000) * 10)) / count;

            if (attacker != null && HuntTable.ContainsKey(attacker))
            {
                bool gainedPath = false;

                if (VirtueHelper.Award(attacker, VirtueName.Humility, points, ref gainedPath))
                {
                    if (gainedPath)
                        attacker.SendLocalizedMessage(1155811); // You have gained a path in Humility!
                    else
                        attacker.SendLocalizedMessage(1155809); // You have gained in Humility!
                }
                else
                    attacker.SendLocalizedMessage(1155808); // You cannot gain more Humility.
            }
        }

        public static bool IsInHunt(Mobile m)
        {
            return HuntTable != null && HuntTable.ContainsKey(m);
        }

        public static bool IsInHunt(PlayerMobile pm)
        {
            return HuntTable.ContainsKey(pm);
        }

        public static void TryAddPetToHunt(Mobile owner, Mobile pet)
        {
            if (HuntTable.ContainsKey(owner))
            {
                if (pet is BaseCreature && ((BaseCreature)pet).GetMaster() == owner)
                {
                    HuntTable[owner].AddPet(pet);
                }
            }
        }

        public static void OnHuntExpired(Mobile m)
        {
            Timer.DelayCall(
                TimeSpan.FromSeconds(60),
                () =>
                {
                    if (HuntTable.ContainsKey(m))
                        HuntTable.Remove(m);
                });
        }

        public class HumilityHuntContext
        {
            public Mobile Owner { get; set; }

            public Dictionary<Mobile, ResistanceMod[]> Table;

            private bool _Expiring;

            public ResistanceMod[] GetMod => new[]
                    {
                        new ResistanceMod(ResistanceType.Physical, -70), new ResistanceMod(ResistanceType.Fire, -70),
                        new ResistanceMod(ResistanceType.Poison, -70), new ResistanceMod(ResistanceType.Cold, -70),
                        new ResistanceMod(ResistanceType.Energy, -70)
                    };

            public bool Expiring
            {
                get { return _Expiring; }
                set
                {
                    if (!_Expiring && value)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(30), DoExpire);
                    }

                    _Expiring = value;
                }
            }

            public HumilityHuntContext(Mobile owner, List<Mobile> pets)
            {
                Owner = owner;

                Table = new Dictionary<Mobile, ResistanceMod[]>();

                ResistanceMod[] mod = GetMod;

                owner.FixedEffect(0x373A, 10, 16);

                foreach (ResistanceMod mods in mod)
                    owner.AddResistanceMod(mods);

                Table[owner] = mod;
                pets.ForEach(
                    m =>
                    {
                        mod = GetMod;

                        foreach (ResistanceMod mods in mod)
                            m.AddResistanceMod(mods);

                        m.FixedEffect(0x373A, 10, 16);
                        Table[m] = mod;
                    });
            }

            public void AddPet(Mobile pet)
            {
                if (!_Expiring && !Table.ContainsKey(pet))
                {
                    ResistanceMod[] mod = GetMod;

                    pet.FixedEffect(0x373A, 10, 16);

                    foreach (ResistanceMod mods in mod)
                        pet.AddResistanceMod(mods);

                    Table[pet] = mod;
                }
            }

            private void DoExpire()
            {
                foreach (KeyValuePair<Mobile, ResistanceMod[]> kvp in Table)
                {
                    foreach (ResistanceMod mod in kvp.Value)
                        kvp.Key.RemoveResistanceMod(mod);

                    BuffInfo.RemoveBuff(kvp.Key, BuffIcon.HumilityDebuff);
                }

                Table.Clear();
                OnHuntExpired(Owner);
            }
        }
    }
}