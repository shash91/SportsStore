using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.CSharp;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
           public void Can_Paginate()
           {
               //Arrange
               Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
               mock.Setup(m => m.Products).Returns(new Product[]
                   {
                       new Product {ProductID=1,Name="P1" },
                       new Product {ProductID=2,Name="P2" },
                       new Product {ProductID=3,Name="P3" },
                         new Product {ProductID=4,Name="P4" },
                           new Product {ProductID=5,Name="P5" }
                   }.AsQueryable());
               ProductController con = new ProductController(mock.Object);
               con.PageSize = 3;

            //ACt
            ProductsListViewModel result = (ProductsListViewModel)con.List(null,2).Model;

               //Assert
               Product[] arr = result.Products.ToArray();
               Assert.IsTrue(arr.Length == 2);
               Assert.AreEqual(arr[0].Name , "P4");
               Assert.AreEqual(arr[1].Name, "P5");
               }
        [TestMethod]

        public void Can_Generate_Page_Links()
        {
            // Arrange - define an HTML helper - we need to do this
            // in order to apply the extension method
            HtmlHelper myHelper = null;
            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;
            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);
            // Assert
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>"
            + @"<a class=""selected"" href=""Page2"">2</a>"
            + @"<a href=""Page3"">3</a>");

        }
        [TestMethod]
        public void Can_Send_Pagination_View_model()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]

                {
                    new Product {ProductID=1,Name="P1" },
                    new Product {ProductID=2,Name="p2" },
                    new Product {ProductID=3,Name="p3" },
                    new Product {ProductID=4,Name="p4" },
                    new Product {ProductID=5,Name="P5" }
                }.AsQueryable());

            ProductController con = new ProductController(mock.Object);
            con.PageSize = 3;
            ProductsListViewModel result = (ProductsListViewModel)con.List(null,2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.Totalpages, 2);
        }
        [TestMethod]
        public void Can_Filter_By_Category()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="P1",Category="cat1" },
                new Product {ProductID=2,Name="P2",Category="cat2" },
                new Product {ProductID=3,Name="P3",Category="cat3" },
                new Product {ProductID=4,Name="P4",Category="cat2" }
            }.AsQueryable());
            ProductController con = new ProductController(mock.Object);
            con.PageSize = 3;

            Product[] result = ((ProductsListViewModel)con.List("cat2", 1).Model).Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "cat2");

        }
        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID=1,Name="P1",Category="cat1"},
                new Product { ProductID=2,Name="P2",Category="cat2"},
                 new Product { ProductID=3,Name="P3",Category="cat3"},
                  new Product { ProductID=4,Name="P4",Category="cat3"},

            }.AsQueryable());

            NavController target = new NavController(mock.Object);
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "cat1");
            Assert.AreEqual(results[1], "cat2");
            Assert.AreEqual(results[2], "cat3");
        }
        [TestMethod]
        public void Indicates_Selected_category()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]

            {
                 new Product { ProductID=1,Name="P1",Category="cat1"},
                new Product { ProductID=2,Name="P2",Category="cat2"},
                 new Product { ProductID=3,Name="P3",Category="cat3"},
                  new Product { ProductID=4,Name="P4",Category="cat3"},
            }.AsQueryable());

            NavController target = new NavController(mock.Object);

            string categorytoselect = "cat3";

            string result = target.Menu(categorytoselect).ViewBag.SelectedCategory;

            Assert.AreEqual(result, categorytoselect);
        }
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            }.AsQueryable());

            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;

            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);

        }
        
    }
}

