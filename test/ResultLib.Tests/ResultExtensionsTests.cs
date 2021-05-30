using NUnit.Framework;
using ResultLib;

namespace ResultDemo.Tests
{
    public class ResultExtensionsTests
    {
        internal interface ITestError { }

        internal class TestError : ITestError { }

        internal class TestError2 : ITestError { }

        [Test]
        public void ContinueWith_Action_ShouldDoIfOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;

            // act
            subject.ContinueWith((ok) =>
            {
                capturedValue = ok;
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
        }

        [Test]
        public void ContinueWith_Action_ShouldNotDoIfError()
        {
            // assert
            var subject = Result<string, TestError>.FromError(new TestError());

            var capturedValue = (string?)default;

            // act
            subject.ContinueWith((ok) =>
            {
                capturedValue = ok;
            });

            // assert
            Assert.AreEqual(default, capturedValue);
        }

        [Test]
        public void ContinueWith_Func_OuterResultError_ShouldReturnOuterResult()
        {
            // assert
            var testError = new TestError();
            var subject = Result<string, TestError>.FromError(testError);

            var capturedValue = (string?)default;

            // act
            var result = subject.ContinueWith((ok) =>
            {
                capturedValue = ok;
                return Result<int, TestError>.FromOk(1);
            });

            // assert
            Assert.AreEqual(default, capturedValue);
            Assert.IsInstanceOf<TestError>(result.GetValue());
            Assert.AreSame(testError, result.Error);
        }

        [Test]
        public void ContinueWith_Func_OuterResultOk_ShouldReturnInnerResult()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;

            // act
            var result = subject.ContinueWith((ok) =>
            {
                capturedValue = ok;
                return Result<int, TestError>.FromOk(1);
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.IsInstanceOf<int>(result.GetValue());
            Assert.AreEqual(1, result.Ok);
        }

        [Test]
        public void ContinueWith_Func_MultipleChained_ShouldReturnFinalOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;
            var capturedValue2 = (string?)default;

            // act
            var result = subject.ContinueWith((ok) =>
            {
                capturedValue = ok;
                var resultFromService = Result<string, TestError>.FromOk("text2");

                return resultFromService.ContinueWith((ok) =>
                {
                    capturedValue2 = ok;
                    return Result<int, TestError>.FromOk(1);
                });
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.AreEqual("text2", capturedValue2);
            Assert.IsInstanceOf<int>(result.GetValue());
            Assert.AreEqual(1, result.Ok);
        }

        [Test]
        public void ContinueWith_Func_MultipleChained_ShouldReturnFirstError()
        {
            // assert
            var testError = new TestError();
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;
            var capturedValue2 = (string?)default;

            // act
            var result = subject.ContinueWith((ok) =>
            {
                capturedValue = ok;
                var resultFromService = Result<string, TestError>.FromError(testError);

                return resultFromService.ContinueWith((ok) =>
                {
                    capturedValue2 = ok;
                    return Result<int, TestError>.FromOk(1);
                });
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.AreEqual(default, capturedValue2);
            Assert.IsInstanceOf<TestError>(result.GetValue());
            Assert.AreSame(testError, result.Error);
        }
    }
}