using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Button))]
    public class DeleteProgressConfirm : MonoBehaviour
    {
        public Action OnConfirm;

        public void OnDataDeleteConfirmButton()
        {
            foreach (var directory in Directory.GetDirectories(Application.persistentDataPath))
            {
                DirectoryInfo data_dir = new(directory);
                data_dir.Delete(true);
            }

            foreach (var file in Directory.GetFiles(Application.persistentDataPath))
            {
                FileInfo file_info = new(file);
                file_info.Delete();
            }

            PlayerPrefs.DeleteAll();
            OnConfirm?.Invoke();
        }
    }
}
