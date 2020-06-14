using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WsDualHttpBinding_SqlTableDependency_PoC.WPF.ServiceProxy;

namespace WsDualHttpBinding_SqlTableDependency_PoC.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IPatientServiceCallback
    {
        public Guid WindowID { get; }
        public ObservableCollection<Patient> Patients { get; }

        private readonly IPatientService _proxy;
        private Patient _changedPatient;
        private Brush _colourChange;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            WindowID = Guid.NewGuid();

            _proxy = new PatientServiceClient(new InstanceContext(this));
            _proxy.Subscribe();

            Patients = new ObservableCollection<Patient>(_proxy.GetAllPatients().OrderBy(p => p.SortOrder));
        }

        /// <summary>
        /// Server callback.
        /// Notifications the received.
        /// </summary>
        /// <param name="message">The message.</param>
        public void NotificationReceived(string message)
        {
            try
            {
                // Cannot have thread blocking calls even on the client side,
                // server WILL timeout!!
                //var notificationWindow = new NotificationWindow(message)
                //{
                //    Owner = this,
                //    WindowStartupLocation = WindowStartupLocation.CenterOwner
                //};
                //notificationWindow.ShowDialog();
                TxtBlkReceivedMessage.Text = $"{message} @ {DateTime.Now}";
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Server callback.
        /// Patients the change.
        /// </summary>
        /// <param name="changedPatient">The changed patient.</param>
        public void PatientChange(Patient changedPatient)
        {
            var patient = Patients.SingleOrDefault(p => p.ID == changedPatient.ID);

            if (patient == null)
            {
                Patients.Add(changedPatient);
                _changedPatient = changedPatient;
                _colourChange = Brushes.Lime;
            }
            else
            {
                patient.FirstName = changedPatient.FirstName;
                patient.LastName = changedPatient.LastName;
                patient.Email = changedPatient.Email;
                patient.DateOfBirth = changedPatient.DateOfBirth;
                _changedPatient = patient;
                _colourChange = Brushes.Yellow;
            }

            // Required for property changes to be reflected on the UI immediately.
            DtgPatient.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                DtgPatient.Items.Refresh();
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //_proxy?.Unsubscribe();
                DoWithTimeout(_proxy.Unsubscribe, 5000);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _proxy.NotifyOthers(WindowID.ToString());
        }

        private void DtgPatient_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (_changedPatient == null) { return; }

            var patientRow = e.Row.Item as Patient;

            e.Row.Background = _changedPatient.ID == patientRow.ID
                ? _colourChange
                : Brushes.White;
        }

        /// <summary>
        /// Provides a time limit for the specified callback.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        private static bool DoWithTimeout(Action action, int timeout)
        {
            Exception ex = null;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
