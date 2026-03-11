namespace Niflib
{
    using System.IO;

    /// <summary>
    /// Class NiCollisionData.
    /// </summary>
    public class NiCollisionData : NiCollisionObject
    {
        /// <summary>
        /// The propagation mode.
        /// </summary>
        public uint PropagationMode;

        /// <summary>
        /// The collision mode flags.
        /// </summary>
        public uint CollisionMode;

        /// <summary>
        /// Indicates whether an alternate bounding volume is present.
        /// </summary>
        public bool UseAlternateBoundingVolume;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiCollisionData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiCollisionData(NiFile file, BinaryReader reader)
            : base(file, reader)
        {
            this.PropagationMode = reader.ReadUInt32();
            this.CollisionMode = reader.ReadUInt32();
            this.UseAlternateBoundingVolume = reader.ReadBoolean(file.Version);
            if (this.UseAlternateBoundingVolume)
                SkipBoundingVolume(reader);
        }

        private static void SkipBoundingVolume(BinaryReader reader)
        {
            uint boundType = reader.ReadUInt32();
            switch (boundType)
            {
                case 0:
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();
                    reader.ReadSingle();
                    break;
                case 1:
                    SkipVec3(reader);
                    SkipVec3(reader);
                    SkipVec3(reader);
                    SkipVec3(reader);
                    SkipVec3(reader);
                    break;
                case 2:
                    SkipVec3(reader);
                    SkipVec3(reader);
                    reader.ReadSingle();
                    reader.ReadSingle();
                    break;
                case 3:
                    break;
                case 4:
                    uint childCount = reader.ReadUInt32();
                    for (uint i = 0; i < childCount; i++)
                        SkipBoundingVolume(reader);
                    break;
                case 5:
                    SkipVec3(reader);
                    reader.ReadSingle();
                    SkipVec3(reader);
                    break;
            }
        }

        private static void SkipVec3(BinaryReader reader)
        {
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
        }
    }
}
