using ResultDemo.Errors;
using ResultLib;

namespace ResultDemo.Operations
{
    public class OtherOperation
    {
        public Result<string, OtherError> Invoke(bool success)
        {
            return success
                ? Result<string, OtherError>.FromOk("other")
                : Result<string, OtherError>.FromError(new OtherError());
        }
    }
}
