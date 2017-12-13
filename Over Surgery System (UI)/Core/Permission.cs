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
        public static bool HaveReceptionistPrivilege    { get => App.LoggedInStaff is Receptionist;             }
        public static bool HaveMedicalPrivilege         { get => App.LoggedInStaff is MedicalStaff;             }
        public static bool HaveAdminPrivilege           { get => Receptionist.IsAdmin(  App.LoggedInStaff );    }
        public static bool HaveGPPrivilege              { get => MedicalStaff.IsGP(     App.LoggedInStaff );    }
        public static bool HaveNursePrivilege           { get => MedicalStaff.IsNurse(  App.LoggedInStaff );    }
        
        public static bool CanAddStaffs                 { get => HaveAdminPrivilege;                                }
        public static bool CanEditOtherStaffs           { get => HaveAdminPrivilege;                                }
        public static bool CanEditTestResults           { get => HaveGPPrivilege;                                   }
        public static bool CanAddTestResults            { get => HaveGPPrivilege;                                   }
        public static bool CanPrescribePatients         { get => HaveGPPrivilege;                                   }
        public static bool CanEditLeaveDates            { get => HaveAdminPrivilege || HaveReceptionistPrivilege;   }
        public static bool CanAppointOtherStaffs        { get => HaveAdminPrivilege || HaveReceptionistPrivilege;   }
        public static bool CanEditOtherStaffWorkingDays { get => HaveAdminPrivilege || HaveReceptionistPrivilege;   }
    }
}
