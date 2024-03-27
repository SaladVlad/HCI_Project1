using HCI___Fashion.Classes;
using HCI___Fashion.Helpers;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
    public partial class MainWindow : Window
    {

        #region Fields and Properties
        public bool IsAdmin { get; }

        Helpers.DataIO IO = new Helpers.DataIO();

        ObservableCollection<ItemContainer> _items;
        private NotificationManager notificationManager;

        Helpers.DataIO io = new Helpers.DataIO();

        public ObservableCollection<ItemContainer> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        #endregion

        #region UI Init
        public MainWindow(bool isAdmin)
        {
            //initializing UI component and setting up functionality

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
                DataGridColumn checkBoxColumn = ContentDataGrid.Columns[0];
                checkBoxColumn.Visibility = Visibility.Collapsed;
                ModeLabel.Content = "Viewer mode";
            }


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

        #endregion

        #region Events
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
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

        private void AddContentButton_Click(object sender, RoutedEventArgs e)
        {

            ItemContainer newItem = new ItemContainer();
            CreateOrEditWindow createOrEditWindow = new CreateOrEditWindow(newItem, Items);
            createOrEditWindow.ShowDialog();

            if (newItem.Name != null)
            {
                Items.Add(newItem);
                RaiseToast("creationSuccessful");
                io.SerializeObject(Items, "items.xml");
            }

        }

        private void DeleteContentButton_Click(object sender, RoutedEventArgs e)
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
            //clear leftover rtf files and save
            CleanRTFFiles();
            io.SerializeObject(Items, "items.xml");

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

                if (IsAdmin)
                {
                    ItemContainer oldItem = item.Clone();

                    CreateOrEditWindow createOrEditWindow = new CreateOrEditWindow(item, Items);
                    createOrEditWindow.ShowDialog();
                    e.Handled = true;

                    if (item.Id == oldItem.Id &&
                        item.Name == oldItem.Name &&
                        item.ImagePath == oldItem.ImagePath &&
                        item.TextPath == oldItem.TextPath)
                    {
                        //nothing changed
                    }
                    else
                    {
                        ContentDataGrid.Items.Refresh();
                        RaiseToast("editSuccessful");

                    }
                }
                else
                {
                    ViewWindow viewWindow = new ViewWindow(item);
                    viewWindow.ShowDialog();
                    e.Handled = true;
                }

            }
        }

        #endregion

        #region Helper Methods
        private void RaiseToast(string semantic)
        {
            if (semantic.Equals("noSelectedItems"))
            {
                notificationManager.Show("Error", "There are no selected items for deletion!", NotificationType.Error, "WindowNotificationArea");
            }
            if (semantic.Equals("creationSuccessful"))
            {
                notificationManager.Show("Creation successful", "Item has been added to the list!", NotificationType.Success, "WindowNotificationArea");
            }
            if (semantic.Equals("editSuccessful"))
            {
                notificationManager.Show("Edit successful", "Item has been edited sucessfully!", NotificationType.Success, "WindowNotificationArea");
            }
        }

        private void CleanRTFFiles()
        {
            string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Collect file names for each item
            HashSet<string> validFileNames = new HashSet<string>();
            foreach (ItemContainer item in Items)
            {
                string id = item.Id.ToString();
                string fileName = "item" + id + ".rtf";
                validFileNames.Add(fileName);
            }

            // Get all .rtf files in the root directory
            string[] files = Directory.GetFiles(rootDirectory, "*.rtf", SearchOption.TopDirectoryOnly);

            // Delete files that are not in the set of relevent file names
            foreach (string filePath in files)
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                if (!validFileNames.Contains(fileName))
                {
                    File.Delete(filePath);
                }
            }
        }

        #endregion
    }
}
