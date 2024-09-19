using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KDicom;

public class DisplayDicomImage2DPlane : MonoBehaviour
{
    public GameObject DICOMLoader_GO;
    [SerializeField]
    MPRType m_MPR_Type = MPRType.Axial;
    
    [SerializeField]
    bool m_AutoIndex = false;
    
    bool m_Reverse = false;
    float m_ElapsedTime = 0.0f;

    [SerializeField]
    int m_Index;

    IDicomVolume m_DicomVolume;
    Texture2D m_Texture = null;

    public enum MPRType
    {
        Axial,
        Coronal,
        Sagittal,
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DicomVolume == null)
        {
            FindDicomVolume();
            return;
        }

        if (m_AutoIndex)
        {
            int max = 0;
            switch (m_MPR_Type)
            {
                case MPRType.Axial:
                    max = m_DicomVolume.Depth;
                    break;
                case MPRType.Coronal:
                    max = m_DicomVolume.Height;
                    break;
                case MPRType.Sagittal:
                    max = m_DicomVolume.Width;
                    break;
            }

            if (m_Index < 0)
                m_Index = 0;
            if (m_Index > max)
                m_Index = max;

            m_ElapsedTime += Time.deltaTime * max / 5.0f;
            var index = (int)m_ElapsedTime;
            if (m_Reverse)
            {
                m_Index -= index;
                m_ElapsedTime -= index;
                if (m_Index < 0)
                {
                    m_Index = 0;
                    m_Reverse = false;

                    System.GC.Collect();
                    Resources.UnloadUnusedAssets();
                }
                UpdateImage();
                
            }
            else
            {
                m_Index += index;
                m_ElapsedTime -= index;
                if (m_Index >= max)
                {
                    m_Index = max;

                    m_Reverse = true;

                    System.GC.Collect();
                    Resources.UnloadUnusedAssets();
                }
                UpdateImage();
                
            }
        }

        else
        {
            switch (m_MPR_Type)
            {
                case MPRType.Axial:
                    m_Index = DICOMLoader_GO.transform.Find("Axial").GetComponent<DisplayDicomImage3DPlane>().m_Index;
                    if (m_Index < 0 || m_DicomVolume.Depth <= m_Index)
                        return;
                    break;
                case MPRType.Coronal:
                    m_Index = DICOMLoader_GO.transform.Find("Coronal").GetComponent<DisplayDicomImage3DPlane>().m_Index;
                    if (m_Index < 0 || m_DicomVolume.Height <= m_Index)
                        return;
                    break;
                case MPRType.Sagittal:
                    m_Index = DICOMLoader_GO.transform.Find("Sagittal").GetComponent<DisplayDicomImage3DPlane>().m_Index;
                    if (m_Index < 0 || m_DicomVolume.Width <= m_Index)
                        return;
                    break;
            }
            UpdateImage();
        }

    }


    void UpdateImage()
    {
        if (m_DicomVolume == null)
            return;
        
        // Get image
        IDicomImage image = null;
        switch (m_MPR_Type)
        {
            case MPRType.Axial:
                if (m_Index < 0 || m_DicomVolume.Depth <= m_Index)
                    return;

                image = m_DicomVolume.ToDicomImageAxial(m_Index);
                break;
            case MPRType.Coronal:
                if (m_Index < 0 || m_DicomVolume.Height <= m_Index)
                    return;

                image = m_DicomVolume.ToDicomImagelCoronal(m_Index);
                break;
            case MPRType.Sagittal:
                if (m_Index < 0 || m_DicomVolume.Width <= m_Index)
                    return;

                image = m_DicomVolume.ToDicomImagelSagittal(m_Index);
                break;
        }

        // Failed to load
        if (image == null) {
            return;
        }

        // Set texture to Image
        if( m_Texture == null ) {
            m_Texture = image.ToTexture2D();
        }
        else {
            m_Texture.Reinitialize(image.Width, image.Height);
            m_Texture.SetPixels32(image.GeneratePixels());
            m_Texture.Apply();
        }

        Sprite texture_sprite = Sprite.Create(m_Texture, new Rect(0, 0, m_Texture.width, m_Texture.height), Vector2.zero);
        this.GetComponent<Image>().sprite = texture_sprite;
        
    }

    void FindDicomVolume()
    {
        var dicomVolumeLoaderBehaviour = DICOMLoader_GO.GetComponent<DicomVolumeLoader>();
        if (dicomVolumeLoaderBehaviour == null)
        {
            return;
        }

        m_DicomVolume = dicomVolumeLoaderBehaviour.DicomVolume;

        if (m_DicomVolume == null)
            return;

        UpdateImage();
    }

}
