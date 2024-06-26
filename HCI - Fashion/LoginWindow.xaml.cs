﻿using HCI___Fashion.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using HCI___Fashion.Helpers;
using Notification.Wpf;
using static System.Net.WebRequestMethods;
using System.Net.NetworkInformation;

namespace HCI___Fashion
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Fields

        readonly List<User> _users;

        readonly NotificationManager _notificationManager;

        #endregion

        #region InitMethods

        public LoginWindow()
        {

            if (!IsConnectedToInternet())
            {
                MessageBox.Show("You need internet to run this application! (This app uses internet resources)",
                                "Critical error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Stop);
                Environment.Exit(-1);

            }


            InitializeComponent();
            DataIO io = new Helpers.DataIO();
            _users = io.DeSerializeObject<List<User>>("users.xml");
            _notificationManager = new NotificationManager();
            UsernameTextBox.Focus();
        }
        public bool IsConnectedToInternet()
        {
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send("google.com", 1000);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        #endregion

        #region Events

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                User foundUser = null;
                foreach(User u in _users)
                {
                    if(u.Username.Equals(UsernameTextBox.Text) && u.Password.Equals(PasswordBox.Password))
                    {
                        foundUser = u;
                        break;
                    }
                }
                if (foundUser != null)
                {
                    MainWindow mainWindow;
                    if(foundUser.Role == UserRole.Visitor)
                    {
                        mainWindow = new MainWindow(false);
                    }
                    else
                    {
                        mainWindow = new MainWindow(true);
                    }
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    RaiseToast("accountNonExistent");
                }
            }
        }

        private void RaiseToast(string semantic)
        {
            if (semantic.Equals("usernameEmpty"))
            {
                _notificationManager.Show("Error","Username cannot be blank!",NotificationType.Error,"WindowNotificationArea");
            }
            if (semantic.Equals("passwordEmpty"))
            {
                _notificationManager.Show("Error", "Password cannot be blank!", NotificationType.Error, "WindowNotificationArea");
            }
            if (semantic.Equals("accountNonExistent"))
            {
                _notificationManager.Show("Error", "User not found!", NotificationType.Error, "WindowNotificationArea");
            }
        }

        private bool ValidateForm()
        {
            bool valid = true;

            if(UsernameTextBox.Text.Equals("Enter username"))
            {
                UsernameTextBoxBorder.BorderBrush = Brushes.Red;
                RaiseToast("usernameEmpty");
                valid = false;
            }
            if (PasswordBox.Password.Equals("Enter password"))
            {
                PasswordBoxBorder.BorderBrush = Brushes.Red;
                RaiseToast("passwordEmpty");
                valid = false;
            }

            return valid;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text.Equals("Enter username"))
            {
                UsernameTextBoxBorder.BorderBrush = Brushes.Transparent;
                UsernameTextBox.Text = string.Empty;
                UsernameTextBox.Foreground = Brushes.Black;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.Equals("Enter password"))
            {
                PasswordBoxBorder.BorderBrush = Brushes.Transparent;
                PasswordBox.Password = string.Empty;
                PasswordBox.Foreground = Brushes.Black;
            }
            
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text.Equals(string.Empty))
            {
                UsernameTextBox.Text = "Enter username";
                UsernameTextBox.Foreground = Brushes.Gray;
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.Equals(string.Empty))
            {
                PasswordBox.Password = "Enter password";
                PasswordBox.Foreground = Brushes.Gray;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                LoginButton_Click(sender,e);
            }
        }

        #endregion
    }
}
