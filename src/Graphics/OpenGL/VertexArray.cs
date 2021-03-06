﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Substructio.Graphics.OpenGL
{
    public class VertexArray : BindeableObject
    {
        public ShaderProgram VAOProgram { get; private set; }
        public List<VertexBuffer> Buffers { get; private set; } 

        public PrimitiveType DrawPrimitiveType { get; set; }

        public VertexArray()
        {
            Create();
            DrawPrimitiveType = PrimitiveType.Triangles;
        }

        public void Load(ShaderProgram program, IEnumerable<VertexBuffer> buffers)
        {
            VAOProgram = program;
            Buffers = new List<VertexBuffer>(buffers);
            UpdateVertexAttributes();
        }

        public void UpdateVertexAttributes()
        {
            foreach (var vertexBuffer in Buffers)
            {
                vertexBuffer.Bind();
                foreach (var spec in vertexBuffer.DataSpecifications)
                {
                    GL.VertexAttribPointer(VAOProgram.AttributeLocation(spec.Name), spec.Count, spec.Type, spec.ShouldBeNormalised, spec.Stride, spec.Offset);
                    GL.EnableVertexAttribArray(VAOProgram.AttributeLocation(spec.Name));
                }
                //vertexBuffer.UnBind();
            }
            
        }

        public void Draw(double time)
        {
            Draw(time, 0, Buffers.Min(b => b.DrawableIndices));
        }

        public void Draw(double time, int start, int count, PrimitiveType? drawPrimitiveType = null)
        {
            Bind();

            var dpt = drawPrimitiveType ?? DrawPrimitiveType;

            foreach (var buffer in Buffers)
            {
                buffer.Bind();
            }
            GL.DrawArrays(dpt, start, count);
            foreach (var buffer in Buffers)
            {
                buffer.UnBind();
            }

            UnBind(); 
        }

        public override void Bind()
        {
            GL.BindVertexArray(ID);
        }

        public override void UnBind()
        {
            GL.BindVertexArray(0);
        }

        public override void Dispose()
        {
            UnBind();
            GL.DeleteVertexArray(ID);
        }

        public override void Create()
        {
            ID = GL.GenVertexArray();
        }

        public void Load(ShaderProgram program, params VertexBuffer[] buffers)
        {
            Load(program, (IEnumerable<VertexBuffer>)buffers);
        }
    }
}
