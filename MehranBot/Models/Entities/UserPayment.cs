using MehranBot.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MehranBot.Models.Entities
{
    public class UserPayment : BaseEntity<long>
    {

        [Display(Name = "کاربر خریدار")]
        public long FkUserId { get; set; }


        [Display(Name = "آگهی")]
        public long FkAdsId { get; set; }


        [Display(Name = "کد رهگیری ")]
        public string RefId { get; set; }


        [Display(Name = "مبلغ پرداختی ")]
        public long Amount { get; set; }


        [Display(Name = "شماره کارت ")]
        public string CardNumber { get; set; }

        public int Status { get; set; }


        [Display(Name = "وضیعت پرداخت ")]
        public bool IsPayed { get; set; }



        [Display(Name = "توضیحات ")]
        public string Description { get; set; }


        public string Code { get; set; }




        // Foreign Key

        [ForeignKey(nameof(FkUserId))]
        public User User { get; set; }


        [ForeignKey(nameof(FkAdsId))]
        public Ads Ads { get; set; }


    }
}
