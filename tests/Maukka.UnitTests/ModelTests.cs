using Maukka.Models;

namespace Maukka.UnitTests;

public class ModelTests
{
    private class TestClothing : Clothing
    {
        public override BrandClothingId BrandClothingId { get; set; }
        public override string ClothingName { get; set; }
        public override ClothingSize Size { get; set; }
    }
    
    private class TestClothingSize : ClothingSize
    {
        public override ClotingCategory Category { get; set; }
    }
    
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
            Category = ClotingCategory.Shirt,
            ClothingSizes = new List<ClothingSize>()
        };

        Assert.Equal(1, clothing.Id);
        Assert.Equal(100, clothing.BrandId);
        Assert.Equal("Test Shirt", clothing.ClothingName);
        Assert.Equal(ClotingCategory.Shirt, clothing.Category);
        Assert.NotNull(clothing.ClothingSizes);
    }
    
    [Fact]
    public void Clothing_Creation_Test()
    {
        var testSize = new TestClothingSize
        {
            CountryCode = CountryCode.EU,
            Category = ClotingCategory.Shirt
        };

        var clothing = new TestClothing
        {
            Id = 1,
            BrandClothingId = 200,
            ClothingName = "Test Jacket",
            Alias = "Winter Jacket",
            Size = testSize
        };

        Assert.Equal(1, clothing.Id.Value);
        Assert.Equal(200, clothing.BrandClothingId);
        Assert.Equal("Test Jacket", clothing.ClothingName);
        Assert.Equal("Winter Jacket", clothing.Alias);
        Assert.Equal(testSize, clothing.Size);
    }
    
    [Fact]
    public void ClothingSize_Creation_Test()
    {
        var size = new TestClothingSize
        {
            CountryCode = CountryCode.US,
            Category = ClotingCategory.Pants
        };

        Assert.Equal(CountryCode.US, size.CountryCode);
        Assert.Equal(ClotingCategory.Pants, size.Category);
    }
    
    [Fact]
    public void Wardrobe_Add_Clothing_Test()
    {
        var wardrobe = new Wardrobe
        {
            Id = 1,
            Description = "Main Wardrobe"
        };

        var clothing = new TestClothing
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