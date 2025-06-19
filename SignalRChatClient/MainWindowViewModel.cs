using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SignalRChatClient
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Event Declarations
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Private Fields
        private HubConnection _connection;
        #endregion

        #region Properties
        private string _CurrentRoom;
        public string CurrentRoom
        {
            get { return _CurrentRoom; }
            set
            {
                if (_CurrentRoom != value)
                {
                    _CurrentRoom = value;
                    RaisePropertyChanged(nameof(CurrentRoom));
                }
            }
        }

        private bool _IsConnected;
        public bool IsConnected
        {
            get { return _IsConnected; }
            set
            {
                if (_IsConnected != value)
                {
                    _IsConnected = value;
                    RaisePropertyChanged(nameof(IsConnected));
                }
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }
        #endregion

        #region Commands
        private ICommand _AddGroupCommand;
        public ICommand AddGroupCommand
        {
            get
            {
                if (_AddGroupCommand == null)
                    _AddGroupCommand = new RelayCommand(p => AddGroupExecuted(), p => AddGroupCanExecute());
                return _AddGroupCommand;
            }
        }

        private ICommand _ConnectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                if (_ConnectCommand == null)
                    _ConnectCommand = new RelayCommand(p => ConnectExecuted(), p => ConnectCanExecute());
                return _ConnectCommand;
            }
        }

        private ICommand _DisconnectCommand;
        public ICommand DisconnectCommand
        {
            get
            {
                if (_DisconnectCommand == null)
                    _DisconnectCommand = new RelayCommand(p => DisconnectExecuted(), p => DisconnectCanExecute());
                return _DisconnectCommand;
            }
        }

        private ICommand _RemoveGroupCommand;
        public ICommand RemoveGroupCommand
        {
            get
            {
                if (_RemoveGroupCommand == null)
                    _RemoveGroupCommand = new RelayCommand(p => RemoveGroupExecuted(), p => RemoveGroupCanExecute());
                return _RemoveGroupCommand;
            }
        }
        #endregion

        #region CTOR
        public MainWindowViewModel()
        {
            _connection = new HubConnectionBuilder().WithUrl(Properties.Settings.Default.Server)
                                                    .WithAutomaticReconnect()
                                                    .Build();

            WireUpEvents();
        }
        #endregion

        #region Methods
        private bool AddGroupCanExecute()
        {
            return IsConnected;
        }

        private void AddGroupExecuted()
        {
        }

        private async Task BeginConnectionAsync()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            IsConnected = _connection.State == HubConnectionState.Connected;
        }

        private bool ConnectCanExecute()
        {
            return !string.IsNullOrEmpty(Name) && !IsConnected;
        }

        private void ConnectExecuted()
        {
            _ = BeginConnectionAsync();
        }

        private bool DisconnectCanExecute()
        {
            return IsConnected;
        }

        private void DisconnectExecuted()
        {
            _ = EndConnection();
            IsConnected = false;
        }

        private async Task EndConnection()
        {
            await _connection.StopAsync();
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool RemoveGroupCanExecute()
        {
            return IsConnected;
        }

        private void RemoveGroupExecuted()
        {
        }

        private async Task SendMessageAsync(string username, string message)
        {
            try
            {
                if (_connection.State == HubConnectionState.Connected)
                {
                    await _connection.InvokeAsync("SendMessage", username, CurrentRoom, message);
                }
                else
                {
                    MessageBox.Show("Not connected to the server.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WireUpEvents()
        {
            // Wire up any events here, e.g. for receiving messages
            _connection.On<string>("ReceiveMessage", message =>
            {
            });

            _connection.Reconnecting += error =>
            {
                //Dispatcher.Invoke(() => StatusLabel.Content = "Reconnecting...");
                return Task.CompletedTask;
            };

            _connection.Reconnected += connectionId =>
            {
                //Dispatcher.Invoke(() => StatusLabel.Content = "Reconnected");

                IsConnected = _connection.State == HubConnectionState.Connected;
                return Task.CompletedTask;
            };

            _connection.Closed += async (error) =>
            {
                //Dispatcher.Invoke(() => StatusLabel.Content = "Disconnected");

                // Optional: wait and retry
                await Task.Delay(3000);

                try
                {
                    await BeginConnectionAsync();
                    //Dispatcher.Invoke(() => StatusLabel.Content = "Reconnected");
                }
                catch (Exception ex)
                {
                    //Dispatcher.Invoke(() =>
                    //{
                    //    StatusLabel.Content = "Reconnect failed";
                    //    MessageBox.Show($"Reconnect failed: {ex.Message}", "Error");
                    //});
                }
            };
        }
        #endregion
    }
}
