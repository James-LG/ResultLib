using NUnit.Framework;

namespace ResultDemo.Tests.Services
{
    public class SampleServiceTests
    {
        public static SampleService CreateSubject()
        {
            return new SampleService(
                new AddOperation(),
                new GetSampleDtoOperation());
        }

        [Test]
        public void GetResult_ResultOk_ShouldGetAddedValue()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.GetResult("sample");

            // assert
            Assert.IsInstanceOf<int>(result.GetValue());
            Assert.AreEqual(3, result.Ok);
        }

        [Test]
        public void GetResult_ResultNotFoundError_ShouldReturnError()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.GetResult("not a valid name");

            // assert
            Assert.IsInstanceOf<NotFoundError>(result.GetValue());
        }

        [Test]
        public void GetResult_ResultAddNegativeError_ShouldReturnError()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.GetResult("negative");

            // assert
            Assert.IsInstanceOf<AddNegativeError>(result.GetValue());
        }
    }
}
