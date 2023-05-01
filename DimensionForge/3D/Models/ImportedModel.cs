using DimensionForge._3D.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DimensionForge._3D.Models
{
    public partial class ImportedModel : BatchedModel3D
    {
        public ImportedModel(string fileName)
        {
            Name = fileName;
            ID = Guid.NewGuid().ToString();
            FileName = fileName;

        }

       


        
    }
}
