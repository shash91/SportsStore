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
            ProductsListViewModel result = (ProductsListViewModel)con.List(2).Model;

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
            ProductsListViewModel result = (ProductsListViewModel)con.List(2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.Totalpages, 2);
        }
    }
}

