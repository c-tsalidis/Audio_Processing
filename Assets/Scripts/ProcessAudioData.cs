using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[RequireComponent(typeof(AudioSource))]
public class ProcessAudioData : MonoBehaviour {
    private Camera camera;
    [SerializeField] private int numRows = 10;
    private Row [] rows;
    private int spectrumLength = 256;
    [SerializeField] private GameObject cube;
    private GameObject[] cubes;
    [SerializeField] private int scaleOffset = 1000;
    private float distanceOffset;
    [SerializeField] private int scale;
    [SerializeField] private int rotationSpeed = 10;
    private GradientColor gradientColor;
    private void Awake() {
        camera = Camera.main;
        rows = new Row[numRows];
        gradientColor = GetComponent<GradientColor>();
        distanceOffset = cube.transform.localScale.x;
        // camera.transform.rotation = Quaternion.identity;
        // scale = (int) (spectrumLength / distanceOffset);
        for (int i = 0; i < numRows; i++) {
            GameObject level = new GameObject();
            level.transform.SetParent(transform);
            level.name = "Level_" + i;
            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(0, 0, - i * 2);
            rows[i] = new Row(transform, level.transform, spectrumLength, cube, i + 1, Random.ColorHSV());
        }
    }


    private void Update() {
        float [] spectrum = new float[spectrumLength];
        // camera.transform.position = Vector3.up;
        // camera.transform.Rotate(0, 0.25f, 0);
        // camera.transform.LookAt(Vector3.zero);
        // camera.transform.Translate(Vector3.right * Time.deltaTime * cameraRotationSpeed);
        transform.Rotate(0.0f, rotationSpeed * Time.deltaTime, 0.0f);
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        foreach (var row in rows) row.Update(spectrum, scaleOffset);
    }
    
    private class Row {
        private int rowNumber;
        private GameObject [] cubes;
        public Row(Transform transform, Transform levelTransform, int spectLength, GameObject cube, int _rowNumber, Color c) {
            this.cubes = new GameObject[spectLength];
            // float degrees = spectrumLength < 360 ? 360.0f / spectrumLength : spectrumLength / 360.0f;
            float degrees = 360.0f / spectLength;
            for (int i = 0; i < spectLength; i++) {
                GameObject go = Instantiate(cube, levelTransform, true);
                go.name = "Cube_" + i;
                transform.eulerAngles = new Vector3(0, i * degrees, 0);
                go.transform.position = Vector3.forward * 50;
                // if(c == null) 
                    c = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                go.GetComponent<Renderer>().material.color = c;
                go.GetComponent<Renderer>().material.SetColor("_EmissionColor", c);
                cubes[i] = go;
            }
            this.rowNumber = _rowNumber;
        }

        public void Update(float [] spectrum, int scaleOffset) {
            for (int i = 0; i < spectrum.Length; i++) {
                if (this.cubes[i] != null) {
                    // Debug.Log(spectrum[i]);
                    this.cubes[i].transform.localScale = new Vector3(cubes[i].transform.localScale.x, spectrum[i] * scaleOffset * this.rowNumber,
                        cubes[i].transform.localScale.z);
                    // cubes[i].transform.localScale = new Vector3(distanceOffset, spectrum[i] * scaleOffset, distanceOffset);
                }
            }
        }
    }
}

