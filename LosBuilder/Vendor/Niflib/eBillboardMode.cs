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
    /// Enum eBillboardMode
    /// </summary>
    public enum eBillboardMode : ushort
    {
        /// <summary>
        /// Always face camera
        /// </summary>
        ALWAYS_FACE_CAMERA,
        /// <summary>
        /// Rotate about up axis
        /// </summary>
        ROTATE_ABOUT_UP,
        /// <summary>
        /// Rigid face camera
        /// </summary>
        RIGID_FACE_CAMERA,
        /// <summary>
        /// Always face center
        /// </summary>
        ALWAYS_FACE_CENTER,
        /// <summary>
        /// Rigid face center
        /// </summary>
        RIGID_FACE_CENTER,
        /// <summary>
        /// Rotate about up axis 2
        /// </summary>
        ROTATE_ABOUT_UP2 = 9
    }
}
