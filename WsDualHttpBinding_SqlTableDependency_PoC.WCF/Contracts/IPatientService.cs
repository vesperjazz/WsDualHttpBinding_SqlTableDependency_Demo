using System.Collections.Generic;
using System.ServiceModel;
using WsDualHttpBinding_SqlTableDependency_PoC.WCF.Entities;

namespace WsDualHttpBinding_SqlTableDependency_PoC.WCF.Contracts
{
    [ServiceContract(CallbackContract = typeof(IPatientChangedCallback))]
    public interface IPatientService
    {
        [OperationContract]
        void Subscribe();

        [OperationContract]
        void Unsubscribe();

        [OperationContract]
        IEnumerable<Patient> GetAllPatients();

        [OperationContract]
        void PublishPatientChange(Patient changedPatient);

        [OperationContract]
        void NotifyOthers(string message);
    }
}
