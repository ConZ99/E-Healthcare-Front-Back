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
using System.Collections.Generic;

namespace BackendTest
{
    public class ProductControllerTests
    {
        [Test, AutoData]
        public async Task GetAllProductsTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var product = fixture.Build<Product>()
                .With(x => x.Name, "Product")
                .With(x => x.Id, 10).Create();
            context.Products.Add(product);

            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();
            
            var sut = new ProductController(context, userService.Object);

            //ACT
            var response = await sut.GetAllProducts();

            //ASSERT
            var result = (response.Result as dynamic).Value;
            Assert.AreEqual(result[0].Name, "Product");
        }

        [Test, AutoData]
        public async Task GetProductByIdTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var product = fixture.Build<Product>()
                .With(x => x.Name, "Product")
                .With(x => x.Id, 10).Create();
            context.Products.Add(product);

            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();

            var sut = new ProductController(context, userService.Object);

            //ACT
            var response = await sut.GetProductById(10);

            //ASSERT
            var result = (response.Result as dynamic).Value;
            Assert.AreEqual(result.Name, "Product");
        }

        [Test, AutoData]
        public async Task GetProductByUseTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var product = fixture.Build<Product>()
                .With(x => x.Name, "Product")
                .With(x => x.Id, 10)
                .With(x => x.Uses, "test").Create();
            context.Products.Add(product);

            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();

            var sut = new ProductController(context, userService.Object);

            //ACT
            var response = await sut.GetProductsByUse("test");

            //ASSERT
            var result = (response.Result as dynamic).Value;
            Assert.AreEqual(result[0].Name, "Product");
        }

        [Test, AutoData]
        public async Task PutProductByUseTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var product = fixture.Build<Product>()
                .With(x => x.Name, "Product")
                .With(x => x.Id, 10)
                .With(x => x.Uses, "test").Create();
            context.Products.Add(product);

            var edit = fixture.Build<Product>()
                .With(x => x.Name, "ProductEdited")
                .With(x => x.Id, 10).Create();

            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();

            var sut = new ProductController(context, userService.Object);

            //ACT
            var response = await sut.PutProduct(10, edit);

            //ASSERT
            var result = (response.Result as dynamic).Value;
            Assert.AreEqual(result.Name, "ProductEdited");
        }

        [Test, AutoData]
        public async Task AddProductByUseTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var product = fixture.Build<Product>()
                .With(x => x.Name, "Product")
                .With(x => x.Id, 10)
                .With(x => x.Uses, "test").Create();
            context.Products.Add(product);

            var edit = fixture.Build<Product>()
                .With(x => x.Name, "ProductAdded")
                .With(x => x.Id, 11).Create();

            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();

            var sut = new ProductController(context, userService.Object);

            //ACT
            var response = await sut.AddProduct(edit);

            //ASSERT
            response.Should().NotBeNull();
        }

        [Test, AutoData]
        public async Task DeleteProductByIdTestAsync()
        {
            //ARRANGE
            var fixture = new Fixture();
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"), b => b.EnableNullChecks(false))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options;
            var context = new DataContext(contextOptions);

            var product = fixture.Build<Product>()
                .With(x => x.Name, "Product")
                .With(x => x.Id, 10)
                .With(x => x.Uses, "test").Create();
            context.Products.Add(product);

            var account = fixture.Build<Account>()
                .With(x => x.Id, 10).Create();
            context.Accounts.Add(account);

            context.SaveChanges();

            var userService = fixture.Create<Mock<IUserService>>();
            userService.Setup(x => x.GetMyId()).Returns("10");

            var sut = new ProductController(context, userService.Object);

            //ACT
            var response = await sut.DeleteProductById(10);

            //ASSERT
            response.Should().NotBeNull();
        }
    }
}