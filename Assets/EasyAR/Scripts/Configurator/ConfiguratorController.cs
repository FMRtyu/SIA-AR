using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class ConfiguratorController : MonoBehaviour
{
    public ColorButton[] textures;
    public Transform qualityControlButton;
    public Sprite ColorButtonSprite;
    float offset = 0;
    private GameObject spawnedObject;

    private float previousValue = 64f;

    private void Start()
    {
        // Assign a callback for when this slider changes
        spawnedObject = PlaceOnPlane.spawnedObject;
        // And current value

        foreach (ColorButton t in textures)
        {
            GameObject button = new GameObject();
            button.transform.parent = gameObject.transform;
            button.AddComponent<RectTransform>();
            button.AddComponent<Image>();
            button.AddComponent<Button>();
            button.GetComponent<Image>().sprite = ColorButtonSprite;
            button.GetComponent<Image>().color = t.color;
            offset -= 130f;
            button.transform.position = new Vector3(qualityControlButton.position.x, qualityControlButton.position.y+offset, qualityControlButton.position.z);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(100f,100f);
           
            button.GetComponent<Button>().onClick.AddListener(() => ChangeColor(t.texture));
        }
    }


    public void ChangeColor(Texture2D texture)
    {
        Debug.Log(spawnedObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].GetTexture("_MainTex"));
        Debug.Log(spawnedObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].shader.name);
        spawnedObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", texture);
    }
}

[System.Serializable]
public class ColorButton
{
    public Texture2D texture;
    public Color color;
}
