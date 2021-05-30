using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultDemo.Errors
{
    public class GenericError : IOtherServiceError, ISampleServiceError
    {
        public GenericError(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Message { get; }
    }
}
