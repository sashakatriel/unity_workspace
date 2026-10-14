using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree_plant : MonoBehaviour
{

    public GameObject tree_prefab;
    public bool plant = false;
    public bool clear = false;


    public Vector3 plant_scale = new Vector3(10, 10, 10);
    public Vector3 plant_center = new Vector3(0, 0, 0);

    public float step_size = 1.0f;
    public float plant_chance = 30.0f;

    public float min_tree_scale = 0.4f;
    public float max_tree_scale = 1.4f;



    public void ClearPlants()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //Destroy(transform.GetChild(i).gameObject);
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void CreatePlants()
    {
        for (float x = 0; x < plant_scale.x*2; x += step_size)
        {
            for (float z = 0; z < plant_scale.z*2; z += step_size)
            {
                float r = Random.Range(0, 100);
                if (r <= plant_chance)
                {
                    Vector3 point = transform.position + plant_center - new Vector3(plant_scale.x / 2, 0, plant_scale.z / 2) + new Vector3(x, plant_scale.y/2, z);

                    RaycastHit hit;
                    if (Physics.Raycast(point, -Vector3.up, out hit, plant_scale.y*2))
                    {
                        if (hit.collider.tag == "Landscape")
                        {
                            var tree = GameObject.Instantiate(tree_prefab, hit.point, tree_prefab.transform.rotation, transform);
                            tree.transform.eulerAngles += Vector3.up * Random.Range(0.0f, 360.0f);
                            tree.transform.localScale *= Mathf.Lerp(min_tree_scale, max_tree_scale, Random.Range(0.0f, 1.0f));
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + plant_center, plant_scale);

        if (plant)
        {
            ClearPlants();
            plant = false;
            CreatePlants();
        }

        if (clear)
        {
            ClearPlants();
            clear = false;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
