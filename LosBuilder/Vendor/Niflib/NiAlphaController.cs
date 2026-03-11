namespace Niflib
{
    using System;
    using System.IO;
#if OpenTK
	using OpenTK;
	using OpenTK.Graphics;
	using Matrix = OpenTK.Matrix4;
	using Color3 = OpenTK.Graphics.Color4;
#elif SharpDX
	using SharpDX;
#elif MonoGame
	using Microsoft.Xna.Framework;
	using Color3 = Microsoft.Xna.Framework.Color;
	using Color4 = Microsoft.Xna.Framework.Color;
#else
    using System.Numerics;
    using Matrix = System.Numerics.Matrix4x4;
    using Color3 = System.Numerics.Vector3;
    using Color4 = System.Numerics.Vector4;
#endif

    /// <summary>
    /// Class NiAlphaController.
    /// </summary>
    public class NiAlphaController : NiFloatInterpController
	{
        /// <summary>
        /// The data
        /// </summary>
        public NiRef<NiFloatData> Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiAlphaController" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiAlphaController(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (this.File.Header.Version <= eNifVersion.v10_1_0_0)
			{
				this.Data = new NiRef<NiFloatData>(reader);
			}
		}
	}
}
