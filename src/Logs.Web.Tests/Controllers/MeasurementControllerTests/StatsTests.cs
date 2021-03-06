﻿using Logs.Authentication.Contracts;
using Logs.Models;
using Logs.Providers.Contracts;
using Logs.Services.Contracts;
using Logs.Web.Controllers;
using Logs.Web.Infrastructure.Factories;
using Logs.Web.Models.Nutrition;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TestStack.FluentMVCTesting;

namespace Logs.Web.Tests.Controllers.MeasurementControllerTests
{
    [TestFixture]
    public class StatsTests
    {
        [Test]
        public void TestStats_NoIdProvided_ShouldCallAuthenticationIsAuthenticated()
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();
            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                  mockedMeasurementService.Object,
                  mockedFactory.Object);

            // Act
            controller.Stats(null);

            // Assert
            mockedAuthenticationProvider.Verify(p => p.IsAuthenticated, Times.Once);
        }

        [Test]
        public void TestStats_NoIdProvidedAndIsAuthenticated_ShouldCallAuthenticationProviderCurrentUserId()
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();
            mockedAuthenticationProvider.Setup(p => p.IsAuthenticated).Returns(true);

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                  mockedMeasurementService.Object,
                  mockedFactory.Object);

            // Act
            controller.Stats(null);

            // Assert
            mockedAuthenticationProvider.Verify(p => p.CurrentUserId, Times.Once);
        }

        [Test]
        public void TestStats_NoIdProvidedAndIsAuthenticated_ShouldSetModelCanDeleteToTrue()
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();
            mockedAuthenticationProvider.Setup(p => p.IsAuthenticated).Returns(true);

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                  mockedMeasurementService.Object,
                  mockedFactory.Object);

            // Act
            controller.Stats(null);

            // Assert
            Assert.IsTrue(model.CanDelete);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestStats_WithId_ShouldNotCallAuthenticationProviderCurrentUserId(string userId)
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();
            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                    mockedMeasurementService.Object,
                    mockedFactory.Object);

            // Act
            controller.Stats(userId);

            // Assert
            mockedAuthenticationProvider.Verify(p => p.CurrentUserId, Times.Never);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestStats_IsNotAuthenticated_ShouldNotCallAuthenticationProviderCurrentUserId(string userId)
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();
            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                 mockedMeasurementService.Object,
                 mockedFactory.Object);

            // Act
            controller.Stats(userId);

            // Assert
            mockedAuthenticationProvider.Verify(p => p.CurrentUserId, Times.Never);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestStats_IsNotAuthenticated_ShouldSetModelCanDeleteToFalse(string userId)
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();
            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                  mockedMeasurementService.Object,
                  mockedFactory.Object);

            // Act
            controller.Stats(userId);

            // Assert
            Assert.IsFalse(model.CanDelete);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestStats_WithoutIdAndIsAuthenticated_ShouldCallMeasurementServiceGetUserMeasurementsSortedByDate(string userId)
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();
            mockedAuthenticationProvider.Setup(p => p.CurrentUserId).Returns(userId);
            mockedAuthenticationProvider.Setup(p => p.IsAuthenticated).Returns(true);

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                 mockedMeasurementService.Object,
                 mockedFactory.Object);

            // Act
            controller.Stats(null);

            // Assert
            mockedMeasurementService.Verify(s => s.GetUserMeasurementsSortedByDate(userId), Times.Once);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestStats_WithoutIdAndIsNotAuthenticated_ShouldCallMeasurementServiceGetUserMeasurementsSortedByDateWithNull(string userId)
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();
            mockedAuthenticationProvider.Setup(p => p.CurrentUserId).Returns(userId);

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                 mockedMeasurementService.Object,
                 mockedFactory.Object);

            // Act
            controller.Stats(null);

            // Assert
            mockedMeasurementService.Verify(s => s.GetUserMeasurementsSortedByDate(null), Times.Once);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestStats_WithId_ShouldCallMeasurementServiceGetUserMeasurementsSortedByDate(string userId)
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();
            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                 mockedMeasurementService.Object,
                 mockedFactory.Object);

            // Act
            controller.Stats(userId);

            // Assert
            mockedMeasurementService.Verify(s => s.GetUserMeasurementsSortedByDate(userId), Times.Once);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestStats_ShouldCallFactoryCreateMeasurementStatsViewModel(string userId)
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var measurements = new List<Measurement> { new Measurement(), new Measurement() };

            var mockedMeasurementService = new Mock<IMeasurementService>();
            mockedMeasurementService.Setup(s => s.GetUserMeasurementsSortedByDate(It.IsAny<string>())).Returns(measurements);

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();
            mockedAuthenticationProvider.Setup(p => p.CurrentUserId).Returns(userId);

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                  mockedMeasurementService.Object,
                  mockedFactory.Object);

            // Act
            controller.Stats(null);

            // Assert
            mockedFactory.Verify(f => f.CreateMeasurementStatsViewModel(measurements), Times.Once);
        }

        [TestCase("d547a40d-c45f-4c43-99de-0bfe9199ff95")]
        [TestCase("99ae8dd3-1067-4141-9675-62e94bb6caaa")]
        public void TestStats_ShouldRenderDefaultViewWithCorrectModel(string userId)
        {
            // Arrange
            var model = new MeasurementStatsViewModel();

            var mockedFactory = new Mock<IViewModelFactory>();
            mockedFactory.Setup(f => f.CreateMeasurementStatsViewModel(It.IsAny<IEnumerable<Measurement>>())).Returns(model);

            var mockedMeasurementService = new Mock<IMeasurementService>();

            var mockedAuthenticationProvider = new Mock<IAuthenticationProvider>();
            mockedAuthenticationProvider.Setup(p => p.CurrentUserId).Returns(userId);

            var controller = new MeasurementController(mockedAuthenticationProvider.Object,
                 mockedMeasurementService.Object,
                 mockedFactory.Object);

            // Act, Assert
            controller
                .WithCallTo(c => c.Stats(null))
                .ShouldRenderDefaultPartialView()
                .WithModel<MeasurementStatsViewModel>(m => Assert.AreSame(model, m));
        }
    }
}
