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
	using System.Data;
	using System.IO;

    /// <summary>
    /// Class NiHeader.
    /// </summary>
    public class NiHeader
	{
        /// <summary>
        /// The version string
        /// </summary>
        public string VersionString;

        /// <summary>
        /// The version
        /// </summary>
        public eNifVersion Version = (eNifVersion)4294967295u;

        /// <summary>
        /// The user version
        /// </summary>
        public uint UserVersion;

        /// <summary>
        /// The user version2
        /// </summary>
        public uint UserVersion2;

        /// <summary>
        /// The little endian marker.
        /// </summary>
        public byte LittleEndian;

        /// <summary>
        /// The block types
        /// </summary>
        public NiString[] BlockTypes;

        /// <summary>
        /// The block type index
        /// </summary>
        public ushort[] BlockTypeIndex;

        /// <summary>
        /// The block sizes
        /// </summary>
        public uint[] BlockSizes;

        /// <summary>
        /// The header string table.
        /// </summary>
        public string[] StringTable;

        /// <summary>
        /// The number of strings in the header table.
        /// </summary>
        public uint NumStrings;

        /// <summary>
        /// The maximum string length in the header table.
        /// </summary>
        public uint MaxStringLength;

        /// <summary>
        /// The group sizes.
        /// </summary>
        public uint[] GroupSizes;

        /// <summary>
        /// The number of groups.
        /// </summary>
        public uint NumGroups;

        /// <summary>
        /// The number blocks
        /// </summary>
        public uint NumBlocks;

        /// <summary>
        /// The unkown int
        /// </summary>
        public uint UnkownInt;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiHeader"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="Exception">
        /// NIF Version not supported yet!
        /// or
        /// NIF Version not supported yet!
        /// or
        /// NIF Version not supported yet!
        /// or
        /// NIF Version not supported yet!
        /// or
        /// NIF Version not supported yet!
        /// </exception>
        /// <exception cref="VersionNotFoundException">Version 20.0.0.5 not supported!</exception>
        public NiHeader(NiFile file, BinaryReader reader)
		{
			int num = 0;
			long position = reader.BaseStream.Position;
			while (reader.ReadByte() != 10)
			{
				num++;
			}
			reader.BaseStream.Position = position;
			this.VersionString = new string(reader.ReadChars(num));
			reader.ReadByte();
			uint version = reader.ReadUInt32();
			this.Version = (eNifVersion)version;
            if (this.Version >= eNifVersion.v20_0_0_3)
            {
                this.LittleEndian = reader.ReadByte();
            }
            else
            {
                this.LittleEndian = 1;
            }
			if (this.Version >= eNifVersion.v10_1_0_0)
			{
				this.UserVersion = reader.ReadUInt32();
			}
			if (this.Version >= eNifVersion.v3_3_0_13)
			{
				this.NumBlocks = reader.ReadUInt32();
			}
			if (this.Version >= eNifVersion.v10_1_0_0 && (this.UserVersion == 10u || this.UserVersion == 11u))
			{
				this.UserVersion2 = reader.ReadUInt32();
			}
			if (this.Version == eNifVersion.v20_0_0_5)
			{
				throw new VersionNotFoundException("Version 20.0.0.5 not supported!");
			}
			if (this.Version == eNifVersion.v10_0_1_2)
			{
				throw new Exception("NIF Version not supported yet!");
			}
			if (this.Version >= eNifVersion.v10_0_1_0)
			{
				ushort num2 = reader.ReadUInt16();
				this.BlockTypes = new NiString[(int)num2];
				for (int i = 0; i < (int)num2; i++)
				{
					this.BlockTypes[i] = new NiString(file, reader, true);
				}
				this.BlockTypeIndex = new ushort[this.NumBlocks];
				int num3 = 0;
				while ((long)num3 < (long)((ulong)this.NumBlocks))
				{
					this.BlockTypeIndex[num3] = reader.ReadUInt16();
					num3++;
				}
			}
            if (this.Version >= eNifVersion.v20_2_0_5)
            {
                this.BlockSizes = new uint[this.NumBlocks];
                int blockIndex = 0;
                while ((long)blockIndex < (long)((ulong)this.NumBlocks))
                {
                    this.BlockSizes[blockIndex] = reader.ReadUInt32();
                    blockIndex++;
                }
            }
            if (this.Version >= eNifVersion.v20_1_0_1)
            {
                this.NumStrings = reader.ReadUInt32();
                this.MaxStringLength = reader.ReadUInt32();
                this.StringTable = new string[this.NumStrings];
                int stringIndex = 0;
                while ((long)stringIndex < (long)((ulong)this.NumStrings))
                {
                    this.StringTable[stringIndex] = NiString.ReadInlineString(reader);
                    stringIndex++;
                }
            }
            if (this.Version >= eNifVersion.v5_0_0_6)
            {
                this.NumGroups = reader.ReadUInt32();
                this.GroupSizes = new uint[this.NumGroups];
                int groupIndex = 0;
                while ((long)groupIndex < (long)((ulong)this.NumGroups))
                {
                    this.GroupSizes[groupIndex] = reader.ReadUInt32();
                    groupIndex++;
                }
            }
			if (this.Version >= eNifVersion.v10_0_1_0 && this.Version < eNifVersion.v20_1_0_1)
			{
				this.UnkownInt = reader.ReadUInt32();
			}
		}
	}
}
