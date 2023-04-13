
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace DimensionForge {
    public static class Serializer {
        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static async Task WriteToJsonFileAsync<T>(string filePath, T objectToWrite, bool append = false) where T : new() {
            // When appending, Check if file exists
            if (append && File.Exists(filePath) == false) {
                MessageBox.Show("Append error", "Can not append to file, file does not exist.");
                return;
            }

            string contentsToWriteToFile = "";
            // Try serialize the object to json
            try {
                var set = new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite,
                    Newtonsoft.Json.Formatting.Indented, set);
            }
            catch (Exception ex) {
                MessageBox.Show("Error while writing file", ex.Message);
            }

            if (string.IsNullOrEmpty(contentsToWriteToFile))
                return;


            // Write json text to file and dispose of the writer
            using (TextWriter writer = new StreamWriter(filePath, append)) {
                await writer.WriteAsync(contentsToWriteToFile);
            }
        }

        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new() {
            // When appending, Check if file exists
            if (append && File.Exists(filePath) == false) {
                MessageBox.Show("Append error", "Can not append to file, file does not exist.");
                return;
            }

            string contentsToWriteToFile = "";
            // Try serialize the object to json
            try {
                var set = new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite,
                    Newtonsoft.Json.Formatting.Indented, set);
            }
            catch (Exception ex) {
                MessageBox.Show("Error while writing file", ex.Message);
            }

            if (string.IsNullOrEmpty(contentsToWriteToFile))
                return;


            // Write json text to file and dispose of the writer
            using (TextWriter writer = new StreamWriter(filePath, append)) {
                writer.Write(contentsToWriteToFile);
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new() {
            if (File.Exists(filePath) == false) {
               // MessageDialog.Show($"File does not exist", filePath);
                return default(T);
            }
            string fileContents = "";
            try {
                fileContents = File.ReadAllText(filePath);

            }
            catch (Exception ex) {

                //MessageDialog.Show($"File does not exist", ex.StackTrace+ex.Message+ex.InnerException);

            }
            try {
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            catch (Exception ex) {

               // MessageDialog.Show($"Could not parse", ex.StackTrace + ex.Message + ex.InnerException);
            }
            return default(T);


        }

        public static void PopulateFromJsonFile(object jsonobject, string filePath) {
            if (System.IO.File.Exists(filePath) == false)
                return;

            TextReader reader = null;
            try {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                reader.Close();
                JsonConvert.PopulateObject(fileContents, jsonobject);
            }
            catch (Exception ex) {
                MessageBox.Show("Error while populating from file", ex.Message);
            }
            finally {
                if (reader != null)
                    reader.Close();

            }
        }


    }
}
