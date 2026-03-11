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
    /// Class NiCamera.
    /// </summary>
    public class NiCamera : NiAVObject
	{
        /// <summary>
        /// The unkown1
        /// </summary>
        public ushort Unkown1;

        /// <summary>
        /// The frustrum left
        /// </summary>
        public float FrustrumLeft;

        /// <summary>
        /// The frustrum right
        /// </summary>
        public float FrustrumRight;

        /// <summary>
        /// The frustrum top
        /// </summary>
        public float FrustrumTop;

        /// <summary>
        /// The frustrum bottom
        /// </summary>
        public float FrustrumBottom;

        /// <summary>
        /// The frustrum near
        /// </summary>
        public float FrustrumNear;

        /// <summary>
        /// The frustrum far
        /// </summary>
        public float FrustrumFar;

        /// <summary>
        /// The use orthographics projection
        /// </summary>
        public bool UseOrthographicsProjection;

        /// <summary>
        /// The viewport left
        /// </summary>
        public float ViewportLeft;

        /// <summary>
        /// The viewport right
        /// </summary>
        public float ViewportRight;

        /// <summary>
        /// The viewport top
        /// </summary>
        public float ViewportTop;

        /// <summary>
        /// The viewport bottom
        /// </summary>
        public float ViewportBottom;

        /// <summary>
        /// The lod adjust
        /// </summary>
        public float LODAdjust;

        /// <summary>
        /// The unkown link
        /// </summary>
        public NiRef<NiObject> UnkownLink;

        /// <summary>
        /// The unkown2
        /// </summary>
        public uint Unkown2;

        /// <summary>
        /// The unkown3
        /// </summary>
        public uint Unkown3;

        /// <summary>
        /// The unkown4
        /// </summary>
        public uint Unkown4;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiCamera"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiCamera(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (this.File.Header.Version >= eNifVersion.v10_1_0_0)
			{
				this.Unkown1 = reader.ReadUInt16();
			}
			this.FrustrumLeft = reader.ReadSingle();
			this.FrustrumRight = reader.ReadSingle();
			this.FrustrumTop = reader.ReadSingle();
			this.FrustrumBottom = reader.ReadSingle();
			this.FrustrumNear = reader.ReadSingle();
			this.FrustrumFar = reader.ReadSingle();
			if (this.File.Header.Version >= eNifVersion.v10_1_0_0)
			{
				this.UseOrthographicsProjection = reader.ReadBoolean(Version);
			}
			this.ViewportLeft = reader.ReadSingle();
			this.ViewportRight = reader.ReadSingle();
			this.ViewportTop = reader.ReadSingle();
			this.ViewportBottom = reader.ReadSingle();
			this.LODAdjust = reader.ReadSingle();
			this.UnkownLink = new NiRef<NiObject>(reader);
			this.Unkown2 = reader.ReadUInt32();
			if (this.File.Header.Version >= eNifVersion.v4_2_1_0)
			{
				this.Unkown3 = reader.ReadUInt32();
			}
			if (this.File.Header.Version <= eNifVersion.v3_1)
			{
				this.Unkown4 = reader.ReadUInt32();
			}
		}
	}
}
