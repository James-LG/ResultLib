using NUnit.Framework;
using ResultDemo.Services;

namespace ResultDemo.Tests.Services
{
    public class HigherServiceTests
    {
        public static HigherService CreateSubject()
        {
            return new HigherService(
                new SampleService(
                    new AddOperation(),
                    new GetSampleDtoOperation()));
        }

        [Test]
        public void HandleSpecificErrors_ResultOk_ShouldReturnAppropriateString()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.HandleSpecificErrors("sample");

            // assert
            Assert.AreEqual("Very important number is 3", result);
        }

        [Test]
        public void HandleSpecificErrors_ResultNotFoundError_ShouldReturnAppropriateString()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.HandleSpecificErrors("not a valid name");

            // assert
            Assert.AreEqual("Could not find sample thingy :(", result);
        }

        [Test]
        public void HandleSpecificErrors_ResultAddNegativeError_ShouldReturnAppropriateString()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.HandleSpecificErrors("negative");

            // assert
            Assert.AreEqual("Cannot add negative numbers!!", result);
        }

        [Test]
        public void HandleGenericError_ResultOk_ShouldReturnAppropriateString()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.HandleGenericError("sample");

            // assert
            Assert.AreEqual("Very important number is 3", result);
        }

        [Test]
        public void HandleGenericError_ResultNotFoundError_ShouldReturnAppropriateString()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.HandleGenericError("not a valid name");

            // assert
            Assert.AreEqual("Something went boom?", result);
        }

        [Test]
        public void HandleGenericError_ResultAddNegativeError_ShouldReturnAppropriateString()
        {
            // arrange
            var subject = CreateSubject();

            // act
            var result = subject.HandleGenericError("negative");

            // assert
            Assert.AreEqual("Something went boom?", result);
        }
    }
}
