using System.IO;
using UnityEngine;

public class BaseDatos
{
    static string rutaArchivo = Path.Combine(Application.persistentDataPath, "link.json");

    public static Individuo GetData()
    {
        Individuo individuo;

        if (File.Exists(rutaArchivo))
        {
            string json = File.ReadAllText(rutaArchivo);
            individuo = JsonUtility.FromJson<Individuo>(json);
        }
        else individuo = new Individuo("Link", 10, 7603);

        if (individuo.casco == "Zora Helm") individuo.casco = "Capucha Zora";
        if (individuo.peto == "Zora Armor") individuo.peto = "Armadura Zora";
        if (individuo.pantalon == "Zora Greaves") individuo.pantalon = "Grebas Zora";
        if (individuo.escudo == "Ancient Shield") individuo.escudo = "Escudo ancestral";
        if (individuo.arma == "Master Sword") individuo.arma = "Espada Maestra";

        return individuo;
    }

    public static void GuardarData(Individuo individuo)
    {
        string json = JsonUtility.ToJson(individuo, true);
        File.WriteAllText(rutaArchivo, json);
        Debug.Log("Datos guardados en " + rutaArchivo);
    }
}
