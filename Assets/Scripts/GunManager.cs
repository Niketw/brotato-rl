using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] GameObject gunPrefab;

    Transform player;
    List<Vector2> gunPositions = new List<Vector2>();
    List<GameObject> spawnedGunObjects = new List<GameObject>(); // Add this to track guns

    int spawnedGuns = 0;
    
    public static GunManager Instance; // Add singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        player = GameObject.Find("Player").transform;

        gunPositions.Add(new Vector2(-1.2f, 1f));
        gunPositions.Add(new Vector2(1.2f, 1f));

        gunPositions.Add(new Vector2(-1f, -0.5f));
        gunPositions.Add(new Vector2(1f, -0.5f));

        AddGun();
        AddGun();
        AddGun();
        AddGun();
    }

    void AddGun()
    {
        var pos = gunPositions[spawnedGuns];
        var newGun = Instantiate(gunPrefab, pos, Quaternion.identity);

        newGun.GetComponent<Gun>().SetOffset(pos);
        spawnedGunObjects.Add(newGun); // Track the spawned gun
        spawnedGuns++;
    }

    public void DisableAllGuns()
    {
        foreach (var gun in spawnedGunObjects)
        {
            if (gun != null)
            {
                gun.SetActive(false);
            }
        }
    }
}
