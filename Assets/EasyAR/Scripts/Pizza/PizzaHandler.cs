using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PizzaHandler : MonoBehaviour
{
    /// <summary>
    /// Reference for Pizza material
    /// </summary>
    public Material pizzaMaterial;

    /// <summary>
    /// Reference for items material
    /// </summary>
    public Material itemsMaterial;
    public Texture2D [] itemsTexture;


    /// <summary>
    /// Reference for Pizza Transform Animnation
    /// </summary>
    public Texture2D [] pizzaTransformAnimTextures;

    /// <summary>
    /// Reference for Cheese
    /// </summary>
    public GameObject cheese;

    /// <summary>
    /// Reference for Cheese
    /// </summary>
    public GameObject tommato;

    /// <summary>
    /// Reference for Cheese
    /// </summary>
    public GameObject olives;

    /// <summary>
    /// Reference for Cheese
    /// </summary>
    public GameObject leaves;

    /// <summary>
    /// Reference for Cheese
    /// </summary>
    public GameObject sosages01;

    /// <summary>
    /// Reference for Cheese
    /// </summary>
    public GameObject sosages02;

    /// <summary>
    /// Selected Object
    /// </summary>
    GameObject selectedItem;

    /// <summary>
    /// Access the Shadow plane
    /// </summary>
    GameObject shadowPlane;

    /// <summary>
    /// Fire particles
    /// </summary>
    public ParticleSystem fire;

    /// <summary>
    /// Smoke particles
    /// </summary>
    public ParticleSystem smoke;

    /// <summary>
    /// Smoke particles
    /// </summary>
    public AudioSource fireSound;

    /// <summary>
    /// Smoke particles
    /// </summary>
    public Button processButton;

    /// <summary>
    /// Access the Touch indicator
    /// </summary>
    GameObject touchIndicator;

    /// <summary>
    /// Access the Touch indicator
    /// </summary>
    public GameObject [] instructionPanels;

    public Text t;

    /// <summary>
    /// Access the Pizza topping adding space 
    /// </summary>
    public GameObject pizzaToppingArea;

    /// <summary>
    /// Access the Pizza topping adding space 
    /// </summary>
    bool isFirstItemAdded;

    [SerializeField]
    private Button[] toppinButtons;

    bool isprocessed = false;

    // Start is called before the first frame update
    void Awake()
    {
        fireSound.Stop();
        pizzaMaterial.mainTexture = pizzaTransformAnimTextures[0];
        isFirstItemAdded = false;
        itemsMaterial.mainTexture = itemsTexture[0];
    }


    // Update is called once per frame
    void Update()
    {
        if (isprocessed)
        {
            foreach (Button toppinButton in toppinButtons)
            {
                toppinButton.interactable = false;
            }
        }

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit Hit;

            if (Physics.Raycast(ray, out Hit) && !PlaceOnPlane.IsPointerOverUIObject())
            {
                
                string hitName = Hit.transform.GetComponent<Collider>().name;
                
                if (hitName != "PizzaObject(Clone)" && !isprocessed)
                {
                    GameObject item = Instantiate(selectedItem, new Vector3(0, 0.1f, 0) + Hit.point, Random.rotation);
                    item.transform.parent = gameObject.transform;
                    instructionPanels[1].SetActive(false);
                    instructionPanels[0].SetActive(false);
                    isFirstItemAdded = true;
                    touchIndicator.SetActive(false);
                }
            }

        }
    }

    /// <summary>
    /// Adding Cheese 
    /// </summary>
    public void AddingCheese()
    {
        selectedItem = cheese;
        if (!isFirstItemAdded)
        {
            instructionPanels[0].SetActive(false);
            instructionPanels[1].SetActive(true);
        }
        
        pizzaToppingArea.transform.localScale = new Vector3(0.96f, 1.24f, 0.96f);
        EnableProcessButton();
    }

    /// <summary>
    /// Adding Tommato
    /// </summary>
    public void AddingTommato()
    {
        selectedItem = tommato;
        if (!isFirstItemAdded)
        {
            instructionPanels[0].SetActive(false);
            instructionPanels[1].SetActive(true);
        }
        pizzaToppingArea.transform.localScale = new Vector3(0.9f, 1.24f, 0.9f);
        EnableProcessButton();
    }

    /// <summary>
    /// Adding Olive 
    /// </summary>
    public void AddingOlive()
    {
        selectedItem = olives;
        if (!isFirstItemAdded)
        {
            instructionPanels[0].SetActive(false);
            instructionPanels[1].SetActive(true);
        }
        pizzaToppingArea.transform.localScale = new Vector3(1.15f, 1.24f, 1.15f);
        EnableProcessButton();
    }

    /// <summary>
    /// Adding sosages01
    /// </summary>
    public void AddingSausages01()
    {
        selectedItem = sosages01;
        if (!isFirstItemAdded)
        {
            instructionPanels[0].SetActive(false);
            instructionPanels[1].SetActive(true);
        }
        pizzaToppingArea.transform.localScale = new Vector3(0.9f, 1.24f, 0.9f);
        EnableProcessButton();
    }

    /// <summary>
    /// Adding sosages02 
    /// </summary>
    public void AddingSausages02()
    {
        selectedItem = sosages02;
        if (!isFirstItemAdded)
        {
            instructionPanels[0].SetActive(false);
            instructionPanels[1].SetActive(true);
        }
        pizzaToppingArea.transform.localScale = new Vector3(0.9f, 1.24f, 0.9f);
        EnableProcessButton();
    }

    /// <summary>
    /// Adding leaves
    /// </summary>
    public void AddingLeaves()
    {
        selectedItem = leaves;
        if (!isFirstItemAdded)
        {
            instructionPanels[0].SetActive(false);
            instructionPanels[1].SetActive(true);
        }
        pizzaToppingArea.transform.localScale = new Vector3(1.15f, 1.24f, 1.15f);
        EnableProcessButton();
    }

    /// <summary>
    /// Adding leaves
    /// </summary>
    public void Process()
    {
        StartCoroutine(WaitForProcess());
        isprocessed = true;
    }

    IEnumerator WaitForProcess()
    {
        fire.Play();
        fireSound.Play();
        yield return new WaitForSeconds(1);
        fire.Stop();
        PizzaTransform();
        itemsMaterial.mainTexture = itemsTexture[1];
        yield return new WaitForSeconds(0.2f);
        fireSound.Stop();
        smoke.Play();
        processButton.interactable = false;
    }

    public void PizzaTransform()
    {
        for(int i=0; i< pizzaTransformAnimTextures.Length; i++)
        {
            pizzaMaterial.mainTexture = pizzaTransformAnimTextures[i];
        }
    }

    public void EnableProcessButton()
    {
        processButton.interactable = true;
    }
}
