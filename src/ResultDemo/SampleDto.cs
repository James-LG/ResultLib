using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultDemo
{
    public record SampleDto
    {
        public SampleDto(int something)
        {
            Something = something;
        }

        public int Something { get; }
    }
}
