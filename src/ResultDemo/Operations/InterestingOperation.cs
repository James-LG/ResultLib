using ResultDemo.Errors;
using ResultLib;

namespace ResultDemo.Operations
{
    public class InterestingOperation
    {
        public Result<string, InterestingError> Invoke(bool success)
        {
            return success
                ? Result<string, InterestingError>.FromOk("other")
                : Result<string, InterestingError>.FromError(new InterestingError());
        }
    }
}
