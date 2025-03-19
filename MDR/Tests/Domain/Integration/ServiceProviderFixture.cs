using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Hospital.Domain.OperationRequest;
using Hospital.Domain.Shared;
using Hospital.Infrastructure;
using Hospital.Infrastructure.operationrequestmanagement;
using Hospital.Services;
using Hospital.Domain.Users.SystemUser;

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