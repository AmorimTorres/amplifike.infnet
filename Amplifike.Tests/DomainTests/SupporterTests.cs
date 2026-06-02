using System;
using Amplifike.Domain;
using Xunit;

namespace Amplifike.Tests.DomainTests
{
    public class SupporterTests
    {
        [Fact]
        public void Should_CreateSupporterSuccessfully_When_ValidParametersProvided()
        {
            // Arrange & Act
            var supporter = new Supporter(
                name: "Rodrigo",
                email: "rodrigo@email.com",
                phone: "11988887777",
                documentOptional: "123.456.789-00"
            );

            // Assert
            Assert.Equal("Rodrigo", supporter.Name);
            Assert.Equal("rodrigo@email.com", supporter.Email);
            Assert.Equal("11988887777", supporter.Phone);
            Assert.Equal("123.456.789-00", supporter.DocumentOptional);
            Assert.NotEqual(Guid.Empty, supporter.Id);
            Assert.True((DateTime.UtcNow - supporter.CreatedAt).TotalSeconds < 5);
        }

        [Theory] // Testes Negativos
        [InlineData("", "email@test.com", "1234", "nome")]
        [InlineData("Rodrigo", "", "1234", "e-mail")]
        [InlineData("Rodrigo", "email@test.com", "", "telefone")]
        public void Should_ThrowException_When_RequiredFieldsAreMissing(string name, string email, string phone, string expectedFieldInError)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Supporter(name, email, phone));
            Assert.Contains(expectedFieldInError, ex.Message);
        }
    }
}
