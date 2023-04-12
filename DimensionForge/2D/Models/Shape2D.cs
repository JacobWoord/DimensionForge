using DimensionForge._2D.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace DimensionForge._2D.Models
{
    public partial class Shape2D : ObservableObject, IShape2D
    {

        private Guid id;  
        public Guid Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        [ObservableProperty]
        Point position;
      
        [ObservableProperty]
        Color fillColor;

        [ObservableProperty]
        bool isSelected;

        partial void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            if (IsSelected)
            {
                FillColor = Color.FromRgb(222, 111, 0);

            }
            else
            {
                FillColor = Color.FromRgb(255, 255, 0);
            }
        }


        public System.Windows.Media.Color StrokeColor { get ; set; }
        public float StrokeThickness { get ; set ; }


       
        [RelayCommand]
        void Click()
        {
            FillColor = Color.FromRgb(222, 111, 0);
        }

        public Shape2D()
        {
            id = Guid.NewGuid();
        }
        [RelayCommand]
        public void Select()
        {
            IsSelected = !IsSelected;
        }

        public void Deselect()
        {
            
        }
    }
}
