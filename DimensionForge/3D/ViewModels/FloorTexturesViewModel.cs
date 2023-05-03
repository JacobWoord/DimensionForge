using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimensionForge._3D.ViewModels
{
    public partial class FloorTexturesViewModel : ObservableObject
    {

        private Canvas3DViewModel canvasViewModel;

        public FloorTexturesViewModel()
        {
            canvasViewModel = Ioc.Default.GetService<Canvas3DViewModel>();
        }

        [RelayCommand]
        void SetFloorParameter(string destination)
        {

            canvasViewModel.Reload(destination);

        }
    }
}
