using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace JsonModel
{
    public class JsonToModelGenerator : EditorWindow
    {
        private TextAsset jsonFile;
        private string outputPath = "Assets/Scripts/GeneratedModels";

        [MenuItem("Tools/JSON to Model Generator")]
        public static void ShowWindow()
        {
            GetWindow<JsonToModelGenerator>("JSON to Model Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("JSON to Model Generator", EditorStyles.boldLabel);

            jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
            outputPath = EditorGUILayout.TextField("Output Path", outputPath);

            if (GUILayout.Button("Generate Models"))
            {
                if (jsonFile == null)
                {
                    EditorUtility.DisplayDialog("Error", "Please select a JSON file.", "OK");
                    return;
                }

                GenerateModels(jsonFile.text);
            }
        }

        private void GenerateModels(string json)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            try
            {
                JObject rootObject = JObject.Parse(json);
                foreach (var property in rootObject.Properties())
                {
                    string className = ConvertToPascalCase(property.Name) + "Model";
                    Debug.Log($"Processing class: {className} from property: {property.Name}");

                    try
                    {
                        JObject classData = (JObject)property.Value;
                        string classCode = GenerateClassCode(className, classData);
                        string filePath = Path.Combine(outputPath, $"{className}.cs");
                        File.WriteAllText(filePath, classCode, Encoding.UTF8);
                        Debug.Log($"Successfully generated class: {className} at {filePath}");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Error generating class for property '{property.Name}': {ex.Message}");
                    }
                }

                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success", "Models generated successfully!", "OK");
            }
            catch (JsonReaderException ex)
            {
                Debug.LogError($"JSON Parsing Error: {ex.Message} at Line {ex.LineNumber}, Position {ex.LinePosition}");
                EditorUtility.DisplayDialog("Error", $"Failed to parse JSON file. Error at Line {ex.LineNumber}, Position {ex.LinePosition}.", "OK");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Unexpected Error: {e.Message}");
                EditorUtility.DisplayDialog("Error", "Failed to generate models. Check the console for details.", "OK");
            }
        }

        private string GenerateClassCode(string className, JObject classData)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine("namespace JsonModel");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");

            foreach (var property in classData.Properties())
            {
                try
                {
                    string propertyName = ConvertToPascalCase(property.Name);
                    string propertyType = GetPropertyType(property.Value);
                    Debug.Log($"Generating property: {propertyName} of type {propertyType} in class {className}");

                    sb.AppendLine($"        [JsonProperty(\"{property.Name}\")]");
                    sb.AppendLine($"        public {propertyType} {propertyName} {{ get; set; }}");
                    sb.AppendLine();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error generating property '{property.Name}' in class '{className}': {ex.Message}");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GetPropertyType(JToken token)
        {
            try
            {
                switch (token.Type)
                {
                    case JTokenType.Integer:
                        return "int";
                    case JTokenType.Float:
                        return "float";
                    case JTokenType.String:
                        return "string";
                    case JTokenType.Boolean:
                        return "bool";
                    case JTokenType.Object:
                        return ConvertToPascalCase(token.Path) + "Model";
                    case JTokenType.Array:
                        var arrayType = GetPropertyType(token.First);
                        return $"{arrayType}[]";
                    default:
                        return "object";
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error determining property type for token: {token.Path}. Error: {ex.Message}");
                return "object";
            }
        }

        private string ConvertToPascalCase(string name)
        {
            string[] parts = name.Split('_');
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
            }
            return string.Join(string.Empty, parts);
        }
    }
}
