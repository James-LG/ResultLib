using ResultLib;

namespace ResultDemo
{
    public class GetSampleDtoOperation
    {
        public Result<SampleDto, NotFoundError> Get(string name)
        {
            if (name == "sample")
            {
                return Result<SampleDto, NotFoundError>.FromOk(new SampleDto(1));
            }
            else if (name == "negative")
            {
                return Result<SampleDto, NotFoundError>.FromOk(new SampleDto(-1));
            }

            return Result<SampleDto, NotFoundError>.FromError(new NotFoundError());
        }
    }
}
