using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Text textInterfaceLeft;
    private Text textInterfaceRight;
    private Button buy_worker;
    private Button buy_explorer;

    private int workerCount = 1;
    private int explorerCount = 2;
    private int goldCollected = 0;
    private int flaggedMines = 0;

    public int ExplorerCount
    {
        get { return explorerCount; }
        set
        {
            explorerCount = value;
            UpdateUI();
        }
    }

    public int WorkerCount
    {
        get { return workerCount; }
        set
        {
            workerCount = value;
            UpdateUI();
        }
    }

    public int FlaggedMines
    {
        get { return flaggedMines; }
        set
        {
            flaggedMines = value;
            UpdateUI();
        }
    }

    public int GoldCollected
    {
        get { return goldCollected; }
        set
        {
            goldCollected = value;
            UpdateUI();
        }
    }
    
    private void Awake()
    {
        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        Text[] texts = gameObject.GetComponentsInChildren<Text>();

        textInterfaceLeft = texts.Single(x => x.name == "ResourcesTextL");
        textInterfaceRight = texts.Single(x => x.name == "ResourcesTextR");

        buy_worker = buttons.Single(x => x.name == "Worker");
        buy_explorer = buttons.Single(x => x.name == "Explorer");

        buy_worker.onClick.AddListener(() =>
        {
            GameManager.Instance.BuyWorker();
        });

        buy_explorer.onClick.AddListener(() =>
        {
            GameManager.Instance.BuyExplorer();
        });
    }

    private void UpdateUI()
    {
        buy_explorer.interactable = GoldCollected >= GameManager.EXPLORER_COST;
        buy_worker.interactable = GoldCollected >= GameManager.WORKER_COST;

        textInterfaceLeft.text = $"Gold: {goldCollected} \nFlaggedMines: {flaggedMines}";
        textInterfaceRight.text = $"Workers: {workerCount} \nExplorers: {explorerCount}";
    }
}
