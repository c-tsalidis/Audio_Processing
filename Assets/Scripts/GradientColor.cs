using UnityEngine;

public class GradientColor : MonoBehaviour {
    private float i = 0; // counter to control the lerp
    [SerializeField] private Color startingColor;
    [SerializeField] private Color endingColor;
    [SerializeField] private float rate = 10; // number of times per second the new color is chosen 
    [SerializeField] private GameObject ground;
    private Material groundMaterial;
    [SerializeField] private Camera _camera;
    private Color newColor;

    void Start() {
        if(_camera == null) _camera = Camera.main;
        startingColor = new Color(Random.value, Random.value, Random.value);
        endingColor = new Color(Random.value, Random.value, Random.value);
        groundMaterial = ground.GetComponent<Renderer>().material;
    }

    void Update() {
        i += Time.deltaTime * rate;
        newColor = Color.Lerp(startingColor, endingColor, i);
        _camera.backgroundColor = newColor;
        groundMaterial.color = newColor;
        if (i >= 1) {
            i = 0;
            startingColor = new Color(Random.value, Random.value, Random.value);
            endingColor = new Color(Random.value, Random.value, Random.value);
        }
    }
}