using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Plugin.Affiliate.Amazon.Core;
using Nop.Plugin.Affiliate.Amazon.Domain;
using Nop.Plugin.Affiliate.Amazon.Models;
using Nop.Plugin.Affiliate.Amazon.Models.Response;
using Nop.Plugin.Affiliate.CategoryMap;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Affiliate.CategoryMap.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    public class AmazonService : IAmazonService
    {
        private const string responseBrowseLookup = "BrowseNodeInfo";
        private const string responseItemSearch = "Small";
        private const string responseItemLookup = "Accessories,EditorialReview,Images,ItemAttributes,Offers,Reviews,SalesRank,Variations,VariationImages,VariationMatrix,VariationOffers";
        private const int _source = 2;
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        public AmazonService()
        {

        }
        #region Methods
        public BrowseNodeLookupResponse BrowseNodeLookup(int accountIndex, string browseNodeId, string responseGroup = "BrowseNodeInfo")
        {
            IDictionary<string, string> request = new Dictionary<string, string>();
            request["Operation"] = "BrowseNodeLookup";
            request["BrowseNodeId"] = browseNodeId;
            request["ResponseGroup"] = HttpUtility.UrlDecode(responseGroup);

            return Fetch<BrowseNodeLookupResponse>(accountIndex, request);
        }

        public ItemLookupResponse ItemLookup(int accountIndex, string itemId, string responseGroup = "", params KeyValuePair<string, string>[] param)
        {
            if (responseGroup == "")
                responseGroup = responseItemLookup;
            IDictionary<string, string> request = new Dictionary<string, string>();
            request["Operation"] = "ItemLookup";
            request["ItemId"] = itemId;
            request["ResponseGroup"] = HttpUtility.UrlDecode(responseGroup);
            if (param != null)
            {
                foreach (var p in param)
                {
                    request.Add(p.Key, p.Value);
                }
            }
            return Fetch<ItemLookupResponse>(accountIndex, request);
        }

        public ItemSearchResponse ItemSearch(int accountIndex, string searchIndex, string browseNode, string keywords, string responseGroup = "", params KeyValuePair<string, string>[] param)
        {
            if (responseGroup == "")
                responseGroup = responseItemSearch;
            IDictionary<string, string> request = new Dictionary<string, String>();
            if (!String.IsNullOrEmpty(keywords))
                request["Keywords"] = keywords;
            request["Operation"] = "ItemSearch";
            request["SearchIndex"] = searchIndex;
            request["BrowseNode"] = browseNode;
            request["ResponseGroup"] = HttpUtility.UrlDecode(responseGroup);
            if (param != null)
            {
                foreach (var p in param)
                {
                    request.Add(p.Key, p.Value);
                }
            }
            return Fetch<ItemSearchResponse>(accountIndex, request);

        }

        public void SyncCategory(IRepository<CategoryAmazon> _categoryAmazonRepo, string browseNodeID = "")
        {
            var query = _categoryAmazonRepo.Table;
            if (browseNodeID != "")
            {
                query = query.Where(x => x.BrowseNodeID == browseNodeID);
            }
            else
            {
                query = query.Where(x => x.IsCategoryRoot);
            }

            var roots = query.ToList();
            if (roots != null)
            {
                int stt = 0;
                Parallel.ForEach(roots, new ParallelOptions { MaxDegreeOfParallelism = AffAmazonContext.Instance.Count },
                    root =>
                    {
                        stt++;
                        int index = stt % AffAmazonContext.Instance.Count;
                        SyncBrowseNode(index, root.BrowseNodeID, root.ParentBrowseNodeID, root.SearchIndex, root.Level);
                    });
            }
        }

        public void SyncProducts(ICategoryService _categoryService,
            ICategoryMappingService _categoryMappingService,
            IRepository<CategoryAmazon> _categoryAmazonRepo,
            int storeId, int categoryId, string keywords = "", SyncProperties syncProperties = SyncProperties.All)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                throw new ArgumentNullException("category");

            var categoryMaps = _categoryMappingService.GetAllByCategoryId(2, categoryId);
            if (categoryMaps != null)
            {
                foreach (var categoryMap in categoryMaps)
                {
                    var browseNode = _categoryAmazonRepo.Table.First(x => x.Id == categoryMap.CategorySourceId);
                    if (browseNode != null)
                    {
                        var asins = new List<string>();
                        var syncItemSearch = Parallel.For(0, 10, new ParallelOptions { MaxDegreeOfParallelism = AffAmazonContext.Instance.Count },
                        stt =>
                        {
                            int pageIndex = stt + 1;
                            var index = stt % AffAmazonContext.Instance.Count;
                            var itemSearchResponse = this.ItemSearch(index, browseNode.SearchIndex, browseNode.BrowseNodeID, keywords, param: new[] { new KeyValuePair<string, string>("ItemPage", pageIndex.ToString()) });
                            if (itemSearchResponse != null)
                            {
                                var _productMappingRepo = EngineContext.Current.Resolve<IRepository<ProductMapping>>();
                                foreach (var i in itemSearchResponse.Items)
                                {
                                    var asin = i.ASIN;// i.ParentASIN != null &&  != i.ParentASIN ? i.ParentASIN : i.ASIN;
                                    var productMapping = _productMappingRepo.TableNoTracking.FirstOrDefault(p => p.ProductSourceId == asin &&
                                                          p.SourceId == 2);
                                    if (productMapping == null)
                                    {
                                        productMapping = new ProductMapping
                                        {
                                            Id = 0,
                                            Price = 0,
                                            ProductId = 0,
                                            ProductSourceId = asin,
                                            ProductSourceLink = "",
                                            SourceId = 2
                                        };
                                        _productMappingRepo.Insert(productMapping);
                                        if (productMapping.Id > 0)
                                            asins.Add(productMapping.ProductSourceId);
                                    }
                                }
                            }
                        });
                        if (syncItemSearch.IsCompleted)
                        {
                            if (asins != null)
                            {
                                int stt = 0;
                                var syncProduct = Parallel.ForEach(asins, new ParallelOptions { MaxDegreeOfParallelism = AffAmazonContext.Instance.Count },
                                    asin =>
                                    {
                                        stt++;
                                        int pageIndex = stt;
                                        var index = stt % AffAmazonContext.Instance.Count;
                                        var itemLookupResponse = this.ItemLookup(index, asin);
                                        if (itemLookupResponse != null)
                                        {
                                            var item = itemLookupResponse.Items.Item;
                                            if (item != null)
                                            {
                                                var _productRepo = EngineContext.Current.Resolve<IRepository<Product>>();
                                                var _productMappingRepo = EngineContext.Current.Resolve<IRepository<ProductMapping>>();
                                                var _categoryService1 = EngineContext.Current.Resolve<ICategoryService>();
                                                string message = "";
                                                SaveProduct(_productRepo, _productMappingRepo, _categoryService1, index, item, category.Id, syncProperties, out message);
                                                var logger = EngineContext.Current.Resolve<ILogger>();
                                                logger.Warning("End " + stt + "/" + asins.Count + ":" + item.ASIN + " is " + message);
                                            }
                                        }
                                    });
                            }
                        }
                    }
                }
            }
        }

        public void UpdateProducts(IRepository<ProductMapping> _productMappingRepo,
            int storeId, int categoryId, SyncProperties syncProperties)
        {
            try
            {
                var _amazonProvider = EngineContext.Current.Resolve<IAmazonProvider>();

                var pids = _amazonProvider.GetAllProductMapping(new ProductParameter
                {
                    CategoryID = categoryId,
                    IsPublished = true
                }).ToList();
                int count = pids.Count;
                int page = 1;
                int size = 10;

                var pidsw = pids.Skip((page - 1) * size).Take(size).ToList();

                int stt = 0;
                while (pidsw != null && pidsw.Count > 0)
                {
                    var productMappings = _productMappingRepo.Table.Where(x => pidsw.Contains(x.ProductId) && x.SourceId == 2).OrderByDescending(x => x.ProductId).ToList();

                    if (productMappings != null)
                    {
                        Parallel.For(0, productMappings.Count - 1, new ParallelOptions { MaxDegreeOfParallelism = AffAmazonContext.Instance.Count }, i =>
                           {
                               string message = "";
                               stt++;
                               var productMapping = productMappings[i];
                               if (productMapping != null)
                               {
                                   int index = i % AffAmazonContext.Instance.Count;
                                   var itemLookupResponse = this.ItemLookup(index, productMapping.ProductSourceId);
                                   if (itemLookupResponse != null && itemLookupResponse.Items.Item != null)
                                   {
                                       var item = itemLookupResponse.Items.Item;
                                       if (item == null)
                                       {
                                           var _productRepo = EngineContext.Current.Resolve<IRepository<Product>>();
                                           var product = _productRepo.Table.FirstOrDefault(x => x.Sku == productMapping.ProductSourceId);
                                           if (product != null)
                                           {
                                               product.Deleted = true;
                                               var _productService = EngineContext.Current.Resolve<IProductService>();
                                               _productService.UpdateProduct(product);
                                           }
                                       }
                                       else
                                       {
                                           var _categoryService = EngineContext.Current.Resolve<ICategoryService>();
                                           var _productRepo = EngineContext.Current.Resolve<IRepository<Product>>();
                                           var _productMappingRepo1 = EngineContext.Current.Resolve<IRepository<ProductMapping>>();

                                           SaveProduct(_productRepo, _productMappingRepo1, _categoryService, index, item, categoryId, syncProperties, out message);
                                       }
                                   }
                               }
                               var logger = EngineContext.Current.Resolve<ILogger>();
                               logger.Information("End " + stt + "/" + count + ":" + productMapping.ProductSourceId + " is " + message);
                           });
                    }
                    page++;
                    pidsw = pids.Skip((page - 1) * size).Take(size).ToList(); //_productAmazonService.GetAllProductMappingByCategoryId(categoryId, true, page, size).ToList();
                }
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(ex.Message, ex);
            }
        }

        public void SyncProduct(IRepository<Product> _productRepo,
            IRepository<ProductMapping> _productMappingRepo,
            int storeId, int id, SyncProperties properties)
        {
            var productMapping = _productMappingRepo.GetById(id);
            if (productMapping != null && productMapping.ProductId != 0)
            {
                var product = _productRepo.GetById(productMapping.ProductId);
                if (product != null)
                {
                    if (AffAmazonContext.Instance.Count > 0)
                    {
                        var itemLookupResponse = this.ItemLookup(0, productMapping.ProductSourceId);
                        if (itemLookupResponse != null)
                        {
                            var item = itemLookupResponse.Items.Item;
                            if (item == null)
                            {
                                if (product != null)
                                {
                                    product.Deleted = true;
                                    var _productService = EngineContext.Current.Resolve<IProductService>();
                                    _productService.UpdateProduct(product);
                                }
                            }
                            else
                            {
                                var _categoryService = EngineContext.Current.Resolve<ICategoryService>();
                                string message = "";
                                SaveProduct(_productRepo, _productMappingRepo, _categoryService, 0, item, 0, properties, out message);
                            }

                        }
                    }

                }
            }
        }
        #endregion


        #region Utilities
        private void SyncBrowseNode(int accountIndex, string browseNodeId, string parentBrowseNodeId, string searchIndex, int level)
        {
            var browseNodeLookup = this.BrowseNodeLookup(accountIndex, browseNodeId);
            if (browseNodeLookup != null && browseNodeLookup.BrowseNodes.BrowseNode != null)
            {

                var browseNode = browseNodeLookup.BrowseNodes.BrowseNode;
                if (browseNode != null)
                {
                    var _categoryAmazonService = EngineContext.Current.Resolve<ICategoryAmazonService>();
                    var categoryAmazon = _categoryAmazonService.GetByBrowseNodeID(browseNode.BrowseNodeId) ??
                 new CategoryAmazon
                 {
                     Id = 0,
                     BrowseNodeID = browseNode.BrowseNodeId,
                     IsCategoryRoot = browseNode.IsCategoryRoot == 1,
                     ParentBrowseNodeID = parentBrowseNodeId,
                     Name = browseNode.Name,
                     SearchIndex = searchIndex,
                     Level = level
                 };
                    if (categoryAmazon.Id == 0)
                        _categoryAmazonService.Insert(categoryAmazon);
                    else
                    {
                        categoryAmazon.BrowseNodeID = browseNode.BrowseNodeId;
                        categoryAmazon.Name = browseNode.Name;
                        categoryAmazon.Level = level;
                        _categoryAmazonService.Update(categoryAmazon);
                    }
                    if (browseNode.Children != null && browseNode.Children.Count() > 0)
                    {
                        foreach (var children in browseNode.Children)
                        {
                            var chirldAmazon = _categoryAmazonService.GetByBrowseNodeID(children.BrowseNodeId);
                            if (chirldAmazon == null)
                            {
                                _categoryAmazonService.Insert(new CategoryAmazon
                                {
                                    BrowseNodeID = children.BrowseNodeId,
                                    Id = 0,
                                    IsCategoryRoot = false,
                                    Name = children.Name,
                                    SearchIndex = categoryAmazon.SearchIndex,
                                    Level = level + 1,
                                    ParentBrowseNodeID = categoryAmazon.BrowseNodeID
                                });
                            }
                            SyncBrowseNode(accountIndex, children.BrowseNodeId, browseNode.BrowseNodeId, categoryAmazon.SearchIndex, level + 1);
                        }
                    }
                }

            }
        }

        private void SaveProduct(IRepository<Product> _productRepo,
            IRepository<ProductMapping> _productMappingRepo,
            ICategoryService _categoryService,
            int accountIndex, Item item, int categoryId, SyncProperties syncProperties, out string message)
        {
            try
            {
                var productMapping = _productMappingRepo.Table.FirstOrDefault(x => x.ProductSourceId == item.ASIN);
                if (productMapping != null)
                {
                    productMapping.ProductSourceLink = item.DetailPageURL;

                    var product = _productRepo.GetById(productMapping.ProductId) ?? new Product
                    {
                        Sku = item.ASIN,
                        Name = item.ItemAttributes.Title,
                        ProductType = ProductType.SimpleProduct,
                        VisibleIndividually = true,
                        AllowCustomerReviews = true,
                        UnlimitedDownloads = true,
                        MaxNumberOfDownloads = 10,
                        RecurringCycleLength = 100,
                        RecurringTotalCycles = 10,
                        RentalPriceLength = 1,

                        IsShipEnabled = true,

                        NotifyAdminForQuantityBelow = 1,

                        OrderMinimumQuantity = 1,
                        OrderMaximumQuantity = 10000,

                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                        Published = false,
                    };

                    if (product.Id == 0)
                    {
                        _productRepo.Insert(product);
                        productMapping.ProductId = product.Id;
                        _productMappingRepo.Update(productMapping);
                    }
                    if (product.Id > 0)
                    {
                        //if (item.ASIN != item.ParentASIN)
                        //{
                        //    var parent = _productRepo.Table.FirstOrDefault(x => x.Sku == item.ParentASIN);

                        //    if (parent != null && parent.Id != product.Id)
                        //    {
                        //        product.ParentGroupedProductId = parent.Id;
                        //        product.VisibleIndividually = false;
                        //        product.ProductType = ProductType.SimpleProduct;
                        //    }
                        //}
                        //else
                        //{
                        //    product.ParentGroupedProductId = 0;
                        //}
                        product.Sku = item.ASIN;
                        if (syncProperties.HasFlag(SyncProperties.DetailPageURL))
                            productMapping.ProductSourceLink = item.DetailPageURL;
                        if (syncProperties.HasFlag(SyncProperties.Name))
                            product.Name = item.ItemAttributes.Title;

                        if (syncProperties.HasFlag(SyncProperties.ShortDescription))
                            product.ShortDescription = item.ItemAttributes.Feature != null ? String.Join("\n", item.ItemAttributes.Feature) : "";

                        if (syncProperties.HasFlag(SyncProperties.FullDescription))
                            product.FullDescription = item.EditorialReviews != null ? item.EditorialReviews.EditorialReview.Content : "";

                        _productRepo.Update(product);

                        if (categoryId > 0)
                        {
                            var _productCategoryRepo = EngineContext.Current.Resolve<IRepository<ProductCategory>>();
                            SaveCategory(_productCategoryRepo, categoryId, product);
                        }

                        if (syncProperties.HasFlag(SyncProperties.Price))
                        {
                            var _workContext = EngineContext.Current.Resolve<IWorkContext>();
                            var _settingService = EngineContext.Current.Resolve<ISettingService>();
                            var _currencyRepo = EngineContext.Current.Resolve<IRepository<Currency>>();

                            SavePrice(_workContext, _settingService, _currencyRepo, _productRepo, _productMappingRepo, product, productMapping, item);
                        }
                        if (syncProperties.HasFlag(SyncProperties.Images))
                        {
                            if (item.ImageSets != null)
                            {
                                var _pictureService = EngineContext.Current.Resolve<IPictureService>();
                                var pictures = _pictureService.GetPicturesByProductId(product.Id).ToList();
                                if (pictures != null)
                                {
                                    foreach (var picture in pictures)
                                    {
                                        _pictureService.DeletePicture(picture);
                                    }
                                }
                                SavePicture(item.ImageSets, product);
                            }
                        }
                        var _productService = EngineContext.Current.Resolve<IProductService>();
                        _productService.UpdateProduct(product);

                        if (syncProperties.HasFlag(SyncProperties.Variations))
                        {
                            var _productAttributeRepo = EngineContext.Current.Resolve<IRepository<ProductAttribute>>();
                            var _productAttributeMappingRepo = EngineContext.Current.Resolve<IRepository<ProductAttributeMapping>>();
                            var _productAttributeService = EngineContext.Current.Resolve<IProductAttributeService>();
                            List<int> cateIds = product.ProductCategories.Select(x => x.CategoryId).ToList();
                            SaveVariations(_productRepo, _productMappingRepo, accountIndex, product, cateIds, item);
                            //var lowestVariation = _productRepo.Table.Where(x => x.ParentGroupedProductId == product.Id && x.Price > 0).OrderBy(x => x.Price).FirstOrDefault();
                            //if (lowestVariation != null)
                            //{
                            //    product.Name = lowestVariation.Name;
                            //    var productMappingVar = _productMappingRepo.Table.FirstOrDefault(x => x.ProductId == lowestVariation.Id);
                            //    if (productMappingVar != null)
                            //        productMapping.ProductSourceLink = productMappingVar.ProductSourceLink;
                            //    _productService.UpdateProduct(product);
                            //    _productMappingRepo.Update(productMapping);
                            //    var picture = lowestVariation.ProductPictures.OrderBy(x => x.DisplayOrder).FirstOrDefault();
                            //    if (picture != null)
                            //    {
                            //        if (product.ProductPictures.FirstOrDefault(x => x.PictureId == picture.PictureId) == null)
                            //        {
                            //            _productService.InsertProductPicture(new ProductPicture
                            //            {
                            //                Id = 0,
                            //                PictureId = picture.PictureId,
                            //                ProductId = product.Id,
                            //                DisplayOrder = -10
                            //            });
                            //        }
                            //        else
                            //        {
                            //            var productPicture = product.ProductPictures.FirstOrDefault(x => x.PictureId == picture.PictureId);
                            //            productPicture.DisplayOrder = -10;
                            //            _productService.UpdateProductPicture(productPicture);
                            //        }
                            //        int displayOrder = 0;
                            //        foreach (var productPicture in product.ProductPictures.OrderBy(x => x.DisplayOrder))
                            //        {
                            //            displayOrder++;
                            //            productPicture.DisplayOrder = displayOrder;
                            //            _productService.UpdateProductPicture(productPicture);
                            //        }
                            //    }

                            //}
                        }
                        
                    }
                }
                message = "Success";
            }
            catch (Exception ex)
            {
                message = "Error: ";
                message += ex.Message;
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(message, ex);
            }
        }

        private void SaveVariations(IRepository<Product> _productRepo,
            IRepository<ProductMapping> _productMappingRepo,
            int accountIndex, Product parent, List<int> cateIds, Item item)
        {
            try
            {
                if (item.VariationAttributes != null)
                {
                    foreach (var varAttr in item.VariationAttributes)
                    {
                    }
                }
                if (item.Variations != null)
                {
                    var variations = item.Variations.Item;
                    if (variations != null)
                    {
                        parent.ProductType = ProductType.GroupedProduct;
                        var asins = new List<string>();
                        foreach (var var in variations)
                        {
                            var asin = var.ASIN;
                            var productMapping = _productMappingRepo.Table.FirstOrDefault(x => x.ProductSourceId == asin) ??
                            new ProductMapping
                            {
                                Id = 0,
                                ProductId = 0,
                                ProductSourceId = asin,
                                ProductSourceLink = var.DetailPageURL,
                                SourceId = 2
                            };
                            if (productMapping.Id == 0)
                            {
                                _productMappingRepo.Insert(productMapping);
                            }
                            if (productMapping.Id > 0)
                            {
                                asins.Add(productMapping.ProductSourceId);
                            }
                        }
                        foreach (var asin in asins)
                        {
                            var itemLookupResponse = this.ItemLookup(accountIndex, asin);
                            if (itemLookupResponse != null)
                            {
                                var itemVar = itemLookupResponse.Items.Item;
                                if (itemVar != null)
                                {
                                    SaveVariation(_productRepo, _productMappingRepo, itemVar, parent, cateIds);
                                }
                            }
                        }
                        _productRepo.Update(parent);
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                Debug.WriteLine(ex.Message);
                logger.Error(ex.Message, ex);
            }
        }

        private void SaveVariation(IRepository<Product> _productRepo,
            IRepository<ProductMapping> _productMappingRepo,
            Item item, Product parent, List<int> cateIds)
        {
            try
            {
                var productMapping = _productMappingRepo.Table.FirstOrDefault(x => x.ProductSourceId == item.ASIN);
                if (productMapping != null)
                {
                    var product = _productRepo.Table.FirstOrDefault(x => x.Sku == productMapping.ProductSourceId) ??
                        new Product
                        {
                            ParentGroupedProductId = parent.Id,
                            Sku = item.ASIN,
                            Name = item.ItemAttributes.Title,
                            ShortDescription = item.ItemAttributes.Feature != null ? String.Join("\n", item.ItemAttributes.Feature) : "",
                            FullDescription = item.EditorialReviews != null ? item.EditorialReviews.EditorialReview.Content : "",
                            VisibleIndividually = false,
                            ProductType = ProductType.SimpleProduct,
                            AllowCustomerReviews = true,
                            UnlimitedDownloads = true,
                            MaxNumberOfDownloads = 10,
                            RecurringCycleLength = 100,
                            RecurringTotalCycles = 10,
                            RentalPriceLength = 1,

                            IsShipEnabled = true,

                            NotifyAdminForQuantityBelow = 1,

                            OrderMinimumQuantity = 1,
                            OrderMaximumQuantity = 10000,

                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow,
                            Published = parent.Published,
                        };
                    bool isNew = false;
                    if (product.Id == 0)
                    {
                        _productRepo.Insert(product);
                        isNew = true;
                    }
                    if (product.Id > 0)
                    {
                        if (cateIds.Count > 0)
                        {

                        }

                        product.ParentGroupedProductId = parent.Id;
                        productMapping.ProductId = product.Id;
                        productMapping.ProductSourceLink = item.DetailPageURL;
                        _productRepo.Update(product);
                        var _workContext = EngineContext.Current.Resolve<IWorkContext>();
                        var _settingService = EngineContext.Current.Resolve<ISettingService>();
                        var _currencyRepo = EngineContext.Current.Resolve<IRepository<Currency>>();
                        SavePrice(_workContext, _settingService, _currencyRepo, _productRepo, _productMappingRepo, product, productMapping, item);
                        if (isNew && product.ProductPictures.Count == 0)
                        {
                            SavePicture(item.ImageSets, product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                Debug.WriteLine(ex.Message);
                logger.Error(ex.Message, ex);
            }
        }

        private void SaveAttribute(Item item, Product product)
        {
            try
            {
                var attributes = item.ItemAttributes;
                if (attributes != null)
                {

                }
            }

            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(ex.Message, ex);
            }
        }



        private void SaveCategory(IRepository<ProductCategory> _productCategoryRepo, int categoryId, Product product)
        {
            try
            {
                var pc = new ProductCategory
                {
                    CategoryId = categoryId,
                    ProductId = product.Id,
                    DisplayOrder = 1
                };
                _productCategoryRepo.Insert(pc);
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                Debug.WriteLine(ex.Message);
                logger.Error(ex.Message, ex);
            }
        }

        private void SavePrice(IWorkContext _workContext,
            ISettingService _settingService,
            IRepository<Currency> _currencyRepo,
            IRepository<Product> _productRepo,
            IRepository<ProductMapping> _productMappingRepo,
             Product product, ProductMapping productMapping, Item item)
        {
            var currency = _currencyRepo.TableNoTracking.FirstOrDefault(x => x.CurrencyCode == "USD");
            decimal rate = _workContext.WorkingCurrency.Rate / currency.Rate;
            try
            {
                decimal originPrice = 0;
                decimal oldPrice = 0;
                decimal price = 0;

                Price(item, out price, out oldPrice, out originPrice, rate);
                var categorySettings = _settingService.LoadSetting<ProductMappingSettings>();
                product.DisableBuyButton = false;
                if (price == 0)
                {
                    product.DisableBuyButton = true;
                    product.Published = false;
                }

                product.Price = price * (1 + categorySettings.AdditionalCostPercent / 100);
                product.OldPrice = 0;// oldPrice * (1 + categorySettings.AdditionalCostPercent / 100);

                productMapping.Price = originPrice;
                _productRepo.Update(product);
                _productMappingRepo.Update(productMapping);
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                Debug.WriteLine(ex.Message);
                logger.Error(ex.Message, ex);
            }
        }

        private void SavePicture(ImageSet[] imageSets, Product product)
        {
            try
            {
                int display = 1;
                int stt = 0;
                Parallel.ForEach(imageSets, new ParallelOptions { MaxDegreeOfParallelism = 3 }, image =>
                {
                    stt++;
                    display = stt;
                    var amazonImage = SelectImage(image);
                    if (amazonImage != null)
                    {
                        display++;
                        if (!string.IsNullOrEmpty(amazonImage.URL))
                        {
                            SyncImage(amazonImage.URL, product.Id, image.Category, display);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                Debug.WriteLine(ex.Message);
                logger.Error(ex.Message, ex);
            }
        }

        private int SyncImage(string url, int productId, string category, int display)
        {
            System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            _HttpWebRequest.AllowWriteStreamBuffering = true;

            _HttpWebRequest.UserAgent = "Chrome/56.0.2924.87";
            _HttpWebRequest.Referer = "http://www.google.com/";

            _HttpWebRequest.Timeout = 20000;
            int pictureId = 0;
            try
            {
                using (System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse())
                {
                    using (System.IO.Stream _WebStream = _WebResponse.GetResponseStream())
                    {
                        using (var _tmpImage = Image.FromStream(_WebStream))
                        {
                            try
                            {
                                var contentType = "";
                                var fileExtension = Path.GetExtension(url);
                                if (!String.IsNullOrEmpty(fileExtension))
                                    fileExtension = fileExtension.ToLowerInvariant();
                                //contentType is not always available 
                                //that's why we manually update it here
                                //http://www.sfsu.edu/training/mimetype.htm
                                if (String.IsNullOrEmpty(contentType))
                                {
                                    switch (fileExtension)
                                    {
                                        case ".bmp":
                                            contentType = MimeTypes.ImageBmp;
                                            break;
                                        case ".gif":
                                            contentType = MimeTypes.ImageGif;
                                            break;
                                        case ".jpeg":
                                        case ".jpg":
                                        case ".jpe":
                                        case ".jfif":
                                        case ".pjpeg":
                                        case ".pjp":
                                            contentType = MimeTypes.ImageJpeg;
                                            break;
                                        case ".png":
                                            contentType = MimeTypes.ImagePng;
                                            break;
                                        case ".tiff":
                                        case ".tif":
                                            contentType = MimeTypes.ImageTiff;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                ImageConverter converter = new ImageConverter();
                                var vendorPictureBinary = (byte[])converter.ConvertTo(_tmpImage, typeof(byte[]));

                                var _pictureService = EngineContext.Current.Resolve<IPictureService>();
                                var picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);
                                if (picture != null && picture.Id > 0)
                                {
                                    var _productService = EngineContext.Current.Resolve<IProductService>();
                                    _productService.InsertProductPicture(new ProductPicture
                                    {
                                        Id = 0,
                                        PictureId = picture.Id,
                                        ProductId = productId,
                                        DisplayOrder = category == "primary" ? 1 : display,
                                    });
                                    pictureId = picture.Id;
                                }

                            }
                            catch (Exception ex)
                            {
                                var logger = EngineContext.Current.Resolve<ILogger>();
                                logger.Error(ex.Message, ex);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                Debug.WriteLine(ex.Message);
                logger.Error(ex.Message, ex);
            }

            return pictureId;
        }



        private T Fetch<T>(int accountIndex, IDictionary<string, string> request) where T : class
        {
            string _awsNamespace = string.Format("http://webservices.amazon.com/{0}/{1}", "AWSECommerceService", "2013-08-01");
            string xmlDoc = null;
            T result = default(T);
            request["Service"] = AffAmazonContext.Instance.Service;
            request["AssociateTag"] = AffAmazonContext.Instance.Accounts[accountIndex].AssociateTag;
            request["Version"] = AffAmazonContext.Instance.Version;
            string requestUrl = "";
            SignedRequestHelper helper = new SignedRequestHelper(AffAmazonContext.Instance.Accounts[accountIndex].AccessKeyID, AffAmazonContext.Instance.Accounts[accountIndex].SecretKey, AffAmazonContext.Instance.Endpoint);
            requestUrl = helper.Sign(request);
            bool saveFile = false;
            if (saveFile)
            {
                var fileName = "";
                var method = request["Operation"];

                string folder = System.IO.Path.Combine(AffAmazonContext.Instance.Folder, DateTime.Now.ToString("MMddyyyy"), method);
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);
                switch (method)
                {
                    case "ItemLookup":
                        fileName = System.IO.Path.Combine(folder, request["ItemId"] + ".xml");
                        break;
                    case "BrowseNodeLookup":
                        fileName = System.IO.Path.Combine(folder, request["BrowseNodeId"] + ".xml");
                        break;
                    case "ItemSearch":
                        folder = System.IO.Path.Combine(folder, request["BrowseNode"]);
                        if (!System.IO.Directory.Exists(folder))
                            System.IO.Directory.CreateDirectory(folder);
                        fileName = System.IO.Path.Combine(folder, request["ItemPage"] + ".xml");
                        break;
                }
                if (!System.IO.File.Exists(fileName))
                {
                    HttpWebRequest webRequest = (System.Net.HttpWebRequest)HttpWebRequest.Create(requestUrl);
                    webRequest.UserAgent = "Chrome/56.0.2924.87";
                    webRequest.AllowWriteStreamBuffering = true;
                    webRequest.Timeout = 20000;

                    if (AffAmazonContext.Instance.IsUsed(accountIndex))
                    {
                        Thread.Sleep(AffAmazonContext.Instance.Wait(accountIndex));
                    }
                    AffAmazonContext.Instance.UpdateAccount(accountIndex, DateTime.Now);
                    try
                    {
                        using (var response = webRequest.GetResponse())
                        {
                            using (StreamReader xmlReader = new StreamReader(response.GetResponseStream()))
                            {
                                if (xmlReader != null)
                                {
                                    if (System.IO.File.Exists(fileName))
                                        System.IO.File.Delete(fileName);
                                    using (StreamWriter xmlWriter = new StreamWriter(fileName, false))
                                        xmlWriter.Write(xmlReader.ReadToEnd());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        //if (ex.Message.Contains("503"))
                        //{
                        //    times++;
                        //    if (times < 5)
                        //    {
                        //        message = string.Format("{0} Error 503 is {1} times: {2}", accountIndex, times, requestUrl);
                        //    }
                        //}
                        //Debug.WriteLine(message);
                        var logger = EngineContext.Current.Resolve<ILogger>();
                        logger.Error(message, ex);
                    }
                }
                try
                {
                    using (StreamReader xmlReader = new StreamReader(fileName))
                    {
                        if (xmlReader != null)
                        {
                            xmlDoc = xmlReader.ReadToEnd();
                        }

                        if (!string.IsNullOrEmpty(xmlDoc))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(T), _awsNamespace);
                            result = (T)serializer.Deserialize(new StringReader(xmlDoc));
                        }

                    }
                }
                catch (Exception ex)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    logger.Error(ex.Message, ex);
                }
            }
            else
            {
                HttpWebRequest webRequest = (System.Net.HttpWebRequest)HttpWebRequest.Create(requestUrl);
                webRequest.UserAgent = "Chrome/56.0.2924.87";
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 20000;

                if (AffAmazonContext.Instance.IsUsed(accountIndex))
                {
                    Thread.Sleep(AffAmazonContext.Instance.Wait(accountIndex));
                }
                AffAmazonContext.Instance.UpdateAccount(accountIndex, DateTime.Now);
                try
                {
                    using (var response = webRequest.GetResponse())
                    {
                        using (StreamReader xmlReader = new StreamReader(response.GetResponseStream()))
                        {
                            if (xmlReader != null)
                            {
                                xmlDoc = xmlReader.ReadToEnd();
                            }

                            if (!string.IsNullOrEmpty(xmlDoc))
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(T), _awsNamespace);
                                result = (T)serializer.Deserialize(new StringReader(xmlDoc));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    logger.Error(message, ex);
                }
            }
            return result;
        }

        private List<String> Images(List<XElement> imageSetXmls)
        {
            string _awsNamespace = string.Format("http://webservices.amazon.com/{0}/{1}", "AWSECommerceService", "2013-08-01");
            var lst = new List<string>();
            foreach (var imageSetXml in imageSetXmls)
            {
                var largeXml = imageSetXml.Element("{" + _awsNamespace + "}" + "LargeImage");
                if (largeXml != null)
                {
                    lst.Add(largeXml.Element("{" + _awsNamespace + "}" + "URL").Value);
                }
                else
                {
                    var mediumXml = imageSetXml.Element("{" + _awsNamespace + "}" + "MediumImage");
                    if (mediumXml != null)
                    {
                        lst.Add(mediumXml.Element("{" + _awsNamespace + "}" + "URL").Value);
                    }
                    else
                    {
                        var smallXml = imageSetXml.Element("{" + _awsNamespace + "}" + "SmallImage");
                        if (smallXml != null)
                        {
                            lst.Add(smallXml.Element("{" + _awsNamespace + "}" + "URL").Value);
                        }
                    }
                }
            }
            return lst;
        }

        //private void ConvertFormatPriceToSystemPrice(string formatPrice, out decimal price)
        //{
        //    price = 0M;
        //    if (!string.IsNullOrEmpty(formatPrice))
        //    {
        //        Regex rg = new Regex(@"[\d]+(.[\d][\d]*)?");
        //        string currencyCode = rg.Replace(formatPrice, "");
        //        string priceStr = formatPrice.Replace(currencyCode, "");

        //        Decimal.TryParse(priceStr, out price);
        //    }
        //}
        private void Price(Item item, out decimal price, out decimal oldPrice, out decimal originPrice, decimal rate)
        {
            price = 0;
            oldPrice = 0;
            originPrice = 0;

            if (item.ItemAttributes.ListPrice != null)
            {

                if (item.ItemAttributes.ListPrice != null)
                {
                    var listPrice = item.ItemAttributes.ListPrice;
                    oldPrice = listPrice.Amount / 100;
                    oldPrice = oldPrice * rate;
                }
            }
            if (item.Offers != null)
            {
                if (item.Offers.Offer != null)
                {
                    var offer = item.Offers.Offer.FirstOrDefault();
                    var offerPrice = offer.OfferListing.SalePrice ?? offer.OfferListing.Price;
                    originPrice = offerPrice.Amount / 100;

                    price = originPrice * rate;
                }
            }

        }
        private AmazonImage SelectImage(ImageSet imageSet)
        {
            return imageSet.HiResImage != null ? imageSet.HiResImage :
                imageSet.LargeImage != null ? imageSet.LargeImage :
                imageSet.MediumImage != null ? imageSet.MediumImage :
                imageSet.ThumbnailImage != null ? imageSet.ThumbnailImage :
                imageSet.SmallImage != null ? imageSet.SmallImage :
                imageSet.SwatchImage != null ? imageSet.SwatchImage :
                null;
        }
        #endregion
    }
}
