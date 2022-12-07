using System;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using COMP2084_Project_Eventour.Controllers;
using COMP2084_Project_Eventour.Data;
using COMP2084_Project_Eventour.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace EventourTest;

[TestClass]
public class CategoriesControllerTests
{
    private ApplicationDbContext context;
    CategoriesController categoryController;
    private RedirectToActionResult result;


    #region "InMemory Database Initialize"
    [TestInitialize]
    public void TestInitalize()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new ApplicationDbContext(options);

        //var eventDetail = new EventDetail { }

        // Seed the db.
        for (int i = 0; i < 5; i++)
        {

            // Create the categories below.
            var category = new Category { CategoryId = i + 1000, Name = "Category - " + i + 1000.ToString(), Color = "#", Description = "Category - " + i + 1000.ToString() + " Description" };

            // Add the categories to the context
            context.Add(category);
        }
        
        //Save the context
        context.SaveChanges();
        categoryController = new CategoriesController(context);
    }
    #endregion

    #region "Index"
    [TestMethod]
    public void IndexLoadsView()
    {
        // Act
        var result = (ViewResult)categoryController.Index().Result;

        // Assert
        Assert.AreEqual("Index", result.ViewName);

    }

    [TestMethod]
    public void IndexLoadsCategories()
    {
        // Act
        var result = (ViewResult)categoryController.Index().Result;
        List<Category> model = (List<Category>)result.Model;

        // Assert
        CollectionAssert.AreEqual(context.Categories.ToList(), model);
    }
    #endregion

    #region "Details Tests"

    [TestMethod]
    public void DetailsNoIdLoads404()
    {
        // Act
        var result = (ViewResult)categoryController.Details(null).Result;

        // Assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void DetailsNoCategoryTableLoads404()
    {
        context.Categories = null;
        // Act
        var result = (ViewResult)categoryController.Details(null).Result;

        // Assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void DetailsInvalidIdLoads404()
    {
        // Act
        var result = (ViewResult)categoryController.Details(500).Result;

        // Assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void DetailsIsValidLoadsView()
    {
        // Act
        var result = (ViewResult)categoryController.Details(1001).Result;

        // Arrange
        Assert.AreEqual("Details", result.ViewName);
    }

    [TestMethod]
    public void DetailsIsValidLoadsCategory()
    {
        // Act
        var result = (ViewResult)categoryController.Details(1001).Result;

        // Arrange
        Assert.AreEqual(context.Categories.Find(1001), result.Model);
    }
    #endregion


    #region "Create Tests"

    [TestMethod]
    public void GetCreateLoadsView()
    {
        // Act
        var result = (ViewResult)categoryController.Create();

        // Assert
        Assert.AreEqual("Create", result.ViewName);
    }

    [TestMethod]
    public void PostCreateCategoryValidReturnsIndex()
    {
        var model = new Category { CategoryId = 1010, Name = "Category 1010",  Color = "#", Description = "Category - 1010 Description" };
        
        // Act
        var result = (RedirectToActionResult)categoryController.Create(model).Result;

        // Assert
        Assert.AreEqual("Index", result.ActionName);
    }

    [TestMethod]
    public void PostCreateCategoryInvalidReturnsCreateView()
    {
        var model = new Category { CategoryId = 1010, Name = "Category - 1010", Color = "#" };
        categoryController.ModelState.AddModelError("Form", "Invalid Form");

        // Act
        var result = (ViewResult)categoryController.Create(model).Result;

        // Assert
        Assert.AreEqual("Create", result.ViewName);
    }

    // Come back here.

    [TestMethod]
    public void PostCreateCategoryNullNameReturnsError()
    {
        // Arrange
        var model = new Category { CategoryId = 1010, Color = "#", Description = "Category - 1010 Description" };
        categoryController.ModelState.AddModelError("Name", "Name can't be empty");

        // Act
        var result = categoryController.Create(model).Result;

        // Assert
        Assert.IsTrue(categoryController.ModelState["Name"].Errors.Count > 0);
    }


    [TestMethod]
    public void PostCreateCategoryNullDescriptionReturnsError()
    {
        // Arrange
        var model = new Category { CategoryId = 1010, Name = "Category - 1010", Color = "#" };
        categoryController.ModelState.AddModelError("Description", "Description can't be empty");

        // Act
        var result = (ViewResult)categoryController.Create(model).Result;

        // Assert
        //Assert.IsTrue(categoryController.ModelState["Description"].Errors.Count > 0);
        Assert.AreEqual(categoryController.ModelState["Description"].Errors.ToString, result.ExecuteResultAsync);
    }

    

    //    //Use controller.ModelState.AddModelError("put a descriptive key name here", "add an appropriate key value here");
    //    //to test any parts of methods that handle errors in the model validation(the CREATE and EDIT POST methods).
    #endregion
}
