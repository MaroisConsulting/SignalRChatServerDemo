using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Windows;

namespace SignalRChatClient
{
    public partial class MainWindow : Window
    {
        private HubConnection _connection;

        public MainWindow()
        {
            InitializeComponent();
            StartConnection();
        }

        private async void StartConnection()
        {
            _connection = new HubConnectionBuilder().WithUrl("https://localhost:7206/chathub")
                                                    .WithAutomaticReconnect()
                                                    .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessagesList.Items.Add($"{user}: {message}");
                });
            });

            try
            {
                await _connection.StartAsync();
                MessagesList.Items.Add("Connected to chat server.");
            }
            catch (Exception ex)
            {
                MessagesList.Items.Add($"Connection error: {ex.Message}");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var user = UserTextBox.Text;
            var message = MessageTextBox.Text;

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(message))
            {
                await _connection.InvokeAsync("SendMessage", user, message);
                MessageTextBox.Clear();
            }
        }
    }
}
