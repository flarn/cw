using CW.Api;
using CW.Api.Models;

namespace cw.api.test
{
    public class InputValidationTests
    {
        [Test]
        public void ShouldSucceed()
        {
            var request = new UpdateOrderRequest
            {
                Id = Guid.NewGuid(),
                Text = "test",
                Count = 1,
                TotalAmount = 2
            };

            ValidationHelper.Validate(request, out var validationErrors);
            Assert.That(validationErrors, Is.Empty);
        }

        [Test]
        public void ShouldFailOnText()
        {
            var request = new UpdateOrderRequest
            {
                Id = Guid.NewGuid(),
                Text = string.Empty,
                Count = 1,
                TotalAmount = 2
            };

            ValidationHelper.Validate(request, out var validationErrors);
            Assert.That(validationErrors, Is.Not.Empty);
        }
        
        [Test]
        public void ShouldFailOnCount()
        {
            var request = new UpdateOrderRequest
            {
                Id = Guid.NewGuid(),
                Text = "test",
                Count = -1,
                TotalAmount = 2
            };

            ValidationHelper.Validate(request, out var validationErrors);
            Assert.That(validationErrors, Is.Not.Empty);
        }
        
        [Test]
        public void ShouldFailOnTotalAmount()
        {
            var request = new UpdateOrderRequest
            {
                Id = Guid.NewGuid(),
                Text = "test",
                Count = 1,
                TotalAmount = 200_000
            };

            ValidationHelper.Validate(request, out var validationErrors);
            Assert.That(validationErrors, Is.Not.Empty);
        }
    }
}