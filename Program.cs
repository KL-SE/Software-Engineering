using System;
using System.Collections.Generic;

namespace Assignment2
{
    class Staff
    {
        private int id;
        private string name;
        private string password;
        private DateTime datejoined;
        private bool active;

        public Staff() { }

        public int getId()              { return id;            }
        protected string getPassword()  { return password;      }
        public string getName()         { return name;          }
        public DateTime getDateJoined() { return datejoined;    }

        protected void setId( int id )                      { this.id = id;                 }
        protected void setName( string name )               { this.name = name;             }
        protected void setDateJoined( DateTime datejoined ) { this.datejoined = datejoined; }
        protected void setPassword( string password )       { this.password = password;     }

        public bool isActive()  { return active;    }
        public void suspend()   { active = false;   }
        public void reinstate() { active = true;    }
        
        public bool isPasswordCorrect( string password )                                    { return this.password == password; }
        public bool registerPatient( string name , string address , DateTime dateOfBirth )  { return true;                      }
    }

    class MedicalStaff : Staff
    {
        private List<bool> workingDays;     // 7 Element Array
        private List<DateTime> leaveDates;

        public static List<DateTime> getAppointments()          { return null;  }
        public bool isWorkingOn()                               { return true;  }
        public bool isOnLeave( DateTime date )                  { return false; }
        public void setWorkingOnDay( int day , bool isWorking ) {               }
        public void setLeaveOnDate( DateTime date )             {               }
    }

    class Receptionist : Staff
    {
        private bool admin;

        public bool isAdmin()                                           { return admin; }
        public void giveAdmin()                                         {               }
        public void removeAdmin()                                       {               }
        public bool registerStaff( string name , bool isMedicalStaff )  { return true;  }
    }

    class Patient
    {
        private int id;
        private string name;
        private string address;
        private DateTime dateOfBirth;

        public Patient() { }

        public static List<Prescription> getPrescriptions() { return null; }
        public static List<DateTime> getAppointments()      { return null; }
        public static List<TestResult> getTests()           { return null; }

        public int getId()                  { return id;            }
        public string getName()             { return name;          }
        public string getAddress()          { return address;       }
        public DateTime getDateOfBirth()    { return dateOfBirth;   }

        public void setName( string name )          { this.name = name;         }
        public void setAddress( string address )    { this.address = address;   }
        public void setDateOfBirth( DateTime date ) { this.dateOfBirth = date;  }
    }

    class Appointment
    {
        private int appointmentID;
        private int medicalStaffID;
        private int patientID;
        //private DateTime appointmentDate;
        private DateTime appointmentTime;
        private bool cancelled;

        public static Appointment makeAppointment( MedicalStaff staff , Patient patient ) { return null; }

        public int getId()                      { return appointmentID;     }
        public MedicalStaff getMedicalStaff()   { return null;              }
        public Patient getPatient()             { return null;              }
        //public DateTime getAppointmentDate()    { return appointmentDate;   }
        
        //public void setAppointmentDate( DateTime date ) { this.appointmentDate = date;  }
        public void setAppointmentTime( DateTime time ) { this.appointmentTime = time;  }
        public bool isCancelled()                       { return cancelled;             }
        public void cancel()                            { cancelled = true;             }
    }

    class TestResult
    {
        private int testID;
        private int patientID;
        private string testDescription;
        private string testResult;
        private string remark;

        public TestResult( Patient patient , string desc , string result , string remark ) { }

        public int getTestId()          { return testID;            }
        public Patient getPatient()     { return null;              }
        public string getDescription()  { return testDescription;   }
        public string getResult()       { return testResult;        }
        public string getRemark()       { return remark;            }
    }

    class Prescription
    {
        private int prescriptionID;
        private int prescriberID;
        private int patientID;
        private DateTime startDate;
        private DateTime endDate;
        private List<Medication> medications;
        private string remark;

        public static Prescription makePrescription( MedicalStaff staff , Patient patient ) { return null; }

        public int getId()                          { return prescriptionID;    }
        public MedicalStaff getPrescriber()         { return null;              }
        public Patient getPatient()                 { return null;              }
        public DateTime getStartDate()              { return startDate;         }
        public DateTime getEndDate()                { return endDate;           }
        public List<Medication> getMedications()    { return medications;       }
        public string getRemark()                   { return remark;            }

        public void extendPrescription( int duration )  {                       }
        public void endPrescription()                   {                       }
        public void addMedication( string code )        {                       }
        public void removeMedication( string code )     {                       }
        public void setRemark( string remark )          { this.remark = remark; }
    }

    class Medication
    {
        private string code;
        private string name;

        public Medication( string code , string name )  { this.code = code; this.name = name;   }
        public string getCode()                         { return code;                          }
        public string getName()                         { return name;                          }
    }
}