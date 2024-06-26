﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HCI___Fashion.Classes
{
    [Serializable]
    public class ItemContainer
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
            this.ImagePath = imagePath;
            this.TextPath = textPath;
            this.CreationDate = DateTime.Now;
        }

        public ItemContainer()
        {

        }

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string ImagePath { get => _imagePath; set => _imagePath = value; }
        public string TextPath { get => _textPath; set => _textPath = value; }
        public DateTime CreationDate { get => _creationDate; set => _creationDate = value; }

        public ItemContainer Clone()
        {
            return new ItemContainer(Id, Name, ImagePath, TextPath);
        }

    }
}
