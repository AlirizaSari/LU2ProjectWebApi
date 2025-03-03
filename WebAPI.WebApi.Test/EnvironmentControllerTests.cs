using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProjectLU2.WebApi.Controllers;
using ProjectLU2.WebApi.Models;
using ProjectLU2.WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;

namespace WebAPI.WebApi.Test
{
    [TestClass]
    public class EnvironmentControllerTests
    {
        private Mock<IEnvironmentRepository> _mockEnvironmentRepository;
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private Mock<ILogger<EnvironmentController>> _mockLogger;
        private EnvironmentController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockEnvironmentRepository = new Mock<IEnvironmentRepository>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<EnvironmentController>>();
            _controller = new EnvironmentController(
                _mockEnvironmentRepository.Object,
                _mockAuthenticationService.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Get_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns<string>(null);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Get_ReturnsEnvironments_WhenUserAuthenticated()
        {
            // Arrange
            var userId = "test-user";
            var environments = new List<Environment2D> { new Environment2D { Id = Guid.NewGuid(), Name = "Test Environment", OwnerUserId = userId } };
            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockEnvironmentRepository.Setup(r => r.ReadByUserIdAsync(userId)).ReturnsAsync(environments);

            // Act
            var result = await _controller.Get();
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<Environment2D>));
            Assert.AreEqual(1, (okResult.Value as IEnumerable<Environment2D>).Count());
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFound_WhenEnvironmentDoesNotExist()
        {
            // Arrange
            _mockEnvironmentRepository.Setup(r => r.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Environment2D)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetById_ReturnsEnvironment_WhenExists()
        {
            // Arrange
            var envId = Guid.NewGuid();
            var environment = new Environment2D { Id = envId, Name = "Test", OwnerUserId = "user" };
            _mockEnvironmentRepository.Setup(r => r.ReadAsync(envId)).ReturnsAsync(environment);

            // Act
            var result = await _controller.Get(envId);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(environment, okResult.Value);
        }

        [TestMethod]
        public async Task Add_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns<string>(null);

            // Act
            var result = await _controller.Add(new Environment2D());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Add_ReturnsBadRequest_WhenUserExceedsMaxEnvironments()
        {
            // Arrange
            var userId = "test-user";

            var maxEnvironments = Environment2D.MaxNumberOfEnvironments;

            var existingEnvironments = Enumerable.Range(0, maxEnvironments)
                .Select(i => new Environment2D { Id = Guid.NewGuid(), Name = $"Env {i}", OwnerUserId = userId })
                .ToList();

            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockEnvironmentRepository.Setup(r => r.ReadByUserIdAsync(userId)).ReturnsAsync(existingEnvironments);

            var newEnvironment = new Environment2D { Name = "New Env" };

            // Act
            var result = await _controller.Add(newEnvironment);

            // Assert
            Assert.AreEqual(maxEnvironments, existingEnvironments.Count, "Aantal bestaande omgevingen is niet gelijk aan de limiet.");
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Add_ReturnsBadRequest_WhenEnvironmentNameAlreadyExists()
        {
            // Arrange
            var userId = "test-user";
            var existingEnvironments = new List<Environment2D> 
            {
                new Environment2D { Id = Guid.NewGuid(), Name = "ExistingEnv", OwnerUserId = userId }
            };

            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockEnvironmentRepository.Setup(r => r.ReadByUserIdAsync(userId)).ReturnsAsync(existingEnvironments);

            var newEnvironment = new Environment2D { Name = "ExistingEnv" };

            // Act
            var result = await _controller.Add(newEnvironment);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenEnvironmentDoesNotExist()
        {
            // Arrange
            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns("user");
            _mockEnvironmentRepository.Setup(r => r.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Environment2D)null);

            // Act
            var result = await _controller.Update(Guid.NewGuid(), new Environment2D());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenEnvironmentIsUpdatedSuccessfully()
        {
            // Arrange
            var userId = "test-user";
            var envId = Guid.NewGuid();
            var existingEnvironment = new Environment2D { Id = envId, Name = "Old Name", OwnerUserId = userId };

            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockEnvironmentRepository.Setup(r => r.ReadAsync(envId)).ReturnsAsync(existingEnvironment);

            var updatedEnvironment = new Environment2D { Name = "New Name" };

            // Act
            var result = await _controller.Update(envId, updatedEnvironment);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Update_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns<string>(null);

            var envId = Guid.NewGuid();
            var updatedEnvironment = new Environment2D { Id = envId, Name = "UpdatedEnv" };

            // Act
            var result = await _controller.Update(envId, updatedEnvironment);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenEnvironmentDoesNotExist()
        {
            // Arrange
            var userId = "test-user";
            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockEnvironmentRepository.Setup(r => r.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Environment2D)null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns<string>(null);

            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsOk_WhenEnvironmentIsDeletedSuccessfully()
        {
            // Arrange
            var userId = "test-user";
            var envId = Guid.NewGuid();
            var existingEnvironment = new Environment2D { Id = envId, Name = "Env to Delete", OwnerUserId = userId };

            _mockAuthenticationService.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockEnvironmentRepository.Setup(r => r.ReadAsync(envId)).ReturnsAsync(existingEnvironment);
            _mockEnvironmentRepository.Setup(r => r.DeleteAsync(envId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(envId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

    }
}