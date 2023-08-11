using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnAI : MonoBehaviour
{
    [SerializeField] private GameObject _objectPrefab;
    [SerializeField] private GameObject _myRoad;
    [SerializeField] private Transform[] _spawnPositions;
    void Start()
    {
        for (int i = 0; i < _spawnPositions.Length; i++)
        {
            Instantiate(_objectPrefab, _spawnPositions[i].position, Quaternion.identity);
        }
        //Spawn();

    }

    // Функция для получения точек на дороге
    void Spawn()
    {
        MeshFilter[] childMeshFilters = _myRoad.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter roadMeshFilter in childMeshFilters)
        {

            //MeshFilter roadMeshFilter = GetComponent<MeshFilter>();
            Mesh roadMesh = roadMeshFilter.mesh;

            Vector3[] vertices = roadMesh.vertices;

            float roadLength = 0f;
            for (int i = 0; i < vertices.Length - 1; i++)
            {
                roadLength += Vector3.Distance(vertices[i], vertices[i + 1]);
            }

            for (int i = 0; i < 1; i++)
            {

                float randomDistance = Random.Range(0f, roadLength);

                float segmentDistance = 0f;
                int segmentIndex = 0;
                for (int j = 0; j < vertices.Length - 1; j++)
                {
                    float segmentLength = Vector3.Distance(vertices[j], vertices[j + 1]);
                    if (segmentDistance + segmentLength >= randomDistance)
                    {
                        segmentIndex = j;
                        break;
                    }
                    segmentDistance += segmentLength;
                }


                // Рахуєм відносну відстань внутрі сегмента
                float t = (randomDistance - segmentDistance) / Vector3.Distance(vertices[segmentIndex], vertices[segmentIndex + 1]);

                // Лінейна інтерполяція позиції на сегменті
                Vector3 spawnPosition = Vector3.Lerp(vertices[segmentIndex], vertices[segmentIndex + 1], t);

                spawnPosition = roadMeshFilter.gameObject.transform.TransformPoint(spawnPosition);
                Instantiate(_objectPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
