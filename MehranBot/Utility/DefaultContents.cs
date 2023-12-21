using Telegram.Bot.Types.ReplyMarkups;

namespace MehranBot.Utility;

public static class DefaultContents
{
    public static string BotToken = "BotToken";
    public static string ChannelUsername = "ChannelUsername";


    public static readonly string Start = "/start".CleanString();
    public static readonly string WelcomeToBot = "به ربات خوش آمدید".CleanString();
    public static readonly string WelcomeToAdminPanel = "به پنل مدیریت خوش آمدید".CleanString();
    public static readonly string Error = "خطایی رخ داده است دوباره تلاش کنید".CleanString();
    public static readonly string ForbiddenEntry = "ورود شما به این بخش ممنوع است".CleanString();
    public static readonly string AssignAds = "این آگهی واگذار کردم ✅ ".CleanString();
    public static readonly string Back = "🔙 بازگشت".CleanString();
    public static readonly string Transactions = "💸 تراکنش ها".CleanString();
    public static readonly string BackToAdminPanel = "🔙 بازگشت به پنل مدیریت ".CleanString();



    // User Buttons
    public static readonly string RegisterAds = "🖋 ثبت آگهی".CleanString();
    public static readonly string MyAds = "🧾 آگهی های من".CleanString();
    public static readonly string AdminLogin = "⚙️ ورود ادمین".CleanString();
    public static readonly string ContactUs = "📢 ارتباط با ما".CleanString();



    // Admin Buttons
    public static readonly string CategoryManagment = "📊 مدیریت دسته بندی ها".CleanString();
    public static readonly string ChangeStartText = "🖺 تغییر متن استارت ربات".CleanString();
    public static readonly string ChangeRulesText = "📖 تغییر متن قوانین مقررات".CleanString();
    public static readonly string ChangeAdvertiseFeeAmount = "💰 تغییر مقدار هزینه ثبت آگهی".CleanString();
    public static readonly string ChangeChannelUsername = "♻️ تغییر کانال".CleanString();
    public static readonly string ChangelBotToken = "🤖 تغییر توکن ربات".CleanString();
    public static readonly string SendAll = "✉️ ارسال پیام همگانی".CleanString();
    public static readonly string ChangelBotUserName = "🤖 تغییر نام کاربری ربات".CleanString();


    public static readonly string Payment = "💰 پرداخت".CleanString();
    public static readonly string ConractToCreator = " ارتباط با کارفرما".CleanString();
    public static readonly string RegisterAdsWithBot = " ثبت آگهی با ربات ".CleanString();
    public static readonly string AddCategory = "➕ افزودن دسته بندی".CleanString();


    public static readonly string Yes = "بله".CleanString();
    public static readonly string No = "نه".CleanString();
    public static readonly string AllowAds = "تایید آگهی".CleanString();
    public static readonly string DenyAds = "لغو آگهی".CleanString();


    public static readonly string Approve = "می پذیرم".CleanString();
    public static readonly string Decline = "نمی پذیرم".CleanString();
    public static readonly string IsMembership = "عضو شدم".CleanString();


    // Bank Port 
    public static string merchant = "merchant code";
    public static string amount = "10000";
    public static string description = "پرداخت برای درج آگهی";
    public static string callbackurl = "callbackurl";





    // Methods
    public static ReplyKeyboardMarkup GenerateMainKeyboard()
    {
        var rows = new List<KeyboardButton[]>();


        rows.Add(new KeyboardButton[]
        {
                new KeyboardButton(DefaultContents.RegisterAds),
                new KeyboardButton(DefaultContents.MyAds)
        });


        rows.Add(new KeyboardButton[]
        {
                  //new KeyboardButton(DefaultContents.ContactUs),
                  new KeyboardButton(DefaultContents.AdminLogin)
        });


        rows.Add(new KeyboardButton[]
         {

         });

        rows.Add(new KeyboardButton[]
           {

           });

        rows.Add(new KeyboardButton[]
         {

         });


        return new ReplyKeyboardMarkup(rows);
    }

    public static ReplyKeyboardMarkup GenerateAdminKeyboard()
    {
        var rows = new List<KeyboardButton[]>();

        rows.Add(new KeyboardButton[]
        {
                new KeyboardButton(DefaultContents.CategoryManagment),
                new KeyboardButton(DefaultContents.ChangeStartText)
        });


        rows.Add(new KeyboardButton[]
        {
                  new KeyboardButton(DefaultContents.ChangeRulesText),
                  new KeyboardButton(DefaultContents.ChangeAdvertiseFeeAmount)
        });

        rows.Add(new KeyboardButton[]
        {
                      new KeyboardButton(DefaultContents.ChangeChannelUsername),
                      new KeyboardButton(DefaultContents.ChangelBotToken)
        });

        rows.Add(new KeyboardButton[]
        {
                      new KeyboardButton(DefaultContents.SendAll),
                  new KeyboardButton(DefaultContents.ChangelBotUserName),
        });


        rows.Add(new KeyboardButton[]
        {
                 new KeyboardButton(DefaultContents.Transactions),
                  new KeyboardButton(DefaultContents.Back),

        });

        return new ReplyKeyboardMarkup(rows);
    }


    public static InlineKeyboardMarkup GenerateAddCategoryInlineKeyboard()
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {
                new InlineKeyboardButton(DefaultContents.AddCategory) { CallbackData = DefaultContents.AddCategory }
        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }


    public static InlineKeyboardMarkup GenerateShowAllCategoryInlineKeyboard(string title, int id)
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {
                new InlineKeyboardButton(title) { CallbackData = $"Cat_{id}" }
        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }


    public static InlineKeyboardMarkup GenerateAssignAdsInlineKeyboard(long adsId)
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {
                new InlineKeyboardButton(DefaultContents.AssignAds) { CallbackData = $"AS_{adsId}" }
        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }



    public static InlineKeyboardMarkup GenerateAdsPaymentInlineKeyboard(long price, string adsToken)
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {
           new InlineKeyboardButton(string.Format(" پرداخت {0} تومان " , price)) { Url = $"https://mehranbot.hamyar-stu.ir/Payment/SetPayment?atk={adsToken}" }
        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }


    public static InlineKeyboardMarkup GenerateAdsPaymentConfirmInlineKeyboard(long adsId)
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {

           new InlineKeyboardButton(DefaultContents.AllowAds) { CallbackData = $"AT_{adsId}" },
           new InlineKeyboardButton(DefaultContents.DenyAds) { CallbackData = $"DD_{adsId}" },

        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }


    public static InlineKeyboardMarkup GenerateShowAdsInChannelButtonsInlineKeyboard(string creatorUsername, string botUsername)
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {

           new InlineKeyboardButton(DefaultContents.ConractToCreator) { Url = $"https://t.me/{creatorUsername}"},
           new InlineKeyboardButton(DefaultContents.RegisterAdsWithBot) { Url = $"https://t.me/{botUsername}" },

        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }

    public static InlineKeyboardMarkup GenerateShowAdsInChannelButtonsInAssainedInlineKeyboard(string botUsername)
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {

           new InlineKeyboardButton(DefaultContents.RegisterAdsWithBot) { Url = $"https://t.me/{botUsername}" },

        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }






    public static InlineKeyboardMarkup GenerateConfirmDeleteCategoryInlineKeyboard()
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {
                new InlineKeyboardButton(DefaultContents.Yes) { CallbackData = DefaultContents.Yes },
                new InlineKeyboardButton(DefaultContents.No) { CallbackData = DefaultContents.No }
        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }



    public static InlineKeyboardMarkup GenerateConfirmRulesTextInlineKeyboard()
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {
                new InlineKeyboardButton(DefaultContents.Approve) { CallbackData = DefaultContents.Approve },
                new InlineKeyboardButton(DefaultContents.Decline) { CallbackData = DefaultContents.Decline },
        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }


    public static InlineKeyboardMarkup GenerateConfirmMembershipTextInlineKeyboard()
    {
        var rows = new List<InlineKeyboardButton[]>();

        rows.Add(new InlineKeyboardButton[]
        {
                new InlineKeyboardButton(DefaultContents.IsMembership) { CallbackData = DefaultContents.IsMembership },

        });

        var keyboard = new InlineKeyboardMarkup(rows);
        return keyboard;
    }

}
