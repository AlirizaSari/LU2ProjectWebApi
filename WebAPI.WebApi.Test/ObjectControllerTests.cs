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

namespace WebAPI.WebApi.Test
{
    [TestClass]
    public class ObjectControllerTests
    {
        private Mock<IObjectRepository> _mockObjectRepository;
        private Mock<IEnvironmentRepository> _mockEnvironmentRepository;
        private Mock<ILogger<ObjectController>> _mockLogger;
        private ObjectController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockObjectRepository = new Mock<IObjectRepository>();
            _mockEnvironmentRepository = new Mock<IEnvironmentRepository>();
            _mockLogger = new Mock<ILogger<ObjectController>>();
            _controller = new ObjectController(
                _mockObjectRepository.Object,
                _mockEnvironmentRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Get_ReturnsObjects()
        {
            // Arrange
            var objects = new List<Object2D>
            {
                new Object2D { Id = Guid.NewGuid(), PrefabId = "Prefab1", PositionX = 1.0f, PositionY = 2.0f },
                new Object2D { Id = Guid.NewGuid(), PrefabId = "Prefab2", PositionX = 3.0f, PositionY = 4.0f }
            };
            _mockObjectRepository.Setup(r => r.ReadAllAsync()).ReturnsAsync(objects);

            // Act
            var result = await _controller.Get();
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<Object2D>));
            Assert.AreEqual(objects.Count, ((IEnumerable<Object2D>)okResult.Value).Count());
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFound_WhenObjectDoesNotExist()
        {
            // Arrange
            _mockObjectRepository.Setup(r => r.ReadAsync(It.IsAny<Guid>())).ReturnsAsync((Object2D)null);

            // Act
            var result = await _controller.Get(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetById_ReturnsObject_WhenExists()
        {
            // Arrange
            var objId = Guid.NewGuid();
            var obj = new Object2D { Id = objId, PrefabId = "TestPrefab", PositionX = 5.0f, PositionY = 6.0f };
            _mockObjectRepository.Setup(r => r.ReadAsync(objId)).ReturnsAsync(obj);

            // Act
            var result = await _controller.Get(objId);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(obj, okResult.Value);
        }

        [TestMethod]
        public async Task GetByEnvironment_ReturnsObjects()
        {
            // Arrange
            var envId = Guid.NewGuid();
            var objects = new List<Object2D>
            {
                new Object2D { Id = Guid.NewGuid(), PrefabId = "Prefab1", EnvironmentId = envId },
                new Object2D { Id = Guid.NewGuid(), PrefabId = "Prefab2", EnvironmentId = envId }
            };
            _mockObjectRepository.Setup(r => r.ReadByEnvironmentIdAsync(envId)).ReturnsAsync(objects);

            // Act
            var result = await _controller.GetByEnvironment(envId);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(objects, okResult.Value);
        }

        [TestMethod]
        public async Task Add_ReturnsCreatedObject()
        {
            // Arrange
            var envId = Guid.NewGuid();
            var newObject = new Object2D { PrefabId = "PrefabNew", PositionX = 7.0f, PositionY = 8.0f };
            var createdObject = new Object2D { Id = Guid.NewGuid(), PrefabId = "PrefabNew", EnvironmentId = envId };

            _mockObjectRepository.Setup(r => r.InsertAsync(It.IsAny<Object2D>())).ReturnsAsync(createdObject);

            // Act
            var result = await _controller.Add(envId, newObject);
            var createdResult = result.Result as CreatedAtRouteResult;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdObject, createdResult.Value);
        }
    }
}
