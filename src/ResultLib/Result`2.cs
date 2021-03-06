// Copyright (c) James La Novara-Gsell. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace ResultLib
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the result of an operation. Contains either <see cref="Ok"/> or <see cref="Error"/> but not both.
    /// </summary>
    /// <typeparam name="TOk">The type of the result if the operation was a success.</typeparam>
    /// <typeparam name="TError">The type of the error if the operation encountered an error.</typeparam>
    public class Result<TOk, TError>
        where TOk : notnull
        where TError : notnull
    {
        private Result(TOk? ok, TError? error, bool isOk)
        {
            this.Ok = ok;
            this.Error = error;
            this.IsOk = isOk;
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
        /// Gets a value indicating whether this Result is <typeparamref name="TOk"/>.
        /// </summary>
        public bool IsOk { get; }

        /// <summary>
        /// Gets a value indicating whether this Result is <typeparamref name="TError"/>.
        /// </summary>
        public bool IsError => !this.IsOk;

        /// <summary>
        /// Converts an <typeparamref name="TOk"/> value into a <see cref="Result{TOk, TError}"/>.
        /// </summary>
        /// <param name="ok">The ok value.</param>
        public static implicit operator Result<TOk, TError>(TOk ok) => FromOk(ok);

        /// <summary>
        /// Converts an <typeparamref name="TError"/> value into a <see cref="Result{TOk, TError}"/>.
        /// </summary>
        /// <param name="error">The error value.</param>
        public static implicit operator Result<TOk, TError>(TError error) => FromError(error);

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
            if (this.IsOk)
            {
                return this.AsOk();
            }
            else
            {
                return this.AsError();
            }
        }

        /// <summary>
        /// Converts the Error type of a result to a more generic type.
        /// </summary>
        /// <typeparam name="TError1">The original Error type.</typeparam>
        /// <typeparam name="TError2">The new (more generic) Error type.</typeparam>
        /// <returns>The modified result.</returns>
        public Result<TOk, TError2> ConvertErrorType<TError1, TError2>()
            where TError2 : notnull
            where TError1 : TError2, TError
        {
            return this.IsOk
                ? this.AsOk()
                : Result<TOk, TError2>.FromError((TError1)this.AsError());
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
            where TOk2 : notnull
        {
            return this.IsOk
                ? continueFunc(this.AsOk())
                : this.AsError();
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
        public Task<Result<TOk2, TError>> ContinueWithAsync<TOk2>(Func<TOk, Task<Result<TOk2, TError>>> continueFunc)
            where TOk2 : notnull
        {
            return this.IsOk
                ? continueFunc(this.AsOk())
                : Task.FromResult(Result<TOk2, TError>.FromError(this.AsError()));
        }

        /// <summary>
        /// Performs the <paramref name="continueFunc"/> if the result is Ok, else it immediately returns.
        /// </summary>
        /// <param name="continueFunc">The function to perform if this result is Ok.</param>
        public void ContinueWithAction(Action<TOk> continueFunc)
        {
            if (this.IsOk)
            {
                continueFunc(this.AsOk());
            }
        }

        /// <summary>
        /// Performs the <paramref name="continueFunc"/> if the result is Ok, else it immediately returns.
        /// </summary>
        /// <param name="continueFunc">The function to perform if this result is Ok.</param>
        /// <returns>The task performing the Ok operation or a completed task.</returns>
        public Task ContinueWithActionAsync(Func<TOk, Task> continueFunc)
        {
            return this.IsOk
                ? continueFunc(this.AsOk())
                : Task.CompletedTask;
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
            return this.Ok ?? throw new InvalidOperationException($"Result not Ok: {this.Error!.GetType().Name}");
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
            return this.Error ?? throw new InvalidOperationException($"Result not Error: {this.Ok!.GetType().Name}");
        }
    }
}
