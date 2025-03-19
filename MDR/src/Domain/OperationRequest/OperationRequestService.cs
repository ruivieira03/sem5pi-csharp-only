using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.ViewModels;
using Hospital.Domain.Shared;
using Hospital.Services;

namespace Hospital.Domain.OperationRequest
{
    public class OperationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationRequestRepository _operationRequestRepository;

        public OperationRequestService(IUnitOfWork unitOfWork, IOperationRequestRepository operationRequestRepository)
        {
            this._unitOfWork = unitOfWork;
            this._operationRequestRepository = operationRequestRepository;
        }

        public async Task<OperationRequestDto> CreateOperationRequestAsync(OperationRequestViewModel model)
        {
            // Create a new OperationRequest
            var newRequest = new OperationRequest(
                model.PatientID,
                model.DoctorID,
                model.OperationTypeID,
                model.DeadlineDate,
                model.Priority
            );

            // Save the request to the repository
            await _operationRequestRepository.AddOperationRequestAsync(newRequest);

            // Commit the transaction
            await _unitOfWork.CommitAsync();

            // Return a DTO with the new request’s details
            return new OperationRequestDto
            {
                Id = newRequest.Id.AsGuid(),
                PatientID = newRequest.PatientID,
                DoctorID = newRequest.DoctorID,
                OperationTypeID = newRequest.OperationTypeID,
                DeadlineDate = newRequest.DeadlineDate,
                Priority = newRequest.Priority
            };
        }

        //fetch all operation requests
        public async Task<IEnumerable<OperationRequestDto>> GetAllAsync()
        {
            // Get all operation requests from the repository
            var requests = await _operationRequestRepository.GetAllAsync();

            // Map the requests to DTOs
            return requests.Select(request => new OperationRequestDto
            {
                Id = request.Id.AsGuid(),
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                OperationTypeID = request.OperationTypeID,
                DeadlineDate = request.DeadlineDate,
                Priority = request.Priority
            });
        }

        //fetch operation request by id
        public async Task<OperationRequestDto> GetByIdAsync(OperationRequestId id)
        {
            // Get the operation request from the repository
            var request = await _operationRequestRepository.GetByIdAsync(id);

            // Map the request to a DTO
            return new OperationRequestDto
            {
                Id = request.Id.AsGuid(),
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                OperationTypeID = request.OperationTypeID,
                DeadlineDate = request.DeadlineDate,
                Priority = request.Priority
            };
        }

        //fetch operation request by patient name
        public async Task<IEnumerable<OperationRequestDto>> GetOperationRequestsByPatientAsync(Guid PatientId)
        {
            // Get all operation requests by patient name from the repository
            var requests = await _operationRequestRepository.GetOperationRequestsByPatientAsync(PatientId);

            // Map the requests to DTOs
            return requests.Select(request => new OperationRequestDto
            {
                Id = request.Id.AsGuid(),
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                OperationTypeID = request.OperationTypeID,
                DeadlineDate = request.DeadlineDate,
                Priority = request.Priority
            });
        }

        //fetch operation request by type
        public async Task<IEnumerable<OperationRequestDto>> GetOperationRequestsByTypeAsync(string operationTypeId)
        {
            // Get all operation requests by type from the repository
            var requests = await _operationRequestRepository.GetOperationRequestsByTypeAsync(operationTypeId);

            // Map the requests to DTOs
            return requests.Select(request => new OperationRequestDto
            {
                Id = request.Id.AsGuid(),
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                OperationTypeID = request.OperationTypeID,
                DeadlineDate = request.DeadlineDate,
                Priority = request.Priority
            });
        }

        //fetch operation request by priority
        public async Task<IEnumerable<OperationRequestDto>> GetOperationRequestsByPriorityAsync(int priority)
        {
            // Get all operation requests by priority from the repository
            var requests = await _operationRequestRepository.GetOperationRequestsByPriorityAsync(priority);

            // Map the requests to DTOs
            return requests.Select(request => new OperationRequestDto
            {
                Id = request.Id.AsGuid(),
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                OperationTypeID = request.OperationTypeID,
                DeadlineDate = request.DeadlineDate,
                Priority = request.Priority
            });
        }

        /* STATUS BELONGS TO APPOINTMENT (See 3.4 and 3.6 on the specifications document)
        //fetch operation request by status
        public async Task<IEnumerable<OperationRequestDto>> GetOperationRequestsByStatusAsync(string status)
        {
            // Get all operation requests by status from the repository
            var requests = await _operationRequestRepository.GetOperationRequestsByStatusAsync(status);

            // Map the requests to DTOs
            return requests.Select(request => new OperationRequestDto
            {
                ID = request.ID.AsGuid(),
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                OperationTypeID = request.OperationTypeID,
                DeadlineDate = request.DeadlineDate,
                Priority = request.Priority
            });
        }
        */

        //Update operation request
        public async Task<OperationRequestDto> UpdateOperationRequestAsync(OperationRequestDto dto)
        {
            // Get the operation request from the repository
            var request = await _operationRequestRepository.GetByIdAsync(new OperationRequestId(dto.Id));

            if (request == null)
            {
                throw new Exception("Operation request not found");
            }

            // Update the request with the new details
            request.OperationTypeID = dto.OperationTypeID;
            request.DeadlineDate = dto.DeadlineDate;
            request.Priority = dto.Priority;

            // Save the updated request to the repository
            await _operationRequestRepository.UpdateOperationRequestAsync(request);

            // Commit the transaction
            await _unitOfWork.CommitAsync();

            // Return a DTO with the updated request’s details
            return new OperationRequestDto
            {
                Id = request.Id.AsGuid(),
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                OperationTypeID = request.OperationTypeID,
                DeadlineDate = request.DeadlineDate,
                Priority = request.Priority
            };
        }

        //Delete operation request
        public async Task<OperationRequestDto> DeleteOperationRequestAsync(OperationRequestId id)
        {
            // Get the operation request from the repository
            var request = await _operationRequestRepository.GetByIdAsync(id);

            if (request == null)
            {
                return null;
            }

            this._operationRequestRepository.Remove(request);
            await this._unitOfWork.CommitAsync();

            return new OperationRequestDto
            {
                Id = request.Id.AsGuid(),
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                OperationTypeID = request.OperationTypeID,
                DeadlineDate = request.DeadlineDate,
                Priority = request.Priority
            };
        }
    }
}