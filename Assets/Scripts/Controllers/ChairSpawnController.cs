using UnityEngine;
using Utils.Dev;
using Utils.Vector;

namespace Controllers
{
    public class ChairSpawnController : MonoBehaviour
    {
        [SerializeField] [ReadOnlyWhenPlaying] public GameObject Prefab;

        [SerializeField] [ReadOnlyWhenPlaying] public int CountZ = 3;
        [SerializeField] [ReadOnlyWhenPlaying] public int CountX = 3;
        private Renderer _renderer;

        private void Start()
        {
            if (Prefab == null)
            {
                Debug.LogError("Prefab is null");
                return;
            }

            _renderer = GetComponent<Renderer>();

            var chairs = new GameObject[CountX, CountZ];
            for (var iz = 0; iz < CountX; iz++)
            {
                for (var ix = 0; ix < CountZ; ix++)
                {
                    var x = _renderer.bounds.min.x + ix * _renderer.bounds.size.x / (CountZ - 1);
                    var z = _renderer.bounds.min.z + iz * _renderer.bounds.size.z / (CountX - 1);
                    Debug.DrawRay(new Vector3(x, 0, z), Vector3.up, Color.green);
                    chairs[iz, ix] = Instantiate(Prefab, new Vector3(x, 0, z), Quaternion.Euler(0, 90, 0));
                }
            }
        }

        private void Update()
        {
            Debug.DrawLine(
                _renderer.bounds.min,
                VectorBuilder.FromVector(_renderer.bounds.min).SetX(_renderer.bounds.max.x).ToVector(),
                Color.red,
                Time.deltaTime
            ); // bottom

            Debug.DrawLine(
                VectorBuilder.FromVector(_renderer.bounds.min)
                    .SetZ(_renderer.bounds.max.z)
                    .ToVector(),
                VectorBuilder.FromVector(_renderer.bounds.min)
                    .SetX(_renderer.bounds.max.x)
                    .SetZ(_renderer.bounds.max.z)
                    .ToVector(),
                Color.red,
                Time.deltaTime
            ); // top
            Debug.DrawLine(
                _renderer.bounds.min,
                VectorBuilder.FromVector(_renderer.bounds.min).SetZ(_renderer.bounds.max.z).ToVector(),
                Color.blue,
                Time.deltaTime
            ); // left

            Debug.DrawLine(
                _renderer.bounds.min,
                VectorBuilder.FromVector(_renderer.bounds.min).SetZ(_renderer.bounds.max.z).ToVector(),
                Color.blue,
                Time.deltaTime
            ); // left
            Debug.DrawLine(
                VectorBuilder.FromVector(_renderer.bounds.min)
                    .SetX(_renderer.bounds.max.x)
                    .ToVector(),
                VectorBuilder.FromVector(_renderer.bounds.min)
                    .SetX(_renderer.bounds.max.x)
                    .SetZ(_renderer.bounds.max.z)
                    .ToVector(),
                Color.blue,
                Time.deltaTime
            ); // right

            Debug.DrawLine(
                _renderer.bounds.min,
                VectorBuilder.FromVector(_renderer.bounds.min)
                    .SetX(_renderer.bounds.max.x)
                    .SetZ(_renderer.bounds.max.z)
                    .ToVector(),
                Color.magenta,
                Time.deltaTime
            ); // Diagonal
        }
    }
}
