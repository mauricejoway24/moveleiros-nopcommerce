using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Validators.Common;
using System.Web.Mvc;

namespace Nop.Web.Models.Chat
{
    [Validator(typeof(ChatContactUsValidator))]
    public class ChatContactUsModel
    {
        [AllowHtml]
        [NopResourceDisplayName("ContactUs.Email")]
        public string Email { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ContactUs.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Moveleiros.ContactUs.Question")]
        public string Question { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("ContactUs.FullName")]
        public string FullName { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Moveleiros.ContactUs.PhoneNumber")]
        public string PhoneNumber { get; set; }

        public int ProductId { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}