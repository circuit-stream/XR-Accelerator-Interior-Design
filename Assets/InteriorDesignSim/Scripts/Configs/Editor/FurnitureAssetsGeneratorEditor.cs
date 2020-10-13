using System.IO;
using UnityEditor;
using UnityEngine;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Configs.Editor
{
    [CustomEditor(typeof(FurnitureAssetsGenerator))]
    public class FurnitureAssetsGeneratorEditor : UnityEditor.Editor
    {
        private FurnitureAssetsGenerator generatorConfig;

        public override void OnInspectorGUI()
        {
            generatorConfig = (FurnitureAssetsGenerator) target;

            base.OnInspectorGUI();

            GUILayout.Space(15);
            GuiLine();
            GUILayout.Space(15);

            EditorGUILayout.HelpBox("This script will generate the 2 furniture placement prefabs for each provided model and their correspondent FurnitureConfig", MessageType.Info);
            EditorGUILayout.HelpBox("The generated FurnitureConfigs are not linked to a Catalog Config.", MessageType.Warning);
            EditorGUILayout.HelpBox("The generated FurnitureConfigs furnitureType field is not set by this script.", MessageType.Warning);

            GUILayout.Space(5);

            if (GUILayout.Button("Generate Prefabs"))
            {
                CreatePrefabs();
            }

            GUILayout.Space(15);
            GuiLine();
            GUILayout.Space(15);

            if (GUILayout.Button("Add Mesh Collier to Models"))
            {
                AddMeshColliderToModels();
            }
        }

        private void CreatePrefabs()
        {
            Debug.Log(":: Starting Prefab Generation ::");

            EnsureDirectories();

            Debug.Log(":: Directories Setup Finished ::");

            foreach (var furnitureModel in generatorConfig.furnitureModels)
            {
                CreateFurnitureConfig(furnitureModel);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.LogWarning(":: Assets Created! CatalogConfig and FurnitureType setup necessary! ::");
        }

        private void CreateFurnitureConfig(GameObject furnitureModel)
        {
            Debug.Log($":: Creating Assets for {furnitureModel.name} ::");

            var furnitureConfig = CreateInstance<FurnitureConfig>();
            var configPath = Path.Combine(generatorConfig.generatedConfigsRelativePath, $"{furnitureModel.name}.asset");
            AssetDatabase.CreateAsset(furnitureConfig, configPath);

            furnitureConfig.Id = furnitureModel.name;

            var thumbnailPath =
                Path.Combine(generatorConfig.thumbnailsRelativePath, $"{furnitureModel.name}.png");
            furnitureConfig.Thumbnail = AssetDatabase.LoadAssetAtPath<Sprite>(thumbnailPath);

            var furnitureBasePrefabPath =
                Path.Combine(generatorConfig.generatedPrefabsRelativePath, $"{furnitureModel.name}FurnitureBase.prefab");
            CreatePrefabVariant(generatorConfig.furnitureBasePrefab, furnitureModel, furnitureBasePrefabPath);
            furnitureConfig.FurniturePrefab =
                AssetDatabase.LoadAssetAtPath<SafeARSelectionInteractable>(furnitureBasePrefabPath);

            var furniturePreviewBasePrefabPath =
                Path.Combine(generatorConfig.generatedPrefabsRelativePath, $"{furnitureModel.name}FurniturePreviewBase.prefab");
            CreatePrefabVariant(generatorConfig.furniturePreviewBasePrefab, furnitureModel, furniturePreviewBasePrefabPath);
            furnitureConfig.furniturePreviewPrefab =
                AssetDatabase.LoadAssetAtPath<SafeARSelectionInteractable>(furniturePreviewBasePrefabPath);

            EditorUtility.SetDirty(furnitureConfig);
        }

        private void CreatePrefabVariant(GameObject prefabParent, GameObject furnitureModel, string newPrefabPath)
        {
            GameObject instantiatedPrefabBase = PrefabUtility.InstantiatePrefab(prefabParent) as GameObject;
            GameObject instantiatedFurnitureModel = PrefabUtility.InstantiatePrefab(furnitureModel) as GameObject;

            var modelTransform = instantiatedFurnitureModel.transform;
            modelTransform.parent = instantiatedPrefabBase.transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;
            modelTransform.localScale = Vector3.one;

            PrefabUtility.SaveAsPrefabAsset(instantiatedPrefabBase, newPrefabPath);

            DestroyImmediate(instantiatedPrefabBase);
            DestroyImmediate(instantiatedFurnitureModel);
        }


        private void AddMeshColliderToModels()
        {
            AssetDatabase.StartAssetEditing();

            foreach (var furnitureModel in generatorConfig.furnitureModels)
            {
                AddMeshColliderToModel(furnitureModel);
            }

            AssetDatabase.StopAssetEditing();
        }

        private void AddMeshColliderToModel(GameObject furnitureModel)
        {
            var modelPath = AssetDatabase.GetAssetPath(furnitureModel);
            var prefab = PrefabUtility.LoadPrefabContents(modelPath);

            var renderers = prefab.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in renderers)
            {
                if (!meshRenderer.GetComponent<MeshCollider>())
                {
                    meshRenderer.gameObject.AddComponent<MeshCollider>();
                }
            }

            PrefabUtility.SaveAsPrefabAsset(prefab, modelPath);
            PrefabUtility.UnloadPrefabContents(prefab);
        }


        #region Directory operations

        private void EnsureDirectories()
        {
            EnsureDirectory(generatorConfig.generatedConfigsRelativePath);
            EnsureDirectory(generatorConfig.generatedPrefabsRelativePath);
        }

        private void EnsureDirectory(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                if (!generatorConfig.deleteExistingAssets)
                {
                    Debug.Log("Should NOT Delete Assets");
                    return;
                }

                ClearDirectory(path);
                CreateDirectory(path);
            }
            else
            {
                CreateDirectory(path);
            }
        }

        private void CreateDirectory(string path)
        {
            var parentFolder = Path.GetDirectoryName(path);
            var folderName = Path.GetFileName(path);

            AssetDatabase.CreateFolder(parentFolder, folderName);
        }

        private void ClearDirectory(string path)
        {
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            Directory.Delete(path);
        }

        #endregion

        #region GUI Utility

        void GuiLine( int i_height = 1 )
        {

            Rect rect = EditorGUILayout.GetControlRect(false, i_height );

            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );

        }

        #endregion
    }
}