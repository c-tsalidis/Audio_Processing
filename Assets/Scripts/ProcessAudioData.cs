using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class ProcessAudioData : MonoBehaviour {
    private Camera camera;
    public int numRows;
    private Row [] rows;
    private GameObject [] rowsGO;
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
        rowsGO = new GameObject[numRows];
        gradientColor = GetComponent<GradientColor>();
        distanceOffset = cube.transform.localScale.x;
        // camera.transform.rotation = Quaternion.identity;
        // scale = (int) (spectrumLength / distanceOffset);
        for (int i = 0; i < numRows; i++) {
            GameObject level = new GameObject();
            level.transform.SetParent(transform);
            level.name = "Level_" + i;
            rowsGO[i] = level;
            transform.rotation = Quaternion.identity;
            transform.position = new Vector3(0, 0, - i * 10);
            rows[i] = new Row(this.gameObject, i + 1);
            rows[i].CreateRow(level.transform, spectrumLength, cube, Random.ColorHSV());
        }
    }


    private void Update() {
        float [] spectrum = new float[spectrumLength];
        // camera.transform.position = Vector3.up;
        // camera.transform.Rotate(0, 0.25f, 0);
        // camera.transform.LookAt(Vector3.zero);
        // camera.transform.Translate(Vector3.right * Time.deltaTime * cameraRotationSpeed);
        // transform.Rotate(0.0f, rotationSpeed * Time.deltaTime, 0.0f);
        
        this.transform.GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        ClassifyRowValues(spectrum, scaleOffset);
        /*
        int counter = 0;
        foreach (var row in rows) {
            Vector3 rotation;
            if (counter % 2 == 0) rotation = Vector3.up;
            else rotation = Vector3.down;
            // rowsGO[counter].transform.Rotate(rotation);
            row.Update(spectrum, scaleOffset);
            counter++;
        }
        */
        /*
        for (int i = 1; i < spectrum.Length - 1; i++) {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }
        */
        /*
        foreach (var row in rows) {
            float [] newSpectrum = ModifySpectrum(spectrum);
            row.Update(newSpectrum, scaleOffset);
        }
        */
    }

    private void ClassifyRowValues(float [] s, int scaleOffset) {
        Array.Sort(s);
        // float min = s[0];
        // float median = s[s.Length / 2];
        float maximum = s[s.Length - 1];
        int l = s.Length / numRows;
        // float [] newSpectrum = new float[s.Length / numRows];
        List<float> newSpectrum = new List<float>();
        int intervalCounter = 1;
        float previousMax = 0;
        int correctCounter = 0;
        foreach (var row in rows) {
            int i = 0;
            float min = previousMax;
            Debug.Log(min + " in level " + intervalCounter);
            float max = s[l * intervalCounter - 1];
            foreach (var value in s) {
                // if (intervalCounter == row.rowNumber) {
                if (value >= min && value < max) {
                    try {
                        // newSpectrum[i] = value;
                        newSpectrum.Add(value);
                        previousMax = value;
                    }
                    catch (Exception e) {
                        Debug.Log(i);
                        // throw;
                    }
                }
                // else break;
                // }
                i++;
            }

            float[] n = newSpectrum.ToArray();
            row.Update(n, scaleOffset);
            intervalCounter++;
        }
    }

    private float [] ModifySpectrum(float [] spectrum) {
        Array.Sort(spectrum);
        float [] values = new float[5];
        float [] newSpectrum = new float[spectrum.Length];
        values[0] = spectrum[0]; // minimum value
        values[2] = (spectrum[spectrum.Length / 2] + spectrum[spectrum.Length / 2 + 1]) / 2; // the median value
        // values[4] = spectrum[spectrum.Length - 1]; // max value
        values[4] = 10;
        values[1] = (spectrum[spectrum.Length / 4] + spectrum[spectrum.Length / 4 + 1]) / 2;
        // values[3] = spectrum[spectrum.Length - 1] * 0.75f;
        values[3] = 5;
        for (int i = 0; i < newSpectrum.Length; i++) {
            newSpectrum[i] = values[Random.Range(0, values.Length)] / values[4];
        }
        return newSpectrum;
    }

    private class Row {
        public int rowNumber;
        private GameObject [] cubes;
        private GameObject audioProcessor;
        
        public Row(GameObject audioProcessor, int rowNumber) {
            this.audioProcessor = audioProcessor;
            this.rowNumber = rowNumber;
        }

        public void CreateRow(Transform levelTransform, int spectLength, GameObject cube, Color c) {
            this.cubes = new GameObject[spectLength];
            // float degrees = spectrumLength < 360 ? 360.0f / spectrumLength : spectrumLength / 360.0f;
            float degrees = 360.0f / spectLength;
            for (int i = 0; i < spectLength; i++) {
                GameObject go = Instantiate(cube, levelTransform, true);
                go.name = "Cube_" + i;
                audioProcessor.transform.eulerAngles = new Vector3(0, i * degrees, 0);
                go.transform.position = Vector3.forward * 50;
                if(c == null) 
                    c = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                go.GetComponent<Renderer>().material.color = c;
                go.GetComponent<Renderer>().material.SetColor("_EmissionColor", c * 10);
                cubes[i] = go;
            }
        }
            /*
        public void Update(float [] spectrum, int scaleOffset) {
            for (int i = 0; i < spectrum.Length; i++) {
                if (this.cubes[i] != null) {
                    // Debug.Log(spectrum[i]);
                    this.cubes[i].transform.localScale = new Vector3(cubes[i].transform.localScale.x, spectrum[i] * scaleOffset * this.rowNumber, cubes[i].transform.localScale.z);
                    // cubes[i].transform.localScale = new Vector3(distanceOffset, spectrum[i] * scaleOffset, distanceOffset);
                }
            }
        }
            */

        public void Update(float [] rowSpectrum, int scaleOffset) {
            int i = 0;
            foreach (var cube in this.cubes) {
                cube.transform.localScale = new Vector3(cube.transform.localScale.x, rowSpectrum[i] * scaleOffset, cube.transform.localScale.z);
                if (i >= (rowSpectrum.Length - 1)) i = 0;
                else i++;
            }
        }
    }
}

