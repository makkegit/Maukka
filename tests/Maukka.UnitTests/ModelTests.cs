using Maukka.Models;

namespace Maukka.UnitTests;

public class ModelTests
{
    [Fact]
    public void BrandId_Init_Test()
    {
        var newBrand = new Brand(1, "Makke");
        BrandId newBrandId = 1;
        
        Assert.Equal(1, newBrandId);
        Assert.Equal(newBrand.BrandId, newBrandId);
    }
    
    [Fact]
    public void Brand_Init_Test()
    {
        var newBrand = new Brand(1, "Makke");
        
        Assert.Equal(new BrandId(1), newBrand.BrandId);
        Assert.Equal(1, newBrand.BrandId.Value);
        Assert.Equal("Makke", newBrand.BrandName);
    }
    
    [Fact]
    public void BrandClothing_Creation_Test()
    {
        var clothing = new BrandClothing
        {
            Id = 1,
            BrandId = 100,
            ClothingName = "Test Shirt",
            Category = ClothingCategory.Tops,
            ClothingSizes = new List<ClothingSize>()
        };

        Assert.Equal(1, clothing.Id);
        Assert.Equal(100, clothing.BrandId);
        Assert.Equal("Test Shirt", clothing.ClothingName);
        Assert.Equal(ClothingCategory.Tops, clothing.Category);
        Assert.NotNull(clothing.ClothingSizes);
    }
    
    [Fact]
    public void Shirt_Creation_Test()
    {
        var testSize = new TopsSize(CountryCode.EU, 86f, 54f, 86f, "1 year");

        var clothing = new Shirt()
        {
            Id = 1,
            BrandClothingId = 200,
            ClothingName = "Test Shirt",
            Alias = "Winter Shirt",
            Size = testSize
        };

        Assert.Equal(1, clothing.Id.Value);
        Assert.Equal(200, clothing.BrandClothingId);
        Assert.Equal("Test Shirt", clothing.ClothingName);
        Assert.Equal("Winter Shirt", clothing.Alias);
        Assert.Equal(ClothingCategory.Tops, clothing.ClothingCategory);
        Assert.Equal(testSize, clothing.Size);
    }
    
    [Fact]
    public void TopsSize_Creation_Test()
    {
        var size = new TopsSize(CountryCode.EU, 86, 52, 86, "1 - 1.5 years");
        
        Assert.NotEqual(CountryCode.US, size.CountryCode);
        Assert.Equal(ClothingCategory.Tops, size.Category);
    }
    
    [Fact]
    public void Pants_Creation_Test()
    {
        var testSize = new BottomsSize(CountryCode.EU, 50f, 55f, 36.5f, "1 - 1.5 years");

        var clothing = new Pants()
        {
            Id = 1,
            BrandClothingId = 200,
            ClothingName = "Test Pants",
            Alias = "Winter Pants",
            Size = testSize
        };

        Assert.Equal(1, clothing.Id.Value);
        Assert.Equal(200, clothing.BrandClothingId);
        Assert.Equal("Test Pants", clothing.ClothingName);
        Assert.Equal("Winter Pants", clothing.Alias);
        Assert.Equal(ClothingCategory.Bottoms, clothing.ClothingCategory);
        Assert.Equal(testSize, clothing.Size);
    }
    
    [Fact]
    public void BottomsSize_Creation_Test()
    {
        var size = new BottomsSize(CountryCode.EU, 50f, 55f, 36.5f, "1 - 1.5 years");
        
        Assert.Equal(CountryCode.EU, size.CountryCode);
        Assert.Equal(ClothingCategory.Bottoms, size.Category);
    }
    
    [Fact]
    public void Wardrobe_Add_Clothing_Test()
    {
        var wardrobe = new Wardrobe
        {
            Id = 1,
            Description = "Main Wardrobe"
        };

        var clothing = new Shirt()
        {
            Id = 10,
            BrandClothingId = 300,
            ClothingName = "Test Sweater"
        };

        wardrobe.Clothes.Add(clothing);

        Assert.Single(wardrobe.Clothes);
        Assert.Equal(clothing, wardrobe.Clothes[0]);
    }
    
    [Fact]
    public void WardrobesJson_Creation_Test()
    {
        var wardrobe = new Wardrobe { Id = 1, Description = "Test Wardrobe" };
        var wardrobesJson = new WardrobesJson { Wardrobes = new List<Wardrobe> { wardrobe } };

        Assert.NotNull(wardrobesJson.Wardrobes);
        Assert.Single(wardrobesJson.Wardrobes);
        Assert.Equal(wardrobe, wardrobesJson.Wardrobes[0]);
    }
}