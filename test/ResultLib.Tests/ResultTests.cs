using NUnit.Framework;
using ResultLib;
using System;

namespace ResultDemo.Tests
{
    public class ResultTests
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
            Assert.Throws<InvalidOperationException>(() => subject.AsOk());
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
            Assert.Throws<InvalidOperationException>(() => subject.AsError());
        }
    }
}