using HtmlAgilityPack;
using SurgicalInstrumentsApi.Db.Sql;
using SurgicalInstrumentsApi.Db.Sql.Model;
using SurgicalInstrumentsApi.Scraper.Model;
using System;
using System.Xml.Linq;

namespace SurgicalInstrumentsApi.Scraper
{
    public class Program
    {
        private delegate T EntityFactory<T>(string name, string imageUrl);

        public static async Task Main(string[] args)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            // Scrape data from website and add it to db
            using (var db = new SurgicalInstrumentsDbContext())
            using (HttpClient client = new HttpClient())
            {
                db.Database.EnsureCreated();
                string htmlString = await client.GetStringAsync("https://www.stille.se/product-category/surgical-instruments/");
                htmlDoc.LoadHtml(htmlString);
                List<ListData<Category>> categories;
                categories = ProcessNodes<Category>(htmlDoc, GetCategory);

                List<ListData<Subcategory>> subcategories = new List<ListData<Subcategory>>();
                //Scrape subgategories
                foreach (var categoryData in categories)
                {
                    db.Categories.Add(categoryData.Entity);
                    htmlString = await client.GetStringAsync(categoryData.Url);
                    htmlDoc.LoadHtml(htmlString);
                    subcategories.AddRange(ProcessNodes<Subcategory>(htmlDoc, GetSubcategory));

                    List<ListData<Instrument>> instruments = new List<ListData<Instrument>>();
                    //Scrape products
                    foreach (var subcategory in subcategories)
                    {
                        subcategory.Entity.Category = categoryData.Entity;
                        db.Subcategories.Add(subcategory.Entity);
                        htmlString = await client.GetStringAsync(subcategory.Url);
                        htmlDoc.LoadHtml(htmlString);
                        instruments.AddRange(ProcessNodes<Instrument>(htmlDoc, GetInstrument));

                        foreach(var product in instruments)
                        {
                            product.Entity.Subcategory = subcategory.Entity;
                            db.Instruments.Add(product.Entity);
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        public static Category GetCategory(string name, string imageUrl)
        {
            return new Category { Name = name, ImageUrl = imageUrl };
        }

        public static Subcategory GetSubcategory(string name, string imageUrl)
        {
            return new Subcategory { Name = name, ImageUrl = imageUrl };
        }

        public static Instrument GetInstrument(string name, string imageUrl)
        {
            return new Instrument { Name = name, ImageUrl = imageUrl };
        }

        private static List<ListData<T>> ProcessNodes<T>(HtmlDocument htmlDoc, EntityFactory<T> factory)
        {
            List<ListData<T>> pageData = new List<ListData<T>>();
            var nodes = htmlDoc.DocumentNode.SelectNodes("//ul[contains(@class, 'products')]/li[contains(@class, 'product')]");
            foreach (var node in nodes)
            {
                var nameNode = node.SelectNodes(".//div[contains(@class, 'woocommerce-title-container')]//h3").FirstOrDefault();
                var imageNode = node.SelectNodes(".//img").FirstOrDefault();
                var anchorNode = node.SelectNodes("./a[1]").FirstOrDefault();
                if (nameNode != null && imageNode != null && anchorNode != null)
                {
                    var entity = factory(nameNode.InnerText, imageNode.GetAttributeValue("src", ""));
                    pageData.Add(new ListData<T> { Entity = entity, Url = anchorNode.GetAttributeValue("href", "") });
                }
            }

            return pageData;
        }
    }
}