using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaDB.SqlServer
{

    /// <summary>
    /// Includes static help methods for the Model Classes
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A entitiy to load foreign properties to</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships<T>(this T result, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, innerModelTypes);
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships<T>(this T result, SqlConnection connection, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, connection, innerModelTypes);
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static T LoadRelationships<T>(this T result, SqlTransaction trans, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, trans, innerModelTypes);
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static async Task<T> LoadRelationshipsAsync<T>(this T result, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, innerModelTypes));
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">An entity to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static async Task<T> LoadRelationshipsAsync<T>(this T result, SqlConnection connection, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, connection, innerModelTypes));
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Class types of the forign properties you want to load</param>
        /// <returns>Returns an IEnumerable of the class type with the resulting foreign properties</returns>
        public static async Task<T> LoadRelationshipsAsync<T>(this T result, SqlTransaction trans, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, trans, innerModelTypes));
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static List<T> LoadRelationships<T>(this List<T> result, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, innerModelTypes).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static List<T> LoadRelationships<T>(this List<T> result, SqlConnection connection, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, connection, innerModelTypes).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static List<T> LoadRelationships<T>(this List<T> result, SqlTransaction trans, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return ViewModel<T>.LoadRelationships(result, trans, innerModelTypes).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static async Task<List<T>> LoadRelationshipsAsync<T>(this List<T> result, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, innerModelTypes)).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="connection">An open SqlConnection to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static async Task<List<T>> LoadRelationshipsAsync<T>(this List<T> result, SqlConnection connection, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, connection, innerModelTypes)).ToList();
        }

        /// <summary>
        /// Load foreign properties decorated with the <see cref="ForeignRelationAttribute"/> to an entity or
        /// collection of entities.
        /// </summary>
        /// <param name="result">A collection of entities to load foreign properties to</param>
        /// <param name="trans">An active SqlTransaction to execute the Select statement against</param>
        /// <param name="innerModelTypes">Foreign Key entities types you want to have retrieved
        /// (through properties decored with the ForeignRelation Attribute)</param>
        /// <returns>Returns a List of the class type with the resulting foreign properties</returns>
        public static async Task<List<T>> LoadRelationshipsAsync<T>(this List<T> result, SqlTransaction trans, params Type[] innerModelTypes) where T : ViewModel<T>, new()
        {
            return (await ViewModel<T>.LoadRelationshipsAsync(result, trans, innerModelTypes)).ToList();
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert<T>(this IEnumerable<T> tableModels) where T : TableModel<T>, new()
        {
            return TableModel<T>.GroupInsert(tableModels);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert<T>(this IEnumerable<T> tableModels, SqlConnection connection) where T : TableModel<T>, new()
        {
            return TableModel<T>.GroupInsert(tableModels, connection);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static bool GroupInsert<T>(this IEnumerable<T> tableModels, SqlTransaction transaction) where T : TableModel<T>, new()
        {
            return TableModel<T>.GroupInsert(tableModels, transaction);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync<T>(this IEnumerable<T> tableModels) where T : TableModel<T>, new()
        {
            return await TableModel<T>.GroupInsertAsync(tableModels);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="connection">An open SqlConnection to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync<T>(this IEnumerable<T> tableModels, SqlConnection connection) where T : TableModel<T>, new()
        {
            return await TableModel<T>.GroupInsertAsync(tableModels, connection);
        }

        /// <summary>
        /// Executes an Insert of multiple entities into this class' table.
        /// </summary>
        /// <param name="tableModels">A collection of entities of this class</param>
        /// <param name="transaction">An active SqlTransaction to execute the Insert statement against</param>
        /// <returns>Returns a boolean value indicating whether the insertion was successful or not</returns>
        public static async Task<bool> GroupInsertAsync<T>(this IEnumerable<T> tableModels, SqlTransaction transaction) where T : TableModel<T>, new()
        {
            return await TableModel<T>.GroupInsertAsync(tableModels, transaction);
        }
    }
}