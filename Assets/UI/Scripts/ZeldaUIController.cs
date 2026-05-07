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
        public int cantidad;
        public string estrellas;
        public string descripcion;
        public Texture2D icono;

        public ItemData(string nombre, string categoria, string hueco, int cantidad, string estrellas, string descripcion, Texture2D icono)
        {
            this.nombre = nombre;
            this.categoria = categoria;
            this.hueco = hueco;
            this.cantidad = cantidad;
            this.estrellas = estrellas;
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
    VisualElement popupIcon;
    Button popupAction;
    Button popupCancel;
    Label popupTitle;
    Label itemStars;
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
        popupIcon = root.Q<VisualElement>("PopupIcon");
        popupAction = root.Q<Button>("PopupAction");
        popupCancel = root.Q<Button>("PopupCancel");
        popupTitle = root.Q<Label>("PopupTitle");

        itemStars = root.Q<Label>("ItemStars");
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
        root.Q<Button>("LifeButton").RegisterCallback<ClickEvent>(evt => QuitarVida());
        root.Q<Button>("RupeeButton").RegisterCallback<ClickEvent>(evt => SumarRupias());
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
            title.text = "Adventure Log";
            leftText.text = "";
            rightText.text = "Inventory";
            tabLeft.style.visibility = Visibility.Hidden;
        }
        else if (pagina == 1)
        {
            inventory.style.display = DisplayStyle.Flex;
            title.text = "Inventory";
            leftText.text = "Adventure Log";
            rightText.text = "System";
        }
        else if (pagina == 2)
        {
            settings.style.display = DisplayStyle.Flex;
            title.text = "System";
            leftText.text = "Inventory";
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
            slot.Q<Label>("Amount").text = lista[i].cantidad.ToString();
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
            Label equipado = slot.Q<Label>("EquippedMark");

            slot.RemoveFromClassList("item-slot-selected");
            slot.RemoveFromClassList("item-slot-equipped");
            equipado.RemoveFromClassList("equipped-mark-on");

            if (item == itemSeleccionado)
                slot.AddToClassList("item-slot-selected");

            if (ItemEquipado(item))
            {
                slot.AddToClassList("item-slot-equipped");
                equipado.AddToClassList("equipped-mark-on");
            }
        }
    }

    void AbrirPopup()
    {
        popupLayer.style.display = DisplayStyle.Flex;
        popupTitle.text = itemSeleccionado.nombre;
        popupIcon.style.backgroundImage = itemSeleccionado.icono;

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
        else slot.style.opacity = 0.18f;
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
        itemStars.text = itemSeleccionado.estrellas;
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

    void SumarRupias()
    {
        link.rupias = link.rupias + 10;
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
        armors.Add(new ItemData("Zora Helm", "armor", "head", 6, "★★", "Zora headgear made from dragon scales.\nIncreases swimming speed and allows spin attacks under water.", Resources.Load<Texture2D>("Images/items/armors/BotW_Zora_Helm_Icon")));
        armors.Add(new ItemData("Zora Armor", "armor", "body", 6, "★★", "Armor crafted by the Zora for a chosen hero.\nIt helps Link swim faster and climb waterfalls.", Resources.Load<Texture2D>("Images/items/armors/BotW_Zora_Armor_Icon")));
        armors.Add(new ItemData("Zora Greaves", "armor", "legs", 4, "★", "Traditional Zora legwear that improves movement in water.\nThe polished scales keep it light and resistant.", Resources.Load<Texture2D>("Images/items/armors/BotW_Zora_Greaves_Icon")));
        armors.Add(new ItemData("Champion's Tunic", "armor", "body", 8, "★★★", "A tunic given to the chosen Champion of Hyrule.\nIts blue fabric makes enemy intent easier to read.", Resources.Load<Texture2D>("Images/items/armors/BotW_Champion's_Tunic_Icon")));
        armors.Add(new ItemData("Cap of the Hero", "armor", "head", 8, "★★", "A green cap said to belong to an ancient hero.\nIt is simple, light and full of old stories.", Resources.Load<Texture2D>("Images/items/armors/BotW_Cap_of_the_Hero_Icon")));
        armors.Add(new ItemData("Hylian Trousers", "armor", "legs", 3, "★", "Common Hylian trousers made for long journeys.\nThey are comfortable enough for climbing and running.", Resources.Load<Texture2D>("Images/items/armors/BotW_Hylian_Trousers_Icon")));
        armors.Add(new ItemData("Flamebreaker Helm", "armor", "head", 5, "★★", "A sturdy helmet made for volcanic heat.\nIt protects the head while crossing Death Mountain.", Resources.Load<Texture2D>("Images/items/armors/BotW_Flamebreaker_Helm_Icon")));
        armors.Add(new ItemData("Flamebreaker Armor", "armor", "body", 5, "★★", "Protective armor from Goron City.\nIts plates keep dangerous heat away from the body.", Resources.Load<Texture2D>("Images/items/armors/BotW_Flamebreaker_Armor_Icon")));
        armors.Add(new ItemData("Flamebreaker Boots", "armor", "legs", 8, "★★", "Heavy boots built for rocky ground and extreme heat.\nThey make travel safer near lava.", Resources.Load<Texture2D>("Images/items/armors/BotW_Flamebreaker_Boots_Icon")));
        armors.Add(new ItemData("Climber's Bandanna", "armor", "head", 1, "★", "A bandanna loved by mountain climbers.\nIts grip-focused weave helps with fast climbing.", Resources.Load<Texture2D>("Images/items/armors/BotW_Climber's_Bandanna_Icon")));
        armors.Add(new ItemData("Climbing Gear", "armor", "body", 12, "★", "Flexible gear reinforced around the shoulders.\nIt keeps Link steady on cliffs.", Resources.Load<Texture2D>("Images/items/armors/BotW_Climbing_Gear_Icon")));
        armors.Add(new ItemData("Climbing Boots", "armor", "legs", 12, "★", "Boots with strong traction for rough walls.\nThey reduce the strain of vertical travel.", Resources.Load<Texture2D>("Images/items/armors/BotW_Climbing_Boots_Icon")));

        shields.Add(new ItemData("Wooden Shield", "shield", "shield", 1, "★", "A light shield made from simple wood.\nIt blocks small hits but burns easily.", Resources.Load<Texture2D>("Images/items/shields/BotW_Wooden_Shield_Icon")));
        shields.Add(new ItemData("Traveler's Shield", "shield", "shield", 1, "★", "A common shield used by travelers across Hyrule.\nIt is reliable against weaker enemies.", Resources.Load<Texture2D>("Images/items/shields/BotW_Traveler's_Shield_Icon")));
        shields.Add(new ItemData("Gerudo Shield", "shield", "shield", 1, "★★", "A decorated shield from the Gerudo region.\nIts metal frame handles strong blows well.", Resources.Load<Texture2D>("Images/items/shields/BotW_Gerudo_Shield_Icon")));
        shields.Add(new ItemData("Ancient Shield", "shield", "shield", 1, "★★★", "A shield made with ancient technology.\nIts surface can deflect focused energy.", Resources.Load<Texture2D>("Images/items/shields/BotW_Ancient_Shield_Icon")));

        weapons.Add(new ItemData("Tree Branch", "weapon", "weapon", 1, "★", "A branch picked up from the ground.\nIt is weak, but useful in a hurry.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Tree_Branch_Icon")));
        weapons.Add(new ItemData("Soldier's Broadsword", "weapon", "weapon", 1, "★★", "A sword issued to Hyrulean soldiers.\nBalanced and easy to use in close combat.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Soldier's_Broadsword_Icon")));
        weapons.Add(new ItemData("Royal Claymore", "weapon", "weapon", 1, "★★★", "A heavy royal sword with excellent reach.\nIts weight rewards committed attacks.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Royal_Claymore_Icon")));
        weapons.Add(new ItemData("Master Sword", "weapon", "weapon", 1, "★★★", "The legendary blade that seals the darkness.\nIt shines brightest when evil is near.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Master_Sword_Icon")));
        weapons.Add(new ItemData("Flameblade", "weapon", "weapon", 1, "★★", "A magical sword with a fire spirit inside.\nIts blade can burn enemies and grass.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Flameblade_Icon")));
        weapons.Add(new ItemData("Feathered Edge", "weapon", "weapon", 1, "★★", "A light Rito sword designed for quick strikes.\nIt is easy to carry during long travel.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Feathered_Edge_Icon")));
        weapons.Add(new ItemData("Dragonbone Boko Club", "weapon", "weapon", 1, "★★", "A brutal club reinforced with bones.\nIt hits hard but leaves Link exposed.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Dragonbone_Boko_Club_Icon")));
        weapons.Add(new ItemData("Ancient Short Sword", "weapon", "weapon", 1, "★★★", "A compact sword forged with ancient parts.\nIts energy edge cuts with precision.", Resources.Load<Texture2D>("Images/items/weapons/BotW_Ancient_Short_Sword_Icon")));
    }

    void CrearMisiones()
    {
        quests.Add(new QuestData("The Spring of Power", "East Akkala Stable", "Nobo mentioned the Spring of Power and the voice of the Goddess.\n\nOffer Dinraal's scale to the red spirit at the spring and discover what sleeps inside the shrine."));
        quests.Add(new QuestData("The Skull's Eye", "Akkala Ancient Tech Lab", "A lone eye rests in the skull-shaped lake of Akkala.\n\nReach the island, search the strange formation and reveal the shrine hidden in the landscape."));
        quests.Add(new QuestData("Trial of the Labyrinth", "Lomei Labyrinth Island", "A maze stands in the sea north of Akkala.\n\nNavigate its walls, avoid the guardians and find the reward at the center of the ruins."));
        quests.Add(new QuestData("The Gut Check Challenge", "Gut Check Rock", "The Gorons have prepared a climbing trial over a tall rock.\n\nCollect enough rupees before time runs out and prove that Link has the strength to continue."));
        quests.Add(new QuestData("A Landscape of a Stable", "Foothill Stable", "A painting at the stable shows a view of a hidden shrine.\n\nCompare the landscape with the picture and follow the clue until the shrine appears."));
        quests.Add(new QuestData("The Perfect Drink", "Gerudo Town", "Pokki is exhausted in the desert and keeps asking for a special drink.\n\nHelp the Gerudo prepare it so the way to the shrine can finally open."));
    }
}
