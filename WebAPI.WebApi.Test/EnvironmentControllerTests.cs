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
        public async Task Add_ReturnsCreatedAtRoute_WhenEnvironmentIsAddedSuccessfully()
        {
            // Arrange
            var userId = "test-user";
            var newEnvironment = new Environment2D { Name = "New Environment", OwnerUserId = userId };

            _mockAuthenticationService!.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockEnvironmentRepository!.Setup(r => r.InsertAsync(newEnvironment)).ReturnsAsync(newEnvironment);

            // Act
            var result = await _controller!.Add(newEnvironment);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
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
    }
}