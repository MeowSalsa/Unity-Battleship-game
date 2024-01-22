using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIBoardManager : MonoBehaviour
{
	
    public delegate void ChangeShip(int id, int size);
    public static event ChangeShip OnChangeShip;

    public delegate void ChangeShipOrientation();
    public static event ChangeShipOrientation OnChangeShipOrientation;

    public delegate void PlayerSelected(PlayerType firstPlayerID);
    public static event PlayerSelected  OnFirstPlayerSelected;

    

    public List<Button> collectionOfShipButtons = new List<Button>(5);
    public Button OrientationButton;
    private bool horizontal = false;
    [SerializeField] private GameObject ShipSelectionPanel;

    [SerializeField]private List<Button> playerSelectionButtons;
    [SerializeField] private GameObject PlayerSelectionPanel;


    //Horizontal + Vertical textures
    [SerializeField]
    private Sprite HorizontalTexture;
    [SerializeField]
    private Sprite VerticalTexture;
    [SerializeField] TMP_Text playerScoreText;
    [SerializeField] TMP_Text AIScoreText;

    Dictionary<int, int> ships = new Dictionary<int, int>() {
        {0,5}, //Aircraft Carrier
        {1,4}, //Battleship
        {2,3}, //Submarine
        {3,3}, //Destroyer
        {4,2}, //Patrol Boat
    };

    private void OnEnable()
    {
        BoardManager.OnBoardPiecePlaced += OnBoardPiecePlaced;
        BoardManager.OnStartGame += OnStartGame;
        Battleship.OnPlayerScored += OnScoreUpdate;
    }
    private void OnDisable()
    {
        BoardManager.OnBoardPiecePlaced -= OnBoardPiecePlaced;
        BoardManager.OnStartGame -= OnStartGame;
        Battleship.OnPlayerScored -= OnScoreUpdate;
    }

    private void Start()
    {   OrientationButton.gameObject.SetActive(true);

        UpdateOrientationUI();
        playerScoreText.text = "Player Score: 0";
        AIScoreText.text = "AI Score: 0";
    }
    private void OnStartGame(bool startGame)
    {
        if (startGame)
        {
            ShipSelectionPanel.SetActive(false);
            PlayerSelectionPanel.SetActive(true); 
        }
    }
	public void UI_OnPlayerSelected(int playerID){
        PlayerType player = (PlayerType)playerID;
        Debug.Log($"Player selected was {player}");
        //Broadcast messgae of player selected.
        OnFirstPlayerSelected?.Invoke(player);
        PlayerSelectionPanel.SetActive(false);
    }

    private void OnBoardPiecePlaced(int shipID)
    {
        Debug.Log("Disactivating Button Number " + shipID);
        collectionOfShipButtons[shipID].gameObject.SetActive(false);
        if (shipID < 4)
        {
            NextShip(shipID);
        }
    }
    private void NextShip(int shipID)
    {
        int newShipID = shipID + 1;
        OnChangeShip?.Invoke(newShipID, ships[newShipID]);
        Debug.Log("Invoking");
    }
    public void OnShipButtonClick(int id)
    {
        Debug.Log("ID " + id);
        OnChangeShip?.Invoke(id, ships[id]);
        string idString = id.ToString();
        Debug.Log("Pressed button " + idString);
    }

    public void OnOrientationClick()
    {
        Debug.Log("Rotation Floatation Motation!");
        UpdateOrientationUI();
        OnChangeShipOrientation?.Invoke();

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnOrientationClick();
            Debug.Log("Rotating Ship! W keypress");
        }
    }
    void UpdateOrientationUI()
    {
        Debug.Log("Initial Rotation " + horizontal);
        horizontal = !horizontal;
        Debug.Log("New rotation " + horizontal);
        if (horizontal)
        {
            Debug.Log("Horizontal Sprite");
            OrientationButton.image.sprite = HorizontalTexture;
        }
        else
        {
            Debug.Log("Vertical Sprite");
            OrientationButton.image.sprite = VerticalTexture;
        }
    }

	void OnScoreUpdate(PlayerType scoringPlayer, int newScore){
        Debug.Log("Updating score :)");
        if(scoringPlayer == PlayerType.HUMAN){
			playerScoreText.text = $"Player Score: {newScore}";
		}else{
            AIScoreText.text = $"AI Score: {newScore}";
        }
		
			
	}
}
