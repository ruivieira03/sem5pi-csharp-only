using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using Hospital.Domain.OperationRequest;
using Hospital.Domain.Shared;
using Hospital.ViewModels;

namespace Hospital.Tests.Domain.Unit
{
    public class OperationRequestServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOperationRequestRepository> _mockOperationRequestRepository;
        private readonly OperationRequestService _service;

        public OperationRequestServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOperationRequestRepository = new Mock<IOperationRequestRepository>();
            _service = new OperationRequestService(_mockUnitOfWork.Object, _mockOperationRequestRepository.Object);
        }

        [Fact]
        public async Task CreateOperationRequestAsync_ShouldCreateRequest()
        {
            // Arrange
            var model = new OperationRequestViewModel
            {
                PatientID = Guid.NewGuid(),
                DoctorID = Guid.NewGuid(),
                OperationTypeID = "some-operation-type-id",
                DeadlineDate = DateTime.UtcNow.AddDays(7),
                Priority = 1
            };

            // Act
            var result = await _service.CreateOperationRequestAsync(model);

            // Assert
            result.Should().NotBeNull();
            result.PatientID.Should().Be(model.PatientID);
            result.DoctorID.Should().Be(model.DoctorID);
            result.OperationTypeID.Should().Be(model.OperationTypeID);
            result.DeadlineDate.Should().Be(model.DeadlineDate);
            result.Priority.Should().Be(model.Priority);
            _mockOperationRequestRepository.Verify(repo => repo.AddOperationRequestAsync(It.IsAny<OperationRequest>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRequests()
        {
            // Arrange
            var requests = new List<OperationRequest>
            {
                new OperationRequest(Guid.NewGuid(), Guid.NewGuid(), "type1", DateTime.UtcNow, 1),
                new OperationRequest(Guid.NewGuid(), Guid.NewGuid(), "type2", DateTime.UtcNow, 2)
            };
            _mockOperationRequestRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().OperationTypeID.Should().Be("type1");
            result.Last().OperationTypeID.Should().Be("type2");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRequest()
        {
            // Arrange
            var requestId = new OperationRequestId(Guid.NewGuid());
            var request = new OperationRequest(requestId.AsGuid(), Guid.NewGuid(), "type1", DateTime.UtcNow, 1);
            _mockOperationRequestRepository.Setup(repo => repo.GetByIdAsync(requestId)).ReturnsAsync(request);

            // Act
            var result = await _service.GetByIdAsync(requestId);

            // Assert
            result.Should().NotBeNull();
            result.OperationTypeID.Should().Be("type1");
        }

        [Fact]
        public async Task UpdateOperationRequestAsync_ShouldUpdateRequest()
        {
            // Arrange
            var requestId = new OperationRequestId(Guid.NewGuid());
            var request = new OperationRequest(requestId.AsGuid(), Guid.NewGuid(), "type1", DateTime.UtcNow, 1);
            _mockOperationRequestRepository.Setup(repo => repo.GetByIdAsync(requestId)).ReturnsAsync(request);

            var dto = new OperationRequestDto
            {
                Id = requestId.AsGuid(),
                OperationTypeID = "updated-type",
                DeadlineDate = DateTime.UtcNow.AddDays(10),
                Priority = 2
            };

            // Act
            var result = await _service.UpdateOperationRequestAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.OperationTypeID.Should().Be("updated-type");
            _mockOperationRequestRepository.Verify(repo => repo.UpdateOperationRequestAsync(It.IsAny<OperationRequest>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteOperationRequestAsync_ShouldDeleteRequest()
        {
            // Arrange
            var requestId = new OperationRequestId(Guid.NewGuid());
            var request = new OperationRequest(requestId.AsGuid(), Guid.NewGuid(), "type1", DateTime.UtcNow, 1);
            _mockOperationRequestRepository.Setup(repo => repo.GetByIdAsync(requestId)).ReturnsAsync(request);

            // Act
            var result = await _service.DeleteOperationRequestAsync(requestId);

            // Assert
            result.Should().NotBeNull();
            _mockOperationRequestRepository.Verify(repo => repo.Remove(It.IsAny<OperationRequest>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }
    }
}