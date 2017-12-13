using OverSurgerySystem.Core.Staffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverSurgerySystem.UI.Core
{
    public abstract class Permission
    {
        public static bool HaveReceptionistPrivilege    { get { return App.LoggedInStaff is Receptionist;           } }
        public static bool HaveMedicalPrivilege         { get { return App.LoggedInStaff is MedicalStaff;           } }
        public static bool HaveAdminPrivilege           { get { return Receptionist.IsAdmin(  App.LoggedInStaff );  } }
        public static bool HaveGPPrivilege              { get { return MedicalStaff.IsGP(     App.LoggedInStaff );  } }
        public static bool HaveNursePrivilege           { get { return MedicalStaff.IsNurse(  App.LoggedInStaff );  } }
        
        public static bool CanAddStaffs                 { get { return HaveAdminPrivilege;                              } }
        public static bool CanEditOtherStaffs           { get { return HaveAdminPrivilege;                              } }
        public static bool CanEditTestResults           { get { return HaveAdminPrivilege;                              } }
        public static bool CanAddTestResults            { get { return HaveGPPrivilege;                                 } }
        public static bool CanAddPrescription           { get { return HaveGPPrivilege;                                 } }
        public static bool CanEditPrescription          { get { return HaveAdminPrivilege || HaveGPPrivilege;           } }
        public static bool CanEditLeaveDates            { get { return HaveAdminPrivilege || HaveReceptionistPrivilege; } }
        public static bool CanAppointOtherStaffs        { get { return HaveAdminPrivilege || HaveReceptionistPrivilege; } }
        public static bool CanEditStaffWorkingDays      { get { return HaveAdminPrivilege || HaveReceptionistPrivilege; } }
    }
}
