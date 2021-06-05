// Copyright (c) James La Novara-Gsell. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace ResultLib
{
    using System;

    /// <summary>
    /// Extension methods use to conveniently work with <see cref="Result{TOk, TError}"/> instances.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Converts the Error type of a result to a more generic type.
        /// </summary>
        /// <typeparam name="TOk">The original Ok type.</typeparam>
        /// <typeparam name="TError1">The original Error type.</typeparam>
        /// <typeparam name="TError2">The new (more generic) Error type.</typeparam>
        /// <param name="result">The result to operate on.</param>
        /// <returns>The modified result.</returns>
        public static Result<TOk, TError2> WithErrorType<TOk, TError1, TError2>(this Result<TOk, TError1> result)
            where TError1 : TError2
        {
            return result.GetValue() switch
            {
                TOk ok => Result<TOk, TError2>.FromOk(ok),
                TError1 error => Result<TOk, TError2>.FromError(error),
                _ => throw new ArgumentException($"{nameof(result)} must have either a non-null {nameof(result.Ok)}, or {nameof(result.Error)} property.")
            };
        }

        /// <summary>
        /// Performs and returns the <paramref name="continueFunc"/> if the result is Ok, else
        /// it immediately returns.
        /// </summary>
        /// <typeparam name="TOk1">The original Ok type.</typeparam>
        /// <typeparam name="TError">The original Error type.</typeparam>
        /// <typeparam name="TOk2">The Ok type of the given <paramref name="continueFunc"/>.</typeparam>
        /// <param name="result">The result to operate on.</param>
        /// <param name="continueFunc">The function to perform if <paramref name="result"/> is Ok.</param>
        /// <returns>
        /// The result of the <paramref name="continueFunc"/> or the transformed <paramref name="result"/> if it was an Error.
        /// </returns>
        public static Result<TOk2, TError> ContinueWith<TOk1, TError, TOk2>(this Result<TOk1, TError> result, Func<TOk1, Result<TOk2, TError>> continueFunc)
        {
            return result.GetValue() switch
            {
                TOk1 ok => continueFunc(ok),
                TError error => Result<TOk2, TError>.FromError(error),
                _ => throw new ArgumentException($"{nameof(result)} must have either a non-null {nameof(result.Ok)}, or {nameof(result.Error)} property.")
            };
        }

        /// <summary>
        /// Performs the <paramref name="continueFunc"/> if the result is Ok, else it immediately returns.
        /// </summary>
        /// <typeparam name="TOk">The original Ok type.</typeparam>
        /// <typeparam name="TError">The original Error type.</typeparam>
        /// <param name="result">The result to operate on.</param>
        /// <param name="continueFunc">The function to perform if <paramref name="result"/> is Ok.</param>
        public static void ContinueWith<TOk, TError>(this Result<TOk, TError> result, Action<TOk> continueFunc)
        {
            switch (result.GetValue())
            {
                case TOk ok:
                    continueFunc(ok);
                    break;
                case TError:
                    // do nothing
                    break;
                default:
                    throw new ArgumentException($"{nameof(result)} must have either a non-null {nameof(result.Ok)}, or {nameof(result.Error)} property.");
            }
        }

        /// <summary>
        /// Checks whether the result is <typeparamref name="TOk"/>.
        /// </summary>
        /// <typeparam name="TOk">The result's Ok type.</typeparam>
        /// <typeparam name="TError">The result's Error type.</typeparam>
        /// <param name="result">The result to operate on.</param>
        /// <returns>Whether the result is <typeparamref name="TOk"/>.</returns>
        public static bool IsOk<TOk, TError>(this Result<TOk, TError> result)
        {
            return result.Ok != null;
        }

        /// <summary>
        /// Checks whether the result is <typeparamref name="TError"/>.
        /// </summary>
        /// <typeparam name="TOk">The result's Ok type.</typeparam>
        /// <typeparam name="TError">The result's Error type.</typeparam>
        /// <param name="result">The result to operate on.</param>
        /// <returns>Whether the result is <typeparamref name="TError"/>.</returns>
        public static bool IsError<TOk, TError>(this Result<TOk, TError> result)
        {
            return result.Error != null;
        }

        /// <summary>
        /// Gets the result's <typeparamref name="TOk"/> value or throws.
        /// </summary>
        /// <typeparam name="TOk">The result's Ok type.</typeparam>
        /// <typeparam name="TError">The result's Error type.</typeparam>
        /// <param name="result">The result to operate on.</param>
        /// <returns>The result's <typeparamref name="TOk"/>.</returns>
        /// <exception cref="ArgumentException">When the result does not contain an <typeparamref name="TOk"/> value.</exception>
        /// <remarks>
        /// Only use this method when you already know the result is Ok, or it is acceptable to throw.
        /// Otherwise prefer switching on the type of <see cref="Result{TOk, TError}.GetValue"/>.
        /// </remarks>
        public static TOk AsOk<TOk, TError>(this Result<TOk, TError> result)
        {
            return result.Ok ?? throw new ArgumentException($"{nameof(result.Ok)} is null");
        }

        /// <summary>
        /// Gets the result's <typeparamref name="TError"/> value or throws.
        /// </summary>
        /// <typeparam name="TOk">The result's Ok type.</typeparam>
        /// <typeparam name="TError">The result's Error type.</typeparam>
        /// <param name="result">The result to operate on.</param>
        /// <returns>The result's <typeparamref name="TError"/>.</returns>
        /// <exception cref="ArgumentException">When the result does not contain an <typeparamref name="TError"/> value.</exception>
        /// <remarks>
        /// Only use this method when you already know the result is Ok, or it is acceptable to throw.
        /// Otherwise prefer switching on the type of <see cref="Result{TOk, TError}.GetValue"/>.
        /// </remarks>
        public static TError AsError<TOk, TError>(this Result<TOk, TError> result)
        {
            return result.Error ?? throw new ArgumentException($"{nameof(result.Error)} is null");
        }
    }
}
