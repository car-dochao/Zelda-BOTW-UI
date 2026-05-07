using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ZeldaUIController : MonoBehaviour
{
    class ItemData
    {
        public string nombre;
        public string categoria;
        public string hueco;
        public string descripcion;
        public Texture2D icono;

        public ItemData(string nombre, string categoria, string hueco, string descripcion, Texture2D icono)
        {
            this.nombre = nombre;
            this.categoria = categoria;
            this.hueco = hueco;
            this.descripcion = descripcion;
            this.icono = icono;
        }
    }

    class QuestData
    {
        public string titulo;
        public string lugar;
        public string descripcion;

        public QuestData(string titulo, string lugar, string descripcion)
        {
            this.titulo = titulo;
            this.lugar = lugar;
            this.descripcion = descripcion;
        }
    }

    VisualElement log;
    VisualElement inventory;
    VisualElement settings;
    VisualElement tabLeft;
    VisualElement tabRight;
    VisualElement titleBlock;
    Label leftText;
    Label rightText;
    Label title;

    VisualElement grid;
    VisualElement questList;
    VisualElement popupLayer;
    Button popupAction;
    Button popupCancel;
    Label itemName;
    Label itemDescription;
    Label questDetailTitle;
    Label questDetailSubtitle;
    Label questDetailBody;
    Hearts hearts;
    Rupees rupees;

    VisualElement tabWeapons;
    VisualElement tabShields;
    VisualElement tabArmors;
    VisualElement equipHead;
    VisualElement equipBody;
    VisualElement equipLegs;
    VisualElement equipShield;
    VisualElement equipWeapon;

    VisualTreeAsset itemTemplate;
    VisualTreeAsset questTemplate;

    List<ItemData> weapons = new List<ItemData>();
    List<ItemData> shields = new List<ItemData>();
    List<ItemData> armors = new List<ItemData>();
    List<QuestData> quests = new List<QuestData>();

    ItemData itemSeleccionado;
    QuestData questSeleccionada;
    Individuo link;
    int pagina = 1;
    int categoria = 2;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        log = root.Q<VisualElement>("Log");
        inventory = root.Q<VisualElement>("Inventory");
        settings = root.Q<VisualElement>("Settings");
        tabLeft = root.Q<VisualElement>("TabLeft");
        tabRight = root.Q<VisualElement>("TabRight");
        titleBlock = root.Q<VisualElement>("TitleBlock");
        leftText = root.Q<Label>("LeftText");
        rightText = root.Q<Label>("RightText");
        title = root.Q<Label>("Title");

        grid = root.Q<VisualElement>("Grid");
        questList = root.Q<VisualElement>("QuestList");
        popupLayer = root.Q<VisualElement>("PopupLayer");
        popupAction = root.Q<Button>("PopupAction");
        popupCancel = root.Q<Button>("PopupCancel");

        itemName = root.Q<Label>("ItemName");
        itemDescription = root.Q<Label>("ItemDescription");
        questDetailTitle = root.Q<Label>("QuestDetailTitle");
        questDetailSubtitle = root.Q<Label>("QuestDetailSubtitle");
        questDetailBody = root.Q<Label>("QuestDetailBody");

        hearts = root.Q<Hearts>("Hearts");
        rupees = root.Q<Rupees>("Rupees");
        tabWeapons = root.Q<VisualElement>("TabWeapons");
        tabShields = root.Q<VisualElement>("TabShields");
        tabArmors = root.Q<VisualElement>("TabArmors");
        equipHead = root.Q<VisualElement>("EquipHead");
        equipBody = root.Q<VisualElement>("EquipBody");
        equipLegs = root.Q<VisualElement>("EquipLegs");
        equipShield = root.Q<VisualElement>("EquipShield");
        equipWeapon = root.Q<VisualElement>("EquipWeapon");

        itemTemplate = Resources.Load<VisualTreeAsset>("Templates/ItemSlot");
        questTemplate = Resources.Load<VisualTreeAsset>("Templates/QuestEntry");
        link = BaseDatos.GetData();

        CrearItems();
        CrearMisiones();
        RegistrarEventos(root);
        MostrarPagina(1);
        MostrarCategoria(2);
        CargarMisiones();
        SeleccionarMision(0);
        ActualizarDatos();
        ActualizarEquipo();
    }

    void RegistrarEventos(VisualElement root)
    {
        tabLeft.RegisterCallback<ClickEvent>(evt => ClickPaginaIzda());
        tabRight.RegisterCallback<ClickEvent>(evt => ClickPaginaDcha());
        titleBlock.RegisterCallback<ClickEvent>(evt => MostrarPagina(1));
        tabWeapons.RegisterCallback<ClickEvent>(evt => MostrarCategoria(0));
        tabShields.RegisterCallback<ClickEvent>(evt => MostrarCategoria(1));
        tabArmors.RegisterCallback<ClickEvent>(evt => MostrarCategoria(2));
        root.Q<VisualElement>("ArrowLeft").RegisterCallback<ClickEvent>(evt => MostrarCategoria(categoria - 1));
        root.Q<VisualElement>("ArrowRight").RegisterCallback<ClickEvent>(evt => MostrarCategoria(categoria + 1));
        root.Q<Button>("SaveButton").RegisterCallback<ClickEvent>(evt => GuardarDatos());
        root.Q<Button>("AddLifeButton").RegisterCallback<ClickEvent>(evt => AnadirVida());
        root.Q<Button>("LifeButton").RegisterCallback<ClickEvent>(evt => QuitarVida());
        root.Q<Button>("RupeeButton").RegisterCallback<ClickEvent>(evt => SumarRupias());
        root.Q<Button>("RemoveRupeeButton").RegisterCallback<ClickEvent>(evt => QuitarRupias());
        popupAction.RegisterCallback<ClickEvent>(evt => AccionPopup());
        popupCancel.RegisterCallback<ClickEvent>(evt => popupLayer.style.display = DisplayStyle.None);
    }

    void ClickPaginaIzda()
    {
        if (pagina == 1) MostrarPagina(0);
        else if (pagina == 2) MostrarPagina(1);
    }

    void ClickPaginaDcha()
    {
        if (pagina == 0) MostrarPagina(1);
        else if (pagina == 1) MostrarPagina(2);
    }

    void MostrarPagina(int indice)
    {
        pagina = indice;
        log.style.display = DisplayStyle.None;
        inventory.style.display = DisplayStyle.None;
        settings.style.display = DisplayStyle.None;
        tabLeft.style.visibility = Visibility.Visible;
        tabRight.style.visibility = Visibility.Visible;

        if (pagina == 0)
        {
            log.style.display = DisplayStyle.Flex;
            title.text = "Diario de aventura";
            leftText.text = "";
            rightText.text = "Inventario";
            tabLeft.style.visibility = Visibility.Hidden;
        }
        else if (pagina == 1)
        {
            inventory.style.display = DisplayStyle.Flex;
            title.text = "Inventario";
            leftText.text = "Diario de aventura";
            rightText.text = "Sistema";
        }
        else if (pagina == 2)
        {
            settings.style.display = DisplayStyle.Flex;
            title.text = "Sistema";
            leftText.text = "Inventario";
            rightText.text = "";
            tabRight.style.visibility = Visibility.Hidden;
        }
    }

    void MostrarCategoria(int indice)
    {
        categoria = indice;

        if (categoria < 0) categoria = 2;
        else if (categoria > 2) categoria = 0;

        tabWeapons.RemoveFromClassList("subtab-selected");
        tabShields.RemoveFromClassList("subtab-selected");
        tabArmors.RemoveFromClassList("subtab-selected");

        if (categoria == 0) tabWeapons.AddToClassList("subtab-selected");
        else if (categoria == 1) tabShields.AddToClassList("subtab-selected");
        else if (categoria == 2) tabArmors.AddToClassList("subtab-selected");

        if (categoria == 0) CargarItems(weapons);
        else if (categoria == 1) CargarItems(shields);
        else if (categoria == 2) CargarItems(armors);
    }

    void CargarItems(List<ItemData> lista)
    {
        grid.Clear();

        for (int i = 0; i < lista.Count; i++)
        {
            VisualElement slotPlantilla = itemTemplate.Instantiate();
            VisualElement slot = slotPlantilla.ElementAt(0);
            slot.userData = lista[i];
            slot.Q<VisualElement>("Icon").style.backgroundImage = lista[i].icono;
            slot.RegisterCallback<ClickEvent>(SeleccionarItem);
            grid.Add(slot);
        }

        if (lista.Count > 0)
        {
            itemSeleccionado = lista[0];
            ActualizarItemInfo();
            ActualizarSeleccion();
        }
    }

    void SeleccionarItem(ClickEvent evt)
    {
        VisualElement slot = evt.currentTarget as VisualElement;
        itemSeleccionado = slot.userData as ItemData;
        ActualizarItemInfo();
        ActualizarSeleccion();
        AbrirPopup();
    }

    void ActualizarSeleccion()
    {
        for (int i = 0; i < grid.childCount; i++)
        {
            VisualElement slot = grid.ElementAt(i);
            ItemData item = slot.userData as ItemData;

            slot.RemoveFromClassList("item-slot-selected");

            if (item == itemSeleccionado)
                slot.AddToClassList("item-slot-selected");
        }
    }

    void AbrirPopup()
    {
        popupLayer.style.display = DisplayStyle.Flex;

        if (ItemEquipado(itemSeleccionado)) popupAction.text = "Desequipar";
        else popupAction.text = "Equipar";
    }

    void AccionPopup()
    {
        if (ItemEquipado(itemSeleccionado)) DesequiparItem(itemSeleccionado);
        else EquiparItem(itemSeleccionado);

        popupLayer.style.display = DisplayStyle.None;
        ActualizarEquipo();
        ActualizarSeleccion();
    }

    void EquiparItem(ItemData item)
    {
        if (item.hueco == "head") link.casco = item.nombre;
        else if (item.hueco == "body") link.peto = item.nombre;
        else if (item.hueco == "legs") link.pantalon = item.nombre;
        else if (item.hueco == "shield") link.escudo = item.nombre;
        else if (item.hueco == "weapon") link.arma = item.nombre;
    }

    void DesequiparItem(ItemData item)
    {
        if (item.hueco == "head" && link.casco == item.nombre) link.casco = "";
        else if (item.hueco == "body" && link.peto == item.nombre) link.peto = "";
        else if (item.hueco == "legs" && link.pantalon == item.nombre) link.pantalon = "";
        else if (item.hueco == "shield" && link.escudo == item.nombre) link.escudo = "";
        else if (item.hueco == "weapon" && link.arma == item.nombre) link.arma = "";
    }

    bool ItemEquipado(ItemData item)
    {
        bool equipado = false;

        if (item.hueco == "head" && link.casco == item.nombre) equipado = true;
        else if (item.hueco == "body" && link.peto == item.nombre) equipado = true;
        else if (item.hueco == "legs" && link.pantalon == item.nombre) equipado = true;
        else if (item.hueco == "shield" && link.escudo == item.nombre) equipado = true;
        else if (item.hueco == "weapon" && link.arma == item.nombre) equipado = true;

        return equipado;
    }

    void ActualizarEquipo()
    {
        ActualizarIconoEquipo(equipHead, link.casco, armors);
        ActualizarIconoEquipo(equipBody, link.peto, armors);
        ActualizarIconoEquipo(equipLegs, link.pantalon, armors);
        ActualizarIconoEquipo(equipShield, link.escudo, shields);
        ActualizarIconoEquipo(equipWeapon, link.arma, weapons);
    }

    void ActualizarIconoEquipo(VisualElement slot, string nombre, List<ItemData> lista)
    {
        ItemData item = BuscarItem(nombre, lista);

        if (item != null)
        {
            slot.style.backgroundImage = item.icono;
            slot.style.opacity = 1.0f;
        }
        else
        {
            slot.style.backgroundImage = null;
            slot.style.opacity = 1.0f;
        }
    }

    ItemData BuscarItem(string nombre, List<ItemData> lista)
    {
        ItemData item = null;
        bool encontrado = false;
        int i = 0;

        while (i < lista.Count && !encontrado)
        {
            if (lista[i].nombre == nombre)
            {
                item = lista[i];
                encontrado = true;
            }
            else i++;
        }

        return item;
    }

    void ActualizarItemInfo()
    {
        itemName.text = itemSeleccionado.nombre;
        itemDescription.text = itemSeleccionado.descripcion;
    }

    void GuardarDatos()
    {
        BaseDatos.GuardarData(link);
    }

    void QuitarVida()
    {
        if (link.vidas > 0)
            link.vidas = link.vidas - 1;

        ActualizarDatos();
    }

    void AnadirVida()
    {
        if (link.vidas < hearts.Max)
            link.vidas = link.vidas + 1;

        ActualizarDatos();
    }

    void SumarRupias()
    {
        link.rupias = link.rupias + 10;
        ActualizarDatos();
    }

    void QuitarRupias()
    {
        if (link.rupias >= 10) link.rupias = link.rupias - 10;
        else link.rupias = 0;

        ActualizarDatos();
    }

    void ActualizarDatos()
    {
        hearts.Current = link.vidas;
        rupees.Value = link.rupias;
    }

    void CargarMisiones()
    {
        questList.Clear();

        for (int i = 0; i < quests.Count; i++)
        {
            VisualElement questPlantilla = questTemplate.Instantiate();
            VisualElement quest = questPlantilla.ElementAt(0);
            quest.userData = quests[i];
            quest.Q<Label>("Title").text = quests[i].titulo;
            quest.Q<Label>("Subtitle").text = quests[i].lugar;
            quest.RegisterCallback<ClickEvent>(SeleccionarMision);
            questList.Add(quest);
        }
    }

    void SeleccionarMision(ClickEvent evt)
    {
        VisualElement quest = evt.currentTarget as VisualElement;
        questSeleccionada = quest.userData as QuestData;
        ActualizarMision();
    }

    void SeleccionarMision(int indice)
    {
        questSeleccionada = quests[indice];
        ActualizarMision();
    }

    void ActualizarMision()
    {
        questDetailTitle.text = questSeleccionada.titulo;
        questDetailSubtitle.text = questSeleccionada.lugar;
        questDetailBody.text = questSeleccionada.descripcion;

        for (int i = 0; i < questList.childCount; i++)
        {
            VisualElement quest = questList.ElementAt(i);
            QuestData data = quest.userData as QuestData;

            quest.RemoveFromClassList("quest-entry-selected");

            if (data == questSeleccionada)
                quest.AddToClassList("quest-entry-selected");
        }
    }

    void CrearItems()
    {
        armors.Add(new ItemData("Capucha Zora", "armor", "head", "Prenda zora elaborada con escamas de dragón.\nAyuda a nadar más rápido y permite ejecutar torbellinos en el agua.", Resources.Load<Texture2D>("Images/items/armors/BotW_Zora_Helm_Icon")));
        armors.Add(new ItemData("Armadura Zora", "armor", "body", "Elaborada generación tras generación por las princesas zora para sus futuros esposos.\nAl vestirla se puede ascender por cascadas.", Resources.Load<Texture2D>("Images/items/armors/BotW_Zora_Armor_Icon")));
        armors.Add(new ItemData("Grebas Zora", "armor", "legs", "Prenda heredada entre los zora.\nLas escamas de dragón que supuestamente la componen permiten nadar más rápido.", Resources.Load<Texture2D>("Images/items/armors/BotW_Zora_Greaves_Icon")));
        armors.Add(new ItemData("Túnica del elegido", "armor", "body", "Prenda que recibieron los elegidos de Hyrule.\nPermite ver la energía vital de los enemigos.", Resources.Load<Texture2D>("Images/items/armors/BotW_Champion's_Tunic_Icon")));
        armors.Add(new ItemData("Gorra del héroe", "armor", "head", "Gorra de un héroe de tiempos remotos.\nEs sencilla, cómoda y conserva el espíritu de antiguas aventuras.", Resources.Load<Texture2D>("Images/items/armors/BotW_Cap_of_the_Hero_Icon")));
        armors.Add(new ItemData("Pantalón hyliano", "armor", "legs", "Prenda tradicional en todo Hyrule.\nSu tejido resistente y suave resulta muy apreciado entre los viajeros.", Resources.Load<Texture2D>("Images/items/armors/BotW_Hylian_Trousers_Icon")));
        armors.Add(new ItemData("Casco ignífugo", "armor", "head", "Accesorio de piedra elaborado para turistas que visitan Ciudad Goron.\nCubre toda la cabeza y es resistente a las llamas.", Resources.Load<Texture2D>("Images/items/armors/BotW_Flamebreaker_Helm_Icon")));
        armors.Add(new ItemData("Armadura ignífuga", "armor", "body", "Armadura fabricada especialmente para quienes se aventuren hasta Ciudad Goron.\nAl estar elaborada a base de piedras, ofrece resistencia al fuego.", Resources.Load<Texture2D>("Images/items/armors/BotW_Flamebreaker_Armor_Icon")));
        armors.Add(new ItemData("Grebas ignífugas", "armor", "legs", "Grebas de piedra elaboradas para atravesar zonas volcánicas.\nAyudan a resistir el calor extremo de la Montaña de la Muerte.", Resources.Load<Texture2D>("Images/items/armors/BotW_Flamebreaker_Boots_Icon")));
        armors.Add(new ItemData("Pañuelo de escalada", "armor", "head", "Pañuelo normal a simple vista, pero mejora el equilibrio de su portador.\nEs un accesorio perfecto para escalar.", Resources.Load<Texture2D>("Images/items/armors/BotW_Climber's_Bandanna_Icon")));
        armors.Add(new ItemData("Camiseta de escalada", "armor", "body", "Atuendo para trepar elaborado mediante tecnología ancestral.\nSus guantes evitan que Link resbale al escalar.", Resources.Load<Texture2D>("Images/items/armors/BotW_Climbing_Gear_Icon")));
        armors.Add(new ItemData("Mallas de escalada", "armor", "legs", "Prenda diseñada para trepar por muros y riscos.\nLas zapatillas cuentan con una sujeción especial que evita resbalar.", Resources.Load<Texture2D>("Images/items/armors/BotW_Climbing_Boots_Icon")));

        shields.Add(new ItemData("Escudo de madera", "shield", "shield", "Escudo sencillo y fácil de manejar, elaborado con madera ligera.\nPuede proteger de ataques leves, como por ejemplo flechas.", Resources.Load<Texture2D>("Images/items/shields/BotW_Wooden_Shield_Icon")));
        shields.Add(new ItemData("Escudo de viajero", "shield", "shield", "Sólido escudo muy apreciado por viajeros.\nElaborado en madera y reforzado con cuero.", Resources.Load<Texture2D>("Images/items/shields/BotW_Traveler's_Shield_Icon")));
        shields.Add(new ItemData("Escudo gerudo", "shield", "shield", "Escudo metálico adaptado al combate cuerpo a cuerpo de la tribu gerudo.\nEs apreciado por soldados y viajeros.", Resources.Load<Texture2D>("Images/items/shields/BotW_Gerudo_Shield_Icon")));
        shields.Add(new ItemData("Escudo ancestral", "shield", "shield", "Escudo hecho usando tecnología sheikah antigua.\nSu funcionalidad mejorada le permite desviar los rayos de los guardianes.", Resources.Load<Texture2D>("Images/items/shields/BotW_Ancient_Shield_Icon")));

        weapons.Add(new ItemData("Rama de árbol", "weapon", "weapon", "Rama de árbol normal y corriente.\nTambién puede hacer las veces de arma o de proyectil.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Tree_Branch_Icon")));
        weapons.Add(new ItemData("Espada de soldado", "weapon", "weapon", "Espada que portaban los soldados que defendían el castillo de Hyrule.\nSu hoja metálica es perfecta para luchar contra monstruos.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Soldier's_Broadsword_Icon")));
        weapons.Add(new ItemData("Mandoble real", "weapon", "weapon", "Espada a dos manos que la familia real entregaba a quienes destacaban con esta arma.\nSus ataques son tan poderosos que aniquilan la moral del rival.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Royal_Claymore_Icon")));
        weapons.Add(new ItemData("Espada Maestra", "weapon", "weapon", "Espada legendaria que sella la oscuridad.\nSolo un verdadero héroe puede blandirla.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Master_Sword_Icon")));
        weapons.Add(new ItemData("Mandoble de fuego", "weapon", "weapon", "Espada mágica de dos manos con el poder del fuego.\nSu hoja ardiente puede quemar enemigos y prender la hierba.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Flameblade_Icon")));
        weapons.Add(new ItemData("Espada orni", "weapon", "weapon", "Espada ligera usada por los orni.\nEstá pensada para ataques rápidos y para llevarla cómodamente durante largos viajes.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Feathered_Edge_Icon")));
        weapons.Add(new ItemData("Garrote boko dragón", "weapon", "weapon", "Garrote brutal reforzado con huesos.\nGolpea con mucha fuerza, aunque deja al portador expuesto.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Dragonbone_Boko_Club_Icon")));
        weapons.Add(new ItemData("Espada ancestral", "weapon", "weapon", "Arma forjada con tecnología ancestral que ya no existe en la actualidad.\nEl brillo azulado solo aparece al desenvainarla.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Ancient_Short_Sword_Icon")));
    }

    void CrearMisiones()
    {
        quests.Add(new QuestData("La fuente del poder", "Posta de Akkala este", "Nobo habla sobre la fuente del Poder y la voz de la Diosa.\n\nOfrece una escama de Eldra al espíritu rojo de la fuente para descubrir el santuario oculto."));
        quests.Add(new QuestData("El ojo de la calavera", "Laboratorio de Akkala", "En el lago con forma de calavera se encuentra un ojo solitario.\n\nAlcanza la isla, examina la extraña formación y revela el santuario escondido."));
        quests.Add(new QuestData("La prueba del laberinto", "Isla Lomei", "Un gran laberinto se alza en el mar al norte de Akkala.\n\nRecorre sus muros, evita a los guardianes y encuentra la recompensa que aguarda en el centro."));
        quests.Add(new QuestData("La prueba de resistencia", "Peñasco Vigía", "Los goron han preparado una prueba de escalada sobre una gran roca.\n\nRecoge suficientes rupias antes de que se acabe el tiempo y demuestra tu fuerza."));
        quests.Add(new QuestData("El paisaje de la posta", "Posta de la montaña", "Una pintura de la posta muestra la vista de un santuario oculto.\n\nCompara el paisaje con el cuadro y sigue la pista hasta que el santuario aparezca."));
        quests.Add(new QuestData("La bebida perfecta", "Ciudadela Gerudo", "Pokki está agotada en el desierto y no deja de pedir una bebida especial.\n\nAyuda a las gerudo a prepararla para que el camino al santuario quede libre."));
    }
}
