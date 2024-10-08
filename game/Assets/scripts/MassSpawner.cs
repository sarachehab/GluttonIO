using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class MassSpawner : MonoBehaviour
{
    #region instance 
    public static MassSpawner ins;

    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
    }
    #endregion

    public GameObject Mass;

    //should be renamed to Splits

    public List<GameObject> Players = new List<GameObject>();
    public List<GameObject> CreatedMasses = new List<GameObject>();


    // public List<Blob> FoodBlobs = new List<Blob>();
    public Dictionary<string, Blob> FoodDict = new Dictionary<string, Blob>();

    public int MaxMass = 50;
    public float Time_To_Instantiate = 0.5f;

    Map map;

    // TODO: rename to MaxSplits
    public int MaxPlayers = 16;
    public PlayerMovement playerMovement;

    public void Init(object msgData) {
        Dictionary <string, object> msgDataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(msgData.ToString());
        // Debug.Log("Init mass spawner: " + msgDataDict["foodBlobs"]);
        Dictionary <string, Blob> foodBlobs = JsonConvert.DeserializeObject<Dictionary<string, Blob>>(msgDataDict["foodBlobs"].ToString());

        foreach (KeyValuePair<string, Blob> foodBlob in foodBlobs) {
            AddFood(foodBlob.Value);
        }
    }


    private void Start()
    {
        playerMovement = PlayerMovement.instance;
        map = Map.ins;
    
    }

    public void AddFood(Blob foodBlob) {
        foodBlob.gameObject = Instantiate(
            Mass, 
            new Vector2(foodBlob.position.x, foodBlob.position.y), 
            Quaternion.identity
        );
        // FoodBlobs.Add(foodBlob);
        FoodDict.Add(foodBlob.id, foodBlob);

        // Randomise the color, but make it bright enough
        foodBlob.gameObject.GetComponent<SpriteRenderer>().color = new Color(
            Random.Range(0.5f, 1f), 
            Random.Range(0.5f, 1f), 
            Random.Range(0.5f, 1f)
        );
    }

    public void RemoveFoodBlobById(string foodBlobId) {
        if (FoodDict.ContainsKey(foodBlobId)) {
            Destroy(FoodDict[foodBlobId].gameObject);
            FoodDict.Remove(foodBlobId);
        }
    }

    public Transform MassPosition;

    public void AddThrownMass(string blobId, float speed, Vector3 direction, Vector2 startPos, Position endPos) {
        GameObject b = Instantiate(Mass, startPos, Quaternion.identity);
        float r = Blob.GetRadius(1);
        b.transform.localScale = new Vector3(r, r, r);
        
        b.GetComponent<MassForce>().ApplyForce = true;
        b.GetComponent<MassForce>().InitParams(speed, direction, startPos, blobId);
        FoodDict.Add(blobId, new Blob(blobId, 1, new Position(startPos.x, startPos.y), b));
    }


    // PREVIOUS STUFF______________________________________________________________
    // public IEnumerator CreateMass()
    // {
    //     // wait for seconds
    //     yield return new WaitForSecondsRealtime(Time_To_Instantiate);

    //     if (CreatedMasses.Count <= MaxMass)
    //     {
    //         Vector2 Position = new Vector2(Random.Range(-map.MapLimits.x, map.MapLimits.x), Random.Range(-map.MapLimits.y, map.MapLimits.y));
    //         Position /= 2;

    //         GameObject m = Instantiate(Mass, Position, Quaternion.identity);

    //         AddMass(m);

    //     }

    //     StartCoroutine(CreateMass());


    // }

    // public void AddMass(GameObject m)
    // {
    //     if (m != null && !m.Equals(null))
    //     {
    //         if (!CreatedMasses.Contains(m))
    //         {
    //             CreatedMasses.Add(m);

    //             for (int i = 0; i < Players.Count; i++)
    //             {
    //                 PlayerEatMass pp = Players[i]?.GetComponent<PlayerEatMass>();
    //                 if (pp != null)
    //                 {
    //                     pp.AddMass(m);
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Tried to add null or destroyed GameObject to mass list.");
    //     }
    // }


    // public void RemoveMass(GameObject m)
    // {
    //     if (CreatedMasses.Contains(m) == true)
    //     {
    //         Debug.LogWarning("Tried to add null or destroyed GameObject to mass list.");
    //     }
    // }


    // public void RemoveMass(GameObject m)
    // {
    //     if(CreatedMasses.Contains(m) == true)
    //     {
    //         CreatedMasses.Remove(m);


    //         for (int i = 0; i < Players.Count; i++)
    //         {
    //             PlayerEatMass pp = Players[i].GetComponent<PlayerEatMass>();
    //             pp.RemoveMass(m);
    //         }
    //     }
    // }

    // public void AddPlayer(GameObject b)
    // {
    //     if (Players.Contains(b) == false)
    //     {
    //         Players.Add(b);
    //     }
    // }

    // public void RemovePlayer(GameObject b)
    // {
    //     Debug.Log("RemovePlayer" + b.name);

    //     bool removed = Players.Remove(b);
    // }

}
