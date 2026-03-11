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
    /// Enum eStencilAction
    /// </summary>
    public enum eStencilAction : uint
	{
        /// <summary>
        /// Keep
        /// </summary>
        KEEP,
        /// <summary>
        /// Zero
        /// </summary>
        ZERO,
        /// <summary>
        ///Replace
        /// </summary>
        REPLACE,
        /// <summary>
        /// Increment
        /// </summary>
        INCREMENT,
        /// <summary>
        /// Decrement
        /// </summary>
        DECREMENT,
        /// <summary>
        /// Invert
        /// </summary>
        INVERT
    }
}
