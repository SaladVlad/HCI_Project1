using HCI___Fashion.Classes;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace HCI___Fashion
{
    /// <summary>
    /// Interaction logic for CreateOrEditWindow.xaml
    /// </summary>
    public partial class CreateOrEditWindow : Window
    {
        #region Fields and Properties

        private readonly Helpers.DataIO _io;

        private readonly NotificationManager _notificationManager;

        private readonly ItemContainer _item;

        private readonly List<int> IDs;

        private readonly int _previousID;

        #endregion

        #region InitFunctions

        public CreateOrEditWindow(ItemContainer itemContainer, ObservableCollection<ItemContainer> items)
        {
            //passing the reference of the modified item
            _item = itemContainer;

            InitializeComponent();
            DataContext = this;

            _notificationManager = new NotificationManager(Application.Current.Dispatcher);
            _io = new Helpers.DataIO();

           

            InitUI();
            if (itemContainer != null)
                FillWithData(itemContainer);


            _previousID = _item.Id;

            IDs = new List<int>();
            foreach (ItemContainer ic in items)
            {
                IDs.Add(ic.Id);
            }


        }
        private void FillWithData(ItemContainer itemContainer)
        {
            IDTextBox.Text = itemContainer.Id.ToString() == "0" ? "" : itemContainer.Id.ToString();
            NameTextBox.Text = itemContainer.Name;
            if (itemContainer.ImagePath != null)
            {
                ImageFrame.Source = new BitmapImage(new Uri(itemContainer.ImagePath, UriKind.Absolute));

            }
            else
            {
                ImageFrame.Source = new BitmapImage(new Uri("ImgSourceUI/template.jpg", UriKind.RelativeOrAbsolute));
            }
            _io.LoadRtfFile(itemContainer.TextPath, EditorRichTextBox);

        }

        private void InitUI()
        {
            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.SelectedIndex = 0;
            ColorPicker.SelectedColor = Brushes.Black.Color;
        }

        #endregion

        #region Events

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            bool allGood = true;

            if (!IDTextBox.Text.Equals(string.Empty))
            {
                int i = int.Parse(IDTextBox.Text);
                if (IDs.Contains(i) && _previousID != i)
                {

                    IDTextBox.BorderBrush = Brushes.Red;

                    RaiseToast("existsID");
                    allGood = false;
                }

            }
            else
            {
                IDTextBox.BorderBrush = Brushes.Red;
                RaiseToast("emptyID");
                allGood = false;
            }



            if (NameTextBox.Text.Equals(""))
            {
                NameTextBox.BorderBrush = Brushes.Red;
                RaiseToast("emptyName");
                allGood = false;
            }


            TextRange textRange = new TextRange(EditorRichTextBox.Document.ContentStart, EditorRichTextBox.Document.ContentEnd);
            if (textRange.Text.Length == 2 || textRange.Text == "")
            {
                EditorRichTextBox.BorderBrush = Brushes.Red;
                RaiseToast("emptyRTB");
                allGood = false;
            }

            string defaultSource = "ImgSourceUI/template.jpg";
            Console.WriteLine(ImageFrame.Source.ToString());
            Console.WriteLine(ImageFrame.Source.ToString().Contains(defaultSource));
            if (ImageURLTextBox.Text.Equals("Paste new URL...") &&
                ImageFrame.Source.ToString().Contains(defaultSource))
            {
                ImageURLTextBox.BorderBrush = Brushes.Red;
                RaiseToast("imageMissing");
                allGood = false;
            }

            if (allGood)
            {
                //item didn't exist before, so mark it's creation time
                if (_item.Name == null)
                {
                    _item.CreationDate = DateTime.Now;
                }

                _item.Id = int.Parse(IDTextBox.Text);
                _item.Name = NameTextBox.Text;
                _item.ImagePath = ImageFrame.Source.ToString();


                //rename the .rtf file if ID changed                
                if (_previousID != _item.Id && File.Exists(_item.TextPath))
                {
                    System.IO.File.Move(_item.TextPath, "item" + IDTextBox.Text + ".rtf");
                }

                _item.TextPath = "item" + IDTextBox.Text + ".rtf";


                _io.SaveAsRtfFile(_item.TextPath, EditorRichTextBox);


                this.Close();
            }

        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null && !EditorRichTextBox.Selection.IsEmpty)
            {
                EditorRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBoxResult.Yes;
            //MessageBoxResult res = MessageBox.Show("Do you want to abandon these changes?","Warning",MessageBoxButton.YesNo,MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void ImageURLTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ImageURLTextBox.BorderBrush = Brushes.DarkCyan;
            ImageURLTextBox.BorderThickness = new Thickness(3);

            if (ImageURLTextBox.Text.Equals("Paste new URL..."))
            {
                ImageURLTextBox.Text = "";
                ImageURLTextBox.Foreground = Brushes.Black;
            }
        }

        private void ImageURLTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ImageURLTextBox.Text.Equals(""))
            {
                ImageURLTextBox.Text = "Paste new URL...";
                ImageURLTextBox.Foreground = Brushes.Gray;
            }
            else
            {
                try
                {
                    ImageFrame.Source = new BitmapImage(new Uri(ImageURLTextBox.Text, UriKind.Absolute));
                }
                catch
                {
                    RaiseToast("wrongURL");
                    ImageURLTextBox.BorderBrush = Brushes.Red;
                    ImageURLTextBox.BorderThickness = new Thickness(3);
                }
            }
        }

        private void ImageURLTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    ImageFrame.Source = new BitmapImage(new Uri(ImageURLTextBox.Text, UriKind.Absolute));
                }
                catch
                {
                    RaiseToast("wrongURL");
                }
            }
        }

        private void ImageURLTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            try
            {
                ImageFrame.Source = new BitmapImage(new Uri(ImageURLTextBox.Text, UriKind.Absolute));
            }
            catch
            {
                if(_item.ImagePath != null)
                {
                    ImageFrame.Source = new BitmapImage(new Uri(_item.ImagePath, UriKind.Absolute));
                }
                else
                {
                    ImageFrame.Source = new BitmapImage(new Uri("ImgSourceUI/template.jpg", UriKind.RelativeOrAbsolute));
                }
                
            }
        }

        private void RaiseToast(string semantic)
        {
            if (semantic.Equals("wrongURL"))
            {
                _notificationManager.Show("URL is not valid!", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
            if (semantic.Equals("emptyID"))
            {
                _notificationManager.Show("ID can't be Empty!", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
            if (semantic.Equals("existsID"))
            {
                _notificationManager.Show("ID already exists!", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
            if (semantic.Equals("emptyName"))
            {
                _notificationManager.Show("Name can't be Empty!", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
            if (semantic.Equals("emptyRTB"))
            {
                _notificationManager.Show("Description can't be empty!", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");
            }
            if (semantic.Equals("imageMissing"))
            {
                _notificationManager.Show("Image can't be empty! Please choose a new Image.", NotificationType.Error, "CreateOrUpdateWindowNotificationArea");

            }
        }

        public int CountWords(RichTextBox richTextBox)
        {
            string text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;

            MatchCollection matches = Regex.Matches(text, @"[\w']+");
            return matches.Count;
        }

        private void EditorRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //update word count
            WordCountLabel.Content = "Word count: " + CountWords(EditorRichTextBox).ToString();
        }

        private void EditorRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //update the UI elements
            object fontWeight = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldToggleButton.IsChecked = (fontWeight != DependencyProperty.UnsetValue) && (fontWeight.Equals(FontWeights.Bold));

            object fontStyle = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicToggleButton.IsChecked = (fontStyle != DependencyProperty.UnsetValue) && (fontStyle.Equals(FontStyles.Italic));

            object textDecoration = EditorRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineToggleButton.IsChecked = (textDecoration != DependencyProperty.UnsetValue) && (textDecoration.Equals(TextDecorations.Underline));

            object fontFamily = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamilyComboBox.SelectedItem = fontFamily;

            object fontSize = EditorRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            FontSizeTextBox.Text = fontSize.ToString();


            object fontColor = EditorRichTextBox.Selection.GetPropertyValue(Inline.ForegroundProperty);

            if (fontColor is Brush brush)
            {
                //if a proper name exists, display it instead of the hex code
                if (brush is SolidColorBrush solidColorBrush)
                {
                    ColorTextBox.Text = GetNameForColor(solidColorBrush.Color);
                }
            }

        }

        public static string GetNameForColor(Color color)
        {
            foreach (var property in typeof(Colors).GetProperties())
            {
                if ((Color)property.GetValue(null) == color)
                {
                    return property.Name;
                }
            }
            return color.ToString();
        }

        private void FontSizeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //if text size is varying, don't change it because it will mess up the text
            if (FontSizeTextBox.Text.Contains("{D"))
            {
                FontSizeTextBox.Text = "";
                return;
            }
            try
            {
                int size = int.Parse(FontSizeTextBox.Text);
                ChangeFontSize(size);
            }
            catch
            {
                FontSizeTextBox.Text = "12";
                ChangeFontSize(12);
            }
        }

        private void ChangeFontSize(double size)
        {

            TextRange selectionRange = new TextRange(EditorRichTextBox.Selection.Start, EditorRichTextBox.Selection.End);

            selectionRange?.ApplyPropertyValue(TextElement.FontSizeProperty, size);
        }

        private void FontSizeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                try
                {
                    double size = int.Parse(FontSizeTextBox.Text);
                    ChangeFontSize(size);
                    FontSizeTextBox.Text = size.ToString();
                }
                catch
                {

                    ChangeFontSize(12.0);
                    FontSizeTextBox.Text = "12";
                }
            }
        }

        private void FontSizeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                // If it's not a digit, mark the event as handled
                e.Handled = true;
            }
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            ColorTextBox.Text = ColorPicker.SelectedColorText;
            SolidColorBrush brush = new SolidColorBrush((System.Windows.Media.Color)ColorPicker.SelectedColor);

            TextRange selectionRange = new TextRange(EditorRichTextBox.Selection.Start, EditorRichTextBox.Selection.End);

            selectionRange?.ApplyPropertyValue(TextElement.ForegroundProperty, brush);

        }

        private void IDTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                // If it's not a digit, mark the event as handled
                e.Handled = true;
            }
        }

        private void EditorRichTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EditorRichTextBox.BorderBrush = Brushes.DarkCyan;
            EditorRichTextBox.BorderThickness = new Thickness(3);
        }

        private void IDTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            IDTextBox.BorderBrush = Brushes.DarkCyan;
            IDTextBox.BorderThickness = new Thickness(3);
        }

        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NameTextBox.BorderBrush = Brushes.DarkCyan;
            NameTextBox.BorderThickness = new Thickness(3);
        }

    }

    #endregion
}
