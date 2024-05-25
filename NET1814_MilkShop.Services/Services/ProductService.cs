using System.Linq.Expressions;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;

namespace NET1814_MilkShop.Services.Services
{
    public interface IProductService
    {
        Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel);
        Task<ResponseModel> GetUnitsAsync(UnitQueryModel request);
        Task<ResponseModel> GetUnitByIdAsync(int id);
        Task<ResponseModel> CreateUnitAsync(CreateUnitModel createUnitModel);
        Task<ResponseModel> UpdateUnitAsync(UnitModel unitModel);
        Task<ResponseModel> DeleteUnitAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepository,
            IBrandRepository brandRepository,
            ICategoryRepository categoryRepository,
            IUnitRepository unitRepository,
            IUnitOfWork unitOfWork
        )
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _unitRepository = unitRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel)
        {
            var query = _productRepository.GetProductsQuery();
            #region Filter, Search
            //thu gọn thành 1 where thôi
            query = query.Where(p =>
                p.IsActive == queryModel.IsActive
                //search theo name, description, brand, unit, category
                && (
                    string.IsNullOrEmpty(queryModel.SearchTerm)
                    || p.Name.Contains(queryModel.SearchTerm)
                    || p.Description!.Contains(queryModel.SearchTerm)
                    || p.Brand!.Name.Contains(queryModel.SearchTerm)
                    || p.Unit!.Name.Contains(queryModel.SearchTerm)
                    || p.Category!.Name.Contains(queryModel.SearchTerm)
                )
                //filter theo brand, category, unit, status, minPrice, maxPrice
                && (
                    string.IsNullOrEmpty(queryModel.Brand)
                    || string.Equals(p.Brand!.Name, queryModel.Brand)
                )
                && (
                    string.IsNullOrEmpty(queryModel.Category)
                    || string.Equals(p.Category!.Name, queryModel.Category)
                )
                && (
                    string.IsNullOrEmpty(queryModel.Unit)
                    || string.Equals(p.Unit!.Name, queryModel.Unit)
                )
                && (
                    string.IsNullOrEmpty(queryModel.Status)
                    || string.Equals(p.ProductStatus!.Name, queryModel.Status)
                )
                && (queryModel.MinPrice <= 0 || p.SalePrice >= queryModel.MinPrice)
                && (queryModel.MaxPrice <= 0 || p.SalePrice <= queryModel.MaxPrice)
            );
            /*if (!string.IsNullOrEmpty(queryModel.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(queryModel.SearchTerm)
                        || p.Description!.Contains(queryModel.SearchTerm)
                        || p.Brand!.Name.Contains(queryModel.SearchTerm)
                        || p.Unit!.Name.Contains(queryModel.SearchTerm)
                        || p.Category!.Name.Contains(queryModel.SearchTerm));
            }
            if (!string.IsNullOrEmpty(queryModel.Brand))
            {
                query = query.Where(p => string.Equals(p.Brand!.Name, queryModel.Brand));
            }
            if (!string.IsNullOrEmpty(queryModel.Category))
            {
                query = query.Where(p => string.Equals(p.Category!.Name, queryModel.Category));
            }
            if (!string.IsNullOrEmpty(queryModel.Unit))
            {
                query = query.Where(p => string.Equals(p.Unit!.Name, queryModel.Unit));
            }
            if (!string.IsNullOrEmpty(queryModel.Status))
            {
                query = query.Where(p => string.Equals(p.ProductStatus!.Name, queryModel.Status));
            }*/
            #endregion
            #region Sort
            if ("desc".Equals(queryModel.SortOrder?.ToLower()))
            {
                query = query.OrderByDescending(GetSortProperty(queryModel));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(queryModel));
            }
            #endregion
            #region Convert to ProductModel
            var productModelQuery = query.Select(p => new ProductModel
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Description = p.Description,
                Quantity = p.Quantity,
                Brand = p.Brand!.Name,
                Category = p.Category!.Name,
                Unit = p.Unit!.Name,
                OriginalPrice = p.OriginalPrice,
                SalePrice = p.SalePrice,
                Status = p.ProductStatus!.Name,
            });
            #endregion
            #region Pagination
            var products = await PagedList<ProductModel>.CreateAsync(
                productModelQuery,
                queryModel.Page,
                queryModel.PageSize
            );
            #endregion
            return new ResponseModel
            {
                Data = products,
                Message =
                    products.TotalCount > 0 ? "Get products successfully" : "No products found",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> GetUnitsAsync(UnitQueryModel request)
        {
            var query = _unitRepository.GetUnitsQuery().Where(c => c.IsActive);

            #region Filter, Search
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(u =>
                    u.Name.Contains(request.SearchTerm)
                    || u.Description!.Contains(request.SearchTerm)
                );
            }
            #endregion
            #region sort
            query = "desc".Equals(request.SortOrder?.ToLower())
                ? query.OrderByDescending(GetSortProperty(request))
                : query.OrderBy(GetSortProperty(request));
            var result = query.Select(u => new UnitModel
            {
                Id = u.Id,
                Name = u.Name,
                Description = u.Description!
            });
            #endregion
            #region page
            var units = await PagedList<UnitModel>.CreateAsync(
                result,
                request.Page,
                request.PageSize
            );

            #endregion
            return new ResponseModel
            {
                Data = units,
                Message = units.TotalCount > 0 ? "Get units successfully" : "No units found",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> GetUnitByIdAsync(int id)
        {
            var unit = await _unitRepository.GetExistIsActiveId(id);
            if (unit == null)
                return new ResponseModel { Status = "failed", Message = "Unit not found" };
            var result = new UnitModel
            {
                Id = id,
                Name = unit.Name,
                Description = unit.Description!
            };
            return new ResponseModel
            {
                Data = result,
                Status = "success",
                Message = "Get unit successfully",
            };
        }

        public async Task<ResponseModel> CreateUnitAsync(CreateUnitModel createUnitModel)
        {
            var unit = new Unit
            {
                Name = createUnitModel.Name,
                Description = createUnitModel.Description,
                IsActive = true
            };
            _unitRepository.Add(unit);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Data = createUnitModel,
                    Status = "success",
                    Message = "Create unit successfully"
                };
            }

            return new ResponseModel
            {
                Status = "Error",
                Message = "An error occured while creating unit"
            };
        }

        public async Task<ResponseModel> UpdateUnitAsync(UnitModel unitModel)
        {
            var isExistUnit = await _unitRepository.GetExistIsActiveId(unitModel.Id);
            if (isExistUnit == null)
            {
                return new ResponseModel { Status = "failed", Message = "Unit not found" };
            }

            isExistUnit.Id = unitModel.Id;
            isExistUnit.Name = unitModel.Name;
            isExistUnit.Description = unitModel.Description;
            _unitRepository.Update(isExistUnit);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel()
                {
                    Data = unitModel,
                    Status = "success",
                    Message = "Update unit successfully"
                };
            }

            return new ResponseModel()
            {
                Status = "Error",
                Message = "An error occured while updating unit"
            };
        }

        public async Task<ResponseModel> DeleteUnitAsync(int id)
        {
            var isExistUnit = await _unitRepository.GetExistIsActiveId(id);
            if (isExistUnit == null)
            {
                return new ResponseModel { Status = "failed", Message = "Unit not found" };
            }

            isExistUnit.IsActive = false;
            isExistUnit.DeletedAt = DateTime.Now;
            _unitRepository.Update(isExistUnit);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Status = "success",
                    Message = "Delete unit successfully"
                };
            }

            return new ResponseModel
            {
                Status = "Error",
                Message = "An error occured while deleting unit"
            };
        }

        /// <summary>
        /// Get sort property as expression
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private static Expression<Func<Product, object>> GetSortProperty(
            ProductQueryModel queryModel
        ) =>
            queryModel.SortColumn?.ToLower() switch
            {
                "name" => product => product.Name,
                "saleprice" => product => product.SalePrice,
                "quantity" => product => product.Quantity,
                _ => product => product.Id,
            };
        
        /// <summary>
        /// Sort property for unit (name, description)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static Expression<Func<Unit, object>> GetSortProperty(UnitQueryModel request)
        {
            return request.SortColumn?.ToLower() switch
            {
                "name" => unit => unit.Name,
                "description" => unit => unit.Description!,
                _ => unit => unit.Id
            };
        }
    }
}
