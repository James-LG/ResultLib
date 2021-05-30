// Copyright (c) James La Novara-Gsell. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace ResultLib
{
    using System;

    /// <summary>
    /// Represents the result of an operation. Contains either <see cref="Ok"/> or <see cref="Error"/> but not both.
    /// </summary>
    /// <typeparam name="TOk">The type of the result if the operation was a success.</typeparam>
    /// <typeparam name="TError">The type of the error if the operation encountered an error.</typeparam>
    public class Result<TOk, TError>
    {
        private readonly bool isOk;

        private Result(TOk? ok, TError? error, bool isOk)
        {
            this.Ok = ok;
            this.Error = error;
            this.isOk = isOk;
        }

        /// <summary>
        /// Gets the successful result, if available.
        /// </summary>
        public TOk? Ok { get; }

        /// <summary>
        /// Gets the error result, if available.
        /// </summary>
        public TError? Error { get; }

        /// <summary>
        /// Create a result containing an <see cref="Ok"/> value.
        /// </summary>
        /// <param name="ok">The <see cref="Ok"/> value.</param>
        /// <returns>The result containing the <see cref="Ok"/> value.</returns>
        public static Result<TOk, TError> FromOk(TOk ok)
        {
            _ = ok ?? throw new ArgumentNullException(nameof(ok));
            return new Result<TOk, TError>(ok, default, true);
        }

        /// <summary>
        /// Create a result containing an <see cref="Error"/> value.
        /// </summary>
        /// <param name="error">The <see cref="Error"/> value.</param>
        /// <returns>The result containing the <see cref="Error"/> value.</returns>
        public static Result<TOk, TError> FromError(TError error)
        {
            _ = error ?? throw new ArgumentNullException(nameof(error));
            return new Result<TOk, TError>(default, error, false);
        }

        /// <summary>
        /// Gets either the contained <see cref="Ok"/> or <see cref="Error"/> value.
        /// </summary>
        /// <returns>Either the contained <see cref="Ok"/> or <see cref="Error"/>.</returns>
        public object GetValue()
        {
            if (this.isOk)
            {
                return this.Ok!;
            }
            else
            {
                return this.Error!;
            }
        }
    }
}
