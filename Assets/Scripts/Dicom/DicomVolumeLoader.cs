using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KDicom;

public class DicomVolumeLoader : MonoBehaviour {
    [SerializeField]
    string m_DicomDirectoryPath;

    IDicomVolume m_DicomVolume;
    public IDicomVolume DicomVolume { get { return m_DicomVolume; } }

    // Use this for initialization
    void Start()
    {
        
        // Load Dicom files
        var volumes = DicomLoader.LoadDicomVolumes(m_DicomDirectoryPath);

        int depthmax = 0;
        foreach (var vol in volumes)
        {
            if (vol.Depth > depthmax)
            {
                depthmax = vol.Depth;
                m_DicomVolume = vol;
            }
        }
        // Dispose unused volumes
        foreach (var vol in volumes)
        {
            if (vol != m_DicomVolume)
            {
                vol.Dispose();
            }
        }

        if (m_DicomVolume == null)
            return;
        
    }

    private void OnDestroy()
    {
        if (m_DicomVolume != null)
        {
            m_DicomVolume.Dispose();
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
