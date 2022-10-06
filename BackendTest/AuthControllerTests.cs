using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ProiectFinal;
using ProiectFinal.Controllers;
using ProiectFinal.Controllers.Services.UserService;
using ProiectFinal.Data;
using ProiectFinal.DTOs;
using ProiectFinal.Models;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using ProiectFinal.Controllers.Services.TokenManager;

namespace BackendTest
{
    public class AuthControllerTests
    {
        [Test, AutoData]
        public async Task LoginTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);
            var account = fixture.Build<Account>().With(x => x.Email, "test@test.com").Without(x => x.Id).Create();
            context.Accounts.Add(account);
            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();
            var tokenManagerMock = fixture.Create<Mock<ITokenManager>>();
            tokenManagerMock.Setup(x => x.CreateToken(It.IsAny<Account>())).Returns("Token");
            tokenManagerMock.Setup(x => x.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);

            var sut = new AuthController(context, userService.Object, tokenManagerMock.Object);

            LoginDto loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "test"
            };
            //ACT
            var result = await sut.Login(loginDto);
            //ASSERT
            result.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task GetMeTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var account = fixture.Build<Account>()
                .With(x => x.Email, "test@test.com")
                .With(x => x.Id, 0).Create();
            context.Accounts.Add(account);
            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();
            userService.Setup(x => x.GetMyId()).Returns("1");
            userService.Setup(x => x.GetMyName()).Returns("test@test.com");
            userService.Setup(x => x.GetMyRole()).Returns("0");
            var tokenManagerMock = fixture.Create<Mock<ITokenManager>>();

            var sut = new AuthController(context, userService.Object, tokenManagerMock.Object);

            //ACT
            var result = await sut.GetMe();
            //ASSERT
            result.Value.Should().BeEquivalentTo(account);
        }

        [Test, AutoData]
        public async Task RegisterTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var userService = fixture.Create<Mock<IUserService>>();

            var tokenManagerMock = fixture.Create<Mock<ITokenManager>>();
            tokenManagerMock.Setup(x => x.CreatePasswordHash(It.IsAny<string>()))
                .Returns((fixture.Create<byte[]>(), fixture.Create<byte[]>()));
            tokenManagerMock.Setup(x => x.VerifyEmail(It.IsAny<string>())).ReturnsAsync(true);

            var registerDto = fixture.Build<RegisterDto>().Create();

            var sut = new AuthController(context, userService.Object, tokenManagerMock.Object);

            //ACT
            var result = await sut.Register(registerDto);
            //ASSERT
            result.Should().NotBeNull();
        }
    }
}