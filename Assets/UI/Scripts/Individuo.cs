using System;

[Serializable]
public class Individuo
{
    public string nombre;
    public int vidas;
    public int rupias;
    public string casco;
    public string peto;
    public string pantalon;
    public string escudo;
    public string arma;

    public Individuo(string nombre, int vidas, int rupias)
    {
        this.nombre = nombre;
        this.vidas = vidas;
        this.rupias = rupias;
        casco = "Zora Helm";
        peto = "Zora Armor";
        pantalon = "Zora Greaves";
        escudo = "Ancient Shield";
        arma = "Master Sword";
    }
}
