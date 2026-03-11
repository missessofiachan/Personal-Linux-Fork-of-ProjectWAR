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
    /// Class VecKey.
    /// </summary>
    public class VecKey : BaseKey
    {
        /// <summary>
        /// The time
        /// </summary>
        public float Time;

        /// <summary>
        /// The value
        /// </summary>
        public Vector3 Value;

        /// <summary>
        /// The forward
        /// </summary>
        public Vector3 Forward;

        /// <summary>
        /// The backward
        /// </summary>
        public Vector3 Backward;

        /// <summary>
        /// The TBC
        /// </summary>
        public Vector3 TBC;

        /// <summary>
        /// Initializes a new instance of the <see cref="VecKey"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="Exception">Invalid eKeyType!</exception>
        public VecKey(BinaryReader reader, eKeyType type) : base(reader, type)
        {
            this.Time = reader.ReadSingle();
            if (type < eKeyType.LINEAR || type > eKeyType.TBC)
            {
                throw new Exception("Invalid eKeyType!");
            }
            if (type == eKeyType.LINEAR)
            {
                this.Value = reader.ReadVector3();
            }
            if (type == eKeyType.QUADRATIC)
            {
                this.Value = reader.ReadVector3();
                this.Forward = reader.ReadVector3();
                this.Backward = reader.ReadVector3();
            }
            if (type == eKeyType.TBC)
            {
                this.Value = reader.ReadVector3();
                this.TBC = reader.ReadVector3();
            }
        }
    }
}
