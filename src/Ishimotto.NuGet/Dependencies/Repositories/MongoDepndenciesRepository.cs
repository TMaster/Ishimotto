using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using NuGet;

namespace Ishimotto.NuGet.Dependencies.Repositories
{
    /// <summary>
    /// An implementation of <see cref="IDependenciesRepostory"/> using MongoDB
    /// </summary>
    public class MongoDepndenciesRepository : IDependenciesRepostory
    {
        //Todo: consider moving this class to a diffrent dll, so this dll would not be depndended on Mongo

        #region Data Members

        /// <summary>
        /// repositorie's depdendencies
        /// </summary>
        private IMongoCollection<PackageDto> mDependencies;

        private object mSyncObject = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// reates new instance of <see cref="MongoDepndenciesRepository"/>
        /// </summary>
        /// <param name="mongoConnection">The connection to the MongoDb</param>
        public MongoDepndenciesRepository(string mongoConnection, string DbName, string collectionName)
        {
            mDependencies =
                new MongoClient(mongoConnection).GetDatabase(DbName).GetCollection<PackageDto>(collectionName);

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines wheter a packages exist in the repository
        /// </summary>
        /// <param name="dependency">The packages to check</param>
        /// <returns>Boolean indicating if <see cref="dependency"/> exists in the repository</returns>
        public async Task<bool> IsExist(PackageDto dependency)
        {
            bool result;

            Monitor.Enter(mSyncObject);
            result = await mDependencies.CountAsync(package => package.MongoID == dependency.MongoID) > 0;
            Monitor.Exit(mSyncObject);

            return result;

        }

        /// <summary>
        /// Adds single package to the repository
        /// </summary>
        /// <param name="package">item to add</param>
        /// <returns>Task to indicate when the process is completed</returns>
        public async Task AddDependnecyAsync(PackageDto package)
        {
            await mDependencies.InsertOneAsync(package).ConfigureAwait(false);
        }


        /// <summary>
        /// Determines whether a depdendency should be download
        /// </summary>
        /// <param name="dependency">Dependency to examine</param>
        /// <returns>Boolean indicating if the <see cref="dependency"/></returns>
        public async Task<bool> ShouldDownloadAsync(PackageDependency dependency)
        {
            var versions = await mDependencies.Find(package => package.ID == dependency.Id).Project(package => package.SemanticVersion).ToListAsync();

            return versions.Any(version => dependency.VersionSpec.Satisfies(version));
        }



        /// <summary>
        /// Adds new depdendencies to the repository
        /// </summary>
        /// <param name="dependencies">The depdendnecies to the repository</param>
        /// <returns>A task to indicate when the process is done</returns>
        public async Task AddDepndenciesAsync(IEnumerable<PackageDto> dependencies)
        {
            await mDependencies.InsertManyAsync(dependencies);
        }

        #endregion
    }
}