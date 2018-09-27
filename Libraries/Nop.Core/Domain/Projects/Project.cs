using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using System.Collections.Generic;

namespace Nop.Core.Domain.Projects
{
    public class Project : BaseEntity
    {
        private ICollection<ProjectPicture> _pictures;
        private ICollection<ProjectCategory> _categories;

        // Product Fields:
        // ProductTypeId = 1;
        // ParentGroupedProductId = 0
        // VisibleIndividually = 1
        // ProductTemplateId = 1
        // VendorId = Current Vendor Logged which selected this project
        // ShowOnHomePage = 1
        // MetaKeywords = NULL
        // MetaDescription = NULL
        // MetaTitle = NULL
        // AllowCustomerReviews = 1
        // ApprovedRatingSum = 0
        // NotApprovedRatingSum = 0
        // ApprovedTotalReviews = 0
        // NotApprovedTotalReviews = 0
        // SubjectToAcl = 0
        // LimitedToStores = 1
        // SubjectToAcl = 0
        // Sku = NULL
        // ManufacturerPartNumber = NULL
        // Gtin = NULL
        // IsGiftCard = 0
        // GiftCardTypeId = 0
        // OverriddenGiftCardAmount = NULL
        // RequireOtherProducts = 0
        // RequiredProductIds = NULL
        // AutomaticallyAddRequiredProducts = 0
        // IsDownload = 0
        // DownloadId = 0
        // UnlimitedDownloads = 1
        // MaxNumberOfDownloads = 10
        // DownloadExpirationDays = NULL
        // DownloadActivationTypeId = 0
        // HasSampleDownload = 0
        // SampleDownloadId = 0
        // HasUserAgreement = 0
        // UserAgreementText = NULL
        // IsRecurring = 0
        // RecurringCycleLength = 100
        // RecurringCyclePeriodId = 0
        // RecurringTotalCycles = 10
        // IsRental = 0
        // RentalPriceLength = 1
        // RentalPricePeriodId = 0
        // IsShipEnabled = 1
        // IsFreeShipping = 0
        // ShipSeparately = 0
        // AdditionalShipCharge = 0.0000
        // DeliveryDateId = 0
        // IsTaxExempt = 0
        // TaxCategoryId = 0
        // IsTelecommunicationsOrBroadcastingOrElectronicServices = 0
        // ManageInventoryMethodId = 0
        // ProductAvailabilityRangeId = 0
        // UseMultipleWarehouses = 0
        // WarehouseId = 0
        // StockQuantity = 10000
        // DisplayStockAvailability = 0
        // DisplayStockQuantity = 0
        // MinStockQuantity = 0
        // LowStockActivityId = 0
        // NotifyAdminForQuantityBelow = 0
        // BackorderModeId = 0
        // AllowBackInStockSubscriptions = 0
        // OrderMinimumQuantity = 1
        // OrderMaximumQuantity = 10000
        // AllowedQuantities = NULL
        // AllowAddingOnlyExistingAttributeCombinations = 0
        // NotReturnable = 0
        // DisableBuyButton = 0
        // DisableWishlistButton = 0
        // AvailableForPreOrder = 0
        // PreOrderAvailabilityStartDateTimeUtc = NULL
        // CallForPrice = 0
        // Price = Current Vendor Logged will choose the price
        // OldPrice = 0.0000
        // ProductCost = 0.0000
        // CustomerEntersPrice = 0
        // MinimumCustomerEnteredPrice = 0.0000
        // MaximumCustomerEnteredPrice = 1000.0000
        // BasepriceEnabled = 0
        // BasepriceAmount = 0.0000
        // BasepriceUnitId = 1
        // BasepriceBaseAmount = 0.0000
        // BasepriceBaseUnitId = 1
        // MarkAsNew = 1
        // MarkAsNewStartDateTimeUtc = NULL
        // MarkAsNewEndDateTimeUtc = NULL
        // HasTierPrices = 0
        // HasDiscountsApplied = 0
        // Weight = 0.0000
        // Length = 0.0000
        // Width = 0.0000
        // Height = 0.0000
        // AvailableStartDateTimeUtc = NULL
        // AvailableEndDateTimeUtc = NULL
        // DisplayOrder = 0
        // Published = Current Vendor Logged will choose if this project will be published
        // Deleted = 0
        // CreatedOnUtc = GETDATE()
        // UpdatedOnUtc = GETDATE()
        // BrandId = Current Vendor Logged will choose the brand for the product

        public string PromobFile { get; set; }
        public int DesignerId { get; set; }
        public int BrandId { get; set; }
        public int Clicks { get; set; }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string AdminComment { get; set; }
        public string Description { get; set; }
        public string ProjectContains { get; set; }
        public string AsideObservation { get; set; }
        public string DimensionDescription { get; set; }
        public int FromArea { get; set; }
        public int ToArea { get; set; }
        public int Area { get; set; }
        public bool Active { get; set; }

        public Customer Designer { get; set; }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<ProjectPicture> Pictures
        {
            get { return _pictures ?? (_pictures = new List<ProjectPicture>()); }
            set { _pictures = value; }
        }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<ProjectCategory> Categories
        {
            get { return _categories ?? (_categories = new List<ProjectCategory>()); }
            protected set { _categories = value; }
        }

        public virtual Brand Brand { get; set; }
    }
}
