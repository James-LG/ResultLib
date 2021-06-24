using NUnit.Framework;
using ResultLib;
using System;
using System.Threading.Tasks;

namespace ResultDemo.Tests
{
    public class ResultTests
    {
        internal interface ITestError { }

        internal class TestError : ITestError { }

        internal class TestError2 : ITestError { }

        [Test]
        public void ContinueWithAction_ShouldDoIfOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;

            // act
            subject.ContinueWithAction((ok) =>
            {
                capturedValue = ok;
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
        }

        [Test]
        public void ContinueWithAction_ShouldNotDoIfError()
        {
            // assert
            var subject = Result<string, TestError>.FromError(new TestError());

            var capturedValue = (string?)default;

            // act
            subject.ContinueWithAction((ok) =>
            {
                capturedValue = ok;
            });

            // assert
            Assert.AreEqual(default, capturedValue);
        }

        [Test]
        public async Task ContinueWithActionAsync_ShouldDoIfOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;

            // act
            await subject.ContinueWithActionAsync((ok) =>
            {
                capturedValue = ok;
                return Task.CompletedTask;
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
        }

        [Test]
        public async Task ContinueWithActionAsync_ShouldNotDoIfError()
        {
            // assert
            var subject = Result<string, TestError>.FromError(new TestError());

            var capturedValue = (string?)default;

            // act
            await subject.ContinueWithActionAsync((ok) =>
            {
                capturedValue = ok;
                return Task.CompletedTask;
            });

            // assert
            Assert.AreEqual(default, capturedValue);
        }

        [Test]
        public void ContinueWith_OuterResultError_ShouldReturnOuterResult()
        {
            // assert
            var testError = new TestError();
            var subject = Result<string, TestError>.FromError(testError);

            var capturedValue = (string?)default;

            // act
            var result = subject.ContinueWith<int>((ok) =>
            {
                capturedValue = ok;
                return 1;
            });

            // assert
            Assert.AreEqual(default, capturedValue);
            Assert.IsInstanceOf<TestError>(result.GetValue());
            Assert.AreSame(testError, result.Error);
        }

        [Test]
        public void ContinueWith_OuterResultOk_ShouldReturnInnerResult()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;

            // act
            var result = subject.ContinueWith<int>((ok) =>
            {
                capturedValue = ok;
                return 1;
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.IsInstanceOf<int>(result.GetValue());
            Assert.AreEqual(1, result.Ok);
        }

        [Test]
        public void ContinueWith_MultipleChained_ShouldReturnFinalOk()
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

                return resultFromService.ContinueWith<int>((ok) =>
                {
                    capturedValue2 = ok;
                    return 1;
                });
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.AreEqual("text2", capturedValue2);
            Assert.IsInstanceOf<int>(result.GetValue());
            Assert.AreEqual(1, result.Ok);
        }

        [Test]
        public void ContinueWith_MultipleChained_ShouldReturnFirstError()
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

                return resultFromService.ContinueWith<int>((ok) =>
                {
                    capturedValue2 = ok;
                    return 1;
                });
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.AreEqual(default, capturedValue2);
            Assert.IsInstanceOf<TestError>(result.GetValue());
            Assert.AreSame(testError, result.Error);
        }

        [Test]
        public async Task ContinueWithAsync_OuterResultError_ShouldReturnOuterResult()
        {
            // assert
            var testError = new TestError();
            var subject = Result<string, TestError>.FromError(testError);

            var capturedValue = (string?)default;

            // act
            var result = await subject.ContinueWithAsync((ok) =>
            {
                capturedValue = ok;
                return Task.FromResult(Result<int, TestError>.FromOk(1));
            });

            // assert
            Assert.AreEqual(default, capturedValue);
            Assert.IsInstanceOf<TestError>(result.GetValue());
            Assert.AreSame(testError, result.Error);
        }

        [Test]
        public async Task ContinueWithAsync_OuterResultOk_ShouldReturnInnerResult()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;

            // act
            var result = await subject.ContinueWithAsync((ok) =>
            {
                capturedValue = ok;
                return Task.FromResult(Result<int, TestError>.FromOk(1));
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.IsInstanceOf<int>(result.GetValue());
            Assert.AreEqual(1, result.Ok);
        }

        [Test]
        public async Task ContinueWithAsync_MultipleChained_ShouldReturnFinalOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;
            var capturedValue2 = (string?)default;

            // act
            var result = await subject.ContinueWithAsync((ok) =>
            {
                capturedValue = ok;
                var resultFromService = Result<string, TestError>.FromOk("text2");

                return resultFromService.ContinueWithAsync((ok) =>
                {
                    capturedValue2 = ok;
                    return Task.FromResult(Result<int, TestError>.FromOk(1));
                });
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.AreEqual("text2", capturedValue2);
            Assert.IsInstanceOf<int>(result.GetValue());
            Assert.AreEqual(1, result.Ok);
        }

        [Test]
        public async Task ContinueWithAsync_MultipleChained_ShouldReturnFirstError()
        {
            // assert
            var testError = new TestError();
            var subject = Result<string, TestError>.FromOk("hi");

            var capturedValue = (string?)default;
            var capturedValue2 = (string?)default;

            // act
            var result = await subject.ContinueWithAsync((ok) =>
            {
                capturedValue = ok;
                var resultFromService = Result<string, TestError>.FromError(testError);

                return resultFromService.ContinueWithAsync((ok) =>
                {
                    capturedValue2 = ok;
                    return Task.FromResult(Result<int, TestError>.FromOk(1));
                });
            });

            // assert
            Assert.AreEqual("hi", capturedValue);
            Assert.AreEqual(default, capturedValue2);
            Assert.IsInstanceOf<TestError>(result.GetValue());
            Assert.AreSame(testError, result.Error);
        }

        [Test]
        public void IsOk_ShouldReturnTrueIfOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            // act
            var val = subject.IsOk();

            // assert
            Assert.IsTrue(val);
        }

        [Test]
        public void IsOk_ShouldReturnFalseIfError()
        {
            // assert
            var subject = Result<string, TestError>.FromError(new TestError());

            // act
            var val = subject.IsOk();

            // assert
            Assert.IsFalse(val);
        }

        [Test]
        public void IsError_ShouldReturnTrueIfError()
        {
            // assert
            var subject = Result<string, TestError>.FromError(new TestError());

            // act
            var val = subject.IsError();

            // assert
            Assert.IsTrue(val);
        }

        [Test]
        public void IsError_ShouldReturnFalseIfOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            // act
            var val = subject.IsError();

            // assert
            Assert.IsFalse(val);
        }

        [Test]
        public void AsOk_ShouldReturnValueIfOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            // act
            var val = subject.AsOk();

            // assert
            Assert.AreEqual("hi", val);
        }

        [Test]
        public void AsOk_ShouldThrowIfNotOk()
        {
            // assert
            var subject = Result<string, TestError>.FromError(new TestError());

            // act
            // assert
            var ex = Assert.Throws<InvalidOperationException>(() => subject.AsOk());
            Assert.AreEqual($"Result not Ok: {typeof(TestError).Name}", ex.Message);
        }

        [Test]
        public void AsOk_ShouldThrowIfNotOkWithInstanceName()
        {
            // assert
            var subject = Result<string, ITestError>.FromError(new TestError());

            // act
            // assert
            var ex = Assert.Throws<InvalidOperationException>(() => subject.AsOk());
            Assert.AreEqual($"Result not Ok: {typeof(TestError).Name}", ex.Message); // Should be name of TestError not ITestError.
        }

        [Test]
        public void AsError_ShouldReturnValueIfError()
        {
            // assert
            var error = new TestError();
            var subject = Result<string, TestError>.FromError(error);

            // act
            var val = subject.AsError();

            // assert
            Assert.AreSame(error, val);
        }

        [Test]
        public void AsError_ShouldThrowIfNotOk()
        {
            // assert
            var subject = Result<string, TestError>.FromOk("hi");

            // act
            // assert
            var ex = Assert.Throws<InvalidOperationException>(() => subject.AsError());
            Assert.AreEqual($"Result not Error: {typeof(string).Name}", ex.Message);
        }

        [Test]
        public void AsError_ShouldThrowIfNotOkWithInstanceName()
        {
            // assert
            var subject = Result<object, TestError>.FromOk("hi");

            // act
            // assert
            var ex = Assert.Throws<InvalidOperationException>(() => subject.AsError());
            Assert.AreEqual($"Result not Error: {typeof(string).Name}", ex.Message); // Should be name of string, not object.
        }
    }
}