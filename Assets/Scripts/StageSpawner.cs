using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSpawner : MonoBehaviour
{
    [SerializeField] Transform _container;
    [SerializeField] MeshRenderer _groundPrefab;
    public float rows = 15;
    public float columns = 15;

    // Start is called before the first frame update
    void Start()
    {
        Material m = Instantiate(_groundPrefab.sharedMaterial);
        m.color = Color.gray;

        // spawn the grounds, every grid aside use different material
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject ground = Instantiate(_groundPrefab.gameObject,  _container);
                ground.transform.localPosition = new Vector3(i, 0, j);
                // ground.transform.localScale = new Vector3(1, 1, 1);
                if((i+j)%2 == 0) 
                    ground.GetComponent<MeshRenderer>().material = m;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
