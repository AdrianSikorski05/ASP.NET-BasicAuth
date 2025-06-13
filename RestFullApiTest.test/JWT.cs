using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace RestFullApiTest.test
{
    public class JWT
    {

        private IConfiguration BuildTestConfiguration()
        {
            return new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Jwt:Key", "siemaTuAdrian256_kluczTajnyZGnieznaiOkolic"},
                {"Jwt:Issuer", "RestFullApiTest"},
                {"Jwt:Audience", "RestFullApiTestClient"},
                {"Jwt:ExpireMinutes", "30"}
            }).Build();
        }
        [Fact]
        public void GenerateToken_Should_Return_Valid_JWT_When_Input_Is_Correct()
        {
            // Arrange
            var config = BuildTestConfiguration();
            var jwtService = new JwtService(config);

            // Act
            var token = jwtService.GenerateToken("admin", "admin");

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("admin", null, "role")]
        [InlineData(null, "admin", "username")]
        [InlineData(null, null, "username")]       
        public void GenerateToken_Should_Throw_When_UsernameOrRole_Is_Null(string username, string role,string expectedParam)
        {
            // Arrange
            var config = BuildTestConfiguration();
            var jwtService = new JwtService(config);

            // Act
            Action act = () => jwtService.GenerateToken(username, role);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName(expectedParam);
        }
    }
}