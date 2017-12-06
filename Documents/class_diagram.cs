using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace classDiagram
{
    class Staff
    {


        private int id;
        private string name;

        private DateJoin(string date);
        private Active(Boolean);



        public int GetId() { return id; }
        static string getPassword() { }
        public string getName() { return name; }
        private getDateJoined(date);
        public Boolean isActive();
        public suspend();
        public reinstate();
        static void (int setId);
                                                static void (string setName);
                                                static setDateJoinedDate(date);

    }

    class MedicalStaff
    {
        private [] workingdays;
        private [] leaveDate;

        public [] getAppointment;
        public boolean(string isPasswordCorrect);
        public MedicalStaff.boolean(int isWorkingOn);
                                        public void (int boolean setWorkingOnDay);
        public void (setLeaveOnDate);
                                    }



    class Prescription
    {
        private int prescribeID;
        private int patientID;
        private starDate(date);
        private endDate(date);
        private [] medications;

        public makePrescription(int MedicalStaff, int Patient, Prescription);
        public int getId() { return prescribeID; }
        public getPrescriber(MedicalStaff);
        public getPatient(Patient);
        public getStartDate(string date) { return starDate; }
        public getEndDate(date);
        public getMedications[];
                                    public string[] getRemark;
        public boolean isPrescriptionAtive();
        public void extendPrescription(int);
        public void endPrescription(string);
        public void removeMedication(string);
        public void seRremark(string);

    }



    class Medication
    {
        private string code;
        private string name;


        string getcode() { return code; }
        public string getName() { return name; }

    }




    class Patient
    {
        private int id;
        private string name;
        private string address;
        private dateOfBirth(int date);

        public [] getPrescriptions();
        public [] getAppointments();
        public [] getTests();
        public int getID() { return id; }
        public string getName() { return name; }
        public string getAddress() { return address; }
        public string getDateOfBirth(string date) { return date; }
        public void setName(string);
        public void setAddress(string);
        public void setDateOfBirth(string date);

    }



    class Appointment {
        private int appointmentID;
        private int medicalStaffID;
        private int patientID;
        private string appointmentDate(date);
        private string appointmentTime(time);
        private boolean cancelled;
        public makeAppointment(MedicalStaff, Patient, Appointment) { }
        //public int getId() { return appointmentID; }
        public string getMedicalStaff(MedicalStaff) { return medicalStaffID; }
        public string getPatient(Patient);
        public string getAppointmentDate(date);
        public string getAppointmentTime(time);
        public void setAppointmentDate(date);
        public void setAppointmentTime(time);
        public Boolean isCancelled();
        public Boolean cancel();

    }



    class TestResult {
        private int testID;
        private int patientID;
        private string testDescription;
        private string testResult;
        private string remark;

        public int getTestId() { return patientID; }
        public string getPatient(Patient);
        public string getDescription() { return testDescription; }
        public string getResult();{ return testResult; }
    public string getRemark() { return remark; }

                    }   


    class receptionist {

        private Boolean admin;

        public Boolean isAdmin() { return admin; }
        public void giveAdmin() { }
        public void removeAdmin() { }
        public Boolean registerStaff(string, Boolean) { return; }
        public Boolean registerPatient(string, date) { return; }






    }


}



