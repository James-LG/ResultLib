using ResultLib;
using System;

namespace ResultDemo.Services
{
    public class HigherService
    {
        private readonly SampleService sampleService;

        public HigherService(SampleService sampleService)
        {
            this.sampleService = sampleService ?? throw new ArgumentNullException(nameof(sampleService));
        }

        public string HandleSpecificErrors(string name)
        {
            var result = this.sampleService.GetResult(name);

            return result.GetValue() switch
            {
                int i => "Very important number is " + i,
                AddNegativeError _ => "Cannot add negative numbers",
                NotFoundError _ => "Could not find sample",
                _ => throw new NotImplementedException(nameof(Result<int, ISampleServiceError>)),
            };
        }

        public string HandleGenericError(string name)
        {
            var result = this.sampleService.GetResult(name);

            return result.GetValue() switch
            {
                int i => "Very important number is " + i,
                ISampleServiceError _ => "Something went boom?",
                _ => throw new NotImplementedException(nameof(Result<int, ISampleServiceError>)),
            };
        }
    }
}
