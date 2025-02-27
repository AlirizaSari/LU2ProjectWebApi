using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProjectLU2.WebApi.Models;
using ProjectLU2.WebApi.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.WebApi.Test
{
    [TestClass]
    public class EnvironmentTests
    {
        private Mock<IEnvironmentRepository> _mockRepository;
        private Environment2D _testEnvironment;
        private ClaimsPrincipal _mockUser;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IEnvironmentRepository>();
            _mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _testEnvironment = new Environment2D
            {
                Id = Guid.NewGuid(),
                Name = "Test Environment",
                OwnerUserId = _mockUser.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                MaxLength = 100,
                MaxHeight = 100
            };
        }

        [TestMethod]
        public async Task InsertAsync_ShouldInsertEnvironment()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.InsertAsync(It.IsAny<Environment2D>())).ReturnsAsync(_testEnvironment);

            // Act
            var result = await _mockRepository.Object.InsertAsync(_testEnvironment);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testEnvironment.Id, result.Id);
            Assert.AreEqual(_testEnvironment.OwnerUserId, _mockUser.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [TestMethod]
        public async Task ReadAsync_ShouldReturnEnvironment()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ReadAsync(It.IsAny<Guid>())).ReturnsAsync(_testEnvironment);

            // Act
            var result = await _mockRepository.Object.ReadAsync(_testEnvironment.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testEnvironment.Id, result.Id);
        }

        [TestMethod]
        public async Task ReadAllAsync_ShouldReturnAllEnvironments()
        {
            // Arrange
            var environments = new List<Environment2D> { _testEnvironment };
            _mockRepository.Setup(repo => repo.ReadAllAsync()).ReturnsAsync(environments);

            // Act
            var result = await _mockRepository.Object.ReadAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, ((List<Environment2D>)result).Count);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateEnvironment()
        {
            // Arrange
            _testEnvironment.Name = "Updated Environment";
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Environment2D>())).Returns(Task.CompletedTask);

            // Act
            await _mockRepository.Object.UpdateAsync(_testEnvironment);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateAsync(_testEnvironment), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteEnvironment()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

            // Act
            await _mockRepository.Object.DeleteAsync(_testEnvironment.Id);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteAsync(_testEnvironment.Id), Times.Once);
        }
    }
}