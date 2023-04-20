using DimensionForge._3D.Data;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using Color = SharpDX.Color;
namespace DimensionForge._3D.Models
{
    public partial class Floor3D : Shape3D
    {
        public float Width {get; set;} = 1000;
        public float Length {get; set;} = 1000;
        public float Height {get; set;} = 0.1f;
       

        public Floor3D()
        {
            //Init
           
            Name = "Floor";
            Id = Guid.NewGuid().ToString();
            PhongMaterial p = PhongMaterials.Red;

           
            p.DiffuseMap = new TextureModel("C:\\Users\\jacob\\Downloads\\pexels-jonathan-borba-5489194.jpg");
            p.RenderDiffuseMap = true;
            p.DiffuseColor = SharpDX.Color.DimGray.ToColor4();        
            Material = p;



            Draw();


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
                (Geometry as MeshGeometry3D).TextureCoordinates[i] *= 5;
            }


        }

    }
}
