using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel sale, string userId)
        {
            // TODO: Make this SOLID/DRY/Better
            // Start in filling in the sale detail models we will save to the database
            List<SaleDetailDBModel> saleDetails = new List<SaleDetailDBModel>();

            ProductData productData = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in sale.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                // Get the information about this product
                var productInfo = productData.GetProductById(detail.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found it the database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                saleDetails.Add(detail);
            }

            // Create the sale model
            SaleDBModel saleToSave = new SaleDBModel
            {
                SubTotal = saleDetails.Sum(x => x.PurchasePrice),
                Tax = saleDetails.Sum(x => x.Tax),
                CachierId = userId
                // Sale date is automaticaly added in db
            };

            saleToSave.Total = saleToSave.SubTotal + saleToSave.Tax;

            #region Old - Replaced with transaction
            //// Save sale model
            //SqlDataAccess sql = new SqlDataAccess();

            //sql.SaveData<SaleDBModel>("dbo.spSaleInsert", saleToSave, "TRMData");

            ////Get the ID from sale model // i dont want to have id in model because of identity increment
            //var saleToSave_Id = sql.LoadData<int, dynamic>("spSaleLookup", new { CachierId = saleToSave.CachierId, SaleDate = saleToSave.SaleDate }, "TRMData").FirstOrDefault();

            //// Finish filling in the sale detail models
            //foreach (var item in saleDetails)
            //{
            //    item.SaleId = saleToSave_Id;

            //    // Save the sale detail model
            //    sql.SaveData("dbo.spSaleDetailInsert", item, "TRMData");
            //}
            #endregion

            using (SqlDataAccess sql = new SqlDataAccess()) // ok
            {
                try
                {
                    sql.StartTransaction("TRMData");

                    // Save the sale model
                    sql.SaveDataInTransaction("dbo.spSaleInsert", saleToSave);

                    //Get the ID from sale model
                    var saleToSave_Id = sql.LoadDataInTransaction<int, dynamic>("spSaleLookup", new { CachierId = saleToSave.CachierId, SaleDate = saleToSave.SaleDate }).FirstOrDefault();

                    // Finish filling in the sale detail models
                    foreach (var item in saleDetails)
                    {
                        item.SaleId = saleToSave_Id;

                        // Save the sale detail model
                        sql.SaveDataInTransaction("dbo.spSaleDetailInsert", item);
                    }

                    sql.CommitTransaction();
                }
                catch (Exception exc)
                {
                    sql.RollbackTransaction();
                    throw exc;
                }
            }
        }
    }
}

