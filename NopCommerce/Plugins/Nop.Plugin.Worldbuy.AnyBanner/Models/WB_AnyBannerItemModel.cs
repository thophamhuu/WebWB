using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Plugin.Worldbuy.AnyBanner.Domain;
using Nop.Services.Media;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Worldbuy.AnyBanner.Models
{
    public partial class WB_AnyBannerItemModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Banner")]
        public int BannerID { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Title")]
        public string Title { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Alt")]
        public string Alt { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Url")]
        public string Url { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.IsActived")]
        public bool IsActived { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Order")]
        public int Order { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Image")]
        [UIHint("Picture")]
        public int PictureId { get; set; }
        public string FullImageUrl { get; set; }
        public string ImageUrl { get; set; }
    }
    public static class WB_AnyBannerExtension
    {
        public static WB_AnyBannerItemModel ToModel(this WB_AnyBannerItem entity)
        {
            var _pictureService = EngineContext.Current.Resolve<IPictureService>();
            var picture = _pictureService.GetPictureById(entity.PictureId);
            return new WB_AnyBannerItemModel
            {
                Alt = entity.Alt,
                BannerID = entity.BannerID,
                Id = entity.Id,
                IsActived = entity.IsActived,
                ImageUrl = picture != null ? _pictureService.GetThumbLocalPath(picture, 100) : _pictureService.GetDefaultPictureUrl(),
                FullImageUrl = picture != null ? _pictureService.GetThumbLocalPath(picture, 0) : _pictureService.GetDefaultPictureUrl(),
                PictureId = entity.PictureId,
                Order = entity.Order,
                Title = entity.Title,
                Url = entity.Url
            };
        }
    }
}
