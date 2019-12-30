using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class ProcessAudioData : MonoBehaviour {
    private Camera camera;
    private int spectrumLength = 256 * 2;
    private float [] spectrum;
    [SerializeField] private GameObject cube;
    private GameObject[] cubes;
    [SerializeField] private int scaleOffset = 1000;
    private float distanceOffset = 100;
    [SerializeField] private int scale = 10;
    private void Awake() {
        camera = Camera.main;
        spectrum = new float[spectrumLength];
        cubes = new GameObject[spectrumLength];
        float degrees = spectrumLength / 360;
        distanceOffset = cube.transform.localScale.magnitude * 50;
        for (int i = 0; i < spectrum.Length; i++) {
            GameObject go = Instantiate(cube, transform, true);
            go.name = "Cube_" + i;
            transform.eulerAngles = new Vector3(0, degrees * i, 0);
            go.transform.position = Vector3.forward * distanceOffset;
            go.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            cubes[i] = go;
        }
    }

    private void Update() {
        camera.transform.Rotate(0, 0.25f, 0);
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        for (int i = 0; i < spectrum.Length; i++) {
            if (cubes[i] != null) {
                cubes[i].transform.localScale = new Vector3(cubes[i].transform.localScale.x, spectrum[i] * scaleOffset, cubes[i].transform.localScale.z);
                // cubes[i].transform.localScale = new Vector3(scale, spectrum[i] * scaleOffset, scale);
            }
        }
    }
}