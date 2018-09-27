﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Stores
{
    [Validator(typeof(StoreValidator))]
    public partial class StoreModel : BaseNopEntityModel, ILocalizedModel<StoreLocalizedModel>
    {
        public StoreModel()
        {
            Locales = new List<StoreLocalizedModel>();
            AvailableLanguages = new List<SelectListItem>();
            AvailableCities = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.Url")]
        [AllowHtml]
        public string Url { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.SslEnabled")]
        public virtual bool SslEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.SecureUrl")]
        [AllowHtml]
        public virtual string SecureUrl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.Hosts")]
        [AllowHtml]
        public string Hosts { get; set; }

        //default language
        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultLanguage")]
        [AllowHtml]
        public int DefaultLanguageId { get; set; }
        public IList<SelectListItem> AvailableLanguages { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyName")]
        [AllowHtml]
        public string CompanyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyAddress")]
        [AllowHtml]
        public string CompanyAddress { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyPhoneNumber")]
        [AllowHtml]
        public string CompanyPhoneNumber { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyVat")]
        [AllowHtml]
        public string CompanyVat { get; set; }


        [NopResourceDisplayName("Moveleiros.Admin.Configuration.Stores.Fields.ChatScript")]
        [AllowHtml]
        public string ChatScript { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Configuration.Stores.Fields.LojistaId")]
        public int LojistaId { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Configuration.Stores.Fields.About")]
        public string About { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Configuration.Stores.Fields.CityId")]
        public int CityId { get; set; }
        public IList<SelectListItem> AvailableCities { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Configuration.Stores.Fields.ProfileShortUrl")]
        public string ProfileShortUrl { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Configuration.Stores.Fields.CompanyNeighborhood")]
        public string CompanyNeighborhood { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Configuration.Stores.Fields.CNPJ")]
        public string CNPJ { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Moveleiros.Admin.Configuration.Stores.Fields.LogoPictureId")]
        public int LogoPictureId { get; set; }

        public IList<StoreLocalizedModel> Locales { get; set; }
    }

    public partial class StoreLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}