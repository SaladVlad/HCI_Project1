using HCI___Fashion.Classes;
using HCI___Fashion.Helpers;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HCI___Fashion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window  //, INotifyPropertyChanged
    {
        public bool IsAdmin { get; }

        Helpers.DataIO IO = new Helpers.DataIO();

        ObservableCollection<ItemContainer> _items;
        private NotificationManager notificationManager;

        //public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ItemContainer> Items
        {
            get { return _items; }
            set { _items = value; }
        }
        public MainWindow(bool isAdmin)
        {
            //initializing UI component and setting up functionality
            #region UI Initialization

            InitializeComponent();
            DataContext = this;

            notificationManager = new NotificationManager();

            IsAdmin = isAdmin;

            if (IsAdmin)
            {
                AddContentButton.Visibility = Visibility.Visible;
                AddContentButton.IsEnabled = true;
                DeleteContentButton.Visibility = Visibility.Visible;
                DeleteContentButton.IsEnabled = true;
                ModeLabel.Content = "Admin mode";
            }
            else
            {
                AddContentButton.Visibility = Visibility.Hidden;
                AddContentButton.IsEnabled = false;
                DeleteContentButton.Visibility = Visibility.Hidden;
                DeleteContentButton.IsEnabled = false;
                ModeLabel.Content = "Viewer mode";
            }

            #endregion

            //Content fetch from xml
            #region Content fetch
            try
            {
                Items = IO.DeSerializeObject<ObservableCollection<ItemContainer>>("items.xml");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            #endregion

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdmin)
            {
                MessageBoxResult result = MessageBox.Show(
                               "Save changes before exiting?",
                               "Confirmation",
                               MessageBoxButton.YesNoCancel,
                               MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Helpers.DataIO io = new Helpers.DataIO();

                    io.SerializeObject(Items, "items.xml");
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();

                }
                else if (result == MessageBoxResult.No)
                {
                    //Dont save anything and exit the program
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }

            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?",
                                                    "Confirmation",
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }

        }

        private void AddContentButton_Click(object sender, RoutedEventArgs e)
        {
            CreateOrEditWindow createOrEditWindow = new CreateOrEditWindow(false);
        }

        private void DeleteContentButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected items?",
                               "Warning",
                               MessageBoxButton.YesNo,
                               MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                List<ItemContainer> itemsToRemove = new List<ItemContainer>();

                foreach (DataGridColumn column in ContentDataGrid.Columns)
                {
                    if (column is DataGridCheckBoxColumn checkBoxColumn)
                    {
                        foreach (ItemContainer ic in ContentDataGrid.Items)
                        {
                            bool? isChecked = checkBoxColumn.GetCellContent(ic) is CheckBox checkBox ? checkBox.IsChecked : null;

                            if (isChecked == true)
                            {
                                itemsToRemove.Add(ic);
                            }
                        }
                    }
                }

                if (itemsToRemove.Count < 1)
                {
                    RaiseToast("noSelectedItems");
                    return;
                }
                foreach (ItemContainer ic in itemsToRemove)
                {
                    Items.Remove(ic);
                }
            }
        }

        private void RaiseToast(string semantic)
        {
            if (semantic.Equals("noSelectedItems"))
            {
                notificationManager.Show("Error", "There are no selected items for deletion!", NotificationType.Error, "WindowNotificationArea");
            }
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {

            if (e.OriginalSource is Hyperlink hyperlink)
            {
                ItemContainer rowData = (ItemContainer)hyperlink.DataContext;

                ItemContainer item = null;
                foreach (ItemContainer ic in Items)
                {
                    if (ic.Id == rowData.Id)
                    {
                        item = ic;
                        break;
                    }
                }

                CreateOrEditWindow createOrEditWindow = new CreateOrEditWindow(item,IsAdmin);
                createOrEditWindow.ShowDialog();
                e.Handled = true;
            }
        }

    }
}
