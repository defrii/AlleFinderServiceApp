using System.ServiceModel;

namespace AlleFinderServiceApp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAlleFinderService" in both code and config file together.
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
