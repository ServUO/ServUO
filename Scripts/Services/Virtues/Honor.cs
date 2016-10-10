using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;

namespace Server
{
    public interface IHonorTarget
    {
        HonorContext ReceivedHonorContext { get; set; }
    }

    public class HonorVirtue
    {
        private static readonly TimeSpan UseDelay = TimeSpan.FromMinutes(5.0);
        public static void Initialize()
        {
            VirtueGump.Register(107, new OnVirtueUsed(OnVirtueUsed));
        }

        public static void ActivateEmbrace(PlayerMobile pm)
        {
            int duration = GetHonorDuration(pm);
            int usedPoints;

            if (pm.Virtues.Honor < 4399)
                usedPoints = 400;
            else if (pm.Virtues.Honor < 10599)
                usedPoints = 600;
            else
                usedPoints = 1000;

            VirtueHelper.Atrophy(pm, VirtueName.Honor, usedPoints);

            pm.HonorActive = true;
            pm.SendLocalizedMessage(1063235); // You embrace your honor

            BuffInfo.AddBuff(pm, new BuffInfo(BuffIcon.Honored, 1075649, BuffInfo.Blank, TimeSpan.FromSeconds(duration), pm, "You have embraced your honor"));

            Timer.DelayCall(TimeSpan.FromSeconds(duration),
                delegate()
                {
                    pm.HonorActive = false;
                    pm.LastHonorUse = DateTime.UtcNow;
                    pm.SendLocalizedMessage(1063236); // You no longer embrace your honor
                });
        }

        private static void OnVirtueUsed(Mobile from)
        {
            if (from.Alive)
            {
                from.SendLocalizedMessage(1063160); // Target what you wish to honor.
                from.Target = new InternalTarget();
            }
        }

        private static int GetHonorDuration(PlayerMobile from)
        {
            switch ( VirtueHelper.GetLevel(from, VirtueName.Honor) )
            {
                case VirtueLevel.Seeker:
                    return 30;
                case VirtueLevel.Follower:
                    return 90;
                case VirtueLevel.Knight:
                    return 300;

                default:
                    return 0 ;
            }
        }

        private static void EmbraceHonor(PlayerMobile pm)
        {
            if (pm.HonorActive)
            {
                pm.SendLocalizedMessage(1063230); // You must wait awhile before you can embrace honor again.
                return;
            }

            if (GetHonorDuration(pm) == 0)
            {
                pm.SendLocalizedMessage(1063234); // You do not have enough honor to do that
                return;
            }

            TimeSpan waitTime = DateTime.UtcNow - pm.LastHonorUse;
            if (waitTime < UseDelay)
            {
                TimeSpan remainingTime = UseDelay - waitTime;
                int remainingMinutes = (int)Math.Ceiling(remainingTime.TotalMinutes);

                pm.SendLocalizedMessage(1063240, remainingMinutes.ToString()); // You must wait ~1_HONOR_WAIT~ minutes before embracing honor again
                return;
            }
			
            pm.SendGump(new HonorSelf(pm));
        }

        private static void Honor(PlayerMobile source, Mobile target)
        {
            IHonorTarget honorTarget = target as IHonorTarget;
            GuardedRegion reg = (GuardedRegion)source.Region.GetRegion(typeof(GuardedRegion));
            Map map = source.Map;

            if (honorTarget == null)
                return;

            if (honorTarget.ReceivedHonorContext != null)
            {
                if (honorTarget.ReceivedHonorContext.Source == source)
                    return;

                if (honorTarget.ReceivedHonorContext.CheckDistance())
                {
                    source.SendLocalizedMessage(1063233); // Somebody else is honoring this opponent
                    return;
                }
            }

            if (target.Hits < target.HitsMax)
            {
                source.SendLocalizedMessage(1063166); // You cannot honor this monster because it is too damaged.
                return;
            }

            BaseCreature cret = target as BaseCreature;
            if (target.Body.IsHuman && (cret == null || (!cret.AlwaysAttackable && !cret.AlwaysMurderer)))
            {
                if (reg == null || reg.IsDisabled())
                { 
                    //Allow honor on blue if Out of guardzone
                }
                else if (map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0)
                {
                    //Allow honor on blue if in Fel
                }
                else
                {
                    source.SendLocalizedMessage(1001018); // You cannot perform negative acts
                    return;					//cannot honor in trammel town on blue
                }
            }

            if (Core.ML && target is PlayerMobile)
            {
                source.SendLocalizedMessage(1075614); // You cannot honor other players.
                return;
            }

            if (source.SentHonorContext != null)
                source.SentHonorContext.Cancel();

            new HonorContext(source, target);

            source.Direction = source.GetDirectionTo(target);

            if (!source.Mounted)
                source.Animate(32, 5, 1, true, true, 0);

            BuffInfo.AddBuff(source, new BuffInfo(BuffIcon.Honored, 1075649, BuffInfo.Blank, String.Format("You are honoring {0}", target.Name)));
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(12, false, TargetFlags.None)
            {
                this.CheckLOS = true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm == null)
                    return;

                if (targeted == pm)
                {
                    EmbraceHonor(pm);
                }
                else if (targeted is Mobile)
                    Honor(pm, (Mobile)targeted);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.SendLocalizedMessage(1063232); // You are too far away to honor your opponent
            }
        }
    }

    public class HonorContext
    {
        private readonly PlayerMobile m_Source;
        private readonly Mobile m_Target;
        private readonly Point3D m_InitialLocation;
        private readonly Map m_InitialMap;
        private readonly InternalTimer m_Timer;
        private double m_HonorDamage;
        private int m_TotalDamage;
        private int m_Perfection;
        private FirstHit m_FirstHit;
        private bool m_Poisoned;
        public HonorContext(PlayerMobile source, Mobile target)
        {
            this.m_Source = source;
            this.m_Target = target;

            this.m_FirstHit = FirstHit.NotDelivered;
            this.m_Poisoned = false;

            this.m_InitialLocation = source.Location;
            this.m_InitialMap = source.Map;

            source.SentHonorContext = this;
            ((IHonorTarget)target).ReceivedHonorContext = this;

            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();
            source.m_hontime = (DateTime.UtcNow + TimeSpan.FromMinutes(40));

            Timer.DelayCall(TimeSpan.FromMinutes(40),
                delegate()
                {
                    if (source.m_hontime < DateTime.UtcNow && source.SentHonorContext != null)
                    {
                        this.Cancel();
                    }
                });
        }

        private enum FirstHit
        {
            NotDelivered,
            Delivered,
            Granted
        }
        public PlayerMobile Source
        {
            get
            {
                return this.m_Source;
            }
        }
        public Mobile Target
        {
            get
            {
                return this.m_Target;
            }
        }
        public int PerfectionDamageBonus
        {
            get
            {
                return this.m_Perfection;
            }
        }
        public int PerfectionLuckBonus
        {
            get
            {
                return (this.m_Perfection * this.m_Perfection) / 10;
            }
        }
        public void OnSourceDamaged(Mobile from, int amount)
        {
            if (from != this.m_Target)
                return;

            if (this.m_FirstHit == FirstHit.NotDelivered)
                this.m_FirstHit = FirstHit.Granted;
        }

        public void OnTargetPoisoned()
        {
            this.m_Poisoned = true; // Set this flag for OnTargetDamaged which will be called next
        }

        public void OnTargetDamaged(Mobile from, int amount)
        {
            if (this.m_FirstHit == FirstHit.NotDelivered)
                this.m_FirstHit = FirstHit.Delivered;

            if (this.m_Poisoned)
            {
                this.m_HonorDamage += amount * 0.8;
                this.m_Poisoned = false; // Reset the flag

                return;
            }

            this.m_TotalDamage += amount;

            if (from == this.m_Source)
            {
                if (this.m_Target.CanSee(this.m_Source) && this.m_Target.InLOS(this.m_Source) && (this.m_Source.InRange(this.m_Target, 1) ||
                                                                                                  (this.m_Source.Location == this.m_InitialLocation && this.m_Source.Map == this.m_InitialMap)))
                {
                    this.m_HonorDamage += amount;
                }
                else
                {
                    this.m_HonorDamage += amount * 0.8;
                }
            }
            else if (from is BaseCreature && ((BaseCreature)from).GetMaster() == this.m_Source)
            {
                this.m_HonorDamage += amount * 0.8;
            }
        }

        public void OnTargetHit(Mobile from)
        {
            if (from != this.m_Source || this.m_Perfection == 100)
                return;

            int bushido = (int)from.Skills.Bushido.Value;
            if (bushido < 50)
                return;

            this.m_Perfection += bushido / 10;

            if (this.m_Perfection >= 100)
            {
                this.m_Perfection = 100;
                this.m_Source.SendLocalizedMessage(1063254); // You have Achieved Perfection in inflicting damage to this opponent!

                BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.Perfection, 1075651, BuffInfo.Blank, String.Format("+100% Damage to {0}", m_Target.Name)));
            }
            else
            {
                this.m_Source.SendLocalizedMessage(1063255); // You gain in Perfection as you precisely strike your opponent.
            }
        }

        public void OnTargetMissed(Mobile from)
        {
            BuffInfo.RemoveBuff(from, BuffIcon.Perfection);

            if (from != this.m_Source || this.m_Perfection == 0)
                return;

            this.m_Perfection -= 25;

            if (this.m_Perfection <= 0)
            {
                this.m_Perfection = 0;
                this.m_Source.SendLocalizedMessage(1063256); // You have lost all Perfection in fighting this opponent.
            }
            else
            {
                this.m_Source.SendLocalizedMessage(1063257); // You have lost some Perfection in fighting this opponent.
            }
        }

        public void OnSourceBeneficialAction(Mobile to)
        {
            if (to != this.m_Target)
                return;

            BuffInfo.RemoveBuff(m_Source, BuffIcon.Perfection);

            if (this.m_Perfection >= 0)
            {
                this.m_Perfection = 0;
                this.m_Source.SendLocalizedMessage(1063256); // You have lost all Perfection in fighting this opponent.
            }
        }

        public void OnSourceKilled()
        {
            return;
        }

        public void OnTargetKilled()
        {
            this.Cancel();

            int targetFame = this.m_Target.Fame;

            if (this.m_Perfection > 0)
            {
                int restore = Math.Min(this.m_Perfection * (targetFame + 5000) / 25000, 10);

                this.m_Source.Hits += restore;
                this.m_Source.Stam += restore;
                this.m_Source.Mana += restore;
            }

            if (this.m_Source.Virtues.Honor > targetFame)
                return;

            double dGain = (targetFame / 100) * (this.m_HonorDamage / this.m_TotalDamage);	//Initial honor gain is 100th of the monsters honor

            if (this.m_HonorDamage == this.m_TotalDamage && this.m_FirstHit == FirstHit.Granted)
                dGain = dGain * 1.5;							//honor gain is increased alot more if the combat was fully honorable
            else
                dGain = dGain * 0.9;

            int gain = Math.Min((int)dGain, 200);

            if (gain < 1)
                gain = 1;		//Minimum gain of 1 honor when the honor is under the monsters fame

            if (VirtueHelper.IsHighestPath(this.m_Source, VirtueName.Honor))
            {
                this.m_Source.SendLocalizedMessage(1063228); // You cannot gain more Honor.
                return;
            }

            bool gainedPath = false;
            if (VirtueHelper.Award(this.m_Source, VirtueName.Honor, (int)gain, ref gainedPath))
            {
                if (gainedPath)
                    this.m_Source.SendLocalizedMessage(1063226); // You have gained a path in Honor!
                else
                    this.m_Source.SendLocalizedMessage(1063225); // You have gained in Honor.
            }
        }

        public bool CheckDistance()
        {
            return true;
        }

        public void Cancel()
        {
            this.m_Source.SentHonorContext = null;
            ((IHonorTarget)this.m_Target).ReceivedHonorContext = null;

            this.m_Timer.Stop();

            BuffInfo.RemoveBuff(m_Source, BuffIcon.Perfection);
            BuffInfo.RemoveBuff(m_Source, BuffIcon.Honored);
        }

        private class InternalTimer : Timer
        {
            private readonly HonorContext m_Context;
            public InternalTimer(HonorContext context)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Context = context;
            }

            protected override void OnTick()
            {
                this.m_Context.CheckDistance();
            }
        }
    }
}