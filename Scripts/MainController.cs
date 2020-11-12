using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{

    [Header("StartGameOptions")]
    public GameObject startFpanel;
    public GameObject startOptionsPanel;
    public GameObject dangerPanelForStart, negativeTimeText;
    private int NumberOfPlayers;
    private float timePerTurnSaveed;
    public float defoltTimePerTurn = 60;
    private float timePerTurn;
    public Dropdown DropDownNumPlayers;
    public Text FieldTimePerTurn;
    private bool oneTimeDanger;
    private int secondTime;
    private int minutsTime;
    private int hoursTime;
    private bool gamePoused;
    public GameObject pousePanel;
    private string oldName;
    public GameObject light1;

    [Header("Table and List")]
    public Transform dropDownGameOby;
    public RectTransform listObj;
    public GameObject moveUI;
    public Transform verticalLines;
    public Transform horizontalLines;
    public Color colorForLine;
    public Canvas mainCanves;


    [Header("OptionsInGame")]
    public List<GameObject> posiblePlayers;
    public List<GameObject> PlayersList;
    public Transform objectOfPlayers;
    public List<Color> colors;
    public List<int> colorInt;


    [Header("SelectPlayer")]
    public float rangeCircle = 1;
    private GameObject closestPlayer;
    private Vector2 mousePosition;
    private float distanceToClosestPlayer;
    public GameObject customPanel;
    public int selectedPlayer;

    [Header("ColorCustomize")]
    public Dropdown colorDropdown;
    private int rememberColorInt;
    public List<int> hadColors;
    private bool colorBool;

    [Header("NameCustomize")]
    public InputField inputNameText;

    [Header("Stats/Lists")]
    public Transform ListObejct;
    public List<GameObject> statsList;
    public List<GameObject> realRowList;
    private int territorisPerPlayer;
    private int tanksPerPlayer;

    [Header("PlayGame")]
    public GameObject playButton;
    public GameObject gameTimeUI;
    public GameObject whoPlayPanel;
    public Text onTurnText;
    public Text timeInGame;
    public Text reminingTime;
    public Dropdown dropDownWhoPlay;
    public GameObject playerOnTurn;

    [Header("Turns")]
    public List<GameObject> rowListPlayers;
    private int rowInt; 
    public int playerOnTurnInt;

    [Header("InfoText")]
    public GameObject infoPanel;
    public Text infoText;
    public GameObject helpPanel;

    [Header("Danger UI")]
    public GameObject dangerPanel;
    private int territorisInGame;
    public Text dangerText;

    [Header("Customization UI")]
    public Text tanksTextCustom;
    public Text killsTextCustom;
    public InputField terretorisImutText;
    public Image icon;
    public Text taskText;


    private void Start()
    {

        startFpanel.SetActive(true);

        mainCanves.sortingOrder = 5;

        for (int i = 0; i < objectOfPlayers.childCount; i++)
        {

            posiblePlayers.Add(objectOfPlayers.GetChild(i).gameObject);

        }

        for (int i = 0; i < ListObejct.childCount; i++)
        {

            statsList.Add(ListObejct.GetChild(i).gameObject);

        }


    }

    public void startOptionsGame()
    {

        if (FieldTimePerTurn.text.Length == 0)
        {

            timePerTurnSaveed = defoltTimePerTurn; //Ako nije uneto vreme po potezu onda mu se stavlja defolt vrednsot od npr 60
            NumberOfPlayers = DropDownNumPlayers.value + 2;

            startOptionsPanel.SetActive(false);
            startGame();

        }
        else
        {
            if (Mathf.Abs(float.Parse(FieldTimePerTurn.text)) < 30 && oneTimeDanger == false) //Ako je previse kratko vreme po potezu
            {

                dangerPanelForStart.SetActive(true); //Daje se upozorenje samo jednom
                oneTimeDanger = true;

            }
            else
            {

                timePerTurnSaveed = Mathf.Abs(float.Parse(FieldTimePerTurn.text));
                NumberOfPlayers = DropDownNumPlayers.value + 2;

                startOptionsPanel.SetActive(false);
                startGame();

            }
        }

    }


    public void startGame()
    {

        territorisPerPlayer = 42 / NumberOfPlayers;
        int visakTeritorija = 42 - territorisPerPlayer * NumberOfPlayers;

        tanksPerPlayer = 40 - ((NumberOfPlayers - 2) * 5);
        // 2 igraca po 40 tenkova
        // 3 igraca po 35 tenkova
        // 4 igraca po 30 tenkova
        // 5 igraca po 25 tenkova
        // 6 igraca po 20 tenkova

        playButton.SetActive(true);

        //Smislja random boje
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            GiveRandomColor();
        }

        //Ubacuje igrace
        for (int i = 0; i < NumberOfPlayers; i++)
        {

            PlayersList.Add(posiblePlayers[i]);
            PlayersList[i].SetActive(true);

            statsList[i].SetActive(true);
            PlayersList[i].GetComponent<PlayerScript>().thisPlayerOnList = statsList[i];

        }

        //Daje im boje
        for (int i = 0; i < NumberOfPlayers; i++)
        {

            PlayersList[i].GetComponent<Image>().color = colors[hadColors[i]];
            PlayersList[i].GetComponent<PlayerScript>().colorInt = hadColors[i];


            statsList[i].GetComponent<PlayerList>().playerColor.color = colors[hadColors[i]]; //Za listu

        }

        //Daje im redosled igranja
        for (int i = 0; i < 6; i++)
        {

            if (rowListPlayers[i].activeInHierarchy == true)
            {

                rowListPlayers[i].GetComponent<PlayerScript>().playerInt = rowInt;
                rowInt += 1;
                realRowList.Add(rowListPlayers[i]);

            }

        }

        //Daje im imena
        for (int i = 0; i < NumberOfPlayers; i++)
        {

            PlayersList[i].GetComponent<PlayerScript>().Name = "Player " + (PlayersList[i].GetComponent<PlayerScript>().playerInt + 1);
            PlayersList[i].transform.GetChild(0).GetComponent<Text>().text = "Player " + (PlayersList[i].GetComponent<PlayerScript>().playerInt + 1);

        }

        //Daje im teritorije i tenkove i na kraju updejtuje podatke na listu
        for (int i = 0; i < NumberOfPlayers; i++)
        {

            PlayersList[i].GetComponent<PlayerScript>().teritoris += territorisPerPlayer;
            PlayersList[i].GetComponent<PlayerScript>().tanks = tanksPerPlayer;

            if (visakTeritorija > 0)
            {
                PlayersList[i].GetComponent<PlayerScript>().teritoris += 1;
                visakTeritorija -= 1;
            }

            PlayersList[i].GetComponent<PlayerScript>().changeStats();

        }

        //Ubacuje postojacu listu playera dropdown opcije kontinenata
        changeDropdowns();

        //Skeluje tabelu
        scaleList();


    }


    public void Update()
    {

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        selectPlayerForCustomize();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pouseGame();
        }

        updateTapTap();
        updateRefleks();

    }



    private void selectPlayerForCustomize()
    {

        closestPlayer = null;

        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < PlayersList.Count; i++)
            {

                float distanceToPlayer = Vector2.Distance(mousePosition, PlayersList[i].transform.position);

                if (distanceToPlayer < rangeCircle)
                {
                    if (closestPlayer != null)
                    {
                        if (distanceToPlayer < distanceToClosestPlayer)
                        {

                            closestPlayer = PlayersList[i];
                            distanceToClosestPlayer = distanceToPlayer;
                            selectedPlayer = i;

                            customPanel.SetActive(true);
                            SetCustomize();

                        }

                    }
                    else
                    {
                        closestPlayer = PlayersList[i];
                        distanceToClosestPlayer = distanceToPlayer;
                        selectedPlayer = i;

                        customPanel.SetActive(true);
                        SetCustomize();

                    }
                }
            }



        }

    }

    //Kada pali customizaciju
    private void SetCustomize()
    {

        mainCanves.sortingOrder = 5;

        taskText.gameObject.SetActive(false);

        icon.color = colors[PlayersList[selectedPlayer].GetComponent<PlayerScript>().colorInt];

        tanksTextCustom.text = "Tanks: " + PlayersList[selectedPlayer].GetComponent<PlayerScript>().tanks;
        killsTextCustom.text = "Kills: " + PlayersList[selectedPlayer].GetComponent<PlayerScript>().tanksKilled;

        terretorisImutText.placeholder.GetComponent<Text>().text = PlayersList[selectedPlayer].GetComponent<PlayerScript>().teritoris.ToString();
        terretorisImutText.text = PlayersList[selectedPlayer].GetComponent<PlayerScript>().teritoris.ToString();

        colorDropdown.value = PlayersList[selectedPlayer].GetComponent<PlayerScript>().colorInt;
        rememberColorInt = PlayersList[selectedPlayer].GetComponent<PlayerScript>().colorInt;

        inputNameText.placeholder.GetComponent<Text>().text = PlayersList[selectedPlayer].GetComponent<PlayerScript>().Name;
        inputNameText.text = PlayersList[selectedPlayer].GetComponent<PlayerScript>().Name;

        oldName = PlayersList[selectedPlayer].GetComponent<PlayerScript>().Name;

        helpPanel.SetActive(false);

        light1.SetActive(false);

    }

    public void closeCustomize()
    {

        PlayersList[selectedPlayer].GetComponent<Image>().color = colors[colorDropdown.value];
        PlayersList[selectedPlayer].GetComponent<PlayerScript>().colorInt = colorDropdown.value;

        statsList[selectedPlayer].GetComponent<PlayerList>().playerColor.color = colors[colorDropdown.value]; //Za listu

        for (int i = 0; i < PlayersList.Count; i++)
        {

            if (i != selectedPlayer)
            {
                if (PlayersList[selectedPlayer].GetComponent<Image>().color == (PlayersList[i].GetComponent<Image>().color))
                {

                    PlayersList[i].GetComponent<Image>().color = colors[rememberColorInt];
                    PlayersList[i].GetComponent<PlayerScript>().colorInt = rememberColorInt;

                    statsList[i].GetComponent<PlayerList>().playerColor.color = colors[rememberColorInt]; //Za listu

                }
            }

        }

        if (playerOnTurn != null)
        {
            light1.GetComponent<SpriteRenderer>().color = playerOnTurn.GetComponent<Image>().color;
            light1.SetActive(true);
        }

        //Cuva ime
        PlayersList[selectedPlayer].GetComponent<PlayerScript>().Name = inputNameText.text;
        PlayersList[selectedPlayer].transform.GetChild(0).GetComponent<Text>().text = inputNameText.text;

        //Cuva teritorije
        PlayersList[selectedPlayer].GetComponent<PlayerScript>().teritoris = int.Parse(terretorisImutText.text);

        PlayersList[selectedPlayer].GetComponent<PlayerScript>().changeStats(); //Za listu

        checkTeritoris();

        mainCanves.sortingOrder = 1;
        customPanel.SetActive(false);

        if (PlayersList[selectedPlayer].GetComponent<PlayerScript>().Name != oldName)
        {
            changeDropdowns();
            changeWhoToAttack();
        }

    }

    public void onChangeColor()
    {
        icon.color = colors[colorDropdown.value];
    }

    //Daje random boju ali samo jedan put
    public void GiveRandomColor()
    {

        rememberColorInt = Random.Range(0, 6);

        foreach (var item in hadColors)
        {
            if (rememberColorInt == item)
            {
                for (int i = 0; i <= hadColors.Count; i++)
                {

                    colorBool = false;

                    foreach (var item1 in hadColors)
                    {
                        if (i == item1)
                        {
                            colorBool = true;
                        }
                    }

                    if (colorBool == false)
                    {
                        hadColors.Add(i);
                        return;
                    }

                }
            }
        }

        hadColors.Add(rememberColorInt);

    }

    //Daje mogucnost da se prvi igrac izabere
    public void prepreToPlayGame()
    {

        mainCanves.sortingOrder = 5;

        whoPlayPanel.SetActive(true);
        playButton.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            if (rowListPlayers[i].activeInHierarchy == true)
            {
                dropDownWhoPlay.options.Add(new Dropdown.OptionData() { text = rowListPlayers[i].GetComponent<PlayerScript>().Name });
            }
        }

    }

    //Zapocinje igru i definise prvog igraca
    public void playGame()
    {

        mainCanves.sortingOrder = 1;

        if (dropDownWhoPlay.value == 0)
        {

            playerOnTurn = PlayersList[Random.Range(0, NumberOfPlayers)];

        }
        else
        {

            for (int i = 0; i < NumberOfPlayers; i++)
            {

                if (PlayersList[i].GetComponent<PlayerScript>().playerInt == dropDownWhoPlay.value - 1)
                {

                    playerOnTurn = PlayersList[i];
                    break;

                }

            }

        }

        timePerTurn = timePerTurnSaveed;

        turns();

        InvokeRepeating("durtingTurn", 1, 1);

        onTurnText.text = playerOnTurn.GetComponent<PlayerScript>().Name;

        gameTimeUI.SetActive(true);
        whoPlayPanel.SetActive(false);

        playerOnTurnInt = playerOnTurn.GetComponent<PlayerScript>().playerInt;

        StartCoroutine(tanksInfo());

        changeWhoToAttack();

        playerOnTurn.GetComponent<PlayerScript>().changeStats();

        light1.SetActive(true);
        light1.GetComponent<SpriteRenderer>().color = colors[playerOnTurn.GetComponent<PlayerScript>().colorInt];
        light1.transform.position = playerOnTurn.transform.position;
        light1.transform.position = new Vector2(light1.transform.position.x - 0.05f, light1.transform.position.y);

    }

    //Belezi potreban format texta
    private void turns()
    {

        reminingTime.text = timePerTurn.ToString();

        if (hoursTime < 1)
        {
            timeInGame.text = minutsTime.ToString("00") + ":" + (secondTime).ToString("00");
        }
        else
        {
            timeInGame.text = hoursTime.ToString("00") + ":" + (minutsTime).ToString("00");
        }

    }


    //Broji vreme i trajanje poteza
    private void durtingTurn()
    {

        secondTime += 1;
        timePerTurn -= 1;

        if (timePerTurn <= 0)
        {
            finishTurn();
        }

        if (secondTime >= 60)
        {
            minutsTime++;
            secondTime = 0;

            if (minutsTime >= 60)
            {
                hoursTime++;
                minutsTime = 0;
            }

        }

        turns();

    }

    public void finishTurn()
    {

        chechPlayers();

        timePerTurn = timePerTurnSaveed;

        playerOnTurnInt++;

        if (playerOnTurnInt < NumberOfPlayers)
        {
            for (int i = 0; i < PlayersList.Count; i++)
            {
                if (playerOnTurnInt == PlayersList[i].GetComponent<PlayerScript>().playerInt)
                {

                    playerOnTurn = PlayersList[i];
                    onTurnText.text = playerOnTurn.GetComponent<PlayerScript>().Name;

                    break;
                }
            }
        }
        else if (playerOnTurnInt >= NumberOfPlayers) //Krece krug iz pocetka
        {
            for (int i = 0; i < PlayersList.Count; i++)
            {
                if (0 == PlayersList[i].GetComponent<PlayerScript>().playerInt)
                {

                    playerOnTurn = PlayersList[i];
                    onTurnText.text = playerOnTurn.GetComponent<PlayerScript>().Name;


                    playerOnTurnInt = 0; //Pocinje novi krug

                    break;
                }
            }
        }

        reminingTime.text = timePerTurn.ToString();

        StopAllCoroutines(); //Zaustavi se prvo prethodni korutin
        StartCoroutine(tanksInfo());

        //Ubacuje igrace koje moze napasti
        changeWhoToAttack();

        playerOnTurn.GetComponent<PlayerScript>().changeStats();

        light1.GetComponent<SpriteRenderer>().color = colors[playerOnTurn.GetComponent<PlayerScript>().colorInt];
        light1.transform.position = playerOnTurn.transform.position;
        light1.transform.position = new Vector2 ( light1.transform.position.x - 0.05f , light1.transform.position.y);

        deffTanksOld.text = "1";
        attackTanksOld.text = "1";

        backAttack();

    }


    private void changeDropdowns()
    {

        for (int b = 0; b < 6; b++)
        {

            dropDownGameOby.GetChild(b).GetComponent<Dropdown>().ClearOptions();
            dropDownGameOby.GetChild(b).GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = "None" });
            dropDownGameOby.GetChild(b).GetComponent<Dropdown>().value = 1;

            for (int i = 0; i < 6; i++)
            {
                if (rowListPlayers[i].activeInHierarchy == true)
                {
                    dropDownGameOby.GetChild(b).GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = rowListPlayers[i].GetComponent<PlayerScript>().Name });
                }
            }
        }
    }


    private void scaleList()
    {

        //Stavlja tekst na poziciju u  zavinsoti od broja igraca
        moveUI.transform.position = new Vector2(moveUI.transform.position.x, moveUI.transform.position.y - (0.7f * (6 - NumberOfPlayers)));

        //Skejluje backgraund tabele i stavlja ga na poziciju u zavisnosti od broja igraca
        listObj.offsetMax = new Vector2(listObj.offsetMax.x, listObj.offsetMax.y - (64 * (6 - NumberOfPlayers)));
        listObj.transform.position = new Vector2(listObj.transform.position.x, listObj.transform.position.y + (0.1f * (6 - NumberOfPlayers)));

        //Skejluje vertikalne linije tabele i stavlja ga na poziciju u zavisnosti od broja igraca
        verticalLines.position = new Vector2(verticalLines.position.x, verticalLines.position.y - ((float)((6 - NumberOfPlayers))) / 2.84f);
        verticalLines.localScale = new Vector2(1, 1 - (0.14f * (6 - NumberOfPlayers)));

        //Deaktivirna nepotrebene horizontalne linije tabele i definise okvrinu liniju u zavisnosti od broja igraca
        for (int i = 0; i < 7; i++)
        {

            if (i < (6 - NumberOfPlayers))
            {

                horizontalLines.GetChild(i).gameObject.SetActive(false);

            }
            else if (i == (6 - NumberOfPlayers))
            {
                horizontalLines.GetChild(i).GetComponent<SpriteRenderer>().color = colorForLine;
                horizontalLines.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 4;
            }
            else
            {
                break;
            }

        }

        //Sortira canves iza linija tabele
        mainCanves.sortingOrder = 1;

    }


    public void pouseGame()
    {
        if (gamePoused == false)
        {
            pousePanel.SetActive(true);
            gamePoused = true;
            mainCanves.sortingOrder = 5;
            light1.SetActive(false);

            Time.timeScale = 0;
        }
        else
        {
            pousePanel.SetActive(false);
            gamePoused = false;
            light1.SetActive(true);

            Time.timeScale = 1;
        }

    }

    public void restartGame()
    {
        SceneManager.LoadScene(0);
    }

    //Nakon sto se unese owner nekog kontineta daje ostvareni bonus i prikazuje ga
    public void changeConcinetOwner()
    {

        for (int i = 0; i < NumberOfPlayers; i++) //Prvo ponisti svim igracima bonus
        {

            PlayersList[i].GetComponent<PlayerScript>().kontinetnGrow = 0;

        }

        for (int i = 0; i < 6; i++) //Onda proveri da li nekom treba da da bonus
        {

            if (dropDownGameOby.GetChild(i).GetComponent<Dropdown>().value != 0)
            {

                int intTanks = int.Parse(dropDownGameOby.GetChild(i).GetComponent<Dropdown>().name);

                realRowList[dropDownGameOby.GetChild(i).GetComponent<Dropdown>().value - 1].GetComponent<PlayerScript>().kontinetnGrow += intTanks;

            }

        }

        for (int i = 0; i < NumberOfPlayers; i++) //Onda priakze taj bonus
        {

            PlayersList[i].GetComponent<PlayerScript>().changeStats();

        }

    }


    private IEnumerator tanksInfo()
    {

        infoText.text = "This turn " + playerOnTurn.GetComponent<PlayerScript>().Name + " earns "
            + (playerOnTurn.GetComponent<PlayerScript>().teritoris / 3 + playerOnTurn.GetComponent<PlayerScript>().kontinetnGrow) + " tanks.";


        playerOnTurn.GetComponent<PlayerScript>().tanks += (playerOnTurn.GetComponent<PlayerScript>().teritoris / 3 + playerOnTurn.GetComponent<PlayerScript>().kontinetnGrow);


        infoPanel.SetActive(true);

        yield return new WaitForSeconds(timePerTurn / 4);

        infoPanel.SetActive(false);

    }

    public void okInfo()
    {
        infoPanel.SetActive(false);
        StopCoroutine(tanksInfo());
    }

    private void checkTeritoris()
    {

        territorisInGame = 0;

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            territorisInGame += PlayersList[i].GetComponent<PlayerScript>().teritoris;
        }

        if (territorisInGame > 42)
        {

            dangerText.text = "Warning!Territory limit is 42, and player occupied " + territorisInGame + ". (You have " + (territorisInGame - 42) + " territories more than it is allowed.)";

            dangerPanel.SetActive(true);

        }
        else if (territorisInGame == 42)
        {
            dangerPanel.SetActive(false);
        }
        else
        {
            dangerText.text = "Warning!Territory limit is 42, and player occupied " + territorisInGame + ". (You need " + (42 - territorisInGame) + "more territories.)";

            dangerPanel.SetActive(true);

        }

    }

    public void back()
    {
        customPanel.SetActive(false);
    }

    public void showTask()
    {
        taskText.gameObject.SetActive(!taskText.gameObject.activeInHierarchy);
    }

    public GameObject setingsPanel;
    public InputField timeText2;

    public void opetSetingsPaenl()
    {

        setingsPanel.SetActive(true);

        timeText2.placeholder.GetComponent<Text>().text = timePerTurnSaveed.ToString();

        light1.SetActive(false);

        mainCanves.sortingOrder = 5;

    }

    public void saveSetings()
    {

        setingsPanel.SetActive(false);

        if (timeText2.text.Length > 1)
        {
            timePerTurnSaveed = Mathf.Abs(int.Parse(timeText2.text));
        }

        light1.SetActive(true);

        mainCanves.sortingOrder = 1;

    }


    [Header("Attack Options")]
    public Dropdown attackOption;
    public Text attackTankText;
    public Text deffTankText;
    public InputField attackTanksOld;
    public InputField deffTanksOld;
    private GameObject deffender;
    public GameObject attackPanelCubes;
    public Text attackerName;
    public Text deffendName;
    public Dropdown whoToAttack;
    public Text AttackerNameMain;
    private int attackInt;
    private int deffInt;
    public List<GameObject> listAttackers;



    private void changeWhoToAttack()
    {

        whoToAttack.ClearOptions();
        listAttackers.Clear();

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            if (playerOnTurn.GetComponent<PlayerScript>().playerInt != realRowList[i].GetComponent<PlayerScript>().playerInt)
            {
                listAttackers.Add(realRowList[i]);
            }
        }

        for (int i = 0; i < listAttackers.Count; i++)
        {

            whoToAttack.options.Add(new Dropdown.OptionData() { text = listAttackers[i].GetComponent<PlayerScript>().Name });

        }


        AttackerNameMain.text = playerOnTurn.GetComponent<PlayerScript>().Name;

        whoToAttack.value = 0;
        whoToAttack.captionText.text = listAttackers[whoToAttack.value].GetComponent<PlayerScript>().Name;


    }

    public void Attack()
    {

        if (attackOption.value == 0)
        {
            standardAttack();
        }
        else if (attackOption.value == 1)
        {

            if(attackTanksOld.text.Length != 0)
            {
                if (int.Parse(attackTanksOld.text) <= 0)
                {

                    attackTanksOld.text = "1";

                }
            }
            if (deffTanksOld.text.Length != 0)
            {
                if (int.Parse(deffTanksOld.text) <= 0)
                {

                    deffTanksOld.text = "1";

                }
            }

            tapTapAttack();

            pressImage.SetActive(false);
            howToPlay.text = " ";

            aLost = 0;
            bLost = 0;

            mainCanves.sortingOrder = 5;

        }
        else if (attackOption.value == 2)
        {

            if (attackTanksOld.text.Length != 0)
            {
                if (int.Parse(attackTanksOld.text) <= 0)
                {

                    attackTanksOld.text = "1";

                }
            }
            if (deffTanksOld.text.Length != 0)
            {
                if (int.Parse(deffTanksOld.text) <= 0)
                {

                    deffTanksOld.text = "1";

                }
            }

            tapTapAttack();

            pressImage.SetActive(true);
            howToPlay.text = "When you see:";

            aLost = 0;
            bLost = 0;

            mainCanves.sortingOrder = 5;

        }

        light1.SetActive(false);

    }


    public void standardAttack()
    {

        attackPanelCubes.SetActive(true);
        winPanel1.SetActive(false);
        mainCanves.sortingOrder = 5;

        deffender = listAttackers[whoToAttack.value];

        attackerName.text = playerOnTurn.GetComponent<PlayerScript>().Name;
        deffendName.text = deffender.GetComponent<PlayerScript>().Name;

        loseLeft.text = " ";
        loseRight.text = " ";

        if (attackTanksOld.text.Length == 0) //Ako se ne unese sa koliko tenkova napada, koristice 1 tank po defoltu
        {
            attackInt = int.Parse(attackTanksOld.placeholder.GetComponent<Text>().text);
            attackTankText.text = "Tanks: " + attackTanksOld.placeholder.GetComponent<Text>().text;
        }
        else
        {

            attackInt = int.Parse(attackTanksOld.text);

            if (attackInt < (playerOnTurn.GetComponent<PlayerScript>().tanks - playerOnTurn.GetComponent<PlayerScript>().teritoris))
            {
                attackTankText.text = "Tanks: " + attackTanksOld.text;
            }
            else //Ako pokusaca da napadne sa vise tenkova nego sto je moguce
            {

                attackInt = playerOnTurn.GetComponent<PlayerScript>().tanks - playerOnTurn.GetComponent<PlayerScript>().teritoris;
                attackTankText.text = "Tanks: " + attackInt;
                attackTanksOld.text = attackInt.ToString();

            }


        }

        if (deffTanksOld.text.Length == 0)
        {
            deffInt = int.Parse(deffTanksOld.placeholder.GetComponent<Text>().text);
            deffTankText.text = "Tanks: " + deffTanksOld.placeholder.GetComponent<Text>().text;
        }
        else
        {

            deffInt = int.Parse(deffTanksOld.text);

            if (deffInt < (deffender.GetComponent<PlayerScript>().tanks - deffender.GetComponent<PlayerScript>().teritoris))
            {
                deffTankText.text = "Tanks: " + deffTanksOld.text;
            }
            else
            {

                deffInt = deffender.GetComponent<PlayerScript>().tanks - deffender.GetComponent<PlayerScript>().teritoris;
                deffTankText.text = "Tanks: " + deffInt;
                deffTanksOld.text = deffInt.ToString();

            }

        }

        setActiveTanks();

    }

    public void backAttack()
    {
        attackPanelCubes.SetActive(false);
        tapTapPanel.SetActive(false);
        winPanel1.SetActive(false);

        canPress = false;
        doTapTap = false;

        light1.SetActive(true);

        mainCanves.sortingOrder = 1;

    }

    [Header("Cube attack")]
    public List<GameObject> attackerActiveTanks;
    public List<GameObject> deffenderActiveTanks;
    public Dropdown attackTankDropdown;
    public Dropdown deffTankDropdown;
    private int attackCount;
    private int deffCount;
    public List<int> valuesAttack;
    public List<int> valuesDefense;
    private int bMax;
    private int aMax;
    private int bLost;
    private int aLost;
    public GameObject winPanel1;
    public Text winText;
    public Text loseLeft;
    public Text loseRight;


    //Namesta broj sa koliko tenkova se napada
    public void onChangeActiveTanks(bool leftDrop)
    {

        if (leftDrop == true)
        {

            if (attackTankDropdown.value + 1 <= attackInt)
            {
                attackCount = attackTankDropdown.value + 1;
            }
            else
            {
                attackTankDropdown.value = 0;
                attackCount = 1;
            }

            for (int i = 0; i < 3; i++)
            {

                if (attackCount > i)
                {
                    attackerActiveTanks[i].SetActive(true);
                }
                else
                {
                    attackerActiveTanks[i].SetActive(false);
                }
            }
        }
        else
        {
            if (deffTankDropdown.value + 1 <= deffInt)
            {
                deffCount = deffTankDropdown.value + 1;
            }
            else
            {
                deffTankDropdown.value = 0;
                deffCount = 1;
            }

            for (int i = 0; i < 3; i++)
            {

                if (deffCount > i)
                {
                    deffenderActiveTanks[i].SetActive(true);
                }
                else
                {
                    deffenderActiveTanks[i].SetActive(false);
                }
            }
        }

    }

    private void setActiveTanks()
    {

        if (attackInt == 1)
        {
            attackTankDropdown.value = 0;
            attackCount = 1;
        }
        else if (attackInt == 2)
        {
            attackTankDropdown.value = 1;
            attackCount = 2;
        }
        else if (attackInt > 2)
        {
            attackTankDropdown.value = 2;
            attackCount = 3;
        }

        if (deffInt == 1)
        {
            deffTankDropdown.value = 0;
            deffCount = 1;
        }
        else if (deffInt == 2)
        {
            deffTankDropdown.value = 1;
            deffCount = 2;
        }
        else if (deffInt > 2)
        {
            deffTankDropdown.value = 2;
            deffCount = 3;
        }

        for (int i = 0; i < 3; i++)
        {

            if (attackCount > i)
            {
                attackerActiveTanks[i].SetActive(true);
            }
            else
            {
                attackerActiveTanks[i].SetActive(false);
            }

            if (deffCount > i)
            {
                deffenderActiveTanks[i].SetActive(true);
            }
            else
            {
                deffenderActiveTanks[i].SetActive(false);
            }

        }

    }

    private bool oneTime;

    public void prepereCubeAttack()
    {

        if (attackInt > 0 && deffInt > 0)
        {

            if (oneTime == false)
            {
                oneTime = true;

                InvokeRepeating("shakeCubes", 0, Random.Range(0.1f, 0.2f));

                Invoke("doCubeAttack", 2);
            }

        }

    }

    private void shakeCubes()
    {


        for (int i = 0; i < attackCount; i++)
        {

            attackerActiveTanks[i].transform.GetChild(0).GetComponent<Text>().text = (Random.Range(1, 7)).ToString();

        }

        for (int i = 0; i < deffCount; i++)
        {

            deffenderActiveTanks[i].transform.GetChild(0).GetComponent<Text>().text = (Random.Range(1, 7)).ToString();

        }

    }


    public void doCubeAttack()
    {

        oneTime = false;

        valuesAttack.Clear();
        valuesDefense.Clear();

        CancelInvoke("shakeCubes");

        aLost = 0;
        bLost = 0;

        for (int i = 0; i < attackCount; i++)
        {

            valuesAttack.Add(Random.Range(1, 7));
            Debug.Log("a" + (i + 1) + " = " + valuesAttack[i]);

            attackerActiveTanks[i].transform.GetChild(0).GetComponent<Text>().text = valuesAttack[i].ToString();

        }

        for (int i = 0; i < deffCount; i++)
        {

            valuesDefense.Add(Random.Range(1, 7));

            Debug.Log("b" + (i + 1) + " = " + valuesDefense[i]);

            deffenderActiveTanks[i].transform.GetChild(0).GetComponent<Text>().text = valuesDefense[i].ToString();

        }


        //Logika borbe
        for (int i = 0; i < 3; i++)
        {

            bMax = 0;
            aMax = 0;


            if (valuesAttack.Count > 0 && valuesDefense.Count > 0)
            {

                for (int b = 0; b < valuesDefense.Count; b++)
                {

                    if (valuesDefense[b] > bMax)
                    {
                        bMax = valuesDefense[b];
                    }

                }

                for (int b = 0; b < valuesAttack.Count; b++)
                {
                    if (valuesAttack[b] > aMax)
                    {
                        aMax = valuesAttack[b];
                    }
                }

                if (bMax >= aMax)
                {

                    Debug.Log(bMax + " b je pobedilo a " + aMax);

                    valuesAttack.Remove(aMax);
                    valuesDefense.Remove(bMax);

                    aLost++;

                }
                else
                {

                    Debug.Log(bMax + " b je izgubilo od a " + aMax);
                    valuesAttack.Remove(aMax);
                    valuesDefense.Remove(bMax);

                    bLost++;

                }
            }
            else
            {
                break;
            }
        }

        //Rezultat borbe
        attackInt -= Mathf.Abs(aLost);
        deffender.GetComponent<PlayerScript>().tanksKilled += aLost;
        playerOnTurn.GetComponent<PlayerScript>().tanks -= aLost;
        playerOnTurn.GetComponent<PlayerScript>().changeStats();
        loseLeft.text = "- " + aLost;
        aLost = 0;

        deffInt -= Mathf.Abs(bLost);
        playerOnTurn.GetComponent<PlayerScript>().tanksKilled += bLost;
        deffender.GetComponent<PlayerScript>().tanks -= bLost;
        deffender.GetComponent<PlayerScript>().changeStats();
        loseRight.text = "- " + bLost;
        bLost = 0;

        attackTankText.text = "Tanks: " + attackInt;
        deffTankText.text = "Tanks: " + deffInt;


        if (deffInt <= 0)
        {

            winText.text = playerOnTurn.GetComponent<PlayerScript>().Name + " won battle!";

            playerOnTurn.GetComponent<PlayerScript>().teritoris += 1;
            deffender.GetComponent<PlayerScript>().teritoris -= 1;

            playerOnTurn.GetComponent<PlayerScript>().changeStats();
            deffender.GetComponent<PlayerScript>().changeStats();

            winPanel1.SetActive(true);

        }
        else if (attackInt <= 0)
        {

            winText.text = deffender.GetComponent<PlayerScript>().Name + " won battle!";

            playerOnTurn.GetComponent<PlayerScript>().changeStats();
            deffender.GetComponent<PlayerScript>().changeStats();

            winPanel1.SetActive(true);

        }

        if (attackInt == 1)
        {
            attackTankDropdown.value = 0;
            attackCount = 1;
        }
        else if (attackInt == 2)
        {
            attackTankDropdown.value = 1;
            attackCount = 2;
        }

        if (deffInt == 1)
        {
            deffTankDropdown.value = 0;
            deffCount = 1;
        }
        else if (deffInt == 2)
        {
            deffTankDropdown.value = 1;
            deffCount = 2;
        }

        for (int i = 0; i < 3; i++)
        {

            if (attackCount > i)
            {
                attackerActiveTanks[i].SetActive(true);
            }
            else
            {
                attackerActiveTanks[i].SetActive(false);
            }

            if (deffCount > i)
            {
                deffenderActiveTanks[i].SetActive(true);
            }
            else
            {
                deffenderActiveTanks[i].SetActive(false);
            }

        }

    }


    [Header("TapTap Attack")]
    public GameObject tapTapPanel;
    public Text name1;
    public Text name2;
    public Text loseleft1;
    public Text loseleft2;
    public Text tanks1;
    public Text tanks2;
    public int score1;
    public int score2;
    public Text score1Text;
    public Text score2Text;
    private bool doTapTap;
    private bool doReleks;
    public Text mainTime;
    private int timeForT = 3;
    public GameObject effect;
    public GameObject startButton;


    //Tap Tap attack
    private void tapTapAttack()
    {

        tapTapPanel.SetActive(true);

        mainTime.text = "";

        deffender = listAttackers[whoToAttack.value];

        name1.text = playerOnTurn.GetComponent<PlayerScript>().Name;
        name2.text = deffender.GetComponent<PlayerScript>().Name;

        loseleft1.text = " ";
        loseleft2.text = " ";

        score1Text.text = " ";
        score2Text.text = " ";

        startRefleksButton.SetActive(true);

        if (attackTanksOld.text.Length == 0) //Ako ne unese sa koliko tenkova napada, koristice 1 tank po defoltu
        {
            attackInt = int.Parse(attackTanksOld.placeholder.GetComponent<Text>().text);
            tanks1.text = "Tanks: " + attackTanksOld.placeholder.GetComponent<Text>().text;
        }
        else
        {

            attackInt = int.Parse(attackTanksOld.text);

            if (attackInt < (playerOnTurn.GetComponent<PlayerScript>().tanks - playerOnTurn.GetComponent<PlayerScript>().teritoris))
            {
                tanks1.text = "Tanks: " + attackTanksOld.text;
            }
            else //Ako pokusaca da napadne sa vise tenkova nego sto je moguce
            {

                attackInt = playerOnTurn.GetComponent<PlayerScript>().tanks - playerOnTurn.GetComponent<PlayerScript>().teritoris;
                tanks1.text = "Tanks: " + attackInt;
                attackTanksOld.text = attackInt.ToString();

            }


        }

        if (deffTanksOld.text.Length == 0)
        {
            deffInt = int.Parse(deffTanksOld.placeholder.GetComponent<Text>().text);
            tanks2.text = "Tanks: " + deffTanksOld.placeholder.GetComponent<Text>().text;
        }
        else
        {

            deffInt = int.Parse(deffTanksOld.text);

            if (deffInt < (deffender.GetComponent<PlayerScript>().tanks - deffender.GetComponent<PlayerScript>().teritoris))
            {
                tanks2.text = "Tanks: " + deffTanksOld.text;
            }
            else
            {

                deffInt = deffender.GetComponent<PlayerScript>().tanks - deffender.GetComponent<PlayerScript>().teritoris;
                tanks2.text = "Tanks: " + deffInt;
                deffTanksOld.text = deffInt.ToString();

            }

        }
    }



    public void startBattle()
    {

        startRefleksButton.SetActive(true);

        if (attackOption.value == 1)
        {

            timeForT = 3;

            InvokeRepeating("rmTime", 0, 1);
            Invoke("begin", 3);

            startButton.SetActive(false);

            score1 = 0;
            score2 = 0;

            howToPlay.text = " ";

        }
        else if (attackOption.value == 2)
        {

            howToPlay.text = "When you see:";

            StartCoroutine(refleksEnumerator());

            canPress = true;

            startRefleksButton.SetActive(false);

        }

    }

    private void rmTime()
    {

        mainTime.text = timeForT + "...";
        timeForT -= 1;

    }

    private void begin()
    {

        doTapTap = true;

        aLost = 0;
        bLost = 0;

        CancelInvoke("rmTime");
        mainTime.text = "";

    }

    private void updateTapTap()
    {
        if (doTapTap == true)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {

                score1++;
                score1Text.text = score1.ToString();

                Instantiate(effect.gameObject, score1Text.transform.position, Quaternion.identity);

                if (score1 >= 10)
                {

                    bLost++;
                    loseleft2.text = "- " + bLost;

                    playerOnTurn.GetComponent<PlayerScript>().tanksKilled += 1;

                    deffender.GetComponent<PlayerScript>().tanks -= 1;
                    deffender.GetComponent<PlayerScript>().changeStats();

                    deffInt -= 1;
                    tanks2.text = "Tanks: " + deffInt;

                    score1 = 0;
                    score1Text.text = "0";

                    score2 = 0;
                    score2Text.text = "0";

                    if (deffInt <= 0)
                    {

                        doTapTap = false;

                        winText.text = playerOnTurn.GetComponent<PlayerScript>().Name + " won battle!";

                        deffender.GetComponent<PlayerScript>().teritoris -= 1;
                        playerOnTurn.GetComponent<PlayerScript>().teritoris += 1;

                        playerOnTurn.GetComponent<PlayerScript>().changeStats();
                        deffender.GetComponent<PlayerScript>().changeStats();

                        winPanel1.SetActive(true);

                    }

                }

            }

            if (Input.GetKeyDown(KeyCode.Return))
            {

                score2++;
                score2Text.text = score2.ToString();

                Instantiate(effect.gameObject, score2Text.transform.position, Quaternion.identity);

                if (score2 >= 10)
                {

                    aLost++;
                    loseleft1.text = "- " + aLost;

                    deffender.GetComponent<PlayerScript>().tanksKilled += 1;

                    playerOnTurn.GetComponent<PlayerScript>().tanks -= 1;
                    playerOnTurn.GetComponent<PlayerScript>().changeStats();

                    attackInt -= 1;
                    tanks1.text = "Tanks: " + attackInt;


                    score1 = 0;
                    score1Text.text = "0";

                    score2 = 0;
                    score2Text.text = "0";

                    if (attackInt <= 0)
                    {

                        doTapTap = false;

                        winText.text = deffender.GetComponent<PlayerScript>().Name + " won battle!";

                        playerOnTurn.GetComponent<PlayerScript>().changeStats();
                        deffender.GetComponent<PlayerScript>().changeStats();

                        winPanel1.SetActive(true);

                    }

                }

            }

        }

    }


    //Refleks attack;
    [Header("Refleks Attack")]
    public Text howToPlay;
    private bool canPress;
    private bool mustPress;
    public GameObject pressImage;
    public GameObject startRefleksButton;
    private int eNumeratorInt;

    private IEnumerator refleksEnumerator()
    {

        mustPress = false;
        pressImage.SetActive(false);


        yield return new WaitForSeconds(Random.Range(2, 5));

        mustPress = true;
        pressImage.SetActive(true);


    }

    private void updateRefleks()
    {
        if (canPress == true)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {

                if (mustPress == true) //Ako je dugme pretisnuto na vreme
                {

                    Instantiate(effect.gameObject, score2Text.transform.position, Quaternion.identity);

                    bLost++;
                    loseleft2.text = "- " + bLost;

                    playerOnTurn.GetComponent<PlayerScript>().tanksKilled += 1;

                    deffender.GetComponent<PlayerScript>().tanks -= 1;
                    deffender.GetComponent<PlayerScript>().changeStats();

                    deffInt -= 1;
                    tanks2.text = "Tanks: " + deffInt;

                    StopAllCoroutines();
                    StartCoroutine(refleksEnumerator());

                    if (deffInt <= 0) //Ako su unisteni sve tenkici
                    {

                        canPress = false;

                        winText.text = playerOnTurn.GetComponent<PlayerScript>().Name + " won battle!";

                        deffender.GetComponent<PlayerScript>().teritoris -= 1;
                        playerOnTurn.GetComponent<PlayerScript>().teritoris += 1;

                        playerOnTurn.GetComponent<PlayerScript>().changeStats();
                        deffender.GetComponent<PlayerScript>().changeStats();

                        winPanel1.SetActive(true);
                        StopAllCoroutines();

                    }

                }
                else
                {

                    Instantiate(effect.gameObject, score1Text.transform.position, Quaternion.identity);

                    aLost++;
                    loseleft1.text = "- " + aLost;

                    deffender.GetComponent<PlayerScript>().tanksKilled += 1;

                    playerOnTurn.GetComponent<PlayerScript>().tanks -= 1;
                    playerOnTurn.GetComponent<PlayerScript>().changeStats();

                    attackInt -= 1;
                    tanks1.text = "Tanks: " + attackInt;

                    StopAllCoroutines();
                    StartCoroutine(refleksEnumerator());

                    if (attackInt <= 0)
                    {

                        canPress = false;

                        winText.text = deffender.GetComponent<PlayerScript>().Name + " won battle!";

                        playerOnTurn.GetComponent<PlayerScript>().changeStats();
                        deffender.GetComponent<PlayerScript>().changeStats();

                        winPanel1.SetActive(true);
                        StopAllCoroutines();

                    }

                }

                

            }

            if (Input.GetKeyDown(KeyCode.Return))
            {

                if (mustPress == true)
                {

                    Instantiate(effect.gameObject, score1Text.transform.position, Quaternion.identity);

                    aLost++;
                    loseleft1.text = "- " + aLost;

                    deffender.GetComponent<PlayerScript>().tanksKilled += 1;

                    playerOnTurn.GetComponent<PlayerScript>().tanks -= 1;
                    playerOnTurn.GetComponent<PlayerScript>().changeStats();

                    attackInt -= 1;
                    tanks1.text = "Tanks: " + attackInt;

                    StopAllCoroutines();
                    StartCoroutine(refleksEnumerator());

                    if (attackInt <= 0)
                    {

                        canPress = false;

                        winText.text = deffender.GetComponent<PlayerScript>().Name + " won battle!";


                        playerOnTurn.GetComponent<PlayerScript>().changeStats();
                        deffender.GetComponent<PlayerScript>().changeStats();

                        winPanel1.SetActive(true);
                        StopAllCoroutines();

                    }

                }
                else //Ako je kliknuto pre vremena igrac desno gubi
                {

                    Instantiate(effect.gameObject, score2Text.transform.position, Quaternion.identity);

                    bLost++;
                    loseleft2.text = "- " + bLost;

                    playerOnTurn.GetComponent<PlayerScript>().tanksKilled += 1;

                    deffender.GetComponent<PlayerScript>().tanks -= 1;
                    deffender.GetComponent<PlayerScript>().changeStats();

                    deffInt -= 1;
                    tanks2.text = "Tanks: " + deffInt;

                    StopAllCoroutines();
                    StartCoroutine(refleksEnumerator());

                    if (deffInt <= 0)
                    {

                        canPress = false;

                        winText.text = playerOnTurn.GetComponent<PlayerScript>().Name + " won battle!";

                        deffender.GetComponent<PlayerScript>().teritoris -= 1;
                        playerOnTurn.GetComponent<PlayerScript>().teritoris += 1;

                        playerOnTurn.GetComponent<PlayerScript>().changeStats();
                        deffender.GetComponent<PlayerScript>().changeStats();

                        winPanel1.SetActive(true);
                        StopAllCoroutines();

                    }

                }

            }

        }

    }

    public GameObject winPaenl;
    public Text winText3;

    //Lose game
    private void chechPlayers()
    {

        for (int i = 0; i < NumberOfPlayers; i++)
        {

            if (PlayersList[i].GetComponent<PlayerScript>().teritoris <= 0 && (PlayersList[i].gameObject.activeInHierarchy == true))
            {

                PlayersList[i].gameObject.SetActive(false);
                NumberOfPlayers -= 1;

                //Neko je pobedio
                if (NumberOfPlayers == 1)
                {

                    for (int b = 0; b < 6; b++)
                    {
                        if (rowListPlayers[b].activeInHierarchy == true)
                        {

                            light1.SetActive(false);
                            mainCanves.sortingOrder = 5;

                            winPaenl.SetActive(true);
                            winText3.text = rowListPlayers[b].GetComponent<PlayerScript>().Name + "won game!";

                            return;

                        }

                    }

                }
                else
                {
                    //Daje redosled igrana drugim igracima

                    realRowList.Clear();
                    rowInt = 0;

                    for (int b = 0; b < 6; b++)
                    {

                        if (rowListPlayers[b].activeInHierarchy == true)
                        {

                            rowListPlayers[b].GetComponent<PlayerScript>().playerInt = rowInt;
                            rowInt += 1;
                            realRowList.Add(rowListPlayers[b]);

                        }

                    }

                    changeDropdowns();
                    changeWhoToAttack();
                }

            }

        }

    }



    /*
      
        Aplikacije je zamisljena da sto vise olaksa i ubrza igru, tako sto ce u sebi skladistii sve potrebne informacije pomocu kojih obavestava igrace o tome koliko tenkova dobjau 
        po potezu, ko je na potezu, koliko vremena im je preostalo, koliko teritorija poseduju i slicno. Potpuno izbacuje upotrebu kockica virtuelnim tenkovima koji funkcionisu
        isto. Takodje daje mogucnost inovatinim borbama brzih refleksa i brzeg kliktanja. Ideja aplikacije je da karakteristike budu jednake za sve igrace tako sto ce ograniciti
        vreme trajanja runde i svakom igracu dati nasumicnu boju i ista pravila.  

    */



}
