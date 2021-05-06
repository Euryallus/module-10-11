using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Text.RegularExpressions;

[CustomEditor(typeof(ItemManager))]
public class ItemManagerEditor : Editor
{
    private int         columnCount;
    private const int   MaxColumns = 4;
    private Vector2     itemScrollPosition;
    private Vector2     recipeScrollPosition;
    private bool        itemArrayMatchesPrefab      = true;
    private bool        recipeArrayMatchesPrefab    = true;
    private List<int>   itemModificationIndexes;
    private List<int>   recipeModificationIndexes;

    //GUI Styles
    private GUIStyle smallLabelStyle, mediumLabelStyle, warningLabelStyle, largeLabelStyle;
    private GUIStyle boxStyle = new GUIStyle(), warningBoxStyle = new GUIStyle();

    private static bool itemsExpanded   = true;
    private static bool recipesExpanded = true;
    private static bool baseInspectorExpanded;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        ItemManager itemManager = (ItemManager)target;

        CheckForPrefabPropertyModifications(itemManager);

        SetupGUIStyles();

        //ITEM MANAGER HEADER
        //===================

        EditorGUILayout.Space(15.0f);
        EditorGUILayout.LabelField("Item Manager", largeLabelStyle);
        EditorGUILayout.LabelField("Please open the prefab to make changes.", mediumLabelStyle);

        //ITEMS HEADER
        //============
        EditorGUILayout.Space(15.0f);

        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Items", largeLabelStyle, GUILayout.Width(80.0f), GUILayout.Height(22.0f));
        if (GUILayout.Button(itemsExpanded ? "Hide" : "Show", GUILayout.Width(80.0f), GUILayout.Height(25.0f)))
        {
            itemsExpanded = !itemsExpanded;
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField(itemArrayMatchesPrefab ? "" : "Warning: The items array for this prefab instance contains changes not applied to the base prefab. Consider reverting changes.", warningLabelStyle);

        EditorGUILayout.Space(itemArrayMatchesPrefab ? 0.0f : 10.0f);

        //ITEMS DISPLAY/SELECTION
        //=======================

        columnCount = 0;
        Item[] items = itemManager.Items;

        if (itemsExpanded && items != null)
        {
            itemScrollPosition = EditorGUILayout.BeginScrollView(itemScrollPosition, GUILayout.ExpandHeight(false));
            GUILayout.BeginHorizontal();

            for (int i = 0; i < (items.Length + 1); i++)
            {
                if (i < items.Length)
                {
                    AddItemPreview(itemManager, items, i);
                }
                else
                {
                    ShowAddItemButton(itemManager);
                }

                GUILayout.Space(8.0f);

                columnCount++;

                if (columnCount == MaxColumns)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8.0f);
                    GUILayout.BeginHorizontal();
                    columnCount = 0;
                }

            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(15.0f);
        }
        else if(items == null)
        {
            ShowAddItemButton(itemManager);
        }

        //RECIPES HEADER
        //============
        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Recipes", largeLabelStyle, GUILayout.Width(80.0f), GUILayout.Height(22.0f));
        if (GUILayout.Button(recipesExpanded ? "Hide" : "Show", GUILayout.Width(80.0f), GUILayout.Height(25.0f)))
        {
            recipesExpanded = !recipesExpanded;
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField(recipeArrayMatchesPrefab ? "" : "Warning: The recipes array for this prefab instance contains changes not applied to the base prefab. Consider reverting changes.", warningLabelStyle);

        EditorGUILayout.Space(recipeArrayMatchesPrefab ? 0.0f : 10.0f);

        //RECIPES DISPLAY/SELECTION
        //=========================

        columnCount = 0;
        CraftingRecipe[] recipes = itemManager.CraftingRecipes;

        if (recipesExpanded && recipes != null)
        {
            recipeScrollPosition = EditorGUILayout.BeginScrollView(recipeScrollPosition, GUILayout.ExpandHeight(false));
            GUILayout.BeginHorizontal();

            for (int i = 0; i < (recipes.Length + 1); i++)
            {
                if (i < recipes.Length)
                {
                    AddRecipePreview(itemManager, recipes, i);
                }
                else
                {
                    ShowAddRecipeButton(itemManager);
                }

                GUILayout.Space(8.0f);

                columnCount++;

                if (columnCount == MaxColumns)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8.0f);
                    GUILayout.BeginHorizontal();
                    columnCount = 0;
                }

            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(15.0f);
        }
        else if(recipes == null)
        {
            ShowAddRecipeButton(itemManager);
        }

        //BASE INSPECTOR
        //==============

        if (GUILayout.Button(baseInspectorExpanded ? "Hide Standard Arrays" : "Show Standard Arrays", GUILayout.Width(163.0f), GUILayout.Height(25.0f)))
        {
            baseInspectorExpanded = !baseInspectorExpanded;
        }

        if (baseInspectorExpanded)
        {
            //Draw the default editor GUI
            base.OnInspectorGUI();
        }

        EditorGUI.EndChangeCheck();

        if(GUI.changed)
        {
            //Apply any properties that have been changed
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(itemManager.gameObject.scene);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowAddItemButton(ItemManager itemManager)
    {
        if (GUILayout.Button("Add", GUILayout.Width(82.0f), GUILayout.Height(72.0f)))
        {
            AddItem(itemManager);
        }
    }

    private void ShowAddRecipeButton(ItemManager itemManager)
    {
        if (GUILayout.Button("Add", GUILayout.Width(82.0f), GUILayout.Height(72.0f)))
        {
            AddRecipe(itemManager);
        }
    }

    private void SetupGUIStyles()
    {
        smallLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 10,
            alignment = TextAnchor.LowerLeft,
            fixedHeight = 9.0f
        };
        smallLabelStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f);

        mediumLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 15,
            alignment = TextAnchor.LowerLeft
        };

        warningLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            alignment = TextAnchor.LowerLeft,
            wordWrap = true
        };
        warningLabelStyle.normal.textColor = new Color(0.8f, 0.98f, 1.0f);//new Color(1.0f, 0.04f, 0.24f);

        largeLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 18,
            alignment = TextAnchor.LowerLeft
        };

        boxStyle.normal.background = GetColourTexture(new Color(1.0f, 1.0f, 1.0f, 0.9f));

        warningBoxStyle.normal.background = GetColourTexture(new Color(0.8f, 0.98f, 1.0f, 0.9f));
    }

    private void AddItemPreview(ItemManager itemManager, Item[] items, int index)
    {
        Item currentItem = items[index];

        GUILayout.BeginVertical(itemModificationIndexes.Contains(index) ? warningBoxStyle : boxStyle, GUILayout.Width(80.0f));  //AREA 1: Start of the main item display area

        GUILayout.Space(5.0f);

        GUILayout.Label(currentItem ? currentItem.UIName : "None", smallLabelStyle, GUILayout.Width(78.0f));

        GUILayout.BeginHorizontal(); //AREA 2: Area containing the item icon and change/delete buttons

        Texture2D textureToShow;

        if (currentItem != null && currentItem.Sprite != null)
        {
            textureToShow = currentItem.Sprite.texture;
        }
        else
        {
            textureToShow = Resources.Load<Sprite>("WarningIcon").texture;
        }

        GUILayout.Label(textureToShow, GUILayout.Width(55.0f), GUILayout.Height(55.0f));

        GUILayout.BeginVertical(); //AREA 3: Area containing just the change and delete buttons

        items[index] = EditorGUILayout.ObjectField("", items[index], typeof(Item), false, GUILayout.Height(35.0f), GUILayout.Width(22.0f)) as Item;

        if (GUILayout.Button("X", GUILayout.Width(22.0f), GUILayout.Height(20.0f)))
        {
            RemoveItemAtIndex(itemManager, index);
        }

        GUILayout.EndVertical();   //End of AREA 1
        GUILayout.EndHorizontal(); //End of AREA 2
        GUILayout.EndVertical();   //End of AREA 3
    }

    private void AddRecipePreview(ItemManager itemManager, CraftingRecipe[] recipes, int index)
    {
        CraftingRecipe currentRecipe = recipes[index];

        GUILayout.BeginVertical(recipeModificationIndexes.Contains(index) ? warningBoxStyle : boxStyle, GUILayout.Width(80.0f));  //AREA 1: Start of the main recipe display area

        GUILayout.Space(5.0f);

        GUILayout.Label((currentRecipe && currentRecipe.ResultItem.Item) ? currentRecipe.ResultItem.Item.UIName : "None", smallLabelStyle, GUILayout.Width(78.0f));

        GUILayout.BeginHorizontal(); //AREA 2: Area containing the recipe result icon and change/delete buttons

        Texture2D textureToShow;

        if (currentRecipe != null && currentRecipe.ResultItem.Item != null && currentRecipe.ResultItem.Item.Sprite != null)
        {
            textureToShow = currentRecipe.ResultItem.Item.Sprite.texture;
        }
        else
        {
            textureToShow = Resources.Load<Sprite>("WarningIcon").texture;
        }

        GUILayout.Label(textureToShow, GUILayout.Width(55.0f), GUILayout.Height(55.0f));

        GUILayout.BeginVertical(); //AREA 3: Area containing just the change and delete buttons

        recipes[index] = EditorGUILayout.ObjectField("", currentRecipe, typeof(CraftingRecipe), false, GUILayout.Height(35.0f), GUILayout.Width(22.0f)) as CraftingRecipe;

        if (GUILayout.Button("X", GUILayout.Width(22.0f), GUILayout.Height(20.0f)))
        {
            RemoveRecipeAtIndex(itemManager, index);
        }

        GUILayout.EndVertical();   //End of AREA 1
        GUILayout.EndHorizontal(); //End of AREA 2
        GUILayout.EndVertical();   //End of AREA 3
    }

    private void AddItem(ItemManager itemManager)
    {
        List<Item> itemList = itemManager.Items.ToList();

        itemList.Add(null);

        itemManager.Items = itemList.ToArray();
    }

    private void AddRecipe(ItemManager itemManager)
    {
        List<CraftingRecipe> recipeList = itemManager.CraftingRecipes.ToList();

        recipeList.Add(null);

        itemManager.CraftingRecipes = recipeList.ToArray();
    }

    private void RemoveItemAtIndex(ItemManager itemManager, int index)
    {
        List<Item> itemList = itemManager.Items.ToList();

        itemList.RemoveAt(index);

        itemManager.Items = itemList.ToArray();
    }

    private void RemoveRecipeAtIndex(ItemManager itemManager, int index)
    {
        List<CraftingRecipe> recipeList = itemManager.CraftingRecipes.ToList();

        recipeList.RemoveAt(index);

        itemManager.CraftingRecipes = recipeList.ToArray();
    }

    private void CheckForPrefabPropertyModifications(ItemManager itemManager)
    {
        itemModificationIndexes = new List<int>();
        recipeModificationIndexes = new List<int>();

        var propertyModifications = PrefabUtility.GetPropertyModifications(itemManager.gameObject);
        if (propertyModifications != null)
        {
            foreach (var modification in propertyModifications)
            {
                string path = modification.propertyPath;

                if (path.Contains("items.Array.data"))
                {
                    //Regex expression taken from: https://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
                    itemModificationIndexes.Add(int.Parse(Regex.Match(path, @"\d+").Value));

                    itemArrayMatchesPrefab = false;
                }
                else if (path.Contains("craftingRecipes.Array.data"))
                {
                    //Regex expression taken from: https://stackoverflow.com/questions/4734116/find-and-extract-a-number-from-a-string
                    recipeModificationIndexes.Add(int.Parse(Regex.Match(path, @"\d+").Value));

                    recipeArrayMatchesPrefab = false;
                }
            }
        }
    }

    private Texture2D GetColourTexture(Color colour)
    {
        Texture2D texture = new Texture2D(128, 128);

        for (int y = 0; y < texture.height; ++y)
        {
            for (int x = 0; x < texture.width; ++x)
            {
                texture.SetPixel(x, y, colour);
            }
        }

        texture.Apply();

        return texture;
    }
}
