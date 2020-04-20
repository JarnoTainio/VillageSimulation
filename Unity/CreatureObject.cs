using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatureObject : MonoBehaviour
{
    public int id;
    public PartRandomizer[] parts;
    public TextMeshProUGUI villagerName;
    public Image icon;
    public Color[] questColors;
    Creature creature;

    public void SetCreature(Creature c)
    {
        id = c.ID;
        creature = c;
        foreach(PartRandomizer pr in parts){
            pr.Randomize(c.random);
        }
        villagerName.text = c.name;
    }

    private void Update()
    {
        bool hasQuest = creature.quest != null;
        icon.gameObject.SetActive(hasQuest);
        if (hasQuest && (int)creature.quest.state - 1 < questColors.Length)
        {
            icon.color = questColors[(int)creature.quest.state - 1];
        }

    }
}
