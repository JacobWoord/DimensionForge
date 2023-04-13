using DimensionForge._3D.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DimensionForge._3D.Models
{
    public partial class Door : BathedModel3D
    {
        public Door(DoorData data)
        {
            Name =  "Bord";
            ID = Guid.NewGuid().ToString();
            FileName = data.FilePath;

        }
    }
}
