using System.ServiceModel;
using WsDualHttpBinding_SqlTableDependency_PoC.WCF.Entities;

namespace WsDualHttpBinding_SqlTableDependency_PoC.WCF.Contracts
{
    public interface IPatientChangedCallback
    {
        [OperationContract]
        void PatientChange(Patient changedPatient);

        [OperationContract]
        void NotificationReceived(string message);
    }
}
