using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Creature : MonoBehaviour
{
    public int _Hp;

    public GameObject bloodPrefab;
    public int attack_range = 3;
    public int attack_speed = 1;
    public int health_speed = 1;
    public int view_range = 10;

    public int Hp
    {
        get
        {
            return _Hp;
        }
        private set
        {
            _Hp = value;
            hpImage.color = Color.Lerp(Color.red, Color.green, (float)value / MaxHp);
            hpImage.fillAmount = (float)value / MaxHp;
        }
    }

    public int Attack { get; private set; }

    private int MaxHp;
    private int Armor;

    Image hpImage;

    float time = 0;

    protected virtual void Update()
    {
        if (Hp == 0)
        {
            var go = Instantiate(bloodPrefab);
            go.transform.position = transform.position;
            GameManager.Instance.KillCreature(gameObject);
        }
        if (Hp >= 10) return;

        time += Time.deltaTime;

        if (time > health_speed)
        {
            time = 0;
            Hp++;
        }
    }

    public bool Alive
    {
        get { return Hp > 0; }
    }

    protected virtual void Awake()
    {
        _Hp = 10;
        MaxHp = 10;

        Armor = Random.Range(1, 5);
        Attack = Random.Range(3, 7);

        var prefab = Resources.Load<GameObject>("HealthBarCanvas");

        var go = Instantiate(prefab);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.up;

        hpImage = go.GetComponentsInChildren<Image>().Single(x => x.name == "HealthBar");
        hpImage.color = Color.green;

    }

    public virtual void RecibeDamage(int dmg)
    {
        if (dmg - Armor <= 0)
            Hp -= 1;
        else
            Hp -= dmg - Armor;

        Hp = Hp < 0 ? 0 : Hp;
    }
}
