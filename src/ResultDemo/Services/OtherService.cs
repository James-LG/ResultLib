using ResultDemo.Errors;
using ResultDemo.Operations;
using ResultLib;
using System;

namespace ResultDemo.Services
{
    public class OtherService
    {
        private readonly OtherOperation otherOperation;
        private readonly InterestingOperation interestingOperation;
        private readonly SampleService sampleService;

        public OtherService(OtherOperation otherOperation, InterestingOperation interestingOperation, SampleService sampleService)
        {
            this.otherOperation = otherOperation ?? throw new ArgumentNullException(nameof(otherOperation));
            this.interestingOperation = interestingOperation ?? throw new ArgumentNullException(nameof(interestingOperation));
            this.sampleService = sampleService ?? throw new ArgumentNullException(nameof(sampleService));
        }

        public Result<string, IOtherServiceError> GetResult(bool success1, bool success2, string sampleName)
        {
            return this.otherOperation.Invoke(success1)
                .WithErrorType<string, OtherError, IOtherServiceError>()
                .ContinueWith((ok) =>
                {
                    return this.interestingOperation.Invoke(success2)
                        .WithErrorType<string, InterestingError, IOtherServiceError>()
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
    }
}
