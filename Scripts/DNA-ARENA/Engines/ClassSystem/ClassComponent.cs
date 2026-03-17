using System;
using Server.Mobiles;

namespace Server.Engines.ClassSystem
{
    /// <summary>
    /// Serializable component attached to a <see cref="PlayerMobile"/> that stores
    /// the player's current class.
    ///
    /// Integration with PlayerMobile
    /// ==============================
    /// Add the following to PlayerMobile:
    ///
    ///   private ClassComponent m_ClassComponent;
    ///
    ///   public ClassComponent ClassComponent
    ///   {
    ///       get { return m_ClassComponent; }
    ///   }
    ///
    ///   // In Serialize():
    ///   m_ClassComponent.Serialize(writer);
    ///
    ///   // In Deserialize():
    ///   m_ClassComponent = new ClassComponent(this);
    ///   m_ClassComponent.Deserialize(reader);
    ///
    ///   // In the constructor:
    ///   m_ClassComponent = new ClassComponent(this);
    ///
    /// Public surface used by the rest of the shard
    /// =============================================
    ///   player.ClassComponent.ClassType   → current ClassType (or None)
    ///   player.ClassComponent.PlayerClass → IPlayerClass instance (or null)
    ///   player.ClassComponent.Assign(ClassType)
    ///   player.ClassComponent.HasClass(ClassType)
    /// </summary>
    public sealed class ClassComponent
    {
        // ── Serialization version ───────────────────────────────────────────────
        private const int SerializationVersion = 1;

        // ── State ───────────────────────────────────────────────────────────────
        private readonly PlayerMobile _owner;
        private ClassType _classType = ClassType.None;

        // ── Constructor ─────────────────────────────────────────────────────────

        public ClassComponent(PlayerMobile owner)
        {
            _owner = owner ?? throw new ArgumentNullException("owner");
        }

        // ── Properties ──────────────────────────────────────────────────────────

        /// <summary>The player's current class enum value.</summary>
        public ClassType ClassType => _classType;

        /// <summary>
        /// The live <see cref="IPlayerClass"/> instance, or <c>null</c> if the
        /// player has no class or the class is not registered.
        /// </summary>
        public IPlayerClass PlayerClass => ClassRegistry.Get(_classType);

        /// <summary>Returns true if the player belongs to the given class.</summary>
        public bool HasClass(ClassType type) => _classType == type;

        // ── Mutation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Assigns a new class to the player.
        /// Fires <c>OnClassRemoved</c> on the old class and <c>OnClassAssigned</c>
        /// on the new one, then re-applies stats.
        /// Pass <see cref="ClassType.None"/> to strip the class entirely.
        /// </summary>
        public void Assign(ClassType newType)
        {
            if (newType == _classType)
                return;

            // Notify old class.
            IPlayerClass oldClass = ClassRegistry.Get(_classType);
            if (oldClass != null)
                oldClass.OnClassRemoved(_owner);

            _classType = newType;

            // Notify new class and apply stats.
            IPlayerClass newClass = ClassRegistry.Get(newType);
            if (newClass != null)
            {
                newClass.OnClassAssigned(_owner);
                ClassRegistry.ApplyClassStats(_owner, newClass);
            }
        }

        // ── Serialization ───────────────────────────────────────────────────────

        public void Serialize(GenericWriter writer)
        {
            writer.Write(SerializationVersion);
            writer.Write((int)_classType);
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    _classType = (ClassType)reader.ReadInt();
                    break;

                default:
                    Console.WriteLine($"[ClassComponent] Unknown serialization version {version} on {_owner}.");
                    _classType = ClassType.None;
                    break;
            }
        }
    }
}
