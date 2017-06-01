using System.ServiceModel;
using System.Threading.Tasks;

namespace AlleFinderServiceApp
{
    [ServiceContract]
    public interface IAlleFinderService
    {
        [OperationContract]
        string GetFiltersListByCategoryIdJson(string categoryId);

        [OperationContract]
        string GetItemsListJson(string filtersJson, int resultSize, int resultOffset);

        [OperationContract]
        string GetCategoriesListJson();

        [OperationContract]
        string GetCategoriesListByPhraseJson(string phrase);

        [OperationContract]
        string[] GetCategoriesListPathsByPhrase(string phrase);
    }
}
