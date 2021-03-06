﻿using System;
using Logs.Authentication.Contracts;
using Logs.Models;
using Logs.Providers.Contracts;
using Logs.Services.Contracts;
using Logs.Web.Controllers;
using Logs.Web.Infrastructure.Factories;
using Logs.Web.Models.Nutrition;
using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace Logs.Web.Tests.Controllers.NutritionControllerTests
{
    [TestFixture]
    public class LoadTests
    {
        [Test]
        public void TestLoad_ModelStateIsNotValid_ShouldRenderDefaultPartialView()
        {
            // Arrange
            var mockedFactory = new Mock<IViewModelFactory>();
            var mockedDateTimeProvider = new Mock<IDateTimeProvider>();
            var mockedNutritionService = new Mock<INutritionService>();
            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var controller = new NutritionController(mockedFactory.Object, mockedDateTimeProvider.Object,
            mockedNutritionService.Object, mockedAuthenticationProvider.Object);
            controller.ModelState.AddModelError("", "");


            // Act, Assert
            controller
                .WithCallTo(c => c.Load(new DateTime()))
                .ShouldRenderDefaultPartialView();
        }

        [Test]
        public void TestLoad_ModelStateIsValid_ShouldCallAuthenticationProviderCurrentUserId()
        {
            // Arrange
            var mockedFactory = new Mock<IViewModelFactory>();
            var mockedDateTimeProvider = new Mock<IDateTimeProvider>();
            var mockedNutritionService = new Mock<INutritionService>();
            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var controller = new NutritionController(mockedFactory.Object, mockedDateTimeProvider.Object,
            mockedNutritionService.Object, mockedAuthenticationProvider.Object);

            // Act
            controller.Load(new DateTime());

            // Assert
            mockedAuthenticationProvider.Verify(p => p.CurrentUserId, Times.Once);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestLoad_ModelStateIsValid_ShouldCallNutritionServiceGetByDate(string userId)
        {
            // Arrange
            var mockedFactory = new Mock<IViewModelFactory>();
            var mockedDateTimeProvider = new Mock<IDateTimeProvider>();
            var mockedNutritionService = new Mock<INutritionService>();

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();
            mockedAuthenticationProvider.Setup(p => p.CurrentUserId).Returns(userId);

            var date = new DateTime(2, 3, 4);

            var controller = new NutritionController(mockedFactory.Object, mockedDateTimeProvider.Object,
            mockedNutritionService.Object, mockedAuthenticationProvider.Object);

            // Act
            controller.Load(date);

            // Assert
            mockedNutritionService.Verify(s => s.GetByDate(userId, date), Times.Once);
        }

        [Test]
        public void TestLoad_NutritionIsNull_ShouldCallFactoryCreateNutritonViewModelWithNull()
        {
            // Arrange
            var mockedFactory = new Mock<IViewModelFactory>();
            var mockedDateTimeProvider = new Mock<IDateTimeProvider>();
            var mockedNutritionService = new Mock<INutritionService>();

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var date = new DateTime(2, 3, 4);

            var controller = new NutritionController(mockedFactory.Object, mockedDateTimeProvider.Object,
            mockedNutritionService.Object, mockedAuthenticationProvider.Object);

            // Act
            controller.Load(date);

            // Assert
            mockedFactory.Verify(f => f.CreateNutritionViewModel(null, date));
        }

        [Test]
        public void TestLoad_NutritionIsNotNull_ShouldCallFactoryCreatNutritionViewModelCorrectly()
        {
            // Arrange
            var mockedFactory = new Mock<IViewModelFactory>();
            var mockedDateTimeProvider = new Mock<IDateTimeProvider>();

            var nutrition = new Nutrition();

            var mockedNutritionService = new Mock<INutritionService>();
            mockedNutritionService.Setup(s => s.GetByDate(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(nutrition);

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var date = new DateTime(2, 3, 4);

            var controller = new NutritionController(mockedFactory.Object, mockedDateTimeProvider.Object,
            mockedNutritionService.Object, mockedAuthenticationProvider.Object);

            // Act
            controller.Load(date);

            // Assert
            mockedFactory.Verify(f => f.CreateNutritionViewModel(nutrition, date));
        }

        [Test]
        public void TestLoad_ShouldRenderCorrectViewWithModel()
        {
            // Arrange
            var viewModel = new NutritionViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateNutritionViewModel(It.IsAny<Nutrition>(), It.IsAny<DateTime>()))
                .Returns(viewModel);

            var mockedDateTimeProvider = new Mock<IDateTimeProvider>();
            var mockedNutritionService = new Mock<INutritionService>();
            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var date = new DateTime(2, 3, 4);

            var controller = new NutritionController(mockedFactory.Object, mockedDateTimeProvider.Object,
            mockedNutritionService.Object, mockedAuthenticationProvider.Object);

            // Act, Assert
            controller
                .WithCallTo(c => c.Load(date))
                .ShouldRenderDefaultPartialView()
                .WithModel<NutritionViewModel>(m => Assert.AreSame(viewModel, m));
        }
    }
}