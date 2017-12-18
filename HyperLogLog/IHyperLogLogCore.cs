namespace HLLCardinalityEstimator
{
    public interface IHyperLogLogCore
    {
        /// <summary>
        /// Adds object's hash to the structure.
        /// Hash needs to be produced by a 'good' hash function.
        /// In practice it does not have to be cryptographycally secure.
        /// </summary>
        void AddHash(ulong hash);

        /// <summary>
        /// Returns estimated count of distict hashes added to HyperLogLogCore.
        /// There is no caching, estimated count is calculated on every call of this method.
        /// </summary>
        int CalculateEstimatedCount();

        /// <summary>
        /// Merges other HyperLogLogCore instance into the current one.
        /// This method does change the current instance, but it does not change the other one.
        /// </summary>
        void Merge(HyperLogLogCore other);


    }
}