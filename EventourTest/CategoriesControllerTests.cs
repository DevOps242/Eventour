﻿using System;
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
        var result = (ViewResult)categoryController.Create(model).Result;

        // Assert
        //Assert.IsTrue(categoryController.ModelState["Name"].Errors.Count > 0);
        Assert.AreEqual(categoryController.ModelState["Name"].Errors.ToString, result.ToString);
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
    #endregion

    //    //Use controller.ModelState.AddModelError("put a descriptive key name here", "add an appropriate key value here");
    //    //to test any parts of methods that handle errors in the model validation(the CREATE and EDIT POST methods).

    #region "Edit Tests"

    [TestMethod]
    public void GetEditNoIdLoads404()
    {
        // Act
        var result = (ViewResult)categoryController.Edit(null).Result;

        // Assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void GetEditNoCategoryTableLoads404()
    {
        context.Categories = null;
        // Act
        var result = (ViewResult)categoryController.Edit(null).Result;

        // Assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void GetEditInvalidIdLoads404()
    {
        // Act
        var result = (ViewResult)categoryController.Edit(500).Result;

        // Assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void GetEditIsValidLoadsView()
    {
        // Act
        var result = (ViewResult)categoryController.Edit(1001).Result;

        // Assert
        Assert.AreEqual("Edit", result.ViewName);
    }

    [TestMethod]
    public void GetEditIsValidLoadsCategory()
    {
        // Act
        var result = (ViewResult)categoryController.Edit(1001).Result;

        // Assert
        Assert.AreEqual(context.Categories.Find(1001), result.Model);
    }

    [TestMethod]
    public void PostEditInvalidIDReturn404()
    {
        var model = context.Categories.Find(1003);

        // Act
        var result = (ViewResult)categoryController.Edit(1004, model).Result;

        // Assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void PostEditNotFoundIDReturn404()
    {
        // Act
        var result = (ViewResult)categoryController.Edit(100000).Result;

        // Assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void PostEditCategoryValidReturnsIndex()
    {
        var model = context.Categories.Find(1003);

        model.Name = "Editing Name";
        model.Description = "Editing Description";
        model.Color = "Editing Color";

        // Act
        var result = (RedirectToActionResult)categoryController.Edit(1003, model).Result;

        // Assert
        Assert.AreEqual("Index", result.ActionName);
    }

    [TestMethod]
    public void PostEditCategoryInvalidReturnsEditView()
    {
        var model = context.Categories.Find(1003);

        model.Name = null;
        model.Description = "Editing Description";
        model.Color = "Editing Color";

        categoryController.ModelState.AddModelError("Form", "Invalid Form");

        // Act
        var result = (ViewResult)categoryController.Edit(1003, model).Result;

        // Assert
        Assert.AreEqual("Edit", result.ViewName);
    }

    //// Come back here.

    [TestMethod]
    public void PostEditCategoryNullNameReturnsError()
    {
        // Arrange
        var model = context.Categories.Find(1004);
        model.Name = null;
        categoryController.ModelState.AddModelError("Name", "Name can't be empty");

        // Act
        var result = (ViewResult)categoryController.Edit(1004, model).Result;

        // Assert
        //Assert.IsTrue(categoryController.ModelState["Name"].Errors.Count > 0);
        Assert.AreEqual(categoryController.ModelState["Name"].Errors.ToString, result.ToString);
    }


    [TestMethod]
    public void PostEditCategoryNullDescriptionReturnsError()
    {
        // Arrange
        var model = context.Categories.Find(1004);
        model.Description = null;
        categoryController.ModelState.AddModelError("Description", "Description can't be empty");

        // Act
        var result = (ViewResult)categoryController.Edit(1004, model).Result;

        // Assert
        //Assert.IsTrue(categoryController.ModelState["Description"].Errors.Count > 0);
        Assert.AreEqual(categoryController.ModelState["Description"].Errors.ToString, "Description can't be empty");
    }
    #endregion


    #region "Delete Tests"

    //    // GET: Categories/Delete/5
    //    public async Task<IActionResult> Delete(int? id)
    //    {
    //        if (id == null || _context.Categories == null)
    //        {
    //            return NotFound();
    //        }

    //        var category = await _context.Categories
    //            .FirstOrDefaultAsync(m => m.CategoryId == id);
    //        if (category == null)
    //        {
    //            return NotFound();
    //        }

    //        return View(category);
    //    }

    //    // POST: Categories/Delete/5
    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> DeleteConfirmed(int id)
    //    {
    //        if (_context.Categories == null)
    //        {
    //            return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
    //        }
    //        var category = await _context.Categories.FindAsync(id);
    //        if (category != null)
    //        {
    //            _context.Categories.Remove(category);
    //        }

    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }

    //    private bool CategoryExists(int id)
    //    {
    //        return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
    //    }
    //}
    #endregion





}
