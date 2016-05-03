using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System.Linq;
using Moq;
using System.Web.Mvc;


namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1"};
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            CartLine[] results = target.Lines.ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product.Name, "P1");
            Assert.AreEqual(results[1].Product.Name, "P2");
            
        }
        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);

            CartLine[] results = target.Lines.ToArray();

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product.Name, "P1");
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Product.Name, "P2");
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Item()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 2);
            target.AddItem(p3, 1);
            target.RemoveLine(p2);

            CartLine[] result = target.Lines.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
        }
        [TestMethod]
        public void Calculate_Cart_Total()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart target = new Cart();

            target.AddItem(p1, 2);
            target.AddItem(p2, 1);
            decimal result = target.ComputeTotalValue();
            Assert.AreEqual(result, 250M);
            
        }
        [TestMethod]
        public void Can_Clear_Content()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();
            target.AddItem(p1, 2);
            target.AddItem(p2, 1);
            target.Clear();

            CartLine[] result = target.Lines.ToArray();
            Assert.AreEqual(result.Length, 0);
        }
        [TestMethod]
        public void Can_add_To_Cart()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="P1",Category="apples" },
            }.AsQueryable());

            Cart cart = new Cart();
            CartController target = new CartController(mock.Object);

            target.AddToCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);

            
        }
        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1,Name="P1",Category="apples" },
            }.AsQueryable());

            Cart cart = new Cart();
            CartController target = new CartController(mock.Object);

            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            Cart cart = new Cart();

            CartController target = new CartController(null);

            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
            
        }

    }
}
