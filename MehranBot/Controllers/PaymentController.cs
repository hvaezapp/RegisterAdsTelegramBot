using MehranBot.Models;
using MehranBot.Models.Entities;
using MehranBot.Models.UnitOfWork;
using MehranBot.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using Telegram.Bot;

namespace MehranBot.Controllers
{

    public class PaymentController : Controller
    {
        private string authority;

        private readonly IUnitOfWork _context;
        private TelegramBotClient _bot;



        public PaymentController(IUnitOfWork context)
        {

            _context = context;

        }


        [HttpGet]
        public async Task<IActionResult> SetPayment(string atk)
        {
            Setting setting = null;

            try
            {

                

                try
                {
                     setting = await _context.SettingManagerUW.GetSingleAsync(s => s.IsEnable);

                    if (setting == null)
                    {
                        TempData["type"] = "info";
                        TempData["status"] = "fail";
                        TempData["message"] = "خطایی رخ داده است";
                        return View();
                    }

                }
                catch (Exception ex)
                {

                    TempData["type"] = "info";
                    TempData["status"] = "fail";
                    TempData["message"] = "خطایی رخ داده است" + ex.Message;
                    return View();
                }

                // _bot = new TelegramBotClient(setting.BotToken);


                var ads = await _context.AdsManagerUW.GetSingleAsync(a => a.Token.Trim() == atk.Trim());
                if (ads == null)
                {
                    TempData["telbotus"] = setting.BotUsername;
                    TempData["type"] = "Warning";
                    TempData["status"] = "fail";
                    TempData["message"] = " این اگهی وجود ندارد آگهی ها بعد از 10 روز حذف میشوند ";
                    return View();
                }

                // var adsInfo = $" \r\n {AppUtility.GetCategoryHashTag(ads.Category.Title)}  \r\n \r\n  توضیحات آگهی  :  {ads.Text}  \r\n \r\n  درخواست کننده  :  {"@" + ads.User.UserName} \r\n \r\n آگهی جدید ثبت و پرداخت شده برای تایید از گزینه های زیر استفاده کنید  \r\n \r\n";

                //var message =   await _bot.SendTextMessageAsync(setting.AdminChatId,
                //                                      adsInfo,
                //                                      replyMarkup: DefaultContents.GenerateAdsPaymentConfirmInlineKeyboard(ads.Id));


                //ads.MessageId = message.MessageId;
                //await _context.saveAsync();

                RequestDataParameters Parameters = new RequestDataParameters(DefaultContents.merchant,
                                                                            setting.AdvertiseFee.ToString(),
                                                                            DefaultContents.description,
                                                                            string.Format(DefaultContents.callbackurl, atk)
                                                                            , "", "");

                var client = new RestClient(URLs.requestUrl);

                Method method = Method.Post;

                var request = new RestRequest("", method);

                request.AddHeader("accept", "application/json");

                request.AddHeader("content-type", "application/json");

                request.AddJsonBody(Parameters);

                var requestresponse = client.ExecuteAsync(request);

                JObject jo = JObject.Parse(requestresponse.Result.Content);

                string errorscode = jo["errors"].ToString();

                JObject jodata = JObject.Parse(requestresponse.Result.Content);

                string dataauth = jodata["data"].ToString();


                if (dataauth != "[]")
                {

                    authority = jodata["data"]["authority"].ToString();

                    string gatewayUrl = URLs.gateWayUrl + authority;

                    // go to bank
                    return Redirect(gatewayUrl);

                }
                else
                {
                    TempData["telbotus"] = setting.BotUsername;
                    TempData["type"] = "info";
                    TempData["status"] = "fail";
                    TempData["message"] = "خطا در اتصال به درگاه بانکی";
                }



            }

            catch (Exception ex)
            {
                TempData["telbotus"] = setting.BotUsername;
                TempData["type"] = "info";
                TempData["status"] = "fail";
                TempData["message"] = "خطایی رخ داده است" + ex.Message;
            }


            return View();

        }



        [HttpGet]
        public async Task<IActionResult> VerifyPayment(string atk)
        {

            Setting setting = null;

            try
            {
                VerifyParameters parameters = new VerifyParameters();


                if (HttpContext.Request.Query["Authority"] != "")
                {
                    authority = HttpContext.Request.Query["Authority"];
                }


               


                 setting = await _context.SettingManagerUW.GetSingleAsync(s => s.IsEnable);
                if (setting == null)
                {
                    TempData["type"] = "info";
                    TempData["status"] = "fail";
                    TempData["message"] = "خطایی رخ داده است";

                    return View();

                }


                UserPayment userPayment = new UserPayment();

                var ads = await _context.AdsManagerUW.GetAsync(a => a.Token.Trim() == atk.Trim(), "Category,User");

                if (ads == null)
                {
                    TempData["telbotus"] = setting.BotUsername;
                    TempData["type"] = "Warning";
                    TempData["status"] = "fail";
                    TempData["message"] = " این اگهی وجود ندارد آگهی ها بعد از 10 روز حذف میشوند ";
                    return View();
                }




                parameters.authority = authority;
                parameters.amount = setting.AdvertiseFee.ToString();
                parameters.merchant_id = DefaultContents.merchant;


               

                // send request
                var client = new RestClient(URLs.verifyUrl);

                Method method = Method.Post;

                var request = new RestRequest("", method);

                request.AddHeader("accept", "application/json");

                request.AddHeader("content-type", "application/json");

                request.AddJsonBody(parameters);

                var response = await client.ExecuteAsync(request);

                JObject jodata = JObject.Parse(response.Content);

                string data = jodata["data"].ToString();

                JObject jo = JObject.Parse(response.Content);

                string errors = jo["errors"].ToString();

                if (data != "[]")
                {
                    string refid = jodata["data"]["ref_id"].ToString();
                    string code = jodata["data"]["code"].ToString();
                    string cardNumber = jodata["data"]["card_pan"].ToString();

                    if (code == "100") // success
                    {

                        userPayment.RefId = refid;
                        userPayment.CardNumber = cardNumber;
                        userPayment.IsPayed = true;
                        userPayment.FkAdsId = ads.Id;
                        userPayment.FkUserId = ads.FkUserId;
                        userPayment.Code = "";
                        userPayment.Description = "";
                        userPayment.Status = 1;
                        userPayment.Amount = setting.AdvertiseFee;


                        ads.IsPayed = true;
                        _context.AdsManagerUW.Update(ads);
                        await _context.saveAsync();


                        _context.UserPaymentManagerUW.Create(userPayment);
                        await _context.saveAsync();


                        // payment is successfully
                        // send messsage to admin

                        #region InitTelegramBotClient
                        if (_bot == null)
                            _bot = new TelegramBotClient(setting == null ? DefaultContents.BotToken : setting.BotToken);
                        #endregion InitTelegramBotClient




                        var adsInfo = $" \r\n {AppUtility.GetCategoryHashTag(ads.Category.Title)}  \r\n \r\n  توضیحات آگهی  :  {ads.Text}  \r\n \r\n  درخواست کننده  :  {ads.User.UserName + "@"} \r\n \r\n آگهی جدید ثبت و پرداخت شده برای بررسی از گزینه های زیر استفاده کنید  \r\n \r\n";



                        await _bot.SendTextMessageAsync(setting.AdminChatId,
                                                             adsInfo,
                                                             replyMarkup: DefaultContents.GenerateAdsPaymentConfirmInlineKeyboard(ads.Id));






                        TempData["telbotus"] = $"https://t.me/{setting.BotUsername}";
                        TempData["refId"] = refid;
                        TempData["message"] = "پرداخت با موفقیت انجام شد";
                        TempData["type"] = "success";
                        TempData["status"] = "success";



                    }


                }
                else if (errors != "[]")
                {
                    TempData["telbotus"] = setting.BotUsername;
                    TempData["type"] = "info";
                    TempData["status"] = "fail";
                    TempData["message"] = "پرداخت  با شکست مواجه شد لطفا دوباره تلاش کنید";

                }


            }
            catch (Exception ex)
            {
                //_logger.Log(ex.ToString(), "PaymentController,VerifyPayment");

                TempData["telbotus"] = setting.BotUsername;
                TempData["type"] = "danger";
                TempData["status"] = "fail";
                TempData["message"] = "خطایی رخ داده است لطفا دوباره تلاش کنید";
            }

            return View();
        }





        /*

        public async Task<IActionResult> PaymenBytHttpClient()
        {

            try
            {

                using (var client = new HttpClient())
                {
                    RequestParameters parameters = new RequestParameters(merchant, amount, description, callbackurl, "", "");

                    var json = JsonConvert.SerializeObject(parameters);

                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(URLs.requestUrl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject jo = JObject.Parse(responseBody);
                    string errorscode = jo["errors"].ToString();

                    JObject jodata = JObject.Parse(responseBody);
                    string dataauth = jodata["data"].ToString();


                    if (dataauth != "[]")
                    {


                        authority = jodata["data"]["authority"].ToString();

                        string gatewayUrl = URLs.gateWayUrl + authority;

                        return Redirect(gatewayUrl);

                    }
                    else
                    {

                        return BadRequest("error " + errorscode);


                    }

                }


            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
            return NotFound();
        }


        public async Task<IActionResult> VerifyByHttpClient()
        {
            try
            {

                VerifyParameters parameters = new VerifyParameters();


                if (HttpContext.Request.Query["Authority"] != "")
                {
                    authority = HttpContext.Request.Query["Authority"];
                }

                parameters.authority = authority;

                parameters.amount = amount;

                parameters.merchant_id = merchant;


                using (HttpClient client = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(parameters);

                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(URLs.verifyUrl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject jodata = JObject.Parse(responseBody);

                    string data = jodata["data"].ToString();

                    JObject jo = JObject.Parse(responseBody);

                    string errors = jo["errors"].ToString();

                    if (data != "[]")
                    {
                        string refid = jodata["data"]["ref_id"].ToString();

                        ViewBag.code = refid;

                        return View();
                    }
                    else if (errors != "[]")
                    {

                        string errorscode = jo["errors"]["code"].ToString();

                        return BadRequest($"error code {errorscode}");

                    }
                }



            }
            catch (Exception ex)
            {

                //throw ex;
            }
            return NotFound();
        }


        */



    }






}
