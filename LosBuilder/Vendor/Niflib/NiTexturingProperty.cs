/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

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
    /// Class NiTexturingProperty.
    /// </summary>
    public class NiTexturingProperty : NiProperty
    {
        /// <summary>
        /// The flags
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// The apply mode
        /// </summary>
        public uint ApplyMode;

        /// <summary>
        /// The texture count
        /// </summary>
        public uint TextureCount;

        /// <summary>
        /// The base texture
        /// </summary>
        public TexDesc BaseTexture;

        /// <summary>
        /// The dark texture
        /// </summary>
        public TexDesc DarkTexture;

        /// <summary>
        /// The detail texture
        /// </summary>
        public TexDesc DetailTexture;

        /// <summary>
        /// The gloss texture
        /// </summary>
        public TexDesc GlossTexture;

        /// <summary>
        /// The glow texture
        /// </summary>
        public TexDesc GlowTexture;

        /// <summary>
        /// The bump map texture
        /// </summary>
        public TexDesc BumpMapTexture;

        /// <summary>
        /// The normal texture.
        /// </summary>
        public TexDesc NormalTexture;

        /// <summary>
        /// The parallax texture.
        /// </summary>
        public TexDesc ParallaxTexture;

        /// <summary>
        /// The parallax offset.
        /// </summary>
        public float ParallaxOffset;

        /// <summary>
        /// The decal0 texture
        /// </summary>
        public TexDesc Decal0Texture;

        /// <summary>
        /// The decal1 texture
        /// </summary>
        public TexDesc Decal1Texture;

        /// <summary>
        /// The decal2 texture
        /// </summary>
        public TexDesc Decal2Texture;

        /// <summary>
        /// The decal3 texture
        /// </summary>
        public TexDesc Decal3Texture;

        /// <summary>
        /// The unkown1
        /// </summary>
        public uint Unkown1;

        /// <summary>
        /// The bump map luma scale
        /// </summary>
        public float BumpMapLumaScale;

        /// <summary>
        /// The bump map luma offset
        /// </summary>
        public float BumpMapLumaOffset;

        /// <summary>
        /// The bump map matrix
        /// </summary>
        public Vector3 BumpMapMatrix;

        /// <summary>
        /// The number shader textures
        /// </summary>
        public uint NumShaderTextures;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiTexturingProperty"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiTexturingProperty(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if (base.Version <= eNifVersion.v10_0_1_2 || base.Version >= eNifVersion.v20_1_0_2)
            {
                this.Flags = reader.ReadUInt16();
            }
            if (base.Version <= eNifVersion.v20_0_0_5)
            {
                this.ApplyMode = reader.ReadUInt32();
            }
            this.TextureCount = reader.ReadUInt32();
            if (reader.ReadBoolean(Version))
            {
                this.BaseTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                this.DarkTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                this.DetailTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                this.GlossTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                this.GlowTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                this.BumpMapTexture = new TexDesc(file, reader);
                this.BumpMapLumaScale = reader.ReadSingle();
                this.BumpMapLumaOffset = reader.ReadSingle();
                this.BumpMapMatrix = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                reader.ReadSingle();
            }
            uint numTextures = this.TextureCount;
            if (numTextures > 6 && reader.ReadBoolean(Version))
                this.NormalTexture = new TexDesc(file, reader);

            if (numTextures > 7 && base.Version >= eNifVersion.v20_2_0_5)
            {
                if (reader.ReadBoolean(Version))
                {
                    this.ParallaxTexture = new TexDesc(file, reader);
                    this.ParallaxOffset = reader.ReadSingle();
                }
            }
            else
            {
                numTextures++;
            }

            if (numTextures > 8 && reader.ReadBoolean(Version))
                this.Decal0Texture = new TexDesc(file, reader);

            if (numTextures > 9 && reader.ReadBoolean(Version))
                this.Decal1Texture = new TexDesc(file, reader);

            if (numTextures > 10 && reader.ReadBoolean(Version))
                this.Decal2Texture = new TexDesc(file, reader);

            if (numTextures > 11 && reader.ReadBoolean(Version))
                this.Decal3Texture = new TexDesc(file, reader);

            if (base.Version >= eNifVersion.v10_0_1_0)
            {
                this.NumShaderTextures = reader.ReadUInt32();
                int num = 0;
                while ((long)num < (long)((ulong)this.NumShaderTextures))
                {
                    if (reader.ReadBoolean(Version))
                    {
                        new TexDesc(file, reader);
                        reader.ReadUInt32();
                    }
                    num++;
                }
            }
        }
    }
}
