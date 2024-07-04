using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.Contracts
{
    /// <summary>
    /// This Class is a middle man between MeadowDataAccessCore and Target Persistent technology. It Provides Read and
    /// Write methods regarding how target persistent would be able to read and write data.
    /// </summary>
    /// <typeparam name="TToStorageCarrier">The class responsible for carrying data towards the storage. Ie SqlCommand
    /// class would be the data carrier towards ms-sql-server.</typeparam>
    /// <typeparam name="TFromStorageCarrier">The class responsible for carrying data from the storage. Ie SqlDataReader
    /// class would be the data carrier that brings the data from ms-sql-server.</typeparam>
    public interface IStandardDataStorageAdapter<TToStorageCarrier, TFromStorageCarrier>
    {
        /// <summary>
        ///  This method reads the data from the carrier.
        /// </summary>
        /// <param name="carrier">The object responsible for carrying data/information from the storage.</param>
        /// <typeparam name="TModel">The Type of data model corresponding to the data being read from storage.</typeparam>
        /// <returns>A List of TModels which are read from carrier.</returns>
        List<TModel> ReadFromStorage<TModel>(TFromStorageCarrier carrier);

        /// <summary>
        /// This method writes data into carrier.
        /// </summary>
        /// <param name="carrier">The object responsible for carrying data/information towards the storage.</param>
        /// <param name="toStorageInclusion">This object indicates each parts of data must be included/excluded to be written
        /// from the carrier.
        /// </param>
        /// <param name="dataEvaluators">An Instance of ObjectEvaluator, initialized with actual object which is going to be
        /// written into carrier.
        /// </param>
        void WriteToStorage(TToStorageCarrier carrier, IFieldInclusion toStorageInclusion, List<ObjectEvaluator> dataEvaluators);
    }
}