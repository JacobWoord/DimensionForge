using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DimensionForge._3D.ViewModels
{
    public partial class MoveControlsViewModel : ObservableObject
    {

        private string _selectedAxis;
        
        public string SelectedAxis
        {
            get { return _selectedAxis; }
            set
            {
                if (_selectedAxis != value)
                {
                    _selectedAxis = value;
                    OnPropertyChanged(nameof(SelectedAxis));
                }
            }
        }


        private string copyButtonContent;

        public string CopyButtonContent
        {
            get { return copyButtonContent; }
            set
            {
                if (copyButtonContent != value)
                {
                    copyButtonContent = value;
                    OnPropertyChanged(nameof(CopyButtonContent));
                }
            }
        }


        [ObservableProperty]
        Vector3 modelPosition;

        [ObservableProperty]
        bool isCopied = false;

        [ObservableProperty]
        float frequency = 0.01f;

       

        private Canvas3DViewModel _canvas ; 
       
        
        
        
        
        public MoveControlsViewModel()
        {
            _canvas = Ioc.Default.GetService<Canvas3DViewModel>();  
            CopyButtonContent = "Copy";
        }





        [RelayCommand]
        public void SelectAxis(string destination)
        {

            switch (destination)
            {
                case "Z":
                    SelectedAxis = "Z";
                        break;
                    case "X":
                    SelectedAxis = "X";
                    break;
                    case "Y":
                    SelectedAxis = "Y";
                    break;

            }
                
        }
                


        [RelayCommand]
        public void MoveValueUp()
        {
            if (SelectedAxis is not null)
            {

                var m = _canvas.SelectedModel;

                switch (SelectedAxis)
                {
                    case "X":
                        m.LinkedNode.Position.X += frequency;
                        break;
                        case "Y":
                            m.LinkedNode.Position.Y += frequency;
                        break;
                        case "Z":
                            m.LinkedNode.Position.Z += frequency;
                        break;
                }
                ModelPosition = m.LinkedNode.Position;
                _canvas.Draw();
            }
        }
                   



            

        [RelayCommand]
        public void MoveValueDown()
        {
            if (SelectedAxis is not null)
            {

                var m = _canvas.SelectedModel;

                switch (SelectedAxis)
                {
                    case "X":
                        m.LinkedNode.Position.X -= frequency;
                        break;
                    case "Y":
                        m.LinkedNode.Position.Y -= frequency;
                        break;
                    case "Z":
                        m.LinkedNode.Position.Z -= frequency;
                        break;
                }
                ModelPosition = m.LinkedNode.Position;

                _canvas.Draw();
            }

        }


        private async Task SetCopied()
        {
            IsCopied = true;
            CopyButtonContent = "Copied!";

            Task.Delay(1000);

            IsCopied = false;
            copyButtonContent = "Copy";
        }

        [RelayCommand]
        public void CopyToClipboard()
        {
            Clipboard.SetText(ModelPosition.ToString());
        }


    }

     


    
}
