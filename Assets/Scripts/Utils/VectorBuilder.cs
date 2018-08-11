using UnityEngine;

namespace Utils
{
    public class VectorBuilder
    {
        private readonly Vector3 _vector;

        private VectorBuilder(Vector3 vector)
        {
            _vector = vector;
        }

        public static VectorBuilder FromVector(Vector3 vector)
        {
            return new VectorBuilder(vector);
        }

        public VectorBuilder SetX(float x)
        {
            return new VectorBuilder(new Vector3(x, _vector.y, _vector.z));
        }

        public VectorBuilder SetY(float y)
        {
            return new VectorBuilder(new Vector3(_vector.x, y, _vector.z));
        }

        public VectorBuilder SetZ(float z)
        {
            return new VectorBuilder(new Vector3(_vector.x, _vector.y, z));
        }

        public Vector3 ToVector()
        {
            return new Vector3(_vector.x, _vector.y, _vector.z);
        }
    }
}