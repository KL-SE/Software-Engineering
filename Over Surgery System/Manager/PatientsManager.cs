using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Core.Base;
using System;
using System.Collections.Generic;

namespace OverSurgerySystem.Manager
{
    public class PatientsManager
    {
        static Manager<Patient>                 PatientManager;
        static Manager<TestResult>              TestResultManager;
        static Manager<Prescription>            PrescriptionManager;
        static Manager<PrescriptionMedication>  PrescriptionMedManager;
        static Manager<Medication>              MedicationManager;
        static Manager<Appointment>             AppointmentManager;
        
        private PatientsManager() { }
        static PatientsManager()
        {
            PatientManager          = new Manager<Patient>();
            TestResultManager       = new Manager<TestResult>();
            PrescriptionManager     = new Manager<Prescription>();
            PrescriptionMedManager  = new Manager<PrescriptionMedication>();
            MedicationManager       = new Manager<Medication>();
            AppointmentManager      = new Manager<Appointment>();
        }

        public static void Add( Patient obj                 )   { PatientManager.Add( obj );            }
        public static void Add( TestResult obj              )   { TestResultManager.Add( obj );         }
        public static void Add( Prescription obj            )   { PrescriptionManager.Add( obj );       }
        public static void Add( PrescriptionMedication obj  )   { PrescriptionMedManager.Add( obj );    }
        public static void Add( Medication obj              )   { MedicationManager.Add( obj );         }
        public static void Add( Appointment obj             )   { AppointmentManager.Add( obj );        }

        public static void Remove( Patient obj                  )   { PatientManager.Remove( obj );         }
        public static void Remove( TestResult obj               )   { TestResultManager.Remove( obj );      }
        public static void Remove( Prescription obj             )   { PrescriptionManager.Remove( obj );    }
        public static void Remove( PrescriptionMedication obj   )   { PrescriptionMedManager.Remove( obj ); }
        public static void Remove( Medication obj               )   { MedicationManager.Remove( obj );      }
        public static void Remove( Appointment obj              )   { AppointmentManager.Remove( obj );     }
        
        public static Patient                   GetPatient( int id                  )   { return PatientManager.Get( id );          }
        public static TestResult                GetTestResult( int id               )   { return TestResultManager.Get( id );       }
        public static Prescription              GetPrescription( int id             )   { return PrescriptionManager.Get( id );     }
        public static PrescriptionMedication    GetPrescriptionMedication( int id   )   { return PrescriptionMedManager.Get( id );  }
        public static Medication                GetMedication( int id               )   { return MedicationManager.Get( id );       }
        public static Appointment               GetAppointment( int id              )   { return AppointmentManager.Get( id );      }
        
        // Multi-Valued Getters
        public static List<Prescription> GetPrescriptions( int staffId , int patientId )
        {
            QueryComparator comparator              = new QueryComparator();
            QueryComparator prescriberComparator    = new QueryComparator();
            QueryComparator patientComparator       = new QueryComparator();

            prescriberComparator.Source     = new QueryElement( Database.Tables.Prescriptions.PrescriberId );
            prescriberComparator.Operand    = new QueryElement( null , staffId );
            prescriberComparator.Equal      = true;

            patientComparator.Source    = new QueryElement( Database.Tables.Prescriptions.PatientId );
            patientComparator.Operand   = new QueryElement( null , patientId );
            patientComparator.Equal     = true;

            comparator.Source   = prescriberComparator;
            comparator.Operand  = patientComparator;
            comparator.And      = true;
            
            return PrescriptionManager.Merge( Database.Tables.PRESCRIPTIONS , comparator );
        }
        
        public static List<Appointment> GetAppointments( int staffId , int patientId )
        {
            QueryComparator comparator              = new QueryComparator();
            QueryComparator prescriberComparator    = new QueryComparator();
            QueryComparator patientComparator       = new QueryComparator();

            prescriberComparator.Source     = new QueryElement( Database.Tables.Appointments.MedicalStaffId );
            prescriberComparator.Operand    = new QueryElement( null , staffId );
            prescriberComparator.Equal      = true;

            patientComparator.Source    = new QueryElement( Database.Tables.Appointments.PatientId );
            patientComparator.Operand   = new QueryElement( null , patientId );
            patientComparator.Equal     = true;

            comparator.Source   = prescriberComparator;
            comparator.Operand  = patientComparator;
            comparator.And      = true;
            
            return AppointmentManager.Merge( Database.Tables.APPOINTMENTS , comparator );
        }

        // Redirected getters~
        public static List<Patient> GetPatientsByDetails( int id            )   { return PatientManager.Merge( Database.Tables.PATIENTS , ManagerHelper.GetEqualComparator( Database.Tables.Patients.DetailsId      , id                                                                                            ) ); }
        public static List<Patient> GetPatientsByFirstName( string name     )   { return PatientManager.Merge( Database.Tables.PATIENTS , ManagerHelper.GetInLikeComparator( Database.Tables.Patients.DetailsId     , Database.Tables.PERSONAL_DETAILS  , Database.Tables.PersonalDetails.FirstName     , name      ) ); }
        public static List<Patient> GetPatientsByLastName( string name      )   { return PatientManager.Merge( Database.Tables.PATIENTS , ManagerHelper.GetInLikeComparator( Database.Tables.Patients.DetailsId     , Database.Tables.PERSONAL_DETAILS  , Database.Tables.PersonalDetails.LastName      , name      ) ); }
        public static List<Patient> GetPatientsByAddress( string address    )   { return PatientManager.Merge( Database.Tables.PATIENTS , ManagerHelper.GetInLikeComparator( Database.Tables.Patients.DetailsId     , Database.Tables.PERSONAL_DETAILS  , Database.Tables.PersonalDetails.Address       , address   ) ); }
        public static List<Patient> GetPatientsByDateOfBirth( DateTime dob  )   { return PatientManager.Merge( Database.Tables.PATIENTS , ManagerHelper.GetInEqualComparator( Database.Tables.Patients.DetailsId    , Database.Tables.PERSONAL_DETAILS , Database.Tables.PersonalDetails.DateOfBirth    , dob       ) ); }
        public static List<Patient> GetPatientsBySex( char sex              )   { return PatientManager.Merge( Database.Tables.PATIENTS , ManagerHelper.GetInEqualComparator( Database.Tables.Patients.DetailsId    , Database.Tables.PERSONAL_DETAILS , Database.Tables.PersonalDetails.Sex            , sex       ) ); }
        public static List<Patient> GetPatientsByPostcode( int id           )   { return PatientManager.Merge( Database.Tables.PATIENTS , ManagerHelper.GetInEqualComparator( Database.Tables.Patients.DetailsId    , Database.Tables.PERSONAL_DETAILS , Database.Tables.PersonalDetails.PostcodeId     , id        ) ); }
        public static List<Patient> GetPatientsByPostcode( string code      )
        {
            DatabaseQuery nestedQuery = new DatabaseQuery( Database.Tables.PERSONAL_DETAILS )
            {
                Comparator = new QueryComparator()
                {
                    Source  = new QueryElement( Database.Tables.PersonalDetails.PostcodeId ),
                    Operand = ManagerHelper.GetInEqualQuery
                    (
                        Database.Tables.POSTAL_CODES,
                        Database.Tables.PostalCodes.Code,
                        code
                    )
                }
            };

            nestedQuery.Add( Database.Tables.Generic.ID );

            return PatientManager.Merge
            (
                Database.Tables.PATIENTS,
                ManagerHelper.GetInComparator
                (
                    Database.Tables.Patients.DetailsId,
                    nestedQuery
                )
            );
        }
        
        // Other getters~
        public static List<TestResult> GetTestResultsByPatient( int id              )   { return TestResultManager.Merge( Database.Tables.TEST_RESULTS  , ManagerHelper.GetEqualComparator( Database.Tables.TestResults.PatientId           , id                ) ); }
        public static List<TestResult> GetTestResultsByLicenseNo( string license    )   { return TestResultManager.Merge( Database.Tables.TEST_RESULTS  , ManagerHelper.GetEqualComparator( Database.Tables.TestResults.MedicalLicenseNo    , license           ) ); }
        public static List<TestResult> GetTestResultsByName( string name            )   { return TestResultManager.Merge( Database.Tables.TEST_RESULTS  , ManagerHelper.GetLikeComparator( Database.Tables.TestResults.Name                 , name              ) ); }
        public static List<TestResult> GetTestResultsByDescription( string desc     )   { return TestResultManager.Merge( Database.Tables.TEST_RESULTS  , ManagerHelper.GetLikeComparator( Database.Tables.TestResults.Description          , desc              ) ); }
        public static List<TestResult> GetTestResultsByResult( string result        )   { return TestResultManager.Merge( Database.Tables.TEST_RESULTS  , ManagerHelper.GetLikeComparator( Database.Tables.TestResults.Result               , result            ) ); }
        public static List<TestResult> GetTestResultsByRemark( string remark        )   { return TestResultManager.Merge( Database.Tables.TEST_RESULTS  , ManagerHelper.GetLikeComparator( Database.Tables.TestResults.Remark               , remark            ) ); }
        public static List<TestResult> GetTestResultsByStaff( MedicalStaff staff    )   { return TestResultManager.Merge( Database.Tables.TEST_RESULTS  , ManagerHelper.GetEqualComparator( Database.Tables.TestResults.MedicalLicenseNo    , staff.LicenseNo   ) ); }
        public static List<TestResult> GetTestResultsByStaff( int id                )
        {
            DatabaseQuery nestedQuery = new DatabaseQuery( Database.Tables.MEDICAL_STAFFS )
            {
                Comparator = new QueryComparator()
                {
                    Source  = new QueryElement( Database.Tables.MedicalStaffs.StaffId ),
                    Operand = new QueryElement( null , id )
                }
            };

            nestedQuery.Add( Database.Tables.MedicalStaffs.LicenseNo );

            return TestResultManager.Merge
            (
                Database.Tables.TEST_RESULTS,
                ManagerHelper.GetInComparator
                (
                    Database.Tables.TestResults.MedicalLicenseNo,
                    nestedQuery
                )
            );
        }

        public static List<Prescription> GetPrescriptionsByPatient( int id          )   { return PrescriptionManager.Merge( Database.Tables.PRESCRIPTIONS  , ManagerHelper.GetEqualComparator( Database.Tables.Prescriptions.PatientId      , id                            ) ); }
        public static List<Prescription> GetPrescriptionsByStaff( int id            )   { return PrescriptionManager.Merge( Database.Tables.PRESCRIPTIONS  , ManagerHelper.GetEqualComparator( Database.Tables.Prescriptions.PrescriberId   , id                            ) ); }
        public static List<Prescription> GetPrescriptionsByName( string name        )   { return PrescriptionManager.Merge( Database.Tables.PRESCRIPTIONS  , ManagerHelper.GetEqualComparator( Database.Tables.Prescriptions.Name           , name                          ) ); }
        public static List<Prescription> GetPrescriptionsByRemark( string remark    )   { return PrescriptionManager.Merge( Database.Tables.PRESCRIPTIONS  , ManagerHelper.GetEqualComparator( Database.Tables.Prescriptions.Remark         , remark                        ) ); }
        public static List<Prescription> GetPrescriptionsByStartDate( DateTime date )   { return PrescriptionManager.Merge( Database.Tables.PRESCRIPTIONS  , ManagerHelper.GetEqualComparator( Database.Tables.Prescriptions.StartDate      , QueryElement.DateOf( date )   ) ); }
        public static List<Prescription> GetPrescriptionsByEndDate( DateTime date   )   { return PrescriptionManager.Merge( Database.Tables.PRESCRIPTIONS  , ManagerHelper.GetEqualComparator( Database.Tables.Prescriptions.EndDate        , QueryElement.DateOf( date )   ) ); }
        public static List<Prescription> GetPrescriptionsByMedication( int id       )
        {
            return PrescriptionManager.Merge
            (
                Database.Tables.PRESCRIPTIONS,
                ManagerHelper.GetInComparator
                (
                    Database.Tables.Generic.ID,
                    ManagerHelper.GetInEqualQuery
                    (
                        Database.Tables.PRESCRIPTION_MEDICATIONS,
                        Database.Tables.PrescriptionMedications.MedicationId,
                        id
                    )
                )
            );
        }

        public static List<Prescription> GetPrescriptionsByMedicationCode( string code )
        {
            DatabaseQuery nestedQuery = new DatabaseQuery( Database.Tables.PRESCRIPTION_MEDICATIONS )
            {
                Comparator = ManagerHelper.GetInLikeComparator
                (
                    Database.Tables.PrescriptionMedications.MedicationId,
                    Database.Tables.MEDICATIONS,
                    Database.Tables.Medications.Code,
                    code
                )
            };

            nestedQuery.Add( Database.Tables.PrescriptionMedications.PrescriptionId );

            return PrescriptionManager.Merge
            (
                Database.Tables.PRESCRIPTIONS,
                ManagerHelper.GetInComparator
                (
                    Database.Tables.Generic.ID,
                    nestedQuery
                )
            );
        }

        public static List<Prescription> GetPrescriptionsByMedicationName( string name )
        {
            DatabaseQuery nestedQuery = new DatabaseQuery( Database.Tables.PRESCRIPTION_MEDICATIONS )
            {
                Comparator = ManagerHelper.GetInLikeComparator
                (
                    Database.Tables.PrescriptionMedications.MedicationId,
                    Database.Tables.MEDICATIONS,
                    Database.Tables.Medications.Name,
                    name
                )
            };

            nestedQuery.Add( Database.Tables.PrescriptionMedications.PrescriptionId );

            return PrescriptionManager.Merge
            (
                Database.Tables.PRESCRIPTIONS,
                ManagerHelper.GetInComparator
                (
                    Database.Tables.Generic.ID,
                    nestedQuery
                )
            );
        }

        public static List<Appointment> GetAppointmentsByPatient( int id            )   { return AppointmentManager.Merge( Database.Tables.APPOINTMENTS  , ManagerHelper.GetEqualComparator( Database.Tables.Appointments.PatientId         , id                ) ); }
        public static List<Appointment> GetAppointmentsByStaff( int id              )   { return AppointmentManager.Merge( Database.Tables.APPOINTMENTS  , ManagerHelper.GetEqualComparator( Database.Tables.Appointments.MedicalStaffId    , id                ) ); }
        public static List<Appointment> GetAppointmentsByStatus( bool cancelled     )   { return AppointmentManager.Merge( Database.Tables.APPOINTMENTS  , ManagerHelper.GetEqualComparator( Database.Tables.Appointments.Cancelled         , cancelled ? 1 : 0 ) ); }
        public static List<Appointment> GetAppointmentsByDateAndTime( DateTime dt   )   { return AppointmentManager.Merge( Database.Tables.APPOINTMENTS  , ManagerHelper.GetEqualComparator( Database.Tables.Appointments.DateAppointed     , dt                ) ); }

        public static List<Medication> GetMedicationByCode( string code )   { return MedicationManager.Merge( Database.Tables.MEDICATIONS , ManagerHelper.GetEqualComparator( Database.Tables.Medications.Code , code ) ); }
        public static List<Medication> GetMedicationByName( string name )   { return MedicationManager.Merge( Database.Tables.MEDICATIONS , ManagerHelper.GetEqualComparator( Database.Tables.Medications.Name , name ) ); }
        
        public static List<PrescriptionMedication> GetMedicationsForPrescription( int id )  { return PrescriptionMedManager.Merge( Database.Tables.PRESCRIPTION_MEDICATIONS , ManagerHelper.GetEqualComparator( Database.Tables.PrescriptionMedications.PrescriptionId , id ) ); }
        
        public static List<Patient>         GetAllPatients()        { return PatientManager.Merge(      Database.Tables.PATIENTS        , null ); }
        public static List<TestResult>      GetAllTestResults()     { return TestResultManager.Merge(   Database.Tables.TEST_RESULTS    , null ); }
        public static List<Appointment>     GetAllAppointments()    { return AppointmentManager.Merge(  Database.Tables.APPOINTMENTS    , null ); }
        public static List<Prescription>    GetAllPrescriptions()   { return PrescriptionManager.Merge(  Database.Tables.PRESCRIPTIONS  , null ); }
        
        // Simple redirect getters...
        public static List<Patient>                 GetPatientsByDetails( PersonalDetails detail                )   { return GetPatientsByDetails( detail.Id                ); }
        public static List<Prescription>            GetPrescriptionsByPatient( Patient patient                  )   { return GetPrescriptionsByPatient( patient.Id          ); }
        public static List<Prescription>            GetPrescriptionsByStaff( Staff staff                        )   { return GetPrescriptionsByStaff( staff.Id              ); }
        public static List<Appointment>             GetAppointmentsByPatient(  Patient patient                  )   { return GetAppointmentsByPatient( patient.Id           ); }
        public static List<Appointment>             GetAppointmentsByStaff( Staff staff                         )   { return GetAppointmentsByStaff( staff.Id               ); }
        public static List<TestResult>              GetTestResultsByPatient( Patient patient                    )   { return GetTestResultsByPatient( patient.Id            ); }
        public static List<Prescription>            GetPrescriptions( MedicalStaff staff , Patient patient      )   { return GetPrescriptions( staff.Id , patient.Id        ); }
        public static List<Appointment>             GetAppointments( MedicalStaff staff , Patient patient       )   { return GetAppointments( staff.Id , patient.Id         ); }
        public static List<PrescriptionMedication>  GetMedicationsForPrescription( Prescription prescription    )   { return GetMedicationsForPrescription( prescription.Id ); }
    }
}
