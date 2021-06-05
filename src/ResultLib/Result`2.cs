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

        /// <summary>
        /// Converts the Error type of a result to a more generic type.
        /// </summary>
        /// <typeparam name="TError1">The original Error type.</typeparam>
        /// <typeparam name="TError2">The new (more generic) Error type.</typeparam>
        /// <returns>The modified result.</returns>
        public Result<TOk, TError2> ConvertErrorType<TError1, TError2>()
            where TError1 : TError2
        {
            return this.GetValue() switch
            {
                TOk ok => Result<TOk, TError2>.FromOk(ok),
                TError1 error => Result<TOk, TError2>.FromError(error),
                _ => throw new InvalidOperationException($"Must have either a non-null {nameof(this.Ok)}, or {nameof(this.Error)} property.")
            };
        }

        /// <summary>
        /// Performs and returns the <paramref name="continueFunc"/> if the result is Ok, else
        /// it immediately returns.
        /// </summary>
        /// <typeparam name="TOk2">The Ok type of the given <paramref name="continueFunc"/>.</typeparam>
        /// <param name="continueFunc">The function to perform if this result is Ok.</param>
        /// <returns>
        /// The result of the <paramref name="continueFunc"/> or a new result if it was an Error.
        /// </returns>
        public Result<TOk2, TError> ContinueWith<TOk2>(Func<TOk, Result<TOk2, TError>> continueFunc)
        {
            return this.GetValue() switch
            {
                TOk ok => continueFunc(ok),
                TError error => Result<TOk2, TError>.FromError(error),
                _ => throw new InvalidOperationException($"Must have either a non-null {nameof(this.Ok)}, or {nameof(this.Error)} property.")
            };
        }

        /// <summary>
        /// Performs the <paramref name="continueFunc"/> if the result is Ok, else it immediately returns.
        /// </summary>
        /// <param name="continueFunc">The function to perform if this result is Ok.</param>
        public void ContinueWith(Action<TOk> continueFunc)
        {
            switch (this.GetValue())
            {
                case TOk ok:
                    continueFunc(ok);
                    break;
                case TError:
                    // do nothing
                    break;
                default:
                    throw new InvalidOperationException($"Must have either a non-null {nameof(this.Ok)}, or {nameof(this.Error)} property.");
            }
        }

        /// <summary>
        /// Checks whether the result is <typeparamref name="TOk"/>.
        /// </summary>
        /// <returns>Whether the result is <typeparamref name="TOk"/>.</returns>
        public bool IsOk()
        {
            return this.Ok != null;
        }

        /// <summary>
        /// Checks whether the result is <typeparamref name="TError"/>.
        /// </summary>
        /// <returns>Whether the result is <typeparamref name="TError"/>.</returns>
        public bool IsError()
        {
            return this.Error != null;
        }

        /// <summary>
        /// Gets the result's <typeparamref name="TOk"/> value or throws.
        /// </summary>
        /// <returns>The result's <typeparamref name="TOk"/>.</returns>
        /// <exception cref="ArgumentException">When the result does not contain an <typeparamref name="TOk"/> value.</exception>
        /// <remarks>
        /// Only use this method when you already know the result is Ok, or it is acceptable to throw.
        /// Otherwise prefer switching on the type of <see cref="Result{TOk, TError}.GetValue"/>.
        /// </remarks>
        public TOk AsOk()
        {
            return this.Ok ?? throw new InvalidOperationException($"{nameof(this.Ok)} is null");
        }

        /// <summary>
        /// Gets the result's <typeparamref name="TError"/> value or throws.
        /// </summary>
        /// <returns>The result's <typeparamref name="TError"/>.</returns>
        /// <exception cref="ArgumentException">When the result does not contain an <typeparamref name="TError"/> value.</exception>
        /// <remarks>
        /// Only use this method when you already know the result is Ok, or it is acceptable to throw.
        /// Otherwise prefer switching on the type of <see cref="Result{TOk, TError}.GetValue"/>.
        /// </remarks>
        public TError AsError()
        {
            return this.Error ?? throw new InvalidOperationException($"{nameof(this.Error)} is null");
        }
    }
}
