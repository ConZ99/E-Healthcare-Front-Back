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

namespace BackendTest
{
    public class AccountControllerTests
    {
        [Test, AutoData]
        public async Task GetAllAccountsTestAsync()
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
            
            var sut = new AccountController(context, userService.Object);

            //ACT
            var result = await sut.GetAllAccounts();

            //ASSERT
            result.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task GetAccountByIdTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var account = fixture.Build<Account>()
                .With(x => x.Id, 1).Create();
            context.Accounts.Add(account);
            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();
            userService.Setup(x => x.GetMyId()).Returns("1");
            
            var sut = new AccountController(context, userService.Object);

            //ACT
            var result = await sut.GetAccountById();
            //ASSERT
            result.Result.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task EditOwnAccountTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var account = fixture.Build<Account>()
                .With(x => x.Id, 1).Create();

            var editDto = fixture.Build<EditDto>().
                With(x => x.FirstName, "test").Create();

            context.Accounts.Add(account);
            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();
            userService.Setup(x => x.GetMyId()).Returns("1");

            var sut = new AccountController(context, userService.Object);

            //ACT
            var response = await sut.EditOwnAccount(editDto);
            //ASSERT
            var result = (response.Result as dynamic).Value;
            Assert.AreEqual(result.FirstName, editDto.FirstName);
        }

        [Test, AutoData]
        public async Task DeleteByIdTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var account = fixture.Build<Account>()
                .With(x => x.Id, 1).Create();
            context.Accounts.Add(account);
            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();

            var sut = new AccountController(context, userService.Object);

            //ACT
            var result = await sut.DeleteAccountById(1);
            //ASSERT
            result.Result.Should().NotBeNull();
        }
    }
}