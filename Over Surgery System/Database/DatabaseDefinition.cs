namespace OverSurgerySystem
{
    public partial class Database
    {
        // Default database connection settings.
        private const string DATABASE_NAME          = "oversurgerysystem";
        private const string DATABASE_ADDRESS       = "localhost";
        private const string DATABASE_USER          = "root";
        private const string DATABASE_PASSWORD      = "root";
        private const int DATABASE_PORT             = 3306;

        // Static class containing all the names of the tables in use, for reference and to avoid magic strings.
        public static class Tables
        {
            public const string POSTAL_CODES                = "postal_codes";
            public const string CITIES                      = "cities";
            public const string STATES                      = "states";
            public const string COUNTRIES                   = "countries";
            public const string CONTACT_NUMBERS             = "contact_numbers";
            public const string IDENTIFICATIONS             = "identifications";
            public const string PERSONAL_DETAILS            = "personal_details";
            public const string STAFFS                      = "staffs";
            public const string WORKING_DAYS                = "working_days";
            public const string LEAVE_DATES                 = "leave_dates";
            public const string RECEPTIONISTS               = "receptionists";
            public const string MEDICAL_STAFFS              = "medical_staffs";
            public const string PATIENTS                    = "patients";
            public const string APPOINTMENTS                = "appointments";
            public const string TEST_RESULTS                = "test_results";
            public const string PRESCRIPTIONS               = "prescriptions";
            public const string MEDICATIONS                 = "medications";
            public const string PRESCRIPTION_MEDICATIONS    = "prescription_medications";
        
            // Static string indicating some of the columns common across all database tables.
            public static class Generic
            {
                public const string ID                          = "id";
                public const string LAST_UPDATED_COLUMN_NAME    = "last_updated";
                public const string CREATED_ON_COLUMN_NAME      = "created_on";
            }
        
            // Column names for postal codes
            public static class PostalCodes
            {
                public const string Code    = "code";
                public const string CityId  = "city_id";
            }
        
            // Column names for cities
            public static class Cities
            {
                public const string Name    = "name";
                public const string StateId = "state_id";
            }
        
            // Column names for states
            public static class States
            {
                public const string Name        = "name";
                public const string CountryId   = "country_id";
            }
        
            // Column names for countries
            public static class Countries
            {
                public const string Name    = "name";
            }
        
            // Column names for contact numbers
            public static class ContactNumbers
            {
                public const string Number      = "number";
                public const string DetailsId   = "personal_details_id";
            }
        
            // Column names for identifications
            public static class Identifications
            {
                public const string Value       = "value";
                public const string DetailsId   = "personal_details_id";
            }
        
            // Column names for personal details
            public static class PersonalDetails
            {
                public const string FirstName   = "first_name";
                public const string LastName    = "last_name";
                public const string Address     = "address";
                public const string PostcodeId  = "postcode_id";
                public const string DateOfBirth = "date_of_birth";
                public const string Sex         = "sex";
            }
        
            // Column names for staffs
            public static class Staffs
            {
                public const string Type        = "type";
                public const string DetailsId   = "personal_details_id";
                public const string Password    = "password";
                public const string DateJoined  = "date_joined";
                public const string Active      = "active";
            }
        
            // Column names for working days
            public static class WorkingDays
            {
                public const string StaffId     = "staff_id";
                public const string Sunday      = "day_1";
                public const string Monday      = "day_2";
                public const string Tuesday     = "day_3";
                public const string Wednesday   = "day_4";
                public const string Thursday    = "day_5";
                public const string Friday      = "day_6";
                public const string Saturday    = "day_7";
            }
        
            // Column names for leave dates
            public static class LeaveDates
            {
                public const string StaffId = "staff_id";
                public const string Date    = "date";
            }
        
            // Column names for receptionists
            public static class Receptionists
            {
                public const string StaffId = "staff_id";
                public const string Admin   = "admin";
            }
        
            // Column names for medical staffs
            public static class MedicalStaffs
            {
                public const string StaffId     = "staff_id";
                public const string LicenseNo   = "license_no";
            }
        
            // Column names for patients
            public static class Patients
            {
                public const string DetailsId   = "personal_details_id";
            }
        
            // Column names for appointments
            public static class Appointments
            {
                public const string MedicalStaffId  = "medical_staff_id";
                public const string PatientId       = "patient_id";
                public const string DateAppointed   = "date_appointed";
                public const string Cancelled       = "cancelled";
            }
        
            // Column names for test results
            public static class TestResults
            {
                public const string PatientId           = "patient_id";
                public const string Name                = "name";
                public const string MedicalLicenseNo    = "medical_license_no";
                public const string Description         = "description";
                public const string Result              = "result";
                public const string Remark              = "remark";
            }
        
            // Column names for prescriptions
            public static class Prescriptions
            {
                public const string PrescriberId    = "prescriber_id";
                public const string PatientId       = "patient_id";
                public const string Name            = "name";
                public const string StartDate       = "start_date";
                public const string EndDate         = "end_date";
                public const string Remark          = "remark";
            }
        
            // Column names for medications
            public static class Medications
            {
                public const string Code    = "code";
                public const string Name    = "name";
            }
        
            // Column names for prescription's medications list
            public static class PrescriptionMedications
            {
                public const string PrescriptionId  = "presciption_id";
                public const string MedicationId    = "medication_id";
            }
        }
    }
}
