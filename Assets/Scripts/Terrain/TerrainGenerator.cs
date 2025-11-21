using UnityEngine;
using System.Collections.Generic;

namespace TanxClone.Terrain
{
    /// <summary>
    /// Generates random destructible terrain for the game
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
    public class TerrainGenerator : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private PolygonCollider2D polygonCollider;
        private List<Vector2> terrainPoints = new List<Vector2>();

        [Header("Terrain Settings")]
        [SerializeField] private Material terrainMaterial;
        [SerializeField] private Color terrainColor = new Color(0.4f, 0.3f, 0.2f);

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            polygonCollider = GetComponent<PolygonCollider2D>();
        }

        /// <summary>
        /// Generate terrain based on landscape type
        /// </summary>
        public void GenerateTerrain(LandscapeType type)
        {
            terrainPoints.Clear();

            switch (type)
            {
                case LandscapeType.Mountains:
                    GenerateMountains();
                    break;
                case LandscapeType.FootHills:
                    GenerateFootHills();
                    break;
                case LandscapeType.Random:
                    GenerateRandomTerrain();
                    break;
            }

            BuildTerrainMesh();
            UpdateCollider();
        }

        private void GenerateMountains()
        {
            int points = GameSettings.TERRAIN_POINTS;
            float width = GameSettings.LANDSCAPE_WIDTH;
            float height = GameSettings.LANDSCAPE_HEIGHT;
            float segmentWidth = width / (points - 1);

            // Start from bottom left
            terrainPoints.Add(new Vector2(0, -height));

            // Generate steep mountains with large valleys
            for (int i = 0; i < points; i++)
            {
                float x = i * segmentWidth;
                float y = Random.Range(height * 0.1f, height * 0.6f);

                // Add some peaks
                if (i % 5 == 0)
                {
                    y = Random.Range(height * 0.4f, height * 0.7f);
                }

                terrainPoints.Add(new Vector2(x, y));
            }

            // Bottom right corner
            terrainPoints.Add(new Vector2(width, -height));
        }

        private void GenerateFootHills()
        {
            int points = GameSettings.TERRAIN_POINTS;
            float width = GameSettings.LANDSCAPE_WIDTH;
            float height = GameSettings.LANDSCAPE_HEIGHT;
            float segmentWidth = width / (points - 1);

            terrainPoints.Add(new Vector2(0, -height));

            // Generate shallow valleys with small hills
            for (int i = 0; i < points; i++)
            {
                float x = i * segmentWidth;
                float y = Random.Range(height * 0.2f, height * 0.4f);
                terrainPoints.Add(new Vector2(x, y));
            }

            terrainPoints.Add(new Vector2(width, -height));
        }

        private void GenerateRandomTerrain()
        {
            int points = GameSettings.TERRAIN_POINTS;
            float width = GameSettings.LANDSCAPE_WIDTH;
            float height = GameSettings.LANDSCAPE_HEIGHT;
            float segmentWidth = width / (points - 1);

            terrainPoints.Add(new Vector2(0, -height));

            // Completely random terrain
            for (int i = 0; i < points; i++)
            {
                float x = i * segmentWidth;
                float y = Random.Range(-height * 0.2f, height * 0.8f);
                terrainPoints.Add(new Vector2(x, y));
            }

            terrainPoints.Add(new Vector2(width, -height));
        }

        private void BuildTerrainMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Terrain Mesh";

            // Create vertices from terrain points
            Vector3[] vertices = new Vector3[terrainPoints.Count];
            for (int i = 0; i < terrainPoints.Count; i++)
            {
                vertices[i] = new Vector3(terrainPoints[i].x, terrainPoints[i].y, 0);
            }

            // Create triangles (simple fan triangulation from first vertex)
            List<int> triangles = new List<int>();
            for (int i = 1; i < terrainPoints.Count - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }

            // Set up UVs
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x / GameSettings.LANDSCAPE_WIDTH,
                                     (vertices[i].y + GameSettings.LANDSCAPE_HEIGHT) / (GameSettings.LANDSCAPE_HEIGHT * 2));
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.mesh = mesh;

            // Set material
            if (terrainMaterial != null)
            {
                GetComponent<MeshRenderer>().material = terrainMaterial;
            }
        }

        private void UpdateCollider()
        {
            polygonCollider.points = terrainPoints.ToArray();
        }

        /// <summary>
        /// Deform terrain at a specific point (for explosions)
        /// </summary>
        public void DeformTerrain(Vector2 position, float radius)
        {
            bool modified = false;

            for (int i = 1; i < terrainPoints.Count - 1; i++)
            {
                float distance = Vector2.Distance(terrainPoints[i], position);

                if (distance < radius)
                {
                    // Lower the terrain point based on distance
                    float factor = 1f - (distance / radius);
                    float offset = radius * factor * 0.5f;
                    terrainPoints[i] = new Vector2(terrainPoints[i].x, terrainPoints[i].y - offset);
                    modified = true;
                }
            }

            if (modified)
            {
                BuildTerrainMesh();
                UpdateCollider();
            }
        }

        /// <summary>
        /// Get terrain height at a specific x position
        /// </summary>
        public float GetTerrainHeightAt(float x)
        {
            for (int i = 0; i < terrainPoints.Count - 1; i++)
            {
                if (x >= terrainPoints[i].x && x <= terrainPoints[i + 1].x)
                {
                    // Linear interpolation between two points
                    float t = (x - terrainPoints[i].x) / (terrainPoints[i + 1].x - terrainPoints[i].x);
                    return Mathf.Lerp(terrainPoints[i].y, terrainPoints[i + 1].y, t);
                }
            }
            return 0f;
        }

        /// <summary>
        /// Check if a position is on solid terrain
        /// </summary>
        public bool IsOnTerrain(Vector2 position)
        {
            float terrainHeight = GetTerrainHeightAt(position.x);
            return position.y <= terrainHeight + 0.5f; // Small tolerance
        }

        /// <summary>
        /// Get list of terrain points for object placement
        /// </summary>
        public List<Vector2> GetTerrainPoints()
        {
            return new List<Vector2>(terrainPoints);
        }
    }
}
