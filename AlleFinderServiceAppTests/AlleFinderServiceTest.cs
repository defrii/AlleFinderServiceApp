using System;
using AlleFinderServiceApp;
using AlleFinderServiceAppTests.AllegroServiceReference;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace AlleFinderServiceAppTests
{
    public class AlleFinderServiceTest
    {
        private readonly IAlleFinderService _service = new AlleFinderService();
        private readonly ITestOutputHelper _output;

        public AlleFinderServiceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("1")]
        public void get_filters_list_by_category_id(string categoryId)
        {
            string filtersJson = _service.GetFiltersListByCategoryIdJson(categoryId);
            FiltersListType[] filters = JsonConvert.DeserializeObject<FiltersListType[]>(filtersJson);
            Assert.NotEmpty(filters);
        }

        [Fact]
        public void get_items_list_from_music_category()
        {
            FilterOptionsType[] filters =
            {
                new FilterOptionsType {filterId = "category", filterValueId = new[] {"1"}}
            };
            string filtersJson = JsonConvert.SerializeObject(filters);
            string itemsListJson = _service.GetItemsListJson(filtersJson, 100, 0);

            ItemsListType[] itemsList = JsonConvert.DeserializeObject<ItemsListType[]>(itemsListJson);

            Assert.Equal(itemsList.Length, 100);
        }

        [Fact]
        public void get_categories_list()
        {
            string categoriesListJson = _service.GetCategoriesListJson();
            CatInfoType[] categoriesList = JsonConvert.DeserializeObject<CatInfoType[]>(categoriesListJson);

            Assert.NotEmpty(categoriesList);
        }

        [Fact]
        public void get_categories_list_by_phrase_muzyka()
        {
            string phrase = "muzyka";
            string categoriesListJson = _service.GetCategoriesListByPhraseJson(phrase);
            _output.WriteLine(categoriesListJson);
            CatInfoType[] categoriesList = JsonConvert.DeserializeObject<CatInfoType[]>(categoriesListJson);

            Assert.NotEmpty(categoriesList);
            Assert.Contains(categoriesList, n => n.catName.IndexOf(phrase, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        [Fact]
        public void get_categories_list_paths_by_phrase_muzyka()
        {
            string phrase = "muzyka";
            string[] categoriesListPaths = _service.GetCategoriesListPathsByPhrase(phrase);
            foreach (var cat in categoriesListPaths)
                _output.WriteLine(cat);
            Assert.NotEmpty(categoriesListPaths);
        }
    }
}
