using System;

namespace OpenMod.Unturned.Effects
{
    /// <summary>
    /// Effect key wrapper struct that is used in OpenMod.Unturned effect APIs
    /// </summary>
    public readonly struct UnturnedUIEffectKey : IEquatable<UnturnedUIEffectKey>, IComparable<UnturnedUIEffectKey>
    {
        /// <summary>
        /// Invalid effect key with value of -1. Sending -1 to client will not show effect
        /// </summary>
        public static readonly UnturnedUIEffectKey Invalid = new(-1);

        /// <summary>
        /// The value of the effect key
        /// </summary>
        public readonly short Value;

        /// <summary>
        /// Checks if this effect key is valid
        /// </summary>
        private bool IsValid => Value != Invalid.Value;

        public UnturnedUIEffectKey(short value) => Value = value;

        #region Equality, Comparable, ToString, conversions

        public bool Equals(UnturnedUIEffectKey other) => Value == other.Value;

        public override bool Equals(object? obj) => obj is UnturnedUIEffectKey other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(UnturnedUIEffectKey left, UnturnedUIEffectKey right) => left.Equals(right);

        public static bool operator !=(UnturnedUIEffectKey left, UnturnedUIEffectKey right) => !left.Equals(right);

        public int CompareTo(UnturnedUIEffectKey other) => Value.CompareTo(other.Value);

        public override string ToString() => Value.ToString();

        public static explicit operator short(UnturnedUIEffectKey key) => key.Value;

        public static explicit operator UnturnedUIEffectKey(short value) => new(value);

        #endregion
    }
}