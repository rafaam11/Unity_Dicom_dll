using System.IO;
using System.IO.Compression;
using UnityEngine;

public class UnzipDicom : MonoBehaviour
{
    // 압축을 해제할 zip 파일 경로
    public string zipFilePath = "Assets/StreamingAssets/Name.zip";

    // 압축을 해제할 목적지 폴더 경로 (zip 파일이 있는 경로에 {file} 폴더를 만들 것임)
    public string extractFolderPath;

    void Start()
    {
        // 압축을 해제할 목적지 폴더 경로를 설정
        SetExtractFolderPath();

        // 압축 해제
        ExtractZipFile(zipFilePath, extractFolderPath);
    }

    public void SetExtractFolderPath()
    {
        // zip 파일이 있는 디렉토리 경로
        string zipDirectory = Path.GetDirectoryName(zipFilePath);

        // zip 파일의 파일명(확장자 제외)
        string zipFileName = Path.GetFileNameWithoutExtension(zipFilePath);

        // {file}에 해당하는 폴더 경로 생성
        extractFolderPath = Path.Combine(zipDirectory, zipFileName);

        // 해당 경로가 존재하는지 확인하고 없으면 폴더 생성
        if (!Directory.Exists(extractFolderPath))
        {
            Directory.CreateDirectory(extractFolderPath);
        }

        Debug.Log($"압축 해제할 폴더 경로: {extractFolderPath}");
    }

    public void ExtractZipFile(string zipPath, string extractPath)
    {
        // zip 파일 압축 해제
        try
        {
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            Debug.Log("Zip 파일 압축 해제 완료!");
        }
        catch (IOException e)
        {
            Debug.LogError($"압축 해제 중 오류 발생: {e.Message}");
        }
    }
}
