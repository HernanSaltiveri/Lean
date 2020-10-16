﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;

namespace QuantConnect.Securities.Option.StrategyMatcher
{
    /// <summary>
    /// Defines a lightweight structure representing a position in an option contract.
    /// This type is heavily utilized by the options strategy matcher and is the parameter
    /// type of option strategy definition predicates.
    /// </summary>
    public struct OptionPosition : IEquatable<OptionPosition>
    {
        /// <summary>
        /// Gets a new <see cref="OptionPosition"/> with zero <see cref="Quantity"/>
        /// </summary>
        public static OptionPosition None(Symbol symbol)
            => new OptionPosition(symbol, 0);

        /// <summary>
        /// Number of contracts held, can be positive or negative
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Option contract symbol
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Option contract expiration date
        /// </summary>
        public DateTime Expiration => Symbol.ID.Date;

        /// <summary>
        /// Option contract strike price
        /// </summary>
        public decimal Strike => Symbol.ID.StrikePrice;

        /// <summary>
        /// Option contract right (put/call)
        /// </summary>
        public OptionRight Right => Symbol.ID.OptionRight;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionPosition"/> structure
        /// </summary>
        /// <param name="symbol">The option contract symbol</param>
        /// <param name="quantity">The number of contracts held</param>
        public OptionPosition(Symbol symbol, int quantity)
        {
            Symbol = symbol;
            Quantity = quantity;
        }

        /// <summary>
        /// Creates a new <see cref="OptionPosition"/> instance with negative <see cref="Quantity"/>
        /// </summary>
        public OptionPosition Negate()
        {
            return new OptionPosition(Symbol, -Quantity);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(OptionPosition other)
        {
            return Equals(Symbol, other.Symbol) && Quantity == other.Quantity;
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((OptionPosition) obj);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Symbol != null ? Symbol.GetHashCode() : 0) * 397) ^ Quantity;
            }
        }

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>The fully qualified type name.</returns>
        public override string ToString()
        {
            var s = Quantity == 1 ? "" : "s";
            if (Symbol.HasUnderlying)
            {
                return $"{Quantity} {Right.ToLower()}{s} on {Symbol.Underlying.Value} at ${Strike} expiring on {Expiration:yyyy-MM-dd}";
            }

            return $"{Quantity} share{s} of {Symbol.Value}";
        }

        public static OptionPosition operator *(OptionPosition left, int factor)
        {
            return new OptionPosition(left.Symbol, factor * left.Quantity);
        }

        public static OptionPosition operator *(int factor, OptionPosition right)
        {
            return new OptionPosition(right.Symbol, factor * right.Quantity);
        }

        public static OptionPosition operator +(OptionPosition left, OptionPosition right)
        {
            if (!Equals(left.Symbol, right.Symbol))
            {
                if (left == default(OptionPosition))
                {
                    return right;
                }

                if (right == default(OptionPosition))
                {
                    return left;
                }

                throw new InvalidOperationException("Unable to add OptionPosition instances with different symbols");
            }

            return new OptionPosition(left.Symbol, left.Quantity + right.Quantity);
        }

        public static OptionPosition operator -(OptionPosition left, OptionPosition right)
        {
            if (!Equals(left.Symbol, right.Symbol))
            {
                if (left == default(OptionPosition))
                {
                    // 0 - right
                    return right.Negate();
                }

                if (right == default(OptionPosition))
                {
                    // left - 0
                    return left;
                }

                throw new InvalidOperationException("Unable to subtract OptionPosition instances with different symbols");
            }

            return new OptionPosition(left.Symbol, left.Quantity - right.Quantity);
        }

        public static bool operator ==(OptionPosition left, OptionPosition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OptionPosition left, OptionPosition right)
        {
            return !Equals(left, right);
        }
    }
}