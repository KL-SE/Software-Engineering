using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Core.Base;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace OverSurgerySystem.Manager
{
    public class StaffsManager
    {
        static StaffManagerBase<Staff>  StaffManager;
        static Manager<LeaveDate>       LeaveDateManager;
        static Manager<WorkingDays>     WorkingDaysManager;
        
        private StaffsManager() { }
        static StaffsManager()
        {
            StaffManager        = new StaffManagerBase<Staff>();
            LeaveDateManager    = new Manager<LeaveDate>();
            WorkingDaysManager  = new Manager<WorkingDays>();
        }

        public static void Add( Staff obj       )   { StaffManager.Add( obj );          }
        public static void Add( LeaveDate obj   )   { LeaveDateManager.Add( obj );      }
        public static void Add( WorkingDays obj )   { WorkingDaysManager.Add( obj );    }

        public static void Remove( Staff obj        )   { StaffManager.Remove( obj );       }
        public static void Remove( LeaveDate obj    )   { LeaveDateManager.Remove( obj );   }
        public static void Remove( WorkingDays obj  )   { WorkingDaysManager.Remove( obj ); }
        
        public static Staff GetStaff( int id                )   { return StaffManager.Get( id );        }
        public static LeaveDate GetLeaveDate( int id        )   { return LeaveDateManager.Get( id );    }
        public static WorkingDays GetWorkingDays( int id    )   { return WorkingDaysManager.Get( id );  }
        public static MedicalStaff GetMedicalStaff( int id  )
        {
            Staff staff = GetStaff( id );
            if( staff is MedicalStaff )
                return ( MedicalStaff )( staff );

            return null;
        }

        public static Receptionist GetReceptionist( int id )
        {
            Staff staff = GetStaff( id );
            if( staff is Receptionist )
                return ( Receptionist )( staff );

            return null;
        }

        // Redirected getters~
        public static List<Staff> GetStaffsByDetails( int id            )   { return StaffManager.Merge( Database.Tables.STAFFS , ManagerHelper.GetEqualComparator( Database.Tables.Staffs.DetailsId    , id                                                                                            ) ); }
        public static List<Staff> GetStaffsByFirstName( string name     )   { return StaffManager.Merge( Database.Tables.STAFFS , ManagerHelper.GetInLikeComparator( Database.Tables.Staffs.DetailsId   , Database.Tables.PERSONAL_DETAILS  , Database.Tables.PersonalDetails.FirstName     , name      ) ); }
        public static List<Staff> GetStaffsByLastName( string name      )   { return StaffManager.Merge( Database.Tables.STAFFS , ManagerHelper.GetInLikeComparator( Database.Tables.Staffs.DetailsId   , Database.Tables.PERSONAL_DETAILS  , Database.Tables.PersonalDetails.LastName      , name      ) ); }
        public static List<Staff> GetStaffsByAddress( string address    )   { return StaffManager.Merge( Database.Tables.STAFFS , ManagerHelper.GetInLikeComparator( Database.Tables.Staffs.DetailsId   , Database.Tables.PERSONAL_DETAILS  , Database.Tables.PersonalDetails.Address       , address   ) ); }
        public static List<Staff> GetStaffsByDateOfBirth( DateTime dob  )   { return StaffManager.Merge( Database.Tables.STAFFS , ManagerHelper.GetInEqualComparator( Database.Tables.Staffs.DetailsId  , Database.Tables.PERSONAL_DETAILS  , Database.Tables.PersonalDetails.DateOfBirth   , dob       ) ); }
        public static List<Staff> GetStaffsByPostcode( int id           )   { return StaffManager.Merge( Database.Tables.STAFFS , ManagerHelper.GetInEqualComparator( Database.Tables.Staffs.DetailsId  , Database.Tables.PERSONAL_DETAILS  , Database.Tables.PersonalDetails.PostcodeId    , id        ) ); }
        public static List<Staff> GetStaffsByPostcode( string code      )
        {
            DatabaseQuery nestedQuery = new DatabaseQuery( Database.Tables.PERSONAL_DETAILS )
            {
                Comparator = new QueryComparator()
                {
                    Source  = new QueryElement( Database.Tables.PersonalDetails.PostcodeId ),
                    Operand = new QueryElement
                    (
                        ManagerHelper.GetInEqualQuery
                        (
                            Database.Tables.POSTAL_CODES,
                            Database.Tables.PostalCodes.Code,
                            code
                        )
                    )
                }
            };

            nestedQuery.Add( Database.Tables.Generic.ID );

            return StaffManager.Merge
            (
                Database.Tables.STAFFS,
                ManagerHelper.GetInComparator
                (
                    Database.Tables.Staffs.DetailsId,
                    nestedQuery
                )
            );
        }
        
        // Simple Getters
        public static List<Staff>       GetActiveStaffs()               { return StaffManager.Merge( Database.Tables.STAFFS             , ManagerHelper.GetEqualComparator( Database.Tables.Staffs.Active       , 1     ) ); }
        public static List<Staff>       GetInactiveStaffs()             { return StaffManager.Merge( Database.Tables.STAFFS             , ManagerHelper.GetEqualComparator( Database.Tables.Staffs.Active       , 0     ) ); }
        public static List<LeaveDate>   GetLeaveDatesByStaff( int id )  { return LeaveDateManager.Merge( Database.Tables.LEAVE_DATES    , ManagerHelper.GetEqualComparator( Database.Tables.LeaveDates.StaffId  , id    ) ); }
        public static List<WorkingDays> GetWorkingDaysByStaff( int id ) { return WorkingDaysManager.Merge( Database.Tables.WORKING_DAYS , ManagerHelper.GetEqualComparator( Database.Tables.WorkingDays.StaffId , id    ) ); }
        
        // Getting admin status through the staff ID.
        public static List<Receptionist> GetReceptionistsWithAdminStatus( bool admin )
        {
            return ManagerHelper.FilterType<Receptionist>
            ( 
                StaffManager.Merge
                (
                    Database.Tables.STAFFS,
                    ManagerHelper.GetInEqualComparator
                    (
                        Database.Tables.Receptionists.StaffId,
                        Database.Tables.RECEPTIONISTS,
                        Database.Tables.Receptionists.Admin,
                        admin ? 1 : 0
                    )
                )
            );
        }
        
        // Getting medical license number through the staff ID.
        public static List<MedicalStaff> GetMedicalStaffWithLicenseNo( string number )
        {
            return ManagerHelper.FilterType<MedicalStaff>
            ( 
                StaffManager.Merge
                (
                    Database.Tables.STAFFS,
                    ManagerHelper.GetInEqualComparator
                    (
                        Database.Tables.MedicalStaffs.StaffId,
                        Database.Tables.MEDICAL_STAFFS,
                        Database.Tables.MedicalStaffs.LicenseNo,
                        number
                    )
                )
            );
        }
        
        // Getting leave dates within a set range through the staff ID.
        public static List<MedicalStaff> GetMedicalStaffLeavesFrom( DateTime start , DateTime end , bool not )
        {
            DatabaseQuery nestedQuery = new DatabaseQuery( Database.Tables.LEAVE_DATES );

            if( start.Date.Equals( end.Date ) )
            {
                nestedQuery.Comparator = new QueryComparator
                {
                    Source  = new QueryElement( Database.Tables.LeaveDates.Date     ),
                    Operand = new QueryElement( null , QueryElement.DateOf( start ) )
                };
            }
            else
            {
                nestedQuery.Comparator = new QueryComparator
                {
                    Source  = new QueryComparator()
                    {
                        Source  = new QueryElement( Database.Tables.LeaveDates.Date     ),
                        Operand = new QueryElement( null , QueryElement.DateOf( start ) ),
                        Equal   = !not,
                        More    = !not,
                        Less    = not
                    },

                    Operand = new QueryComparator()
                    {
                        Source  = new QueryElement( Database.Tables.LeaveDates.Date     ),
                        Operand = new QueryElement( null , QueryElement.DateOf( end )   ),
                        Equal   = !not,
                        Less    = !not,
                        More    = not
                    },

                    And = !not,
                    Or  = not
                };
            }

            nestedQuery.Add( Database.Tables.LeaveDates.StaffId );

            return ManagerHelper.FilterType<MedicalStaff>
            ( 
                StaffManager.Merge
                (
                    Database.Tables.STAFFS,
                    ManagerHelper.GetInComparator
                    (
                        Database.Tables.Generic.ID,
                        nestedQuery
                    )
                )
            );
        }
        
        // Getting working days through the staff ID.
        public static List<MedicalStaff> GetMedicalStaffsWorkingOn( int day , bool not )
        {
            string[] translationTable =
            {
                Database.Tables.WorkingDays.Sunday,
                Database.Tables.WorkingDays.Monday,
                Database.Tables.WorkingDays.Tuesday,
                Database.Tables.WorkingDays.Wednesday,
                Database.Tables.WorkingDays.Thursday,
                Database.Tables.WorkingDays.Friday,
                Database.Tables.WorkingDays.Saturday
            };

            return ManagerHelper.FilterType<MedicalStaff>
            ( 
                StaffManager.Merge
                (
                    Database.Tables.STAFFS,
                    ManagerHelper.GetInEqualComparator
                    (
                        Database.Tables.Generic.ID,
                        Database.Tables.WORKING_DAYS,
                        translationTable[day],
                        !not ? 1 : 0
                    )
                )
            );
        }

        // Simple redirect getters...
        public static List<Staff>           GetStaffsByDetails( PersonalDetails details )                   { return GetStaffsByDetails( details.Id );                  }
        public static List<Receptionist>    GetAdminReceptionists()                                         { return GetReceptionistsWithAdminStatus( true );           }
        public static List<Receptionist>    GetNonAdminReceptionists()                                      { return GetReceptionistsWithAdminStatus( false );          }
        public static List<MedicalStaff>    GetMedicalStaffsWorkingOn( int day )                            { return GetMedicalStaffsWorkingOn( day , false );          }
        public static List<MedicalStaff>    GetMedicalStaffsNotWorkingOn( int day )                         { return GetMedicalStaffsWorkingOn( day , true );           }
        public static List<MedicalStaff>    GetMedicalStaffOnLeaveFrom( DateTime start , DateTime end )     { return GetMedicalStaffLeavesFrom( start , end , false );  }
        public static List<MedicalStaff>    GetMedicalStaffNotOnLeaveFrom( DateTime start , DateTime end )  { return GetMedicalStaffLeavesFrom( start , end , true  );  }
        public static List<MedicalStaff>    GetMedicalStaffOnLeaveOn( DateTime date )                       { return GetMedicalStaffLeavesFrom( date , date , false );  }
        public static List<MedicalStaff>    GetMedicalStaffNotOnLeaveOn( DateTime date )                    { return GetMedicalStaffLeavesFrom( date , date , true  );  }
        public static List<LeaveDate>       GetLeaveDatesByStaff( MedicalStaff staff )                      { return GetLeaveDatesByStaff( staff.Id );                  }
        public static List<WorkingDays>     GetWorkingDaysByStaff( MedicalStaff staff )                     { return GetWorkingDaysByStaff( staff.Id );                 }
    }

    [Serializable]
    public class UnknownStaffTypeError: Exception
    {
        public UnknownStaffTypeError() {}
        public UnknownStaffTypeError( string message ) : base( message ) { }
        public UnknownStaffTypeError( string message , Exception inner ) : base( message , inner ) { }
    }

    public class StaffManagerBase<T> : Manager<T> where T: Staff, new()
    {
        // Custom load method for staffs because there are 2 types of staffs.
        public override T Load( int id )
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.STAFFS );
            query.Comparator    = new QueryComparator()
            {
                Source  = new QueryElement( Database.Tables.Generic.ID  ),
                Operand = new QueryElement( null , id                   ),
                Equal   = true
            };

            query.Add( Database.Tables.Staffs.Type );
            
            MySqlCommand command    = new MySqlCommand( query.Select , Database.Connection );
            MySqlDataReader reader  = command.ExecuteReader();

            if( reader.Read() )
            {
                Staff obj;
                char type = reader.GetChar( 0 );

                if( type.ToString().ToUpper().Equals( "M" ) )
                    obj = new MedicalStaff();

                else if( type.ToString().ToUpper().Equals( "R" ) )
                    obj = new Receptionist();

                else
                    throw new UnknownStaffTypeError();

                obj.Load( id );
                if( obj.Loaded )
                    Add( ( T )( obj ) );
            }

            return null;
        }
    }
}
