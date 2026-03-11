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
    /// Class Color4Key.
    /// </summary>
    public class Color4Key : BaseKey
    {
        /// <summary>
        /// The time
        /// </summary>
        public float Time;

        /// <summary>
        /// The value
        /// </summary>
        public Color4 Value;

        /// <summary>
        /// The forward
        /// </summary>
        public Color4 Forward;

        /// <summary>
        /// The backward
        /// </summary>
        public Color4 Backward;

        /// <summary>
        /// Initializes a new instance of the <see cref="Color4Key"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="Exception">Invalid eKeyType!</exception>
        public Color4Key(BinaryReader reader, eKeyType type) : base(reader, type)
        {
            this.Time = reader.ReadSingle();
            if (type < eKeyType.LINEAR || type > eKeyType.TBC)
            {
                throw new Exception("Invalid eKeyType!");
            }
            this.Value = reader.ReadColor4();
            if (type == eKeyType.QUADRATIC)
            {
                this.Forward = reader.ReadColor4();
                this.Backward = reader.ReadColor4();
            }
        }
    }
}
