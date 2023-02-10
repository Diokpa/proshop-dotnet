using System.Security.Principal;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  
  public class ProductsController : BaseApiController
  {

    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<ProductBrand> _productBrandRepo;
    private readonly IGenericRepository<ProductType> _productType;
    private readonly IMapper _mapper;

    public ProductsController(IGenericRepository<Product> productRepo, IGenericRepository<ProductBrand> productBrandRepo, IGenericRepository<ProductType> productType, IMapper mapper)
    {
      _productType = productType;
      _productBrandRepo = productBrandRepo;
      _productRepo = productRepo;
      _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
    {
      var spec = new ProductsWithTypesAndBrandsSpecification();

      var products = await _productRepo.ListAsync(spec);

      return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
      var spec = new ProductsWithTypesAndBrandsSpecification(id);

      var product = await _productRepo.GetEntityWithSpec(spec);

      if(product == null) return NotFound(new ApiResponse(404));

      return _mapper.Map<ProductToReturnDto>(product);
      //return await _context.Products.SingleOrDefaultAsync(x => x.Id == id);
      //return await _context.Products.FindAsync(id);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
      var productBrands = await _productBrandRepo.ListAllAsync();
      return Ok(productBrands);
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
    {
      var productTypes = await _productType.ListAllAsync();
      return Ok(productTypes);
    }


  }
}
