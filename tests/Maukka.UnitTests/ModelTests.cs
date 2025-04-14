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
            BrandClothingId = 11,
            Brand = new Brand(1, "Makke"),
            Name = "Test Shirt",
            Category = ClothingCategory.Tops,
            ClothingSizes = new List<ClothingSize>()
        };

        Assert.Equal(11, clothing.BrandClothingId);
        Assert.Equal(1, clothing.Brand.BrandId);
        Assert.Equal("Test Shirt", clothing.Name);
        Assert.Equal(ClothingCategory.Tops, clothing.Category);
        Assert.NotNull(clothing.ClothingSizes);
    }
    
    [Fact]
    public void Shirt_Creation_Test()
    {
        var testSize = ClothingSize.CreateTopsSize(CountryCode.EU, MeasurementUnit.Cm, "86",
            12, 12+6, 86f, 55f, 1.5f);
        var brandClothing = new BrandClothing()
        {
            BrandClothingId = 11,
            Brand = new Brand(1, "MakkeBrand"),
            Category = ClothingCategory.Tops,
            Name = "Test Shirt",
            ClothingSizes = new List<ClothingSize>()
            {
                testSize
            }
        };
            
        var clothing = Clothing.InitClothing(1, brandClothing, testSize, "Makee paita");

        
        Assert.Equal(1, clothing.ClothingId);
        Assert.Equal(11, clothing.BrandClothingId);
        Assert.Equal("Test Shirt", clothing.ClothingName);
        Assert.Equal("Makee paita", clothing.Alias);
        Assert.Equal(ClothingCategory.Tops, clothing.Category);
        Assert.Equal(testSize, clothing.Size);
    }
    
    [Fact]
    public void TopsSize_Creation_Test()
    {
        var testSize = ClothingSize.CreateTopsSize(CountryCode.EU, MeasurementUnit.Cm, "86",
            12, 12+6, 86f, 55f, 1.5f);
        
        Assert.NotEqual(CountryCode.US, testSize.CountryCode);
        Assert.Equal(ClothingCategory.Tops, testSize.Category);
        Assert.True(testSize.Measurements.ContainsKey(ClothingSize.ChestKey));
        Assert.True(testSize.Measurements.ContainsKey(ClothingSize.HeightKey));
    }
    
    [Fact]
    public void Pants_Creation_Test()
    {
        var testSize = ClothingSize.CreateBottomsSize(CountryCode.EU,
            MeasurementUnit.Cm,
            "86",
            12,
            12 + 6,
            50f,
            55f,
            36.5f);
        
        var brandClothing = new BrandClothing()
        {
            BrandClothingId = 12,
            Brand = new Brand(1, "MakkeBrand"),
            Category = ClothingCategory.Bottoms,
            Name = "Test Pants",
            ClothingSizes = new List<ClothingSize>()
            {
                testSize
            }
        };
        
        var clothing = Clothing.InitClothing(12, brandClothing, testSize, "Makeet housut");
        
        Assert.Equal(12, clothing.ClothingId);
        Assert.Equal("Test Pants", clothing.ClothingName);
        Assert.Equal("Makeet housut", clothing.Alias);
        Assert.Equal(ClothingCategory.Bottoms, clothing.Category);
        Assert.Equal(testSize, clothing.Size);
    }
    
    [Fact]
    public void BottomsSize_Creation_Test()
    {
        var testSize = ClothingSize.CreateBottomsSize(CountryCode.EU,
            MeasurementUnit.Cm,
            "86",
            12,
            12 + 6,
            50f,
            55f,
            36.5f);
        
        Assert.Equal(CountryCode.EU, testSize.CountryCode);
        Assert.Equal(ClothingCategory.Bottoms, testSize.Category);
        Assert.True(testSize.Measurements.ContainsKey(ClothingSize.WaistKey));
        Assert.True(testSize.Measurements.ContainsKey(ClothingSize.HipsKey));
        Assert.True(testSize.Measurements.ContainsKey(ClothingSize.InsideLegLengthKey));
    }
    
    [Fact]
    public void Wardrobe_Add_Clothing_Test()
    {
        var wardrobe = new Wardrobe
        {
            WardrobeId = 1,
            Description = "Main Wardrobe"
        };

        var testSize = ClothingSize.CreateTopsSize(CountryCode.EU, MeasurementUnit.Cm, "86",
            12, 12+6, 86f, 55f, 1.5f);
        
        var brandClothing = new BrandClothing()
        {
            BrandClothingId = 11,
            Brand = new Brand(1, "MakkeBrand"),
            Category = ClothingCategory.Tops,
            Name = "Test Shirt",
            ClothingSizes = new List<ClothingSize>()
            {
                testSize
            }
        };
            
        var clothing = Clothing.InitClothing(1, brandClothing, testSize, "Makee paita");

        wardrobe.Items.Add(clothing);

        Assert.Single(wardrobe.Items);
        Assert.Equal(clothing, wardrobe.Items[0]);
    }
    
    [Fact]
    public void WardrobesJson_Creation_Test()
    {
        var wardrobe = new Wardrobe { WardrobeId = 1, Description = "Test Wardrobe" };
        var wardrobesJson = new WardrobesJson { Wardrobes = new List<Wardrobe> { wardrobe } };

        Assert.NotNull(wardrobesJson.Wardrobes);
        Assert.Single(wardrobesJson.Wardrobes);
        Assert.Equal(wardrobe, wardrobesJson.Wardrobes[0]);
    }
}