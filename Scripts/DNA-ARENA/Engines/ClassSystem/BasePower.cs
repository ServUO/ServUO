using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Server.Engines.ClassSystem
{
    // ── Enums ───────────────────────────────────────────────────────────────────

    /// <summary>Broad category of a power's effect.</summary>
    public enum PowerType
    {
        Harmful,
        Beneficial,
        Transformation,
        AreaField,
        Neutral
    }

    // ── BasePower ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Lightweight base for every class power in the custom class system.
    ///
    /// War mode
    /// ========
    /// ALL powers require war mode by default (RequiresWarMode = true).
    /// Override to false only for powers usable outside combat.
    ///
    /// Target timeout
    /// ==============
    /// After the cast phase, the target cursor stays open for TargetTimeout seconds.
    /// If the player does not click within that window:
    ///   - No mana is consumed.
    ///   - No cooldown is applied.
    ///   - Player sees "You waited too long to choose a target."
    /// Mana and cooldowns are consumed ONLY after a valid target is confirmed.
    /// </summary>
    public abstract class BasePower : ISpell
    {
        // ── Backing fields ──────────────────────────────────────────────────────

        private readonly Mobile         _caster;
        private          Timer          _activeTimer;
        private          InternalTarget _openTarget;

        // ── Definition properties ───────────────────────────────────────────────

        public virtual string    CastPhrase         { get; } = string.Empty;
        public virtual int       ManaCost           { get; } = 0;
        public virtual double    CastTime           { get; } = 0.0;

        /// <summary>Seconds the target cursor stays open. Default 8s. Override per power.</summary>
        public virtual int       TargetTimeout      { get; } = 8;

        public virtual int       GlobalCooldown     { get; } = 0;
        public virtual int       IndividualCooldown { get; } = 0;
        public virtual int       EffectDuration     { get; } = 0;
        public virtual bool      RequiresTarget     { get; } = false;
        public virtual int       MaxRange           { get; } = 0;
        public virtual bool      IsAreaEffect       { get; } = false;
        public virtual PowerType PowerType          { get; } = PowerType.Neutral;

        /// <summary>
        /// ALL powers require war mode by default.
        /// Set to false only for powers usable outside combat (e.g. utility buffs).
        /// </summary>
        public virtual bool      RequiresWarMode    { get; } = true;

        public virtual bool      RequiresPeaceMode  { get; } = false;
        public virtual bool      RequiresLOS        { get; } = true;
        public virtual bool      InterruptsSwing    { get; } = false;

        // ── Constructor ─────────────────────────────────────────────────────────

        protected BasePower(Mobile caster)
        {
            _caster = caster ?? throw new ArgumentNullException("caster");
        }

        // ── ISpell ──────────────────────────────────────────────────────────────

        public bool IsCasting => _caster.Spell == this;
        public int  ID        => -1;

        public void OnCasterHurt()        { }
        public void OnCasterKilled()      => Cancel();
        public void OnConnectionChanged() => Cancel();

        public bool OnCasterMoving(Direction d)
        {
            if (IsCasting && CastTime > 0)
            {
                _caster.SendMessage("You moved and interrupted your power.");
                Cancel();
            }
            return true;
        }

        public bool CheckMovement(Mobile caster)
        {
            if (IsCasting && CastTime > 0)
            {
                caster.SendMessage("You moved and interrupted your power.");
                Cancel();
                return false;
            }
            return true;
        }

        public bool OnCasterEquiping(Item item)    => true;
        public bool OnCasterUsingObject(object o)  => true;
        public bool OnCastInTown(Region r)         => true;

        // ── Entry point ─────────────────────────────────────────────────────────

        /// <summary>
        /// Called by ClassPowerSystem.Cast().
        /// Prerequisites are checked here; mana is consumed only after target confirmation.
        /// </summary>
        internal void Begin()
        {
            if (!_caster.Alive)
            {
                _caster.SendMessage("You cannot use your powers while dead.");
                return;
            }

            if (RequiresWarMode && !_caster.Warmode)
            {
                _caster.SendMessage("You must be in war mode to use this power.");
                return;
            }

            if (RequiresPeaceMode && _caster.Warmode)
            {
                _caster.SendMessage("You must be in peace mode.");
                return;
            }

            if (_caster.Frozen || _caster.Paralyzed)
            {
                _caster.SendMessage("You are unable to use your powers.");
                return;
            }

            if (GlobalCooldown > 0 && ClassPowerSystem.IsOnGlobalCooldown(_caster))
            {
                int secs = ClassPowerSystem.GetGlobalCooldownRemaining(_caster);
                _caster.SendMessage($"You must wait {ClassPowerSystem.FormatDuration(secs)} before using another power.");
                return;
            }

            if (IndividualCooldown > 0 && ClassPowerSystem.IsOnIndividualCooldown(_caster, GetType()))
            {
                int secs = ClassPowerSystem.GetIndividualCooldownRemaining(_caster, GetType());
                _caster.SendMessage($"That power is not ready yet. ({ClassPowerSystem.FormatDuration(secs)} remaining)");
                return;
            }

            if (_caster.Mana < ManaCost)
            {
                _caster.SendMessage("You do not have enough mana.");
                return;
            }

            if (!OnBeforeCast())
                return;

            if (_caster.Spell is ISpell existing)
            {
                existing.OnCasterKilled();
                return;
            }

            _caster.Spell = this;

            if (!string.IsNullOrEmpty(CastPhrase))
                _caster.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, $"*{CastPhrase}*");

            if (CastTime > 0)
                _activeTimer = Timer.DelayCall(TimeSpan.FromSeconds(CastTime), Finish);
            else
                Finish();
        }

        // ── Finish ──────────────────────────────────────────────────────────────

        private void Finish()
        {
            _caster.Spell = null;

            if (RequiresTarget)
            {
                // Open cursor – mana and cooldowns applied only after confirmation.
                _openTarget = new InternalTarget(this);
                _caster.Target = _openTarget;

                // Timeout: if no target confirmed in time, cancel the cursor.
                // We check _openTarget != null AND !_openTarget.Done to make sure
                // the player has not already confirmed a valid target in the meantime.
                _activeTimer = Timer.DelayCall(
                    TimeSpan.FromSeconds(TargetTimeout),
                    () =>
                    {
                        if (_openTarget != null && !_openTarget.Done)
                        {
                            // Mark expired BEFORE cancelling to prevent OnTargetCancel
                            // from printing a second message.
                            _openTarget.MarkDone();
                            _caster.Target?.Cancel(_caster, TargetCancelType.Timeout);
                            _caster.SendMessage("You waited too long to choose a target.");
                            _openTarget = null;
                        }
                    });
            }
            else if (IsAreaEffect)
            {
                ConsumeResources();
                ExecuteArea();
            }
            else
            {
                ConsumeResources();
                OnCast();
                if (EffectDuration > 0)
                    RegisterEffect(_caster);
            }
        }

        // ── Resource consumption ─────────────────────────────────────────────────

        private void ConsumeResources()
        {
            _caster.Mana -= ManaCost;

            if (GlobalCooldown > 0)
                ClassPowerSystem.SetGlobalCooldown(_caster, GlobalCooldown);

            if (IndividualCooldown > 0)
                ClassPowerSystem.SetIndividualCooldown(_caster, GetType(), IndividualCooldown);

            if (InterruptsSwing)
                _caster.NextCombatTime = Core.TickCount + 1000;
        }

        // ── Area ────────────────────────────────────────────────────────────────

        private void ExecuteArea()
        {
            var targets = new List<Mobile>();

            foreach (Mobile m in _caster.GetMobilesInRange(MaxRange))
                targets.Add(m);

            foreach (Mobile m in targets)
            {
                if (!m.Alive) continue;
                if (RequiresLOS && !_caster.InLOS(m)) continue;

                OnCastTarget(m);

                if (EffectDuration > 0)
                    RegisterEffect(m);
            }
        }

        // ── Effect timer ─────────────────────────────────────────────────────────

        private void RegisterEffect(Mobile target)
        {
            ClassPowerSystem.RegisterActiveEffect(target, this);
            _activeTimer = Timer.DelayCall(
                TimeSpan.FromSeconds(EffectDuration),
                () =>
                {
                    OnExpire(target);
                    ClassPowerSystem.UnregisterActiveEffect(target, this);
                });
        }

        // ── Cancel ───────────────────────────────────────────────────────────────

        private void Cancel()
        {
            _caster.Spell = null;
            _activeTimer?.Stop();
            _openTarget = null;
        }

        // ── Subclass hooks ───────────────────────────────────────────────────────

        /// <summary>Called before the cast. No mana consumed. Return false to abort.</summary>
        public virtual bool OnBeforeCast() => true;

        /// <summary>Called after target chosen, before effect fires. Return false to abort.</summary>
        public virtual bool OnBeforeCastTarget(Mobile target) => true;

        /// <summary>Main effect for self / untargeted powers.</summary>
        public virtual void OnCast() { }

        /// <summary>Main effect for targeted / area powers.</summary>
        public virtual void OnCastTarget(Mobile target) { }

        /// <summary>Called when the timed effect expires.</summary>
        public virtual void OnExpire(Mobile target) { }

        // ── Accessor ─────────────────────────────────────────────────────────────

        public Mobile Caster => _caster;

        // ── Inner target cursor ────────────────────────────────────────────────

        private sealed class InternalTarget : Target
        {
            private readonly BasePower _power;
            private          bool      _done; // true once confirmed OR timed out

            public bool Done => _done;
            public void MarkDone() => _done = true;

            public InternalTarget(BasePower power)
                : base(power.MaxRange, false,
                       power.PowerType == PowerType.Beneficial
                           ? TargetFlags.Beneficial
                           : TargetFlags.Harmful)
            {
                _power = power;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                // ── Validation ──────────────────────────────────────────────────────────
                if (!(targeted is Mobile target))
                {
                    from.SendMessage("You must target a living creature.");
                    RecastSamePower(from);
                    return;
                }

                if (!from.InRange(target, _power.MaxRange))
                {
                    from.SendMessage("That is too far away. Move closer.");
                    RecastSamePower(from);
                    return;
                }

                if (_power.RequiresLOS && !from.InLOS(target))
                {
                    from.SendMessage("That is not in your line of sight.");
                    RecastSamePower(from);
                    return;
                }

                if (!_power.OnBeforeCastTarget(target))
                {
                    RecastSamePower(from);
                    return;
                }

                // ── Valid target confirmed ────────────────────────────────────────────
                _done = true;
                _power._activeTimer?.Stop();
                _power._openTarget = null;

                _power.ConsumeResources();
                _power.OnCastTarget(target);

                if (_power.EffectDuration > 0)
                    _power.RegisterEffect(target);
            }

            /// <summary>
            /// On invalid target: stop the current timeout, create a fresh instance
            /// of the same power type and open a new cursor – bypassing mana and
            /// cooldown checks since they already passed on the first cast.
            /// </summary>
            private void RecastSamePower(Mobile from)
            {
                if (_done) return;

                _done = true;
                _power._activeTimer?.Stop();
                _power._openTarget = null;

                // Create a new instance of the exact same power type via reflection.
                BasePower newPower;
                try
                {
                    newPower = (BasePower)Activator.CreateInstance(
                        _power.GetType(), new object[] { from });
                }
                catch
                {
                    return; // reflection failed – give up silently
                }

                // Open a fresh cursor directly (no Begin() – skips mana/cooldown).
                var newCursor = new InternalTarget(newPower);
                newPower._openTarget = newCursor;
                from.Target = newCursor;

                // Restart the timeout for the new cursor.
                newPower._activeTimer = Timer.DelayCall(
                    TimeSpan.FromSeconds(newPower.TargetTimeout),
                    () =>
                    {
                        if (newPower._openTarget != null && !newPower._openTarget.Done)
                        {
                            newPower._openTarget.MarkDone();
                            from.Target?.Cancel(from, TargetCancelType.Timeout);
                            from.SendMessage("You waited too long to choose a target.");
                            newPower._openTarget = null;
                        }
                    });
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (_done) return;

                _done = true;
                _power._activeTimer?.Stop();
                _power._openTarget = null;

                if (cancelType != TargetCancelType.Timeout)
                    from.SendMessage("You decided not to use that power.");
            }
        }
    }
}
