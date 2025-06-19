using Microsoft.AspNetCore.SignalR.Client;
using SignalRChatShared;
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

            _connection = new HubConnectionBuilder().WithUrl("https://yourserver.com/chathub")
                                                    .WithAutomaticReconnect()
                                                    .Build();

            // Start connection on load or button click
            _ = ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            await _connection.StartAsync();

            //// After connection is started, call JoinRoom
            //string username = UserTextBox.Text;
            //string room = RoomTextBox.Text;

            //if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(room))
            //{
            //    await _connection.InvokeAsync("JoinRoom", new ChatUserInfo
            //    {
            //        Username = username,
            //        Room = room
            //    });
            //}
        }

        private async Task SendMessageAsync(string username, string room, string message)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("SendMessage", username, room, message);
            }
            else
            {
                // Handle disconnected state or reconnect
            }
        }

        // Example usage: wired to a button click event
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //string username = UserTextBox.Text; // your username input control
            //string room = RoomTextBox.Text;         // your room input control
            //string message = MessageTextBox.Text;   // your message input control

            //if (!string.IsNullOrWhiteSpace(username) &&
            //    !string.IsNullOrWhiteSpace(room) &&
            //    !string.IsNullOrWhiteSpace(message))
            //{
            //    await SendMessageAsync(username, room, message);
            //    MessageTextBox.Clear();
            //}
        }
    }
}
