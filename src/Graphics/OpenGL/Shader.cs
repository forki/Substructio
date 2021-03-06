﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Substructio.Graphics.OpenGL
{
    public class Shader : BindeableObject
    {
        public ShaderType ShaderType { get; private set; }
        public string Source { get; private set; }
        public string Name { get; private set; }

        public Shader(string path, string version = "")
        {
            Load(path, version);
        }

        public Shader(string path, ShaderType type)
        {
            Load(path, type);
        }

        public Shader(string source, string name, ShaderType type)
        {
            Load(source, name, type);
        }

        public void Load(string path, ShaderType type, string version = "")
        {
            var source = IO.ASCIIFileHelper.ReadFileToEnd(path);
            source = version + "\n" + source;
            var name = Path.GetFileNameWithoutExtension(path);
            Load(source, name, type);
        }

        public void Load(string path, string version = "")
        {
            var ext = Path.GetExtension(path);
            ShaderType type;
            switch (ext)
            {
                case ".fs":
                    type = ShaderType.FragmentShader;
                    break;
                case ".vs":
                    type = ShaderType.VertexShader;
                    break;
                case ".gs":
                    type = ShaderType.GeometryShader;
                    break;
                default:
                    throw new Exception("Unknown shader file specified");
            }

            Load(path, type, version);
        }

        public void Load(string source, string name, ShaderType type)
        {
            Source = source;
            ShaderType = type;
            Name = name;

            Create();
            GL.ShaderSource(ID, Source);
            GL.CompileShader(ID);
        }

        public override void Bind()
        {
            throw new Exception("Shaders can not be bound");
        }

        public override void UnBind()
        {
            throw new Exception("Shaders can not be bound");
        }

        public override void Dispose()
        {
            GL.DeleteShader(ID);
        }

        public override void Create()
        {
            ID = GL.CreateShader(ShaderType);
        }
    }
}
