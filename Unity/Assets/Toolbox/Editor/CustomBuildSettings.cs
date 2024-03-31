using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class CustomBuildSettings : EditorWindow
{
    [MenuItem("Edit/WebGL Memory Settings")]
    private static void OpenWindow()
    {
        EditorWindow window = GetWindow<CustomBuildSettings>();
        window.position = new Rect(50f, 50f, 200f, 24f);
        window.Show();
    }



    [SerializeField]
    private float RamInGb = 2f;
    private void OnEnable()
    {
        titleContent = new GUIContent("WebGL Memory");
    }

    private void AddMenuItemForFloat(GenericMenu menu, string menuPath, float value)
    {
        menu.AddItem(new GUIContent(menuPath), RamInGb.Equals(value), OnFloatSelected, value);
    }

    private void OnFloatSelected(object value)
    {
        RamInGb = (float)value;
        // PlayerSettings.WebGL.memorySize = (int)(RamInGb * 1024);
        PlayerSettings.WebGL.emscriptenArgs = $"-s WASM_MEM_MAX={RamInGb}GB";
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Select Heap Size"))
        {
            GenericMenu menu = new GenericMenu();

            AddMenuItemForFloat(menu, "1GB", 1f);
            AddMenuItemForFloat(menu, "2GB - Default", 2f);
            AddMenuItemForFloat(menu, "3GB", 3f);
            AddMenuItemForFloat(menu, "4GB", 4f);
            AddMenuItemForFloat(menu, "5GB", 5f);
            AddMenuItemForFloat(menu, "6GB", 6f);
            AddMenuItemForFloat(menu, "7GB", 7f);
            AddMenuItemForFloat(menu, "8GB", 8f);
            AddMenuItemForFloat(menu, "9GB", 9f);
            AddMenuItemForFloat(menu, "10GB", 10f);
            AddMenuItemForFloat(menu, "11GB", 11f);
            AddMenuItemForFloat(menu, "12GB", 12f);
            AddMenuItemForFloat(menu, "13GB", 13f);
            AddMenuItemForFloat(menu, "14GB", 14f);
            AddMenuItemForFloat(menu, "15GB", 15f);
            AddMenuItemForFloat(menu, "16GB", 16f);
            AddMenuItemForFloat(menu, "17GB", 17f);
            AddMenuItemForFloat(menu, "18GB", 18f);
            AddMenuItemForFloat(menu, "19GB", 19f);
            AddMenuItemForFloat(menu, "20GB", 20f);
            AddMenuItemForFloat(menu, "21GB", 21f);
            AddMenuItemForFloat(menu, "22GB", 22f);
            AddMenuItemForFloat(menu, "23GB", 23f);
            AddMenuItemForFloat(menu, "24GB", 24f);
            AddMenuItemForFloat(menu, "25GB", 25f);
            AddMenuItemForFloat(menu, "26GB", 26f);
            AddMenuItemForFloat(menu, "27GB", 27f);
            AddMenuItemForFloat(menu, "28GB", 28f);
            AddMenuItemForFloat(menu, "29GB", 29f);
            AddMenuItemForFloat(menu, "30GB", 30f);
            AddMenuItemForFloat(menu, "31GB", 31f);
            AddMenuItemForFloat(menu, "32GB", 32f);

            menu.ShowAsContext();


        }
    }
}


public class GenericMenuExample : EditorWindow
{
    // open the window from the menu item Example -> GUI Color
    [MenuItem("Example/GUI Color")]
    static void Init()
    {
        EditorWindow window = GetWindow<GenericMenuExample>();
        window.position = new Rect(50f, 50f, 200f, 24f);
        window.Show();
    }

    // serialize field on window so its value will be saved when Unity recompiles
    [SerializeField]
    Color m_Color = Color.white;

    void OnEnable()
    {
        titleContent = new GUIContent("GUI Color");
    }

    // a method to simplify adding menu items
    void AddMenuItemForColor(GenericMenu menu, string menuPath, Color color)
    {
        // the menu item is marked as selected if it matches the current value of m_Color
        menu.AddItem(new GUIContent(menuPath), m_Color.Equals(color), OnColorSelected, color);
    }

    // the GenericMenu.MenuFunction2 event handler for when a menu item is selected
    void OnColorSelected(object color)
    {
        m_Color = (Color)color;
    }

    void OnGUI()
    {
        // set the GUI to use the color stored in m_Color
        GUI.color = m_Color;

        // display the GenericMenu when pressing a button
        if (GUILayout.Button("Select GUI Color"))
        {
            // create the menu and add items to it
            GenericMenu menu = new GenericMenu();

            // forward slashes nest menu items under submenus
            AddMenuItemForColor(menu, "RGB/Red", Color.red);
            AddMenuItemForColor(menu, "RGB/Green", Color.green);
            AddMenuItemForColor(menu, "RGB/Blue", Color.blue);

            // an empty string will create a separator at the top level
            menu.AddSeparator("");

            AddMenuItemForColor(menu, "CMYK/Cyan", Color.cyan);
            AddMenuItemForColor(menu, "CMYK/Yellow", Color.yellow);
            AddMenuItemForColor(menu, "CMYK/Magenta", Color.magenta);
            // a trailing slash will nest a separator in a submenu
            menu.AddSeparator("CMYK/");
            AddMenuItemForColor(menu, "CMYK/Black", Color.black);

            menu.AddSeparator("");

            AddMenuItemForColor(menu, "White", Color.white);

            // display the menu
            menu.ShowAsContext();
        }
    }
}
