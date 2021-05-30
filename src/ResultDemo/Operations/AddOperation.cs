using ResultLib;

namespace ResultDemo
{
    public class AddOperation
    {
        public Result<int, AddNegativeError> Add(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return Result<int, AddNegativeError>.FromError(new AddNegativeError());
            }

            return Result<int, AddNegativeError>.FromOk(x + y);
        }
    }
}
