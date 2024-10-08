using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

// using ServerConnect;

public class PlayerMovement : MonoBehaviour
{
    //==========================================================================
    // Fields
    //==========================================================================
    
    const int MsgInterval = 20;
    const int StartingSize = 30;

    public Blob blob;
    private Actions actions;
    private Map map;
    private ServerConnect server;
    private MassSpawner massSpawner;
    private GameObject[] Mass;
    private FpgaController fpgaController;
    private PlayersManager playersManager;
    private int msgCount = 0;

    public bool LockActions = false;
    public float Speed = 10f;
    public Vector3 Direction;

    public bool Died = false;

    public bool ChangesOccurLocally;

    #region Instance
    public static PlayerMovement instance { get; private set; } // Singleton instance


    //==========================================================================
    // Methods
    //==========================================================================

    public bool isMac = false;
    void Awake()
    {
        try {
            fpgaController = new FpgaController();
            Debug.Log("Using Windows with FPGA");
        } catch {
            Debug.Log("Using Mac without FPGA");
            isMac = true;
        }

        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        blob = new Blob("0", StartingSize, new Position(0, 0), gameObject);
        float r = Blob.GetRadius(StartingSize);
        transform.localScale = new Vector3(r, r, r);

        map = Map.ins;
        server = ServerConnect.instance;
        actions = GetComponent<Actions>();
        massSpawner = MassSpawner.ins;
        ChangesOccurLocally = true;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    void UpdateMac()
    {
        float Speed_ = (float) blob.GetSpeed() * 2;
        Vector2 Direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Direction.x = Mathf.Clamp(Direction.x, map.MapLimits.x * -1 / 2, map.MapLimits.x / 2);
        Direction.y = Mathf.Clamp(Direction.y, map.MapLimits.y * -1 / 2, map.MapLimits.y / 2);


        // Update blob's position
        transform.position = Vector2.MoveTowards(transform.position, Direction, Speed_ * Time.deltaTime);
        blob.position.x = transform.position.x;
        blob.position.y = transform.position.y;
        

        // Send message to server
        if (msgCount % MsgInterval == 0)
        {
            Dictionary<string, object> updatePlayerPosMsg = new Dictionary<string, object> {
                {"x", transform.position.x},
                {"y", transform.position.y}
            };

            var UpdatePosMsg = new ClientMessage(
                ClientMsgType.UpdatePosition, 
                updatePlayerPosMsg
            );

            server.SendWsMessage(UpdatePosMsg).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError($"Error sending message: {task.Exception}");
                }
            });
        }

        msgCount++;


        if (LockActions)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            actions.ThrowMass(Direction);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMac)
        {
            UpdateMac();
            return;
        }

        fpgaController.UpdateData();

        float accel_x = fpgaController.GetReadingX();
        float accel_y = fpgaController.GetReadingY();
        int switches = fpgaController.GetSwitchValue();
        int throwMass = fpgaController.GetKey0();
        int split = fpgaController.GetKey1();

       // Debug.Log("Input " + accel_x + " " + accel_y + " " + switches + " " + throwMass + " " + split);
    
        Direction = new Vector3(accel_x*1.5f, accel_y*1.5f, 0);

        // The magnitude of the direction vector does not affect the speed in
        // the MoveTowards function, so we have to calculate the speed manually
        float Speed_ = Speed * Direction.magnitude / transform.localScale.x;

        // Update the position of the player
        transform.position += Direction * Speed_ * Time.deltaTime;

        // Clamp the player position to the map limits
        transform.position = new Vector3(
            Mathf.Clamp(
                transform.position.x, 
                map.MapLimits.x * -1 / 2, 
                map.MapLimits.x / 2
            ),
            Mathf.Clamp(
                transform.position.y, 
                map.MapLimits.y * -1 / 2, 
                map.MapLimits.y / 2
            ),
            transform.position.z
        );

        // Send message to the server
        if (msgCount % MsgInterval == 0)
        {
            Dictionary<string, object> updatePlayerPosMsg = new Dictionary<string, object> {
                {"x", transform.position.x},
                {"y", transform.position.y}
            };

            var UpdatePosMsg = new ClientMessage(
                ClientMsgType.UpdatePosition, 
                updatePlayerPosMsg
            );

            server.SendWsMessage(UpdatePosMsg).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError($"Error sending message: {task.Exception}");
                }
            });
        }

        msgCount++;


        if (LockActions)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            actions.ThrowMass(Direction);
        }

        if (LockActions)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) || split == 1)
        {
            actions.ThrowMass(Direction);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    void OnDestroy()
    {
        // Camera.main.GetComponent<CamerFollow>().RemovePlayerFromTrack(transform);
    }
}

