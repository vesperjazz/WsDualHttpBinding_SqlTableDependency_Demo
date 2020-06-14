using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using WsDualHttpBinding_SqlTableDependency_PoC.WCF.Contracts;
using WsDualHttpBinding_SqlTableDependency_PoC.WCF.Entities;

namespace WsDualHttpBinding_SqlTableDependency_PoC.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PatientService : IPatientService, IDisposable
    {
        private readonly string _connectionString;
        private readonly SqlTableDependency<Patient> _sqlTableDependency;
        private readonly List<IPatientChangedCallback> _callbackList;
        private IPatientChangedCallback CurrentUser => OperationContext.Current.GetCallbackChannel<IPatientChangedCallback>();

        public PatientService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString;
            _sqlTableDependency = new SqlTableDependency<Patient>(_connectionString, nameof(Patient));
            _callbackList = new List<IPatientChangedCallback>();

            _sqlTableDependency.OnChanged += TableDependency_Changed;
            _sqlTableDependency.OnError += (sender, args) => Console.WriteLine($"Error: {args.Message}");
            _sqlTableDependency.Start();
        }

        public void Dispose()
        {
            _sqlTableDependency.Stop();
            _sqlTableDependency.Dispose();
            _callbackList.Clear();
        }

        public void Subscribe()
        {
            var registeredUser = OperationContext.Current.GetCallbackChannel<IPatientChangedCallback>();
            if (!_callbackList.Contains(registeredUser))
            {
                _callbackList.Add(registeredUser);
            }
        }

        public void Unsubscribe()
        {
            var registeredUser = OperationContext.Current.GetCallbackChannel<IPatientChangedCallback>();
            if (_callbackList.Contains(registeredUser))
            {
                _callbackList.Remove(registeredUser);
            }
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            using (var context = new DatabaseContext(_connectionString))
            {
                return context.Patients.ToList();
            }
        }

        public void PublishPatientChange(Patient changedPatient)
        {
            _callbackList.ForEach(delegate (IPatientChangedCallback callback)
            {
                callback.PatientChange(changedPatient);
            });
        }

        private void TableDependency_Changed(object sender, RecordChangedEventArgs<Patient> e)
        {
            PublishPatientChange(e.Entity);
        }

        public void NotifyOthers(string message)
        {
            try
            {
                _callbackList.ForEach(delegate (IPatientChangedCallback callback)
                {
                    if (callback != CurrentUser)
                    {
                        ExecuteWithTimeout(() => { callback.NotificationReceived(message); }, 5000);
                        //callback.NotificationReceived(message);
                    }
                });
            }
            catch (Exception ex)
            {

            }
        }

        private static bool ExecuteWithTimeout(Action action, int timeout)
        {
            Exception ex = null;
            var cancellationTokenSource = new CancellationTokenSource();

            Task task = Task.Run(() =>
            {
                try
                {
                    using (cancellationTokenSource.Token.Register(Thread.CurrentThread.Abort))
                    {
                        action();
                    }
                }
                catch (Exception e)
                {
                    if (!(e is ThreadAbortException))
                        ex = e;
                }
            }, cancellationTokenSource.Token);

            bool done = task.Wait(timeout);

            if (ex != null) { throw ex; }
            if (!done) { cancellationTokenSource.Cancel(); }

            return done;
        }
    }
}
