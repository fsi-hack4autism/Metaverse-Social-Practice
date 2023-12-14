using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

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
    private float HeapInMB = 200f;
    [SerializeField] private float MaxHeapInMB = 2000f;
    private void OnEnable()
    {
        titleContent = new GUIContent("WebGL Memory");
    }

    private void AddMenuItemForFloat(GenericMenu menu, string menuPath, float value, GenericMenu.MenuFunction2 callback)
    {
        menu.AddItem(new GUIContent(menuPath), HeapInMB.Equals(value), callback, value);
    }

    private void OnDefaultRamSelected(object value)
    {
        HeapInMB = (float)value;
        PlayerSettings.WebGL.memorySize = (int)HeapInMB;
    }

    private void OnMaxHeapSelected(object value)
    {
        MaxHeapInMB = (float)value;
        PlayerSettings.WebGL.emscriptenArgs = $"-s WASM_MEM_MAX={MaxHeapInMB}MB";
    }

    private void OnGUI()
    {
        // current heap size
        GUILayout.Label($"Current Heap Size: {PlayerSettings.WebGL.memorySize}MB");
        // change heap size
        if (GUILayout.Button("Select Heap Size"))
        {
            GenericMenu menu = new GenericMenu();
            // 100 MB - 2 GB
            for (int i = 1; i <= 20; i++)
            {
                AddMenuItemForFloat(menu, $"{i * 100}MB", i * 100, OnDefaultRamSelected);
            }

            menu.ShowAsContext();
        }

        // current max heap size
        GUILayout.Label($"Current Max Heap Size: {Regex.Match(PlayerSettings.WebGL.emscriptenArgs, @"(?<=WASM_MEM_MAX=)\d+").Value}MB");
        // change max heap size
        if (GUILayout.Button("Select Max Heap Size"))
        {
            GenericMenu menu = new GenericMenu();
            // 1 GB - 10 GB
            for (int i = 1; i <= 10; i++)
            {
                AddMenuItemForFloat(menu, $"{i}GB", i * 1024, OnMaxHeapSelected);
            }

            menu.ShowAsContext();
        }

        // reset
        if (GUILayout.Button("Reset"))
        {
            PlayerSettings.WebGL.emscriptenArgs = "";
            PlayerSettings.WebGL.memorySize = 0;
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
