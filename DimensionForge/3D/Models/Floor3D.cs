using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using SharpDX.DirectWrite;

namespace DimensionForge._3D.Models
{
    public partial class Floor3D : Shape3D
    {
        public float Width {get; set;} = 100;
        public float Length {get; set;} = 100;
        public float Height {get; set;} = 0.001f;
       
        public string FloorNumber { get; set;} = "1";
       

        public Floor3D(string num)
        {
            //Init          
            Name = "Floor";
            Id = Guid.NewGuid().ToString();
            FloorNumber = num;
            PhongMaterial p = PhongMaterials.Red;

            p.DiffuseMap = new TextureModel(SetFloor());
            p.RenderDiffuseMap = true;
            p.DiffuseColor = SharpDX.Color.DimGray.ToColor4();
            Material = p;

            


        }

        public override void Draw()
        {

         
            var pos = new Vector3(0, Length / 4, -Height / 2);

            // Floor Geometry
            MeshBuilder mesh = new MeshBuilder();

            mesh.AddBox(new Vector3(0, 0, -Height / 2), Length, Width, Height);

            Geometry = mesh.ToMeshGeometry3D();

            for (int i = 0; i < (Geometry as MeshGeometry3D).TextureCoordinates.Count; ++i)
            {
                (Geometry as MeshGeometry3D).TextureCoordinates[i] *= 2;
            }

        }


        private string SetFloor()
        {
            string path = "";

            switch (FloorNumber)
            {
                case "1":
                    path = "C:\\Users\\jacob\\source\\repos\\DimensionForge\\Textures\\granite.jpg";
                    break;
                    case "2":
                    path = "C:\\Users\\jacob\\source\\repos\\DimensionForge\\Textures\\wood.jpg";
                    break;
                    case "3":
                    path = "C:\\Users\\jacob\\source\\repos\\DimensionForge\\Textures\\blue.jpg";
                    break;
                    case "4":
                    path = "C:\\Users\\jacob\\source\\repos\\DimensionForge\\Textures\\sand.jpg";
                    break;
                    case "5": 
                    path = "C:\\Users\\jacob\\source\\repos\\DimensionForge\\Textures\\abstract.jpg";
                    break;
               
            }


            return path;

        }


        public override List<verletElement3D> GetElements()
        {
            return base.GetElements();
        }

    }
}
