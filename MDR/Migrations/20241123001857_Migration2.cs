using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sem5pi_24_25_g034.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OperationRequest",
                keyColumn: "Id",
                keyValue: new Guid("991a8ef3-c8e9-4e76-ab93-e14948c8b2c7"));

            migrationBuilder.DeleteData(
                table: "OperationRequest",
                keyColumn: "Id",
                keyValue: new Guid("d2739ced-14f0-4176-969b-6b68d78a9e16"));

            migrationBuilder.DeleteData(
                table: "OperationRequest",
                keyColumn: "Id",
                keyValue: new Guid("f60c21dc-08ac-4391-bd0b-cb79414c2aeb"));

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("d3e7d836-26b4-4dfb-bbdc-d3fd68bdb148"));

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("ecee7034-ec21-456e-b67f-83029c1d3983"));

            migrationBuilder.DeleteData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: new Guid("0ef57135-1af2-4773-b135-2c24eedad910"));

            migrationBuilder.DeleteData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: new Guid("c59b6105-1b75-466c-9b73-dbce16afe9ec"));

            migrationBuilder.DeleteData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: new Guid("c5b02e09-6fd6-4abb-8b08-66639bb90d3e"));

            migrationBuilder.InsertData(
                table: "OperationRequest",
                columns: new[] { "Id", "DeadlineDate", "DoctorID", "OperationTypeID", "PatientID", "Priority" },
                values: new object[,]
                {
                    { new Guid("1f2fc28f-9729-447a-8960-2f58c8f55ec1"), new DateTime(2022, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("687930d3-beed-4d91-986b-d29cce841c88"), "3", new Guid("40af2889-c958-4dad-ae6e-019d5cee2788"), 3 },
                    { new Guid("4963e4fd-d6a2-4c3e-8bbb-26a4985c1271"), new DateTime(2022, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("bfcef3f8-0a0e-4af1-8b2d-a63905ede16a"), "2", new Guid("e305b6ec-bfda-43d5-b828-a167993bdd85"), 2 },
                    { new Guid("d522f7f0-9eae-4513-8838-c2123456f301"), new DateTime(2022, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("dfe418a4-b020-4f3d-8559-b27af20105d0"), "1", new Guid("1b74aef0-8af6-4c67-bdee-17dc5fc7d32f"), 1 }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "AllergiesOrMedicalConditions", "AppointmentHistory", "DateOfBirth", "Email", "EmergencyContact", "FirstName", "Gender", "LastName", "MedicalRecordNumber", "PhoneNumber" },
                values: new object[,]
                {
                    { new Guid("5922bb42-daa2-4e85-8a56-0b340d3100ce"), "[\"Penicillin allergy\"]", "[\"Checkup on 2024-01-20\"]", new DateTime(1985, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "1220741@isep.ipp.pt", "0987654321", "Bernardo", "Male", "Giao", "MRN123456", "1234567890" },
                    { new Guid("b4f8511b-e64e-4a98-a333-1f6eb8999461"), "[\"Nut allergy\"]", "[\"Vaccination on 2023-05-15\"]", new DateTime(1999, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "ruimdvieir@gmail.com", "0987654322", "Rui", "Male", "Vieira", "MRN987654", "1234567891" }
                });

            migrationBuilder.InsertData(
                table: "SystemUser",
                columns: new[] { "Id", "DeleteToken", "Email", "IAMId", "Password", "PatientId", "PhoneNumber", "ResetToken", "Role", "TokenExpiry", "Username", "VerifyToken", "isVerified" },
                values: new object[,]
                {
                    { new Guid("1490d277-8118-48d6-9c80-bf0b9da3b29d"), null, "doctor@hospital.com", "2", "bf32388f0f958a12428ebc237a8d0863265e795ceb5c5f3d013b062f75bfad9e", null, "1234567891", null, 1, null, "doctorUser", null, true },
                    { new Guid("3bdfe1e8-6fbc-47b5-b14e-64dbe3a7f734"), null, "ruimdv13@gmail.com", "1", "bf32388f0f958a12428ebc237a8d0863265e795ceb5c5f3d013b062f75bfad9e", null, "912028969", null, 0, null, "adminUser", null, true },
                    { new Guid("4ef9458f-3993-4175-ba2b-a53624db896b"), null, "nurse@hospital.com", "3", "bf32388f0f958a12428ebc237a8d0863265e795ceb5c5f3d013b062f75bfad9e", null, "1234567892", null, 2, null, "nurseUser", null, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OperationRequest",
                keyColumn: "Id",
                keyValue: new Guid("1f2fc28f-9729-447a-8960-2f58c8f55ec1"));

            migrationBuilder.DeleteData(
                table: "OperationRequest",
                keyColumn: "Id",
                keyValue: new Guid("4963e4fd-d6a2-4c3e-8bbb-26a4985c1271"));

            migrationBuilder.DeleteData(
                table: "OperationRequest",
                keyColumn: "Id",
                keyValue: new Guid("d522f7f0-9eae-4513-8838-c2123456f301"));

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("5922bb42-daa2-4e85-8a56-0b340d3100ce"));

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("b4f8511b-e64e-4a98-a333-1f6eb8999461"));

            migrationBuilder.DeleteData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: new Guid("1490d277-8118-48d6-9c80-bf0b9da3b29d"));

            migrationBuilder.DeleteData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: new Guid("3bdfe1e8-6fbc-47b5-b14e-64dbe3a7f734"));

            migrationBuilder.DeleteData(
                table: "SystemUser",
                keyColumn: "Id",
                keyValue: new Guid("4ef9458f-3993-4175-ba2b-a53624db896b"));

            migrationBuilder.InsertData(
                table: "OperationRequest",
                columns: new[] { "Id", "DeadlineDate", "DoctorID", "OperationTypeID", "PatientID", "Priority" },
                values: new object[,]
                {
                    { new Guid("991a8ef3-c8e9-4e76-ab93-e14948c8b2c7"), new DateTime(2022, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("6209c907-2977-45a1-b583-aa539eb426fc"), "3", new Guid("60796f2a-1313-41c0-ae5a-f6e08bb7925b"), 3 },
                    { new Guid("d2739ced-14f0-4176-969b-6b68d78a9e16"), new DateTime(2022, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("719e5a15-e611-4ada-8a0e-c74aba732b85"), "1", new Guid("bec063b5-cc8c-424d-a6f8-7a96c63d3db2"), 1 },
                    { new Guid("f60c21dc-08ac-4391-bd0b-cb79414c2aeb"), new DateTime(2022, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("3e6813df-ed69-4c60-a396-6033ae4a5533"), "2", new Guid("79e01a30-f4cc-41c3-bfc7-d4b950b74140"), 2 }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "AllergiesOrMedicalConditions", "AppointmentHistory", "DateOfBirth", "Email", "EmergencyContact", "FirstName", "Gender", "LastName", "MedicalRecordNumber", "PhoneNumber" },
                values: new object[,]
                {
                    { new Guid("d3e7d836-26b4-4dfb-bbdc-d3fd68bdb148"), "[\"Penicillin allergy\"]", "[\"Checkup on 2024-01-20\"]", new DateTime(1985, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "1220741@isep.ipp.pt", "0987654321", "Bernardo", "Male", "Giao", "MRN123456", "1234567890" },
                    { new Guid("ecee7034-ec21-456e-b67f-83029c1d3983"), "[\"Nut allergy\"]", "[\"Vaccination on 2023-05-15\"]", new DateTime(1999, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "ruimdvieir@gmail.com", "0987654322", "Rui", "Male", "Vieira", "MRN987654", "1234567891" }
                });

            migrationBuilder.InsertData(
                table: "SystemUser",
                columns: new[] { "Id", "DeleteToken", "Email", "IAMId", "Password", "PatientId", "PhoneNumber", "ResetToken", "Role", "TokenExpiry", "Username", "VerifyToken", "isVerified" },
                values: new object[,]
                {
                    { new Guid("0ef57135-1af2-4773-b135-2c24eedad910"), null, "nurse@hospital.com", "3", "bf32388f0f958a12428ebc237a8d0863265e795ceb5c5f3d013b062f75bfad9e", null, "1234567892", null, 2, null, "nurseUser", null, true },
                    { new Guid("c59b6105-1b75-466c-9b73-dbce16afe9ec"), null, "doctor@hospital.com", "2", "bf32388f0f958a12428ebc237a8d0863265e795ceb5c5f3d013b062f75bfad9e", null, "1234567891", null, 1, null, "doctorUser", null, true },
                    { new Guid("c5b02e09-6fd6-4abb-8b08-66639bb90d3e"), null, "ruimdv13@gmail.com", "1", "bf32388f0f958a12428ebc237a8d0863265e795ceb5c5f3d013b062f75bfad9e", null, "912028969", null, 0, null, "adminUser", null, true }
                });
        }
    }
}
