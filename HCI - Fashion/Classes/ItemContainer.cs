using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HCI___Fashion.Classes
{
    [Serializable]
    public class ItemContainer /*: INotifyPropertyChanged*/
    {
        int _id;
        string _name;
        string _imagePath;
        string _textPath;
        DateTime _creationDate;

        public ItemContainer(int id, string name, string imagePath, string textPath)
        {
            this.Id = id;
            this.Name = name;
            this.ImagePath = "\\img\\" + imagePath;
            this.TextPath = textPath;
            this.CreationDate = DateTime.Now;
        }

        public ItemContainer()
        {

        }

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                //_imagePath = value;
                //OnPropertyChanged(nameof(ImageSource));
            }
        }
        public string TextPath { get => _textPath; set => _textPath = value; }
        public DateTime CreationDate { get => _creationDate; set => _creationDate = value; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public override string ToString()
        {
            return "Id: "+Id+" Name: "+Name+" ImgPath: "+ImagePath+" TextPath: "+TextPath+"\n";
        }
    }
}
