using MehranBot.Models.Entities;
using MehranBot.Models.UnitOfWork;
using MehranBot.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = MehranBot.Models.Entities.User;

namespace MehranBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private TelegramBotClient _bot;
        private readonly IUnitOfWork _context;

        private Setting setting = null;
        private User user = null;
       

        public BotController(IUnitOfWork context)
        {
            _context = context;
        }




        #region GetUpdate
        [HttpPost]
        public async Task<IActionResult> GetUpdate(object model, CancellationToken cancellationToken)
        {
            try
            {
                if (model != null)
                {

                    var data = JsonConvert
                                  .DeserializeObject<Telegram.Bot.Types.Update>(model.ToString());

                    if (data != null)
                    {

                        long chatId = data.CallbackQuery != null ? data.CallbackQuery.From.Id : data.Message.Chat.Id;
                        long userId = data.CallbackQuery != null ? data.CallbackQuery.From.Id : data.Message.From.Id;
                        string text = data.CallbackQuery != null ? data.CallbackQuery.Data.CleanString() : data.Message.Text.CleanString();
                        string username = data.CallbackQuery != null ? data.CallbackQuery.From.Username : data.Message.Chat.Username.CleanString();


                        setting = await _context.SettingManagerUW.GetSingleAsync(s => s.IsEnable);
                        user = await _context.UserManagerUW.GetSingleAsync(a => a.IsEnable && a.ChatId == chatId);


                        if (setting == null)
                        {
                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                                      "عدم وجود  تنظمیات !! خطایی رخ داده است به مدیر ربات اطلاع ذهید",
                                                                      replyMarkup: DefaultContents.GenerateMainKeyboard());



                            return Ok();
                        }



                        #region InitTelegramBotClient
                        if (_bot == null)
                            _bot = new TelegramBotClient(setting == null ? DefaultContents.BotToken : setting.BotToken);
                        #endregion InitTelegramBotClient



                        if (text == DefaultContents.Start)
                        {

                            if (user == null)
                            {
                                // user add when start the bot
                                var newUser = new Models.Entities.User
                                {
                                    ChatId = chatId,
                                    UserName = username,
                                    FirstName = data.Message.Chat.FirstName,
                                    LastName = data.Message.Chat.LastName,
                                    IsAdmin = false,
                                    IsConfirmedRules = false,
                                    IsMembership = false


                                };

                                _context.UserManagerUW.Create(newUser);
                                await _context.saveAsync();


                                user = newUser;
                            }



                            // start message 
                            string welcomeMsg = "";

                            if (setting != null)
                                welcomeMsg = setting.StartBotText;
                            else
                                welcomeMsg = DefaultContents.WelcomeToBot;

                            await _bot.SendTextMessageAsync(chatId: chatId, welcomeMsg);

                            await SetUserActivity(user, ActivityType.StartBot);
                            // end send message




                            //check user approve Confirms 
                            await CheckRulesAndMembershipStatuses(setting, chatId, user);


                        }


                        else if (text == DefaultContents.AdminLogin)
                        {
                            if (user != null && user.IsAdmin)
                            {
                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                                    DefaultContents.WelcomeToAdminPanel,
                                                                    replyMarkup: DefaultContents.GenerateAdminKeyboard());


                                await SetUserActivity(user, ActivityType.AdminPanel);


                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                                        DefaultContents.ForbiddenEntry,
                                                                        replyMarkup: DefaultContents.GenerateMainKeyboard());


                                return Ok();
                            }
                        }


                        #region AdminPanel Start Here


                        else if (text == DefaultContents.CategoryManagment)
                        {
                            if (user != null && user.IsAdmin)
                            {

                                await ShowAllCategories(chatId);
                                await SetUserActivity(user, ActivityType.ShowCategory);



                            }
                            else
                            {

                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                       DefaultContents.ForbiddenEntry,
                                                       replyMarkup: DefaultContents.GenerateMainKeyboard());

                                return Ok();
                            }



                        }
                        else if (text.StartsWith("/del_"))
                        {
                            if (user != null && user.IsAdmin)
                            {
                                var delText = text.Split('_');

                                var category = await _context.CategoryManagerUW.GetSingleAsync(a => a.IsEnable && a.Id == int.Parse(delText[1]));



                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                             $" آیا از حذف دسته بندی {category.Title}  مطمئن هستید ؟ ",
                                                            replyMarkup: DefaultContents.GenerateConfirmDeleteCategoryInlineKeyboard());





                                await SetUserActivity(user, ActivityType.DelCategory, category.Id.ToString());


                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                     DefaultContents.ForbiddenEntry,
                                                     replyMarkup: DefaultContents.GenerateMainKeyboard());


                                return Ok();
                            }
                        }
                        else if (text.StartsWith("/edit_"))
                        {

                            if (user != null && user.IsAdmin)
                            {
                                var editText = text.Split('_');

                                var category = await _context.CategoryManagerUW.GetSingleAsync(a => a.IsEnable && a.Id == long.Parse(editText[1]));

                                if (category != null)
                                {


                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                               " ویرایش دسته بندی  " + category.Title,
                                                               replyMarkup: DefaultContents.GenerateAdminKeyboard());



                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                               " حالا  عنوان جدید برای این دسته بندی رو وارد و ارسال کنید",
                                                               replyMarkup: DefaultContents.GenerateAdminKeyboard());



                                    await SetUserActivity(user, ActivityType.EditCategory, category.Id.ToString(), stepCount: 1);


                                }
                                else
                                {
                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                              " داده ای یافت نشد دوباره امتحان کنید ",
                                                              replyMarkup: DefaultContents.GenerateAdminKeyboard());

                                    return Ok();
                                }



                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                     DefaultContents.ForbiddenEntry,
                                                     replyMarkup: DefaultContents.GenerateMainKeyboard());


                                return Ok();
                            }
                        }
                        else if (text == DefaultContents.ChangeStartText)
                        {

                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                              " متن فعلی : \r\n  " + setting.StartBotText,
                                                             replyMarkup: DefaultContents.GenerateAdminKeyboard());



                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                              " مقدار جدیدی را برای متن استارت ربات  وارد کنید ",
                                                              replyMarkup: DefaultContents.GenerateAdminKeyboard());



                            await SetUserActivity(user, ActivityType.ChangeStartText, stepCount: 1);

                        }
                        else if (text == DefaultContents.ChangeRulesText)
                        {

                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                              " متن فعلی : \r\n  " + setting.RulesMessage,
                                                             replyMarkup: DefaultContents.GenerateAdminKeyboard());



                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                              " مقدار جدیدی را برای متن قوانین و مقررات  وارد کنید ",
                                                              replyMarkup: DefaultContents.GenerateAdminKeyboard());



                            await SetUserActivity(user, ActivityType.ChangeRulesText, stepCount: 1);

                        }
                        else if (text == DefaultContents.ChangeAdvertiseFeeAmount)
                        {

                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                              " (تومان) مقدار هزینه فعلی : \r\n  " + setting.AdvertiseFee,
                                                             replyMarkup: DefaultContents.GenerateAdminKeyboard());



                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                              "مقدار هزینه درج اگهی را بصورت عددی (تومان) وارد کنید ",
                                                              replyMarkup: DefaultContents.GenerateAdminKeyboard());



                            await SetUserActivity(user, ActivityType.ChangeAdvertiseFeeAmount, stepCount: 1);

                        }
                        else if (text == DefaultContents.AddCategory)
                        {

                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                "عنوان دسته بندی جدید خود را وارد و ارسال کنید ",
                                                 replyMarkup: DefaultContents.GenerateAdminKeyboard());


                            await SetUserActivity(user, ActivityType.AddCategory, stepCount: 1);
                        }
                        else if (text == DefaultContents.ChangeChannelUsername)
                        {
                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                "نام کاربری کانال مدنظر خودتون رو وارد و ارسال کنید و دقت کنید بدون @ باشد \r\n نام کاربری فعلی : \r\n " + "@" + setting.TargetChannelUserName,
                                                 replyMarkup: DefaultContents.GenerateAdminKeyboard());


                            await SetUserActivity(user, ActivityType.ChangeChannelUsername, stepCount: 1);
                        }
                        else if (text == DefaultContents.ChangelBotToken)
                        {
                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                " توکن ربات تلگرامی مدنظر خودتون رو وارد و ارسال کنید \r\n توکن فعلی : \r\n " + setting.BotToken,
                                                 replyMarkup: DefaultContents.GenerateAdminKeyboard());


                            await SetUserActivity(user, ActivityType.ChangeBotToken, stepCount: 1);
                        }
                        else if (text == DefaultContents.SendAll)
                        {
                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                 "متن پیام خود را وارد و ارسال کنید",
                                                 replyMarkup: DefaultContents.GenerateAdminKeyboard());


                            await SetUserActivity(user, ActivityType.SendAll, stepCount: 1);


                            return Ok();
                        }
                        else if (text == DefaultContents.ChangelBotUserName)
                        {
                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                 "نام کاربری ربات را وارد و ارسال کنید بدون @ وارد کنید",
                                                 replyMarkup: DefaultContents.GenerateAdminKeyboard());


                            await SetUserActivity(user, ActivityType.ChangeBotUserName, stepCount: 1);


                            return Ok();
                        }
                        else if (text == DefaultContents.Transactions)
                        {
                            var trans = await _context.UserPaymentManagerUW.GetAsync(a => true, s => s.OrderByDescending(s => s.Id), "User,Ads");

                            string res = "\n\r";

                            foreach (var transItem in trans)
                            {
                                res += $" خریدار : @{transItem.User.UserName} \n\r " +
                                           $" آگهی : {transItem.Ads.Text} \n\r  " +
                                           $" مبلغ : {transItem.Amount.ToString("###,###") + "تومان"} \n\r  " +
                                           $" شماره کارت : {transItem.CardNumber} \n\r  " +
                                           $" کدپیگری : {transItem.RefId} \n\r  " +
                                           $" توضیحات : {transItem.Description} \n\r \n\r " +
                                           "----------------------------------------- \n\r \n\r ";



                            }

                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                 res,
                                                 replyMarkup: DefaultContents.GenerateAdminKeyboard());


                            await SetUserActivity(user, ActivityType.ShowTransList, stepCount: 1);


                            return Ok();
                        }



                        #endregion AdminPanel End Here







                        #region UserPanel Start Here

                        else if (text == DefaultContents.RegisterAds)
                        {
                            // delete ads which is't payed
                            var removableAds = await _context.AdsManagerUW.GetAsync(a => a.IsPayed == false && a.CreateDateMl.AddDays(10).Date <= DateTime.Now.Date);
                            if (removableAds.Any())
                            {
                                _context.AdsManagerUW.DeleteAll(removableAds);
                                await _context.saveAsync();
                            }


                            var categories = await _context.CategoryManagerUW.GetAsync(a => a.IsEnable);

                            var rows = new List<InlineKeyboardButton[]>();

                            foreach (var category in categories)
                            {

                                rows.Add(new InlineKeyboardButton[]
                                {
                                    new InlineKeyboardButton(category.Title) { CallbackData = $"Cat_{category.Id}" }
                                });


                            }


                            var keyboard = new InlineKeyboardMarkup(rows);

                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                              " دسته بندی  خود را انتخاب کنید ",
                                                               replyMarkup: keyboard);



                            await SetUserActivity(user, ActivityType.RegisterAds, stepCount: 1);





                            return Ok();

                        }
                        else if (text == DefaultContents.MyAds)
                        {
                            // delete ads which is't payed
                            var removableAds = await _context.AdsManagerUW.GetAsync(a => a.IsPayed == false && a.CreateDateMl.AddDays(10).Date <= DateTime.Now.Date);
                            if (removableAds.Any())
                            {
                                _context.AdsManagerUW.DeleteAll(removableAds);
                                await _context.saveAsync();
                            }


                            var ads = await _context.AdsManagerUW.GetAsync(a => a.IsEnable && a.User.ChatId == chatId, s => s.OrderByDescending(s => s.Id), "User,Category");

                            if (ads.Any())
                            {
                                InlineKeyboardMarkup replyMarkup = null;

                                foreach (var adsItem in ads)
                                {
                                    var adsInfo = $" \r\n {AppUtility.GetCategoryHashTag(adsItem.Category.Title)} " +
                                                    $" \r\n \r\n  توضیحات آگهی  :  {adsItem.Text}  \r\n \r\n  " +
                                                    $" وضیعت واگذاری   :  {(adsItem.IsAssigned ? " ✅ واگذار شده" : " واگذار نشده ")} \r\n \r\n " +
                                                    $" وضیعت پرداخت  :  {(adsItem.IsPayed ? " ✅ پرداخت شده" : " پرداخت نشده ")} \r\n \r\n " +
                                                    $" وضیعت تایید مدیر  :  {(adsItem.IsConfirmed ? " ✅ تایید شده" : " تایید نشده ")} \r\n \r\n ";


                                    if (!adsItem.IsPayed)
                                    {
                                        var adsDate = adsItem.CreateDateMl.Date.AddDays(10);
                                        var currentDate = DateTime.Now.Date;

                                        var totalDay = (adsDate - currentDate).TotalDays;

                                        adsInfo += $" \n\r  تعداد روز تا حذف اگهی : {totalDay} روز";

                                    }


                                    if (!adsItem.IsPayed)
                                        replyMarkup = DefaultContents.GenerateAdsPaymentInlineKeyboard(setting.AdvertiseFee, adsItem.Token);
                                    else if (adsItem.IsConfirmed && !adsItem.IsAssigned)
                                        replyMarkup = DefaultContents.GenerateAssignAdsInlineKeyboard(adsItem.Id);
                                    else
                                        replyMarkup = null;



                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                    adsInfo,
                                                    replyMarkup: replyMarkup);


                                }

                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                                  " شما اگهی درج نکردید یا  یادتان باشد اگهی ها پرداخت نشده بعد از 10 روز آگهی های پرداخت نشده حذف میشوند ",
                                                                   replyMarkup: DefaultContents.GenerateMainKeyboard());

                            }



                            await SetUserActivity(user, ActivityType.MyAds, stepCount: 1);



                            return Ok();

                        }
                        else if (text.StartsWith("Cat_"))
                        {
                            int categoryId = int.Parse(text.Split('_')[1]);


                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                    " متن آگهی خود را وارد کنید (کلمات توهین آمیز وارد نکنید) ",
                                                     replyMarkup: DefaultContents.GenerateMainKeyboard());


                            var newAds = new Ads
                            {
                                Text = "",
                                FkUserId = user.Id,
                                FkCategoryId = categoryId,
                                MessageId = 0,

                            };

                            _context.AdsManagerUW.Create(newAds);
                            await _context.saveAsync();


                            await SetUserActivity(user, ActivityType.RegisterAds, value: newAds.Id.ToString(), stepCount: 2);


                            return Ok();
                        }
                        else if (text.StartsWith("AT_"))
                        {
                            int adsId = int.Parse(text.Split('_')[1]);

                            if (adsId > 0)
                            {
                                var ads = await _context.AdsManagerUW.GetAsync(a => a.Id == adsId, "User,Category");

                                if (ads != null)
                                {

                                    try
                                    {
                                        // send to channel

                                        var adsInfo = $" \r\n {AppUtility.GetCategoryHashTag(ads.Category.Title)}  \r\n \r\n  توضیحات آگهی  :  {ads.Text}  \r\n \r\n  ";



                                        var message = await _bot.SendTextMessageAsync($"@{setting.TargetChannelUserName}",
                                                                             adsInfo,
                                                                             replyMarkup: DefaultContents.GenerateShowAdsInChannelButtonsInlineKeyboard(ads.User.UserName, setting.BotUsername));



                                        ads.IsConfirmed = true;

                                        if (message != null)
                                        {
                                            ads.MessageId = message.MessageId;

                                        }


                                        _context.AdsManagerUW.Update(ads);
                                        await _context.saveAsync();



                                        // send message to user

                                        await _bot.SendTextMessageAsync(chatId: ads.User.ChatId,
                                                       $" کاربر گرامی آگهی شما با موفقیت در کانال @{setting.TargetChannelUserName} درج شد ",
                                                       replyMarkup: DefaultContents.GenerateMainKeyboard());


                                        // send message to admin

                                        await _bot.SendTextMessageAsync(chatId: setting.AdminChatId,
                                                       $" عملیات موفقیت آمیز بود ",
                                                       replyMarkup: DefaultContents.GenerateMainKeyboard());


                                    }
                                    catch (Exception)
                                    {
                                        await _bot.SendTextMessageAsync(chatId: chatId,
                                                   DefaultContents.Error,
                                                   replyMarkup: DefaultContents.GenerateMainKeyboard());


                                        return Ok();
                                    }



                                }
                                else
                                {
                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                    DefaultContents.Error,
                                                    replyMarkup: DefaultContents.GenerateMainKeyboard());



                                }




                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                DefaultContents.Error,
                                                replyMarkup: DefaultContents.GenerateMainKeyboard());



                            }



                            return Ok();
                        }
                        else if (text.StartsWith("DD_"))
                        {
                            int adsId = int.Parse(text.Split('_')[1]);


                            if (adsId > 0)
                            {
                                var ads = await _context.AdsManagerUW.GetAsync(a => a.Id == adsId, "User");

                                if (ads is not null)
                                {
                                    await _bot.SendTextMessageAsync(chatId: ads.User.ChatId,
                                                $" این آگهی شما با توضیحات {ads.Text} بدلیل عدم تایید مدیر ربات   درج نشد ",
                                                replyMarkup: DefaultContents.GenerateMainKeyboard());




                                    // send message to admin

                                    await _bot.SendTextMessageAsync(chatId: setting.AdminChatId,
                                                   $" عملیات موفقیت آمیز بود ",
                                                   replyMarkup: DefaultContents.GenerateMainKeyboard());

                                }

                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                DefaultContents.Error,
                                                replyMarkup: DefaultContents.GenerateMainKeyboard());



                            }


                            return Ok();
                        }
                        else if (text.StartsWith("AS_"))
                        {
                            int adsId = int.Parse(text.Split('_')[1]);


                            if (adsId > 0)
                            {
                                var ads = await _context.AdsManagerUW.GetAsync(a => a.Id == adsId, "User,Category");

                                if (ads is not null)
                                {
                                    try
                                    {
                                        await _bot.SendTextMessageAsync(chatId: ads.User.ChatId,
                                               $" عملیات موفقیت آمیز بود این آگهی شما با توضیحات {ads.Text}  واگذار شد  ✅",
                                               replyMarkup: DefaultContents.GenerateMainKeyboard());


                                        long messageId = ads.MessageId;
                                        if (messageId > 0)
                                        {

                                            var adsInfo = $" \r\n {AppUtility.GetCategoryHashTag(ads.Category.Title)}  \r\n \r\n  توضیحات آگهی  :  {ads.Text}  \r\n \r\n   این اگهی واگذار شد ✅ ";



                                            await _bot.EditMessageTextAsync(chatId: $"@{setting.TargetChannelUserName}", messageId: Convert.ToInt32(messageId),
                                                    adsInfo,
                                                    replyMarkup: DefaultContents.GenerateShowAdsInChannelButtonsInAssainedInlineKeyboard(setting.BotUsername));



                                            ads.IsAssigned = true;
                                            _context.AdsManagerUW.Update(ads);
                                            _context.saveAsync();
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        await _bot.SendTextMessageAsync(chatId: chatId,
                                                DefaultContents.Error,
                                                replyMarkup: DefaultContents.GenerateMainKeyboard());

                                    }

                                }

                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(chatId: chatId,
                                                DefaultContents.Error,
                                                replyMarkup: DefaultContents.GenerateMainKeyboard());



                            }


                            return Ok();
                        }


                        #endregion UserPanel End Here






                        // back button
                        else if (text == DefaultContents.Back)
                        {
                            await _bot.SendTextMessageAsync(chatId: chatId,
                                                            DefaultContents.WelcomeToBot,
                                                            replyMarkup: DefaultContents.GenerateMainKeyboard());


                            await SetUserActivity(user, ActivityType.BackToMainPanel);


                            return Ok();
                        }







                        // check activity

                        var userActivity = await _context
                                                    .UserActivityManagerUW
                                                    .GetSingleAsync(a => a.IsEnable && a.FkUserId == user.Id);

                        if (userActivity != null)
                        {

                            if (userActivity.ActivityType == ActivityType.EditCategory)
                            {

                                if (userActivity.StepCount == 1)
                                {
                                    userActivity.StepCount = 2;
                                    _context.UserActivityManagerUW.Update(userActivity);
                                    await _context.saveAsync();
                                }
                                else if (userActivity.StepCount == 2)
                                {
                                    var category = await _context.CategoryManagerUW.GetByIdAsync(int.Parse(userActivity.Value));
                                    if (category != null)
                                    {
                                        category.Title = text;
                                        _context.CategoryManagerUW.Update(category);
                                        await _context.saveAsync();

                                        await _bot.SendTextMessageAsync(chatId: chatId,
                                                                      " دسته بندی مدنظر شما باموفقیت ویرایش شد ✅",
                                                                      replyMarkup: DefaultContents.GenerateAdminKeyboard());

                                        await ShowAllCategories(chatId);

                                        await SetUserActivity(user, ActivityType.ShowCategory);

                                    }


                                }



                            }
                            else if (userActivity.ActivityType == ActivityType.DelCategory)
                            {

                                if (text == DefaultContents.Yes)
                                {
                                    var category = await _context.CategoryManagerUW.GetByIdAsync(int.Parse(userActivity.Value));
                                    if (category != null)
                                    {
                                        category.IsEnable = false;
                                        _context.CategoryManagerUW.Update(category);
                                        await _context.saveAsync();

                                        await _bot.SendTextMessageAsync(chatId: chatId,
                                                                     " دسته بندی مدنظر شما باموفقیت حذف شد ✅",
                                                                     replyMarkup: DefaultContents.GenerateAdminKeyboard());


                                        await ShowAllCategories(chatId);
                                        await SetUserActivity(user, ActivityType.ShowCategory);
                                    }
                                }
                                else if (text == DefaultContents.No)
                                {
                                    await ShowAllCategories(chatId);
                                    await SetUserActivity(user, ActivityType.ShowCategory);
                                }



                            }
                            else if (userActivity.ActivityType == ActivityType.AddCategory)
                            {

                                if (userActivity.StepCount == 1)
                                {
                                    userActivity.StepCount = 2;
                                    _context.UserActivityManagerUW.Update(userActivity);
                                    await _context.saveAsync();
                                }
                                else
                                {
                                    //add category
                                    _context.CategoryManagerUW.Create(new Category
                                    {
                                        Title = text,

                                    });

                                    await _context.saveAsync();

                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                                    " دسته بندی مدنظر شما باموفقیت اضافه شد ✅",
                                                                    replyMarkup: DefaultContents.GenerateAdminKeyboard());


                                    await ShowAllCategories(chatId);
                                    await SetUserActivity(user, ActivityType.ShowCategory);
                                }



                            }
                            else if (userActivity.ActivityType == ActivityType.ChangeStartText)
                            {
                                if (userActivity.StepCount == 1)
                                {
                                    userActivity.StepCount = 2;
                                    _context.UserActivityManagerUW.Update(userActivity);
                                    await _context.saveAsync();
                                }
                                else
                                {


                                    setting.StartBotText = text;
                                    _context.SettingManagerUW.Update(setting);
                                    await _context.saveAsync();


                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                                  " عملیات با موفقیت انجام شد ✅",
                                                                  replyMarkup: DefaultContents.GenerateAdminKeyboard());
                                }



                            }
                            else if (userActivity.ActivityType == ActivityType.ChangeRulesText)
                            {
                                if (userActivity.StepCount == 1)
                                {
                                    userActivity.StepCount = 2;
                                    _context.UserActivityManagerUW.Update(userActivity);
                                    await _context.saveAsync();
                                }
                                else
                                {
                                    setting.RulesMessage = text;
                                    _context.SettingManagerUW.Update(setting);
                                    await _context.saveAsync();


                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                                  " عملیات با موفقیت انجام شد ✅",
                                                                  replyMarkup: DefaultContents.GenerateAdminKeyboard());
                                }


                            }
                            else if (userActivity.ActivityType == ActivityType.ChangeAdvertiseFeeAmount)
                            {
                                if (userActivity.StepCount == 1)
                                {
                                    userActivity.StepCount = 2;
                                    _context.UserActivityManagerUW.Update(userActivity);
                                    await _context.saveAsync();
                                }
                                else
                                {
                                    //check input is number format
                                    long feeAmount = 0;

                                    if (text.IsNumber())
                                    {
                                        feeAmount = long.Parse(text);
                                    }
                                    else
                                    {
                                        await _bot.SendTextMessageAsync(chatId: chatId,
                                                                 " مقدار مبلغ باید بصورت عددی باشد ",
                                                                 replyMarkup: DefaultContents.GenerateAdminKeyboard());


                                        return Ok();
                                    }


                                    setting.AdvertiseFee = feeAmount;
                                    _context.SettingManagerUW.Update(setting);
                                    await _context.saveAsync();


                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                                  " عملیات با موفقیت انجام شد ✅",
                                                                  replyMarkup: DefaultContents.GenerateAdminKeyboard());
                                }


                            }
                            else if (userActivity.ActivityType == ActivityType.ChangeChannelUsername)
                            {
                                if (userActivity.StepCount == 1)
                                {
                                    userActivity.StepCount = 2;
                                    _context.UserActivityManagerUW.Update(userActivity);
                                    await _context.saveAsync();
                                }
                                else
                                {
                                    setting.TargetChannelUserName = text;
                                    _context.SettingManagerUW.Update(setting);
                                    await _context.saveAsync();


                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                                  " عملیات با موفقیت انجام شد ✅",
                                                                  replyMarkup: DefaultContents.GenerateAdminKeyboard());
                                }


                            }
                            else if (userActivity.ActivityType == ActivityType.ChangeBotToken)
                            {
                                if (userActivity.StepCount == 1)
                                {
                                    userActivity.StepCount = 2;
                                    _context.UserActivityManagerUW.Update(userActivity);
                                    await _context.saveAsync();
                                }
                                else
                                {
                                    setting.BotToken = text;
                                    _context.SettingManagerUW.Update(setting);
                                    await _context.saveAsync();


                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                                  " عملیات با موفقیت انجام شد ✅",
                                                                  replyMarkup: DefaultContents.GenerateAdminKeyboard());
                                }


                            }
                            else if (userActivity.ActivityType == ActivityType.SendAll)
                            {
                                if (userActivity.StepCount == 1)
                                {
                                    var users = await _context.UserManagerUW.GetAsync(a => a.IsEnable && a.ChatId != chatId);

                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                                 " عملیات با موفقیت انجام شد ✅",
                                                                 replyMarkup: DefaultContents.GenerateAdminKeyboard());

                                    foreach (var myuser in users)
                                    {
                                        await _bot.SendTextMessageAsync(chatId: user.ChatId, text);

                                    }
                                }


                            }
                            else if (userActivity.ActivityType == ActivityType.ChangeBotUserName)
                            {
                                if (userActivity.StepCount == 1)
                                {
                                    setting.BotUsername = text;
                                    _context.SettingManagerUW.Update(setting);
                                    await _context.saveAsync();


                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                                  " عملیات با موفقیت انجام شد ✅",
                                                                  replyMarkup: DefaultContents.GenerateAdminKeyboard());
                                }



                            }
                            else if (userActivity.ActivityType == ActivityType.ConfirmRules)
                            {
                                // check rules confirm is approved
                                if (text == DefaultContents.Approve)
                                {

                                    user.IsConfirmedRules = true;
                                    _context.UserManagerUW.Update(user);
                                    await _context.saveAsync();

                                    // check user membership status

                                    var chatMember = await _bot.GetChatMemberAsync($"@{setting.TargetChannelUserName} ", userId);

                                    if (chatMember != null)
                                    {
                                        // user has join to channel
                                        if (chatMember.Status == ChatMemberStatus.Member)
                                        {
                                            // it's ok 

                                            user.IsMembership = true;
                                            _context.UserManagerUW.Update(user);
                                            await _context.saveAsync();


                                            // show the main menu and let user to add ads and so on ... 
                                            await ShowCanAddAdsMessage(user, setting, chatId);
                                        }
                                        else
                                        {
                                            await ShowConfirmMembership(user, setting, chatId);
                                        }
                                    }
                                    else
                                    {
                                        await ShowConfirmMembership(user, setting, chatId);
                                    }



                                }
                                else if (text == DefaultContents.Decline)
                                {



                                    await ShowConfirmRules(user, setting, chatId);


                                    user.IsConfirmedRules = false;
                                    _context.UserManagerUW.Update(user);
                                    await _context.saveAsync();

                                }

                            }
                            else if (userActivity.ActivityType == ActivityType.ConfirmMembership)
                            {
                                // check rules confirm is approved
                                if (text == DefaultContents.IsMembership)
                                {

                                    var chatMember = await _bot.GetChatMemberAsync($"@{setting.TargetChannelUserName} ", userId);

                                    if (chatMember != null)
                                    {
                                        // user has join to channel
                                        if (chatMember.Status == ChatMemberStatus.Member)
                                        {
                                            user.IsMembership = true;
                                            _context.UserManagerUW.Update(user);
                                            await _context.saveAsync();


                                            // show the main menu and let user to add ads and so on ... 
                                            await ShowCanAddAdsMessage(user, setting, chatId);
                                        }
                                        else
                                            await ShowConfirmMembership(user, setting, chatId);
                                    }
                                    else
                                        await ShowConfirmMembership(user, setting, chatId);

                                }


                            }
                            else if (userActivity.ActivityType == ActivityType.RegisterAds)
                            {
                                //check user have any username
                                if (string.IsNullOrEmpty(username))
                                {
                                    await _bot.SendTextMessageAsync(chatId: chatId,
                                                              " برای درج آگهی ابتدا برای حساب خود یک نام کاربری ایجاد کنید شما در حال حاضر دارای نام کاربری نیستید",
                                                              replyMarkup: DefaultContents.GenerateMainKeyboard());


                                    return Ok();
                                }




                                if (userActivity.StepCount == 2)
                                {
                                    long adsId = long.Parse(userActivity.Value);

                                    var ads = await _context.AdsManagerUW.GetAsync(a => a.Id == adsId, "Category");

                                    if (ads != null)
                                    {
                                        //var category = await _context.CategoryManagerUW.GetByIdAsync(ads.FkCategoryId);

                                        ads.Text = text;
                                        _context.AdsManagerUW.Update(ads);
                                        await _context.saveAsync();



                                        await _bot.SendTextMessageAsync(chatId: chatId,
                                                                " آگهی  شما باموفقیت ایجاد شد ✅",
                                                                replyMarkup: DefaultContents.GenerateMainKeyboard());





                                        await _bot.SendTextMessageAsync(chatId: chatId,
                                                                        $" \r\n {AppUtility.GetCategoryHashTag(ads.Category.Title)}  \r\n \r\n  توضیحات آگهی  :  {ads.Text}  \r\n \r\n  برای درج آگهی در کانال هزینه درج اگهی رو پرداخت کنید  \r\n \r\n",
                                                                         replyMarkup: DefaultContents.GenerateAdsPaymentInlineKeyboard(setting.AdvertiseFee, ads.Token));




                                    }

                                }



                            }

                        }


                    }
                    else
                        return Ok();



                }
                else
                    return Ok();


            }
            catch (Exception ex)
            {
                return Ok();
            }


            return Ok();
        }

        #endregion EndGetUpdate





        #region Methods
        private async Task ShowAllCategories(long chatId)
        {

            // category management
            var categories = await _context.CategoryManagerUW.GetAsync(a => a.IsEnable);

            string msg = "لیست دسته بندی ها" + "\r\n"
                + "*****************************" + "\r\n";



            foreach (var categoryitem in categories)
            {
                msg += categoryitem.Title + "\r\n"
                                        + "/del_" + categoryitem.Id
                                        + "\r\n" + "/edit_" + categoryitem.Id + "\r\n" +
                                        "----------------------------------------" + "\r\n";

            }



            await _bot.SendTextMessageAsync(chatId: chatId,
                                        msg,
                                        replyMarkup: DefaultContents.GenerateAddCategoryInlineKeyboard());


        }
        private async Task ShowCanAddAdsMessage(User user, Setting setting, long chatId)
        {
            await _bot.SendTextMessageAsync(chatId: chatId,
                            "  از همراهی شما سپاسگذاریم  💕💕  \r\n  میتوانید اگهی خود را ثبت کنید  \r\n",
                             replyMarkup: DefaultContents.GenerateMainKeyboard());
        }
        private async Task ShowConfirmMembership(User user, Setting setting, long chatId)
        {
            await _bot.SendTextMessageAsync(chatId: chatId,
                                                               "  شما عضو کانال نیستید برای  استفاده از امکانات ربات باید حتما عضو کانال زیر باشید   \r\n " +
                                                               $"@{setting.TargetChannelUserName} \r\n",
                                                               replyMarkup: DefaultContents.GenerateConfirmMembershipTextInlineKeyboard());


            await SetUserActivity(user, ActivityType.ConfirmMembership, stepCount: 1);

        }
        private async Task CheckRulesAndMembershipStatuses(Setting setting, long chatId, Models.Entities.User user)
        {

            if (!user.IsConfirmedRules)
                await ShowConfirmRules(user, setting, chatId);
            else if (!user.IsMembership)
                await ShowConfirmMembership(user, setting, chatId);
            else
                await ShowCanAddAdsMessage(user, setting, chatId);

        }
        private async Task ShowConfirmRules(User user, Setting setting, long chatId)
        {
            await _bot.SendTextMessageAsync(chatId: chatId,
                                                                " متن قوانین و مقررات :" + setting.RulesMessage,
                                                                 replyMarkup: DefaultContents.GenerateConfirmRulesTextInlineKeyboard());


            await SetUserActivity(user, ActivityType.ConfirmRules, stepCount: 1);

        }
        private async Task SetUserActivity(User user, ActivityType activityType, string value = "", byte stepCount = 0)
        {
            var userActivity = await _context.UserActivityManagerUW.GetSingleAsync(a => a.IsEnable && a.FkUserId == user.Id);
            if (userActivity != null)
            {
                userActivity.ActivityType = activityType;
                userActivity.Value = value;
                userActivity.StepCount = stepCount;

                _context.UserActivityManagerUW.Update(userActivity);

            }
            else
            {
                _context.UserActivityManagerUW.Create(new UserActivity
                {
                    FkUserId = user.Id,
                    Value = value,
                    StepCount = stepCount,
                    ActivityType = activityType
                });

            }

            await _context.saveAsync();

        }
        public async Task<User> GetUser(long chatId, string username)
        {
            return await _context.UserManagerUW.GetSingleAsync(a => a.IsEnable &&
                                                                   (a.ChatId == chatId || a.UserName.Trim() == username));

        }

        #endregion EndMethods

    }
}
