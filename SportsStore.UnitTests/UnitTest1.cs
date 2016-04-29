using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.Domain.Entities;
using System.Linq;
using System.Collections.Generic;
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
            IEnumerable<Product> result = (IEnumerable<Product>)con.List(2).Model;

            //Assert
            Product[] arr = result.ToArray();
            Assert.IsTrue(arr.Length == 2);
            Assert.AreEqual(arr[0].Name , "P4");
            Assert.AreEqual(arr[1].Name, "P5");
            }
        }
 }

