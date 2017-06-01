using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using System.Threading.Tasks;
using AlleFinderServiceApp.AllegroServiceReference;
using AlleFinderServiceApp.Properties;
using Newtonsoft.Json;

namespace AlleFinderServiceApp
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AlleFinderService : IAlleFinderService
    {
        private readonly servicePortClient _client = new servicePortClient();
        private readonly string _webapiKey = "fbf1731c";
        private long _verKey;
        private string _sessionId;
        private CatInfoType[] _categoriesList;
        private readonly Timer _updateTimer = new Timer(1000 * 60 * 60 * 24);

        public AlleFinderService()
        {
            string[] credentials = Resources.Credentials.Split(' ');
            _client.doQuerySysStatus(3, 1, _webapiKey, out _verKey);
            _sessionId = _client.doLoginEnc(credentials[0], credentials[1], 1, _webapiKey, _verKey,
                out long userId, out long serverTime);
            UpdateCategories();
            _updateTimer.Elapsed += (sender, args) => UpdateCategories();
        }

        public void UpdateCategories()
        {
            _categoriesList = _client.doGetCatsData(1, 0, _webapiKey, out _verKey, out string verStr);
        }

        public string GetFiltersListByCategoryIdJson(string categoryId)
        {
            var filters = new[] { new FilterOptionsType { filterId = "category", filterValueId = new[] { categoryId } } };

            _client.doGetItemsList(_webapiKey, 1, filters, null, 0, 0, 6, out var itemsFeaturedCount,
                out var itemsList, out var categoriesList, out var filtersList, out var filtersRejected);

            return JsonConvert.SerializeObject(filtersList);
        }

        public string GetItemsListJson(string filtersJson, int resultSize, int resultOffset)
        {
            FilterOptionsType[] filters = JsonConvert.DeserializeObject<FilterOptionsType[]>(filtersJson);
            var sortOptions = new SortOptionsType { sortType = "startingTime", sortOrder = "desc" };
            _client.doGetItemsList(_webapiKey, 1, filters, sortOptions, resultSize, resultOffset, 3, out var itemsFeaturedCount,
                out var itemsList, out var categoriesList, out var filtersList, out var filtersRejected);

            return JsonConvert.SerializeObject(itemsList);
        }

        public string GetCategoriesListJson() => JsonConvert.SerializeObject(_categoriesList);

        public string GetCategoriesListByPhraseJson(string phrase) =>
            JsonConvert.SerializeObject(
                _categoriesList.Where(n => n.catName.IndexOf(phrase, StringComparison.CurrentCultureIgnoreCase) >= 0));

        public string[] GetCategoriesListPathsByPhrase(string phrase)
        {
            var foundCategories = _categoriesList.Where
                (n => n.catName.IndexOf(phrase, StringComparison.CurrentCultureIgnoreCase) >= 0).ToArray();

            string[] categoriesPaths = new string[foundCategories.Length];
            for (int i = 0; i < foundCategories.Length; ++i)
            {
                categoriesPaths[i] = foundCategories[i].catName;
                var catIt = foundCategories[i];
                while (true)
                {
                    if (catIt.catParent != 0)
                    {
                        catIt = _categoriesList.Single(n => n.catId == catIt.catParent);
                        categoriesPaths[i] = catIt.catName + " -> " + categoriesPaths[i];
                    }
                    else break;
                }
            }
            return categoriesPaths;
        }
    }
}
