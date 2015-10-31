using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public class Program : IDisposable
    {
        public int ProgramId { private set; get; }
        private Dictionary<string, int> variables = new Dictionary<string, int>();

        public Program()
        {
            ProgramId = GL.CreateProgram();
        }

        public void Use()
        {
            GLStates.UseProgram(ProgramId);
        }

        public void SetVariable(string name, float x)
        {
            if (ProgramId > 0)
            {
                GLStates.UseProgram(ProgramId);
                int location = GetUniformLocation(name);
                if (location != -1)
                    GL.Uniform1(location, x);
            }
        }

        public void SetVariable(string name, int x)
        {
            if (ProgramId > 0)
            {
                GLStates.UseProgram(ProgramId);
                int location = GetUniformLocation(name);
                if (location != -1)
                    GL.Uniform1(location, x);
            }
        }

        public void SetVariable(string name, Matrix4 matrix)
        {
            if (ProgramId > 0)
            {
                GLStates.UseProgram(ProgramId);
                int location = GetUniformLocation(name);
                if (location != -1)
                    GL.UniformMatrix4(location, false, ref matrix);
            }
        }

        public void SetVariable(string name, Matrix2 matrix)
        {
            if (ProgramId > 0)
            {
                GLStates.UseProgram(ProgramId);
                int location = GetUniformLocation(name);
                if (location != -1)
                    GL.UniformMatrix2(location, false, ref matrix);
            }
        }

        public void SetVariable(string name, Vector2 x)
        {
            if (ProgramId > 0)
            {
                GLStates.UseProgram(ProgramId);
                int location = GetUniformLocation(name);
                if (location != -1)
                    GL.Uniform2(location, x);
            }
        }

        public int GetUniformLocation(string name)
        {
            if (variables.ContainsKey(name))
                return variables[name];

            int location = GL.GetUniformLocation(ProgramId, name);

            if (location != -1)
                variables.Add(name, location);
            else
                throw new Exception("Failed To Get Uniform Location " + name);

            return location;
        }

        public int GetAttribLocation(string name)
        {
            if (variables.ContainsKey(name))
                return variables[name];

            int location = GL.GetAttribLocation(ProgramId, name);

            if (location != -1)
                variables.Add(name, location);
            else
                throw new Exception("Failed To Get Attribute Location " + name);

            return location;
        }

        

        public void AddShader(ShaderType shaderType, string source)
        {
            string infoLog = "";
            int statusCode = -1;

            int shaderId = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderId, source);
            GL.CompileShader(shaderId);
            GL.GetShaderInfoLog(shaderId, out infoLog);
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                GL.DeleteShader(shaderId);
                GL.DeleteProgram(ProgramId);
                ProgramId = 0;

                throw new Exception("Failed to Compile Shader Source." + Environment.NewLine + infoLog + Environment.NewLine + "Status Code: " + statusCode.ToString());
            }
            else
            {
                GL.AttachShader(ProgramId, shaderId);
                GL.DeleteShader(shaderId);
            }

            GL.LinkProgram(ProgramId);
            GL.GetProgramInfoLog(ProgramId, out infoLog);
            GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out statusCode);

            if (statusCode != 1)
            {
                GL.DeleteProgram(ProgramId);
                ProgramId = 0;

                throw new Exception("Failed to Link Shader Program." + Environment.NewLine + infoLog + Environment.NewLine + "Status Code: " + statusCode.ToString());
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GL.DeleteProgram(ProgramId);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Program() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
