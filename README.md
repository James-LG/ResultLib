# ResultLib

Small library containing a `Result<TOk, TError>` class to be returned from operations that can fail in non-exceptional ways.

## Usage

You can chain together results such that any errors along the way will short-circuit the chain, allowing you to handle the errors only once per chain.

Example from `src/ResultDemo/Services/OtherService.cs`:

```csharp
public Result<string, IOtherServiceError> GetResult(bool success1, bool success2, string sampleName)
{
    return this.otherOperation.Invoke(success1)
        .ConvertErrorType<OtherError, IOtherServiceError>()
        .ContinueWith((ok) =>
        {
            return this.interestingOperation.Invoke(success2)
                .ConvertErrorType<InterestingError, IOtherServiceError>()
                .ContinueWith((ok) =>
                {
                    return this.sampleService.GetResult(sampleName).GetValue() switch
                    {
                        int i => Result<string, IOtherServiceError>.FromOk("ok"),
                        AddNegativeError => Result<string, IOtherServiceError>.FromError(new GenericError("Cannot add negative numbers")),
                        NotFoundError => Result<string, IOtherServiceError>.FromError(new GenericError("Sample not found")),
                        _ => throw new NotImplementedException(nameof(ISampleServiceError)),
                    };
                });
        });
}
```

Take a peek through `src/ResultDemo` and `test/ResultDemo.Tests` for more examples of how it can be used.
