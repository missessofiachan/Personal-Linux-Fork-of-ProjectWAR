using System.Numerics;

namespace UnexpectedBytes.War.NIF
{
    internal static class Matrix4x4Extensions
    {
        public static Matrix4x4 MultiplyInto(this Matrix4x4 left, Matrix4x4 right, out Matrix4x4 result)
        {
            result = Matrix4x4.Multiply(left, right);
            return result;
        }

        public static Vector3 TransformInto(this Vector3 value, Matrix4x4 matrix, out Vector3 result)
        {
            result = Vector3.Transform(value, matrix);
            return result;
        }
    }
}
