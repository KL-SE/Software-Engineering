using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Staffs
{
    public class WorkingDays : DatabaseObject
    {
        // Constants, holding the index of each day of a week for the "days" array below.
        public static class Day
        {
            public const int SUNDAY     = 0;
            public const int MONDAY     = 1;
            public const int TUESDAY    = 2;
            public const int WEDNESDAY  = 3;
            public const int THURSDAY   = 4;
            public const int FRIDAY     = 5;
            public const int SATURDAY   = 6;
            public const int TOTAL      = 7;
        };
        
        public MedicalStaff Owner { private set; get; } // Owner of this object
        bool[] days;                                    // Array that stores whether staff is working on a day of the week.

        public WorkingDays() : base()
        {
            days = new bool[Day.TOTAL];
        }

        public WorkingDays( MedicalStaff owner )
        {
            days = new bool[Day.TOTAL];
            Owner = owner;
        }

        // Properties to access specific days of a week.
        public bool Sunday
        {
            set { days[Day.SUNDAY] = value;     }
            get { return days[Day.SUNDAY];      }
        }

        public bool Monday
        {
            set { days[Day.MONDAY] = value;     }
            get { return days[Day.MONDAY];      }
        }

        public bool Tuesday
        {
            set { days[Day.TUESDAY] = value;    }
            get { return days[Day.TUESDAY];     }
        }

        public bool Wednesday
        {
            set { days[Day.WEDNESDAY] = value;  }
            get { return days[Day.WEDNESDAY];   }
        }

        public bool Thursday
        {
            set { days[Day.THURSDAY] = value;   }
            get { return days[Day.THURSDAY];    }
        }

        public bool Friday
        {
            set { days[Day.FRIDAY] = value;     }
            get { return days[Day.FRIDAY];      }
        }

        public bool Saturday
        {
            set { days[Day.SATURDAY] = value;   }
            get { return days[Day.SATURDAY];    }
        }
        
        // Properties to access whether the staff is working on weekdays or weekends.
        public bool Weekdays
        {
            set
            {
                for( int i = Day.MONDAY ; i <= Day.FRIDAY ; i++ )
                {
                    days[i] = value;
                }
            }
            get
            {
                bool returnValue = true;
                for( int i = Day.MONDAY ; i <= Day.FRIDAY ; i++ )
                {
                    returnValue = returnValue && days[i];
                }

                return returnValue;
            }
        }

        public bool Weekends
        {
            set
            {
                 days[Day.SUNDAY  ] = value;
                 days[Day.SATURDAY] = value;
            }
            get
            {
                return days[Day.SUNDAY] && days[Day.SATURDAY];
            }
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.WORKING_DAYS );
            DoDelete( query );
            StaffsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.WORKING_DAYS );
            query.Add( Database.Tables.WorkingDays.StaffId      );
            query.Add( Database.Tables.WorkingDays.Sunday       );
            query.Add( Database.Tables.WorkingDays.Monday       );
            query.Add( Database.Tables.WorkingDays.Tuesday      );
            query.Add( Database.Tables.WorkingDays.Wednesday    );
            query.Add( Database.Tables.WorkingDays.Thursday     );
            query.Add( Database.Tables.WorkingDays.Friday       );
            query.Add( Database.Tables.WorkingDays.Saturday     );
            
            MySqlDataReader reader  = DoLoad( query );
            int ownerId             = INVALID_ID;
            
            if( Loaded )
            {
                ownerId     = reader.GetInt32( 0 );
                Sunday      = reader.GetByte( 1 ) > 0 ? true : false;
                Monday      = reader.GetByte( 2 ) > 0 ? true : false;
                Tuesday     = reader.GetByte( 3 ) > 0 ? true : false;
                Wednesday   = reader.GetByte( 4 ) > 0 ? true : false;
                Thursday    = reader.GetByte( 5 ) > 0 ? true : false;
                Friday      = reader.GetByte( 6 ) > 0 ? true : false;
                Saturday    = reader.GetByte( 7 ) > 0 ? true : false;
                StaffsManager.Add( this );
            }

            reader.Close();
            Owner = StaffsManager.GetMedicalStaff( ownerId );
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.WORKING_DAYS );
            query.Add( Database.Tables.WorkingDays.StaffId      , Owner             );
            query.Add( Database.Tables.WorkingDays.Sunday       , Sunday    ? 1 : 0 );
            query.Add( Database.Tables.WorkingDays.Monday       , Monday    ? 1 : 0 );
            query.Add( Database.Tables.WorkingDays.Tuesday      , Tuesday   ? 1 : 0 );
            query.Add( Database.Tables.WorkingDays.Wednesday    , Wednesday ? 1 : 0 );
            query.Add( Database.Tables.WorkingDays.Thursday     , Thursday  ? 1 : 0 );
            query.Add( Database.Tables.WorkingDays.Friday       , Friday    ? 1 : 0 );
            query.Add( Database.Tables.WorkingDays.Saturday     , Saturday  ? 1 : 0 );
            DoSave( query );
            StaffsManager.Add( this );
        }
    }
}
