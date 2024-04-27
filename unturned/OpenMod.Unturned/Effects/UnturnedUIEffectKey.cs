using System;
using JetBrains.Annotations;

namespace OpenMod.Unturned.Effects
{
    /// <summary>
    /// Effect key wrapper struct that is used in OpenMod.Unturned effect APIs
    /// </summary>
    public readonly struct UnturnedUIEffectKey : IEquatable<UnturnedUIEffectKey>, IComparable<UnturnedUIEffectKey>
    {
        /// <summary>
        /// Invalid effect key with value of -1. Sending -1 to client will launch effect as one shot and not register the key.
        /// </summary>
        public static readonly UnturnedUIEffectKey Invalid = new(-1);

        /// <summary>
        /// The value of the effect key
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly short Value;

        /// <summary>
        /// Checks if this effect key is valid
        /// </summary>
        [UsedImplicitly]
        public bool IsValid
        {
            get => Value != Invalid.Value;
        }

        public UnturnedUIEffectKey(short value)
        {
            Value = value;
        }

        #region Equality, Comparable, ToString, conversions

        public bool Equals(UnturnedUIEffectKey other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is UnturnedUIEffectKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(UnturnedUIEffectKey left, UnturnedUIEffectKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UnturnedUIEffectKey left, UnturnedUIEffectKey right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(UnturnedUIEffectKey other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static explicit operator short(UnturnedUIEffectKey key)
        {
            return key.Value;
        }

        public static explicit operator UnturnedUIEffectKey(short value)
        {
            return new UnturnedUIEffectKey(value);
        }

        #endregion
    }
}