/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Domain.Shared;
using Hospital.Infraestructure;

namespace Hospital.Domain.Patients{
    public class PatientService{
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PatientService(IPatientRepository patientRepository, IUnitOfWork unitOfWork){

            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
        }


        /*

         public async Task<List<PatientDto>> ListPatiensByDifferentAttributes(){

            var patients = await _patientRepository.GetAllAsync();
            List<PatientDto> patientDto = patients.ConvertAll(patient => new PatientDto{
                Id = patient.Id.AsGuid(),
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                EmergencyContact = patient.EmergencyContact,
                AllergiesOrMedicalConditions = patient.AllergiesOrMedicalConditions,
                AppointmentHistory = patient.AppointmentHistory
            });
            return patientDto; // To DO this .
        }
        */

    


