using NUnit.Framework;
using ResultDemo.Errors;
using ResultDemo.Operations;
using ResultDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultDemo.Tests.Services
{
    public class OtherServiceTests
    {
        public static OtherService CreateSubject()
        {
            return new OtherService(
                new OtherOperation(),
                new InterestingOperation(),
                new SampleService(
                    new AddOperation(),
                    new GetSampleDtoOperation()));
        }

        [Test]
        public void GetResult_FirstError_ShouldReturnOtherError()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.GetResult(false, default, default!);

            // assert
            Assert.IsInstanceOf<OtherError>(result.GetValue());
        }

        [Test]
        public void GetResult_SecondError_ShouldReturnInterestingError()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.GetResult(true, false, default!);

            // assert
            Assert.IsInstanceOf<InterestingError>(result.GetValue());
        }

        [Test]
        public void GetResult_ThirdError_ShouldReturnGenericErrorForInvalidSample()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.GetResult(true, true, "not a valid sample name");

            // assert
            Assert.IsInstanceOf<GenericError>(result.GetValue());
            Assert.AreEqual("Sample not found", ((GenericError)result.Error!).Message);
        }

        [Test]
        public void GetResult_ThirdError_ShouldReturnGenericErrorForNegativeAddition()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.GetResult(true, true, "negative");

            // assert
            Assert.IsInstanceOf<GenericError>(result.GetValue());
            Assert.AreEqual("Cannot add negative numbers", ((GenericError)result.Error!).Message);
        }

        [Test]
        public void GetResult_AllOk_ShouldReturnOk()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.GetResult(true, true, "sample");

            // assert
            Assert.IsInstanceOf<string>(result.GetValue());
            Assert.AreEqual("ok", result.Ok);
        }
    }
}
