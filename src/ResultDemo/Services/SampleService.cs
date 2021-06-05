using ResultLib;
using System;

namespace ResultDemo
{
    public class SampleService
    {
        private readonly AddOperation addOperation;
        private readonly GetSampleDtoOperation getSampleDtoOperation;

        public SampleService(AddOperation addOperation, GetSampleDtoOperation getSampleDtoOperation)
        {
            this.addOperation = addOperation ?? throw new ArgumentNullException(nameof(addOperation));
            this.getSampleDtoOperation = getSampleDtoOperation ?? throw new ArgumentNullException(nameof(getSampleDtoOperation));
        }

        public Result<int, ISampleServiceError> GetResult(string sampleName)
        {
            return this.getSampleDtoOperation.Get(sampleName)
                .ConvertErrorType<NotFoundError, ISampleServiceError>()
                .ContinueWith((ok) =>
                {
                    return this.addOperation.Add(ok.Something, 1)
                        .ContinueWith((ok) =>
                        {
                            return this.addOperation.Add(ok, 1);
                        })
                        .ConvertErrorType<AddNegativeError, ISampleServiceError>();
                });
        }
    }
}
