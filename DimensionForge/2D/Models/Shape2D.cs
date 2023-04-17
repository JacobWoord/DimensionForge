using DimensionForge._2D.interfaces;
using Vector2 = SharpDX.Vector2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace DimensionForge._2D.Models
{
    public partial class Shape2D : ObservableObject, IShape2D
    {
        public string ID { get; set; }

        [ObservableProperty]
        Point position;


        [ObservableProperty]
        Point oldPosition;


        [ObservableProperty]
        Color fillColor;

        [ObservableProperty]
        float maxVelocity = 10f;

        [ObservableProperty]
        bool isSelected;
        public Color StrokeColor { get; set; }
        public float StrokeThickness { get; set; }
        
       
 
        public Shape2D()
        {
            ID = Guid.NewGuid().ToString();
        }

      
      

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


        [RelayCommand]
        void Click()
        {
            FillColor = Color.FromRgb(222, 111, 0);
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
