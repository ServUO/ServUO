using System;
using Server.Engines.CannedEvil;
using Server.Mobiles;
using Server.Targeting;

namespace Server
{
    public class ValorVirtue
    {
        private static readonly TimeSpan LossDelay = TimeSpan.FromDays(7.0);
        private const int LossAmount = 250;
        public static void Initialize()
        {
            VirtueGump.Register(112, new OnVirtueUsed(OnVirtueUsed));
        }

        public static void OnVirtueUsed(Mobile from)
        {
            if (from.Alive)
            {
                from.SendLocalizedMessage(1054034); // Target the Champion Idol of the Champion you wish to challenge!.
                from.Target = new InternalTarget();
            }
        }

        public static void CheckAtrophy(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            try
            {
                if ((pm.LastValorLoss + LossDelay) < DateTime.UtcNow)
                {
                    if (VirtueHelper.Atrophy(from, VirtueName.Valor, LossAmount))
                        from.SendLocalizedMessage(1054040); // You have lost some Valor.

                    pm.LastValorLoss = DateTime.UtcNow;
                }
            }
            catch
            {
            }
        }

        public static void Valor(Mobile from, object targ)
        {
            IdolOfTheChampion idol = targ as IdolOfTheChampion;

            if (idol == null || idol.Deleted || idol.Spawn == null || idol.Spawn.Deleted)
                from.SendLocalizedMessage(1054035); // You must target a Champion Idol to challenge the Champion's spawn!
            else if (from.Hidden)
                from.SendLocalizedMessage(1052015); // You cannot do that while hidden.
            else if (idol.Spawn.HasBeenAdvanced)
                from.SendLocalizedMessage(1054038); // The Champion of this region has already been challenged!
            else
            {
                VirtueLevel vl = VirtueHelper.GetLevel(from, VirtueName.Valor);
                if (idol.Spawn.Active)
                {
                    if (idol.Spawn.Champion != null)	//TODO: Message?
                        return;

                    int needed, consumed;
                    switch( idol.Spawn.GetSubLevel() )
                    {
                        case 0:
                            {
                                needed = consumed = 2500;
                                break;
                            }
                        case 1:
                            {
                                needed = consumed = 5000;
                                break;
                            }
                        case 2:
                            {
                                needed = 10000;
                                consumed = 7500;
                                break;
                            }
                        default:
                            {
                                needed = 20000;
                                consumed = 10000;
                                break;
                            }
                    }

                    if (from.Virtues.GetValue((int)VirtueName.Valor) >= needed)
                    {
                        VirtueHelper.Atrophy(from, VirtueName.Valor, consumed);
                        from.SendLocalizedMessage(1054037); // Your challenge is heard by the Champion of this region! Beware its wrath!
                        idol.Spawn.HasBeenAdvanced = true;
                        idol.Spawn.AdvanceLevel();
                    }
                    else
                        from.SendLocalizedMessage(1054039); // The Champion of this region ignores your challenge. You must further prove your valor.
                }
                else
                {
                    if (vl == VirtueLevel.Knight)
                    {
                        VirtueHelper.Atrophy(from, VirtueName.Valor, 11000);
                        from.SendLocalizedMessage(1054037); // Your challenge is heard by the Champion of this region! Beware its wrath!
                        idol.Spawn.EndRestart();
                        idol.Spawn.HasBeenAdvanced = true;
                    }
                    else
                    {
                        from.SendLocalizedMessage(1054036); // You must be a Knight of Valor to summon the champion's spawn in this manner!
                    }
                }
            }
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(14, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Valor(from, targeted);
            }
        }
    }
}