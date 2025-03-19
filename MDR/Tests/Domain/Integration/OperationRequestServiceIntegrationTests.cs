using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Hospital.Domain.OperationRequest;
using Hospital.Domain.Shared;
using Hospital.ViewModels;
using Hospital.Infrastructure;
using Hospital.Infrastructure.operationrequestmanagement;
using Hospital.Services;
using Hospital.Domain.Users.SystemUser;

namespace Hospital.Tests.Domain.Integration
{
    public class OperationRequestServiceIntegrationTests : IClassFixture<ServiceProviderFixture>
    {
        private readonly ServiceProvider _serviceProvider;

        public OperationRequestServiceIntegrationTests(ServiceProviderFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;

            // Seed data
            SeedData();
        }

        private void SeedData()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

            var operationRequest = new OperationRequest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "some-operation-type-id",
                DateTime.UtcNow.AddDays(7),
                1
            );

            context.OperationRequests.Add(operationRequest);
            context.SaveChanges();
        }

        [Fact]
        public async Task CreateOperationRequestAsync_ShouldCreateRequest()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<OperationRequestService>();
            var model = new OperationRequestViewModel
            {
                PatientID = Guid.NewGuid(),
                DoctorID = Guid.NewGuid(),
                OperationTypeID = "some-operation-type-id",
                DeadlineDate = DateTime.UtcNow.AddDays(7),
                Priority = 1
            };

            // Act
            var result = await service.CreateOperationRequestAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.PatientID, result.PatientID);
            Assert.Equal(model.DoctorID, result.DoctorID);
            Assert.Equal(model.OperationTypeID, result.OperationTypeID);
            Assert.Equal(model.DeadlineDate, result.DeadlineDate);
            Assert.Equal(model.Priority, result.Priority);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRequests()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<OperationRequestService>();

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRequest()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<OperationRequestService>();
            var context = _serviceProvider.GetRequiredService<HospitalDbContext>();
            var existingRequest = context.OperationRequests.FirstOrDefault();
            var requestId = new OperationRequestId(existingRequest.Id.AsGuid());

            // Act
            var result = await service.GetByIdAsync(requestId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateOperationRequestAsync_ShouldUpdateRequest()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<OperationRequestService>();
            var context = _serviceProvider.GetRequiredService<HospitalDbContext>();
            var existingRequest = context.OperationRequests.FirstOrDefault();
            var requestId = new OperationRequestId(existingRequest.Id.AsGuid());
            var dto = new OperationRequestDto
            {
                Id = requestId.AsGuid(),
                OperationTypeID = "updated-type",
                DeadlineDate = DateTime.UtcNow.AddDays(10),
                Priority = 2
            };

            // Act
            var result = await service.UpdateOperationRequestAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated-type", result.OperationTypeID);
        }

        [Fact]
        public async Task DeleteOperationRequestAsync_ShouldDeleteRequest()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<OperationRequestService>();
            var context = _serviceProvider.GetRequiredService<HospitalDbContext>();
            var existingRequest = context.OperationRequests.FirstOrDefault();
            var requestId = new OperationRequestId(existingRequest.Id.AsGuid());

            // Act
            var result = await service.DeleteOperationRequestAsync(requestId);

            // Assert
            Assert.NotNull(result);
        }
    }

    public class ServiceProviderFixture : IDisposable
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public ServiceProviderFixture()
        {
            var services = new ServiceCollection();

            // Register your DbContext with an in-memory database
            services.AddDbContext<HospitalDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Register your services and repositories here
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOperationRequestRepository, OperationRequestRepository>();
            services.AddScoped<OperationRequestService>();

            // Register the IPasswordService
            services.AddScoped<IPasswordService, MockPasswordService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private class MockPasswordService : IPasswordService
        {
            public string GenerateTemporaryPassword(string passwd)
            {
                return "temporaryPassword";
            }

            public string HashPassword(string password)
            {
                return "hashedPassword";
            }

            public bool VerifyPassword(string hashedPassword, string password)
            {
                return hashedPassword == "hashedPassword" && password == "temporaryPassword";
            }

            public bool ValidatePassword(string password, string confirmPassword)
            {
                return password == confirmPassword;
            }
        }
    }
}