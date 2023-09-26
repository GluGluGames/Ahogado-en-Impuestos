using System.IO;
using TMPro;
using UnityEngine;

public class LogInManager : MonoBehaviour
{
    private class User
    {
        // Unity to JSON method requires atributes to be public :)
        public string userName;

        public string sex;
        public int age = 10;

        public void SetUserName(string userName)
        { this.userName = userName; }

        public void SetSex(string sex)
        { this.sex = sex; }

        public void SetAge(int age)
        { this.age = age; }

        public int GetAge()
        { return age; }

        public string GetSex()
        { return sex; }

        public string GetUserName()
        { return userName; }

        public void PlusOneAge()
        { age++; }

        public void MinusOneAge()
        { age--; }
    }

    [SerializeField]
    private TextMeshProUGUI _questionText;

    [SerializeField]
    private TextMeshProUGUI ageText;

    private User user = new User();

    [SerializeField]
    private GameObject[] LogInUI_Items;

    public void setSex(string sex)
    {
        user.SetSex(sex);
        _questionText.text = "¿Cuál es tu edad?";

        for (int i = 0; i < LogInUI_Items.Length; i++)
        {
            bool active = i >= 2;
            LogInUI_Items[i].SetActive(active);
        }
    }

    public void PlusOneAge()
    {
        user.PlusOneAge();
        UpdateAgeText();
    }

    public void MinusOneAge()
    {
        user.MinusOneAge();
        UpdateAgeText();
    }

    private void UpdateAgeText()
    {
        ageText.text = user.GetAge().ToString();
    }

    public void lockAge()
    {
        _questionText.text = "¿Cuál es tu nombre?";
    }

    public void SetUserName(string userName)
    {
        user.SetUserName(userName);
    }

    public void SerializeJSON()
    {
        try
        {
            // Path to the file, persistentData path is required since we dont have admission to write everywhere
            string filePath = Path.Combine(Application.persistentDataPath, "userData.json");
            string userToJson = JsonUtility.ToJson(user);
            File.WriteAllText(filePath, userToJson);

            Debug.Log($"JSON guardado en {filePath}");
        }
        catch { }
    }

    public void DeserializeJSON()
    {
        // Path to the file, persistentData path is required since we dont have admission to write everywhere
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, "userData.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                user = JsonUtility.FromJson<User>(json);

                // Acceder a los datos del usuario
                Debug.Log($"Nombre: {user.userName}, Sexo: {user.sex}, Edad: {user.age}");
            }
            else
            {
                Debug.LogWarning("El archivo JSON no existe.");
            }
        }
        catch { }
    }
}