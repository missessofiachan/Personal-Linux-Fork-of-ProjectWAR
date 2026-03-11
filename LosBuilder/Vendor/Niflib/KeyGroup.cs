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
    /// Class KeyGroup.
    /// </summary>
    /// <typeparam name="T">BaseKey</typeparam>
    public class KeyGroup<T> where T : BaseKey
	{
        /// <summary>
        /// The interpolation
        /// </summary>
        public eKeyType Interpolation;

        /// <summary>
        /// The values
        /// </summary>
        public T[] Values;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGroup{T}"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public KeyGroup(BinaryReader reader)
		{
			this.Values = new T[reader.ReadUInt32()];
			if (this.Values.Length != 0)
			{
				this.Interpolation = (eKeyType)reader.ReadUInt32();
			}
			for (int i = 0; i < this.Values.Length; i++)
			{
				this.Values[i] = (T)((object)Activator.CreateInstance(typeof(T), new object[]
				{
					reader,
					this.Interpolation
				}));
			}
		}
	}
}
